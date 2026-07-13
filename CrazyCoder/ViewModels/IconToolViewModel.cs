#nullable enable
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrazyCoder.Models;
using Microsoft.Win32;
using NewLife;

namespace CrazyCoder.ViewModels;

/// <summary>图标水印处理工具 ViewModel</summary>
public partial class IconToolViewModel : ObservableObject
{
    #region 属性
    /// <summary>源图片路径</summary>
    [ObservableProperty]
    private String _sourceFile = "";

    /// <summary>源图片</summary>
    [ObservableProperty]
    private ImageSource? _sourceImage;

    /// <summary>预览图片</summary>
    [ObservableProperty]
    private ImageSource? _previewImage;

    /// <summary>水印文字</summary>
    [ObservableProperty]
    private String _watermarkText = "";

    /// <summary>字体名称</summary>
    [ObservableProperty]
    private String _fontName = "Arial";

    /// <summary>字体大小</summary>
    [ObservableProperty]
    private Int32 _fontSize = 96;

    /// <summary>水印 X 位置</summary>
    [ObservableProperty]
    private Int32 _watermarkX;

    /// <summary>水印 Y 位置</summary>
    [ObservableProperty]
    private Int32 _watermarkY;

    /// <summary>水印颜色</summary>
    [ObservableProperty]
    private String _watermarkColor = "#FF0000";

    /// <summary>可用字体列表</summary>
    public ObservableCollection<String> FontNames { get; } = [];

    /// <summary>ICO 大小选项</summary>
    public ObservableCollection<IconSizeOption> IconSizes { get; } =
    [
        new() { Name = "16x16", Size = 16 },
        new() { Name = "24x24", Size = 24 },
        new() { Name = "32x32", Size = 32, IsSelected = true },
        new() { Name = "48x48", Size = 48, IsSelected = true },
        new() { Name = "64x64", Size = 64 },
        new() { Name = "96x96", Size = 96 },
        new() { Name = "128x128", Size = 128 },
        new() { Name = "256x256", Size = 256 },
    ];

    private Bitmap? _originalBitmap;
    #endregion

    #region 构造
    /// <summary>实例化图标水印处理工具 ViewModel</summary>
    public IconToolViewModel()
    {
        foreach (var ft in System.Drawing.FontFamily.Families)
        {
            FontNames.Add(ft.Name);
        }
    }
    #endregion

    #region 命令
    /// <summary>加载图片</summary>
    [RelayCommand]
    private void LoadImage()
    {
        var dlg = new OpenFileDialog
        {
            Filter = "图片文件|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.ico|所有文件|*.*",
            Title = "选择图片"
        };
        if (dlg.ShowDialog() != true) return;

        LoadFromFile(dlg.FileName);
    }

    /// <summary>从文件加载图片</summary>
    public void LoadFromFile(String fileName)
    {
        SourceFile = fileName;

        _originalBitmap?.Dispose();
        _originalBitmap = new Bitmap(fileName);

        SourceImage = BitmapToImageSource(_originalBitmap);

        // 自动设置水印位置到右下角
        WatermarkX = _originalBitmap.Width - 150;
        WatermarkY = _originalBitmap.Height - 50;

        MakeWater();
    }

    /// <summary>应用水印</summary>
    [RelayCommand]
    private void MakeWater()
    {
        if (_originalBitmap == null) return;

        var bmp = MakeWatermarkImage();
        PreviewImage = BitmapToImageSource(bmp);
        bmp.Dispose();
    }

    /// <summary>保存图片</summary>
    [RelayCommand]
    private void SaveImage()
    {
        if (_originalBitmap == null) return;

        var dlg = new SaveFileDialog
        {
            Filter = "PNG 图片|*.png|JPEG 图片|*.jpg|BMP 图片|*.bmp",
            Title = "保存图片",
            DefaultExt = "png"
        };
        if (dlg.ShowDialog() != true) return;

        var bmp = MakeWatermarkImage();
        var format = dlg.FilterIndex switch
        {
            2 => ImageFormat.Jpeg,
            3 => ImageFormat.Bmp,
            _ => ImageFormat.Png
        };
        bmp.Save(dlg.FileName, format);
        bmp.Dispose();
    }

    /// <summary>生成 ICO 图标</summary>
    [RelayCommand]
    private void MakeIco()
    {
        if (_originalBitmap == null)
        {
            MessageBox.Show("请先加载图片！", "提示");
            return;
        }

        var sizes = IconSizes.Where(e => e.IsSelected).Select(e => e.Size).ToArray();
        if (sizes.Length == 0)
        {
            MessageBox.Show("请至少选择一个图标尺寸！", "提示");
            return;
        }

        var dlg = new SaveFileDialog
        {
            Filter = "ICO 图标|*.ico",
            Title = "保存图标",
            DefaultExt = "ico"
        };
        if (dlg.ShowDialog() != true) return;

        var bmp = MakeWatermarkImage();
        var ms = new MemoryStream();
        IconHelper.Convert(bmp, ms, sizes, [32]);
        bmp.Dispose();

        File.WriteAllBytes(dlg.FileName, ms.ToArray());
    }
    #endregion

    #region 辅助
    private Bitmap MakeWatermarkImage()
    {
        var bmp = new Bitmap(_originalBitmap!);

        if (!WatermarkText.IsNullOrEmpty())
        {
            using var g = Graphics.FromImage(bmp);
            using var brush = new SolidBrush(ColorTranslator.FromHtml(WatermarkColor));
            using var ft = new System.Drawing.Font(FontName, FontSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            g.DrawString(WatermarkText, ft, brush, WatermarkX, WatermarkY);
        }

        return bmp;
    }

    private static BitmapImage BitmapToImageSource(Bitmap bmp)
    {
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);
        ms.Position = 0;

        var img = new BitmapImage();
        img.BeginInit();
        img.CacheOption = BitmapCacheOption.OnLoad;
        img.StreamSource = ms;
        img.EndInit();
        img.Freeze();
        return img;
    }
    #endregion
}

/// <summary>ICO 尺寸选项</summary>
public partial class IconSizeOption : ObservableObject
{
    /// <summary>显示名称</summary>
    public String Name { get; set; } = "";

    /// <summary>尺寸</summary>
    public Int32 Size { get; set; }

    /// <summary>是否选中</summary>
    [ObservableProperty]
    private Boolean _isSelected;
}
