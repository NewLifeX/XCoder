using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using XCoderAv.Models;
using XCoderAv.ViewModels;
using XCoderMac.Dialogs;

namespace XCoderMac.Views;

/// <summary>Redis 列表显示项，引用原 RedisTreeNode，不复制 Redis 业务实现。</summary>
public sealed record RedisBrowserItem(RedisTreeNode Node, int Depth)
{
    public string Title => Node.Title;
    public Thickness Indent => new(Depth * 15, 2, 0, 2);
    public string Icon => Node.Tag switch
    {
        RedisConfig => "◉",
        NewLife.Caching.FullRedis => "▤",
        string => "▫",
        _ => "◇"
    };
}

/// <summary>Mac Redis 节点导航，复用原 RedisManagerViewModel 连接、搜索及读 Key 的逻辑。</summary>
public partial class MacRedisWindow : Window
{
    public RedisManagerViewModel Vm { get; } = new();
    public ObservableCollection<RedisBrowserItem> VisibleNodes { get; } = new();

    private RedisTreeNode? _selectedNode;
    private RedisConfig? _selectedConfig;
    private bool _loading;

    // .app 内部文件不能作为可写配置，使用当前用户的 Application Support 目录。
    private static readonly string ConfigPath = Path.Combine(
        Services.MacUserPaths.ConfigDirectory, "redis-nodes.json");
    private static readonly string LegacyConfigPath = Path.Combine(
        Services.MacUserPaths.UserHome,
        "Library", "Application Support", "XCoderMac", "RedisNodes.json");

    public MacRedisWindow()
    {
        InitializeComponent();
        DataContext = this;
        LoadMacNodes();
        RefreshBrowser();
    }

