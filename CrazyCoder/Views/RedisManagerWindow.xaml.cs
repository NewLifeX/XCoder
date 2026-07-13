using System.Windows;
using System.Windows.Controls;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>Redis 管理器窗口</summary>
public partial class RedisManagerWindow : Window
{
    /// <summary>ViewModel</summary>
    public RedisManagerViewModel ViewModel { get; }

    /// <summary>实例化 Redis 管理器窗口</summary>
    public RedisManagerWindow()
    {
        InitializeComponent();

        ViewModel = new RedisManagerViewModel();
        DataContext = ViewModel;
    }

    private void OnTreeSelectedItemChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
    {
        if (e.NewValue is RedisTreeNode node)
        {
            node.IsSelected = true;
        }
        if (e.OldValue is RedisTreeNode oldNode)
        {
            oldNode.IsSelected = false;
        }
    }

    private void OnTreeMouseDoubleClick(Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (treeView.SelectedItem is RedisTreeNode node)
        {
            ViewModel.HandleNodeDoubleClick(node);
        }
    }
}
