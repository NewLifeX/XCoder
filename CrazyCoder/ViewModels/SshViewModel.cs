using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Net;
using Renci.SshNet;

namespace CrazyCoder.ViewModels;

/// <summary>SSH 工具 ViewModel</summary>
public partial class SshViewModel : ObservableObject
{
    #region 属性
    private SshClient _client;

    /// <summary>远程地址</summary>
    [ObservableProperty]
    private String _remoteAddress = "";

    /// <summary>用户名</summary>
    [ObservableProperty]
    private String _userName = "root";

    /// <summary>密码</summary>
    [ObservableProperty]
    private String _password = "";

    /// <summary>端口</summary>
    [ObservableProperty]
    private Int32 _port = 22;

    /// <summary>是否已连接</summary>
    [ObservableProperty]
    private Boolean _isConnected;

    /// <summary>连接按钮文本</summary>
    [ObservableProperty]
    private String _connectButtonText = "打开";

    /// <summary>发送命令文本</summary>
    [ObservableProperty]
    private String _sendText = "";

    /// <summary>接收日志</summary>
    [ObservableProperty]
    private String _receiveLog = "";
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
        _client?.Dispose();
        _client = null;

        var remote = RemoteAddress;
        if (remote.IsNullOrEmpty()) return;

        var uri = new NetUri(remote);
        if (uri.Type == NetType.Unknown) uri.Type = NetType.Tcp;
        if (uri.Port == 0) uri.Port = Port;

        try
        {
            var client = new SshClient(uri.Host ?? (uri.Address + ""), uri.Port, UserName, Password);
            client.ErrorOccurred += (_, e) => AppendLog($"错误: {e.Exception.Message}");
            client.Connect();

            _client = client;
            IsConnected = true;
            ConnectButtonText = "关闭";
            AppendLog($"已连接到 {uri.Host}:{uri.Port}");
        }
        catch (Exception ex)
        {
            AppendLog($"连接失败: {ex.Message}");
        }
    }

    private void Disconnect()
    {
        if (_client != null)
        {
            _client.Dispose();
            _client = null;
        }

        IsConnected = false;
        ConnectButtonText = "打开";
        AppendLog("已断开连接");
    }

    /// <summary>发送命令</summary>
    [RelayCommand]
    private void Send()
    {
        var str = SendText;
        if (str.IsNullOrEmpty()) return;

        if (_client == null || !_client.IsConnected)
        {
            AppendLog("未连接到服务器");
            return;
        }

        try
        {
            AppendLog($"> {str}");
            var rs = _client.RunCommand(str);
            if (rs != null)
            {
                if (!rs.Result.IsNullOrEmpty()) AppendLog(rs.Result);
                if (!rs.Error.IsNullOrEmpty()) AppendLog($"错误: {rs.Error}");
            }
        }
        catch (Exception ex)
        {
            AppendLog($"命令执行失败: {ex.Message}");
        }
    }

    /// <summary>清空接收日志</summary>
    [RelayCommand]
    private void ClearLog()
    {
        ReceiveLog = "";
    }

    private void AppendLog(String msg)
    {
        ReceiveLog += msg + "\r\n";
    }
    #endregion
}
