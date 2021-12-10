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
    [Category("工业电子")]
    [DisplayName("ModbusMaster")]
    public partial class FrmModbusMaster : Form, IXForm
    {
        private ControlConfig _config;
        private ILog _log;
        private ModbusTcp _modbus;
        private List<RegisterUnit> _data = new();

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

            txtReceive.SetDefaultStyle(12);

            var list = new List<FunctionCodes>();
            foreach (FunctionCodes item in Enum.GetValues(typeof(FunctionCodes)))
            {
                if (item.ToString().StartsWith("Read")) list.Add(item);
            }
            cbFunctionCode.DataSource = list;

            // 加载保存的颜色
            UIConfig.Apply(txtReceive);

            dataGridView1.DataSource = _data;

            _config = new ControlConfig { Control = this, FileName = "ModbusMaster.json" };
            _config.Load();
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

                _data.Clear();
                dataGridView1.DataSource = null;

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

            var data = _modbus.Read(code, host, address, count);
            if (data != null && data.Length > 0)
            {
                var dt = _data;
                var len = dt.Count;
                for (var i = 0; i < count * 2 && i < data.Length; i += 2)
                {
                    var addr = address + i;
                    var unit = dt.FirstOrDefault(e => e.Address == addr);
                    if (unit == null) dt.Add(unit = new RegisterUnit { Address = addr });

                    unit.Value = data.ToUInt16(i, true);
                }

                this.Invoke(() =>
                {
                    if (len != _data.Count)
                    {
                        _data = _data.OrderBy(e => e.Address).ToList();
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = _data;
                    }
                    dataGridView1.Refresh();
                });
            }
        }
        #endregion
    }
}