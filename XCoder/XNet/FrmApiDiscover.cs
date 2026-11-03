using System.ComponentModel;
using System.Net;
using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Net;
using NewLife.Remoting;
using NewLife.Security;
using Stardust.Models;
using XCoder;
using XCoder.Common;

namespace XNet;

[Category("网络通信")]
[DisplayName("ApiServer搜索")]
public partial class FrmApiDiscover : Form, IXForm
{
    private ControlConfig _config;
    private Int32 _port;

    #region 窗体
    public FrmApiDiscover()
    {
        InitializeComponent();
    }

    private void FrmMain_Load(Object sender, EventArgs e)
    {
        _config = new ControlConfig { Control = this, FileName = "ApiDiscover.json" };
        _config.Load();
    }
    #endregion

    #region 核心操作
    private async void btnConnect_Click(Object sender, EventArgs e)
    {
        _config.Save();

        var btn = sender as Button;
        btn.Enabled = false;
        var port = (Int32)numPort.Value;
        _port = port;
        try
        {
            var ts = new List<Task>();

            {
                var ep = new IPEndPoint(IPAddress.Broadcast, port);
                var task = Task.Run(() => DiscoverUdp(null, ep));
                ts.Add(task);
            }
            foreach (var ip in NetHelper.GetIPs())
            {
                var ep = new IPEndPoint(IPAddress.Broadcast, port);
                var task = Task.Run(() => DiscoverUdp(ip, ep));
                ts.Add(task);
            }

            //await Task.WaitAll(ts.ToArray(), 5_000);
            await Task.WhenAll(ts);
        }
        finally
        {
            btn.Enabled = true;
        }
    }

