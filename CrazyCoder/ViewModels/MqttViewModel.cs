#nullable enable
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.MQTT;
using NewLife.MQTT.Messaging;
using NewLife.Net;

namespace CrazyCoder.ViewModels;

/// <summary>MQTT 客户端 ViewModel</summary>
public partial class MqttViewModel : ObservableObject, IDisposable
{
    #region 属性
    private MqttClient? _client;
    private readonly ILog _log;

    /// <summary>服务器地址</summary>
    [ObservableProperty]
    private String _server = "127.0.0.1:1883";

    /// <summary>客户端标识</summary>
    [ObservableProperty]
    private String _clientId = "";

    /// <summary>用户名</summary>
    [ObservableProperty]
    private String _userName = "";

    /// <summary>密码</summary>
    [ObservableProperty]
    private String _password = "";

    /// <summary>是否已连接</summary>
    [ObservableProperty]
    private Boolean _isConnected;

    /// <summary>连接按钮文本</summary>
    [ObservableProperty]
    private String _connectButtonText = "连接";

    /// <summary>订阅主题</summary>
    [ObservableProperty]
    private String _subscribeTopic = "";

    /// <summary>订阅 QoS</summary>
    [ObservableProperty]
    private Int32 _subscribeQos;

    /// <summary>发布主题</summary>
    [ObservableProperty]
    private String _publishTopic = "";

    /// <summary>发布内容</summary>
    [ObservableProperty]
    private String _publishText = "";

    /// <summary>发布 QoS</summary>
    [ObservableProperty]
    private Int32 _publishQos;

    /// <summary>保留消息</summary>
    [ObservableProperty]
    private Boolean _retain;

    /// <summary>QoS 选项</summary>
    public String[] QosOptions { get; } = ["0", "1", "2"];

    /// <summary>日志文本</summary>
    [ObservableProperty]
    private String _logText = "";

    /// <summary>日志回调</summary>
    public Action<String>? OnLog { get; set; }
    #endregion

    #region 构造
    /// <summary>实例化 MQTT 客户端 ViewModel</summary>
    public MqttViewModel()
    {
        ClientId = Environment.MachineName;
        SubscribeTopic = "topic/test";
        PublishTopic = "topic/test";
        PublishText = "Hello MQTT";

        _log = new MqttLog(this);
    }
    #endregion

    #region 连接/断开
    /// <summary>切换连接/断开</summary>
    [RelayCommand]
    private async Task ToggleConnect()
    {
        if (IsConnected)
            await Disconnect();
        else
            await Connect();
    }

    private async Task Connect()
    {
        _client = null;

        var remote = Server;
        var uri = new NetUri(remote);
        if (uri.Type == NetType.Unknown) uri.Type = NetType.Tcp;
        if (uri.Port == 0) uri.Port = 1883;

        var client = new MqttClient
        {
            Server = $"{uri}",
            ClientId = ClientId,
            UserName = UserName,
            Password = Password,
            Log = _log,
        };
        client.Received += OnReceived;
        client.Connected += (s, e) => WriteLog("连接成功");
        client.Disconnected += (s, e) => WriteLog("连接断开");

        try
        {
            await client.ConnectAsync();
            _client = client;

            IsConnected = true;
            ConnectButtonText = "断开";

            WriteLog("已连接服务器");
        }
        catch (Exception ex)
        {
            WriteLog($"连接失败：{ex.Message}");
            client.Dispose();
        }
    }

    private async Task Disconnect()
    {
        if (_client != null)
        {
            _client.Reconnect = false;
            try
            {
                await _client.DisconnectAsync();
            }
            catch (Exception ex)
            {
                WriteLog($"断开异常：{ex.Message}");
            }
            _client.Dispose();
            _client = null;
        }

        IsConnected = false;
        ConnectButtonText = "连接";
        WriteLog("已断开连接");
    }

    private void OnReceived(Object? sender, EventArgs<PublishMessage> e)
    {
        var msg = e.Arg;
        var payload = msg.Payload?.ToStr() ?? "";
        WriteLog($"[{msg.Topic}] {payload}");
    }
    #endregion

    #region 订阅/发布
    /// <summary>订阅主题</summary>
    [RelayCommand]
    private async Task Subscribe()
    {
        if (_client == null) return;

        var topic = SubscribeTopic;
        if (topic.IsNullOrEmpty())
        {
            MessageBox.Show("请输入订阅主题！", "提示");
            return;
        }

        var qos = (QualityOfService)SubscribeQos;

        try
        {
            var rs = await _client.SubscribeAsync([topic], qos);
            WriteLog($"订阅成功，Id={rs?.Id}");
        }
        catch (Exception ex)
        {
            WriteLog($"订阅失败：{ex.Message}");
        }
    }

    /// <summary>发布消息</summary>
    [RelayCommand]
    private async Task Publish()
    {
        if (_client == null) return;

        var topic = PublishTopic;
        var str = PublishText;

        if (topic.IsNullOrEmpty())
        {
            MessageBox.Show("请输入发布主题！", "提示");
            return;
        }
        if (str.IsNullOrEmpty())
        {
            MessageBox.Show("请输入发布内容！", "提示");
            return;
        }

        var qos = (QualityOfService)PublishQos;

        try
        {
            WriteLog(str);
            var rs = await _client.PublishAsync(topic, str, qos);
            if (rs != null)
                WriteLog($"发布成功，Id={rs.Id}");
        }
        catch (Exception ex)
        {
            WriteLog($"发布失败：{ex.Message}");
        }
    }

    /// <summary>清空日志</summary>
    [RelayCommand]
    private void ClearLog()
    {
        LogText = "";
        OnLog?.Invoke("__CLEAR__");
    }
    #endregion

    #region 日志
    private void WriteLog(String msg)
    {
        OnLog?.Invoke(msg);
    }

    private class MqttLog : Logger
    {
        private readonly MqttViewModel _vm;
        public MqttLog(MqttViewModel vm) => _vm = vm;
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
        if (_client != null)
        {
            _client.Reconnect = false;
            try { _client.DisconnectAsync().GetAwaiter().GetResult(); } catch { }
            _client.Dispose();
            _client = null;
        }
        GC.SuppressFinalize(this);
    }
    #endregion
}
