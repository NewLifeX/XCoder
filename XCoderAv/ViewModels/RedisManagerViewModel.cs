using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Serialization;
using XCoderAv.Models;

namespace XCoderAv.ViewModels;

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
                if (rds.CreateSub(i) is FullRedis sub)
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
    /// <summary>添加 Redis 节点（简化版，直接添加默认配置）</summary>
    [RelayCommand]
    private void AddNode()
    {
        var cfg = new RedisConfig
        {
            Name = $"NewNode{Nodes.Count + 1}",
            Server = "127.0.0.1",
            Port = 6379,
        };

        Nodes.Add(cfg);
        BuildTree();
        SaveConfig();
        Status = $"已添加节点：{cfg.Name}";
    }

    /// <summary>删除选中的 Redis 节点</summary>
    [RelayCommand]
    private void DeleteNode()
    {
        var node = FindSelectedConfigNode();
        if (node?.Tag is not RedisConfig cfg) return;

        Nodes.Remove(cfg);
        BuildTree();
        SaveConfig();
        Status = $"已删除节点：{cfg.Name}";
    }

    /// <summary>搜索 Key</summary>
    [RelayCommand]
    private void SearchKeys()
    {
        var dbNode = FindSelectedDbNode();
        if (dbNode?.Tag is FullRedis redis)
        {
            ExpandDatabase(dbNode, redis);
        }
    }

    private RedisTreeNode? FindSelectedConfigNode()
    {
        foreach (var node in TreeNodes)
        {
            if (node.IsSelected) return node;
            foreach (var child in GetAllNodes(node))
            {
                if (child.IsSelected) return child;
            }
        }
        return null;
    }

    private RedisTreeNode? FindSelectedDbNode()
    {
        foreach (var node in TreeNodes)
        {
            foreach (var child in node.Children)
            {
                if (child.IsSelected) return child;
                if (child.Children.Any(c => c.IsSelected)) return child;
            }
        }
        return null;
    }

    private static IEnumerable<RedisTreeNode> GetAllNodes(RedisTreeNode root)
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
}