    private void LoadMacNodes()
    {
        // 首次启动迁移旧 Mac 预览版数据，不覆盖用户新目录的设置。
        if (!File.Exists(ConfigPath) && File.Exists(LegacyConfigPath))
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                File.Copy(LegacyConfigPath, ConfigPath);
                File.SetUnixFileMode(ConfigPath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            }
            catch (Exception ex)
            {
                Vm.Status = $"迁移旧版 Redis 配置失败：{ex.Message}";
            }
        }
        if (!File.Exists(ConfigPath)) return;
        try
        {
            var nodes = JsonSerializer.Deserialize<RedisConfig[]>(File.ReadAllText(ConfigPath)) ?? [];
            Vm.Nodes.Clear();
            Vm.TreeNodes.Clear();
            foreach (var cfg in nodes)
            {
                cfg.Password = ""; // 密码不从磁盘读取，必须当次会话提供。
                Vm.Nodes.Add(cfg);
                Vm.TreeNodes.Add(new RedisTreeNode { Title = cfg.Name, Tag = cfg });
            }
        }
        catch (Exception ex)
        {
            Vm.Status = $"读取 Mac 连接配置失败：{ex.Message}";
        }
    }

    private void SaveMacNodes()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
        // 永不落盘保存 Redis 密码；当前会话的密码仍可用于 NewLife.Redis 连接。
        var safeConfigs = Vm.Nodes.Select(cfg => new RedisConfig
        {
            Name = cfg.Name,
            Server = cfg.Server,
            Port = cfg.Port,
            Username = cfg.Username,
            Password = ""
        }).ToArray();
        File.WriteAllText(ConfigPath, JsonSerializer.Serialize(safeConfigs,
            new JsonSerializerOptions { WriteIndented = true }));
        if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
            File.SetUnixFileMode(ConfigPath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
    }

    private void RefreshBrowser()
    {
        VisibleNodes.Clear();
        foreach (var node in Vm.TreeNodes)
            AppendNode(node, null, 0);
    }

    private void AppendNode(RedisTreeNode node, RedisTreeNode? parent, int depth)
    {
        node.Parent = parent; // 上游展开方法未设置 Parent，读取 Key 时需要此关系。
        VisibleNodes.Add(new RedisBrowserItem(node, depth));
        if (!node.IsExpanded) return;

        foreach (var child in node.Children)
            AppendNode(child, node, depth + 1);
    }

    private void SelectNode(RedisTreeNode node)
    {
        foreach (var root in Vm.TreeNodes) ClearSelected(root);
        node.IsSelected = true;
        _selectedNode = node;
        _selectedConfig = node.Tag as RedisConfig;

        if (_selectedConfig is RedisConfig cfg)
        {
            NameBox.Text = cfg.Name;
            HostBox.Text = cfg.Server;
            PortBox.Text = cfg.Port.ToString();
            UserBox.Text = cfg.Username;
            PasswordBox.Text = cfg.Password;
        }
        else
        {
            NameBox.Text = HostBox.Text = PortBox.Text = UserBox.Text = PasswordBox.Text = "";
        }
    }

    private static void ClearSelected(RedisTreeNode node)
    {
        node.IsSelected = false;
        foreach (var child in node.Children) ClearSelected(child);
    }

    private async void Node_Click(object? sender, RoutedEventArgs e)
    {
        if (_loading || sender is not Button { Tag: RedisBrowserItem row }) return;
        var node = row.Node;
        SelectNode(node);

        if (node.Tag is string key)
        {
            Vm.SelectedKey = key;
            await ExpandAsync(node);
            return;
        }

        if (node.IsExpanded)
        {
            node.IsExpanded = false;
            RefreshBrowser();
            return;
        }
        await ExpandAsync(node);
    }

    private async Task ExpandAsync(RedisTreeNode node)
    {
        _loading = true;
        try
        {
            Vm.Status = "正在读取节点…";
            await Task.Run(() => Vm.HandleNodeDoubleClick(node));
            RefreshBrowser();
        }
        catch (Exception ex)
        {
            Vm.Status = $"节点操作失败：{ex.Message}";
        }
        finally
        {
            _loading = false;
        }
    }

    private void Add_Click(object? sender, RoutedEventArgs e)
    {
        var cfg = new RedisConfig { Name = $"NewNode{Vm.Nodes.Count + 1}", Server = "127.0.0.1", Port = 6379 };
        Vm.Nodes.Add(cfg);
        var node = new RedisTreeNode { Title = cfg.Name, Tag = cfg };
        Vm.TreeNodes.Add(node);
        RefreshBrowser();
        SelectNode(node);
        try
        {
            SaveMacNodes();
            Vm.Status = $"已添加：{cfg.Name}";
        }
        catch (Exception ex)
        {
            Vm.Status = $"保存失败：{ex.Message}";
        }
    }

    private async void Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedNode?.Tag is not RedisConfig cfg)
        {
            Vm.Status = "请先选择一个服务器节点";
            return;
        }

        if (!await ConfirmDialog.ShowAsync((TopLevel.GetTopLevel(sender as Control) as Window) ?? this, "删除 Redis 节点？",
            $"即将从本地配置中删除“{cfg.Name}”。不会删除 Redis 服务器上的数据。", "确认删除"))
            return;

        Vm.Nodes.Remove(cfg);
        Vm.TreeNodes.Remove(_selectedNode);
        _selectedNode = null;
        _selectedConfig = null;
        RefreshBrowser();
        SelectCleared();
        try
        {
            SaveMacNodes();
            Vm.Status = $"已删除本地连接配置：{cfg.Name}";
        }
        catch (Exception ex)
        {
            Vm.Status = $"保存失败：{ex.Message}";
        }
    }

    private void SelectCleared() =>
        NameBox.Text = HostBox.Text = PortBox.Text = UserBox.Text = PasswordBox.Text = "";

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedConfig is not RedisConfig cfg)
        {
            Vm.Status = "请先选择服务器节点后再编辑";
            return;
        }

        if (!int.TryParse(PortBox.Text, out var port) || port is < 1 or > 65535 ||
            string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(HostBox.Text))
        {
            Vm.Status = "名称、主机不能为空，端口必须在 1～65535 之间";
            return;
        }

        cfg.Name = NameBox.Text.Trim();
        cfg.Server = HostBox.Text.Trim();
        cfg.Port = port;
        cfg.Username = UserBox.Text?.Trim() ?? "";
        cfg.Password = PasswordBox.Text ?? "";
        _selectedNode!.Title = cfg.Name;

        try
        {
            SaveMacNodes();
            Vm.Status = $"已保存：{cfg.Name}。密码仅用于当前会话，不写入磁盘。";
            RefreshBrowser();
        }
        catch (Exception ex)
        {
            Vm.Status = $"配置保存失败：{ex.Message}";
        }
    }

    private async void Search_Click(object? sender, RoutedEventArgs e)
    {
        if (_loading) return;
        if (_selectedNode?.Tag is not NewLife.Caching.FullRedis &&
            _selectedNode?.Parent?.Tag is not NewLife.Caching.FullRedis)
        {
            Vm.Status = "请先展开服务器并选择一个数据库";
            return;
        }

        _loading = true;
        try
        {
            Vm.Status = "正在检索 Key…";
            await Task.Run(() => Vm.SearchKeysCommand.Execute(null));
            RefreshBrowser();
        }
        catch (Exception ex)
        {
            Vm.Status = $"检索失败：{ex.Message}";
        }
        finally
        {
            _loading = false;
        }
    }
}
