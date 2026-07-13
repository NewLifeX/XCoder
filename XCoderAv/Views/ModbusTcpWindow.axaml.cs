using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>Modbus TCP 调试窗口</summary>
public partial class ModbusTcpWindow : Window
{
    /// <summary>实例化 Modbus TCP 调试窗口</summary>
    public ModbusTcpWindow()
    {
        InitializeComponent();
        DataContext = new ModbusTcpViewModel();
    }
}
