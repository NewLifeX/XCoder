using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewLife;
using NewLife.Log;
using NewLife.Serialization;

namespace XCoder.Tools
{
    [DisplayName("MD5解密")]
    public partial class FrmMD5 : Form, IXForm
    {
        #region 窗体初始化
        public FrmMD5()
        {
            InitializeComponent();

            // 动态调节宽度高度，兼容高DPI
            this.FixDpi();
        }

        private void FrmSecurity_Load(Object sender, EventArgs e) => LoadConfig();

        private void LoadConfig()
        {
            var dataPath = Setting.Current.DataPath;
            var file = dataPath.CombinePath("md5.json").GetFullPath();
            if (File.Exists(file))
            {
                var dic = new JsonParser(File.ReadAllText(file)).Decode() as IDictionary<String, Object>;
                LoadConfig(dic, this);
            }
        }

        private void LoadConfig(IDictionary<String, Object> dic, Control control)
        {
            foreach (Control item in control.Controls)
            {
                switch (item)
                {
                    case RadioButton rb:
                        if (dic.TryGetValue(item.Name, out var v)) rb.Checked = v.ToBoolean();
                        break;
                    case CheckBox cb:
                        if (dic.TryGetValue(item.Name, out v)) cb.Checked = v.ToBoolean();
                        break;
                    case RichTextBox rtb:
                        if (dic.TryGetValue(item.Name, out v)) rtb.Text = v + "";
                        break;
                    case NumericUpDown nud:
                        if (dic.TryGetValue(item.Name, out v)) nud.Value = v.ToInt();
                        break;
                    default:
                        if (item.Controls.Count > 0) LoadConfig(dic, item);
                        break;
                }
            }
        }

        private void SaveConfig()
        {
            var dic = new Dictionary<String, Object>();
            SaveConfig(dic, this);

            var dataPath = Setting.Current.DataPath;
            var file = dataPath.CombinePath("md5.json").GetFullPath();
            file.EnsureDirectory(true);
            File.WriteAllText(file, dic.ToJson(true));
        }

        private void SaveConfig(IDictionary<String, Object> dic, Control control)
        {
            foreach (Control item in control.Controls)
            {
                switch (item)
                {
                    case RadioButton rb: dic[item.Name] = rb.Checked; break;
                    case CheckBox cb: dic[item.Name] = cb.Checked; break;
                    case RichTextBox rtb: dic[item.Name] = rtb.Text; break;
                    case NumericUpDown nud: dic[item.Name] = nud.Value; break;
                    default:
                        if (item.Controls.Count > 0) SaveConfig(dic, item);
                        break;
                }
            }
        }
        #endregion

        #region 辅助
        /// <summary>从原文中获取字节数组</summary>
        /// <returns></returns>
        private Byte[] GetSource()
        {
            var v = rtSource.Text;

            if (rbString.Checked) return v.GetBytes();
            if (rbHex.Checked) return v.ToHex();
            if (rbBase64.Checked) return v.ToBase64();

            return null;
        }

        private void rtSource_TextChanged(Object sender, EventArgs e)
        {
            var v = rtSource.Text;
            if (v.IsNullOrEmpty()) return;

            // 单字节
            var enc = Encoding.UTF8;
            if (enc.GetByteCount(v) != v.Length)
            {
                rbHex.Enabled = false;
                rbBase64.Enabled = false;
                return;
            }

            try
            {
                rbHex.Enabled = v.ToHex().Length > 0;
            }
            catch
            {
                rbHex.Enabled = false;
            }

            try
            {
                rbBase64.Enabled = v.ToBase64().Length > 0;
            }
            catch
            {
                rbBase64.Enabled = false;
            }
        }
        #endregion

        #region 功能
        private String GetChars()
        {
            var length = (Int32)numLength.Value;

            // 可用字符
            var sb = new StringBuilder();
            if (cbNumber.Checked)
            {
                for (var c = '0'; c <= '9'; c++)
                    sb.Append(c);
            }
            if (cbCharLower.Checked)
            {
                for (var c = 'a'; c <= 'z'; c++)
                    sb.Append(c);
            }
            if (cbCharUpper.Checked)
            {
                for (var c = 'A'; c <= 'Z'; c++)
                    sb.Append(c);
            }
            if (cbSymbol.Checked)
            {
                for (var c = ' '; c <= '/'; c++)
                    sb.Append(c);
                for (var c = ':'; c <= '@'; c++)
                    sb.Append(c);
                for (var c = '['; c <= '\''; c++)
                    sb.Append(c);
                for (var c = '{'; c <= '~'; c++)
                    sb.Append(c);
            }

            // 总计算量
            _total = (Int64)Math.Pow(sb.Length, length);
            lbTotal.Text = _total.ToString("n0");

            return sb.ToString();
        }

        private Int64 _total;
        private Int64 _p;
        private Stopwatch _watch;
        private async void btnMD5_Click(Object sender, EventArgs e)
        {
            SaveConfig();

            var chars = GetChars();
            var buf = GetSource();
            var result = buf.ToStr().ToUpper();

            var length = (Int32)numLength.Value;
            var cpu = Environment.ProcessorCount;
            var step = _total / cpu;
            _watch = Stopwatch.StartNew();
            rtResult.Text = null;

            var btn = sender as Button;
            btn.Enabled = false;
            timer1.Enabled = true;
            try
            {
                //var source = new TaskCompletionSource<String>();
                var cts = new CancellationTokenSource();
                var ts = new List<Task<String>>();
                for (var i = 0; i < cpu; i++)
                {
                    var start = i * step;
                    if (!cts.Token.IsCancellationRequested)
                        ts.Add(Task.Run(() => DoWork(result, chars, length, start, start + step, cts)));
                }

                var rs = await Task.WhenAll(ts);

                rtResult.Text = rs.Where(e => e != null).Join(Environment.NewLine);
            }
            finally
            {
                btn.Enabled = true;
                timer1.Enabled = false;

                timer1_Tick(null, null);
                _watch.Stop();
            }
        }

        private String DoWork(String result, String cs, Int32 length, Int64 start, Int64 end, CancellationTokenSource cts)
        {
            XTrace.WriteLine("可用字符串：{0}，长度：{1}，区间({2:n0} {3:n0})", cs.Length, length, start, end);

            // 需要把数字转为密码字符串
            // 第0个字符串应该是 000000
            var txt = new Char[length];

            for (var i = start; i < end && !cts.Token.IsCancellationRequested; i++)
            {
                Interlocked.Increment(ref _p);

                // 生成字符串。倒序，逐级取余，得到字符
                var n = i;
                for (var j = length - 1; j >= 0; j--)
                {
                    n = Math.DivRem(n, cs.Length, out var rs);

                    txt[j] = cs[(Int32)rs];
                }

                var pass = new String(txt);
                if (pass.MD5() == result)
                {
                    XTrace.WriteLine("发现密码：{0}", pass);
                    cts.Cancel();
                    return pass;
                }
            }

            return null;
        }
        #endregion

        private void numLength_ValueChanged(Object sender, EventArgs e)
        {
            var chars = GetChars();
        }

        private void timer1_Tick(Object sender, EventArgs e)
        {
            lbPosition.Text = _p.ToString("n0");
            lbProgress.Text = ((Double)_p / _total).ToString("p2");
            lbCost.Text = _watch.Elapsed.ToString();

            var speed = _p * 1000d / _watch.ElapsedMilliseconds;
            lbSpeed.Text = speed.ToString("n0") + "/s";
            lbMaxCost.Text = TimeSpan.FromSeconds(_total / speed).ToString();
        }
    }
}