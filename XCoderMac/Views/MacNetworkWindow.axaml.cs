using System;
using Avalonia.Controls;
using Avalonia.Threading;
using XCoderAv.ViewModels;

namespace XCoderMac.Views;

/// <summary>复用原 NetworkViewModel，通过回调更新 Mac 通信日志。</summary>
public partial class MacNetworkWindow : Window
{
    private NetworkViewModel ViewModel => (NetworkViewModel)DataContext!;

    public MacNetworkWindow()
    {
        InitializeComponent();
        var vm = new NetworkViewModel { OnLog = AppendLog };
        DataContext = vm;
        Closed += (_, _) =>
        {
            if (vm.IsConnected && vm.ToggleConnectCommand.CanExecute(null))
                vm.ToggleConnectCommand.Execute(null);
        };
    }

    private void AppendLog(string msg)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (msg == "__CLEAR__")
            {
                txtLog.Text = "";
                return;
            }

            var old = txtLog.Text ?? "";
            var next = old + msg + Environment.NewLine;
            txtLog.Text = next.Length > 30000 ? next[^15000..] : next;
            txtLog.CaretIndex = txtLog.Text.Length;
        });
    }
}
