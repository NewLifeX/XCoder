using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using NewLife;
using NewLife.IoT.Protocols;
using NewLife.Log;
using XCoder;
using XCoder.Common;
using XCoder.XNet;

namespace XNet
{
    [Category("网络通信")]
    [DisplayName("ModbusMaster")]
    public partial class FrmModbusMaster : Form, IXForm
    {
        private ControlConfig _config;
        private ILog _log;
        private ModbusTcp _modbus;
        private readonly List<RegisterUnit> _data = new List<RegisterUnit>();

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

            cbFunctionCode.DataSource = Enum.GetValues(typeof(FunctionCodes));

            // 加载保存的颜色
            UIConfig.Apply(txtReceive);

            dataGridView1.DataSource = _data;
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
                gbSend.Enabled = true;
            }
            else
            {
                _modbus.TryDispose();
                _modbus = null;

                pnlSetting.Enabled = true;
                btn.Text = "打开";
                gbSend.Enabled = false;
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
            var code = (FunctionCodes)cbFunctionCode.SelectedItem;
            var host = (Byte)numHost.Value;
            var address = (UInt16)numAddress.Value;
            var count = (UInt16)numCount.Value;

            _config.Save();

            Byte[] data = null;
            switch (code)
            {
                case FunctionCodes.ReadCoil:
                    data = _modbus.ReadCoil(host, address, count);
                    break;
                case FunctionCodes.ReadDiscrete:
                    data = _modbus.ReadDiscrete(host, address, count);
                    break;
                case FunctionCodes.ReadRegister:
                    data = _modbus.ReadRegister(host, address, count);
                    break;
                case FunctionCodes.ReadInput:
                    data = _modbus.ReadInput(host, address, count);
                    break;
                //case FunctionCodes.WriteCoil:
                //    break;
                //case FunctionCodes.WriteHolding:
                //    break;
                default:
                    break;
            }

            if (data != null && data.Length > 0)
            {
                var dt = _data;
                for (var i = 0; i < count && i < data.Length; i++)
                {
                    var addr = address + i;
                    var unit = dt.FirstOrDefault(e => e.Address == addr);
                    if (unit == null) dt.Add(unit = new RegisterUnit { Address = addr });

                    unit.Value = data[i];
                }
                _data.Sort((x, y) => x.Address.CompareTo(y.Address));

                this.Invoke(() =>
                {
                    dataGridView1.DataSource = _data;
                    dataGridView1.Refresh();
                });
            }
        }
        #endregion
    }
}