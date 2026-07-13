using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>API 调试工具窗口</summary>
public partial class ApiDebugWindow : Window
{
    /// <summary>实例化 API 调试工具窗口</summary>
    public ApiDebugWindow()
    {
        InitializeComponent();
        DataContext = new ApiDebugViewModel();
    }
}
