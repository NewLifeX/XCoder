using System.Windows;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>API 服务发现窗口</summary>
public partial class ApiDiscoverWindow : Window
{
    /// <summary>实例化 API 服务发现窗口</summary>
    public ApiDiscoverWindow()
    {
        InitializeComponent();

        DataContext = new ApiDiscoverViewModel();
    }
}
