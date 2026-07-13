using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>音频梅尔频谱窗口</summary>
public partial class MelSpectrumWindow : Window
{
    /// <summary>实例化音频梅尔频谱窗口</summary>
    public MelSpectrumWindow()
    {
        InitializeComponent();
        DataContext = new MelSpectrumViewModel();
    }
}
