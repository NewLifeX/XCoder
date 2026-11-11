using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.VisualTree;
using Avalonia.Layout;
using Avalonia.Media;
using XCode.DataAccessLayer;
using XCoderMac.Services;

namespace XCoderMac.Views;

/// <summary>
/// Mac 版模型管理：对应 WinForms FrmModel 的表/字段/索引管理。
/// 所有操作针对 XML 深度复制，只有点击“应用模型”才会返回给主窗体；不会自动执行数据库 DDL。
/// </summary>
public sealed class MacModelManagerWindow : Window
{
    private readonly List<IDataTable> _tables;
    private readonly ListBox _tablesList = new();
    private readonly ListBox _columnsList = new();
    private readonly ListBox _indexesList = new();
    private readonly TextBlock _message = new() { Foreground = Brush("#6886A4"), FontSize = 12 };
    private readonly TextBox _tableName = Field(), _tablePhysicalName = Field(), _tableDescription = Field();
    private readonly TextBox _columnName = Field(), _columnPhysicalName = Field(),
        _columnDescription = Field(), _columnLength = Field();
    private readonly ComboBox _columnType = new() { Height = 35, HorizontalAlignment = HorizontalAlignment.Stretch };
    private readonly CheckBox _columnPrimary = new() { Content = "主键", Margin = new Thickness(0, 0, 14, 0) };
    private readonly CheckBox _columnIdentity = new() { Content = "自增", Margin = new Thickness(0, 0, 14, 0) };
    private readonly CheckBox _columnNullable = new() { Content = "允许为空" };
    private readonly TextBox _indexName = Field(), _indexColumns = Field();
    private readonly CheckBox _indexUnique = new() { Content = "唯一索引" };
    private readonly CheckBox _indexPrimary = new() { Content = "主键索引" };
    private bool _loading;

    private static readonly string[] Types =
        ["String", "Int32", "Int64", "Int16", "Boolean", "DateTime", "Decimal",
         "Double", "Single", "Guid", "Byte", "Byte[]", "TimeSpan"];
    public IReadOnlyList<IDataTable> Tables => _tables;

    public MacModelManagerWindow(IEnumerable<IDataTable> source)
    {
        _tables = source.Any() ? MacDataToolsService.CloneModel(source) : [];
        Title = "模型管理 · 表 / 字段 / 索引 · XCoder";
        Width = 1130;
        Height = 795;
        MinWidth = 855;
        MinHeight = 600;
        Background = Brush("#F7F9FC");
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        _columnType.ItemsSource = Types;
        _tablesList.ItemTemplate = new FuncDataTemplate<IDataTable>((item, _) =>
            new TextBlock
            {
                Text = item == null ? "" : $"{item.Name}  ·  {item.TableName}",
                Foreground = Brush("#2F577C"), FontSize = 13,
                Margin = new Thickness(8, 9),
                TextTrimming = TextTrimming.CharacterEllipsis
            });
        _columnsList.ItemTemplate = new FuncDataTemplate<IDataColumn>((item, _) =>
            new TextBlock
            {
                Text = item == null ? "" : $"{item.ColumnName}  ·  {item.DataType?.Name}",
                Foreground = Brush("#2F577C"), FontSize = 12,
                Margin = new Thickness(8, 8),
                TextTrimming = TextTrimming.CharacterEllipsis
            });
        _indexesList.ItemTemplate = new FuncDataTemplate<IDataIndex>((item, _) =>
            new TextBlock
            {
                Text = item == null ? "" : $"{item.Name}  ({string.Join(",", item.Columns ?? [])})",
                Foreground = Brush("#2F577C"), FontSize = 12,
                Margin = new Thickness(8, 8),
                TextTrimming = TextTrimming.CharacterEllipsis
            });
        BuildLayout();
        ReloadTables(0);
    }

    private static SolidColorBrush Brush(string color) => new(Color.Parse(color));
    private static TextBox Field() => new()
    {
        Height = 35, BorderBrush = Brush("#DCE6F3"), Background = Brush("#F9FBFD"),
        Foreground = Brush("#294563"), CornerRadius = new CornerRadius(7)
    };

