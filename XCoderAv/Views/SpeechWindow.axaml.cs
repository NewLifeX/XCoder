using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>语音助手窗口</summary>
public partial class SpeechWindow : Window
{
    /// <summary>实例化语音助手窗口</summary>
    public SpeechWindow()
    {
        InitializeComponent();
        DataContext = new SpeechViewModel();
    }
}
