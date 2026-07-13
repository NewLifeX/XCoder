using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>Redis 管理器窗口</summary>
public partial class RedisManagerWindow : Window
{
    /// <summary>实例化 Redis 管理器窗口</summary>
    public RedisManagerWindow()
    {
        InitializeComponent();
        DataContext = new RedisManagerViewModel();
    }
}
