using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;

namespace XCoder
{
    public class MultiChannelAudioProcessor
    {
        // 梅尔频率转换参数
        public int SampleRate = 44100;     // 采样率
        public int FftSize = 2048;         // FFT窗口大小
        public int HopSize = 512;          // 帧移
        public int MelBands = 80;          // 梅尔带数量
        public float MinFrequency = 100f;  // 最小频率(Hz)
        public float MaxFrequency = 8000f; // 最大频率(Hz)

        /// <summary>多声道梅尔频谱列表(值)</summary>
        public List<double[][]> MulMelSpectrum;
        /// <summary>多声道音量曲线列表(db)</summary>
        public List<double[]> MulVolumeCurve_Db;

        public MultiChannelAudioProcessor(string audioFilePath)
        {
            // 1. 读取音频文件(多声道)
            float[][] audioChannels = ReadAudioFile(audioFilePath);
            var spectrograms = new List<Bitmap>();
            // 初始化多声道梅尔频谱列表
            MulMelSpectrum = new List<double[][]>(audioChannels.Length);

            // 为每个声道生成频谱图
            for (int channel = 0; channel < audioChannels.Length; channel++)
            {
                float[] audioData = audioChannels[channel];

                // 2. 预处理音频(加窗)
                int frames = (audioData.Length - FftSize) / HopSize + 1;
                Complex[][] framesComplex = new Complex[frames][];

                for (int i = 0; i < frames; i++)
                {
                    // 提取帧
                    float[] frame = new float[FftSize];
                    Array.Copy(audioData, i * HopSize, frame, 0, FftSize);

                    // 应用汉宁窗
                    var window = Window.Hann(FftSize);
                    for (int j = 0; j < FftSize; j++)
                    {
                        frame[j] *= (float)window[j];
                    }

                    // 转换为复数
                    framesComplex[i] = frame.Select(x => new Complex(x, 0)).ToArray();
                }

                // 3. 计算FFT和功率谱
                double[][] powerSpectrum = new double[frames][];
                int spectrumSize = FftSize / 2 + 1;
                for (int i = 0; i < frames; i++)
                {
                    // 执行FFT
                    Fourier.Forward(framesComplex[i], FourierOptions.NoScaling);

                    // 计算功率谱(只取前半部分)
                    powerSpectrum[i] = new double[spectrumSize];
                    for (int j = 0; j < spectrumSize; j++)
                    {
                        powerSpectrum[i][j] = framesComplex[i][j].MagnitudeSquared();
                    }
                }

                // 4. 创建梅尔滤波器组
                double[][] melFilterBank = CreateMelFilterBank();

                // 5. 应用梅尔滤波器组
                double[][] melSpectrum = new double[frames][];
                for (int i = 0; i < frames; i++)
                {
                    melSpectrum[i] = new double[MelBands];
                    for (int j = 0; j < MelBands; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < spectrumSize; k++)
                        {
                            sum += powerSpectrum[i][k] * melFilterBank[j][k];
                        }
                        melSpectrum[i][j] = sum;
                    }
                }

                MulMelSpectrum.Add(melSpectrum);
            }


            // 初始化多声道音量曲线列表
            MulVolumeCurve_Db = new List<double[]>(audioChannels.Length);
            // 为每个声道生成音量图
            for (int channel = 0; channel < audioChannels.Length; channel++)
            {
                // 6. 转换为分贝每帧的音量
                var melSpectrum = MulMelSpectrum[channel];
                var volumeCurve = new double[melSpectrum.Length];
                for (int i = 0; i < melSpectrum.Length; i++)
                {
                    // 计算每帧的音量。 直接拿梅尔频谱的最大值作为音量
                    volumeCurve[i] = melSpectrum[i].Max();
                }

                for (int i = 0; i < volumeCurve.Length; i++)
                {
                    if (volumeCurve[i] > 0)
                    {
                        // 转换为分贝
                        volumeCurve[i] = 10 * Math.Log10(volumeCurve[i] + double.Epsilon);
                        // 如果计算结果为NaN，设置为-100dB
                        // if (volumeCurve[i] == double.NaN) volumeCurve[i] = -100.0;

                        // 限制分贝值
                        volumeCurve[i] = Math.Max(volumeCurve[i], -100.0);
                        volumeCurve[i] = Math.Min(volumeCurve[i], 120.0);
                    }
                    else
                    {
                        // 如果音量为0，设置为-100dB
                        volumeCurve[i] = -100.0;
                    }
                }

                MulVolumeCurve_Db.Add(volumeCurve);
            }
        }

