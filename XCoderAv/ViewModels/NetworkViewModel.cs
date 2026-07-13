using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Net;
using NewLife.Threading;

namespace XCoderAv.ViewModels;

/// <summary>网络调试工具工作模式</summary>
public enum NetworkWorkMode
{
    /// <summary>TCP/UDP混合</summary>
    UdpTcp = 1,

    /// <summary>UDP服务端</summary>
    UdpServer,

    /// <summary>UDP客户端</summary>
    UdpClient,

    /// <summary>TCP服务端</summary>
    TcpServer,

    /// <summary>TCP客户端</summary>
    TcpClient
}

/// <summary>网络调试工具ViewModel</summary>
public partial class NetworkViewModel : ObservableObject
{
    #region 属性
    private NetServer _server;
    private ISocketClient _client;
    private TimerX _timer;

    /// <summary>工作模式列表</summary>
    public ObservableCollection<String> Modes { get; } =
    [
        "TCP/UDP混合",
        "UDP服务端",
        "UDP客户端",
        "TCP服务端",
        "TCP客户端"
    ];

    /// <summary>本地地址列表</summary>
    public ObservableCollection<String> LocalAddresses { get; } = [];

    [ObservableProperty]
    private Int32 _selectedModeIndex;

    [ObservableProperty]
    private String _localAddress = "";

    [ObservableProperty]
    private String _remoteAddress = "";

    [ObservableProperty]
    private Int32 _port = 8080;

    [ObservableProperty]
    private Boolean _isConnected;

    [ObservableProperty]
    private String _connectButtonText = "打开";

    [ObservableProperty]
    private String _sendText = "新生命开发团队，学无先后达者为师";

    [ObservableProperty]
    private Boolean _hexSend;

    [ObservableProperty]
    private Int32 _sendTimes = 1;

    [ObservableProperty]
    private Int32 _sendSleep = 1000;

    [ObservableProperty]
    private Int32 _sendThreads = 1;

    [ObservableProperty]
    private Boolean _showLog = true;

    [ObservableProperty]
    private Boolean _showSocketLog = true;

    [ObservableProperty]
    private Boolean _showSend = true;

    [ObservableProperty]
    private Boolean _showReceive = true;

    [ObservableProperty]
    private Boolean _showStat = true;

    [ObservableProperty]
    private Boolean _showReceiveString = true;

    [ObservableProperty]
    private Int64 _receivedBytes;

    [ObservableProperty]
    private Int64 _sentBytes;

    [ObservableProperty]
    private Int32 _sessionCount;

    /// <summary>日志回调，由View绑定到TextBox</summary>
    public Action<String> OnLog { get; set; }
    #endregion

    #region 构造
    /// <summary>实例化网络调试工具ViewModel</summary>
    public NetworkViewModel()
    {
        LoadLocalAddresses();
    }

    private void LoadLocalAddresses()
    {
        LocalAddresses.Clear();
        LocalAddresses.Add("所有本地IPv4/IPv6");
        LocalAddresses.Add(IPAddress.Any.ToString());
        LocalAddresses.Add(IPAddress.IPv6Any.ToString());

        try
        {
            var host = System.Net.Dns.GetHostEntry("");
            foreach (var addr in host.AddressList)
            {
                LocalAddresses.Add(addr.ToString());
            }
        }
        catch
        {
        }
    }
    #endregion

    #region 连接/断开
    [RelayCommand]
    private void ToggleConnect()
    {
        if (IsConnected)
        {
            Disconnect();
        }
        else
        {
            Connect();
        }
    }

    private void Connect()
    {
        _server = null;
        _client = null;

        var mode = (NetworkWorkMode)(SelectedModeIndex + 1);
        var local = LocalAddress;
        var remote = RemoteAddress;
        var port = Port;

        switch (mode)
        {
            case NetworkWorkMode.UdpTcp:
                _server = new NetServer();
                break;
            case NetworkWorkMode.UdpServer:
                _server = new NetServer
                {
                    ProtocolType = NetType.Udp
                };
                break;
            case NetworkWorkMode.TcpServer:
                _server = new NetServer
                {
                    ProtocolType = NetType.Tcp
                };
                break;
            case NetworkWorkMode.TcpClient:
                _client = new TcpSession();
                break;
            case NetworkWorkMode.UdpClient:
                _client = new UdpServer();
                break;
        }

        if (_client != null)
        {
            _client.Log = ShowLog ? CreateBizLog() : Logger.Null;
            if (!local.IsNullOrEmpty() && !local.StartsWith("所有本地"))
                _client.Local.Host = local;
            _client.Received += OnReceived;
            _client.Remote.Port = port;
            _client.Remote.Host = remote;

            _client.LogSend = ShowSend;
            _client.LogReceive = ShowReceive;

            if (!_client.Open())
            {
                WriteLog("连接失败");
                return;
            }

            WriteLog("已连接服务器");
        }
        else if (_server != null)
        {
            _server.Log = ShowLog ? CreateBizLog() : Logger.Null;
            _server.SocketLog = ShowSocketLog ? CreateBizLog() : Logger.Null;
            _server.Port = port;
            if (!local.IsNullOrEmpty() && !local.StartsWith("所有本地"))
                _server.Local.Host = local;
            _server.Received += OnReceived;

            _server.LogSend = ShowSend;
            _server.LogReceive = ShowReceive;

            _server.Start();

            WriteLog($"正在监听 {port}");
        }

        IsConnected = true;
        ConnectButtonText = "关闭";

        _timer = new TimerX(OnShowStat, null, 5000, 5000) { Async = true };
    }

