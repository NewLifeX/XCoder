using Avalonia.Controls;
using Avalonia.Threading;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>MQTT 客户端窗口</summary>
public partial class MqttWindow : Window
{
    /// <summary>实例化 MQTT 客户端窗口</summary>
    public MqttWindow()
    {
        InitializeComponent();

        var vm = new MqttViewModel();
        vm.OnLog = OnLog;
        DataContext = vm;
    }

    private void OnLog(String msg)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (msg == "__CLEAR__")
            {
                txtLog.Text = "";
                return;
            }

            txtLog.Text += msg + "\n";
            if (txtLog.Text.Length > 10000)
                txtLog.Text = txtLog.Text[^5000..];

            txtLog.CaretIndex = txtLog.Text.Length;
        });
    }
}
