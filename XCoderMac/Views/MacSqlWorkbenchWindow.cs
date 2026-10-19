using System.Data;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using XCode.DataAccessLayer;
using XCoderMac.Dialogs;

namespace XCoderMac.Views;

/// <summary>Windows FrmSchema / FrmQuery 的 Mac 版：架构元数据、SQL 查询与明确确认后的 DML/DDL。</summary>
public sealed class MacSqlWorkbenchWindow : Window
{
    private readonly string _connection;
    private readonly ComboBox _schemaTypes = new() { MinWidth = 210, Height = 38 };
    private readonly ListBox _schemaTables = new();
    private readonly TextBox _sql = new() { AcceptsReturn = true, MinHeight = 150,
        Text = "SELECT * FROM ", TextWrapping = TextWrapping.Wrap,
        MinWidth = 250 };
    private readonly TextBlock _status = new() { Foreground = new SolidColorBrush(Color.Parse("#607D9B")), FontSize = 12 };
    public string StatusText => _status.Text ?? "";
    public MacSqlWorkbenchWindow(string conn)
    {
        _connection = conn;
        Title = $"数据库架构与 SQL 查询 · {conn}";
        Width = 1050;
        Height = 760;
        MinWidth = 750;
        MinHeight = 520;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Background = new SolidColorBrush(Color.Parse("#F7F9FC"));
        _schemaTables.ItemTemplate = new FuncDataTemplate<IDataTable>((item, _) =>
            new TextBlock { Text = item == null ? "" : $"{item.TableName} · {item.Description}",
                Foreground = new SolidColorBrush(Color.Parse("#2F567C")), FontSize = 12,
                Margin = new Thickness(8, 9), TextTrimming = TextTrimming.CharacterEllipsis });
        BuildLayout();
    }

    private static Button Button(string name, Action click, bool danger = false)
    {
        var btn = new Button
        {
            Content = name, Background = new SolidColorBrush(Color.Parse(danger ? "#FBE7E8" : "#E9F1FC")),
            Foreground = new SolidColorBrush(Color.Parse(danger ? "#962E43" : "#265991")),
            Padding = new Thickness(14, 9), CornerRadius = new CornerRadius(8)
        };
        btn.Click += (_, _) => click();
        return btn;
    }

