using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib
{
    /// <summary>位图图像</summary>
    public class BitmapImage
    {
        /// <summary>列数，每行个数,建议是8的倍数</summary>
        public int Columns;
        /// <summary>总bit数</summary>
        public int BitCount;

        /// <summary>位图数据</summary>
        public Byte[] Bits;

        /// <summary>位图 图</summary>
        /// <param name="bits"></param>
        /// <param name="bitCount">bit总数</param>
        /// <param name="cols">列数</param>
        public BitmapImage(byte[] bits, int bitCount, int cols = 8)
        {
            if (cols < 1) cols = 8;
            if (bitCount > bits.Length * 8) bitCount = bits.Length * 8;

            Bits = bits;
            BitCount = bitCount;
            Columns = cols;
        }

        /// <summary>位图，图</summary>
        /// <param name="bitCount">bit数</param>
        /// <param name="cols"></param>
        public BitmapImage(int bitCount, int cols = 8)
        {
            if (cols < 1) cols = 8;

            Bits = new Byte[bitCount];
            BitCount = bitCount;
            Columns = cols;
        }

        /// <summary>获取标志</summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool GetBit(int idx)
        {
            idx %= BitCount;

            int byteIdx = idx / 8;
            var temp = Bits[byteIdx];

            int bitIdx = idx % 8;
            int mask = 1 << bitIdx;

            return (mask & temp) > 0;
        }

        /// <summary>设置标志，测试用的</summary>
        /// <param name="idx"></param>
        /// <param name="bit"></param>
        public void SetBit(int idx, bool bit)
        {
            idx %= BitCount;

            int byteIdx = idx / 8;
            var temp = Bits[byteIdx];

            int bitIdx = idx % 8;
            int bitMask = 1 << bitIdx;

            if (bit)
            {
                temp = (byte)(temp | bitMask);
            }
            else
            {
                bitMask = ~bitMask;
                temp = (byte)(temp & bitMask);
            }

            Bits[byteIdx] = temp;
        }

        /// <summary>获取图像</summary>
        /// <returns></returns>
        public Bitmap GetImage()
        {
            int rowCount = (BitCount + Columns - 1) / Columns;
            // 每个标志占 10x10 pix ，计算整体大小。
            var bmap = new Bitmap(Columns * 10, rowCount * 10);
            var g = Graphics.FromImage(bmap);
            g.Clear(Color.LightGray);

            // 每个标志占 10x10 pix, 红色
            for (int i = 0; i < BitCount; i++)
            {
                var brush = Brushes.Red;
                if (!GetBit(i)) brush = Brushes.Green;

                int x = i % Columns * 10;
                int y = i / Columns * 10;
                g.FillRectangle(brush, x, y, 10, 10);
            }

            // 画线分隔图块
            // 竖线
            var pen2 = new Pen(Color.White, 2);
            for (int i = 1; i < Columns; i++)
            {
                int x = i * 10;
                g.DrawLine(pen2, x, 0, x, rowCount * 10);
            }
            // 横线
            for (int i = 1; i < rowCount; i++)
            {
                int y = i * 10;
                g.DrawLine(pen2, 0, y, Columns * 10, y);
            }

            g.Dispose();

            return bmap;
        }
    }
}
