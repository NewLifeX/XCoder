using System.Windows;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>手机备份工具窗口</summary>
public partial class BackupWindow : Window
{
    /// <summary>实例化手机备份工具窗口</summary>
    public BackupWindow()
    {
        InitializeComponent();

        DataContext = new BackupViewModel();
    }
}
