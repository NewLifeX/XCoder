using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using CrazyCoder.ViewModels;

using static System.Windows.Media.Brushes;

namespace CrazyCoder.Views;

/// <summary>串口调试工具窗口</summary>
public partial class SerialPortWindow : Window
{
    /// <summary>ViewModel</summary>
    public SerialPortViewModel ViewModel { get; }

    /// <summary>实例化串口调试工具窗口</summary>
    public SerialPortWindow()
    {
        InitializeComponent();

        ViewModel = new SerialPortViewModel();
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
        if (msg.StartsWith("[发送]"))
            brush = Brushes.LimeGreen;
        else if (msg.StartsWith("[接收]"))
            brush = Brushes.Orange;
        else if (msg.Contains("错误") || msg.Contains("失败") || msg.Contains("异常"))
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
