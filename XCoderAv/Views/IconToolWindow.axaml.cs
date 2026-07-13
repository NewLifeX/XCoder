using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>图标水印处理工具窗口</summary>
public partial class IconToolWindow : Window
{
    /// <summary>实例化图标水印处理工具窗口</summary>
    public IconToolWindow()
    {
        InitializeComponent();
        DataContext = new IconToolViewModel();
    }
}
