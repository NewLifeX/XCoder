#nullable enable

using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Log;
using NewLife.Serial.Protocols;

namespace CrazyCoder.ViewModels;

/// <summary>Modbus RTU 工具 ViewModel</summary>
public partial class ModbusRtuViewModel : ObservableObject, IDisposable
{
    #region 属性
    private ModbusRtu? _modbus;

    /// <summary>端口名称列表</summary>
    public ObservableCollection<String> PortNames { get; } = [];

    /// <summary>波特率列表</summary>
    public ObservableCollection<Int32> BaudRates { get; } = [1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 128000, 194000, 256000, 512000, 1024000, 2048000];

    /// <summary>校验位列表</summary>
    public ObservableCollection<String> ParityOptions { get; } = ["None", "Odd", "Even", "Mark", "Space"];

    /// <summary>停止位列表</summary>
    public ObservableCollection<String> StopBitsOptions { get; } = ["One", "Two", "OnePointFive"];

    /// <summary>数据位列表</summary>
    public ObservableCollection<Int32> DataBitsOptions { get; } = [5, 6, 7, 8];

    /// <summary>功能码列表</summary>
    public ObservableCollection<String> FunctionCodes { get; } =
    [
        "01 读线圈",
        "02 读离散量输入",
        "03 读保持寄存器",
        "04 读输入寄存器",
        "05 写单线圈",
        "06 写单寄存器",
        "0F 写多线圈",
        "10 写多寄存器",
    ];

    /// <summary>端口名称</summary>
    [ObservableProperty]
    private String _portName = "COM1";

    /// <summary>波特率</summary>
    [ObservableProperty]
    private Int32 _baudRate = 115200;

    /// <summary>校验位索引</summary>
    [ObservableProperty]
    private Int32 _parityIndex;

    /// <summary>数据位</summary>
    [ObservableProperty]
    private Int32 _dataBits = 8;

    /// <summary>停止位索引</summary>
    [ObservableProperty]
    private Int32 _stopBitsIndex;

    /// <summary>从站地址</summary>
    [ObservableProperty]
    private Byte _slaveAddress = 1;

    /// <summary>功能码索引</summary>
    [ObservableProperty]
    private Int32 _functionCodeIndex;

    /// <summary>起始地址</summary>
    [ObservableProperty]
    private UInt16 _startAddress;

    /// <summary>读取数量</summary>
    [ObservableProperty]
    private UInt16 _readCount = 10;

    /// <summary>写入值</summary>
    [ObservableProperty]
    private String _writeValue = "0";

    /// <summary>是否已连接</summary>
    [ObservableProperty]
    private Boolean _isConnected;

    /// <summary>连接按钮文本</summary>
    [ObservableProperty]
    private String _connectButtonText = "连接";

    /// <summary>读取结果（十六进制）</summary>
    [ObservableProperty]
    private String _readResult = "";

    /// <summary>日志回调</summary>
    public Action<String>? OnLog { get; set; }
    #endregion

    #region 构造
    /// <summary>实例化 Modbus RTU 工具 ViewModel</summary>
    public ModbusRtuViewModel()
    {
        RefreshPorts();
    }
    #endregion

    #region 方法
    /// <summary>刷新串口列表</summary>
    [RelayCommand]
    private void RefreshPorts()
    {
        PortNames.Clear();
        foreach (var name in SerialPort.GetPortNames())
        {
            PortNames.Add(name);
        }
        if (PortNames.Count > 0) PortName = PortNames[0];
    }

    private ModbusRtu CreateModbus()
    {
        var parity = ParityIndex switch
        {
            1 => Parity.Odd,
            2 => Parity.Even,
            3 => Parity.Mark,
            4 => Parity.Space,
            _ => Parity.None,
        };
        var stopBits = StopBitsIndex switch
        {
            1 => StopBits.Two,
            2 => StopBits.OnePointFive,
            _ => StopBits.One,
        };

        return new ModbusRtu
        {
            PortName = PortName,
            Baudrate = BaudRate,
            DataBits = DataBits,
            Parity = parity,
            StopBits = stopBits,
            Log = new ModbusLog(this),
        };
    }
    #endregion

    #region 连接/断开
    /// <summary>切换连接/断开</summary>
    [RelayCommand]
    private void ToggleConnect()
    {
        if (IsConnected)
            Disconnect();
        else
            Connect();
    }

    private void Connect()
    {
        try
        {
            _modbus?.Dispose();
            _modbus = CreateModbus();
            _modbus.Open();

            IsConnected = true;
            ConnectButtonText = "断开";

            WriteLog($"Modbus RTU 已连接 {PortName}，{BaudRate}/{DataBits}/{ParityOptions[ParityIndex]}/{StopBitsOptions[StopBitsIndex]}");
        }
        catch (Exception ex)
        {
            WriteLog($"连接失败：{ex.Message}");
            MessageBox.Show($"连接失败：{ex.Message}", "错误");
        }
    }

