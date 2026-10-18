using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using XCoderMac.Services;
using XCode.DataAccessLayer;

namespace XCoderMac.Views;

/// <summary>可视化连接管理；所有写入都回到用户级 connections.json，密码进钥匙串。</summary>
public partial class MacConnectionManagerWindow : Window
{
    public ObservableCollection<string> Names { get; } = [];
    public string[] Providers { get; } =
        ["SQLite", "SqlServer", "MySql", "PostgreSQL", "Oracle", "DuckDB", "ClickHouse"];

    private string? _editingName;
    private bool _suspendSelection;
    // 双向同步时抑制 TextChanged，避免输入框互相覆盖与事件递归。
    private bool _syncingFields;
    private string _lastProvider = "";
    private string _lastValidConnectionString = "";
    public bool SavedChanges { get; private set; }

    public MacConnectionManagerWindow()
    {
        InitializeComponent();
        DataContext = this;
        ConnectionList.ItemsSource = Names;
        ProviderInput.PropertyChanged += (_, e) =>
        {
            if (e.Property == ComboBox.TextProperty) RefreshProviderFields();
        };
        AdvancedToggle.PropertyChanged += (_, e) =>
        {
            if (e.Property == ToggleButton.IsCheckedProperty) RefreshProviderFields();
        };
        // Routed TextChanged 在 Avalonia 12 中可能延后排队触发，导致双向同步时序错乱。
        // StyledProperty 变更通知同步执行，可稳定抑制递归。
        foreach (var field in new[] { HostInput, PortInput, DatabaseInput, UsernameInput, DatabaseFileInput })
            field.PropertyChanged += (_, e) =>
            {
                if (e.Property == TextBox.TextProperty) BasicFieldChanged();
            };
        ConnectionStringInput.PropertyChanged += (_, e) =>
        {
            if (e.Property == TextBox.TextProperty) ConnectionStringChanged();
        };
        LoadNames();
        NewRecord();
    }

    private void SetStatus(string status) => StatusLabel.Text = status;

    private void LoadNames()
    {
        _suspendSelection = true;
        Names.Clear();
        foreach (var entry in MacConnectionStore.Connections)
            Names.Add(entry.Name);
        ConnectionList.SelectedIndex = -1;
        _suspendSelection = false;
    }

    private void NewRecord()
    {
        _syncingFields = true;
        try
        {
            _editingName = null;
            NameInput.Text = "";
            ProviderInput.Text = "SQLite";
            ConnectionStringInput.Text = "";
            HostInput.Text = "";
            PortInput.Text = "";
            DatabaseInput.Text = "";
            UsernameInput.Text = "";
            DatabaseFileInput.Text = "";
            _lastValidConnectionString = "";
            PasswordInput.Text = "";
            PasswordModeInput.SelectedIndex = 0;
            AdvancedToggle.IsChecked = false;
            ConnectionList.SelectedIndex = -1;
        }
        finally { _syncingFields = false; }
        RefreshProviderFields();
        SetStatus("新增连接：选择数据库类型，直接填写 IP、数据库和用户名即可。");
    }

    private void New_Click(object? sender, RoutedEventArgs e) => NewRecord();