        /// <summary>生成mel频谱图</summary>
        /// <param name="hasTitle">启用图像抬头</param>
        /// <returns></returns>
        public List<Bitmap> GenerateMelSpectrograms(bool hasTitle = false)
        {
            var spectrograms = new List<Bitmap>();

            // 为每个声道生成频谱图
            for (int channel = 0; channel < MulMelSpectrum.Count; channel++)
            {
                // 6. 转换为分贝单位
                double[][] melSpectrogramDb = ConvertToDecibels(MulMelSpectrum[channel]);

                // 7. 创建图像
                Bitmap spectrogramImage = CreateSpectrogramImage(melSpectrogramDb, $"Channel {channel + 1}", hasTitle);

                spectrograms.Add(spectrogramImage);
            }

            return spectrograms;
        }

        /// <summary>生成音量曲线图</summary>
        /// <param name="hasTitle">启用抬头</param>
        /// <returns></returns>
        public List<Bitmap> GenerateVolumeCurve(bool hasTitle = false)
        {
            var spectrograms = new List<Bitmap>();

            // 为每个声道生成音量图
            for (int channel = 0; channel < MulVolumeCurve_Db.Count; channel++)
            {
                // 7. 创建图像
                Bitmap spectrogramImage = CreateVolumeCurveImage(MulVolumeCurve_Db[channel], $"Channel {channel + 1}");

                spectrograms.Add(spectrogramImage);
            }

            return spectrograms;
        }

        /// <summary>生成音量曲线图，多通道在一张图内</summary>
        /// <returns></returns>
        public Bitmap GenerateVolumeCurveOverlay()
        {
            int width = MulVolumeCurve_Db[0].Length;
            int height = 200 * MulVolumeCurve_Db.Count;

            var hasTitle = false;

            // 增加高度以容纳标题
            int titleHeight = 30;
            if (!hasTitle) titleHeight = 0;
            Bitmap image = new Bitmap(width, height + titleHeight, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(image))
            {
                // 绘制黑色背景
                g.Clear(Color.LightGray);

                /*
                // 绘制标题
                if (hasTitle)
                {
                    using (Font font = new Font("Arial", 12))
                    using (Brush brush = new SolidBrush(Color.White))
                    {
                        g.DrawString(channelName, font, brush, 10, 5);
                    }
                }
                */
            }

            double min = -100.5;
            double max = 120.5;

            var cls = new Color[]
            {
                Color.Red,
                Color.Green,
                Color.Yellow,
                Color.Orange,
                Color.Blue,
                Color.Cyan,
                Color.Magenta,
            };

            for (int ch = 0; ch < MulVolumeCurve_Db.Count; ch++)
            {
                // 每个声道的音量曲线
                var volumes = MulVolumeCurve_Db[ch];
                var color = cls[ch % cls.Length];

                for (int x = 0; x < width; x++)
                {
                    int y = (int)((volumes[x] - min) / (max - min) * height);

                    // 注意: 图像坐标系y轴向下，频谱y轴向上，所以需要翻转
                    // 并且向下偏移titleHeight像素
                    image.SetPixel(x, height - 1 - y + titleHeight, color);
                }
            }

            return image;
        }


        private float[][] ReadAudioFile(string filePath)
        {
            using (var audioFile = new AudioFileReader(filePath))
            {
                // 获取参数
                int channelCount = audioFile.WaveFormat.Channels;
                int sampleRate = audioFile.WaveFormat.SampleRate;
                SampleRate = sampleRate;

                // 为每个声道创建缓冲区
                float[][] channelBuffers = new float[channelCount][];
                for (int i = 0; i < channelCount; i++)
                {
                    channelBuffers[i] = new float[audioFile.Length / (4 * channelCount)]; // 每个float占4字节
                }

                // 临时缓冲区用于读取
                float[] interleavedBuffer = new float[audioFile.Length / 4];
                int samplesRead = audioFile.Read(interleavedBuffer, 0, interleavedBuffer.Length);

                // 解交错音频数据(多声道交错存储)
                for (int i = 0; i < samplesRead; i++)
                {
                    int channel = i % channelCount;
                    int position = i / channelCount;
                    if (position < channelBuffers[channel].Length)
                    {
                        channelBuffers[channel][position] = interleavedBuffer[i];
                    }
                }

                return channelBuffers;
            }
        }

