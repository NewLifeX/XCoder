using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using XCoderAv.ViewModels;

namespace XCoderAv;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
