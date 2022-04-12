using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.Serialization;
using NewLife;
using NewLife.Log;
using NewLife.Messaging;
using NewLife.Net;
using NewLife.Remoting;
using XCoder;
using XCoder.Common;

namespace XNet
{
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
            try
            {
                var ts = new List<Task>();

                var task = Task.Run(DiscoverUdp);
                ts.Add(task);

                //await Task.WaitAll(ts.ToArray(), 5_000);
                await Task.WhenAll(ts);
            }
            finally
            {
                btn.Enabled = true;
            }
        }

        async Task DiscoverUdp()
        {
            // 本地网广播，然后找到目标
            var uri = new NetUri("udp://255.255.255.255");
            uri.Port = (Int32)numPort.Value;

            // 构建请求
            var enc = new JsonEncoder();
            var msg = enc.CreateRequest("Api/Info", null);
            var req = msg.ToPacket().ReadBytes();

            var udp = new UdpClient
            {
                EnableBroadcast = true
            };

            // 发送
            udp.Send(req, req.Length, uri.EndPoint);

            // 多次接收
            while (true)
            {
                try
                {
                    var source = new CancellationTokenSource(3_000);
                    var rs = await udp.ReceiveAsync(source.Token);
                    if (rs.Buffer != null)
                    {
                        msg = new DefaultMessage();
                        msg.Read(rs.Buffer);

                        if (enc.Decode(msg, out var action, out var code, out var data) && code == 0)
                        {
                            // 解码结果
                            var result = enc.DecodeResult(action, data, msg);

                            var ai = enc.Convert(result, typeof(ApiItem)) as ApiItem;
                            if (ai != null) Invoke(() => ShowItem(ai));
                            //XTrace.WriteLine("{0}", result);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
        #endregion

        #region 显示
        private IList<ApiItem> _data;
        void ShowItem(ApiItem ai)
        {
            if (_data == null) _data = new List<ApiItem>();

            var old = _data.FirstOrDefault(e => e.Name == ai.Name && e.IP == ai.IP);
            if (old != null)
            {
                old.Time = ai.Time;

                dgv.Refresh();
            }
            else
            {
                _data.Add(ai);

                dgv.DataSource = null;
                dgv.DataSource = _data;
            }
        }

        class ApiItem
        {
            [DataMember(Name = "Server")]
            public String Name { get; set; }

            [DataMember(Name = "LocalIP")]
            public String IP { get; set; }

            public String MachineName { get; set; }

            [DataMember(Name = "ApiVersion")]
            public String Version { get; set; }

            public String OS { get; set; }

            //public String Server { get; set; }

            public DateTime Time { get; set; }
        }
        #endregion
    }
}