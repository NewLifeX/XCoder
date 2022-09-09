using System;
using System.ComponentModel;
using System.Windows.Forms;
using NewLife;

namespace XCoder.Tools
{
    [Category("地理信息")]
    [DisplayName("坐标转换")]
    public partial class FrmMain : Form, IXForm
    {
        public FrmMain()
        {
            InitializeComponent();

            // 动态调节宽度高度，兼容高DPI
            this.FixDpi();
        }

        private void FrmMain_Load(Object sender, EventArgs e)
        {
            var frm = new FrmGPS();
            ShowForm(frm);
        }
        private void btn_Include_Click(Object sender, EventArgs e)
        {
            var frm = new FrmInclude();
            ShowForm(frm);
        }
        private void btn_gps_Click(Object sender, EventArgs e)
        {
            var frm = new FrmGPS();
            ShowForm(frm);
        }
        public void ShowForm(Form frm)
        {
            frm.TopLevel = false;
            frm.Dock = DockStyle.Fill;
            frm.FormBorderStyle = FormBorderStyle.None;

            frm.Parent = this;

            var sps = panel2.Controls;
            foreach (var item in sps)
            {
                try
                {
                    var mm = item as Form;
                    if (mm != null) mm.Close();
                }
                catch { }
                item.TryDispose();
            }
            sps.Clear();
            sps.Add(frm);
            frm.Show();
        }


    }
}
