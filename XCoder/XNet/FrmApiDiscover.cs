using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using NewLife;
using NewLife.Log;
using NewLife.Messaging;
using NewLife.Net;
using NewLife.Remoting;
using NewLife.Security;
using NewLife.Serialization;
using Stardust.Models;
using XCoder;
using XCoder.Common;

namespace XNet;

[Category("网络通信")]
[DisplayName("ApiServer搜索")]
public partial class FrmApiDiscover : Form, IXForm
{
    private ControlConfig _config;

    #region 窗体
    public FrmApiDiscover()
    {
        InitializeComponent();

        // 动态调节宽度高度，兼容高DPI
        this.FixDpi();
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
        try
        {
            var ts = new List<Task>();

            {
                var ep = new IPEndPoint(IPAddress.Broadcast, port);
                var task = Task.Run(() => DiscoverUdp(null, ep));
                ts.Add(task);
            }
            foreach (var ip in NetHelper.GetIPs().Where(e => e.IsIPv4()))
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

        //var uri = new NetUri("udp://255.255.255.255:5500");
        var client = new ApiClient($"udp://{ep.Address}:{ep.Port}");
        client.Received += Client_Received;

        // 异步发送，但是不等待返回，因为可能会有多个返回，在事件里处理
        _ = client.InvokeAsync<Object>("Api/Info");

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

    private void Client_Received(Object sender, ApiReceivedEventArgs e)
    {
        if (e.Message == null || !e.Message.Reply) return;

        var msg = e.ApiMessage;
        XTrace.WriteLine("Received [{0}]: {1}", msg.Action, msg.Data.ToStr());

        var client = sender as ApiClient;
        var enc = client.Encoder;

        // 解码结果
        var result = enc.DecodeResult(msg.Action, msg.Data, e.Message);
        //XTrace.WriteLine("Receive[{0}] {1}", udp.Client.LocalEndPoint, result.ToJson());
        var remote = (e.UserState as ReceivedEventArgs)?.Remote;

        if (msg.Action == "Api/Info" && enc.Convert(result, typeof(ApiItem)) is ApiItem ai)
        {
            ai.RemoteIP = remote.Address + "";

            if (ai.Name == "StarAgent")
            {
                //_ = client.InvokeAsync<Object>("Info");
                Task.Run(async () =>
                {
                    var client2 = new ApiClient($"udp://{remote}");
                    var rs = await client2.InvokeAsync<AgentInfo>("Info");
                    if (rs != null)
                    {
                        ai.Code = rs.Code;
                        ai.Address = rs.Server;
                    }
                });
            }

            if (ai.Id > 0) Invoke(() => ShowItem(ai));
        }
    }
    #endregion

    #region 显示
    private IDictionary<String, ApiItem> _data;
    void ShowItem(ApiItem ai)
    {
        _data ??= new Dictionary<String, ApiItem>();

        var key = $"{ai.Name}-{ai.IP}-{ai.Id}";

        if (_data.TryGetValue(key, out var old))
        {
            old.Time = ai.Time;
            old.Code = ai.Code;
            //old.Server = ai.Server;

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

        [DataMember(Name = "Server")]
        public String Name { get; set; }

        [DataMember(Name = "LocalIP")]
        public String IP { get; set; }

        public String MachineName { get; set; }

        [DataMember(Name = "FileVersion")]
        public String Version { get; set; }

        public String OS { get; set; }

        //public String Server { get; set; }

        public String RemoteIP { get; set; }

        public DateTime Time { get; set; }

        public String Code { get; set; }

        public String Address { get; set; }
    }
    #endregion
}