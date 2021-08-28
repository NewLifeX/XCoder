using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using NewLife;
using NewLife.Log;

namespace XCoder.Tools
{
    [DisplayName("图片备份")]
    public partial class FrmBackup : Form, IXForm
    {
        #region 窗体
        public FrmBackup()
        {
            InitializeComponent();

            // 动态调节宽度高度，兼容高DPI
            this.FixDpi();

            //Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Icon = IcoHelper.GetIcon("Backup");
        }

        private void FrmMain_Load(Object sender, EventArgs e)
        {
            txtLog.UseWinFormControl();
            txtLog.SetDefaultStyle(12);

            LoadConfig();
        }
        #endregion

        #region 加载/保存 配置
        void LoadConfig()
        {
            var cfg = BackupConfig.Current;
            txtDest.Text = cfg.DestDir;
            cbAllowDelete.Checked = cfg.AllowDelete;

            txtSrc1.Text = cfg.SrcDir1;
            txtSrc2.Text = cfg.SrcDir2;
            txtSrc3.Text = cfg.SrcDir3;
            txtSrc4.Text = cfg.SrcDir4;
            txtSrc5.Text = cfg.SrcDir5;
        }

        void SaveConfig()
        {
            var cfg = BackupConfig.Current;
            cfg.DestDir = txtDest.Text;
            cfg.AllowDelete = cbAllowDelete.Checked;

            cfg.SrcDir1 = txtSrc1.Text;
            cfg.SrcDir2 = txtSrc2.Text;
            cfg.SrcDir3 = txtSrc3.Text;
            cfg.SrcDir4 = txtSrc4.Text;
            cfg.SrcDir5 = txtSrc5.Text;

            cfg.Save();
        }
        #endregion

        private void btnBackup_Click(Object sender, EventArgs e)
        {
            SaveConfig();

            var cfg = BackupConfig.Current;
            if (cfg.DestDir.IsNullOrEmpty()) throw new InvalidOperationException("目的目录不能为空！");
            if (cfg.SrcDir1.IsNullOrEmpty() &&
                cfg.SrcDir2.IsNullOrEmpty() &&
                cfg.SrcDir3.IsNullOrEmpty() &&
                cfg.SrcDir4.IsNullOrEmpty() &&
                cfg.SrcDir5.IsNullOrEmpty()) throw new InvalidOperationException("源目录不能为空！");
        }

        private void btnBrowser_Click(Object sender, EventArgs e)
        {
            var btn = sender as Button;
            var txt = pnlSetting.Controls.Find(btn.Tag as String, false).FirstOrDefault() as TextBox;
            if (txt == null) return;

            var fbd = folderBrowserDialog1;
            fbd.SelectedPath = txt.Text;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txt.Text = fbd.SelectedPath;
            }
        }
    }
}