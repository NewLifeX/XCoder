using System.Windows;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>文件编码转换工具窗口</summary>
public partial class FileEncodingWindow : Window
{
    /// <summary>实例化文件编码转换工具窗口</summary>
    public FileEncodingWindow()
    {
        InitializeComponent();

        DataContext = new FileEncodingViewModel();
    }
}
