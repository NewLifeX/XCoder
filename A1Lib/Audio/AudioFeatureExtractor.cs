using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace ALib.Audio
{
    /// <summary>
    /// 音频特征提取器，处理单声道音频数据
    /// </summary>
    public class AudioFeatureExtractor : IDisposable
    {
        #region 参数 修改后调用一下 Initialize，确保参数正确

        /// <summary>采样率 (默认48000Hz)</summary>
        public int SampleRate { get; set; } = 48000;

        /// <summary>FFT窗口大小 (默认2048，必须为2的幂)</summary>
        public int FftSize { get; set; } = 2048;

        /// <summary>每秒输出的Mel点数 (默认30)</summary>
        public int MelPointsPerSecond { get; set; } = 30;

        /// <summary>梅尔带数量 (默认80)</summary>
        public int MelBands { get; set; } = 80;

        /// <summary>最小频率Hz (默认100)</summary>
        public float MinFrequency { get; set; } = 200f;

        /// <summary>最大频率Hz (默认8000)</summary>
        public float MaxFrequency { get; set; } = 13500f;

        /// <summary>帧移 (根据采样率和MelPointsPerSecond自动计算)</summary>
        public int HopSize => SampleRate / MelPointsPerSecond;

        #endregion

        #region 输出结果

        /// <summary>Mel频谱 [帧数, MelBands]</summary>
        public double[][] MelSpectrum { get; private set; }

        /// <summary>音量曲线(dB) [帧数]</summary>
        public double[] VolumeCurveDb { get; private set; }

        #endregion

        #region 私有字段


        /// <summary>预计算的Mel滤波器组</summary>
        private double[][] _melFilterBank;

        /// <summary>_melFilterBank对应的频点mel数</summary>
        private double[] _melPoints;

        /// <summary>_melFilterBank对应的频点hz数</summary>
        private double[] _hzPoints;

        /// <summary>预计算的汉宁窗</summary>
        private double[] _window;

        /// <summary>音频数据缓冲区（固定大小 = FftSize）</summary>
        private float[] _buffer;

        /// <summary>缓冲区中的有效数据长度（0 到 FftSize-1）</summary>
        private int _bufferLength = 0;

        #endregion

        #region 构造

        /// <summary>
        /// 创建音频特征提取器实例
        /// </summary>
        public AudioFeatureExtractor()
        {
            Initialize();
        }

        /// <summary>
        /// 创建音频特征提取器实例，指定采样率
        /// </summary>
        /// <param name="sampleRate">音频采样率</param>
        /// <param name="melPoint">每秒Mel点数</param>
        public AudioFeatureExtractor(int sampleRate, int melPoint = 30)
        {
            SampleRate = sampleRate;
            MelPointsPerSecond = melPoint;
            Initialize();
        }

        /// <summary>
        /// 创建音频特征提取器实例，指定音频格式
        /// </summary>
        public AudioFeatureExtractor(WaveFormat format, int melPoint = 30)
        {
            SampleRate = format.SampleRate;
            MelPointsPerSecond = melPoint;
            Initialize();
        }

        public void Dispose() { }

        #endregion

        #region 私有方法

        public void Initialize()
        {
            // 初始化固定大小缓冲区
            _buffer = new float[FftSize];
            _bufferLength = 0;

            // 预计算汉宁窗
            _window = Window.Hann(FftSize);

            // 预计算Mel滤波器组
            _melFilterBank = CreateMelFilterBank();
        }

        #endregion

        #region 核心处理

        /// <summary>
        /// 处理单声道音频数据，生成Mel频谱（使用缓冲区+内存池，支持持续输入）
        /// </summary>
        /// <param name="audioData">单声道音频数据 float[]，取值范围[-1, 1]</param>
        /// <returns>Mel频谱 [帧数, MelBands]</returns>
        private double[][] Process(float[] audioData)
        {
            int newDataLength = audioData?.Length ?? 0;
            int totalLength = _bufferLength + newDataLength;

            // 数据不足，追加到缓冲区并返回空结果
            if (totalLength < FftSize)
            {
                if (newDataLength > 0)
                {
                    Array.Copy(audioData, 0, _buffer, _bufferLength, newDataLength);
                    _bufferLength += newDataLength;
                }
                MelSpectrum = new double[0][];
                VolumeCurveDb = new double[0];
                return MelSpectrum;
            }

            // 缓冲区没有数据
            if (_bufferLength == 0)
            {
                // 执行处理
                ProcessInternal(audioData, totalLength);
                return MelSpectrum;
            }

            // 需要结合缓冲区和新数据
            // 从内存池租借临时缓冲区
            float[] pooledBuffer = ArrayPool<float>.Shared.Rent(totalLength);
            try
            {
                // 复制缓冲区数据
                if (_bufferLength > 0)
                {
                    Array.Copy(_buffer, 0, pooledBuffer, 0, _bufferLength);
                }
                // 追加新数据
                if (newDataLength > 0)
                {
                    Array.Copy(audioData, 0, pooledBuffer, _bufferLength, newDataLength);
                }

                // 执行处理
                ProcessInternal(pooledBuffer, totalLength);
                return MelSpectrum;
            }
            finally
            {
                // 归还内存池
                ArrayPool<float>.Shared.Return(pooledBuffer);
            }
        }

        /// <summary>
        /// 内部处理方法
        /// </summary>
        private void ProcessInternal(float[] data, int dataLength)
        {
            // 1. 计算帧数
            int frames = (dataLength - FftSize) / HopSize + 1;

            // 计算已处理的数据长度
            int processedLength = (frames - 1) * HopSize + FftSize;

            // 2. 预处理音频(分帧、加窗)
            Complex[][] framesComplex = new Complex[frames][];
            for (int i = 0; i < frames; i++)
            {
                // 应用汉宁窗并转换为复数
                framesComplex[i] = new Complex[FftSize];
                for (int j = 0; j < FftSize; j++)
                {
                    framesComplex[i][j] = new Complex(data[i * HopSize + j] * _window[j], 0);
                }
            }

            // 3. 计算FFT和功率谱
            int spectrumSize = FftSize / 2 + 1;
            double[][] powerSpectrum = new double[frames][];
            for (int i = 0; i < frames; i++)
            {
                Fourier.Forward(framesComplex[i], FourierOptions.NoScaling);

                powerSpectrum[i] = new double[spectrumSize];
                for (int j = 0; j < spectrumSize; j++)
                {
                    powerSpectrum[i][j] = framesComplex[i][j].MagnitudeSquared();
                }
            }

            // 4. 应用Mel滤波器组
            MelSpectrum = new double[frames][];
            for (int i = 0; i < frames; i++)
            {
                MelSpectrum[i] = new double[MelBands];
                for (int j = 0; j < MelBands; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < spectrumSize; k++)
                    {
                        sum += powerSpectrum[i][k] * _melFilterBank[j][k];
                    }
                    MelSpectrum[i][j] = sum;
                }
            }

            // 5. 计算音量曲线(dB)
            VolumeCurveDb = new double[frames];
            for (int i = 0; i < frames; i++)
            {
                double rms = Math.Sqrt(MelSpectrum[i].Sum(x => x * x) / MelSpectrum[i].Length);
                if (rms > 0)
                {
                    VolumeCurveDb[i] = 10 * Math.Log10(rms + double.Epsilon);
                    VolumeCurveDb[i] = Math.Max(VolumeCurveDb[i], -100.0);
                    VolumeCurveDb[i] = Math.Min(VolumeCurveDb[i], 120.0);
                }
                else
                {
                    VolumeCurveDb[i] = -100.0;
                }
            }

            // 6. 更新缓冲区：保留未处理的数据
            int remainingLength = dataLength - processedLength;
            if (remainingLength > 0)
            {
                Array.Copy(data, processedLength, _buffer, 0, remainingLength);
            }
            _bufferLength = remainingLength;
        }

        /// <summary>通过mel获取音量(RMS)</summary>
        /// <returns>音量数组</returns>
        public double[] Volumes()
        {
            var mel = MelSpectrum;
            if (mel == null || mel.Length == 0) return null;

            var vol = new double[mel.Length];
            for (int i = 0; i < vol.Length; i++)
            {
                vol[i] = Math.Sqrt(mel[i].Sum(x => x * x) / mel[i].Length);
            }
            return vol;
        }

        /// <summary>
        /// 处理单声道音频数据，返回Mel频谱(dB单位)
        /// </summary>
        /// <param name="audioData">单声道音频数据 float[]</param>
        /// <returns>Mel频谱(dB) [帧数, MelBands]</returns>
        public double[][] ProcessAsDecibels(float[] audioData)
        {
            Process(audioData);
            if (MelSpectrum == null || MelSpectrum.Length == 0) return MelSpectrum;

            var dbs = new double[MelSpectrum.Length][];

            // 转换为分贝
            for (int i = 0; i < MelSpectrum.Length; i++)
            {
                dbs[i] = new double[MelSpectrum[i].Length];
                for (int j = 0; j < MelSpectrum[i].Length; j++)
                {
                    double db = 10 * Math.Log10(MelSpectrum[i][j] + double.Epsilon);
                    dbs[i][j] = Math.Max(db, -100.0);
                    // dbs[i][j] = Math.Min(db, 120.0);
                }
            }

            return dbs;
        }

        #endregion

        #region 缓冲区管理

        /// <summary>
        /// 清空缓冲区
        /// </summary>
        public void ClearBuffer()
        {
            _bufferLength = 0;
        }

        /// <summary>
        /// 获取缓冲区中当前的数据长度
        /// </summary>
        public int BufferedLength => _bufferLength;

        #endregion

        #region Mel滤波器组

        private double[][] CreateMelFilterBank()
        {
            double[][] filterBank = new double[MelBands][];
            int spectrumSize = FftSize / 2 + 1;

            // 计算Mel频率范围
            double minMel = HzToMel(MinFrequency);
            double maxMel = HzToMel(MaxFrequency);

            // 创建Mel点
            double[] melPoints = new double[MelBands + 2];
            for (int i = 0; i < MelBands + 2; i++)
            {
                melPoints[i] = minMel + i * (maxMel - minMel) / (MelBands + 1);
            }
            _melPoints = melPoints;

            // 将Mel点转换回Hz
            double[] hzPoints = melPoints.Select(MelToHz).ToArray();
            _hzPoints = hzPoints;

            // 将Hz点转换为FFT bin索引
            int[] binIndices = hzPoints.Select(hz => (int)Math.Floor((FftSize + 1) * hz / SampleRate)).ToArray();

            // 创建三角形滤波器
            for (int i = 0; i < MelBands; i++)
            {
                filterBank[i] = new double[spectrumSize];
                int start = binIndices[i];
                int peak = binIndices[i + 1];
                int end = binIndices[i + 2];

                // 防止除零
                if (peak == start) peak = start + 1;
                if (end == peak) end = peak + 1;

                // 上升斜坡
                for (int j = start; j <= peak && j < spectrumSize; j++)
                {
                    if (j >= 0)
                    {
                        filterBank[i][j] = (double)(j - start) / (peak - start);
                    }
                }

                // 下降斜坡
                for (int j = peak; j <= end && j < spectrumSize; j++)
                {
                    if (j >= 0)
                    {
                        filterBank[i][j] = 1.0 - (double)(j - peak) / (end - peak);
                    }
                }
            }

            return filterBank;
        }

        private static double HzToMel(double hz)
        {
            return 2595 * Math.Log10(1 + hz / 700.0);
        }

        private static double MelToHz(double mel)
        {
            return 700 * (Math.Pow(10, mel / 2595.0) - 1);
        }

        #endregion

        #region 参数更新

        /// <summary>
        /// 更新采样率，需要重新初始化滤波器组
        /// </summary>
        /// <param name="sampleRate">新的采样率</param>
        public void UpdateSampleRate(int sampleRate)
        {
            if (SampleRate != sampleRate)
            {
                SampleRate = sampleRate;
                _melFilterBank = CreateMelFilterBank();
            }
        }

        /// <summary>
        /// 更新FFT大小，需要重新初始化窗函数和滤波器组
        /// </summary>
        /// <param name="fftSize">新的FFT大小(必须为2的幂)</param>
        public void UpdateFftSize(int fftSize)
        {
            if (FftSize != fftSize)
            {
                FftSize = fftSize;
                _buffer = new float[FftSize];
                _bufferLength = 0;
                _window = Window.Hann(FftSize);
                _melFilterBank = CreateMelFilterBank();
            }
        }

        /// <summary>
        /// 更新Mel滤波器参数，需要重新计算滤波器组
        /// </summary>
        /// <param name="melBands">Mel带数量</param>
        /// <param name="minFreq">最小频率</param>
        /// <param name="maxFreq">最大频率</param>
        public void UpdateMelParameters(int melBands, float minFreq, float maxFreq)
        {
            MelBands = melBands;
            MinFrequency = minFreq;
            MaxFrequency = maxFreq;
            _melFilterBank = CreateMelFilterBank();
        }

        #endregion

        #region 辅助

        /// <summary>根据Mel带索引获取对应的中心频率</summary>
        /// <param name="melIdx">Mel带索引 (0 ~ MelBands-1)</param>
        /// <returns>对应的中心频率 (Hz)</returns>
        public double GetHzFromMelIdx(int melIdx)
        {
            if (melIdx < 0 || melIdx >= MelBands)
                throw new ArgumentOutOfRangeException(nameof(melIdx), $"Mel索引必须在0到{MelBands - 1}之间");

            return _hzPoints[melIdx];
        }

        /// <summary>根据Mel带索引获取对应的MEL值</summary>
        /// <param name="melIdx">Mel带索引 (0 ~ MelBands-1)</param>
        /// <returns>对应的中心频率 (Hz)</returns>
        public double GetMelFromMelIdx(int melIdx)
        {
            if (melIdx < 0 || melIdx >= MelBands)
                throw new ArgumentOutOfRangeException(nameof(melIdx), $"Mel索引必须在0到{MelBands - 1}之间");

            return _melPoints[melIdx];
        }

        #endregion

        #region 单一频率（扫频）处理

        /// <summary>从mel点拿到中心频点</summary>
        /// <remarks>针对单一频率音频，需要中心频率的音量大于其他频率 20db</remarks>
        /// <param name="dbs">mel点，db数值。 ProcessAsDecibels返回值逐个分析</param>
        /// <param name="diff">db差值。数学 10xlog(10)</param>
        /// <param name="maxInp">外部限定的max值，多点时统一标准</param>
        /// <returns>default没有中心点，MaxValue不是单一频点</returns>
        public double GetHzFromMel(double[] dbs, double diff = 4.0, double maxInp = -1000.0)
        {
            if (dbs == null) return default;
            if (dbs.Length < 8) return default;

            // 找到最大值位置, 中心频点位置
            int maxidx = 0;
            double max = -1000.0;
            for (int i = 0; i < dbs.Length; i++)
            {
                if (dbs[i] > max)
                {
                    max = dbs[i];
                    maxidx = i;
                }
            }

            max = Math.Max(max, maxInp);

            // 阈值
            var thr = max - diff;
            // 上升沿个数（小于thr到大于thr 的上升沿）
            int cnt = 0;
            // 大于thr的个数，用于判断是不是单一频率。
            int upCnt = 0;
            // 一开始就大
            if (dbs[0] > thr) { cnt++; upCnt++; }
            for (int i = 1; i < dbs.Length; i++)
            {
                if (dbs[i] > thr) upCnt++;
                if ((dbs[i - 1] < thr) && (dbs[i] > thr))
                {
                    cnt++;
                }
            }

            if (cnt == 0) return default;
            // 一个上升沿 的情况下
            // 大于判定阈值的频点超过x个，应该是有问题的。
            // 正常来说，3个是正常的。
            if (upCnt > 6) return double.MaxValue;
            if (cnt > 1) return double.MaxValue;

            return _hzPoints[maxidx];
        }

        /// <summary>从mels获取hzs</summary>
        /// <remarks>针对单一频率音频，需要中心频率的音量大于其他频率 diff db</remarks>
        /// <param name="mels">mel图，db数值。ProcessAsDecibels返回值逐个分析</param>
        /// <param name="diff">db差值。数学 10xlog(10)</param>
        /// <param name="maxInp">外部限定的max值，多点时统一标准</param>
        /// <returns></returns>
        public double[] GetHzsFromMels(double[][] dbss, double diff = 4.0)
        {
            if (dbss == null) return null;

            // 10 x log(10)
            // 4db 大概是 2.512 倍
            var max = dbss.Max(fram => fram.Max());
            var hzs = new double[dbss.Length];
            for (int i = 0; i < hzs.Length; i++) hzs[i] = GetHzFromMel(dbss[i], diff, max);

            return hzs;
        }

        /// <summary>单频点图</summary>
        /// <param name="hzs">GetHzsFromMels</param>
        /// <returns></returns>
        public Bitmap GetSingleHzImage(double[] hzs)
        {
            var image = new Bitmap(hzs.Length, MelBands);
            using (Graphics g = Graphics.FromImage(image))
            {
                // 绘制黑色背景
                g.Clear(Color.Black);
            }

            var div = (MaxFrequency - MinFrequency) / MelBands;
            var clr = Color.White;

            for (int i = 0; i < hzs.Length; i++)
            {
                if (hzs[i] < MinFrequency) continue;
                if (hzs[i] > MaxFrequency) continue;

                int x = i;
                var dy = (hzs[i] - MinFrequency) / div;
                int y = MelBands - (int)dy;
                if (y < 0) continue;
                if (y >= MelBands) continue;

                image.SetPixel(x, y, clr);
            }

            for (int i = 0; i < hzs.Length; i++)
            {
                // if (hzs[i] == default) image.SetPixel(i, 0, Color.Green);
                if (hzs[i] == double.MaxValue) image.SetPixel(i, 0, Color.Red);
            }

            return image;
        }

        /// <summary>从mel 获取单频点图</summary>
        /// <param name="diff">db差值。数学 10xlog(10)</param>
        /// <returns></returns>
        public Bitmap GetSingleHzImageFromMels(double[][] dbmels, double diff = 4.0)
        {
            // 10 x log(10)
            // 4db 大概是 2.512 倍
            var hzs = GetHzsFromMels(dbmels, diff);
            return GetSingleHzImage(hzs);
        }

        #endregion
    }
}
