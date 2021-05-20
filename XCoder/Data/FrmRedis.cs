using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using NewLife;
using NewLife.Caching;
using NewLife.Serialization;
using XCoder.Common;

namespace XCoder.Data
{
    [Category("数据工具")]
    [DisplayName("Redis管理")]
    public partial class FrmRedis : Form, IXForm
    {
        private ControlConfig _config;
        private IList<RedisConfig> _rdsConfigs;

        #region 窗体初始化
        public FrmRedis()
        {
            InitializeComponent();

            // 动态调节宽度高度，兼容高DPI
            this.FixDpi();
        }

        private void FrmRedis_Load(Object sender, EventArgs e)
        {
            _config = new ControlConfig { Control = this, FileName = "Redis.json" };
            _config.Load();

            var nodes = _config.Items?["Nodes"];
            if (nodes != null) _rdsConfigs = JsonHelper.Convert<IList<RedisConfig>>(nodes);
            if (_rdsConfigs == null) _rdsConfigs = new List<RedisConfig>();
        }
        #endregion

        #region 辅助
        /// <summary>从字符串中获取字节数组</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private Byte[] GetBytes(String str)
        {
            if (str.IsNullOrEmpty()) return new Byte[0];

            try
            {
                if (str.Contains("-")) return str.ToHex();
            }
            catch { }

            try
            {
                return str.ToBase64();
            }
            catch { }

            return str.GetBytes();
        }

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

        private Byte[] GetPass()
        {
            var v = rtPass.Text;

            if (rbString2.Checked) return v.GetBytes();
            if (rbHex2.Checked) return v.ToHex();
            if (rbBase642.Checked) return v.ToBase64();

            return null;
        }

        private void SetResult(params String[] rs)
        {
            var sb = new StringBuilder();
            foreach (var item in rs)
            {
                if (sb.Length > 0) sb.AppendLine();
                sb.Append(item);
            }
            rtResult.Text = sb.ToString();

            _config.Save();
        }

        private void SetResult(Byte[] data)
        {
            //SetResult("/*HEX编码、Base64编码、Url改进Base64编码*/", data.ToHex("-"), data.ToBase64(), data.ToUrlBase64());

            var list = new List<String>();
            if (cbString.Checked) list.Add(data.ToStr());
            if (cbHex.Checked)
            {
                list.Add(data.ToHex().ToUpper());
                list.Add(data.ToHex().ToLower());
                list.Add(data.ToHex("-"));
                list.Add(data.ToHex(" "));
            }
            if (cbBase64.Checked)
            {
                list.Add(data.ToBase64());
                list.Add(data.ToUrlBase64());
            }

            SetResult(list.ToArray());
        }

        private void ShowTree()
        {
            var tv = treeNodes;
            tv.SuspendLayout();
            foreach (var cfg in _rdsConfigs)
            {
                if (!tv.Nodes.ContainsKey(cfg.Name))
                {
                    var node = tv.Nodes.Add(cfg.Name);
                    node.Tag = cfg;
                }
            }
            tv.ResumeLayout();
        }
        #endregion

        #region 功能
        private void btnExchange_Click(Object sender, EventArgs e)
        {
            var v = rtSource.Text;
            var v2 = rtResult.Text;
            // 结果区只要第一行
            if (!v2.IsNullOrEmpty())
            {
                var ss = v2.Split("\n");
                var n = 0;
                if (ss.Length > n + 1 && ss[n].StartsWith("/*") && ss[n].EndsWith("*/")) n++;
                v2 = ss[n];
            }
            rtSource.Text = v2;
            rtResult.Text = v;
        }
        #endregion

        private void menuEditNode_Click(Object sender, EventArgs e)
        {
            var frm = new FrmRedisConfig();

            if (frm.ShowDialog() == DialogResult.OK)
            {
                var cfg = frm.Config;
                if (!cfg.Name.IsNullOrEmpty() && !_rdsConfigs.Contains(cfg)) _rdsConfigs.Add(cfg);

                ShowTree();
            }
        }

        private void menuNodes_Opening(Object sender, CancelEventArgs e)
        {

        }

        private void treeNodes_MouseDoubleClick(Object sender, MouseEventArgs e)
        {
            var node = treeNodes.SelectedNode;
            var cfg = node?.Tag as RedisConfig;
            if (cfg == null) return;

            var rds = new FullRedis
            {
                Name = cfg.Name,
                Server = $"{cfg.Server}:{cfg.Port}",
                Password = cfg.Password,
                UserName = cfg.Username
            };

            var list = new List<FullRedis>
            {
                rds
            };
            for (var i = 1; i < 16; i++)
            {
                var r = rds.CreateSub(i);
                list.Add(r as FullRedis);
            }

            node.Nodes.Clear();
            for (var i = 0; i < list.Count; i++)
            {
                var r = list[i];
                var count = r.Count;
                node.Nodes.Add($"db{i}({count})");
            }
            node.Expand();
        }
    }
}