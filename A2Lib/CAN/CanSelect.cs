using ALib.WinForm;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ALib.CAN
{
    /// <summary>
    /// CAN 口选择与开关控件。
    /// 内部枚举 <see cref="ICanPort.EnumDevices"/> 得到的设备与波特率，供用户选择后打开/关闭 CAN 口。
    /// 打开成功后通过 <see cref="PortOpened"/> 暴露 <see cref="ICanPort"/>；
    /// 收到的 CAN 帧通过 <see cref="Received"/> 事件向外抛出（后台接收线程上触发，订阅方自行切回 UI 线程）。
    /// </summary>
    public partial class CanSelect : UserControl
    {
        #region 属性

        private ICanPort _Port = null;
        private System.Windows.Forms.Timer _timer = null;

        /// <summary>当前打开或外部注入的 CAN 口，未打开时为 null</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICanPort Port
        {
            get => _Port;
            set
            {
                if (ReferenceEquals(_Port, value)) return;

                // 摘掉旧订阅；外部注入的端口不关闭，避免把宿主正在用的 CAN 口关掉
                DetachPort();

                _Port = value;
                if (value == null)
                {
                    _opened = false;
                    if (cbCan != null) cbCan.Enabled = AllowSelect;
                    if (cbBaudrate != null) cbBaudrate.Enabled = AllowSelect;
                    if (btCanOpen != null)
                    {
                        btCanOpen.Text = "打开";
                        btCanOpen.Enabled = true;
                    }
                    return;
                }

                // 外部注入的端口由外部负责关闭，不由本控件释放
                _needClose = false;
                _opened = true;

                // 已注入端口，停止定时枚举，避免干扰正在运行的接收线程
                _timer?.Stop();

                if (cbCan != null) cbCan.Text = value.Name;
                if (cbBaudrate != null) cbBaudrate.Text = value.Baudrate.ToString();

                if (btCanOpen != null)
                {
                    btCanOpen.Text = "关闭";
                    btCanOpen.Enabled = false;
                }
                if (cbCan != null) cbCan.Enabled = false;
                if (cbBaudrate != null) cbBaudrate.Enabled = false;

                value.Received += Port_Received;
            }
        }

        /// <summary>
        /// 仅取消事件订阅并把内部状态复位，不关闭硬件。
        /// 供 <see cref="Port"/> 换绑与 <see cref="ClosePort"/> 复用，保证订阅不会重复或残留。
        /// </summary>
        private void DetachPort()
        {
            var pt = _Port;
            if (pt == null) return;

            pt.Received -= Port_Received;
            _Port = null;
            _opened = false;
        }

        /// <summary>是否已打开</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Opened => _opened;

        /// <summary>任一下拉框是否处于展开状态，供宿主窗体拦截快捷键时判断</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DroppedDown => cbCan.DroppedDown || cbBaudrate.DroppedDown;

        /// <summary>是否允许选择波特率（外部注入端口时 false）</summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("是否允许在界面上选择 CAN 设备与波特率")]
        public bool AllowSelect { get; set; } = true;

        /// <summary>是否在打开成功后把波特率选择框禁用</summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("打开前是否禁用波特率与设备下拉框")]
        public bool LockOnOpen { get; set; } = true;

        /// <summary>控件显示名称，仅用于日志</summary>
        [Browsable(true)]
        [Description("控件显示名称，仅用于日志")]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        #endregion

        #region 事件

        /// <summary>CAN 口打开成功后触发</summary>
        [Category("CAN")]
        [Description("CAN 口打开成功后触发")]
        public event EventHandler PortOpened;

        /// <summary>CAN 口已关闭后触发</summary>
        [Category("CAN")]
        [Description("CAN 口已关闭后触发")]
        public event EventHandler PortClosed;

        /// <summary>收到 CAN 帧时触发（后台线程）</summary>
        [Category("CAN")]
        [Description("收到 CAN 帧时触发")]
        public event EventHandler<CanMsg> Received;

        #endregion

        #region 构造与初始化

        public CanSelect()
        {
            InitializeComponent();
        }

        private void CanSelect_Load(object sender, EventArgs e)
        {
            // 已注入/已打开端口时不再枚举设备。
            // 部分驱动（如 ZLG 的 VCI_FindUsbDevice2）在设备已打开的情况下再次枚举
            // 会干扰正在运行的接收线程，导致宿主与监视器都收不到数据。
            if (_Port == null)
            {
                InitBaudrate();
                AutoRefreshDevices();
            }
            else
            {
                // 只填充波特率列表，不回写 Text，保持注入时的显示
                InitBaudrate();
                if (cbBaudrate != null) cbBaudrate.Text = _Port.Baudrate.ToString();
            }

            if (!AllowSelect)
            {
                cbCan.Enabled = false;
                cbBaudrate.Enabled = false;
            }

            // 父窗体关闭时确保释放
            if (FindForm() is Form frm) frm.FormClosed += Form_FormClosed;
        }

        /// <summary>
        /// 定时枚举 CAN 设备。已打开端口或外部注入端口时不枚举，
        /// 避免驱动在设备已打开的情况下再次枚举而干扰正在运行的接收线程。
        /// </summary>
        private void AutoRefreshDevices()
        {
            _timer = new System.Windows.Forms.Timer { Interval = 2000 };
            _timer.Tick += (s, e) =>
            {
                // 已打开或已注入端口，无需再枚举
                if (_opened || _Port != null) return;
                if (!AllowSelect) return;
                // 用户正在展开下拉框时不要刷新，避免打断选择
                if (cbCan.DroppedDown) return;

                InitCanPort();
            };
            _timer.Start();
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e) => ClosePort();

        /// <summary>枚举 CAN 设备，填充设备下拉框。设备拔掉后同步移除列表项</summary>
        private void InitCanPort()
        {
            var devices = ICanPort.EnumDevices();

            var keys = devices.Keys.ToList();
            var last = cbCan.Text;

            // 每次重新绑定前先断开，避免 DataSource 复用旧集合导致条目残留
            cbCan.DataSource = null;
            cbCan.Items.Clear();

            cbCan.Tag = devices;
            cbCan.DataSource = keys;

            if (keys.Count == 0)
            {
                WriteLog("未发现 CAN 设备");
                return;
            }

            // 保持用户原先选中的设备；已拔掉则回退到首项
            var idx = keys.IndexOf(last);
            cbCan.SelectedIndex = idx >= 0 ? idx : 0;
        }

        /// <summary>初始化波特率下拉框，默认 250K</summary>
        private void InitBaudrate()
        {
            var list = new List<ICanPort.Baudrate_e>
            {
                ICanPort.Baudrate_e.B1000K,
                ICanPort.Baudrate_e.B500K,
                ICanPort.Baudrate_e.B250K,
                ICanPort.Baudrate_e.B200K,
                ICanPort.Baudrate_e.B125K,
                ICanPort.Baudrate_e.B100K,
            };

            cbBaudrate.DataSource = list;
            cbBaudrate.SelectedItem = ICanPort.Baudrate_e.B250K;
        }

        #endregion

        #region 打开与关闭

        private bool _opened = false;

        /// <summary>
        /// 外部注入的端口不需要本控件关闭（例如上层从别处拿到的共享 CAN 口）
        /// </summary>
        private bool _needClose = true;

        /// <summary>重新枚举设备并刷新下拉框</summary>
        public void RefreshDevices()
        {
            InitCanPort();
            InitBaudrate();
        }

        /// <summary>
        /// 按当前下拉框选择打开 CAN 口。
        /// 未注入 <see cref="Port"/> 时使用设备下拉框当前选中项创建；
        /// 已打开则返回 true。
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (_opened) return true;

            // 未注入端口时，从下拉框取设备
            if (_Port == null)
            {
                var devs = cbCan.Tag as Dictionary<String, ICanPort>;
                if (devs == null || devs.Count == 0)
                {
                    WriteLog("未发现 CAN 设备");
                    return false;
                }

                if (!devs.TryGetValue(cbCan.Text, out var dev))
                {
                    WriteLog($"未找到 CAN 设备: {cbCan.Text}");
                    return false;
                }

                WriteLog($"使用 CAN 设备: {cbCan.Text}");
                _Port = dev;
            }

            // 注入的端口可能已经打开，此时不重复 Open，避免打断其接收线程
            if (!_opened)
            {
                if (cbBaudrate.SelectedItem is ICanPort.Baudrate_e bd) _Port.Baudrate = bd;
                if (!_Port.Open())
                {
                    WriteLog($"CAN 打开失败: {_Port.Name}");
                    return false;
                }

                _Port.Received += Port_Received;
                _opened = true;
            }

            // 已打开，停止定时枚举
            _timer?.Stop();

            if (btCanOpen != null) btCanOpen.Text = "关闭";
            if (LockOnOpen)
            {
                if (cbBaudrate != null) cbBaudrate.Enabled = false;
                if (cbCan != null) cbCan.Enabled = false;
            }

            PortOpened?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>关闭 CAN 口（外部注入的端口不会真正关闭，仅取消订阅）</summary>
        public void ClosePort()
        {
            var pt = _Port;
            if (pt == null) return;

            // 是否需要真正关闭硬件，在 DetachPort 清空 _Port 之前先取
            var needClose = _needClose;

            DetachPort();

            if (needClose)
            {
                try { pt.Close(); } catch { }
            }

            if (btCanOpen != null)
            {
                btCanOpen.Text = "打开";
                btCanOpen.Enabled = true;
            }
            if (cbBaudrate != null) cbBaudrate.Enabled = AllowSelect;
            if (cbCan != null) cbCan.Enabled = AllowSelect;

            // 关闭后恢复定时枚举
            if (_Port == null && AllowSelect) _timer?.Start();

            PortClosed?.Invoke(this, EventArgs.Empty);
        }

        private void btCanOpen_Click(object sender, EventArgs e)
        {
            if (_opened) ClosePort();
            else Open();
        }

        private void Port_Received(object sender, CanMsg e) => Received?.Invoke(this, e);

        #endregion

        #region 发送

        /// <summary>发送 CAN 帧，未打开时返回 false</summary>
        public bool Send(CanMsg msg)
        {
            var pt = _Port;
            if (pt == null || !_opened || msg == null) return false;

            pt.Send(msg);
            return true;
        }

        /// <summary>发送数据，未打开时返回 false</summary>
        public bool Send(UInt32 id, byte[] data, bool remote = false, bool ext = false)
        {
            var pt = _Port;
            if (pt == null || !_opened) return false;

            pt.Send(id, data, remote, ext);
            return true;
        }

        #endregion

        #region 日志

        /// <summary>日志</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ILog Log { get; set; } = Logger.Null;

        /// <summary>写日志</summary>
        public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);

        #endregion
    }
}
