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
        // 上游已填充“所有本地IPv4/IPv6”、0.0.0.0、:: 和本机网卡地址，
        // 但 LocalAddress 初始为空；Mac 端默认选择第一个合法选项，不影响用户后续选择。
        if (vm.LocalAddresses.Count > 0 && String.IsNullOrWhiteSpace(vm.LocalAddress))
            vm.LocalAddress = vm.LocalAddresses[0];
        DataContext = vm;
        Closed += (_, _) =>
        {
            if (vm.IsConnected && vm.ToggleConnectCommand.CanExecute(null))
                vm.ToggleConnectCommand.Execute(null);
        };
    }

#if DEBUG
    /// <summary>只验证本地网卡列表与首项默认选中，绝不建立网络连接。</summary>
    public void SmokeTestInitialLocalAddress()
    {
        var vm = ViewModel;
        if (vm.LocalAddresses.Count == 0 || vm.LocalAddress != vm.LocalAddresses[0] ||
            !Equals(LocalBindingAddressSelect.SelectedItem, vm.LocalAddress))
            throw new InvalidOperationException(
                $"本地绑定地址首项未默认选中：Local={vm.LocalAddress}, UI={LocalBindingAddressSelect.SelectedItem}");

        // 验证改选网卡仍能通过 SelectedItem 双向绑定；还原首项。
        if (vm.LocalAddresses.Count > 1)
        {
            var first = vm.LocalAddress;
            LocalBindingAddressSelect.SelectedIndex = 1;
            if (vm.LocalAddress != vm.LocalAddresses[1])
                throw new InvalidOperationException("本地绑定地址手动选择未更新 ViewModel");
            LocalBindingAddressSelect.SelectedIndex = 0;
            if (vm.LocalAddress != first)
                throw new InvalidOperationException("本地绑定地址未成功还原默认值");
        }
        System.Console.WriteLine($"NETWORK LOCAL ADDRESS OK: default={vm.LocalAddress}, count={vm.LocalAddresses.Count}, manual selection works");
    }
#endif

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
