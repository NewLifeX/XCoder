using System.Windows;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>地图接口窗口</summary>
public partial class MapWindow : Window
{
    /// <summary>实例化地图接口窗口</summary>
    public MapWindow()
    {
        InitializeComponent();

        DataContext = new MapViewModel();
    }
}
