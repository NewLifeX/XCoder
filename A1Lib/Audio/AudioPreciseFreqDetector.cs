using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ALib.Audio
{
    /// <summary>
    /// 精确频率检测器
    /// 对输入音频做FFT，每秒输出多个点；
    /// 每个点在有效频率范围(MinFrequency~MaxFrequency)内扫描所有bin找到功率峰值，
    /// 再用二次插值精确定位频率（精度50Hz），输出频率(Hz)和音量(dB)
    /// </summary>
    public class AudioPreciseFreqDetector : IDisposable
    {
        #region 参数 修改后调用一下 Initialize，确保参数正确

        /// <summary>采样率 (默认48000Hz)</summary>
        public int SampleRate { get; set; } = 48000;

        /// <summary>FFT窗口大小 (默认4096，必须为2的幂)</summary>
        public int FftSize { get; set; } = 4096;

        /// <summary>每秒输出的点数 (默认30)</summary>
        public int PointsPerSecond { get; set; } = 30;

        /// <summary>粗略筛选的bin搜索起始索引（由MinFrequency自动计算）</summary>
        private int _minSearchBin;

        /// <summary>粗略筛选的bin搜索结束索引（由MaxFrequency自动计算）</summary>
        private int _maxSearchBin;

        /// <summary>精确查找时的频率精度Hz (默认50)</summary>
        public float FinePrecisionHz { get; set; } = 50f;

        /// <summary>最小频率Hz</summary>
        public float MinFrequency { get; set; } = 200f;

        /// <summary>最大频率Hz</summary>
        public float MaxFrequency { get; set; } = 13500f;

        /// <summary>帧移 (根据采样率和PointsPerSecond自动计算)</summary>
        public int HopSize => SampleRate / PointsPerSecond;

        #endregion

        #region 输出结果

        /// <summary>每帧检测到的精确频率Hz [帧数]</summary>
        public double[] DetectedFrequencies { get; private set; }

        /// <summary>每帧检测到的音量dB [帧数]</summary>
        public double[] DetectedDecibels { get; private set; }

        /// <summary>每帧粗略筛选的频率段起始Hz [帧数]</summary>
        public double[] CoarseFrequencies { get; private set; }

        /// <summary>每帧的功率谱(dB) [帧数, spectrumSize]，用于画频谱图</summary>
        public double[][] FrameSpectrumDb { get; private set; }

        #endregion

        #region 私有字段

        /// <summary>预计算的汉宁窗</summary>
        private double[] _window;

        /// <summary>音频数据缓冲区</summary>
        private float[] _buffer;

        /// <summary>缓冲区中的有效数据长度</summary>
        private int _bufferLength = 0;

        /// <summary>频谱分辨率 (Hz/bin)，用于粗略筛选</summary>
        private double _binResolution;

        #endregion

        #region 构造

        public AudioPreciseFreqDetector()
        {
            Initialize();
        }

        public AudioPreciseFreqDetector(int sampleRate, int pointsPerSecond = 30)
        {
            SampleRate = sampleRate;
            PointsPerSecond = pointsPerSecond;
            Initialize();
        }

        public AudioPreciseFreqDetector(WaveFormat format, int pointsPerSecond = 30)
        {
            SampleRate = format.SampleRate;
            PointsPerSecond = pointsPerSecond;
            Initialize();
        }

        public void Dispose() { }

        #endregion

        #region 初始化

        public void Initialize()
        {
            _buffer = new float[FftSize];
            _bufferLength = 0;

            // 预计算汉宁窗
            _window = Window.Hann(FftSize);

            // 计算频谱分辨率
            _binResolution = (double)SampleRate / FftSize;

            // 预计算有效频率范围对应的bin搜索区间
            RecalcSearchRange();
        }

        /// <summary>
        /// 根据 MinFrequency / MaxFrequency 预计算有效bin搜索区间
        /// </summary>
        private void RecalcSearchRange()
        {
            int minBin = Math.Max(1, (int)Math.Round(MinFrequency / _binResolution));
            int maxBin = Math.Min(FftSize / 2 - 1, (int)Math.Round(MaxFrequency / _binResolution));
            if (minBin >= maxBin)
            {
                minBin = 1;
                maxBin = FftSize / 2 - 1;
            }
            _minSearchBin = minBin;
            _maxSearchBin = maxBin;
        }

        #endregion

        #region 核心处理

        /// <summary>
        /// 处理单声道音频数据，输出每帧的精确频率和音量
        /// </summary>
        /// <param name="audioData">单声道音频数据 float[]，取值范围[-1, 1]</param>
        /// <returns>检测到的频率Hz数组</returns>
        public double[] Process(float[] audioData)
        {
            int newDataLength = audioData?.Length ?? 0;
            int totalLength = _bufferLength + newDataLength;

            // 数据不足，追加到缓冲区
            if (totalLength < FftSize)
            {
                if (newDataLength > 0)
                {
                    Array.Copy(audioData, 0, _buffer, _bufferLength, newDataLength);
                    _bufferLength += newDataLength;
                }
                DetectedFrequencies = Array.Empty<double>();
                DetectedDecibels = Array.Empty<double>();
                CoarseFrequencies = Array.Empty<double>();
                return DetectedFrequencies;
            }

            if (_bufferLength == 0)
            {
                ProcessInternal(audioData, totalLength);
                return DetectedFrequencies;
            }

            // 需要结合缓冲区和新数据
            float[] pooledBuffer = ArrayPool<float>.Shared.Rent(totalLength);
            try
            {
                if (_bufferLength > 0)
                    Array.Copy(_buffer, 0, pooledBuffer, 0, _bufferLength);
                if (newDataLength > 0)
                    Array.Copy(audioData, 0, pooledBuffer, _bufferLength, newDataLength);

                ProcessInternal(pooledBuffer, totalLength);
                return DetectedFrequencies;
            }
            finally
            {
                ArrayPool<float>.Shared.Return(pooledBuffer);
            }
        }

        private void ProcessInternal(float[] data, int dataLength)
        {
            int spectrumSize = FftSize / 2 + 1;
            int frames = (dataLength - FftSize) / HopSize + 1;
            int processedLength = (frames - 1) * HopSize + FftSize;

            DetectedFrequencies = new double[frames];
            DetectedDecibels = new double[frames];
            CoarseFrequencies = new double[frames];
            FrameSpectrumDb = new double[frames][];

            // 预分配复数数组，复用减少GC
            Complex[] fftBuffer = new Complex[FftSize];

            for (int frame = 0; frame < frames; frame++)
            {
                // 1. 分帧 + 加窗
                int offset = frame * HopSize;
                for (int j = 0; j < FftSize; j++)
                {
                    fftBuffer[j] = new Complex(data[offset + j] * _window[j], 0);
                }

                // 2. FFT
                Fourier.Forward(fftBuffer, FourierOptions.NoScaling);

                // 3. 计算功率谱 (平方幅度)
                double[] powerSpectrum = new double[spectrumSize];
                for (int j = 0; j < spectrumSize; j++)
                {
                    powerSpectrum[j] = fftBuffer[j].MagnitudeSquared();
                }

                // 4. 在有效频率范围内扫描所有bin，找到功率最大的bin（峰值）
                int bestCoarseBin = FindBestCoarseBin(powerSpectrum);
                double coarseHz = bestCoarseBin * _binResolution;
                CoarseFrequencies[frame] = coarseHz;

                // 5. 精确查找：在粗略频率附近，以50Hz精度精确定位
                var (preciseHz, preciseDb) = FindPreciseFrequency(fftBuffer, powerSpectrum, bestCoarseBin);
                DetectedFrequencies[frame] = preciseHz;
                DetectedDecibels[frame] = preciseDb;

                // 6. 保存该帧的dB频谱（用于画图）
                FrameSpectrumDb[frame] = new double[spectrumSize];
                for (int j = 0; j < spectrumSize; j++)
                {
                    double db = 10 * Math.Log10(powerSpectrum[j] + double.Epsilon);
                    FrameSpectrumDb[frame][j] = Math.Max(db, -100.0);
                }
            }

            // 7. 更新缓冲区
            int remainingLength = dataLength - processedLength;
            if (remainingLength > 0)
                Array.Copy(data, processedLength, _buffer, 0, remainingLength);
            _bufferLength = remainingLength;
        }

        /// <summary>
        /// 在有效频率范围内扫描所有bin，找到功率最大的bin（峰值）
        /// </summary>
        private int FindBestCoarseBin(double[] powerSpectrum)
        {
            int bestBin = _minSearchBin;
            double bestPower = powerSpectrum[bestBin];

            for (int bin = _minSearchBin + 1; bin <= _maxSearchBin && bin < powerSpectrum.Length; bin++)
            {
                if (powerSpectrum[bin] > bestPower)
                {
                    bestPower = powerSpectrum[bin];
                    bestBin = bin;
                }
            }

            return bestBin;
        }

        /// <summary>
        /// 在粗略筛选出的频率附近，以FinePrecisionHz精度进行精确频率查找
        /// 使用二次插值法提高频率分辨率
        /// </summary>
        /// <param name="fftBuffer">FFT结果(复数)</param>
        /// <param name="powerSpectrum">功率谱</param>
        /// <param name="coarseBin">粗略筛选出的bin</param>
        /// <returns>(精确频率Hz, 音量dB)</returns>
        private (double freqHz, double db) FindPreciseFrequency(Complex[] fftBuffer, double[] powerSpectrum, int coarseBin)
        {
            int spectrumSize = powerSpectrum.Length;

            // 使用二次抛物线插值提高频率精度
            // 在coarseBin附近取3个点做二次拟合，找到真正的峰值位置
            int leftBin = Math.Max(1, coarseBin - 1);
            int rightBin = Math.Min(spectrumSize - 1, coarseBin + 1);

            double yL = powerSpectrum[leftBin];
            double yC = powerSpectrum[coarseBin];
            double yR = powerSpectrum[rightBin];

            double preciseBin = coarseBin;

            // 二次插值: 峰值偏移 = 0.5 * (yL - yR) / (yL - 2*yC + yR)
            double denominator = yL - 2 * yC + yR;
            if (Math.Abs(denominator) > 1e-15)
            {
                double delta = 0.5 * (yL - yR) / denominator;
                // 限制偏移在合理范围内 [-1, 1]
                delta = Math.Max(-1.0, Math.Min(1.0, delta));
                preciseBin = coarseBin + delta;
            }

            // 将精确bin转换为频率
            double preciseHz = preciseBin * _binResolution;

            // 取附近多个bin的功率平均值作为该频率的音量
            // 精细精度50Hz对应的大致bin数
            int fineBinWidth = Math.Max(1, (int)Math.Round(FinePrecisionHz / _binResolution));
            double powerSum = 0;
            int powerCount = 0;

            int searchStart = Math.Max(0, (int)Math.Floor(preciseBin) - fineBinWidth);
            int searchEnd = Math.Min(spectrumSize - 1, (int)Math.Ceiling(preciseBin) + fineBinWidth);

            for (int j = searchStart; j <= searchEnd; j++)
            {
                powerSum += powerSpectrum[j];
                powerCount++;
            }

            double avgPower = powerSum / powerCount;
            double db = -100.0;
            if (avgPower > 0)
            {
                // 功率谱是MagnitudeSquared，转dB用 10*log10
                db = 10 * Math.Log10(avgPower + double.Epsilon);
                db = Math.Max(db, -100.0);
                db = Math.Min(db, 120.0);
            }

            return (preciseHz, db);
        }

        #endregion

        #region 缓冲区管理

        /// <summary>清空缓冲区</summary>
        public void ClearBuffer()
        {
            _bufferLength = 0;
        }

        /// <summary>缓冲区中当前的数据长度</summary>
        public int BufferedLength => _bufferLength;

        #endregion

        #region 参数更新

        /// <summary>更新采样率，重新初始化</summary>
        public void UpdateSampleRate(int sampleRate)
        {
            if (SampleRate != sampleRate)
            {
                SampleRate = sampleRate;
                _binResolution = (double)SampleRate / FftSize;
                RecalcSearchRange();
            }
        }

        /// <summary>更新FFT大小，重新初始化</summary>
        public void UpdateFftSize(int fftSize)
        {
            if (FftSize != fftSize)
            {
                FftSize = fftSize;
                _buffer = new float[FftSize];
                _bufferLength = 0;
                _window = Window.Hann(FftSize);
                _binResolution = (double)SampleRate / FftSize;
                RecalcSearchRange();
            }
        }

        #endregion

        #region 便捷方法

        /// <summary>
        /// 获取检测结果的简明摘要
        /// </summary>
        public string GetSummary()
        {
            if (DetectedFrequencies == null || DetectedFrequencies.Length == 0)
                return "No data";

            var validFreqs = DetectedFrequencies.Where(f => f > 0).ToArray();
            if (validFreqs.Length == 0)
                return "No valid frequencies detected";

            return $"Frames:{DetectedFrequencies.Length}, " +
                   $"Freq: {validFreqs.Average():F1}Hz, " +
                   $"dB: {DetectedDecibels.Where(d => d > -100).Average():F1}dB";
        }

        #endregion

        #region 频谱图生成

        /// <summary>
        /// 生成精确频率检测的频谱图（无坐标轴标签）
        /// X轴=时间(帧)，Y轴=频率(Hz)，颜色=dB值
        /// 并在图上用亮色十字标记每帧检测到的精确频率位置
        /// </summary>
        /// <param name="channelName">未使用（保留兼容）</param>
        /// <param name="freqMarkerColor">频率标记颜色，null则使用亮绿</param>
        /// <param name="heatmap">true=热力图配色（默认）；false=灰度图（亮=能量强）</param>
        /// <returns>频谱图 Bitmap</returns>
        public Bitmap GetSpectrumImage(string channelName = "", Color? freqMarkerColor = null, bool heatmap = true)
        {
            if (FrameSpectrumDb == null || FrameSpectrumDb.Length == 0)
                return null;

            int frames = FrameSpectrumDb.Length;
            int spectrumSize = FrameSpectrumDb[0].Length;
            var markerColor = freqMarkerColor ?? Color.Lime;

            // 只绘制有效频率范围 [MinFrequency, MaxFrequency] 对应的 bin 范围
            int minBin = Math.Max(0, (int)(MinFrequency / _binResolution));
            int maxBin = Math.Min(spectrumSize - 1, (int)Math.Ceiling(MaxFrequency / _binResolution));
            int dataHeight = maxBin - minBin + 1;
            if (dataHeight <= 0)
                return null;

            // 每帧2像素宽；超长音频会生成数万像素宽的超大图（内存爆炸 + 超出 GDI+ 尺寸限制），
            // 这里封顶最大宽度并按比例降采样，避免绘制/内存开销失控。
            const int MaxWidth = 4000;
            int imgWidth = Math.Max(200, frames * 2);
            double frameToX = 2.0;
            if (imgWidth > MaxWidth)
            {
                imgWidth = MaxWidth;
                frameToX = (double)imgWidth / frames;
            }
            int imgHeight = dataHeight;

            // 预计算 dB 归一化范围
            double minDb = double.MaxValue, maxDb = double.MinValue;
            for (int f = 0; f < frames; f++)
            {
                var row = FrameSpectrumDb[f];
                for (int b = minBin; b <= maxBin; b++)
                {
                    double db = row[b];
                    if (db < minDb) minDb = db;
                    if (db > maxDb) maxDb = db;
                }
            }
            if (maxDb <= minDb) maxDb = minDb + 1;
            double invRange = 1.0 / (maxDb - minDb);

            // 用 LockBits 直接写像素：避免逐像素 new SolidBrush + FillRectangle
            // （在长音频下会产生数千万次 GDI 调用与海量临时对象，是单文件最耗时的步骤）。
            Bitmap image = new Bitmap(imgWidth, imgHeight, PixelFormat.Format32bppArgb);
            BitmapData bd = null;
            try
            {
                bd = image.LockBits(new Rectangle(0, 0, imgWidth, imgHeight),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                int stride = bd.Stride;
                int[] line = new int[imgWidth];
                for (int y = 0; y < imgHeight; y++)
                {
                    // Y轴：顶部=高频，底部=低频
                    int b = minBin + (imgHeight - 1 - y);
                    for (int x = 0; x < imgWidth; x++)
                    {
                        int f = (int)(x / frameToX);
                        if (f >= frames) f = frames - 1;
                        double db = FrameSpectrumDb[f][b];
                        double normalized = (db - minDb) * invRange;
                        normalized = normalized < 0 ? 0 : (normalized > 1 ? 1 : normalized);
                        Color color = heatmap
                            ? HeatMapColor(normalized)
                            : GrayscaleColor(normalized);
                        line[x] = (255 << 24) | (color.R << 16) | (color.G << 8) | color.B;
                    }
                    Marshal.Copy(line, 0, (IntPtr)((long)bd.Scan0 + (long)y * stride), imgWidth);
                }
            }
            finally
            {
                if (bd != null) image.UnlockBits(bd);
            }

            /*
            // 频率十字标记（数量=帧数，相对整图绘制量很小）
            using (Graphics g = Graphics.FromImage(image))
            using (Pen pen = new Pen(markerColor))
            {
                for (int f = 0; f < frames; f++)
                {
                    double freqHz = DetectedFrequencies[f];
                    if (freqHz < MinFrequency || freqHz > MaxFrequency)
                        continue;
                    int freqBin = (int)Math.Round(freqHz / _binResolution);
                    freqBin = Math.Max(minBin, Math.Min(maxBin, freqBin));
                    int markerY = dataHeight - 1 - (freqBin - minBin);
                    int markerX = (int)(f * frameToX) + 1;
                    g.DrawLine(pen, markerX - 2, markerY, markerX + 2, markerY);
                    g.DrawLine(pen, markerX, markerY - 1, markerX, markerY + 1);
                }
            }
            */
            return image;
        }

        /// <summary>
        /// 热力图颜色映射
        /// </summary>
        private static Color HeatMapColor(double value)
        {
            int r, g, b;

            if (value < 0.25)
            {
                r = 0;
                g = (int)(4 * 255 * value);
                b = 255;
            }
            else if (value < 0.5)
            {
                r = 0;
                g = 255;
                b = (int)(255 - 4 * 255 * (value - 0.25));
            }
            else if (value < 0.75)
            {
                r = (int)(4 * 255 * (value - 0.5));
                g = 255;
                b = 0;
            }
            else
            {
                r = 255;
                g = (int)(255 - 4 * 255 * (value - 0.75));
                b = 0;
            }

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// 灰度图颜色映射：归一化值直接映射为灰阶，亮(白)=能量强，暗(黑)=能量弱
        /// </summary>
        private static Color GrayscaleColor(double value)
        {
            int v = (int)Math.Round(value * 255.0);
            v = v < 0 ? 0 : (v > 255 ? 255 : v);
            return Color.FromArgb(v, v, v);
        }

        #endregion
    }
}
