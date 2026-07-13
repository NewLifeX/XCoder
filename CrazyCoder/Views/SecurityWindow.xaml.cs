using System.Windows;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>加密解密工具窗口</summary>
public partial class SecurityWindow : Window
{
    /// <summary>实例化加密解密工具窗口</summary>
    public SecurityWindow()
    {
        InitializeComponent();

        DataContext = new SecurityViewModel();
    }
}
