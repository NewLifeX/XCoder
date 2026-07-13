using System.Windows;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>SSH 工具窗口</summary>
public partial class SshWindow : Window
{
    /// <summary>实例化 SSH 工具窗口</summary>
    public SshWindow()
    {
        InitializeComponent();

        DataContext = new SshViewModel();
    }
}
