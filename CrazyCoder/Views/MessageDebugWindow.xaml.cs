using System.Windows;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>消息调试工具窗口</summary>
public partial class MessageDebugWindow : Window
{
    /// <summary>实例化消息调试工具窗口</summary>
    public MessageDebugWindow()
    {
        InitializeComponent();

        DataContext = new MessageDebugViewModel();
    }
}
