using System.Windows;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>数据建模工具窗口</summary>
public partial class DataModelingWindow : Window
{
    /// <summary>ViewModel</summary>
    public DataModelingViewModel ViewModel { get; }

    /// <summary>实例化数据建模工具窗口</summary>
    public DataModelingWindow()
    {
        InitializeComponent();

        ViewModel = new DataModelingViewModel();
        DataContext = ViewModel;
    }
}