    private void BuildLayout()
    {
        var root = new Grid { Margin = new Thickness(23), RowDefinitions = new RowDefinitions("Auto,12,*,10,Auto") };
        root.Children.Add(new StackPanel { Spacing = 5, Children =
        {
            new TextBlock { Text = "架构管理 / SQL 查询器", FontSize = 23, FontWeight = FontWeight.SemiBold,
                Foreground = new SolidColorBrush(Color.Parse("#284665")) },
            new TextBlock { Text = $"当前连接：{_connection}  ·  架构只读浏览；执行写入 SQL 前会二次确认",
                FontSize = 12, Foreground = new SolidColorBrush(Color.Parse("#8297AF")) }
        } });

        var tabs = new TabControl { Name = "SqlWorkbenchTabs", FontSize = 14 };
        Grid.SetRow(tabs, 2); root.Children.Add(tabs);
        var schemaPane = new Grid
        {
            Margin = new Thickness(0, 12),
            RowDefinitions = new RowDefinitions("Auto,10,*"),
            ColumnDefinitions = new ColumnDefinitions("240,12,*")
        };
        var tools = new StackPanel { Spacing = 8 };
        tools.Children.Add(Button("刷新架构集合", () => _ = LoadSchemaTypesAsync()));
        tools.Children.Add(_schemaTypes);
        tools.Children.Add(Button("查看架构元数据", () => _ = LoadSchemaCollectionAsync()));
        tools.Children.Add(Button("读取数据库表", () => _ = LoadTablesAsync()));
        schemaPane.Children.Add(tools);

        _schemaTables.SelectionChanged += (_, _) =>
        {
            if (_schemaTables.SelectedItem is IDataTable table)
                _schemaDisplay.Text = $"表：{table.TableName}   {table.Description}\n" +
                    string.Join("\n", table.Columns.Select(c => $"{c.ColumnName,-28} {c.DataType?.Name,-12} {c.Description}")) +
                    "\n\n索引：" + string.Join(", ", table.Indexes.Select(i => i.Name));
        };
        Grid.SetRow(_schemaTables, 2); schemaPane.Children.Add(_schemaTables);
        var schemaResult = new TextBox
        {
            IsReadOnly = true, TextWrapping = TextWrapping.NoWrap, AcceptsReturn = true,
            FontFamily = new FontFamily("Menlo,Consolas,monospace"),
            Background = new SolidColorBrush(Color.Parse("#F8FBFE")),
            Foreground = new SolidColorBrush(Color.Parse("#294663")),
            MinWidth = 240
        };
        _schemaDisplay = schemaResult;
        Grid.SetColumn(schemaResult, 2);
        Grid.SetRowSpan(schemaResult, 3); schemaPane.Children.Add(schemaResult);
        tabs.Items.Add(new TabItem
        {
            Header = "数据库架构", Content = schemaPane,
            FontSize = 14, Padding = new Thickness(14, 8),
            MinHeight = 36
        });

        var queryPane = new Grid
        {
            Margin = new Thickness(0, 12),
            RowDefinitions = new RowDefinitions("Auto,12,Auto,12,*")
        };
        queryPane.Children.Add(_sql);
        var commands = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
        commands.Children.Add(Button("执行查询（只读）", () => _ = QueryAsync()));
        commands.Children.Add(Button("执行写入 SQL（需确认）", () => _ = ExecuteAsync(), danger: true));
        Grid.SetRow(commands, 2); queryPane.Children.Add(commands);
        var results = new TextBox
        {
            Name = "SqlQueryResultText", IsReadOnly = true,
            AcceptsReturn = true, TextWrapping = TextWrapping.NoWrap,
            FontFamily = new FontFamily("Menlo,Consolas,monospace"), FontSize = 12
        };
        results.Classes.Add("dark-console");
        _queryResults = results;
        Grid.SetRow(results, 4); queryPane.Children.Add(results);
        tabs.Items.Add(new TabItem
        {
            Header = "SQL 查询器", Content = queryPane,
            FontSize = 14, Padding = new Thickness(14, 8),
            MinHeight = 36
        });
        tabs.SelectedIndex = 0;

        Grid.SetRow(_status, 4); root.Children.Add(_status);
        Content = root;
        _status.Text = "就绪 · 查看元数据不会修改数据库";
    }

    private TextBox _schemaDisplay = null!;
    private TextBox _queryResults = null!;

    private async Task LoadSchemaTypesAsync()
    {
        try
        {
            _status.Text = "正在读取架构元数据集合…";
            var values = await Task.Run(() => DAL.Create(_connection).Db.CreateMetaData()
                .MetaDataCollections.ToArray());
            _schemaTypes.ItemsSource = values;
            _schemaTypes.SelectedIndex = values.Length > 0 ? 0 : -1;
            _status.Text = $"已读取 {values.Length} 个元数据集合";
        }
        catch (Exception ex) { Error(ex); }
    }

    private async Task LoadTablesAsync()
    {
        try
        {
            _status.Text = "正在读取数据库表…";
            var list = await Task.Run(() => DAL.Create(_connection).Db.CreateMetaData().GetTables());
            _schemaTables.ItemsSource = list.ToArray();
            _schemaTables.SelectedIndex = list.Count > 0 ? 0 : -1;
            _status.Text = $"已读取 {list.Count} 张数据表";
        }
        catch (Exception ex) { Error(ex); }
    }

    private async Task LoadSchemaCollectionAsync()
    {
        if (_schemaTypes.SelectedItem is not string collection) return;
        try
        {
            _status.Text = $"正在获取 {collection}…";
            var data = await Task.Run(() =>
                DAL.Create(_connection).Db.CreateSession().GetSchema(null, collection, null));
            _schemaDisplay.Text = FormatResult(data);
            _status.Text = $"已读取 {collection} 元数据";
        }
        catch (Exception ex) { Error(ex); }
    }

