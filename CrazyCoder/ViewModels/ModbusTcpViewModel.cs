#nullable enable

using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Buffers;
using NewLife.Data;
using NewLife.IoT.Protocols;
using NewLife.Log;
using NewLife.Messaging;
using NewLife.Net;
using NewLife.Security;
using NewLife.Threading;

namespace CrazyCoder.ViewModels;

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
            MessageBox.Show($"连接失败：{ex.Message}", "错误");
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
        svr.Protocol = new LengthFieldCodec { Offset = 4, Size = -2 };
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

    private void ShowResult(String title, Byte[]? data)
    {
        if (data == null || data.Length == 0)
        {
            WriteLog($"{title}：无返回数据");
            ReadResult = "无返回数据";
            return;
        }

        var hex = data.ToHex(" ", 0, data.Length);
        WriteLog($"{title} [{data.Length}字节] {hex}");
        ReadResult = hex;
    }
    #endregion

    #region 从站处理
    private void RefreshSlaveData()
    {
        var addr = SlaveDataAddress;
        var count = SlaveDataCount;
        var modeIndex = DataModeIndex;

        Registers.Clear();
        Coils.Clear();

        // 从站使用寄存器模式（默认）
        for (var i = 0; i < count; i++)
        {
            var value = modeIndex switch
            {
                0 => (UInt16)0x0000,
                1 => (UInt16)0x7777,
                2 => (UInt16)0xFFFF,
                3 => (UInt16)i,
                _ => (UInt16)Rand.Next(UInt16.MaxValue),
            };
            Registers.Add(new RegisterUnit { Address = addr + i, Value = value });
        }

        if (modeIndex == 5) // 动态随机
        {
            _timer = new TimerX(DoRefreshData, null, 1_000, 1_000) { Async = true };
        }

        WriteLog($"从站数据已初始化：{count} 个寄存器");
    }

    private void DoRefreshData(Object? state)
    {
        foreach (var reg in Registers)
        {
            var val = reg.Value;
            if (val == 0)
                val = (UInt16)Rand.Next(UInt16.MaxValue);
            else
            {
                var x = (Rand.Next(75) - 30) / 100.0;
                val = (UInt16)(val * (1 + x));
            }
            reg.Value = val;
        }
    }

    /// <summary>刷新从站数据</summary>
    [RelayCommand]
    private void RefreshData()
    {
        if (!IsConnected || SelectedMode != 1) return;
        RefreshSlaveData();
    }

    private void OnSlaveReceived(Object? sender, ReceivedEventArgs e)
    {
        var session = sender as NetSession;
        if (session == null) return;

        var pk = e.Packet is ArrayPacket ap ? ap : new ArrayPacket(e.Packet.ReadBytes());
        try
        {
            var spanReader = new NewLife.Buffers.SpanReader(pk);
            var msg = new ModbusIpMessage();
            msg.Read(ref spanReader);
            if (msg == null) return;

            session.Log?.Info("<= {0}", msg);

            var addrMsg = msg.GetAddress();
            var addrAddress = addrMsg;
            var rs = msg.CreateReply();

            switch (msg.Code)
            {
                case FunctionCodes.ReadCoil:
                case FunctionCodes.ReadDiscrete:
                    // Return coils data
                    {
                        var regCount = msg.Payload.ReadBytes(2, 2).ToUInt16(0, false);
                        var bytesCount = (Int32)Math.Ceiling(regCount / 8.0);
                        var bits = new Byte[1 + bytesCount];
                        bits[0] = (Byte)bytesCount;
                        for (var i = 0; i < bytesCount; i++)
                        {
                            var b = 0;
                            var max = regCount - i * 8;
                            if (max > 8) max = 8;
                            for (var j = 0; j < max; j++)
                            {
                                var idx = addrAddress + i * 8 + j;
                                var coil = Coils.FirstOrDefault(e => e.Address == idx);
                                if (coil != null && coil.Value > 0)
                                    b |= 1 << j;
                            }
                            bits[1 + i] = (Byte)b;
                        }
                        rs.Payload = new ArrayPacket(bits);
                    }
                    break;

                case FunctionCodes.ReadRegister:
                case FunctionCodes.ReadInput:
                    {
                        var regCount = msg.Payload.ReadBytes(2, 2).ToUInt16(0, false);
                        var regs = Registers.Where(e => e.Address >= addrAddress && e.Address < addrAddress + regCount).ToList();
                        if (regs.Count > 0)
                        {
                            var buf2 = regs.SelectMany(e => e.Value.GetBytes(false)).ToArray();
                            rs.Payload = new ArrayPacket(new Byte[] { (Byte)buf2.Length }.Concat(buf2).ToArray());
                        }
                    }
                    break;

                case FunctionCodes.WriteCoil:
                    break;

                case FunctionCodes.WriteRegister:
                    {
                        var value = msg.Payload.ReadBytes(2, 2).ToUInt16(0, false);
                        var reg = Registers.FirstOrDefault(e => e.Address == addrAddress);
                        if (reg != null)
                        {
                            reg.Value = value;
                            Application.Current?.Dispatcher.Invoke(() => { });
                        }
                    }
                    break;

                case FunctionCodes.WriteCoils:
                case FunctionCodes.WriteRegisters:
                    break;
            }

            session.Log?.Info("=> {0}", rs);
            session.Send(rs.ToPacket());
        }
        catch (Exception ex)
        {
            WriteLog($"从站处理异常：{ex.Message}");
        }
    }
    #endregion

    #region 清空日志
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
