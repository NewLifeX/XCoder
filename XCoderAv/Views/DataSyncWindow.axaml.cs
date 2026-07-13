using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>数据同步窗口</summary>
public partial class DataSyncWindow : Window
{
    /// <summary>实例化数据同步窗口</summary>
    public DataSyncWindow()
    {
        InitializeComponent();
        DataContext = new DataSyncViewModel();
    }
}
