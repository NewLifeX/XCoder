using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>声卡选择器窗口</summary>
public partial class AudioDeviceWindow : Window
{
    /// <summary>实例化声卡选择器窗口</summary>
    public AudioDeviceWindow()
    {
        InitializeComponent();
        DataContext = new AudioDeviceViewModel();
    }
}
