using System;
using System.ComponentModel;
using System.Windows.Forms;
using NewLife;
using NewLife.Log;
using NewLife.Net;
using XCoder;
using XCoder.Common;
using XCoder.XNet;

namespace XNet
{
    [Category("网络通信")]
    [DisplayName("SSH工具")]
    public partial class FrmSsh : Form, IXForm
    {
        private ControlConfig _config;
        ISocketClient _Client;

        #region 窗体
        public FrmSsh()
        {
            InitializeComponent();

            // 动态调节宽度高度，兼容高DPI
            this.FixDpi();

            Icon = IcoHelper.GetIcon("SSH");
        }

        private void FrmMain_Load(Object sender, EventArgs e)
        {
            _config = new ControlConfig { Control = this, FileName = "ssh.json" };
            _config.Load();

            txtReceive.UseWinFormControl();
            txtReceive.Clear();
            txtReceive.SetDefaultStyle(12);
            txtSend.SetDefaultStyle(12);

            gbReceive.Tag = gbReceive.Text;
            gbSend.Tag = gbSend.Text;

            // 加载保存的颜色
            UIConfig.Apply(txtReceive);
        }
        #endregion

        #region 收发数据
        void Connect()
        {
            _Client = null;

            var remote = cbRemote.Text;
            var uri = new NetUri(remote);
            if (uri.Type == NetType.Unknown) uri.Type = NetType.Tcp;
            if (uri.Port == 0) uri.Port = 22;

            var client = uri.CreateRemote();

            client.Received += OnReceived;
            client.Log = XTrace.Log;

            if (!client.Open()) return;

            _Client = client;

            "已连接服务器".SpeechTip();

            pnlSetting.Enabled = false;
            btnConnect.Text = "关闭";

            _config.Save();
        }

        void Disconnect()
        {
            if (_Client != null)
            {
                _Client.Dispose();
                _Client = null;

                "关闭连接".SpeechTip();
            }

            pnlSetting.Enabled = true;
            btnConnect.Text = "打开";
        }

        private void btnConnect_Click(Object sender, EventArgs e)
        {
            _config.Save();

            var btn = sender as Button;
            if (btn.Text == "打开")
                Connect();
            else
                Disconnect();
        }

        void OnReceived(Object sender, ReceivedEventArgs e)
        {
            XTrace.WriteLine(e.Packet.ToStr());
        }

        Int32 _pColor = 0;
        Int32 BytesOfReceived = 0;
        Int32 BytesOfSent = 0;
        Int32 lastReceive = 0;
        Int32 lastSend = 0;
        private void timer1_Tick(Object sender, EventArgs e)
        {
            var rcount = BytesOfReceived;
            var tcount = BytesOfSent;
            if (rcount != lastReceive)
            {
                gbReceive.Text = (gbReceive.Tag + "").Replace("0", rcount + "");
                lastReceive = rcount;
            }
            if (tcount != lastSend)
            {
                gbSend.Text = (gbSend.Tag + "").Replace("0", tcount + "");
                lastSend = tcount;
            }

            var cfg = NetConfig.Current;
            if (cfg.ColorLog) txtReceive.ColourDefault(_pColor);
            _pColor = txtReceive.TextLength;
        }

        private void btnSend_Click(Object sender, EventArgs e)
        {
            var str = txtSend.Text;
            if (String.IsNullOrEmpty(str))
            {
                MessageBox.Show("发送内容不能为空！", Text);
                txtSend.Focus();
                return;
            }

            // 处理换行
            str = str.Replace("\n", "\r\n");

            if (_Client != null) _Client.Send(str);
        }
        #endregion

        #region 右键菜单
        private void mi清空_Click(Object sender, EventArgs e)
        {
            txtReceive.Clear();
            BytesOfReceived = 0;
        }

        private void mi清空2_Click(Object sender, EventArgs e)
        {
            txtSend.Clear();
            BytesOfSent = 0;
        }

        private void mi显示应用日志_Click(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            mi.Checked = !mi.Checked;
        }

        private void mi显示网络日志_Click(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            mi.Checked = !mi.Checked;
        }

        private void mi显示发送数据_Click(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            mi.Checked = !mi.Checked;
        }

        private void mi显示接收数据_Click(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            mi.Checked = !mi.Checked;
        }

        private void mi显示统计信息_Click(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            NetConfig.Current.ShowStat = mi.Checked = !mi.Checked;
        }

        private void mi显示接收字符串_Click(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            NetConfig.Current.ShowReceiveString = mi.Checked = !mi.Checked;
        }

        private void miHex发送_Click(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            NetConfig.Current.HexSend = mi.Checked = !mi.Checked;
        }

        private void 查看Tcp参数ToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            NetHelper2.ShowTcpParameters();
        }

        private void 设置最大TcpToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            NetHelper2.SetTcpMax();
        }

        private void mi日志着色_Click(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            mi.Checked = !mi.Checked;
        }
        #endregion
    }
}