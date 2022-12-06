using System.ComponentModel;
using NewLife;
using NewLife.Caching;
using NewLife.Serialization;
using XCoder.Common;

namespace XCoder.Data;

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

        if (_config.Items.TryGetValue("Nodes", out var nodes))
        {
            if (nodes != null) _rdsConfigs = JsonHelper.Convert<IList<RedisConfig>>(nodes);
        }

        _rdsConfigs ??= new List<RedisConfig>();

        ShowTree();
    }
    #endregion

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

    private void ShowRedis(TreeNode node, RedisConfig cfg)
    {
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
            var n = node.Nodes.Add($"db{i}({count})");
            n.Tag = r;
        }
        node.Expand();
    }

    private void ShowKeys(TreeNode node, FullRedis redis)
    {
        var list = redis.Search("*", 100).ToList();

        node.Nodes.Clear();
        foreach (var item in list)
        {
            var n = node.Nodes.Add(item);
            n.Tag = item;
        }
        node.Expand();
    }

    private void ShowValue(TreeNode node, String key)
    {
        var redis = node.Parent.Tag as FullRedis;
        if (redis == null) return;

        var value = redis.Get<String>(key);
        richTextBox1.Text = value;
    }

    private void menuEditNode_Click(Object sender, EventArgs e)
    {
        var frm = new FrmRedisConfig();

        if (frm.ShowDialog() == DialogResult.OK)
        {
            var cfg = frm.Config;
            if (!cfg.Name.IsNullOrEmpty() && !_rdsConfigs.Contains(cfg)) _rdsConfigs.Add(cfg);

            ShowTree();

            _config.Items["Nodes"] = _rdsConfigs;
            _config.Save();
        }
    }

    private void menuNodes_Opening(Object sender, CancelEventArgs e)
    {

    }

    private void treeNodes_MouseDoubleClick(Object sender, MouseEventArgs e)
    {
        var node = treeNodes.SelectedNode;
        if (node == null) return;

        if (node.Tag is RedisConfig cfg)
            ShowRedis(node, cfg);
        else if (node.Tag is FullRedis redis)
            ShowKeys(node, redis);
        else if (node.Tag is String key)
            ShowValue(node, key);
    }
}