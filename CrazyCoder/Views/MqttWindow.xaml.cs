using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>MQTT 客户端窗口</summary>
public partial class MqttWindow : Window
{
    /// <summary>ViewModel</summary>
    public MqttViewModel ViewModel { get; }

    /// <summary>实例化 MQTT 客户端窗口</summary>
    public MqttWindow()
    {
        InitializeComponent();

        ViewModel = new MqttViewModel();
        DataContext = ViewModel;

        // 绑定日志回调
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

        var paragraph = new Paragraph(new Run(msg))
        {
            Margin = new Thickness(0),
            Padding = new Thickness(0)
        };

        txtLog.Document.Blocks.Add(paragraph);
        txtLog.ScrollToEnd();
    }
}
