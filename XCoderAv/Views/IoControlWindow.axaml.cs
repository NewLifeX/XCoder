using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>I/O 控制面板窗口</summary>
public partial class IoControlWindow : Window
{
    /// <summary>实例化 I/O 控制面板窗口</summary>
    public IoControlWindow()
    {
        InitializeComponent();
        DataContext = new IoControlViewModel();
    }
}
