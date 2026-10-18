using ALib.UsbHub;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace xDesktopInfo
{
    public partial class MainForm : Form
    {
        // 定时刷新
        private readonly System.Windows.Forms.Timer _timer;

        // 定时刷新间隔（毫秒），默认10秒
        private int _refreshInterval = 10000;

        // 当前显示所在屏幕，默认主屏幕
        private Screen _currentScreen;

        // 文字样式：分层窗口由本窗体自绘，不再使用 Label 子控件
        // （UpdateLayeredWindow 会用位图替换整个窗口内容，子控件画了也看不见）
        private readonly Font _font = new Font("Microsoft YaHei", 10F);
        private readonly Color _foreColor = Color.White;
        private readonly Padding _padding = new Padding(8, 4, 8, 4);

        // 当前要显示的文本
        private string _text = string.Empty;

        /// <summary>
        /// 一张网卡的有效信息（Id 用于跨刷新稳定标识并持久化勾选状态）
        /// </summary>
        private class NetCard
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string IPv4 { get; set; }
            public int Prefix { get; set; }   // 子网掩码前缀长度(如 255.255.255.0 => 24)
            public string Gateway { get; set; }
        }

        public MainForm()
        {
            InitializeComponent();

            // 加载配置（显示屏幕 + 刷新间隔）
            var cfg = DesktopInfoConfig.Current;
            _refreshInterval = Math.Max(500, cfg.RefreshInterval * 1000);

            // 当前屏幕：默认主屏幕；若配置的屏幕编号有效则使用配置的屏幕
            _currentScreen = Screen.PrimaryScreen ?? Screen.AllScreens.FirstOrDefault() ?? Screen.FromPoint(new Point(0, 0));
            if (cfg.ScreenIndex >= 0 && cfg.ScreenIndex < Screen.AllScreens.Length)
            {
                _currentScreen = Screen.AllScreens[cfg.ScreenIndex];
            }

            // 窗体基本属性：无边框、透明、底层显示
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = false;
            StartPosition = FormStartPosition.Manual;

            // 不使用 TransparencyKey / AllowTransparency：
            // 颜色抠像是"精确比色"，文字抗锯齿边缘与底色混合后留下的中间色抠不掉，会形成暗色描边。
            // 这里改用每像素 Alpha：先在 32bpp ARGB 位图上画字，再用 UpdateLayeredWindow 合成，
            // 边缘带真实 alpha 通道，既无描边也保留平滑抗锯齿。
            AllowTransparency = false;

            // 首次显示后再渲染一次：Shown 时窗口 DPI 才最终确定，可避免缩放屏上字号偏小
            Shown += (s, e) => RefreshInfo();

            // 定位到主屏幕右上角
            PositionTopRight();

            // 定时器
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = _refreshInterval;
            _timer.Tick += (s, e) => RefreshInfo();
            _timer.Start();

            // 托盘图标
            SetupNotifyIcon();

            // 初次刷新
            RefreshInfo();
        }

        #region 信息内容（用户填充区）

        /// <summary>
        /// 在此函数中填充要显示的系统信息，每行一条，返回后将在桌面右上角按行右对齐显示。
        /// </summary>
        /// <returns>要显示的多行信息</returns>
        public string BuildInfoText()
        {
            // TODO: 在这里填充你的系统信息内容
            var sb = new StringBuilder();
            sb.AppendLine($"系统：{Environment.OSVersion}");
            sb.AppendLine($"机器名：{Environment.MachineName}");

            // 勾选的网卡信息：显示在计算机名下方、HUB 上方
            var selIds = DesktopInfoConfig.Current.SelectedNetCards ?? new List<string>();
            var cards = GetNetCards().Where(c => selIds.Contains(c.Id)).ToList();
            if (cards.Count > 0)
            {
                sb.AppendLine();
                foreach (var card in cards)
                {
                    sb.AppendLine(card.Name);
                    if (!string.IsNullOrEmpty(card.IPv4))
                    {
                        sb.AppendLine($"  {card.IPv4}/{card.Prefix}");
                    }
                    if (!string.IsNullOrEmpty(card.Gateway))
                    {
                        sb.AppendLine($"  {card.Gateway}");
                    }
                }
            }

            sb.AppendLine();

            var hubs = UsbHelper.GetAllUsbInfo();
            foreach (var hub in hubs.OrderBy(h => h.Key))
            {
                sb.AppendLine($"====  HUB {hub.Key}  ====");
                foreach (var dev in hub.Value)
                {
                    var name = dev.Name;
                    if ((dev.SubDev != null)&&(dev.SubDev.Count > 0))
                    {
                        name = string.Join(" + ", dev.SubDev);
                    }

                    sb.AppendLine($"{dev.Port} {name}");
                }
            }

            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// 枚举当前处于活动状态且拥有 IPv4 地址的网卡列表。
        /// </summary>
        private static List<NetCard> GetNetCards()
        {
            var list = new List<NetCard>();
            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
                    if (ni.OperationalStatus != OperationalStatus.Up) continue;

                    var props = ni.GetIPProperties();
                    var ipv4 = props.UnicastAddresses.FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork);
                    if (ipv4 == null) continue;

                    var gw = props.GatewayAddresses
                        .FirstOrDefault(g => g.Address != null && g.Address.AddressFamily == AddressFamily.InterNetwork)?.Address?.ToString();

                    list.Add(new NetCard
                    {
                        Id = ni.Id,
                        Name = ni.Name,
                        IPv4 = ipv4.Address.ToString(),
                        Prefix = MaskToPrefix(ipv4.IPv4Mask),
                        Gateway = gw ?? string.Empty
                    });
                }
            }
            catch
            {
                // 枚举失败时返回空列表，不影响其他信息显示
            }
            return list;
        }

        /// <summary>
        /// 把子网掩码(如 255.255.255.0)转成 CIDR 前缀长度(如 24)。
        /// </summary>
        private static int MaskToPrefix(IPAddress mask)
        {
            if (mask == null) return 0;
            var bytes = mask.GetAddressBytes();
            int bits = 0;
            foreach (var b in bytes)
            {
                for (int i = 7; i >= 0; i--)
                {
                    if ((b & (1 << i)) == 0) return bits;
                    bits++;
                }
            }
            return bits;
        }

        /// <summary>
        /// 刷新显示信息
        /// </summary>
        public void RefreshInfo()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(RefreshInfo));
                return;
            }

            _text = BuildInfoText();

            // 根据文字内容调整窗体宽高，并重新贴靠到屏幕右上角
            ResizeToContent();
            PositionTopRight();

            // 把文字位图合成到分层窗口
            UpdateLayered();

            // 压到底层显示
            SetWindowToBottom();
        }

        /// <summary>
        /// 创建一个与当前屏幕 DPI 一致、带 Alpha 通道的位图
        /// </summary>
        private Bitmap CreateBitmap(int width, int height)
        {
            var bmp = new Bitmap(Math.Max(1, width), Math.Max(1, height), PixelFormat.Format32bppArgb);
            bmp.SetResolution(DeviceDpi, DeviceDpi);
            return bmp;
        }

        /// <summary>
        /// 窗体宽高根据文字内容自适应
        /// </summary>
        private void ResizeToContent()
        {
            using var measure = CreateBitmap(1, 1);
            using var g = Graphics.FromImage(measure);

            // 逐行量宽：比整体 MeasureString 稳，且能正确处理末尾空行
            float lineHeight = _font.GetHeight(g);
            float maxWidth = 0;
            foreach (var line in _text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                var w = g.MeasureString(line, _font, 100000, StringFormat.GenericTypographic).Width;
                if (w > maxWidth)
                {
                    maxWidth = w;
                }
            }

            int lineCount = _text.Split('\n').Length;
            ClientSize = new Size(
                (int)Math.Ceiling(maxWidth) + _padding.Horizontal,
                (int)Math.Ceiling(lineHeight * lineCount) + _padding.Vertical);
        }

        /// <summary>
        /// 将窗体定位到当前屏幕右上角（右、上各留一个字的空隙）
        /// </summary>
        private void PositionTopRight()
        {
            var wa = _currentScreen.WorkingArea;
            var gap = _font.Height;   // 一个字的空隙
            Left = wa.Right - Width - gap;
            Top = wa.Top + gap;
        }

        /// <summary>
        /// 设置托盘图标与右键菜单
        /// </summary>
        private void SetupNotifyIcon()
        {
            // 右键菜单
            var menu = new ContextMenuStrip();

            // 定时刷新子菜单：1/5/10 秒，默认10秒（初始勾选跟随配置）
            int refreshSec = DesktopInfoConfig.Current.RefreshInterval;
            var miTimer = new ToolStripMenuItem("定时刷新");
            var mi1 = new ToolStripMenuItem("1秒") { CheckOnClick = true, Checked = refreshSec == 1 };
            var mi5 = new ToolStripMenuItem("5秒") { CheckOnClick = true, Checked = refreshSec == 5 };
            var mi10 = new ToolStripMenuItem("10秒") { CheckOnClick = true, Checked = refreshSec >= 10 || (refreshSec != 1 && refreshSec != 5) };
            mi1.Click += (s, e) => SetRefreshInterval(1000, mi1);
            mi5.Click += (s, e) => SetRefreshInterval(5000, mi5);
            mi10.Click += (s, e) => SetRefreshInterval(10000, mi10);
            miTimer.DropDownItems.AddRange(new ToolStripItem[] { mi1, mi5, mi10 });

            // 屏幕选择子菜单：列出所有屏幕，单选切换
            var miScreen = new ToolStripMenuItem("屏幕选择");
            var screens = Screen.AllScreens;
            var miScreens = new List<ToolStripMenuItem>();
            for (int i = 0; i < screens.Length; i++)
            {
                var sc = screens[i];
                int index = i;
                string label = (sc.Primary ? "主屏" : "屏") + (i + 1) +
                               $" ({sc.Bounds.Width}x{sc.Bounds.Height})";
                var item = new ToolStripMenuItem(label) { CheckOnClick = true, Checked = ReferenceEquals(sc, _currentScreen) };
                item.Click += (s, e) => SelectScreen(sc, miScreens);
                miScreens.Add(item);
            }
            miScreen.DropDownItems.AddRange(miScreens.ToArray());

            // 网卡选择子菜单：列出所有网卡，勾选结果显示在桌面上
            // 每次展开时重建，以跟随网卡插拔等变化并回读勾选状态
            var miNetCard = new ToolStripMenuItem("网卡选择");
            var rebuildNetCardMenu = new Action(() => RebuildNetCardMenu(miNetCard));
            menu.Opening += (s, e) => rebuildNetCardMenu();

            // 退出
            var miExit = new ToolStripMenuItem("退出");
            miExit.Click += (s, e) => ExitApplication();

            menu.Items.AddRange(new ToolStripItem[] { miTimer, miScreen, miNetCard, new ToolStripSeparator(), miExit });

            // 托盘图标（双击刷新）
            notifyIcon = new NotifyIcon
            {
                Icon = LoadLogoIcon(),
                Text = "桌面信息（双击刷新）",
                ContextMenuStrip = menu,
                Visible = true
            };
            // 左键双击托盘图标刷新信息
            notifyIcon.DoubleClick += (s, e) => RefreshInfo();
        }

        /// <summary>
        /// 重建【网卡选择】子菜单项：列出所有活动网卡，勾选/取消并回写配置。
        /// </summary>
        private void RebuildNetCardMenu(ToolStripMenuItem miNetCard)
        {
            var cfg = DesktopInfoConfig.Current;
            var selected = cfg.SelectedNetCards ?? new List<string>();

            miNetCard.DropDownItems.Clear();

            var cards = GetNetCards();
            if (cards.Count == 0)
            {
                miNetCard.DropDownItems.Add(new ToolStripMenuItem("（未发现活动网卡）") { Enabled = false });
                return;
            }

            foreach (var card in cards)
            {
                bool isChecked = selected.Contains(card.Id);
                var label = string.IsNullOrEmpty(card.IPv4) ? card.Name : $"{card.Name} ({card.IPv4})";
                var item = new ToolStripMenuItem(label) { CheckOnClick = true, Checked = isChecked };
                string cardId = card.Id;
                item.Click += (s, e) => ToggleNetCard(cardId, item.Checked);
                miNetCard.DropDownItems.Add(item);
            }
        }

        /// <summary>
        /// 勾选/取消勾选某张网卡，并把最新勾选集合回写保存到配置，随后刷新桌面显示。
        /// </summary>
        private void ToggleNetCard(string cardId, bool checkedState)
        {
            var cfg = DesktopInfoConfig.Current;
            var selected = cfg.SelectedNetCards ?? new List<string>();
            if (checkedState)
            {
                if (!selected.Contains(cardId)) selected.Add(cardId);
            }
            else
            {
                selected.Remove(cardId);
            }
            cfg.SelectedNetCards = selected;
            cfg.Save();
            RefreshInfo();
        }

        /// <summary>
        /// 从嵌入资源加载托盘图标 logo.ico
        /// </summary>
        private static Icon LoadLogoIcon()
        {
            using var stream = typeof(MainForm).Assembly.GetManifestResourceStream("xDesktopInfo.logo.ico");
            if (stream == null)
            {
                return SystemIcons.Application;
            }
            return new Icon(stream);
        }

        /// <summary>
        /// 干净退出：停止定时器、隐藏并销毁托盘图标，再结束应用。
        /// 确保进程真正退出，避免 exe/dll 被占用导致编译无法复制文件。
        /// </summary>
        private void ExitApplication()
        {
            try
            {
                _timer?.Stop();
                _timer?.Dispose();
                if (notifyIcon != null)
                {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                    notifyIcon = null;
                }
            }
            catch
            {
                // 清理异常不影响退出
            }

            Application.Exit();
        }

        /// <summary>
        /// 切换显示屏幕并重新定位到新屏幕右上角
        /// </summary>
        private void SelectScreen(Screen screen, List<ToolStripMenuItem> items)
        {
            _currentScreen = screen;
            // 更新子菜单勾选状态
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Checked = ReferenceEquals(Screen.AllScreens[i], screen);
            }
            // 回写配置并保存
            var cfg = DesktopInfoConfig.Current;
            cfg.ScreenIndex = Array.IndexOf(Screen.AllScreens, screen);
            cfg.Save();
            PositionTopRight();
            // 不同屏幕 DPI 可能不同，换屏后必须重新渲染，否则字号与实际 DPI 不匹配
            RefreshInfo();
        }

        /// <summary>
        /// 设置定时刷新间隔
        /// </summary>
        private void SetRefreshInterval(int ms, ToolStripMenuItem checkedItem)
        {
            _refreshInterval = ms;
            _timer.Interval = ms;
            foreach (ToolStripMenuItem it in ((ToolStripMenuItem)checkedItem.OwnerItem).DropDownItems)
            {
                it.Checked = ReferenceEquals(it, checkedItem);
            }
            // 回写配置并保存
            var cfg = DesktopInfoConfig.Current;
            cfg.RefreshInterval = ms / 1000;
            cfg.Save();
        }

        #region 点击穿透：鼠标不可选中文字，也不阻挡桌面操作

        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                // WS_EX_LAYERED 是实现每像素 Alpha 的必要条件（此处不能依赖 AllowTransparency）
                cp.ExStyle |= WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        #endregion

        #region 每像素 Alpha：把文字位图合成到分层窗口

        private const uint ULW_ALPHA = 0x00000002;
        private const byte AC_SRC_OVER = 0x00;
        private const byte AC_SRC_ALPHA = 0x01;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst,
            ref Size psize, IntPtr hdcSrc, ref Point pptSrc, uint crKey, ref BLENDFUNCTION pblend, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// 将当前文本渲染到 ARGB 位图，并用 UpdateLayeredWindow 设为窗口内容。
        /// 文字边缘带真实 alpha 通道，因此不会出现抠像透明那样的暗色描边。
        /// </summary>
        private void UpdateLayered()
        {
            if (!IsHandleCreated)
            {
                return;
            }

            int w = Math.Max(1, ClientSize.Width);
            int h = Math.Max(1, ClientSize.Height);

            using var bmp = CreateBitmap(w, h);
            using (var g = Graphics.FromImage(bmp))
            {
                // 透明背景上必须用灰度抗锯齿；ClearType 需要不透明底色，用在 alpha 位图上会出现彩边
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                using var brush = new SolidBrush(_foreColor);
                g.DrawString(_text, _font, brush, _padding.Left, _padding.Top, StringFormat.GenericTypographic);
            }

            var screenDc = GetDC(IntPtr.Zero);
            var memDc = CreateCompatibleDC(screenDc);
            var hBitmap = IntPtr.Zero;
            var oldBitmap = IntPtr.Zero;
            try
            {
                hBitmap = bmp.GetHbitmap(Color.FromArgb(0));
                oldBitmap = SelectObject(memDc, hBitmap);

                var dst = new Point(Left, Top);
                var size = new Size(w, h);
                var src = Point.Empty;
                var blend = new BLENDFUNCTION
                {
                    BlendOp = AC_SRC_OVER,
                    BlendFlags = 0,
                    SourceConstantAlpha = 255,
                    AlphaFormat = AC_SRC_ALPHA
                };

                UpdateLayeredWindow(Handle, screenDc, ref dst, ref size, memDc, ref src, 0, ref blend, ULW_ALPHA);
            }
            finally
            {
                if (oldBitmap != IntPtr.Zero)
                {
                    SelectObject(memDc, oldBitmap);
                }
                if (hBitmap != IntPtr.Zero)
                {
                    DeleteObject(hBitmap);
                }
                DeleteDC(memDc);
                ReleaseDC(IntPtr.Zero, screenDc);
            }
        }

        #endregion

        #region 底层显示：将窗体压到所有窗口最底部

        private static readonly IntPtr HWND_BOTTOM = (IntPtr)1;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_NOOWNERZORDER = 0x0200;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        /// <summary>
        /// 将窗体压到 Z 序最底部，使其显示在其他窗口之下
        /// </summary>
        private void SetWindowToBottom()
        {
            if (!IsHandleCreated)
            {
                return;
            }
            SetWindowPos(Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOOWNERZORDER);
        }

        #endregion
    }
}
