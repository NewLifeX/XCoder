using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>手机备份窗口</summary>
public partial class BackupWindow : Window
{
    /// <summary>实例化手机备份窗口</summary>
    public BackupWindow()
    {
        InitializeComponent();
        DataContext = new BackupViewModel();
    }
}
