using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>正则表达式工具窗口</summary>
public partial class RegexWindow : Window
{
    /// <summary>实例化正则表达式工具窗口</summary>
    public RegexWindow()
    {
        InitializeComponent();
        DataContext = new RegexViewModel();
    }
}
