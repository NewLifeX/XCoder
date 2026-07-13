using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Threading;

namespace XCoderAv.ViewModels;

/// <summary>串口调试工具 ViewModel</summary>
public partial class SerialPortViewModel : ObservableObject, IDisposable
{
    #region 属性
    private SerialPort? _port;
    private TimerX? _timer;

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

    /// <summary>十六进制显示</summary>
    [ObservableProperty]
    private Boolean _hexDisplay;

    /// <summary>十六进制发送</summary>
    [ObservableProperty]
    private Boolean _hexSend;

    /// <summary>是否已连接</summary>
    [ObservableProperty]
    private Boolean _isConnected;

    /// <summary>连接按钮文本</summary>
    [ObservableProperty]
    private String _connectButtonText = "打开";

    /// <summary>发送文本</summary>
    [ObservableProperty]
    private String _sendText = "";

    /// <summary>已发送字节数</summary>
    [ObservableProperty]
    private Int64 _bytesSent;

    /// <summary>已接收字节数</summary>
    [ObservableProperty]
    private Int64 _bytesReceived;

    /// <summary>日志文本</summary>
    [ObservableProperty]
    private String _log = "";

    /// <summary>日志回调，由 View 绑定</summary>
    public Action<String>? OnLog { get; set; }
    #endregion

    #region 构造
    /// <summary>实例化串口调试工具 ViewModel</summary>
    public SerialPortViewModel()
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

            var port = new SerialPort(PortName, BaudRate, parity, DataBits, stopBits);
            port.DataReceived += OnDataReceived;
            port.ErrorReceived += OnErrorReceived;
            port.Open();

            _port = port;

            IsConnected = true;
            ConnectButtonText = "关闭";

            WriteLog($"串口 {PortName} 已打开，{BaudRate}/{DataBits}/{ParityOptions[ParityIndex]}/{StopBitsOptions[StopBitsIndex]}");

            BytesSent = 0;
            BytesReceived = 0;

            _timer = new TimerX(OnTimer, null, 1000, 1000) { Async = true };
        }
        catch (Exception ex)
        {
            WriteLog($"打开失败：{ex.Message}");
        }
    }

    private void Disconnect()
    {
        _timer.TryDispose();
        _timer = null;

        if (_port != null)
        {
            try
            {
                if (_port.IsOpen) _port.Close();
            }
            catch (Exception ex)
            {
                WriteLog($"关闭异常：{ex.Message}");
            }
            _port.Dispose();
            _port = null;
        }

        IsConnected = false;
        ConnectButtonText = "打开";

        WriteLog($"串口 {PortName} 已关闭");
    }

    private void OnTimer(Object? state)
    {
        var port = _port;
        if (port == null || !port.IsOpen)
        {
            if (IsConnected)
            {
                WriteLog("串口已断开");
                Disconnect();
            }
            return;
        }
    }

    private void OnDataReceived(Object sender, SerialDataReceivedEventArgs e)
    {
        var port = _port;
        if (port == null) return;

        try
        {
            var count = port.BytesToRead;
            if (count <= 0) return;

            var buf = new Byte[count];
            var n = port.Read(buf, 0, count);
            if (n <= 0) return;

            BytesReceived += n;

            var hex = buf.ToHex(" ", 0, n);
            var str = Encoding.UTF8.GetString(buf, 0, n);

            if (HexDisplay)
                WriteLog($"[接收] [{n}字节] {hex}");
            else
                WriteLog($"[接收] [{n}字节] {str}");
        }
        catch (Exception ex)
        {
            WriteLog($"接收异常：{ex.Message}");
        }
    }

    private void OnErrorReceived(Object sender, SerialErrorReceivedEventArgs e)
    {
        WriteLog($"串口错误：{e.EventType}");
    }
    #endregion

    #region 发送
    /// <summary>发送数据</summary>
    [RelayCommand]
    private void Send()
    {
        var port = _port;
        if (port == null || !port.IsOpen)
        {
            WriteLog("串口未打开");
            return;
        }

        var str = SendText;
        if (String.IsNullOrEmpty(str))
        {
            WriteLog("发送内容不能为空！");
            return;
        }

        try
        {
            if (HexSend)
            {
                var hex = str.Replace(" ", "").Replace("-", "");
                if (hex.Length % 2 != 0) hex = "0" + hex;
                var buf = hex.ToHex();
                if (buf.Length > 0)
                {
                    port.Write(buf, 0, buf.Length);
                    BytesSent += buf.Length;
                    WriteLog($"[发送] [{buf.Length}字节] {buf.ToHex(" ", 0, buf.Length)}");
                }
            }
            else
            {
                var data = Encoding.UTF8.GetBytes(str);
                port.Write(data, 0, data.Length);
                BytesSent += data.Length;
                WriteLog($"[发送] [{data.Length}字节] {str}");
            }
        }
        catch (Exception ex)
        {
            WriteLog($"发送失败：{ex.Message}");
        }
    }

    /// <summary>清空日志</summary>
    [RelayCommand]
    private void ClearLog()
    {
        WriteLog("__CLEAR__");
    }
    #endregion

    #region 日志
    private void WriteLog(String msg)
    {
        OnLog?.Invoke(msg);
        if (msg == "__CLEAR__")
            Log = "";
        else
            Log += msg + "\r\n";
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