    private static TextBlock Label(string text) => new()
    {
        Text = text, FontSize = 12, Foreground = Brush("#6E849D"),
        Margin = new Thickness(0, 9, 0, 4)
    };

    private static TextBlock Heading(string text) => new()
    {
        Text = text, FontSize = 17, FontWeight = FontWeight.SemiBold,
        Foreground = Brush("#284769"), Margin = new Thickness(0, 0, 0, 9)
    };

    private static Border Panel(Control child) => new()
    {
        Child = child, Padding = new Thickness(15), Background = Brushes.White,
        BorderBrush = Brush("#DCE6F3"), BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(13)
    };

    private static Button Action(string text, Action callback, bool primary = false)
    {
        var button = new Button
        {
            Content = text, Padding = new Thickness(12, 8),
            Background = Brush(primary ? "#0875CF" : "#EAF1FA"),
            Foreground = Brush(primary ? "#FFFFFF" : "#285686"),
            CornerRadius = new CornerRadius(8)
        };
        button.Click += (_, _) => callback();
        return button;
    }

    private void BuildLayout()
    {
        var root = new Grid
        {
            Margin = new Thickness(21),
            RowDefinitions = new RowDefinitions("Auto,12,*,12,Auto")
        };
        root.Children.Add(new StackPanel
        {
            Spacing = 5,
            Children =
            {
                new TextBlock { Text = "模型管理", FontSize = 24, FontWeight = FontWeight.SemiBold, Foreground = Brush("#203D5D") },
                new TextBlock { Text = "编辑副本：数据表、字段和索引可新增、修改、删除；点击“应用模型”才会写回数据建模页面，不会修改数据库。", FontSize = 12, Foreground = Brush("#8095AE") }
            }
        });
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("0.95*,12,1.05*,12,1.6*") };
        Grid.SetRow(grid, 2);
        root.Children.Add(grid);

