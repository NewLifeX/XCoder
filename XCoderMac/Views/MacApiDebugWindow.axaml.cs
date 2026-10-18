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
}
