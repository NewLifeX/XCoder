using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderMac.Views;

/// <summary>Mac RPC 面板。NewLife.Remoting 调用仍使用上游 ViewModel。</summary>
public partial class MacApiDebugWindow : Window
{
    private ApiDebugViewModel ViewModel => (ApiDebugViewModel)DataContext!;

    public MacApiDebugWindow()
    {
        InitializeComponent();
        DataContext = new ApiDebugViewModel();
        Closed += (_, _) =>
        {
            if (ViewModel.IsConnected && ViewModel.ToggleConnectCommand.CanExecute(null))
                ViewModel.ToggleConnectCommand.Execute(null);
        };
    }

#if DEBUG
    /// <summary>不连接网络，确认服务端初始无远端 Action，且控件仍绑定上游集合。</summary>
    public void SmokeTestActionListInitialState()
    {
        if (ViewModel.SelectedModeIndex != 0 || ViewModel.ApiActions.Count != 0 ||
            !ReferenceEquals(RpcActionSelect.ItemsSource, ViewModel.ApiActions))
            throw new System.InvalidOperationException("RPC Action 下拉框的初始数据绑定与服务端状态异常");
        if (Avalonia.Controls.ToolTip.GetTip(RpcActionSelect) is not string tip ||
            !tip.Contains("客户端"))
            throw new System.InvalidOperationException("RPC Action 下拉框没有解释空列表原因");
        System.Console.WriteLine("RPC ACTION EMPTY STATE OK: server mode has no remote Api/All until client connection");
    }
#endif
}