        var left = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto,*") };
        left.Children.Add(Heading("数据表"));
        var tableButtons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 7 };
        tableButtons.Children.Add(Action("+ 新增表", AddTable));
        tableButtons.Children.Add(Action("删除表", DeleteTable));
        Grid.SetRow(tableButtons, 1);
        left.Children.Add(tableButtons);
        _tablesList.SelectionChanged += (_, _) => ReloadDetails();
        Grid.SetRow(_tablesList, 2);
        _tablesList.Margin = new Thickness(0, 12, 0, 0);
        left.Children.Add(_tablesList);
        var leftPanel = Panel(left);
        grid.Children.Add(leftPanel);

        var middle = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto,2*,12,Auto,Auto,*") };
        middle.Children.Add(Heading("字段"));
        var columnButtons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 7 };
        columnButtons.Children.Add(Action("+ 字段", AddColumn));
        columnButtons.Children.Add(Action("删除字段", DeleteColumn));
        Grid.SetRow(columnButtons, 1); middle.Children.Add(columnButtons);
        _columnsList.SelectionChanged += (_, _) => LoadColumn();
        Grid.SetRow(_columnsList, 2);
        _columnsList.Margin = new Thickness(0, 9, 0, 0); middle.Children.Add(_columnsList);
        var indexTitle = Heading("索引");
        Grid.SetRow(indexTitle, 4); middle.Children.Add(indexTitle);
        var indexButtons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 7 };
        indexButtons.Children.Add(Action("+ 索引", AddIndex));
        indexButtons.Children.Add(Action("删除索引", DeleteIndex));
        Grid.SetRow(indexButtons, 5); middle.Children.Add(indexButtons);
        _indexesList.SelectionChanged += (_, _) => LoadIndex();
        Grid.SetRow(_indexesList, 6);
        _indexesList.Margin = new Thickness(0, 9, 0, 0); middle.Children.Add(_indexesList);
        var middlePanel = Panel(middle);
        Grid.SetColumn(middlePanel, 2); grid.Children.Add(middlePanel);

        var forms = new StackPanel { Spacing = 3 };
        forms.Children.Add(Heading("属性编辑"));
        forms.Children.Add(Label("实体名称")); forms.Children.Add(_tableName);
        forms.Children.Add(Label("数据库表名")); forms.Children.Add(_tablePhysicalName);
        forms.Children.Add(Label("表说明")); forms.Children.Add(_tableDescription);
        forms.Children.Add(Action("应用表属性", ApplyTable));
        forms.Children.Add(new Border { Height = 1, Background = Brush("#E5EDF6"), Margin = new Thickness(0, 12) });
        forms.Children.Add(Heading("字段属性"));
        forms.Children.Add(Label("字段实体名")); forms.Children.Add(_columnName);
        forms.Children.Add(Label("数据库列名")); forms.Children.Add(_columnPhysicalName);
        forms.Children.Add(Label("类型")); forms.Children.Add(_columnType);
        forms.Children.Add(Label("长度")); forms.Children.Add(_columnLength);
        forms.Children.Add(Label("说明")); forms.Children.Add(_columnDescription);
        forms.Children.Add(new StackPanel { Orientation = Orientation.Horizontal,
            Children = { _columnPrimary, _columnIdentity, _columnNullable } });
        forms.Children.Add(Action("应用字段属性", ApplyColumn));
        forms.Children.Add(new Border { Height = 1, Background = Brush("#E5EDF6"), Margin = new Thickness(0, 12) });
        forms.Children.Add(Heading("索引属性"));
        forms.Children.Add(Label("索引名")); forms.Children.Add(_indexName);
        forms.Children.Add(Label("列名（用逗号分隔）")); forms.Children.Add(_indexColumns);
        forms.Children.Add(new StackPanel { Orientation = Orientation.Horizontal, Children = { _indexUnique, _indexPrimary } });
        forms.Children.Add(Action("应用索引属性", ApplyIndex));

        var scroll = new ScrollViewer { Content = forms, VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto };
        var rightPanel = Panel(scroll);
        Grid.SetColumn(rightPanel, 4); grid.Children.Add(rightPanel);

        var footer = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto,12,Auto") };
        footer.Children.Add(_message);
        Grid.SetColumn(_message, 0);
        var cancel = Action("取消", () => Close(false));
        Grid.SetColumn(cancel, 1); footer.Children.Add(cancel);
        var save = Action("应用模型", Save, true);
        Grid.SetColumn(save, 3); footer.Children.Add(save);
        Grid.SetRow(footer, 4); root.Children.Add(footer);
        Content = root;
    }

    private IDataTable? CurrentTable => _tablesList.SelectedItem as IDataTable;
    private IDataColumn? CurrentColumn => _columnsList.SelectedItem as IDataColumn;
    private IDataIndex? CurrentIndex => _indexesList.SelectedItem as IDataIndex;

    private void ReloadTables(int index)
    {
        _tablesList.ItemsSource = null;
        _tablesList.ItemsSource = _tables.ToArray();
        _tablesList.SelectedIndex = Math.Min(index, _tables.Count - 1);
        _message.Text = $"模型包含 {_tables.Count} 张表；所有更改均为本地草稿";
        ReloadDetails();
    }

    private void ReloadDetails()
    {
        _loading = true;
        var table = CurrentTable;
        _tableName.Text = table?.Name ?? "";
        _tablePhysicalName.Text = table?.TableName ?? "";
        _tableDescription.Text = table?.Description ?? "";
        _columnsList.ItemsSource = null;
        _columnsList.ItemsSource = table?.Columns.ToArray() ?? [];
        _indexesList.ItemsSource = null;
        _indexesList.ItemsSource = table?.Indexes.ToArray() ?? [];
        _loading = false;
        _columnsList.SelectedIndex = table?.Columns.Count > 0 ? 0 : -1;
        _indexesList.SelectedIndex = table?.Indexes.Count > 0 ? 0 : -1;
        LoadColumn(); LoadIndex();
    }

    private void LoadColumn()
    {
        if (_loading) return;
        var field = CurrentColumn;
        _columnName.Text = field?.Name ?? "";
        _columnPhysicalName.Text = field?.ColumnName ?? "";
        _columnDescription.Text = field?.Description ?? "";
        _columnLength.Text = field?.Length.ToString() ?? "0";
        var typeName = field?.DataType == typeof(byte[]) ? "Byte[]" :
            field?.DataType?.FullName ?? "System.String";
        var normalized = typeName.StartsWith("System.", StringComparison.Ordinal) ?
            typeName["System.".Length..] : typeName;
        if (!Types.Contains(normalized) && _columnType.ItemsSource is string[] options)
            _columnType.ItemsSource = options.Append(normalized).ToArray();
        _columnType.SelectedItem = normalized;
        _columnPrimary.IsChecked = field?.PrimaryKey ?? false;
        _columnIdentity.IsChecked = field?.Identity ?? false;
        _columnNullable.IsChecked = field?.Nullable ?? true;
    }

    private void LoadIndex()
    {
        if (_loading) return;
        var index = CurrentIndex;
        _indexName.Text = index?.Name ?? "";
        _indexColumns.Text = index?.Columns is { } columns ? string.Join(", ", columns) : "";
        _indexUnique.IsChecked = index?.Unique ?? false;
        _indexPrimary.IsChecked = index?.PrimaryKey ?? false;
    }

    private static string NewTableXml(string name) =>
        $"<EntityModel xmlns=\"https://newlifex.com/Model202509.xsd\"><Tables><Table Name=\"{name}\"><Columns><Column Name=\"ID\" DataType=\"Int32\" PrimaryKey=\"True\" /></Columns></Table></Tables></EntityModel>";

    private void AddTable()
    {
        try
        {
            var n = 1;
            while (_tables.Any(t => t.Name == $"NewTable{n}")) n++;
            _tables.Add(MacDataToolsService.ImportXml(NewTableXml($"NewTable{n}")).Single());
            ReloadTables(_tables.Count - 1);
        }
        catch (Exception ex) { Error(ex); }
    }

    private void DeleteTable()
    {
        if (CurrentTable is not { } table) return;
        var index = _tables.IndexOf(table);
        _tables.Remove(table);
        ReloadTables(Math.Max(index - 1, 0));
    }

    private void AddColumn()
    {
        if (CurrentTable is not { } table) { _message.Text = "请先选择或新增一张表"; return; }
        var n = 1;
        while (table.Columns.Any(c => c.Name == $"Column{n}")) n++;
        var column = table.CreateColumn();
        column.Name = $"Column{n}";
        column.ColumnName = column.Name;
        column.DataType = typeof(string);
        table.Columns.Add(column);
        ReloadDetails();
        _columnsList.SelectedItem = column;
    }

    private void DeleteColumn()
    {
        if (CurrentTable is not { } table || CurrentColumn is not { } column) return;
        if (table.Indexes.Any(i => i.Columns.Contains(column.ColumnName, StringComparer.OrdinalIgnoreCase)))
        {
            _message.Text = "该字段被索引引用，请先修改或删除对应索引";
            return;
        }
        table.Columns.Remove(column);
        ReloadDetails();
    }

    private void AddIndex()
    {
        if (CurrentTable is not { } table || table.Columns.Count == 0)
        { _message.Text = "请先新增字段"; return; }
        var index = table.CreateIndex();
        index.Columns = [table.Columns[0].ColumnName];
        index.Name = $"IX_{table.Name}_{table.Indexes.Count + 1}";
        table.Indexes.Add(index);
        ReloadDetails();
        _indexesList.SelectedItem = index;
    }

    private void DeleteIndex()
    {
        if (CurrentTable is not { } table || CurrentIndex is not { } index) return;
        table.Indexes.Remove(index);
        ReloadDetails();
    }

    private void ApplyTable()
    {
        if (CurrentTable is not { } t) return;
        if (string.IsNullOrWhiteSpace(_tableName.Text) ||
            string.IsNullOrWhiteSpace(_tablePhysicalName.Text)) { _message.Text = "实体名和数据库表名不能为空"; return; }
        t.Name = _tableName.Text.Trim();
        t.TableName = _tablePhysicalName.Text.Trim();
        t.Description = _tableDescription.Text ?? "";
        ReloadTables(_tables.IndexOf(t));
    }

    private void ApplyColumn()
    {
        if (CurrentTable is not { } t || CurrentColumn is not { } c) return;
        if (string.IsNullOrWhiteSpace(_columnName.Text) || string.IsNullOrWhiteSpace(_columnPhysicalName.Text))
        { _message.Text = "字段名不能为空"; return; }
        if (!int.TryParse(_columnLength.Text, out var length) || length < 0)
        { _message.Text = "长度必须是非负整数"; return; }
        var name = _columnPhysicalName.Text.Trim();
        if (t.Columns.Any(other => other != c && other.ColumnName.Equals(name, StringComparison.OrdinalIgnoreCase)))
        { _message.Text = "同一表不能存在重复列名"; return; }
        var type = _columnType.SelectedItem?.ToString() ?? "String";
        c.Name = _columnName.Text.Trim();
        c.ColumnName = name;
        var dataType = type == "Byte[]" ? typeof(byte[]) :
            Type.GetType(type.StartsWith("System.", StringComparison.Ordinal) ? type : "System." + type);
        if (dataType == null) { _message.Text = $"未知 .NET 字段类型：{type}"; return; }
        c.DataType = dataType;
        c.Length = length;
        c.Description = _columnDescription.Text ?? "";
        c.PrimaryKey = _columnPrimary.IsChecked == true;
        c.Identity = _columnIdentity.IsChecked == true;
        c.Nullable = _columnNullable.IsChecked == true;
        ReloadDetails();
        _columnsList.SelectedItem = c;
    }

    private void ApplyIndex()
    {
        if (CurrentTable is not { } t || CurrentIndex is not { } index) return;
        var columns = (_indexColumns.Text ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (columns.Length == 0 || columns.Any(name =>
            t.Columns.All(column => !column.ColumnName.Equals(name, StringComparison.OrdinalIgnoreCase))))
        { _message.Text = "索引引用了不存在的字段，请使用数据库列名并以逗号分隔"; return; }
        index.Name = _indexName.Text?.Trim() ?? "";
        index.Columns = columns;
        index.Unique = _indexUnique.IsChecked == true;
        index.PrimaryKey = _indexPrimary.IsChecked == true;
        ReloadDetails();
        _indexesList.SelectedItem = index;
    }

    private void Save()
    {
        try
        {
            MacDataToolsService.ValidateModel(_tables);
            // Export/Import 往返验证结构兼容，出错则不关闭编辑窗口。
            _ = MacDataToolsService.CloneModel(_tables);
            Close(true);
        }
        catch (Exception ex) { Error(ex); }
    }

    private void Error(Exception ex) => _message.Text = $"操作失败：{ex.Message}";

#if DEBUG
    public void AssertVisibleLists()
    {
        if (_tablesList.GetVisualDescendants().OfType<TextBlock>().Count(t => !string.IsNullOrWhiteSpace(t.Text)) < _tables.Count ||
            _columnsList.GetVisualDescendants().OfType<TextBlock>().Count(t => !string.IsNullOrWhiteSpace(t.Text)) < (CurrentTable?.Columns.Count ?? 0) ||
            _indexesList.GetVisualDescendants().OfType<TextBlock>().Count(t => !string.IsNullOrWhiteSpace(t.Text)) < (CurrentTable?.Indexes.Count ?? 0))
            throw new InvalidOperationException("模型管理的表/字段/索引列表尚未完整渲染");
        Console.WriteLine("MODEL EDITOR RENDER OK: tables, columns and indexes visible");
    }

    public void SmokeTestOfflineEditor()
    {
        var oldCount = _tables.Count;
        AddTable();
        if (_tables.Count != oldCount + 1) throw new InvalidOperationException("新建数据表失败");
        AddColumn();
        if (CurrentTable is not { } t || t.Columns.Count < 2)
            throw new InvalidOperationException("新建字段失败");
        AddIndex();
        if (t.Indexes.Count < 1) throw new InvalidOperationException("新建索引失败");
        var clone = MacDataToolsService.CloneModel(_tables);
        MacDataToolsService.ValidateModel(clone);
        Console.WriteLine($"MODEL EDITOR OFFLINE OK: tables={clone.Count}, fields={t.Columns.Count}, indexes={t.Indexes.Count}");
    }
#endif
}
