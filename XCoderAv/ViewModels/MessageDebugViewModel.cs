using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Net;
using NewLife.Threading;

namespace XCoderAv.ViewModels;

/// <summary>消息调试工具 ViewModel</summary>
public partial class MessageDebugViewModel : ObservableObject
{
    #region 属性
    private NetServer? _server;
    private ISocketClient? _client;
    private TimerX? _timer;

    /// <summary>工作模式列表</summary>
    public ObservableCollection<String> Modes { get; } = ["服务端", "客户端"];

    /// <summary>选中模式</summary>
    [ObservableProperty]
    private Int32 _selectedModeIndex;

    /// <summary>地址</summary>
    [ObservableProperty]
    private String _address = "tcp://127.0.0.1:8888";

    /// <summary>端口</summary>
    [ObservableProperty]
    private Int32 _port = 8888;

    /// <summary>是否已连接</summary>
    [ObservableProperty]
    private Boolean _isConnected;

    /// <summary>连接按钮文本</summary>
    [ObservableProperty]
    private String _connectButtonText = "打开";

    /// <summary>发送内容</summary>
    [ObservableProperty]
    private String _sendText = "";

    /// <summary>HEX 发送</summary>
    [ObservableProperty]
    private Boolean _hexSend;

    /// <summary>接收日志</summary>
    [ObservableProperty]
    private String _receiveLog = "";

    /// <summary>显示应用日志</summary>
    [ObservableProperty]
    private Boolean _showLog = true;

    /// <summary>显示网络日志</summary>
    [ObservableProperty]
    private Boolean _showSocketLog = true;

    /// <summary>显示接收字符串</summary>
    [ObservableProperty]
    private Boolean _showReceiveString = true;

    /// <summary>显示发送数据</summary>
    [ObservableProperty]
    private Boolean _showSend;

    /// <summary>显示接收数据</summary>
    [ObservableProperty]
    private Boolean _showReceive;

    /// <summary>显示统计信息</summary>
    [ObservableProperty]
    private Boolean _showStat;

    /// <summary>发送次数</summary>
    [ObservableProperty]
    private Int32 _sendTimes = 1;

    /// <summary>发送间隔(ms)</summary>
    [ObservableProperty]
    private Int32 _sendSleep = 1000;

    /// <summary>并发数</summary>
    [ObservableProperty]
    private Int32 _sendThreads = 1;
    #endregion

    #region 连接/断开
    /// <summary>切换连接状态</summary>
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
        _server = null;
        _client = null;
        _timer = null;

        var uri = new NetUri(Address);
        var log = CreateLog();

        try
        {
            if (SelectedModeIndex == 0) // 服务端
            {
                var svr = new NetServer();
                svr.Log = ShowLog ? log : Logger.Null;
                svr.SocketLog = ShowSocketLog ? log : Logger.Null;
                svr.Port = uri.Port;
                if (uri.IsTcp || uri.IsUdp) svr.ProtocolType = uri.Type;

                svr.LogSend = ShowSend;
                svr.LogReceive = ShowReceive;

                svr.Received += OnReceived;
                svr.Start();

                AppendLog($"正在监听 {svr.Port}");
                _server = svr;
            }
            else // 客户端
            {
                var client = uri.CreateRemote();
                client.Log = ShowLog ? log : Logger.Null;

                client.LogSend = ShowSend;
                client.LogReceive = ShowReceive;

                client.Received += OnReceived;
                client.Open();

                AppendLog($"已连接服务器 {uri}");
                _client = client;
            }

            IsConnected = true;
            ConnectButtonText = "关闭";
            _timer = new TimerX(RefreshStat, null, 5000, 5000) { Async = true };
        }
        catch (Exception ex)
        {
            AppendLog($"连接失败: {ex.Message}");
        }
    }

    private void Disconnect()
    {
        _client?.Dispose();
        _client = null;

        if (_server != null)
        {
            _server.Dispose();
            _server = null;
        }

        _timer?.Dispose();
        _timer = null;

        IsConnected = false;
        ConnectButtonText = "打开";
        AppendLog("已断开连接");
    }

    private void OnReceived(Object? sender, ReceivedEventArgs e)
    {
        if (ShowReceiveString && e.Packet != null)
        {
            var line = e.Packet.ToStr();
            AppendLog(line);
        }
    }

    /// <summary>发送消息</summary>
    [RelayCommand]
    private void Send()
    {
        var str = SendText;
        if (str.IsNullOrEmpty()) return;

        var buf = HexSend ? str.ToHex() : str.GetBytes();
        var pk = new ArrayPacket(buf);

        if (_client != null)
        {
            try
            {
                for (var i = 0; i < SendTimes; i++)
                {
                    _client.Send(pk);
                    if (i < SendTimes - 1 && SendSleep > 0)
                        Thread.Sleep(SendSleep);
                }
                AppendLog($"发送: {str}");
            }
            catch (Exception ex)
            {
                AppendLog($"发送失败: {ex.Message}");
            }
        }
        else
        {
            AppendLog("客户端未连接");
        }
    }

    /// <summary>清空日志</summary>
    [RelayCommand]
    private void ClearLog()
    {
        ReceiveLog = "";
    }

    private void RefreshStat(Object? state)
    {
    }

    private void AppendLog(String msg)
    {
        ReceiveLog += $"{msg}\r\n";
    }

    private ILog CreateLog()
    {
        return new SimpleLog { WriteAction = msg => AppendLog(msg) };
    }
    #endregion
}
