using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CrazyCoder.Models;

/// <summary>ICO 图标文件辅助类</summary>
public class IconHelper
{
    #region 静态方法
    /// <summary>转换源图片到 ICO 文件流</summary>
    /// <param name="bmp">源图片</param>
    /// <param name="des">目标流</param>
    /// <param name="sizes">尺寸数组</param>
    /// <param name="bits">位深数组</param>
    public static void Convert(Image bmp, Stream des, Int32[] sizes, Int32[] bits)
    {
        var ico = new IconHelper();
        foreach (var bit in bits)
        {
            foreach (var item in sizes)
            {
                if (bit == 8)
                    ico.AddBmp(bmp, item, bit);
                else
                    ico.AddPng(bmp, item, bit);
            }
        }
        ico.Sort();
        ico.Save(des);
    }

    /// <summary>转换源图片到 ICO 文件</summary>
    /// <param name="srcFile">源图片路径</param>
    /// <param name="desFile">目标 ICO 文件路径</param>
    /// <param name="sizes">尺寸数组</param>
    /// <param name="bits">位深数组</param>
    public static void Convert(String srcFile, String desFile, Int32[] sizes, Int32[] bits)
    {
        using var bmp = new Bitmap(srcFile);
        using var fs = File.Create(desFile);
        Convert(bmp, fs, sizes, bits);
    }
    #endregion

    #region 属性
    private readonly List<IconItem> _items = [];
    #endregion

    #region 方法
    private void AddPng(Image bmp, Int32 size, Int32 bit)
    {
        var ico = new Bitmap(bmp, size, size);

        var ms = new MemoryStream();
        ico.Save(ms, ImageFormat.Png);

        var item = new IconItem
        {
            Data = ms.ToArray(),
            Size = (UInt32)ms.Length,
            BitCount = (UInt16)bit,
            Width = (Byte)(size >= 256 ? 0 : size),
            Height = (Byte)(size >= 256 ? 0 : size)
        };

        _items.Add(item);
        ResetOffset();
    }

    private void AddBmp(Image bmp, Int32 size, Int32 bit)
    {
        var ico = new Bitmap(bmp, size, size);

        var ms = new MemoryStream();
        ico.Save(ms, ImageFormat.Bmp);

        var item = new IconItem();
        // BMP 跳过 14 字节文件头
        ms.Position = 14;
        item.Data = new Byte[ms.Length - 14];
        ms.Read(item.Data, 0, item.Data.Length);
        item.Size = (UInt32)item.Data.Length;
        item.BitCount = (UInt16)bit;
        // ICO 的高是 BMP 的 2 倍
        var h = BitConverter.ToInt32(item.Data, 8);
        h *= 2;
        Buffer.BlockCopy(BitConverter.GetBytes(h), 0, item.Data, 8, 4);

        item.Width = (Byte)size;
        item.Height = (Byte)size;

        _items.Add(item);
        ResetOffset();
    }

    private void Sort()
    {
        _items.Sort((a, b) =>
        {
            var cmp = (a.Width == 0 ? 256 : a.Width).CompareTo(b.Width == 0 ? 256 : b.Width);
            if (cmp != 0) return -cmp;
            return -a.BitCount.CompareTo(b.BitCount);
        });

        ResetOffset();
    }

    private void Save(Stream stream)
    {
        var writer = new BinaryWriter(stream);
        writer.Write((UInt16)0); // Reserved
        writer.Write((UInt16)1); // ICO = 1
        writer.Write((UInt16)_items.Count);

        foreach (var item in _items)
        {
            item.Save(writer);
        }
        foreach (var item in _items)
        {
            writer.Write(item.Data);
        }
    }

    private void ResetOffset()
    {
        var idx = (UInt32)(6 + _items.Count * 16);
        foreach (var item in _items)
        {
            item.Offset = idx;
            idx += item.Size;
        }
    }
    #endregion

    #region 内部类
    private class IconItem
    {
        public Byte Width { get; set; } = 16;
        public Byte Height { get; set; } = 16;
        public Byte ColorCount { get; set; }
        public Byte Reserved { get; set; }
        public UInt16 Planes { get; set; } = 1;
        public UInt16 BitCount { get; set; } = 32;
        public UInt32 Size { get; set; }
        public UInt32 Offset { get; set; }
        public Byte[] Data { get; set; } = [];

        public void Save(BinaryWriter writer)
        {
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(ColorCount);
            writer.Write(Reserved);
            writer.Write(Planes);
            writer.Write(BitCount);
            writer.Write(Size);
            writer.Write(Offset);
        }
    }
    #endregion
}
