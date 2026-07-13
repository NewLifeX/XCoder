using System.Windows;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>图标水印处理工具窗口</summary>
public partial class IconToolWindow : Window
{
    /// <summary>ViewModel</summary>
    public IconToolViewModel ViewModel { get; }

    /// <summary>实例化图标水印处理工具窗口</summary>
    public IconToolWindow()
    {
        InitializeComponent();

        ViewModel = new IconToolViewModel();
        DataContext = ViewModel;
    }

    private void OnImageDrop(Object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (String[])e.Data.GetData(DataFormats.FileDrop);
            if (files is { Length: > 0 })
            {
                ViewModel.LoadFromFile(files[0]);
            }
        }
    }

    private void OnImageDragOver(Object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effects = DragDropEffects.Copy;
        else
            e.Effects = DragDropEffects.None;

        e.Handled = true;
    }
}
