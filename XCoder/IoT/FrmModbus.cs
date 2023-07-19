using System.ComponentModel;
using System.IO.Ports;
using NewLife;
using System.Xml.Linq;
using NewLife.IoT.Protocols;
using NewLife.Log;
using NewLife.Serial.Protocols;
using XCoder;
using NewLife.Net;
using NewLife.Threading;

namespace WinModbus;

[Category("工业电子")]
[DisplayName("Modbus工具")]
public partial class FrmModbus : Form, IXForm
{
    private Modbus _modbus;
    private ILog _log;
    TimerX _timer;

    public FrmModbus()
    {
        InitializeComponent();
    }

    private void FrmMain_Load(Object sender, EventArgs e)
    {
        cbPorts.DataSource = SerialPort.GetPortNames();

        var bs = new List<Int32>(new Int32[] { 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 128000, 194000, 256000, 512000, 1024000, 2048000 });
        cbBaudrate.DataSource = bs;

        _log = new TextControlLog
        {
            Control = richTextBox1,
            Level = LogLevel.Debug
        };

        // 挂载定时器
        _timer = new TimerX(OnCheck, null, 300, 300) { Async = true };
    }

    private void OnCheck(Object state)
    {
        if (groupBox1.Enabled) ShowPorts();
    }

    String _ports = null;
    DateTime _nextport = DateTime.MinValue;
    public void ShowPorts()
    {
        if (_nextport > DateTime.Now) return;
        _nextport = DateTime.Now.AddSeconds(1);

        var ps = SerialTransport.GetPortNames();
        var str = String.Join(",", ps);
        // 如果端口有所改变，则重新绑定
        if (_ports != str)
        {
            if (_ports != null) "串口有改变".SpeechTip();

            _ports = str;

            Invoke(() =>
            {
                var old = cbPorts.SelectedItem + "";
                cbPorts.DataSource = ps;
                if (!String.IsNullOrEmpty(old) && Array.IndexOf(ps, old) >= 0) cbPorts.SelectedItem = old;
            });
        }
    }

    private void btnConnect_Click(Object sender, EventArgs e)
    {
        var btn = sender as Button;
        if (btn.Text == "连接")
        {
            var name = cbPorts.SelectedItem + "";
            if (String.IsNullOrEmpty(name))
            {
                "请选择串口".SpeechTip();
                MessageBox.Show("请选择串口！", Text);
                cbPorts.Focus();
                return;
            }
            var p = name.IndexOf("(");
            if (p > 0) name = name.Substring(0, p);

            _modbus = new ModbusRtu
            {
                PortName = name,
                Baudrate = (Int32)cbBaudrate.SelectedItem,
                Log = _log,
            };

            _modbus.Open();

            $"连接串口{name}".SpeechTip();

            btn.Text = "断开";
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = true;
            btnConnect2.Enabled = false;
        }
        else
        {
            _modbus.Dispose();

            "串口已断开".SpeechTip();

            btn.Text = "连接";
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = false;
            btnConnect2.Enabled = true;
        }
    }

    private void btnConnect2_Click(Object sender, EventArgs e)
    {
        var btn = sender as Button;
        if (btn.Text == "连接")
        {
            _modbus = new ModbusTcp
            {
                Server = $"{txtAddress.Text}:{numPort.Value}",
                Log = _log,
            };

            _modbus.Open();

            "网络已连接".SpeechTip();

            btn.Text = "断开";
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = true;
            btnConnect.Enabled = false;
        }
        else
        {
            _modbus.Dispose();

            "连接已断开".SpeechTip();

            btn.Text = "连接";
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = false;
            btnConnect.Enabled = true;
        }
    }

    private void btnSwitch_Click(Object sender, EventArgs e)
    {
        var host = (Byte)numHost.Value;
        var frm = new FrmSwitch
        {
            Modbus = _modbus,
            Host = host,
        };
        frm.Show();
    }

    private void btnI6O6N_Click(Object sender, EventArgs e)
    {
        var host = (Byte)numHost.Value;
        var frm = new FrmI6O6N
        {
            Modbus = _modbus,
            Host = host,
        };
        frm.Show();
    }
}
