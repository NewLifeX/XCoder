using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>串口调试工具窗口</summary>
public partial class SerialPortWindow : Window
{
    /// <summary>实例化串口调试工具窗口</summary>
    public SerialPortWindow()
    {
        InitializeComponent();

        var vm = new SerialPortViewModel();
        vm.OnLog = OnLog;
        DataContext = vm;
    }

    private void OnLog(String msg)
    {
        // ViewModel 已处理日志写入 _log 属性
    }
}
