using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Data;
using NewLife.IoT.Protocols;
using NewLife.Log;
using NewLife.Net;
using NewLife.Net.Handlers;
using NewLife.Security;
using NewLife.Threading;

namespace XCoderAv.ViewModels;

/// <summary>Modbus TCP 主站/从站模式</summary>
public enum ModbusTcpMode
{
    /// <summary>主站（Master）</summary>
    Master,

    /// <summary>从站（Slave）</summary>
    Slave
}

/// <summary>寄存器单元</summary>
public class RegisterUnit
{
    /// <summary>寄存器地址</summary>
    public Int32 Address { get; set; }

    /// <summary>寄存器数值</summary>
    public UInt16 Value { get; set; }

    /// <summary>十六进制显示</summary>
    public String Hex => Value.GetBytes(false).ToHex();
}

/// <summary>线圈单元</summary>
public class CoilUnit
{
    /// <summary>线圈地址</summary>
    public Int32 Address { get; set; }

    /// <summary>线圈数值</summary>
    public Byte Value { get; set; }
}

/// <summary>Modbus TCP 工具 ViewModel</summary>
public partial class ModbusTcpViewModel : ObservableObject, IDisposable
{
    #region 属性
    private ModbusTcp? _modbus;
    private NetServer? _server;
    private TimerX? _timer;
    private List<RegisterUnit> _regs = [];
    private List<CoilUnit> _coils = [];

    /// <summary>功能码列表</summary>
    public ObservableCollection<String> FuncCodeOptions { get; } =
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

    /// <summary>工作模式列表</summary>
    public ObservableCollection<String> ModeOptions { get; } = ["主站（Master）", "从站（Slave）"];

    /// <summary>寄存器数据模式列表</summary>
    public ObservableCollection<String> DataModeOptions { get; } = ["0x0000", "0x7777", "0xFFFF", "递增", "静态随机", "动态随机"];

    /// <summary>工作模式（0=Master, 1=Slave）</summary>
    [ObservableProperty]
    private Int32 _selectedMode;

    /// <summary>服务器地址</summary>
    [ObservableProperty]
    private String _serverAddress = "127.0.0.1";

    /// <summary>端口</summary>
    [ObservableProperty]
    private Int32 _port = 502;

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
    private String _connectButtonText = "打开";

    /// <summary>读取结果</summary>
    [ObservableProperty]
    private String _readResult = "";

    /// <summary>从站数据模式索引</summary>
    [ObservableProperty]
    private Int32 _dataModeIndex;

    /// <summary>从站数据地址</summary>
    [ObservableProperty]
    private UInt16 _slaveDataAddress;

    /// <summary>从站数据数量</summary>
    [ObservableProperty]
    private UInt16 _slaveDataCount = 100;

    /// <summary>寄存器列表（从站模式）</summary>
    public ObservableCollection<RegisterUnit> Registers { get; } = [];

    /// <summary>线圈列表（从站模式）</summary>
    public ObservableCollection<CoilUnit> Coils { get; } = [];

    /// <summary>是否从站模式</summary>
    public Boolean IsSlaveMode => SelectedMode == 1;

    /// <summary>是否主站模式</summary>
    public Boolean IsMasterMode => SelectedMode == 0;

    /// <summary>日志文本</summary>
    [ObservableProperty]
    private String _log = "";

    /// <summary>日志回调</summary>
    public Action<String>? OnLog { get; set; }
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
            if (SelectedMode == 0)
                ConnectMaster();
            else
                ConnectSlave();
        }
        catch (Exception ex)
        {
            WriteLog($"连接失败：{ex.Message}");
        }
    }

    private void ConnectMaster()
    {
        _modbus?.Dispose();

        var mb = new ModbusTcp
        {
            Server = $"{ServerAddress}:{Port}",
            Log = new ModbusTcpLog(this),
        };
        mb.Open();

        _modbus = mb;
        IsConnected = true;
        ConnectButtonText = "断开";

        WriteLog($"Modbus TCP 主站已连接 {ServerAddress}:{Port}");
    }

    private void ConnectSlave()
    {
        _server?.Dispose();

        var svr = new NetServer(Port)
        {
            Log = new ModbusTcpLog(this),
            SessionLog = new ModbusTcpLog(this),
        };
        svr.Add(new LengthFieldCodec { Offset = 4, Size = -2 });
        svr.Received += OnSlaveReceived;
        svr.Start();

        _server = svr;
        _regs = [];
        _coils = [];
        Registers.Clear();
        Coils.Clear();

        RefreshSlaveData();

        IsConnected = true;
        ConnectButtonText = "停止";

        WriteLog($"Modbus TCP 从站已启动，监听端口 {Port}");
    }

    private void Disconnect()
    {
        if (_modbus != null)
        {
            try { _modbus.Dispose(); } catch { }
            _modbus = null;
        }

        if (_server != null)
        {
            try { _server.Dispose(); } catch { }
            _server = null;
        }

        _timer.TryDispose();
        _timer = null;

        IsConnected = false;
        ConnectButtonText = "打开";

        WriteLog("连接已关闭");
    }
    #endregion

    #region 主站操作
    /// <summary>执行读写操作</summary>
    [RelayCommand]
    private void Execute()
    {
        var mb = _modbus;
        if (mb == null || SelectedMode != 0)
        {
            WriteLog("主站未连接");
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
    #endregion

    #region 从站操作
    private void OnSlaveReceived(Object sender, ReceivedEventArgs e)
    {
        // 从站收到请求，自动响应
    }

    /// <summary>刷新从站数据</summary>
    [RelayCommand]
    private void RefreshSlaveData()
    {
        Registers.Clear();
        Coils.Clear();

        _regs = [];
        _coils = [];

        for (var i = 0; i < SlaveDataCount; i++)
        {
            var addr = SlaveDataAddress + i;
            var value = DataModeIndex switch
            {
                1 => (UInt16)0x7777,
                2 => (UInt16)0xFFFF,
                3 => (UInt16)(addr % 65536),
                4 => (UInt16)Rand.Next(65536),
                _ => (UInt16)0,
            };

            _regs.Add(new RegisterUnit { Address = addr, Value = value });
            Registers.Add(new RegisterUnit { Address = addr, Value = value });
            _coils.Add(new CoilUnit { Address = addr, Value = (Byte)(addr % 2) });
            Coils.Add(new CoilUnit { Address = addr, Value = (Byte)(addr % 2) });
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

    private class ModbusTcpLog : Logger
    {
        private readonly ModbusTcpViewModel _vm;
        public ModbusTcpLog(ModbusTcpViewModel vm) => _vm = vm;
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
