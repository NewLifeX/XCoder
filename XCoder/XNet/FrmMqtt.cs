using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewLife;
using NewLife.Log;
using NewLife.MQTT;
using NewLife.MQTT.Messaging;
using NewLife.Net;
using XCoder;
using XCoder.Common;

namespace XNet
{
    [Category("网络通信")]
    [DisplayName("MQTT客户端")]
    public partial class FrmMqtt : Form, IXForm
    {
        private ControlConfig _config;
        private MqttClient _Client;
        private ILog _log;

        #region 窗体
        public FrmMqtt()
        {
            InitializeComponent();

            // 动态调节宽度高度，兼容高DPI
            this.FixDpi();

            //Icon = IcoHelper.GetIcon("SSH");
        }

        private void FrmMain_Load(Object sender, EventArgs e)
        {
            _config = new ControlConfig { Control = this, FileName = "mqtt.json" };
            _config.Load();

            _log = new TextControlLog { Control = txtReceive };

            if (cbRemote.Items.Count == 0) cbRemote.Items.Add("127.0.0.1:1883");
            if (txtClientId.Text.IsNullOrEmpty()) txtClientId.Text = Environment.MachineName;
            if (cbQos.SelectedIndex < 0) cbQos.SelectedIndex = 0;
            if (cbQos2.SelectedIndex < 0) cbQos2.SelectedIndex = 0;

            //txtReceive.UseWinFormControl();
            txtReceive.Clear();
            txtReceive.SetDefaultStyle(12);
            txtSend.SetDefaultStyle(12);
        }
        #endregion

        #region 收发数据
        private async Task Connect()
        {
            _Client = null;

            var remote = cbRemote.Text;
            var uri = new NetUri(remote);
            if (uri.Type == NetType.Unknown) uri.Type = NetType.Tcp;
            if (uri.Port == 0) uri.Port = 1883;

            var client = new MqttClient
            {
                Server = uri + "",
                ClientId = txtClientId.Text,
                UserName = txtUser.Text,
                Password = txtPass.Text,

                Log = _log,
                //LogMessage = true,
            };
            client.Received += Client_Received;
            client.Connected += (s, e) => XTrace.WriteLine("连接成功");
            client.Disconnected += (s, e) => XTrace.WriteLine("连接断开");
            await client.ConnectAsync();

            _Client = client;

            "已连接服务器".SpeechTip();

            gbSetting.Enabled = false;
            gbSubscribe.Enabled = true;
            gbSend.Enabled = true;
            btnConnect.Text = "断开";

            _config.Save();
        }

        private void Client_Received(Object sender, EventArgs<PublishMessage> e)
        {
            _log.Info("{0}", e.Arg.Payload.ToStr());
        }

        private async Task Disconnect()
        {
            if (_Client != null)
            {
                _Client.Reconnect = false;
                await _Client.DisconnectAsync();
                _Client.Dispose();
                _Client = null;

                "关闭连接".SpeechTip();
            }

            gbSetting.Enabled = true;
            gbSubscribe.Enabled = false;
            gbSend.Enabled = false;
            btnConnect.Text = "连接";
        }

        private async void btnConnect_Click(Object sender, EventArgs e)
        {
            _config.Save();

            var btn = sender as Button;
            if (btn.Text == "连接")
                await Connect();
            else
                await Disconnect();
        }

        private Int32 _pColor = 0;
        private void timer1_Tick(Object sender, EventArgs e)
        {
            var cfg = NetConfig.Current;
            if (cfg.ColorLog) txtReceive.ColourDefault(_pColor);
            _pColor = txtReceive.TextLength;
        }

        private async void btnSend_Click(Object sender, EventArgs e)
        {
            var str = txtSend.Text;
            if (String.IsNullOrEmpty(str))
            {
                MessageBox.Show("发送内容不能为空！", Text);
                txtSend.Focus();
                return;
            }

            _config.Save();

            var topic = cbTopic2.Text;
            var qos = (QualityOfService)cbQos2.SelectedItem.ToInt();

            // 处理换行
            str = str.Replace("\n", "\r\n");

            if (_Client != null)
            {
                _log.Info(str);

                var rs = await _Client.PublishAsync(topic, str, qos);
                if (rs != null) _log.Info("发布成功，Id={0}", rs.Id);
            }
        }

        private async void btnSubscribe_Click(Object sender, EventArgs e)
        {
            var topic = cbTopic.Text;
            var qos = (QualityOfService)cbQos.SelectedItem.ToInt();

            if (_Client != null)
            {
                var rs = await _Client.SubscribeAsync(new[] { topic }, qos);
                if (rs != null) _log.Info("订阅成功，Id={0}", rs.Id);
            }
        }
        #endregion

        #region 右键菜单
        private void mi清空_Click(Object sender, EventArgs e)
        {
            txtReceive.Clear();
        }

        private void mi清空2_Click(Object sender, EventArgs e)
        {
            txtSend.Clear();
        }

        private void mi日志着色_Click(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            mi.Checked = !mi.Checked;
        }
        #endregion
    }
}