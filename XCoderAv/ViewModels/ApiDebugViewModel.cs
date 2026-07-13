using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Net;
using NewLife.Remoting;
using NewLife.Threading;

namespace XCoderAv.ViewModels;

/// <summary>API 调试工具 ViewModel</summary>
public partial class ApiDebugViewModel : ObservableObject
{
    #region 属性
    private ApiServer? _server;
    private ApiClient? _client;
    private TimerX? _timer;

    /// <summary>工作模式列表</summary>
    public ObservableCollection<String> Modes { get; } = ["服务端", "客户端"];

    /// <summary>选中模式</summary>
    [ObservableProperty]
    private Int32 _selectedModeIndex;

    /// <summary>地址</summary>
    [ObservableProperty]
    private String _address = "tcp://127.0.0.1:8080";

    /// <summary>端口</summary>
    [ObservableProperty]
    private Int32 _port = 8080;

    /// <summary>是否已连接</summary>
    [ObservableProperty]
    private Boolean _isConnected;

    /// <summary>连接按钮文本</summary>
    [ObservableProperty]
    private String _connectButtonText = "打开";

    /// <summary>发送内容</summary>
    [ObservableProperty]
    private String _sendText = "";

    /// <summary>API 动作列表</summary>
    public ObservableCollection<String> ApiActions { get; } = [];

    /// <summary>选中动作</summary>
    [ObservableProperty]
    private String _selectedAction = "";

    /// <summary>接收日志</summary>
    [ObservableProperty]
    private String _receiveLog = "";

    /// <summary>显示应用日志</summary>
    [ObservableProperty]
    private Boolean _showLog = true;

    /// <summary>显示编码日志</summary>
    [ObservableProperty]
    private Boolean _showEncoderLog;

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

        var port = Port;
        var uri = new NetUri(Address);
        var log = CreateLog();

        try
        {
            if (SelectedModeIndex == 0) // 服务端
            {
                var svr = new ApiServer(port)
                {
                    Log = ShowLog ? log : Logger.Null,
                    EncoderLog = ShowEncoderLog ? log : Logger.Null
                };

                if (ShowSend || ShowReceive)
                {
                    svr.EnsureCreate();
                    if (svr.Server is NetServer ns)
                    {
                        ns.Log = log;
                        ns.LogSend = ShowSend;
                        ns.LogReceive = ShowReceive;
                    }
                }

                svr.Start();

                AppendLog($"正在监听 {port}");
                _server = svr;
            }
            else // 客户端
            {
                var client = new ApiClient(uri + "")
                {
                    Log = ShowLog ? log : Logger.Null,
                    EncoderLog = ShowEncoderLog ? log : Logger.Null
                };

                _client = client;
                client.Open();

                GetApiAll();

                AppendLog($"已连接服务器 {uri}");
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

    private async void GetApiAll()
    {
        try
        {
            var apis = await _client!.InvokeAsync<String[]>("Api/All");
            if (apis != null)
            {
                ApiActions.Clear();
                foreach (var item in apis)
                {
                    ApiActions.Add(item);
                }
                if (ApiActions.Count > 0) SelectedAction = ApiActions[0];
            }
        }
        catch (Exception ex)
        {
            AppendLog($"获取 API 列表失败: {ex.Message}");
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

    /// <summary>发送请求</summary>
    [RelayCommand]
    private async void Send()
    {
        var str = SendText;
        if (str.IsNullOrEmpty()) return;

        if (_client == null)
        {
            AppendLog("客户端未连接");
            return;
        }

        var act = SelectedAction;
        if (act.IsNullOrEmpty()) return;

        try
        {
            AppendLog($"> {act}: {str}");

            Object args = str;
            var result = await _client.InvokeAsync<Object>(act, args);
            AppendLog($"< {result}");
        }
        catch (Exception ex)
        {
            AppendLog($"请求失败: {ex.Message}");
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
