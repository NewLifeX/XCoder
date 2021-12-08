using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewLife;
using NewLife.IoT.Protocols;
using NewLife.Log;
using XCoder;
using XCoder.Common;

namespace XNet
{
    [Category("网络通信")]
    [DisplayName("ModbusMaster")]
    public partial class FrmModbusMaster : Form, IXForm
    {
        private ControlConfig _config;
        private ILog _log;
        private ModbusTcp _modbus;

        #region 窗体
        public FrmModbusMaster()
        {
            InitializeComponent();

            // 动态调节宽度高度，兼容高DPI
            this.FixDpi();
        }

        private void FrmMain_Load(Object sender, EventArgs e)
        {
            _log = new TextControlLog { Control = txtReceive };

            _config = new ControlConfig { Control = this, FileName = "ModbusMaster.json" };
            _config.Load();

            txtReceive.SetDefaultStyle(12);

            // 加载保存的颜色
            UIConfig.Apply(txtReceive);
        }
        #endregion

        #region 收发数据
        private void btnConnect_Click(Object sender, EventArgs e)
        {
            _config.Save();

            var btn = sender as Button;
            if (btn.Text == "打开")
            {
                var address = txtAddress.Text;

                var mb = new ModbusTcp
                {
                    Server = address,
                    Log = _log
                };
                mb.Open();

                _modbus = mb;

                pnlSetting.Enabled = false;
                btn.Text = "断开";
            }
            else
            {
                _modbus.TryDispose();
                _modbus = null;

                pnlSetting.Enabled = true;
                btn.Text = "打开";
            }
        }

        private Int32 _pColor = 0;
        private void timer1_Tick(Object sender, EventArgs e)
        {
            var cfg = NetConfig.Current;
            if (cfg.ColorLog) txtReceive.ColourDefault(_pColor);
            _pColor = txtReceive.TextLength;
        }

        private void btnSend_Click(Object sender, EventArgs e)
        {
            var mode = cbFunctionCode.SelectedItem + "";
            var code = mode.Substring(null, " ").ToInt();
            var host = (Byte)numHost.Value;
            var address = (UInt16)numAddress.Value;
            var count = (UInt16)numCount.Value;

            _config.Save();

            switch (code)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    _modbus.ReadHoldingRegister(host, address, count);
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}