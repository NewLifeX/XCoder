using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views;

/// <summary>网络调试工具窗口</summary>
public partial class NetworkWindow : Window
{
    /// <summary>ViewModel</summary>
    public NetworkViewModel ViewModel { get; }

    /// <summary>实例化网络调试工具窗口</summary>
    public NetworkWindow()
    {
        InitializeComponent();

        ViewModel = new NetworkViewModel();
        DataContext = ViewModel;

        // 绑定日志回调
        ViewModel.OnLog += AppendLog;
    }

    private void AppendLog(String msg)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => AppendLog(msg));
            return;
        }

        if (msg == "__CLEAR__")
        {
            txtReceive.Document.Blocks.Clear();
            return;
        }

        var paragraph = new Paragraph(new Run(msg))
        {
            Margin = new Thickness(0),
            Padding = new Thickness(0)
        };

        txtReceive.Document.Blocks.Add(paragraph);
        txtReceive.ScrollToEnd();
    }
}
