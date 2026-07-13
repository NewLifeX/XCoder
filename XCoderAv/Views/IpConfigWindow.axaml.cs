using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>IP 设置工具窗口</summary>
public partial class IpConfigWindow : Window
{
    /// <summary>实例化 IP 设置工具窗口</summary>
    public IpConfigWindow()
    {
        InitializeComponent();
        DataContext = new IpConfigViewModel();
    }
}
