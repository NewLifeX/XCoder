using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrazyCoder.Models;
using NewLife;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Serialization;

namespace CrazyCoder.ViewModels;

/// <summary>Redis 管理器 ViewModel</summary>
public partial class RedisManagerViewModel : ObservableObject
{
    #region 属性

    /// <summary>Redis 节点配置列表</summary>
    public ObservableCollection<RedisConfig> Nodes { get; } = [];

    /// <summary>树形节点数据</summary>
    public ObservableCollection<RedisTreeNode> TreeNodes { get; } = [];

    /// <summary>Key 搜索模式</summary>
    [ObservableProperty]
    private String _searchPattern = "*";

    /// <summary>Key 值内容</summary>
    [ObservableProperty]
    private String _keyValue = "";

    /// <summary>选中的 Key</summary>
    [ObservableProperty]
    private String _selectedKey = "";

    /// <summary>状态文本</summary>
    [ObservableProperty]
    private String _status = "就绪";

    /// <summary>配置数据路径</summary>
    private readonly String _configFile;

    #endregion

    #region 构造

    /// <summary>实例化 Redis 管理器 ViewModel</summary>
    public RedisManagerViewModel()
    {
        _configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "RedisNodes.json");

        LoadConfig();
    }

    #endregion

    #region 配置持久化

    private void LoadConfig()
    {
        try
        {
            var file = _configFile.GetFullPath();
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                var list = JsonHelper.Convert<IList<RedisConfig>>(json);
                if (list != null)
                {
                    Nodes.Clear();
                    foreach (var node in list)
                    {
                        Nodes.Add(node);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            XTrace.WriteException(ex);
        }

        BuildTree();
    }

    private void SaveConfig()
    {
        try
        {
            var json = Nodes.ToJson(true);
            var file = _configFile.GetFullPath();
            file.EnsureDirectory(true);
            File.WriteAllText(file, json);
        }
        catch (Exception ex)
        {
            XTrace.WriteException(ex);
        }
    }

    #endregion

    #region 树形结构

    /// <summary>构建树节点</summary>
    private void BuildTree()
    {
        TreeNodes.Clear();

        foreach (var cfg in Nodes)
        {
            var node = new RedisTreeNode
            {
                Title = cfg.Name,
                Tag = cfg,
                IsExpanded = false,
            };
            TreeNodes.Add(node);
        }
    }

    /// <summary>展开 Redis 服务器节点，显示 db0~db15</summary>
    private void ExpandServer(RedisTreeNode serverNode, RedisConfig cfg)
    {
        try
        {
            var rds = new FullRedis
            {
                Name = cfg.Name,
                Server = $"{cfg.Server}:{cfg.Port}",
                Password = cfg.Password,
                UserName = cfg.Username,
            };

            serverNode.Children.Clear();
            for (var i = 0; i < 16; i++)
            {
                var sub = rds.CreateSub(i) as FullRedis;
                if (sub != null)
                {
                    var count = sub.Count;
                    var dbNode = new RedisTreeNode
                    {
                        Title = $"db{i}({count})",
                        Tag = sub,
                        IsExpanded = false,
                    };
                    serverNode.Children.Add(dbNode);
                }
            }
            serverNode.IsExpanded = true;
        }
        catch (Exception ex)
        {
            Status = $"连接失败：{ex.Message}";
            XTrace.WriteException(ex);
        }
    }

    /// <summary>展开数据库节点，显示 Key 列表</summary>
    private void ExpandDatabase(RedisTreeNode dbNode, FullRedis redis)
    {
        try
        {
            var list = redis.Search(SearchPattern, 0, 100).ToList();

            dbNode.Children.Clear();
            foreach (var key in list)
            {
                var keyNode = new RedisTreeNode
                {
                    Title = key,
                    Tag = key,
                    IsExpanded = false,
                };
                dbNode.Children.Add(keyNode);
            }
            dbNode.IsExpanded = true;
            Status = $"共 {list.Count} 个 Key";
        }
        catch (Exception ex)
        {
            Status = $"搜索失败：{ex.Message}";
            XTrace.WriteException(ex);
        }
    }

    /// <summary>加载 Key 的值</summary>
    private void LoadKeyValue(FullRedis redis, String key)
    {
        try
        {
            var value = redis.Get<String>(key);
            KeyValue = value ?? "";
            Status = $"Key: {key}";
        }
        catch (Exception ex)
        {
            Status = $"读取失败：{ex.Message}";
            XTrace.WriteException(ex);
        }
    }

    /// <summary>处理节点双击</summary>
    public void HandleNodeDoubleClick(RedisTreeNode node)
    {
        if (node == null) return;

        if (node.Tag is RedisConfig cfg)
        {
            ExpandServer(node, cfg);
        }
        else if (node.Tag is FullRedis redis)
        {
            ExpandDatabase(node, redis);
        }
        else if (node.Tag is String key)
        {
            if (node.Parent?.Tag is FullRedis parentRedis)
                LoadKeyValue(parentRedis, key);
        }
    }

    #endregion

    #region 命令

    /// <summary>添加 Redis 节点</summary>
    [RelayCommand]
    private void AddNode()
    {
        var cfg = new RedisConfig
        {
            Name = "NewNode",
            Server = "127.0.0.1",
            Port = 6379,
        };

        // 简单编辑对话框（使用 WPF 内置方式）
        var result = ShowEditDialog(cfg, "添加Redis节点");
        if (result == true)
        {
            Nodes.Add(cfg);
            BuildTree();
            SaveConfig();
            Status = $"已添加节点：{cfg.Name}";
        }
    }

    /// <summary>编辑选中的 Redis 节点</summary>
    [RelayCommand]
    private void EditNode()
    {
        var node = FindSelectedConfigNode();
        if (node == null) return;

        var cfg = node.Tag as RedisConfig;
        if (cfg == null) return;

        // 复制一份以便取消
        var copy = new RedisConfig
        {
            Name = cfg.Name,
            Server = cfg.Server,
            Port = cfg.Port,
            Username = cfg.Username,
            Password = cfg.Password,
        };

        var result = ShowEditDialog(copy, "编辑Redis节点");
        if (result == true)
        {
            cfg.Name = copy.Name;
            cfg.Server = copy.Server;
            cfg.Port = copy.Port;
            cfg.Username = copy.Username;
            cfg.Password = copy.Password;

            BuildTree();
            SaveConfig();
            Status = $"已更新节点：{cfg.Name}";
        }
    }

    /// <summary>删除选中的 Redis 节点</summary>
    [RelayCommand]
    private void DeleteNode()
    {
        var node = FindSelectedConfigNode();
        if (node == null) return;

        var cfg = node.Tag as RedisConfig;
        if (cfg == null) return;

        var result = MessageBox.Show($"确定删除节点「{cfg.Name}」吗？", "确认删除",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            Nodes.Remove(cfg);
            BuildTree();
            SaveConfig();
            Status = $"已删除节点：{cfg.Name}";
        }
    }

    /// <summary>搜索 Key</summary>
    [RelayCommand]
    private void SearchKeys()
    {
        // 在当前展开的数据库节点上搜索
        var dbNode = FindSelectedDbNode();
        if (dbNode?.Tag is FullRedis redis)
        {
            ExpandDatabase(dbNode, redis);
        }
    }

    private RedisTreeNode FindSelectedConfigNode()
    {
        // 遍历 TreeNodes 查找选中项
        foreach (var node in TreeNodes)
        {
            if (node.IsSelected) return node;
            // 检查子节点
            foreach (var child in GetAllNodes(node))
            {
                if (child.IsSelected) return child;
            }
        }
        return null;
    }

    private RedisTreeNode FindSelectedDbNode()
    {
        foreach (var node in TreeNodes)
        {
            foreach (var child in node.Children)
            {
                if (child.IsSelected) return child;
                // 也可能选中了 key 节点，取父节点
                if (child.Children.Any(c => c.IsSelected)) return child;
            }
        }
        return null;
    }

    private IEnumerable<RedisTreeNode> GetAllNodes(RedisTreeNode root)
    {
        foreach (var child in root.Children)
        {
            yield return child;
            foreach (var grandChild in GetAllNodes(child))
            {
                yield return grandChild;
            }
        }
    }

    #endregion

    #region 编辑对话框

    /// <summary>显示编辑对话框</summary>
    private Boolean? ShowEditDialog(RedisConfig cfg, String title)
    {
        // 使用简化的 WPF 输入对话框
        var window = new Window
        {
            Title = title,
            Width = 420,
            Height = 320,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
        };

        var stack = new System.Windows.Controls.StackPanel { Margin = new Thickness(12) };

        AddTextField(stack, "名称：", cfg, nameof(cfg.Name));
        AddTextField(stack, "服务器：", cfg, nameof(cfg.Server));
        AddTextField(stack, "端口：", cfg, nameof(cfg.Port));
        AddTextField(stack, "用户名：", cfg, nameof(cfg.Username));
        AddPasswordField(stack, "密码：", cfg, nameof(cfg.Password));

        var btnPanel = new System.Windows.Controls.StackPanel
        {
            Orientation = System.Windows.Controls.Orientation.Horizontal,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
            Margin = new Thickness(0, 12, 0, 0),
        };

        var btnOk = new System.Windows.Controls.Button
        {
            Content = "确定",
            Width = 80,
            Height = 30,
            Margin = new Thickness(0, 0, 8, 0),
            IsDefault = true,
        };
        var btnCancel = new System.Windows.Controls.Button
        {
            Content = "取消",
            Width = 80,
            Height = 30,
            IsCancel = true,
        };

        btnOk.Click += (s, e) => window.DialogResult = true;
        btnPanel.Children.Add(btnOk);
        btnPanel.Children.Add(btnCancel);
        stack.Children.Add(btnPanel);

        window.Content = stack;
        return window.ShowDialog();
    }

    private static void AddTextField(System.Windows.Controls.StackPanel stack, String label, Object dataContext, String propertyName)
    {
        var panel = new System.Windows.Controls.WrapPanel { Margin = new Thickness(0, 4, 0, 4) };
        var lbl = new System.Windows.Controls.Label
        {
            Content = label,
            Width = 60,
            VerticalAlignment = System.Windows.VerticalAlignment.Center,
        };
        var txt = new System.Windows.Controls.TextBox
        {
            Width = 300,
            VerticalAlignment = System.Windows.VerticalAlignment.Center,
            DataContext = dataContext,
        };

        var prop = dataContext.GetType().GetProperty(propertyName);
        if (prop != null)
        {
            txt.Text = prop.GetValue(dataContext)?.ToString() ?? "";
            txt.TextChanged += (s, e) =>
            {
                var value = txt.Text;
                if (prop.PropertyType == typeof(Int32))
                {
                    if (Int32.TryParse(value, out var n))
                        prop.SetValue(dataContext, n);
                }
                else
                {
                    prop.SetValue(dataContext, value);
                }
            };
        }

        panel.Children.Add(lbl);
        panel.Children.Add(txt);
        stack.Children.Add(panel);
    }

    private static void AddPasswordField(System.Windows.Controls.StackPanel stack, String label, Object dataContext, String propertyName)
    {
        var panel = new System.Windows.Controls.WrapPanel { Margin = new Thickness(0, 4, 0, 4) };
        var lbl = new System.Windows.Controls.Label
        {
            Content = label,
            Width = 60,
            VerticalAlignment = System.Windows.VerticalAlignment.Center,
        };
        var txt = new System.Windows.Controls.PasswordBox
        {
            Width = 300,
            VerticalAlignment = System.Windows.VerticalAlignment.Center,
        };

        var prop = dataContext.GetType().GetProperty(propertyName);
        if (prop != null)
        {
            txt.Password = prop.GetValue(dataContext)?.ToString() ?? "";
            txt.PasswordChanged += (s, e) => prop.SetValue(dataContext, txt.Password);
        }

        panel.Children.Add(lbl);
        panel.Children.Add(txt);
        stack.Children.Add(panel);
    }

    #endregion
}

/// <summary>Redis 树节点</summary>
public partial class RedisTreeNode : ObservableObject
{
    /// <summary>节点标题</summary>
    [ObservableProperty]
    private String _title = "";

    /// <summary>关联数据</summary>
    public Object Tag { get; set; }

    /// <summary>是否展开</summary>
    [ObservableProperty]
    private Boolean _isExpanded;

    /// <summary>是否选中</summary>
    [ObservableProperty]
    private Boolean _isSelected;

    /// <summary>父节点</summary>
    public RedisTreeNode Parent { get; set; }

    /// <summary>子节点集合</summary>
    public ObservableCollection<RedisTreeNode> Children { get; } = [];

    /// <summary>层级</summary>
    public Int32 Level => Parent?.Level + 1 ?? 0;

    /// <summary>缩进</summary>
    public Thickness Indent => new Thickness(Level * 16, 0, 0, 0);
}
