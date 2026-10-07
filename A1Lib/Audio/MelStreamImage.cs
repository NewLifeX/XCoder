using Microsoft.VisualBasic;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.Audio
{
    public class MelStreamImage
    {
        /// <summary>ProcessAsDecibels返回值</summary>
        public CircularQueue<double[]> Queue { get; private set; } = null;

        /// <summary>帧数</summary>
        public int FrameSize { get; private set; } = 30;

        private int _ColorMode = 0;
        /// <summary>
        /// MEL图颜色模式，0热力图，1灰度图
        /// </summary>
        public int ColorMode
        {
            get { return _ColorMode; }
            set
            {
                _ColorMode = value;
                if (value < 0) _ColorMode = 0;
                if (value > 1) _ColorMode = 1;
            }
        }

        /// <summary>mel流动图</summary>
        /// <param name="frameSize"></param>
        public MelStreamImage(int frameSize = 30)
        {
            this.FrameSize = frameSize;
            Queue = new CircularQueue<double[]>(frameSize);
        }

        /// <summary>加入数据</summary>
        /// <param name="dbs"></param>
        public void PushDbs(double[][] dbs)
        {
            if (Queue == null)
            {
                Queue = new CircularQueue<double[]>(FrameSize);
            }

            // 一次塞满队列，则清空队列并重新写入
            if (FrameSize <= dbs.Length)
            {
                dbs = dbs.TakeLast(FrameSize).ToArray();
                Queue.Clear();
                Queue.WriteArray(dbs);
                return;
            }

            var remain = Queue.Remain();
            if (remain < dbs.Length)
            {
                Queue.Seek(dbs.Length - remain);
            }
            Queue.WriteArray(dbs);
        }

        /// <summary>清理数据</summary>
        public void Clear()
        {
            Queue.Clear();
            Normalization_Min = 10000;
            Normalization_Max = -10000;
        }

        /// <summary>获取图像</summary>
        /// <param name="channelName"></param>
        /// <param name="hasTitle"></param>
        /// <returns></returns>
        public Bitmap GetImage(string channelName, bool hasTitle = false)
        {
            if (Queue == null) return null;
            if (Queue.Count() < 1) return null;
            if (Queue.Count() < FrameSize)
            {
                var dbs = new double[FrameSize][];
                var has = Queue.ToArray();
                for (int i = 0; i < FrameSize - has.Length; i++)
                {
                    dbs[i] = new double[has[0].Length];
                }
                for (int i = 0; i < has.Length; i++)
                {
                    dbs[FrameSize - has.Length + i] = has[i];
                }

                return CreateSpectrogramImage(dbs, channelName, hasTitle);
            }
            else
            {
                var dbs = Queue.ToArray();
                return CreateSpectrogramImage(dbs, channelName, hasTitle);
            }
        }

        /// <summary>归一化最小</summary>
        public double Normalization_Min = 10000;
        /// <summary>归一化最大</summary>
        public double Normalization_Max = -10000;

        /// <summary>创建图像</summary>
        /// <param name="melSpectrogramDb"></param>
        /// <param name="channelName"></param>
        /// <param name="hasTitle"></param>
        /// <returns></returns>
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

            if (max != default && max > Normalization_Max) Normalization_Max = max;
            if (min != default && min < Normalization_Min) Normalization_Min = min;

            if (max < Normalization_Max) max = Normalization_Max;
            if (min > Normalization_Min) min = Normalization_Min;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // 归一化到0-1范围
                    double normalized = (melSpectrogramDb[x][y] - min) / (max - min);

                    // 使用热图颜色方案
                    Color color = Color.Black;
                    if (ColorMode == 0) color = HeatMapColor(normalized);
                    if (ColorMode == 1) color = GrayScale(normalized);

                    // 注意: 图像坐标系y轴向下，频谱y轴向上，所以需要翻转
                    // 并且向下偏移titleHeight像素
                    image.SetPixel(x, height - 1 - y + titleHeight, color);
                }
            }

            return image;
        }

        /// <summary>热力图颜色</summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>灰度颜色</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Color GrayScale(double value)
        {
            // 灰度颜色方案：黑色(0) -> 白色(1)
            int gray = (int)(255 * value);
            return Color.FromArgb(gray, gray, gray);
        }
    }
}