    private async Task QueryAsync()
    {
        var sql = _sql.Text?.Trim();
        if (string.IsNullOrWhiteSpace(sql)) return;
        // 仅提供保守只读查询入口；无法安全判断的语句一律要求使用明确写入入口。
        if (!sql.StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase) &&
            !sql.StartsWith("WITH ", StringComparison.OrdinalIgnoreCase))
        {
            _status.Text = "只读查询仅允许 SELECT / WITH 开头；其他 SQL 请使用需要确认的执行入口";
            return;
        }
        // 有些数据库支持 SELECT INTO、锁行、或通过分号拼接写入语句。
        // “查询”按钮只走保守单条只读路径，其余交给需要二次确认的入口。
        var sqlWithoutTrailing = sql.TrimEnd().TrimEnd(';');
        if (sqlWithoutTrailing.Contains(';') ||
            System.Text.RegularExpressions.Regex.IsMatch(sql,
                @"\b(INTO|FOR\s+UPDATE|INSERT|UPDATE|DELETE|DROP|ALTER|CREATE|TRUNCATE|EXEC(?:UTE)?)\b",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            _status.Text = "该 SQL 可能包含写入、锁定或多语句；请使用需要确认的 SQL 执行入口";
            return;
        }
        try
        {
            _status.Text = "查询中…";
            var stopwatch = Stopwatch.StartNew();
            var ds = await Task.Run(() => DAL.Create(_connection).Session.Query(sql));
            _queryResults.Text = ds.Tables.Count > 0 ? FormatResult(ds.Tables[0]) : "查询完成，没有结果表";
            stopwatch.Stop();
            _status.Text = $"查询完成 · {ds.Tables.Count} 个结果表 · 用时 {stopwatch.ElapsedMilliseconds}ms";
        }
        catch (Exception ex) { Error(ex); }
    }

    private async Task ExecuteAsync()
    {
        var sql = _sql.Text?.Trim();
        if (string.IsNullOrWhiteSpace(sql)) return;
        var confirmation = await ConfirmDialog.ShowAsync(this, "确认执行写入 SQL？",
            $"将在数据库“{_connection}”执行当前 SQL，可能修改或删除业务数据。请确认已备份并理解风险。\n\nSQL 摘要：{sql[..Math.Min(180, sql.Length)]}",
            "确认执行 SQL");
        if (!confirmation) return;
        try
        {
            _status.Text = "正在执行 SQL…";
            var n = await Task.Run(() => DAL.Create(_connection).Session.Execute(sql));
            _queryResults.Text = $"执行成功，影响 {n} 行";
            _status.Text = $"SQL 执行完成，影响 {n} 行";
        }
        catch (Exception ex) { Error(ex); }
    }

    public static string FormatResult(DataTable? data)
    {
        if (data == null) return "没有返回数据";
        var cols = data.Columns.Cast<DataColumn>().Take(16).ToArray();
        var sb = new System.Text.StringBuilder();
        sb.AppendLine(string.Join("  |  ", cols.Select(c => c.ColumnName)));
        sb.AppendLine(new string('-', 86));
        foreach (DataRow row in data.Rows.Cast<DataRow>().Take(300))
            sb.AppendLine(string.Join("  |  ", cols.Select(c =>
                (row[c]?.ToString() ?? "").Replace("\r", " ").Replace("\n", " ")[..Math.Min(90, (row[c]?.ToString() ?? "").Length)])));
        if (data.Rows.Count > 300) sb.AppendLine($"… 只显示前 300 行，共 {data.Rows.Count} 行");
        return sb.ToString();
    }

    private void Error(Exception ex)
    {
        _status.Text = $"操作失败：{ex.Message}";
    }

#if DEBUG
    public void SmokeTestOffline()
    {
        if (!_status.Text!.Contains("就绪") || _schemaTables == null || _queryResults == null)
            throw new InvalidOperationException("架构/SQL 查询器窗口初始化失败");
        var example = new DataTable();
        example.Columns.Add("ID");
        example.Columns.Add("Name");
        example.Rows.Add("1", "测试");
        if (!FormatResult(example).Contains("测试"))
            throw new InvalidOperationException("SQL 结果格式化错误");
        Console.WriteLine("DATABASE SCHEMA/SQL WORKBENCH OFFLINE OK: tabs, rows, no database accessed");
    }
#endif
}
