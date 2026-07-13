using System.Windows;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>跨库数据同步窗口</summary>
public partial class DataSyncWindow : Window
{
    /// <summary>ViewModel</summary>
    public DataSyncViewModel ViewModel { get; }

    /// <summary>实例化跨库数据同步窗口</summary>
    public DataSyncWindow()
    {
        InitializeComponent();

        ViewModel = new DataSyncViewModel();
        DataContext = ViewModel;
    }
}
