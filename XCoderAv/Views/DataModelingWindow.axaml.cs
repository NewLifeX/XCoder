using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>代码生成器窗口</summary>
public partial class DataModelingWindow : Window
{
    /// <summary>实例化代码生成器窗口</summary>
    public DataModelingWindow()
    {
        InitializeComponent();
        DataContext = new DataModelingViewModel();
    }
}