    async Task DiscoverUdp(IPAddress local, IPEndPoint ep)
    {
        XTrace.WriteLine("DiscoverUdp: {0} -> {1}", local, ep);

        try
        {
            //var uri = new NetUri("udp://255.255.255.255:5500");
            var client = new ApiClient($"udp://{ep.Address}:{ep.Port}");
            if (local != null) client.Local = new NetUri { Address = local };
            client.Received += Client_Received;

            // 异步发送，但是不等待返回，因为可能会有多个返回，在事件里处理
            await client.InvokeAsync<Object>("Api/Info");

            await Task.Delay(1_000);

            //client.InvokeOneWay("Info");

            //// 构建请求
            //var enc = new JsonEncoder();
            //var msg = enc.CreateRequest("Api/Info", null);
            //var req = msg.ToPacket().ReadBytes();

            //var udp = new UdpClient(ep.AddressFamily)
            //{
            //    EnableBroadcast = true
            //};

            //if (local != null) udp.Client.Bind(new IPEndPoint(local, Rand.Next(1000, 60000)));

            //// 发送
            //udp.Send(req, req.Length, ep);

            //// 多次接收
            //while (true)
            //{
            //    try
            //    {
            //        var source = new CancellationTokenSource(3_000);
            //        var rs = await udp.ReceiveAsync(source.Token);
            //        if (rs.Buffer != null)
            //        {
            //            msg = new DefaultMessage();
            //            msg.Read(rs.Buffer);

            //            if (enc.Decode(msg, out var action, out var code, out var data) && code == 0)
            //            {
            //                // 解码结果
            //                var result = enc.DecodeResult(action, data, msg);
            //                XTrace.WriteLine("Receive[{0}] {1}", udp.Client.LocalEndPoint, result.ToJson());

            //                if (enc.Convert(result, typeof(ApiItem)) is ApiItem ai)
            //                {
            //                    ai.RemoteIP = rs.RemoteEndPoint.Address + "";

            //                    Invoke(() => ShowItem(ai));
            //                }
            //            }
            //        }
            //    }
            //    catch (OperationCanceledException)
            //    {
            //        break;
            //    }
            //}
        }
        catch (Exception ex)
        {
            XTrace.WriteException(ex);

            MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Client_Received(Object sender, ApiReceivedEventArgs e)
    {
        if (e.Message == null || !e.Message.Reply) return;

        var msg = e.ApiMessage;
        XTrace.WriteLine("Received [{0}]: {1}", msg.Action, msg.Data.ToStr());

        var client = sender as ApiClient;
        var enc = client.Encoder;

        // 解码结果。属性名与服务端字段保持一致，NewLife.JsonReader 反序列化时忽略大小写，兼容 PascalCase 与 camelCase
        var result = enc.DecodeResult(msg.Action, msg.Data, e.Message, typeof(ApiItem));
        //XTrace.WriteLine("Receive[{0}] {1}", udp.Client.LocalEndPoint, result.ToJson());
        var remote = (e.UserState as ReceivedEventArgs)?.Remote;

        if (msg.Action == "Api/Info" && result is ApiItem ai)
        {
            // 广播会话会把任意来源的回应归到同一个会话上，旧版 NewLife.Core 的事件远程地址只能是广播地址，
            // 此时退化为应答里设备自报的本机 IP（优先与工具同网段），否则所有设备都会显示成同一个广播地址
            var address = remote?.Address;
            if (address == null || address.IsAny() || Equals(address, IPAddress.Broadcast)) address = GetDeviceAddress(ai.LocalIP);

            ai.RemoteIP = address + "";

            if (ai.Name == "StarAgent" && address != null)
            {
                var port = remote?.Port ?? _port;

                //_ = client.InvokeAsync<Object>("Info");
                Task.Run(async () =>
                {
                    using var client2 = new ApiClient($"udp://{new IPEndPoint(address, port)}");
                    var rs = await client2.InvokeAsync<AgentInfo>("Info");
                    if (rs != null)
                    {
                        ai.Code = rs.Code;
                        ai.Address = rs.Server;

                        // 取回编码后回填当前行，否则要等下一次搜索才能看到
                        Invoke(() => ShowItem(ai));
                    }
                });
            }

            if (ai.Id > 0) Invoke(() => ShowItem(ai));
        }
    }

    /// <summary>从设备上报的本机地址列表里挑一个可达地址，优先与工具本机同网段</summary>
    /// <param name="localIPs">设备上报的本机IP列表，逗号分隔</param>
    /// <returns>设备地址，无法解析时返回 null</returns>
    private static IPAddress GetDeviceAddress(String localIPs)
    {
        if (localIPs.IsNullOrEmpty()) return null;

        var ips = localIPs.Split(',', ';').Select(e => e.Trim()).Where(e => IPAddress.TryParse(e, out var ip) && ip.IsIPv4()).ToList();
        if (ips.Count == 0) return null;

        // 优先选择与工具本机网卡同网段的地址
        var locals = NetHelper.GetIPs();
        foreach (var item in ips)
        {
            var ip = IPAddress.Parse(item);
            var buf = ip.GetAddressBytes();
            foreach (var local in locals)
            {
                var lb = local.GetAddressBytes();
                if (lb.Length == buf.Length && buf[0] == lb[0] && buf[1] == lb[1] && buf[2] == lb[2]) return ip;
            }
        }

        return IPAddress.Parse(ips[0]);
    }
    #endregion

    #region 显示
    private IDictionary<String, ApiItem> _data;
    void ShowItem(ApiItem ai)
    {
        _data ??= new Dictionary<String, ApiItem>();

        var key = $"{ai.Name}-{ai.LocalIP}-{ai.Id}";

        if (_data.TryGetValue(key, out var old))
        {
            old.Time = ai.Time;
            old.Code = ai.Code;
            old.Address = ai.Address;

            dgv.Refresh();
        }
        else
        {
            _data.Add(key, ai);

            dgv.DataSource = null;
            dgv.DataSource = _data.Values.ToArray();
        }
    }

    class ApiItem
    {
        public Int32 Id { get; set; }

        public String Name { get; set; }

        public String LocalIP { get; set; }

        public String MachineName { get; set; }

        public String FileVersion { get; set; }

        public String OS { get; set; }

        public String RemoteIP { get; set; }

        public DateTime Time { get; set; }

        public String Code { get; set; }

        public String Address { get; set; }
    }
    #endregion
}