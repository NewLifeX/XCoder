using Avalonia.Controls;
using XCoderAv.ViewModels;

namespace XCoderAv.Views;

/// <summary>Modbus RTU 调试窗口</summary>
public partial class ModbusRtuWindow : Window
{
    /// <summary>实例化 Modbus RTU 调试窗口</summary>
    public ModbusRtuWindow()
    {
        InitializeComponent();
        DataContext = new ModbusRtuViewModel();
    }
}
