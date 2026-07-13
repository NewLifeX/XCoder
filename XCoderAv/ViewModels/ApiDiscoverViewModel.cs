using System.Collections.ObjectModel;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Log;
using NewLife.Net;
using NewLife.Remoting;
using NewLife.Serialization;
using XCoderAv.Models;

namespace XCoderAv.ViewModels;

/// <summary>API 服务发现 ViewModel</summary>
public partial class ApiDiscoverViewModel : ObservableObject
{
    #region 属性
    /// <summary>端口</summary>
    [ObservableProperty]
    private Int32 _port = 5500;

    /// <summary>发现的服务列表</summary>
    public ObservableCollection<ApiServiceInfo> Services { get; } = [];

    /// <summary>日志输出</summary>
    [ObservableProperty]
    private String _log = "";

    /// <summary>是否正在扫描</summary>
    [ObservableProperty]
    private Boolean _isScanning;
    #endregion

    #region 扫描
    /// <summary>扫描网络中的 API 服务</summary>
    [RelayCommand]
    private async void Scan()
    {
        if (IsScanning) return;

        IsScanning = true;
        Services.Clear();
        AppendLog($"开始在端口 {Port} 上扫描 API 服务...");

        try
        {
            var port = Port;
            var ts = new List<Task>();

            var ep = new IPEndPoint(IPAddress.Broadcast, port);
            ts.Add(Task.Run(() => DiscoverUdp(null, ep)));

            foreach (var ip in NetHelper.GetIPs())
            {
                var ep2 = new IPEndPoint(IPAddress.Broadcast, port);
                ts.Add(Task.Run(() => DiscoverUdp(ip, ep2)));
            }

            await Task.WhenAll(ts);
            AppendLog($"扫描完成，发现 {Services.Count} 个服务");
        }
        catch (Exception ex)
        {
            AppendLog($"扫描出错: {ex.Message}");
        }
        finally
        {
            IsScanning = false;
        }
    }

    private async Task DiscoverUdp(IPAddress? local, IPEndPoint ep)
    {
        try
        {
            var client = new ApiClient($"udp://{ep.Address}:{ep.Port}");
            if (local != null) client.Local = new NetUri { Address = local };
            client.Received += OnReceived;

            await client.InvokeAsync<Object>("Api/Info");
            await Task.Delay(1000);
        }
        catch
        {
        }
    }

    private void OnReceived(Object? sender, ApiReceivedEventArgs e)
    {
        if (e.Message == null || !e.Message.Reply) return;

        var msg = e.ApiMessage;
        var client = sender as ApiClient;
        var enc = client?.Encoder;
        if (enc == null) return;

        try
        {
            var result = enc.DecodeResult(msg.Action, msg.Data, e.Message, typeof(Object));
            var remote = (e.UserState as ReceivedEventArgs)?.Remote;

            if (msg.Action == "Api/Info" && result != null)
            {
                var json = result.ToJson();
                AppendLog($"发现服务: {remote} -> {json}");
                Services.Add(new ApiServiceInfo
                {
                    Name = remote?.Address + "",
                    RemoteIP = remote?.Address + "",
                    Port = remote?.Port ?? 0,
                });
            }
        }
        catch
        {
        }
    }

    /// <summary>清空日志</summary>
    [RelayCommand]
    private void ClearLog()
    {
        Log = "";
    }

    /// <summary>清空服务列表</summary>
    [RelayCommand]
    private void ClearServices()
    {
        Services.Clear();
    }

    private void AppendLog(String msg)
    {
        Log += $"{msg}\r\n";
    }
    #endregion
}
