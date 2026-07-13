using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>文件编码转换窗口</summary>
public partial class FileEncodingWindow : Window
{
    /// <summary>实例化文件编码转换窗口</summary>
    public FileEncodingWindow()
    {
        InitializeComponent();
        DataContext = new FileEncodingViewModel();
    }
}
