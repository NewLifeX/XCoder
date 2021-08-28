using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        private async void btnBackup_Click(Object sender, EventArgs e)
        {
            SaveConfig();

            var cfg = BackupConfig.Current;
            if (cfg.DestDir.IsNullOrEmpty()) throw new InvalidOperationException("目的目录不能为空！");
            if (cfg.SrcDir1.IsNullOrEmpty() &&
                cfg.SrcDir2.IsNullOrEmpty() &&
                cfg.SrcDir3.IsNullOrEmpty() &&
                cfg.SrcDir4.IsNullOrEmpty() &&
                cfg.SrcDir5.IsNullOrEmpty()) throw new InvalidOperationException("源目录不能为空！");

            pnlSetting.Enabled = false;
            btnBackup.Enabled = false;

            await Task.Run(() => DoBackup(cfg.DestDir, cfg.SrcDir1, cfg.AllowDelete));
            await Task.Run(() => DoBackup(cfg.DestDir, cfg.SrcDir2, cfg.AllowDelete));
            await Task.Run(() => DoBackup(cfg.DestDir, cfg.SrcDir3, cfg.AllowDelete));
            await Task.Run(() => DoBackup(cfg.DestDir, cfg.SrcDir4, cfg.AllowDelete));
            await Task.Run(() => DoBackup(cfg.DestDir, cfg.SrcDir5, cfg.AllowDelete));

            XTrace.WriteLine("备份完成！");

            pnlSetting.Enabled = true;
            btnBackup.Enabled = true;
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

        void DoBackup(String dest, String src, Boolean allowDelete)
        {
            if (src.IsNullOrEmpty()) return;

            foreach (var fi in src.AsDirectory().GetAllFiles("*.jpg;*.jpeg;*.png;*.mp4", true))
            {
                var newName = fi.Name;

                var p = fi.Name.IndexOf('~');
                if (p > 0)
                {
                    // 新的名称
                    newName = $"{fi.Name.Substring(0, p)}{fi.Extension}";
                }

                XTrace.WriteLine("处理：{0}", fi.FullName);

                // 识别时间
                if (TryGetTime(fi, ref newName, out var dt))
                {
                    newName = $"{dest}\\{dt:yyyy}\\{dt:yyyyMM}\\{newName}";
                    newName.EnsureDirectory(true);
                    if (!File.Exists(newName))
                    {
                        if (allowDelete)
                            fi.MoveTo(newName);
                        else
                            fi.CopyTo(newName);
                    }
                    else
                    {
                        var nfi = newName.AsFile();
                        XTrace.WriteLine("已存在");
                        XTrace.WriteLine("{0} {1:n0} byte", fi.FullName, fi.Length);
                        XTrace.WriteLine("{0} {1:n0} byte", nfi.FullName, nfi.Length);

                        // 如果大小相同则删除
                        if (allowDelete && fi.Length == nfi.Length)
                        {
                            XTrace.WriteLine("删除：{0}", fi.FullName);
                            fi.Delete();
                        }
                    }
                }
                else
                {
                    XTrace.WriteLine("无法识别时间！");
                }
            }
        }

        static Boolean TryGetTime(FileInfo fi, ref String fileName, out DateTime time)
        {
            time = DateTime.MinValue;

            var ss = fileName.Split('_', '-', '.');
            if (ss.Length >= 2 && ss[1].Length >= 6 && DateTime.TryParseExact($"{ss[0]}_{ss[1].Substring(0, 6)}", "yyyyMMdd_HHmmss", null, DateTimeStyles.None, out var dt) && dt.Year > 2000)
            {
                time = dt;
                return true;
            }
            else if (ss.Length >= 3 && ss[2].Length >= 6 && DateTime.TryParseExact($"{ss[1]}_{ss[2].Substring(0, 6)}", "yyyyMMdd_HHmmss", null, DateTimeStyles.None, out dt) && dt.Year > 2000)
            {
                time = dt;
                return true;
            }
            else if (ss.Length >= 2 && ss[0].Length >= 14 && DateTime.TryParseExact($"{ss[0].Substring(0, 14)}", "yyyyMMddHHmmss", null, DateTimeStyles.None, out dt) && dt.Year > 2000)
            {
                time = dt;
                return true;
            }
            else if (ss.Length >= 2 && ss[1].Length >= 14 && DateTime.TryParseExact($"{ss[1].Substring(0, 14)}", "yyyyMMddHHmmss", null, DateTimeStyles.None, out dt) && dt.Year > 2000)
            {
                time = dt;
                return true;
            }
            else if (fileName.Length == 3 + 14 + 4 && fileName.StartsWith("IMG"))
            {
                var str = fileName.TrimStart("IMG").Substring(null, ".");
                if (DateTime.TryParseExact(str, "yyyyMMddHHmmss", null, DateTimeStyles.None, out var dt2))
                {
                    time = dt2;
                    return true;
                }
            }
            else if (fileName.StartsWith("WP_") && fileName.EndsWithIgnoreCase(".mp4"))
            {
                var str = fileName.TrimStart("WP_").Substring(null, ".").TrimEnd("_Pro");
                if (DateTime.TryParseExact(str, "yyyyMMdd_HH_mm_ss", null, DateTimeStyles.None, out var dt2))
                {
                    time = dt2;
                    return true;
                }
            }
            else if (fileName.EndsWithIgnoreCase(".jpg", ".jpeg", ".png"))
            {
                try
                {
                    using var img = Image.FromFile(fi.FullName);
                    var pi = img.PropertyItems.FirstOrDefault(e => e.Id == 0x9003);
                    if (pi != null)
                    {
                        var str = pi.Value.ToStr().Trim('\0');
                        str = str.Substring(null, " ").Replace(":", "-") + " " + str.Substring(" ", null);
                        var dt3 = str.ToDateTime();
                        if (dt3.Year > 2000)
                        {
                            fileName = $"IMG_{dt3:yyyyMMdd}_{dt3:HHmmss}_{fileName.TrimStart("_")}";

                            time = dt3;
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    XTrace.WriteException(ex);
                }
            }

            return false;
        }
    }
}