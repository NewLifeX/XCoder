using System;
using System.ComponentModel;
using System.Windows.Forms;
using NewLife;
using NewLife.Log;
using NewLife.Net;
using Renci.SshNet;
using XCoder;
using XCoder.Common;

namespace XNet
{
    [Category("网络通信")]
    [DisplayName("SSH工具")]
    public partial class FrmSsh : Form, IXForm
    {
        private ControlConfig _config;
        private SshClient _Client;
        private ILog _log;

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

            _log = new TextControlLog { Control = txtReceive };

            //txtReceive.UseWinFormControl();
            txtReceive.Clear();
            txtReceive.SetDefaultStyle(12);
            txtSend.SetDefaultStyle(12);

            gbReceive.Tag = gbReceive.Text;
            gbSend.Tag = gbSend.Text;
        }
        #endregion

        #region 收发数据
        private void Connect()
        {
            _Client = null;

            var remote = cbRemote.Text;
            var uri = new NetUri(remote);
            if (uri.Type == NetType.Unknown) uri.Type = NetType.Tcp;
            if (uri.Port == 0) uri.Port = 22;

            var client = new SshClient(uri.Host ?? (uri.Address + ""), uri.Port, txtUser.Text, txtPass.Text);
            client.ErrorOccurred += Client_ErrorOccurred;
            client.Connect();

            _Client = client;

            "已连接服务器".SpeechTip();

            pnlSetting.Enabled = false;
            btnConnect.Text = "关闭";

            _config.Save();
        }

        private void Disconnect()
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

        private void Client_ErrorOccurred(Object sender, Renci.SshNet.Common.ExceptionEventArgs e)
        {
            _log.Error(e.Exception.ToString());
        }

        private Int32 _pColor = 0;
        private Int32 BytesOfReceived = 0;
        private Int32 BytesOfSent = 0;
        private Int32 lastReceive = 0;
        private Int32 lastSend = 0;
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

            _config.Save();

            // 处理换行
            str = str.Replace("\n", "\r\n");

            if (_Client != null)
            {
                _log.Info(str);

                var rs = _Client.RunCommand(str);
                if (rs != null)
                {
                    if (!rs.Result.IsNullOrEmpty()) _log.Info(rs.Result);
                    if (!rs.Error.IsNullOrEmpty()) _log.Error(rs.Error);
                }
            }
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

        private void mi日志着色_Click(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            mi.Checked = !mi.Checked;
        }
        #endregion
    }
}