    private void SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suspendSelection || ConnectionList.SelectedItem is not string name) return;
        var entry = MacConnectionStore.Connections.FirstOrDefault(c => c.Name == name);
        if (entry == null) return;
        _syncingFields = true;
        try
        {
            _editingName = entry.Name;
            NameInput.Text = entry.Name;
            ProviderInput.Text = entry.Provider;
            ConnectionStringInput.Text = entry.ConnectionString;
            _lastValidConnectionString = entry.ConnectionString;
            PasswordInput.Text = "";
            PasswordModeInput.SelectedIndex = entry.PasswordMode switch
            {
                "plaintext" => 2, "none" => 1, _ => 0,
            };
            AdvancedToggle.IsChecked = false;
        }
        finally { _syncingFields = false; }
        RefreshProviderFields();
        ReflectFieldsFromConnection();
        SetStatus($"正在编辑连接：{name}；已保存的密码不会显示。");
    }

    private string CurrentProvider => ProviderInput?.Text?.Trim() ?? "";

    private void RefreshProviderFields()
    {
        if (_syncingFields || NetworkFields == null || FileFields == null ||
            AdvancedToggle == null || AdvancedFields == null) return;

        var provider = CurrentProvider;
        var supported = MacConnectionFields.IsSimpleProvider(provider);
        var file = MacConnectionFields.IsFileProvider(provider);

        _syncingFields = true;
        try
        {
            NetworkFields.IsVisible = supported && !file;
            FileFields.IsVisible = supported && file;
            AdvancedToggle.IsVisible = supported;
            AdvancedFields.IsVisible = !supported || AdvancedToggle.IsChecked == true;
            PortInput.Watermark = string.IsNullOrWhiteSpace(MacConnectionFields.DefaultPort(provider))
                ? "可选" : "默认 " + MacConnectionFields.DefaultPort(provider);
        }
        finally { _syncingFields = false; }

        if (_lastProvider != provider)
        {
            _lastProvider = provider;
            ReflectFieldsFromConnection();
        }
    }

    private void ConnectionStringChanged()
    {
        if (_syncingFields || ConnectionStringInput == null) return;
        ReflectFieldsFromConnection();
    }

    private void ReflectFieldsFromConnection()
    {
        if (_syncingFields || HostInput == null || ConnectionStringInput == null) return;
        var provider = CurrentProvider;
        var input = ConnectionStringInput.Text ?? "";
        MacConnectionFields parsed;
        try { parsed = MacConnectionFields.Parse(provider, input); }
        catch (ArgumentException)
        {
            SetStatus("连接字符串尚未完整或格式有误；请修正后自动同步，现有文本框内容暂不改变。");
            return;
        }

        _syncingFields = true;
        try
        {
            HostInput.Text = parsed.Host;
            PortInput.Text = parsed.Port;
            DatabaseInput.Text = parsed.Database;
            UsernameInput.Text = parsed.Username;
            DatabaseFileInput.Text = parsed.FilePath;
            _lastValidConnectionString = input;
        }
        finally { _syncingFields = false; }
    }

    private MacConnectionFields ReadFields() => new()
    {
        Host = HostInput.Text?.Trim() ?? "",
        Port = PortInput.Text?.Trim() ?? "",
        Database = DatabaseInput.Text?.Trim() ?? "",
        Username = UsernameInput.Text?.Trim() ?? "",
        FilePath = DatabaseFileInput.Text?.Trim() ?? "",
    };

    private void BasicFieldChanged()
    {
        if (_syncingFields || ConnectionStringInput == null ||
            !MacConnectionFields.IsSimpleProvider(CurrentProvider)) return;

        try
        {
            // 基于最近一份有效连接字符串调整已知字段，其余高阶参数原样保留。
            var computed = ReadFields().Build(CurrentProvider, _lastValidConnectionString, allowPartial: true);
            _syncingFields = true;
            try
            {
                ConnectionStringInput.Text = computed;
                _lastValidConnectionString = computed;
            }
            finally { _syncingFields = false; }
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            SetStatus($"请检查端口或数据库参数：{ex.Message}");
        }
    }

    /// <summary>只用于开发 UI 回归测试，不修改用户配置与钥匙串。</summary>
    public void SmokeTestFieldSynchronization()
    {
        _syncingFields = true;
        try
        {
            ProviderInput.SelectedItem = "MySql";
            ProviderInput.Text = "MySql";
        }
        finally { _syncingFields = false; }
        RefreshProviderFields();

        AdvancedToggle.IsChecked = true;
        ConnectionStringInput.Text = "Server=192.168.1.20;Port=3307;Database=Orders;User Id=alice;SslMode=Preferred";
        if (HostInput.Text != "192.168.1.20" || PortInput.Text != "3307" ||
            DatabaseInput.Text != "Orders" || UsernameInput.Text != "alice")
            throw new InvalidOperationException(
                $"输入完整连接串后字段未同步: provider={CurrentProvider}; host={HostInput.Text}; " +
                $"port={PortInput.Text}; db={DatabaseInput.Text}; user={UsernameInput.Text}; " +
                $"advancedVisible={AdvancedFields.IsVisible}");

        HostInput.Text = "10.10.0.8";
        var result = ConnectionStringInput.Text ?? "";
        if (!result.Contains("10.10.0.8") || !result.Contains("SslMode=Preferred", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("修改服务器 IP 后未同步高级连接串，或丢失高级参数");

        var prior = HostInput.Text;
        ConnectionStringInput.Text = "Server=\"partial";
        if (HostInput.Text != prior)
            throw new InvalidOperationException("格式不正确的连接串清空了已有文本框");

        // 常见驱动连接字符串的别名和 SQL Server 逗号端口兼容性。
        var sql = MacConnectionFields.Parse("SqlServer",
            "Server=10.0.0.7,1544;Initial Catalog=ERP;User ID=sa;Encrypt=True");
        if (sql.Host != "10.0.0.7" || sql.Port != "1544" ||
            sql.Database != "ERP" || sql.Username != "sa")
            throw new InvalidOperationException("SQL Server 地址/端口/库名解析失败");
        var sqlGenerated = sql.Build("SqlServer", "Encrypt=True");
        if (!sqlGenerated.Contains("10.0.0.7,1544") ||
            !sqlGenerated.Contains("Encrypt=True", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("SQL Server 连接串未保留扩展参数");

        var pg = MacConnectionFields.Parse("PostgreSQL",
            "Host=pg.internal;Port=5433;Database=Warehouse;Username=postgres;Timeout=8");
        if (pg.Host != "pg.internal" || pg.Port != "5433" ||
            pg.Database != "Warehouse" || pg.Username != "postgres")
            throw new InvalidOperationException("PostgreSQL 字段解析失败");
        if (!pg.Build("PostgreSQL", "Timeout=8").Contains("Timeout=8", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("PostgreSQL 连接串未保留超时参数");

        var sqlite = MacConnectionFields.Parse("SQLite", "Data Source=/tmp/test.db;Mode=ReadOnly");
        if (sqlite.FilePath != "/tmp/test.db" ||
            !sqlite.Build("SQLite", "Mode=ReadOnly").Contains("Mode=ReadOnly", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("SQLite 文件路径/扩展参数解析失败");

        NewRecord();
        Console.WriteLine("CONNECTION SYNC OK: MySql, SqlServer, PostgreSQL, SQLite; bidirectional fields, advanced options, invalid input");
    }

#if DEBUG
    /// <summary>检查在 macOS 深色系统模式下，本应用统一亮色控件仍可阅读。</summary>
    public void SmokeTestLightPalette()
    {
        static bool IsLight(Avalonia.Media.IBrush? brush)
        {
            if (brush is not Avalonia.Media.ISolidColorBrush solid) return false;
            var c = solid.Color;
            return c.R > 180 && c.G > 180 && c.B > 180;
        }

        static bool IsDark(Avalonia.Media.IBrush? brush)
        {
            if (brush is not Avalonia.Media.ISolidColorBrush solid) return false;
            var c = solid.Color;
            return c.R < 160 && c.G < 160 && c.B < 180;
        }

        if (ActualThemeVariant != Avalonia.Styling.ThemeVariant.Light)
            throw new InvalidOperationException($"App 应为 Light，实际主题：{ActualThemeVariant}");

        foreach (var combo in new[] { ProviderInput, PasswordModeInput })
        {
            if (!IsLight(combo.Background) || !IsDark(combo.Foreground))
                throw new InvalidOperationException(
                    $"下拉框 {combo.Name} 的底色/字体对比度不合格：{combo.Background} / {combo.Foreground}");
        }

        if (!IsLight(NameInput.Background) || !IsDark(NameInput.Foreground))
            throw new InvalidOperationException("输入框前景色和背景色仍是深色主题");

        // 仅更换窗口内的演示数据源，不保存任何连接或修改用户配置。
        ConnectionList.ItemsSource = new[] { "未选中示例", "选中示例" };
        ConnectionList.SelectedIndex = 1;
        UpdateLayout();
        var items = ConnectionList.GetVisualDescendants()
            .OfType<ListBoxItem>().ToArray();
        if (items.Length < 2 ||
            !IsDark(items[0].Foreground) || !IsDark(items[1].Foreground) ||
            !IsLight(items[1].Background))
            throw new InvalidOperationException("连接列表的未选中/选中项仍存在浅色文字或不合格的背景");

        // 通过 DEBUG-only 的伪类模拟，验证 hover 不会重新变成深灰/黑底或白字。
        var pseudoProperty = typeof(Avalonia.StyledElement).GetProperty("PseudoClasses",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("无法获取 Avalonia 伪类状态进行悬停测试");

        void CheckHover(Avalonia.Controls.Primitives.TemplatedControl control, bool expectLightBackground, bool expectDarkForeground)
        {
            var pseudo = pseudoProperty.GetValue(control)
                ?? throw new InvalidOperationException("无法测试控件伪类状态");
            var interfaceType = pseudo.GetType().GetInterfaces()
                .FirstOrDefault(i => i.Name == "IPseudoClasses")
                ?? throw new InvalidOperationException("Avalonia 伪类测试接口不存在");
            var add = interfaceType.GetMethod("Add")
                ?? throw new InvalidOperationException("Avalonia 伪类 Add 方法不存在");
            var remove = interfaceType.GetMethod("Remove")
                ?? throw new InvalidOperationException("Avalonia 伪类 Remove 方法不存在");
            void SetHover(bool value)
            {
                if (value) add.Invoke(pseudo, [":pointerover"]);
                else remove.Invoke(pseudo, [":pointerover"]);
            }
            SetHover(true);
            try
            {
                UpdateLayout();
                if ((expectLightBackground && !IsLight(control.Background)) ||
                    (expectDarkForeground && !IsDark(control.Foreground)))
                    throw new InvalidOperationException(
                        $"{control.GetType().Name} 悬停色不合格：{control.Background} / {control.Foreground}");
            }
            finally { SetHover(false); }
        }

        CheckHover(ProviderInput, true, true);
        CheckHover(PasswordModeInput, true, true);
        CheckHover(items[0], true, true);
        CheckHover(items[1], true, true);
        var testButton = this.GetVisualDescendants().OfType<Button>()
            .FirstOrDefault(b => b.Content?.ToString() == "测试连接")
            ?? throw new InvalidOperationException("测试连接按钮未找到");
        CheckHover(testButton, true, true);

        // 主按钮在悬停时必须保持蓝底白字，危险按钮保持淡红底深红字。
        var primary = this.GetVisualDescendants().OfType<Button>()
            .FirstOrDefault(b => b.Content?.ToString() == "保存连接")
            ?? throw new InvalidOperationException("保存连接按钮未找到");
        var danger = this.GetVisualDescendants().OfType<Button>()
            .FirstOrDefault(b => b.Content?.ToString() == "删除")
            ?? throw new InvalidOperationException("删除按钮未找到");

        static bool IsBlue(Avalonia.Media.IBrush? brush)
        {
            if (brush is not Avalonia.Media.ISolidColorBrush solid) return false;
            var c = solid.Color;
            return c.B > c.R + 30 && c.B > c.G && c.B >= 140 && c.R < 120;
        }

        static bool IsWhite(Avalonia.Media.IBrush? brush)
        {
            if (brush is not Avalonia.Media.ISolidColorBrush solid) return false;
            var c = solid.Color;
            return c.R > 230 && c.G > 230 && c.B > 230;
        }

        void CheckSemanticHover(Button button, bool blue)
        {
            var pseudo = pseudoProperty.GetValue(button)!;
            var iface = pseudo.GetType().GetInterfaces().First(i => i.Name == "IPseudoClasses");
            var add = iface.GetMethod("Add")!;
            var remove = iface.GetMethod("Remove")!;
            add.Invoke(pseudo, [":pointerover"]);
            try
            {
                UpdateLayout();
                if (blue)
                {
                    if (!IsBlue(button.Background) || !IsWhite(button.Foreground))
                        throw new InvalidOperationException("主按钮悬停后不是蓝底白字");
                }
                else if (!IsLight(button.Background) || !IsDark(button.Foreground))
                    throw new InvalidOperationException("删除按钮悬停后文字不清晰");
            }
            finally { remove.Invoke(pseudo, [":pointerover"]); }
        }

        CheckSemanticHover(primary, blue: true);
        CheckSemanticHover(danger, blue: false);
        Console.WriteLine("LIGHT PALETTE OK: Light theme, ComboBox, TextBox, ListBox, hover, primary/danger button contrast");
    }
#endif

    /// <summary>只填充无密码的演示数据供截图，不写入真实配置。</summary>
    public void PrepareSampleForScreenshot()
    {
        NewRecord();
        _syncingFields = true;
        try
        {
            NameInput.Text = "演示连接（未保存）";
            ProviderInput.SelectedItem = "MySql";
            ProviderInput.Text = "MySql";
            AdvancedToggle.IsChecked = true;
        }
        finally { _syncingFields = false; }
        RefreshProviderFields();
        ConnectionStringInput.Text =
            "Server=192.168.1.20;Port=3306;Database=Demo;User Id=tester;SslMode=Preferred";
        if (HostInput.Text != "192.168.1.20" ||
            DatabaseInput.Text != "Demo" || UsernameInput.Text != "tester")
            throw new InvalidOperationException("双向同步截图准备失败");
        SetStatus("演示数据：连接字符串已自动识别到相应字段（未保存，不访问数据库）。");
    }

    private void PasswordMode_Changed(object? sender, SelectionChangedEventArgs e)
    {
        if (PasswordHint == null || PasswordModeInput == null) return;
        var mode = (PasswordModeInput.SelectedItem as ComboBoxItem)?.Tag?.ToString();
        PasswordHint.Text = mode switch
        {
            "plaintext" => "警告：JSON 明文模式会把密码写入配置文件及备份文件，文件权限仅限当前用户。",
            "none" => "不保存密码，适用于无密码的本地数据库或其他认证方式。",
            _ => "推荐：密码只保存在 macOS 钥匙串，配置文件不包含明文密码。",
        };
    }

    private MacConnection ReadForm()
    {
        var mode = (PasswordModeInput.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "keychain";
        var entry = new MacConnection
        {
            Name = NameInput.Text?.Trim() ?? "",
            Provider = ProviderInput.Text?.Trim() ?? "",
            ConnectionString = MacConnectionFields.IsSimpleProvider(CurrentProvider)
                ? ReadFields().Build(CurrentProvider,
                    ConnectionStringInput.Text?.Trim() ?? "")
                : ConnectionStringInput.Text?.Trim() ?? "",
            PasswordMode = mode,
            Password = mode == "plaintext" && !string.IsNullOrWhiteSpace(PasswordInput.Text)
                ? PasswordInput.Text : null,
        };
        var existing = MacConnectionStore.Connections.FirstOrDefault(c => c.Name == _editingName);
        if (mode == "plaintext" && entry.Password == null && existing?.PasswordMode == "plaintext")
            entry.Password = existing.Password;
        if (_editingName != null && _editingName != entry.Name && mode == "keychain" &&
            string.IsNullOrWhiteSpace(PasswordInput.Text))
            throw new InvalidOperationException("修改连接名称时请重新输入钥匙串密码，以便保存到新名称。");
        MacConnectionStore.Validate(entry);
        return entry;
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var entry = ReadForm();
            var oldName = _editingName;
            var items = MacConnectionStore.Connections.Select(x => x.Copy()).ToList();
            if (oldName != null) items.RemoveAll(x => x.Name == oldName);

            if (items.Any(x => string.Equals(x.Name, entry.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("连接名称已存在");

            if (entry.PasswordMode == "keychain" && !string.IsNullOrEmpty(PasswordInput.Text))
                MacKeychainSecrets.Write(entry.Name, PasswordInput.Text);

            items.Add(entry);
            MacConnectionStore.Save(items);
            if (oldName != null && oldName != entry.Name)
                MacKeychainSecrets.Remove(oldName);
            if (entry.PasswordMode != "keychain")
                MacKeychainSecrets.Remove(entry.Name);

            SavedChanges = true;
            LoadNames();
            ConnectionList.SelectedItem = entry.Name;
            SetStatus($"已保存 {entry.Name}；下拉框已更新。");
        }
        catch (Exception ex) { SetStatus($"保存失败：{ex.Message}"); }
    }

    private async void Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_editingName == null) { SetStatus("请先选择要删除的连接"); return; }
        var name = _editingName;
        var ok = await Dialogs.ConfirmDialog.ShowAsync(this, "删除数据库连接？",
            $"将从连接配置文件删除“{name}”，并移除相应钥匙串密码。请确认。", "删除");
        if (!ok) return;
        try
        {
            MacConnectionStore.Save(MacConnectionStore.Connections.Where(x => x.Name != name).ToList());
            MacKeychainSecrets.Remove(name);
            SavedChanges = true;
            LoadNames();
            NewRecord();
            SetStatus($"已删除 {name}");
        }
        catch (Exception ex) { SetStatus($"删除失败：{ex.Message}"); }
    }

    private void Reload_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            MacConnectionStore.Reload();
            SavedChanges = true;
            LoadNames();
            NewRecord();
            SetStatus("已重新读取 connections.json；外部编辑已同步到连接列表。");
        }
        catch (Exception ex) { SetStatus($"重载失败，保留之前的配置：{ex.Message}"); }
    }

    private void OpenFolder_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "open",
                ArgumentList = { "-R", MacUserPaths.ConnectionsFile },
                UseShellExecute = false,
            });
        }
        catch (Exception ex) { SetStatus($"Finder 打开失败：{ex.Message}"); }
    }

    private async void Test_Click(object? sender, RoutedEventArgs e)
    {
        string? tempName = null;
        try
        {
            var entry = ReadForm();
            tempName = "__Newlife_XCoder_Test_" + Guid.NewGuid().ToString("N");
            var connectionString = MacConnectionStore.Compose(entry);
            // 当前表单里新输入的密码优先使用；测试连接不保存任何文件。
            if (entry.PasswordMode == "keychain" && !string.IsNullOrEmpty(PasswordInput.Text))
            {
                var builder = new System.Data.Common.DbConnectionStringBuilder { ConnectionString = connectionString };
                builder["Password"] = PasswordInput.Text;
                connectionString = builder.ConnectionString;
            }

            SetStatus("正在测试数据库连接（不保存、不修改数据库）...");
            DAL.AddConnStr(tempName, connectionString, null, entry.Provider);
            var key = tempName;
            await Task.Run(() =>
            {
                var dal = DAL.Create(key);
                using var connection = dal.Db.OpenConnection();
            }).WaitAsync(TimeSpan.FromSeconds(15));
            SetStatus("连接测试成功（仅打开/关闭连接，没有执行 SQL）。");
        }
        catch (Exception ex) { SetStatus($"连接测试未通过：{ex.GetBaseException().Message}"); }
        finally
        {
            if (tempName != null) DAL.ConnStrs.TryRemove(tempName, out _);
        }
    }

    private void Close_Click(object? sender, RoutedEventArgs e) => Close();
}
