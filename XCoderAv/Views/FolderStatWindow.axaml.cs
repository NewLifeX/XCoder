using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>文件夹大小统计工具窗口</summary>
public partial class FolderStatWindow : Window
{
    /// <summary>实例化文件夹大小统计窗口</summary>
    public FolderStatWindow()
    {
        InitializeComponent();

        var vm = new FolderStatViewModel();
        vm.OnLog = OnLog;
        DataContext = vm;

        // 加载根节点
        treeView.ItemsSource = vm.Roots;

        // 订阅 TreeViewItem 的 Expanded/Collapsed 路由事件
        treeView.AddHandler(TreeViewItem.ExpandedEvent, OnItemExpanded);
        treeView.AddHandler(TreeViewItem.CollapsedEvent, OnItemCollapsed);
    }

    private async void OnItemExpanded(Object? sender, RoutedEventArgs e)
    {
        if (e.Source is TreeViewItem tvi && tvi.DataContext is FolderItem folderItem && DataContext is FolderStatViewModel vm)
        {
            await vm.ExpandFolder(folderItem);
        }
    }

    private void OnItemCollapsed(Object? sender, RoutedEventArgs e)
    {
        if (e.Source is TreeViewItem tvi && tvi.DataContext is FolderItem folderItem && DataContext is FolderStatViewModel vm)
        {
            vm.CollapseFolder(folderItem);
        }
    }

    private void OnLog(String msg)
    {
        Dispatcher.UIThread.Post(() =>
        {
            txtLog.Text += msg + "\n";
            if (txtLog.Text.Length > 10000)
                txtLog.Text = txtLog.Text[^5000..];

            txtLog.CaretIndex = txtLog.Text.Length;
        });
    }
}
