using ALib.ComPort;
using NAudio.Mixer;
using NewLife;
using NewLife.Data;
using NewLife.Net;
using NewLife.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ALib
{
    public partial class ComForm : UserControl
    {
        #region 属性
        /// <summary>端口</summary>
        public SerialTransport Port { get; set; }

        /// <summary>选择的端口</summary>
        public String SelectedPort { get { return cbName.SelectedItem + ""; } set { } }


        /// <summary>波特率。默认115200</summary>
        public Int32 BaudRate { get; set; } = 115200;

        /// <summary>奇偶校验位。默认None</summary>
        public Parity Parity { get; set; } = Parity.None;

        /// <summary>数据位。默认8</summary>
        public Int32 DataBits { get; set; } = 8;

        /// <summary>停止位。默认One</summary>
        public StopBits StopBits { get; set; } = StopBits.One;

        #endregion

        public ComForm()
        {
            InitializeComponent();
        }

        private void ComFrom_Load(object sender, EventArgs e)
        {
            cbName.DropDownStyle = ComboBoxStyle.DropDownList;

            // 挂载定时器
            _timer = new TimerX(OnCheck, null, 300, 300) { Async = true };

            if (Parent is Form frm)
            {
                frm.FormClosing += frm_FormClosing;
            }
        }

        #region 更新端口

        TimerX _timer;
        private void OnCheck(Object state)
        {
            if (Enabled) ShowPorts();
        }

        void frm_FormClosing(Object sender, FormClosingEventArgs e)
        {
            _timer.Dispose();
            _timer = null;

            Port?.Close();
        }

        String _ports = null;
        DateTime _nextport = DateTime.MinValue;
        /// <summary>下拉框显示串口</summary>
        public void ShowPorts()
        {
            if (_nextport > DateTime.Now) return;
            _nextport = DateTime.Now.AddSeconds(1);

            var ps = SerialTransport.GetPortNames();
            var str = String.Join(",", ps);
            // 如果端口有所改变，则重新绑定
            if (_ports != str)
            {
                // if (_ports != null) "串口有改变".SpeechTip();

                _ports = str;

                Invoke(() =>
                {
                    var old = cbName.SelectedItem + "";
                    cbName.DataSource = ps;
                    if (!String.IsNullOrEmpty(old) && Array.IndexOf(ps, old) >= 0) cbName.SelectedItem = old;
                });
            }
        }

        #endregion

        #region 窗体

        /// <summary>连接串口</summary>
        public void Connect()
        {
            var name = cbName.SelectedItem + "";
            if (String.IsNullOrEmpty(name))
            {
                // "请选择串口".SpeechTip();
                MessageBox.Show("请选择串口！", Text);
                cbName.Focus();
                return;
            }
            var p = name.IndexOf("(");
            if (p > 0) name = name.Substring(0, p);

            var pt = Port;
            if (pt != null)
            {
                // 如果上次没有关闭，则关闭
                try { pt.Close(); }
                catch { pt = null; }
            }

            if (pt == null) pt = new SerialTransport();
            pt.PortName = name;
            pt.BaudRate = BaudRate;
            pt.Parity = Parity;
            pt.DataBits = DataBits;
            pt.StopBits = StopBits;

            //pt.Open();

            pt.Disconnected += Port_Disconnected;
            pt.Received += OnReceived;
            pt.Open();

            //pt.EnsureCreate();
            Port = pt;
            Enabled = false;
        }

        void Port_Disconnected(Object sender, EventArgs e)
        {
            Disconnect();
        }

        /// <summary>断开串口连接</summary>
        public void Disconnect()
        {
            var pt = Port;
            if (pt != null)
            {
                Port = null;
                pt.Disconnected -= Port_Disconnected;
                pt.Received -= OnReceived;
                pt.Close();
            }

            ShowPorts();
            Enabled = true;
        }


        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEWHEEL = 0x020A;

            if (m.Msg == WM_MOUSEWHEEL)
            {
                // 在这里处理你的全局滚轮事件
                short delta = 0;
                if (IntPtr.Size == 8)
                {
                    // 64位系统
                    delta = (short)(m.WParam.ToInt64() >> 16);
                }
                else
                {
                    // 32位系统
                    delta = (short)(m.WParam.ToInt32() >> 16);
                }

                if (delta > 0)
                {
                    // 向上滚动
                    if (cbName.SelectedIndex > 0)
                    {
                        cbName.SelectedIndex--;
                    }
                }
                else if (delta < 0)
                {
                    // 向下滚动
                    if (cbName.SelectedIndex < cbName.Items.Count - 1)
                    {
                        cbName.SelectedIndex++;
                    }
                }

                // 如果你想阻止事件传递给其他控件，可以在这里返回
                return;
            }

            base.WndProc(ref m);
        }

        #endregion

        #region 收发数据
        /// <summary>发送字节数组</summary>
        /// <param name="data"></param>
        public void Send(Byte[] data)
        {
            if (data == null || data.Length <= 0) return;

            Port?.Send(data);
        }

        //public event SerialReceived Received;
        /// <summary>收到数据时触发。第一个参数是数据，第二个参数返回是否继续往下传递数据</summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event EventHandler<BufferEventArgs> Received;

        void OnReceived(Object sender, ReceivedEventArgs e)
        {
            var data = e.Packet.ReadBytes();
            if (data == null || data.Length < 1) return;

            // 处理数据委托
            if (Received != null)
            {
                var e2 = new BufferEventArgs { Value = data };
                Received(this, e2);
                if (!e2.Cancel) return;
                // 外部可能修改了数据
                // data = e2.Value;
                // if (!BufferEventArgs.Invoke(Received, data)) return null;
            }
        }
        #endregion
    }

    /// <summary>缓冲区事件参数</summary>
    public class BufferEventArgs : CancelEventArgs
    {
        private Byte[] _Value;

        /// <summary>缓冲区数据</summary>
        public Byte[] Value { get { return _Value; } set { _Value = value; } }
    }
}
