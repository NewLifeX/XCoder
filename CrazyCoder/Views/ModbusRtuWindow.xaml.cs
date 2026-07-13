using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using CrazyCoder.ViewModels;

using static System.Windows.Media.Brushes;

namespace CrazyCoder.Views;

/// <summary>Modbus RTU 工具窗口</summary>
public partial class ModbusRtuWindow : Window
{
    /// <summary>ViewModel</summary>
    public ModbusRtuViewModel ViewModel { get; }

    /// <summary>实例化 Modbus RTU 工具窗口</summary>
    public ModbusRtuWindow()
    {
        InitializeComponent();

        ViewModel = new ModbusRtuViewModel();
        DataContext = ViewModel;

        ViewModel.OnLog += AppendLog;
    }

    private void AppendLog(String msg)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => AppendLog(msg));
            return;
        }

        if (msg == "__CLEAR__")
        {
            txtLog.Document.Blocks.Clear();
            return;
        }

        var brush = new SolidColorBrush(Color.FromRgb(0xD4, 0xD4, 0xD4));
        if (msg.Contains("失败") || msg.Contains("错误") || msg.Contains("异常"))
            brush = Brushes.Red;

        var run = new Run(msg) { Foreground = brush };
        var paragraph = new Paragraph(run)
        {
            Margin = new Thickness(0),
            Padding = new Thickness(0)
        };

        txtLog.Document.Blocks.Add(paragraph);
        txtLog.ScrollToEnd();
    }
}
