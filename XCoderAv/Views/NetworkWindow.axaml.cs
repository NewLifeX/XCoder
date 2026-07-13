using Avalonia.Controls;
using Avalonia.Threading;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>网络调试工具窗口</summary>
public partial class NetworkWindow : Window
{
    /// <summary>实例化网络调试工具窗口</summary>
    public NetworkWindow()
    {
        InitializeComponent();

        var vm = new NetworkViewModel();
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
