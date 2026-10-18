using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using XCoderAv.ViewModels;
using XCoderMac.Dialogs;

namespace XCoderMac.Views;

/// <summary>复用正则处理 ViewModel 的 macOS 风格窗口。</summary>
public partial class MacRegexWindow : Window
{
    private RegexViewModel ViewModel => (RegexViewModel)DataContext!;

    public MacRegexWindow()
    {
        InitializeComponent();
        DataContext = new RegexViewModel();
        // 隧道阶段先于多行文本框收到回车；普通回车为 Key.Return，数字键盘为 Key.Enter。
        AddHandler(InputElement.KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
    }

    private async void BatchReplace_Click(object? sender, RoutedEventArgs e)
    {
        if (!Directory.Exists(ViewModel.DirectoryPath))
        {
            ViewModel.Status = "请先选择有效的批量替换目录";
            return;
        }

        var ok = await ConfirmDialog.ShowAsync((TopLevel.GetTopLevel(sender as Control) as Window) ?? this, "确认批量替换文件？",
            $"将在目录 {ViewModel.DirectoryPath} 及其子目录中，按照文件过滤器“{ViewModel.FileFilter}”直接覆盖匹配的文件（原工具最多处理 1000 个文件）。请先备份相关文件。",
            "确认替换");
        if (ok && ViewModel.BatchReplaceCommand.CanExecute(null))
            ViewModel.BatchReplaceCommand.Execute(null);
    }

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if ((e.Key == Key.Return || e.Key == Key.Enter) && e.KeyModifiers.HasFlag(KeyModifiers.Meta))
        {
            if (ViewModel.ExecuteCommand.CanExecute(null))
                ViewModel.ExecuteCommand.Execute(null);
            e.Handled = true;
        }
    }
}
