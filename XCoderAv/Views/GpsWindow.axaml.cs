using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>GPS 辅助工具窗口</summary>
public partial class GpsWindow : Window
{
    /// <summary>实例化 GPS 辅助工具窗口</summary>
    public GpsWindow()
    {
        InitializeComponent();
        DataContext = new GpsViewModel();
    }
}
