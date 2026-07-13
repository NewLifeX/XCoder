using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Log;
using NewLife.Serial.Protocols;

namespace XCoderAv.ViewModels;

/// <summary>I/O 控制面板 ViewModel</summary>
public partial class IoControlViewModel : ObservableObject, IDisposable
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

    /// <summary>是否已连接</summary>
    [ObservableProperty]
    private Boolean _isConnected;

    /// <summary>连接按钮文本</summary>
    [ObservableProperty]
    private String _connectButtonText = "连接";

    /// <summary>输出 1 状态</summary>
    [ObservableProperty]
    private Boolean _output1;

    /// <summary>输出 2 状态</summary>
    [ObservableProperty]
    private Boolean _output2;

    /// <summary>输出 3 状态</summary>
    [ObservableProperty]
    private Boolean _output3;

    /// <summary>输出 4 状态</summary>
    [ObservableProperty]
    private Boolean _output4;

    /// <summary>输出 5 状态</summary>
    [ObservableProperty]
    private Boolean _output5;

    /// <summary>输出 6 状态</summary>
    [ObservableProperty]
    private Boolean _output6;

    /// <summary>输出 7 状态</summary>
    [ObservableProperty]
    private Boolean _output7;

    /// <summary>输出 8 状态</summary>
    [ObservableProperty]
    private Boolean _output8;

    /// <summary>输入 1 状态</summary>
    [ObservableProperty]
    private Boolean _input1;

    /// <summary>输入 2 状态</summary>
    [ObservableProperty]
    private Boolean _input2;

    /// <summary>输入 3 状态</summary>
    [ObservableProperty]
    private Boolean _input3;

    /// <summary>输入 4 状态</summary>
    [ObservableProperty]
    private Boolean _input4;

    /// <summary>输入 5 状态</summary>
    [ObservableProperty]
    private Boolean _input5;

    /// <summary>输入 6 状态</summary>
    [ObservableProperty]
    private Boolean _input6;

    /// <summary>输入 7 状态</summary>
    [ObservableProperty]
    private Boolean _input7;

    /// <summary>输入 8 状态</summary>
    [ObservableProperty]
    private Boolean _input8;

    /// <summary>延迟（毫秒）</summary>
    [ObservableProperty]
    private Int32 _delay = 100;

    /// <summary>产品型号</summary>
    [ObservableProperty]
    private String _productType = "";

    /// <summary>固件版本</summary>
    [ObservableProperty]
    private String _firmwareVersion = "";

    /// <summary>日志文本</summary>
    [ObservableProperty]
    private String _log = "";

    /// <summary>日志回调</summary>
    public Action<String>? OnLog { get; set; }
    #endregion

    #region 构造
    /// <summary>实例化 I/O 控制面板 ViewModel</summary>
    public IoControlViewModel()
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
            Log = new IoControlLog(this),
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

            WriteLog($"已连接 {PortName}，{BaudRate}/{DataBits}/{ParityOptions[ParityIndex]}/{StopBitsOptions[StopBitsIndex]}");

            ReadDeviceInfo();
        }
        catch (Exception ex)
        {
            WriteLog($"连接失败：{ex.Message}");
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

        WriteLog("已断开连接");
    }
    #endregion

    #region I/O 操作
    /// <summary>读取设备信息</summary>
    [RelayCommand]
    private void ReadDeviceInfo()
    {
        var mb = _modbus;
        if (mb == null) return;

        try
        {
            var data = mb.ReadRegister(SlaveAddress, 0x1000, 14);
            if (data == null || data.Length < 8)
            {
                WriteLog("读取设备信息失败：无返回数据");
                return;
            }

            var buf = new Byte[data.Length * 2];
            for (var i = 0; i < data.Length; i++)
            {
                buf[i * 2] = (Byte)(data[i] >> 8);
                buf[i * 2 + 1] = (Byte)(data[i] & 0xFF);
            }

            ProductType = Encoding.ASCII.GetString(buf, 0, 4);
            FirmwareVersion = Encoding.ASCII.GetString(buf, 4, 4);
            WriteLog($"设备信息：型号={ProductType} 版本={FirmwareVersion}");
        }
        catch (Exception ex)
        {
            WriteLog($"读取设备信息失败：{ex.Message}");
        }
    }

    /// <summary>打开指定输出</summary>
    [RelayCommand]
    private void TurnOn(String index)
    {
        var mb = _modbus;
        if (mb == null) return;

        var addr = index.ToInt() - 1;
        try
        {
            if (Delay > 0)
                mb.WriteRegisters(SlaveAddress, (UInt16)(0x0003 + addr * 5), [0x0004, (UInt16)(Delay / 100)]);
            else
                mb.WriteCoil(SlaveAddress, (UInt16)addr, 0xFF00);

            WriteLog($"输出 {index} 已打开 (地址={addr})");
            UpdateOutputProperty(index, true);
        }
        catch (Exception ex)
        {
            WriteLog($"打开输出 {index} 失败：{ex.Message}");
        }
    }

    /// <summary>关闭指定输出</summary>
    [RelayCommand]
    private void TurnOff(String index)
    {
        var mb = _modbus;
        if (mb == null) return;

        var addr = index.ToInt() - 1;
        try
        {
            if (Delay > 0)
                mb.WriteRegisters(SlaveAddress, (UInt16)(0x0003 + addr * 5), [0x0002, (UInt16)(Delay / 100)]);
            else
                mb.WriteCoil(SlaveAddress, (UInt16)addr, 0);

            WriteLog($"输出 {index} 已关闭 (地址={addr})");
            UpdateOutputProperty(index, false);
        }
        catch (Exception ex)
        {
            WriteLog($"关闭输出 {index} 失败：{ex.Message}");
        }
    }

    /// <summary>打开所有输出</summary>
    [RelayCommand]
    private void TurnOnAll()
    {
        var mb = _modbus;
        if (mb == null) return;

        try
        {
            mb.WriteCoils(SlaveAddress, 0, [0xFF00, 0xFF00, 0xFF00, 0xFF00, 0xFF00, 0xFF00, 0xFF00, 0xFF00]);
            WriteLog("所有输出已打开");
            for (var i = 1; i <= 8; i++) UpdateOutputProperty(i.ToString(), true);
        }
        catch (Exception ex)
        {
            WriteLog($"打开所有输出失败：{ex.Message}");
        }
    }

    /// <summary>关闭所有输出</summary>
    [RelayCommand]
    private void TurnOffAll()
    {
        var mb = _modbus;
        if (mb == null) return;

        try
        {
            mb.WriteCoils(SlaveAddress, 0, [0, 0, 0, 0, 0, 0, 0, 0]);
            WriteLog("所有输出已关闭");
            for (var i = 1; i <= 8; i++) UpdateOutputProperty(i.ToString(), false);
        }
        catch (Exception ex)
        {
            WriteLog($"关闭所有输出失败：{ex.Message}");
        }
    }

    /// <summary>读取所有输入</summary>
    [RelayCommand]
    private void ReadInputs()
    {
        var mb = _modbus;
        if (mb == null) return;

        try
        {
            var data = mb.ReadDiscrete(SlaveAddress, 0x0100, 8);
            if (data == null || data.Length == 0)
            {
                WriteLog("读取输入失败：无返回数据");
                return;
            }

            for (var i = 0; i < Math.Min(8, data.Length); i++)
            {
                UpdateInputProperty((i + 1).ToString(), data[i]);
            }
            var bits = String.Join(" ", data.Select(e => e ? "1" : "0"));
            WriteLog($"输入已读取：{bits}");
        }
        catch (Exception ex)
        {
            WriteLog($"读取输入失败：{ex.Message}");
        }
    }

    /// <summary>读取所有输出状态</summary>
    [RelayCommand]
    private void ReadOutputs()
    {
        var mb = _modbus;
        if (mb == null) return;

        try
        {
            var data = mb.ReadCoil(SlaveAddress, 0, 8);
            if (data == null || data.Length == 0)
            {
                WriteLog("读取输出失败：无返回数据");
                return;
            }

            for (var i = 0; i < Math.Min(8, data.Length); i++)
            {
                UpdateOutputProperty((i + 1).ToString(), data[i]);
            }
            var bits = String.Join(" ", data.Select(e => e ? "1" : "0"));
            WriteLog($"输出已读取：{bits}");
        }
        catch (Exception ex)
        {
            WriteLog($"读取输出失败：{ex.Message}");
        }
    }

    private void UpdateOutputProperty(String index, Boolean value)
    {
        switch (index)
        {
            case "1": Output1 = value; break;
            case "2": Output2 = value; break;
            case "3": Output3 = value; break;
            case "4": Output4 = value; break;
            case "5": Output5 = value; break;
            case "6": Output6 = value; break;
            case "7": Output7 = value; break;
            case "8": Output8 = value; break;
        }
    }

    private void UpdateInputProperty(String index, Boolean value)
    {
        switch (index)
        {
            case "1": Input1 = value; break;
            case "2": Input2 = value; break;
            case "3": Input3 = value; break;
            case "4": Input4 = value; break;
            case "5": Input5 = value; break;
            case "6": Input6 = value; break;
            case "7": Input7 = value; break;
            case "8": Input8 = value; break;
        }
    }
    #endregion

    #region 日志
    /// <summary>清空日志</summary>
    [RelayCommand]
    private void ClearLog()
    {
        WriteLog("__CLEAR__");
    }

    private void WriteLog(String msg)
    {
        OnLog?.Invoke(msg);
        if (msg == "__CLEAR__")
            Log = "";
        else
            Log += msg + "\r\n";
    }

    private class IoControlLog : Logger
    {
        private readonly IoControlViewModel _vm;
        public IoControlLog(IoControlViewModel vm) => _vm = vm;
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