        private double[][] CreateMelFilterBank()
        {
            double[][] filterBank = new double[MelBands][];
            int spectrumSize = FftSize / 2 + 1;

            // 计算梅尔频率范围
            double minMel = HzToMel(MinFrequency);
            double maxMel = HzToMel(MaxFrequency);

            // 创建梅尔点
            double[] melPoints = new double[MelBands + 2];
            for (int i = 0; i < MelBands + 2; i++)
            {
                melPoints[i] = minMel + i * (maxMel - minMel) / (MelBands + 1);
            }

            // 将梅尔点转换回Hz
            double[] hzPoints = melPoints.Select(MelToHz).ToArray();

            // 将Hz点转换为FFT bin索引
            int[] binIndices = hzPoints.Select(hz => (int)Math.Floor((FftSize + 1) * hz / SampleRate)).ToArray();

            // 创建三角形滤波器
            for (int i = 0; i < MelBands; i++)
            {
                filterBank[i] = new double[spectrumSize];
                int start = binIndices[i];
                int peak = binIndices[i + 1];
                int end = binIndices[i + 2];

                // 上升斜坡
                for (int j = start; j <= peak; j++)
                {
                    if (j >= 0 && j < spectrumSize)
                    {
                        filterBank[i][j] = (j - start) / (double)(peak - start);
                    }
                }

                // 下降斜坡
                for (int j = peak; j <= end; j++)
                {
                    if (j >= 0 && j < spectrumSize)
                    {
                        filterBank[i][j] = 1 - (j - peak) / (double)(end - peak);
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

        private static double[][] ConvertToDecibels(double[][] melSpectrum)
        {
            double minDb = -100; // 最小分贝值

            for (int i = 0; i < melSpectrum.Length; i++)
            {
                for (int j = 0; j < melSpectrum[i].Length; j++)
                {
                    // 计算分贝值，并限制最小值
                    double db = 10 * Math.Log10(melSpectrum[i][j] + double.Epsilon);
                    melSpectrum[i][j] = Math.Max(db, minDb);
                }
            }

            return melSpectrum;
        }

        private Bitmap CreateSpectrogramImage(double[][] melSpectrogramDb, string channelName, bool hasTitle = false)
        {
            int width = melSpectrogramDb.Length;
            int height = melSpectrogramDb[0].Length;

            // 增加高度以容纳标题
            int titleHeight = 30;
            if (!hasTitle) titleHeight = 0;
            Bitmap image = new Bitmap(width, height + titleHeight, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(image))
            {
                // 绘制黑色背景
                g.Clear(Color.Black);

                // 绘制标题
                if (hasTitle)
                {
                    using (Font font = new Font("Arial", 12))
                    using (Brush brush = new SolidBrush(Color.White))
                    {
                        g.DrawString(channelName, font, brush, 10, 5);
                    }
                }
            }

            // 找到最小和最大值用于归一化
            double min = melSpectrogramDb.Min(frame => frame.Min());
            double max = melSpectrogramDb.Max(frame => frame.Max());

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // 归一化到0-1范围
                    double normalized = (melSpectrogramDb[x][y] - min) / (max - min);

                    // 使用热图颜色方案
                    Color color = HeatMapColor(normalized);

                    // 注意: 图像坐标系y轴向下，频谱y轴向上，所以需要翻转
                    // 并且向下偏移titleHeight像素
                    image.SetPixel(x, height - 1 - y + titleHeight, color);
                }
            }

            return image;
        }

        private Bitmap CreateVolumeCurveImage(double[] volumes, string channelName, bool hasTitle = false)
        {
            int width = volumes.Length;
            int height = 200;

            // 增加高度以容纳标题
            int titleHeight = 30;
            if (!hasTitle) titleHeight = 0;
            Bitmap image = new Bitmap(width, height + titleHeight, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(image))
            {
                // 绘制黑色背景
                g.Clear(Color.LightGray);

                // 绘制标题
                if (hasTitle)
                {
                    using (Font font = new Font("Arial", 12))
                    using (Brush brush = new SolidBrush(Color.White))
                    {
                        g.DrawString(channelName, font, brush, 10, 5);
                    }
                }
            }

            double min = -100.5;
            double max = 120.5;

            for (int x = 0; x < width; x++)
            {
                int y = (int)((volumes[x] - min) / (max - min) * height);

                // 注意: 图像坐标系y轴向下，频谱y轴向上，所以需要翻转
                // 并且向下偏移titleHeight像素
                image.SetPixel(x, height - 1 - y + titleHeight, Color.Red);
            }

            return image;
        }


        private static Color HeatMapColor(double value)
        {
            // 简单的热图颜色方案
            // 蓝色(冷) -> 青色 -> 绿色 -> 黄色 -> 红色(热)

            int r, g, b;

            if (value < 0.25)
            {
                // 蓝色到青色
                r = 0;
                g = (int)(4 * 255 * value);
                b = 255;
            }
            else if (value < 0.5)
            {
                // 青色到绿色
                r = 0;
                g = 255;
                b = (int)(255 - 4 * 255 * (value - 0.25));
            }
            else if (value < 0.75)
            {
                // 绿色到黄色
                r = (int)(4 * 255 * (value - 0.5));
                g = 255;
                b = 0;
            }
            else
            {
                // 黄色到红色
                r = 255;
                g = (int)(255 - 4 * 255 * (value - 0.75));
                b = 0;
            }

            return Color.FromArgb(r, g, b);
        }
    }
}