    private void Disconnect()
    {
        if (_client != null)
        {
            _client.Dispose();
            _client = null;
            WriteLog("关闭连接");
        }

        if (_server != null)
        {
            WriteLog($"停止监听 {_server.Port}");
            _server.Dispose();
            _server = null;
        }

        _timer?.Dispose();
        _timer = null;

        IsConnected = false;
        ConnectButtonText = "打开";
    }
    #endregion

    #region 收发数据
    private String _lastStat = "";

    private void OnShowStat(Object state)
    {
        if (!ShowStat) return;

        var msg = "";
        if (_server != null)
            msg = _server.GetStat();

        if (!msg.IsNullOrEmpty() && msg != _lastStat)
        {
            _lastStat = msg;
            WriteLog(msg);
        }
    }

    private void OnReceived(Object sender, ReceivedEventArgs e)
    {
        var session = sender as ISocketSession;
        if (session == null)
        {
            if (sender is INetSession ns)
                session = ns.Session;
        }

        if (session == null) return;

        if (ShowReceiveString)
        {
            var line = e.Packet.ToStr();
            WriteLog(line);
        }

        ReceivedBytes += e.Packet.Total;
        SessionCount = _server?.SessionCount ?? (_client != null ? 1 : 0);
    }

    [RelayCommand]
    private async Task Send()
    {
        var str = SendText;
        if (String.IsNullOrEmpty(str))
        {
            WriteLog("发送内容不能为空！");
            return;
        }

        var count = SendTimes;
        var sleep = SendSleep;
        var ths = SendThreads;
        if (count <= 0) count = 1;
        if (sleep <= 0) sleep = 1;

        // 处理换行
        str = str.Replace("\n", "\r\n");
        var buf = HexSend ? str.ToHex() : str.GetBytes();
        var pk = new ArrayPacket(buf);

        if (_client != null)
        {
            if (ths <= 1)
            {
                await SendHelper.SendConcurrency(_client, pk, count, sleep);
            }
            else
            {
                var any = _client.Local.Address.IsAny();
                var list = new List<ISocketClient>();
                for (var i = 0; i < ths; i++)
                {
                    var client = _client.Remote.CreateRemote();
                    if (!any) client.Local.EndPoint = new IPEndPoint(_client.Local.Address, 2000 + i);
                    list.Add(client);
                }

                var tasks = list.Select(c => SendHelper.SendConcurrency(c, pk, count, sleep));
                await Task.WhenAll(tasks);

                foreach (var item in list)
                {
                    item.TryDispose();
                }
            }

            SentBytes += buf.Length * count;
        }
        else if (_server != null)
        {
            WriteLog($"准备向[{_server.SessionCount}]个客户端发送[{count}]次[{buf.Length}]的数据");
            for (var i = 0; i < count && _server != null; i++)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var cs = await _server.SendAllAsync(pk);
                sw.Stop();
                WriteLog($"{i + 1}/{count} 已向[{cs}]个客户端发送[{buf.Length}]数据 {sw.ElapsedMilliseconds}ms");
                if (sleep > 0) await Task.Delay(sleep);
            }

            SentBytes += buf.Length * count * (_server?.SessionCount ?? 0);
        }
    }

    [RelayCommand]
    private void ClearReceiveLog()
    {
        WriteLog("__CLEAR__");
        ReceivedBytes = 0;
    }
    #endregion

    #region 日志
    private ILog CreateBizLog()
    {
        return TextFileLog.Create(null, "Net_{0:yyyy_MM_dd}.log");
    }

    /// <summary>写入日志到UI</summary>
    /// <param name="msg">日志消息</param>
    public void WriteLog(String msg)
    {
        OnLog?.Invoke(msg);
    }
    #endregion
}

/// <summary>异步多次发送数据</summary>
internal static class SendHelper
{
    public static async Task SendConcurrency(ISocketRemote session, IPacket pk, Int32 times, Int32 msInterval)
    {
        await Task.Run(async () =>
        {
            for (var i = 0; i < times; i++)
            {
                session.Send(pk);

                if (msInterval > 0)
                    await Task.Delay(msInterval);
            }
        });
    }
}