    private void Disconnect()
    {
        if (_modbus != null)
        {
            try { _modbus.Dispose(); } catch { }
            _modbus = null;
        }

        IsConnected = false;
        ConnectButtonText = "连接";

        WriteLog("Modbus RTU 已断开");
    }
    #endregion

    #region 读写操作
    /// <summary>执行读写操作</summary>
    [RelayCommand]
    private void Execute()
    {
        var mb = _modbus;
        if (mb == null)
        {
            WriteLog("未连接");
            return;
        }

        var host = SlaveAddress;
        var address = StartAddress;
        var code = FunctionCodeIndex;

        try
        {
            switch (code)
            {
                case 0: // 01 读线圈
                    {
                        var count = ReadCount;
                        var data = mb.ReadCoil(host, address, count);
                        if (data != null)
                        {
                            var bits = String.Join(" ", data.Select(e => e ? "1" : "0"));
                            WriteLog($"读线圈 [{data.Length}位] {bits}");
                            ReadResult = bits;
                        }
                        else
                        {
                            WriteLog("读线圈：无返回数据");
                            ReadResult = "无返回数据";
                        }
                        break;
                    }
                case 1: // 02 读离散量输入
                    {
                        var count = ReadCount;
                        var data = mb.ReadDiscrete(host, address, count);
                        if (data != null)
                        {
                            var bits = String.Join(" ", data.Select(e => e ? "1" : "0"));
                            WriteLog($"读离散量输入 [{data.Length}位] {bits}");
                            ReadResult = bits;
                        }
                        else
                        {
                            WriteLog("读离散量输入：无返回数据");
                            ReadResult = "无返回数据";
                        }
                        break;
                    }
                case 2: // 03 读保持寄存器
                    {
                        var count = ReadCount;
                        var data = mb.ReadRegister(host, address, count);
                        if (data != null)
                        {
                            var hex = String.Join(", ", data);
                            WriteLog($"读保持寄存器 [{data.Length}个] {hex}");
                            ReadResult = hex;
                        }
                        else
                        {
                            WriteLog("读保持寄存器：无返回数据");
                            ReadResult = "无返回数据";
                        }
                        break;
                    }
                case 3: // 04 读输入寄存器
                    {
                        var count = ReadCount;
                        var data = mb.ReadInput(host, address, count);
                        if (data != null)
                        {
                            var hex = String.Join(", ", data);
                            WriteLog($"读输入寄存器 [{data.Length}个] {hex}");
                            ReadResult = hex;
                        }
                        else
                        {
                            WriteLog("读输入寄存器：无返回数据");
                            ReadResult = "无返回数据";
                        }
                        break;
                    }
                case 4: // 05 写单线圈
                    {
                        var value = (UInt16)(WriteValue.ToInt() > 0 ? 0xFF00 : 0x0000);
                        mb.WriteCoil(host, address, value);
                        WriteLog($"写单线圈 地址={address} 值=0x{value:X4}");
                        ReadResult = $"写入成功：地址={address} 值=0x{value:X4}";
                        break;
                    }
                case 5: // 06 写单寄存器
                    {
                        var value = (UInt16)WriteValue.ToInt();
                        mb.WriteRegister(host, address, value);
                        WriteLog($"写单寄存器 地址={address} 值=0x{value:X4}");
                        ReadResult = $"写入成功：地址={address} 值=0x{value:X4}";
                        break;
                    }
                case 6: // 0F 写多线圈
                    {
                        var count = ReadCount;
                        var values = new UInt16[count];
                        for (var i = 0; i < count; i++) values[i] = 0xFF00;
                        mb.WriteCoils(host, address, values);
                        WriteLog($"写多线圈 地址={address} 数量={count}");
                        ReadResult = $"写入成功：地址={address} 数量={count}";
                        break;
                    }
                case 7: // 10 写多寄存器
                    {
                        var count = ReadCount;
                        var values = new UInt16[count];
                        for (var i = 0; i < count; i++) values[i] = (UInt16)(WriteValue.ToInt() + i);
                        mb.WriteRegisters(host, address, values);
                        WriteLog($"写多寄存器 地址={address} 数量={count}");
                        ReadResult = $"写入成功：地址={address} 数量={count}";
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            WriteLog($"操作失败：{ex.Message}");
            ReadResult = $"操作失败：{ex.Message}";
        }
    }


    /// <summary>清空日志</summary>
    [RelayCommand]
    private void ClearLog()
    {
        OnLog?.Invoke("__CLEAR__");
    }
    #endregion

    #region 日志
    private void WriteLog(String msg)
    {
        OnLog?.Invoke(msg);
    }

    private class ModbusLog : Logger
    {
        private readonly ModbusRtuViewModel _vm;
        public ModbusLog(ModbusRtuViewModel vm) => _vm = vm;
        protected override void OnWrite(LogLevel level, String format, params Object?[] args)
        {
            var msg = args is { Length: > 0 } ? String.Format(format, args) : format;
            _vm.WriteLog(msg);
        }
    }
    #endregion

    #region IDisposable
    /// <summary>释放资源</summary>
    public void Dispose()
    {
        Disconnect();
        GC.SuppressFinalize(this);
    }
    #endregion
}
