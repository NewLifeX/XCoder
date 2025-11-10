
using NewLife;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static XCoder.UsbHelper;

namespace XCoder
{
    [DisplayName("USB HUB 设备")]
    public partial class UsbHubSelect : Form, IXForm
    {
        public UsbHubSelect()
        {
            InitializeComponent();
        }

        /// <summary>HUB号 & HUB下所有信息</summary>
        public Dictionary<int, List<UsbDevice>> Hubs;

        /// <summary>选中的 Hub 编号。 负数无效</summary>
        public int SelectedHub { get; private set; } = -1;

        private void UsbHubSelect_Load(object sender, EventArgs e)
        {
            rtb_Show.MouseWheel += Rtb_Show_MouseWheel;


            Hubs = UsbHelper.GetAllUsbInfo();

            foreach (var kv in Hubs)
            {
                var hub = kv.Key;
                var devs = kv.Value;

                cb_Hub.Items.Add($"{hub}");
            }

            cb_Hub.SelectedValueChanged += Cb_Hub_SelectedValueChanged;
            if (cb_Hub.Items.Count > 0) cb_Hub.SelectedIndex = 0;

            bt_Canel.Hide();
            bt_Comf.Hide();
        }

        private void Rtb_Show_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                // 向上滚动
                if (cb_Hub.SelectedIndex > 0)
                {
                    cb_Hub.SelectedIndex--;
                }
            }
            else if (e.Delta < 0)
            {
                // 向下滚动
                if (cb_Hub.SelectedIndex < cb_Hub.Items.Count - 1)
                {
                    cb_Hub.SelectedIndex++;
                }
            }

        }

        private void Cb_Hub_SelectedValueChanged(object sender, EventArgs e)
        {
            rtb_Show.Clear();

            var hub = cb_Hub.Text.ToInt(-1);
            if (hub < 0) return;

            var devs = Hubs[hub];
            var sb = new StringBuilder();
            foreach (var dev in devs)
            {
                sb.AppendLine(dev.ToString());
            }

            rtb_Show.Text = sb.ToString();
        }

        private void bt_Comf_Click(object sender, EventArgs e)
        {
            SelectedHub = cb_Hub.Text.ToInt(-1);
            this.Close();
        }

        private void bt_Canel_Click(object sender, EventArgs e)
        {
            SelectedHub = -1;
            this.Close();
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

                // int delta = (short)(m.WParam.ToInt32() >> 16);
                // Console.WriteLine("Mouse wheel moved: " + delta);

                if (delta > 0)
                {
                    // 向上滚动
                    if (cb_Hub.SelectedIndex > 0)
                    {
                        cb_Hub.SelectedIndex--;
                    }
                }
                else if (delta < 0)
                {
                    // 向下滚动
                    if (cb_Hub.SelectedIndex < cb_Hub.Items.Count - 1)
                    {
                        cb_Hub.SelectedIndex++;
                    }
                }

                // 如果你想阻止事件传递给其他控件，可以在这里返回
                return;
            }

            base.WndProc(ref m);
        }

        private void bt_flash_Click(object sender, EventArgs e)
        {
            cb_Hub.Items.Clear();
            Hubs = UsbHelper.GetAllUsbInfo();

            foreach (var kv in Hubs)
            {
                var hub = kv.Key;
                var devs = kv.Value;

                cb_Hub.Items.Add($"{hub}");
            }

            cb_Hub.SelectedValueChanged += Cb_Hub_SelectedValueChanged;
            if (cb_Hub.Items.Count > 0) cb_Hub.SelectedIndex = 0;
        }
    }
}
