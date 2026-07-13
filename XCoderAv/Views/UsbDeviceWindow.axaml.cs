using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>USB 设备检测窗口</summary>
public partial class UsbDeviceWindow : Window
{
    /// <summary>实例化 USB 设备检测窗口</summary>
    public UsbDeviceWindow()
    {
        InitializeComponent();
        DataContext = new UsbDeviceViewModel();
    }
}
