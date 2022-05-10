using System.ComponentModel;
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

            var dic = EnumHelper.GetDescriptions(typeof(FunctionCodes));
            //var dic = EnumHelper.GetDescriptions<FunctionCodes>();
            //var list = new List<FunctionCodes>();
            //foreach (FunctionCodes item in Enum.GetValues(typeof(FunctionCodes)))
            //{
            //    if (item.ToString().StartsWith("Read")) list.Add(item);
            //}
            cbFunctionCode.ValueMember = "key";
            cbFunctionCode.DisplayMember = "value";
            cbFunctionCode.DataSource = new BindingSource { DataSource = dic };
            if (cbFunctionCode.SelectedIndex < 0) cbFunctionCode.SelectedIndex = 0;

            // 加载保存的颜色
            UIConfig.Apply(txtReceive);

            //dgv.DataSource = _data;

            _config = new ControlConfig { Control = this, FileName = "ModbusMaster.json" };
            _config.Load();
            LoadConfig();
        }
        #endregion

        #region 加载/保存 配置
        private void LoadConfig()
        {
            var cfg = NetConfig.Current;
            mi显示应用日志.Checked = cfg.ShowLog;
            mi显示网络日志.Checked = cfg.ShowSocketLog;
            mi显示接收字符串.Checked = cfg.ShowReceiveString;
            mi显示发送数据.Checked = cfg.ShowSend;
            mi显示接收数据.Checked = cfg.ShowReceive;
            mi日志着色.Checked = cfg.ColorLog;
        }

        private void SaveConfig()
        {
            var cfg = NetConfig.Current;
            cfg.ShowLog = mi显示应用日志.Checked;
            cfg.ShowSocketLog = mi显示网络日志.Checked;
            cfg.ShowReceiveString = mi显示接收字符串.Checked;
            cfg.ShowSend = mi显示发送数据.Checked;
            cfg.ShowReceive = mi显示接收数据.Checked;
            cfg.ColorLog = mi日志着色.Checked;

            cfg.Save();
        }
        #endregion

        #region 收发数据
        private void btnConnect_Click(Object sender, EventArgs e)
        {
            _config.Save();
            SaveConfig();

            var btn = sender as Button;
            if (btn.Text == "打开")
            {
                var address = txtAddress.Text;
                var set = NetConfig.Current;

                var mb = new ModbusTcp
                {
                    Server = address,
                    //Log = _log
                };

                if (set.ShowLog) mb.Log = _log;

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
                dgv.DataSource = null;

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
            var code = (FunctionCodes)Enum.ToObject(typeof(FunctionCodes), cbFunctionCode.SelectedValue);
            var host = (Byte)numHost.Value;
            var address = (UInt16)numAddress.Value;
            var count = (UInt16)numCount.Value;

            _config.Save();
            SaveConfig();

            // 读取
            if (code <= FunctionCodes.ReadInput)
            {
                var data = _modbus.Read(code, host, address, count);
                if (data != null && data.Length > 0)
                {
                    // 按照寄存器遍历，每个寄存器2字节
                    var dt = _data;
                    var len = dt.Count;
                    for (var i = 0; i < count && i < data.Length / 2; i++)
                    {
                        var addr = address + i;
                        var unit = dt.FirstOrDefault(e => e.Address == addr);
                        if (unit == null) dt.Add(unit = new RegisterUnit { Address = addr });

                        unit.Value = data.ToUInt16(i * 2, false);
                    }

                    Invoke(() =>
                    {
                        if (len != _data.Count)
                        {
                            _data = _data.OrderBy(e => e.Address).ToList();
                            dgv.DataSource = null;
                            dgv.DataSource = _data;
                        }
                        dgv.Refresh();
                    });
                }
            }
            // 写入
            else
            {
                var values = new UInt16[count];
                for (var i = 0; i < count; i++)
                {
                    var addr = address + i * 2;
                    var unit = _data.FirstOrDefault(e => e.Address == addr);
                    if (unit != null) values[i] = unit.Value;
                }

                var rs = _modbus.Write(code, host, address, values);
            }
        }
        #endregion

        #region 右键菜单
        private void mi清空_Click(Object sender, EventArgs e) => txtReceive.Clear();

        private void Menu_Click(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            mi.Checked = !mi.Checked;
        }
        #endregion

        private void cbFunctionCode_SelectedIndexChanged(Object sender, EventArgs e)
        {
            var code = (FunctionCodes)Enum.ToObject(typeof(FunctionCodes), cbFunctionCode.SelectedValue);

            numCount.Enabled = true;

            switch (code)
            {
                case FunctionCodes.ReadCoil:
                    break;
                case FunctionCodes.ReadDiscrete:
                    break;
                case FunctionCodes.ReadRegister:
                    break;
                case FunctionCodes.ReadInput:
                    break;
                case FunctionCodes.WriteCoil:
                    numCount.Enabled = false;
                    break;
                case FunctionCodes.WriteRegister:
                    numCount.Enabled = false;
                    break;
                case FunctionCodes.WriteCoils:
                    break;
                case FunctionCodes.WriteRegisters:
                    break;
                default:
                    break;
            }
        }

        private void dataGridView1_CellValueChanged(Object sender, DataGridViewCellEventArgs e) => dgv.Refresh();

        private void btnAdd_Click(Object sender, EventArgs e)
        {
            var unit = new RegisterUnit();
            if (_data.Count > 0) unit.Address = _data[^1].Address + 1;
            _data.Add(unit);

            dgv.DataSource = null;
            dgv.DataSource = _data;
            dgv.Refresh();
        }
    }
}