using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using XCoderAv.Models;

namespace XCoderAv.ViewModels;

/// <summary>图标水印处理工具 ViewModel（跨平台版，基于 Avalonia + Skia）</summary>
public partial class IconToolViewModel : ObservableObject
{
    #region 属性
    /// <summary>源图片路径</summary>
    [ObservableProperty]
    private String _sourceFile = "";

    /// <summary>源图片</summary>
    [ObservableProperty]
    private Bitmap? _sourceImage;

    /// <summary>预览图片</summary>
    [ObservableProperty]
    private Bitmap? _previewImage;

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

    /// <summary>状态文本</summary>
    [ObservableProperty]
    private String _status = "就绪";

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
        // 加载系统字体
        foreach (var ft in FontManager.Current.SystemFonts)
        {
            FontNames.Add(ft.Name);
        }
    }
    #endregion

    #region 命令
    /// <summary>加载图片</summary>
    [RelayCommand]
    private async Task LoadImage()
    {
        var window = GetParentWindow();
        if (window == null) return;

        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择图片",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("图片文件") { Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif", "*.ico"] },
                new FilePickerFileType("所有文件") { Patterns = ["*"] }
            ]
        });
        if (files.Count == 0) return;

        var file = files[0].TryGetLocalPath();
        if (file != null) LoadFromFile(file);
    }

    /// <summary>从文件加载图片</summary>
    public void LoadFromFile(String fileName)
    {
        SourceFile = fileName;

        _originalBitmap?.Dispose();
        _originalBitmap = new Bitmap(fileName);

        SourceImage = _originalBitmap;

        // 自动设置水印位置到右下角
        WatermarkX = _originalBitmap.PixelSize.Width - 150;
        WatermarkY = _originalBitmap.PixelSize.Height - 50;

        MakeWater();
    }

    /// <summary>应用水印</summary>
    [RelayCommand]
    private void MakeWater()
    {
        if (_originalBitmap == null) return;

        var bmp = MakeWatermarkImage();
        PreviewImage = bmp;
    }

    /// <summary>保存图片</summary>
    [RelayCommand]
    private async Task SaveImage()
    {
        if (_originalBitmap == null) return;

        var window = GetParentWindow();
        if (window == null) return;

        var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "保存图片",
            SuggestedFileName = "watermark",
            DefaultExtension = "png",
            FileTypeChoices =
            [
                new FilePickerFileType("PNG 图片") { Patterns = ["*.png"] },
                new FilePickerFileType("JPEG 图片") { Patterns = ["*.jpg"] },
                new FilePickerFileType("BMP 图片") { Patterns = ["*.bmp"] }
            ]
        });
        if (file == null) return;

        var result = file.TryGetLocalPath();
        if (result == null) return;

        var bmp = MakeWatermarkImage();
        bmp.Save(result);
        bmp.Dispose();

        Status = $"已保存到 {result}";
    }

    /// <summary>生成 ICO 图标</summary>
    [RelayCommand]
    private async Task MakeIco()
    {
        if (_originalBitmap == null)
        {
            Status = "请先加载图片！";
            return;
        }

        var sizes = IconSizes.Where(e => e.IsSelected).Select(e => e.Size).ToArray();
        if (sizes.Length == 0)
        {
            Status = "请至少选择一个图标尺寸！";
            return;
        }

        var window = GetParentWindow();
        if (window == null) return;

        var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "保存图标",
            SuggestedFileName = "icon",
            DefaultExtension = "ico",
            FileTypeChoices =
            [
                new FilePickerFileType("ICO 图标") { Patterns = ["*.ico"] }
            ]
        });
        if (file == null) return;

        var result = file.TryGetLocalPath();
        if (result == null) return;

        var bmp = MakeWatermarkImage();
        var ms = new MemoryStream();
        IconHelper.Convert(bmp, ms, sizes, [32]);
        bmp.Dispose();

        await File.WriteAllBytesAsync(result, ms.ToArray());

        Status = $"已保存 ICO 到 {result}";
    }
    #endregion

    #region 辅助
    private Bitmap MakeWatermarkImage()
    {
        if (_originalBitmap == null) throw new InvalidOperationException(L.T("No image is loaded."));

        var size = _originalBitmap.PixelSize;
        var dpi = _originalBitmap.Dpi;

        // 使用渲染目标位图来绘制
        var rtBmp = new RenderTargetBitmap(size, dpi);
        using (var ctx = rtBmp.CreateDrawingContext())
        {
            ctx.DrawImage(_originalBitmap, new Rect(0, 0, size.Width, size.Height));

            if (!WatermarkText.IsNullOrEmpty())
            {
                var color = Color.Parse(WatermarkColor);
                var typeface = new Typeface(FontName);
                var formattedText = new FormattedText(
                    WatermarkText,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    FontSize,
                    new SolidColorBrush(color));
                formattedText.TextAlignment = TextAlignment.Left;
                formattedText.MaxTextWidth = size.Width;

                ctx.DrawText(formattedText, new Point(WatermarkX, WatermarkY));
            }
        }

        return rtBmp;
    }

    private static Window? GetParentWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.Windows?.OfType<Window>().FirstOrDefault(w => w.IsActive);
        }
        return null;
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
