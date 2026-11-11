using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using XCode.DataAccessLayer;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using NewLife;
using XCoderAv.ViewModels;
using XCoderMac.Dialogs;
using XCoderMac.Services;

namespace XCoderMac.Views;

/// <summary>复用 NewLife.XCode 数据建模 ViewModel 的 Mac 窗口。</summary>
public partial class MacDataModelingWindow : Window
{
    private DataModelingViewModel ViewModel => (DataModelingViewModel)DataContext!;

    public MacDataModelingWindow()
    {
        InitializeComponent();
        DataContext = new DataModelingViewModel();
    }

    // 注意：内容被迁移进主窗口标签页后，源 Window 不会显示，
    // 因此必须监听实际嵌入页面 PageViewport 的尺寸，而不是 Window.SizeChanged。
    private void PageViewport_SizeChanged(object? sender, SizeChangedEventArgs e) =>
        UpdateResponsiveLayout(e.NewSize.Width);

    private bool? _configurationSideBySide;
    private bool? _structureSideBySide;

    private void UpdateResponsiveLayout(double viewportWidth)
    {
        if (viewportWidth <= 0 || double.IsNaN(viewportWidth) || double.IsInfinity(viewportWidth)) return;

        // ScrollViewer 的水平视口包含滚动区域；扣除 StackPanel 左右各 25px 的空白。
        var contentWidth = viewportWidth - 50;
        var wideConfiguration = contentWidth >= 870;
        var wideStructure = contentWidth >= 800;

        if (_configurationSideBySide != wideConfiguration)
        {
            _configurationSideBySide = wideConfiguration;
            ConfigurationGrid.ColumnDefinitions = wideConfiguration
                ? new ColumnDefinitions("0.85*,16,1.5*")
                : new ColumnDefinitions("*");
            ConfigurationGrid.RowDefinitions = wideConfiguration
                ? new RowDefinitions("Auto")
                : new RowDefinitions("Auto,16,Auto");
            Grid.SetColumn(ConnectionPanel, 0);
            Grid.SetRow(ConnectionPanel, 0);
            Grid.SetColumn(GenerationPanel, wideConfiguration ? 2 : 0);
            Grid.SetRow(GenerationPanel, wideConfiguration ? 0 : 2);
        }

        if (_structureSideBySide != wideStructure)
        {
            _structureSideBySide = wideStructure;
            StructureGrid.ColumnDefinitions = wideStructure
                ? new ColumnDefinitions("1*,16,1.3*")
                : new ColumnDefinitions("*");
            StructureGrid.RowDefinitions = wideStructure
                ? new RowDefinitions("Auto")
                : new RowDefinitions("Auto,16,Auto");
            Grid.SetColumn(TablesPanel, 0);
            Grid.SetRow(TablesPanel, 0);
            Grid.SetColumn(FieldsPanel, wideStructure ? 2 : 0);
            Grid.SetRow(FieldsPanel, wideStructure ? 0 : 2);
        }
    }

    private async void ConnectionSettings_Click(object? sender, RoutedEventArgs e)
    {
        var owner = (TopLevel.GetTopLevel(sender as Control) as Window) ?? this;
        var dialog = new MacConnectionManagerWindow();
        await dialog.ShowDialog(owner);
        if (dialog.SavedChanges)
        {
            var oldName = ViewModel.ConnName;
            ViewModel.Connections.Clear();
            foreach (var name in XCode.DataAccessLayer.DAL.ConnStrs.Keys.OrderBy(x => x))
                ViewModel.Connections.Add(name);
            ViewModel.ConnName = ViewModel.Connections.Contains(oldName) ? oldName :
                ViewModel.Connections.FirstOrDefault() ?? "";
            ViewModel.Status = $"已重新载入 {ViewModel.Connections.Count} 个数据库连接";
        }
    }

    private async void GenerateAll_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.Tables.Count == 0)
        {
            ViewModel.Status = "请先连接数据库并加载数据表";
            return;
        }

        if (string.IsNullOrWhiteSpace(ViewModel.OutputPath))
        {
            ViewModel.Status = "请先填写代码输出目录";
            return;
        }

        var ok = await ConfirmDialog.ShowAsync((TopLevel.GetTopLevel(sender as Control) as Window) ?? this, "确认生成全部实体代码？",
            $"即将按当前配置，为 {ViewModel.Tables.Count} 张表生成代码，写入目录“{ViewModel.OutputPath}”。已有同名文件可能被覆盖，请确认输出目录并备份。",
            "确认生成");
        if (ok && ViewModel.GenerateAllCommand.CanExecute(null))
            ViewModel.GenerateAllCommand.Execute(null);
    }

    /// <summary>使用当前可见主窗口的 macOS 原生目录选择器，支持嵌入式工具标签。</summary>
    private async void BrowseOutputDirectory_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            // 业务 Window 的 Content 被嵌入主窗体后，源窗口从未 Show，
            // 故必须从被点击按钮查找实际 TopLevel，不能使用 this.StorageProvider。
            var topLevel = TopLevel.GetTopLevel(sender as Control);
            if (topLevel?.StorageProvider is not { CanPickFolder: true } storage)
            {
                ViewModel.Status = "当前窗口不支持选择目录，请直接输入代码输出路径";
                return;
            }

            var options = new FolderPickerOpenOptions
            {
                Title = "选择实体代码输出目录",
                AllowMultiple = false
            };

            // 如果输出目录已经存在，再次选择时从该目录开始。
            var current = ViewModel.OutputPath?.Trim();
            if (!string.IsNullOrWhiteSpace(current))
            {
                try
                {
                    var existing = current.GetFullPath();
                    if (Directory.Exists(existing))
                        options.SuggestedStartLocation = await storage.TryGetFolderFromPathAsync(existing);
                }
                catch (Exception)
                {
                    // 无效的旧路径不会阻断目录选择器，系统默认目录仍可打开。
                }
            }

            var selected = await storage.OpenFolderPickerAsync(options);
            if (selected.Count == 0) return; // 用户取消，不改变已填写的路径。

            var folder = selected[0].TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(folder))
            {
                ViewModel.Status = "所选目录不是本机路径，请选择 Mac 上的本地文件夹";
                return;
            }

            ViewModel.OutputPath = folder;
            ViewModel.Status = $"代码输出目录已设置：{folder}";
        }
        catch (Exception ex)
        {
            ViewModel.Status = $"选择输出目录失败：{ex.Message}";
        }
    }

    // 原 ViewModel 使用 explorer.exe，此处只在 Mac 窗口中用 Finder 打开路径。
    private void OpenOutput_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ViewModel.OutputPath))
            {
                ViewModel.Status = "请先设置输出目录";
                return;
            }

            var path = ViewModel.OutputPath.GetFullPath();
            if (!Directory.Exists(path))
            {
                ViewModel.Status = $"输出目录不存在：{path}";
                return;
            }

            var info = new ProcessStartInfo
            {
                FileName = "open",
                UseShellExecute = false
            };
            info.ArgumentList.Add(path);
            Process.Start(info);
        }
        catch (Exception ex)
        {
            ViewModel.Status = $"打开 Finder 失败：{ex.Message}";
        }
    }

    private Window Owner(object? sender) =>
        (TopLevel.GetTopLevel(sender as Control) as Window) ?? this;

    private void ReplaceModel(IEnumerable<IDataTable> tables, string description)
    {
        var list = tables.ToList();
        ViewModel.Tables.Clear();
        foreach (var table in list.OrderBy(t => t.Name)) ViewModel.Tables.Add(table);
        ViewModel.SelectedTable = ViewModel.Tables.FirstOrDefault();
        ViewModel.Status = $"{description}，共 {list.Count} 张表";
    }

    private async void ImportModel_Click(object? sender, RoutedEventArgs e)
    {
        var owner = Owner(sender);
        try
        {
            var storage = owner.StorageProvider;
            var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "导入 XCode XML 数据模型",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("XCode XML 模型") { Patterns = ["*.xml"] }]
            });
            if (files.Count == 0) return;
            await using var stream = await files[0].OpenReadAsync();
            if (stream.CanSeek && stream.Length > 32 * 1024 * 1024)
                throw new InvalidDataException("模型 XML 不能超过 32MB");
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            var xml = await reader.ReadToEndAsync();
            if (xml.Length > 32 * 1024 * 1024)
                throw new InvalidDataException("模型 XML 不能超过 32MB");
            var loaded = MacDataToolsService.ImportXml(xml);
            if (loaded.Count == 0) throw new InvalidDataException("模型 XML 中没有数据表");
            if (ViewModel.Tables.Count > 0)
            {
                var agreed = await ConfirmDialog.ShowAsync(owner, "替换当前模型？",
                    $"已加载 {loaded.Count} 张表。导入将替换界面当前的 {ViewModel.Tables.Count} 张表（不会改动数据库）。请先导出当前模型备份。", "确认导入");
                if (!agreed) return;
            }
            ReplaceModel(loaded, "已导入 XML 模型");
            ViewModel.IsConnected = false; // 当前列表来源于离线 XML，而非数据库连接。
            // 本地文件模型也允许生成实体，即使没有连接数据库。
            ViewModel.Status += "（离线模型，可直接生成实体）";
        }
        catch (Exception ex) { ViewModel.Status = $"导入模型失败：{ex.Message}"; }
    }

    private async void ExportModel_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var xml = MacDataToolsService.ExportXml(ViewModel.Tables);
            var output = await Owner(sender).StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "导出 XCode XML 模型",
                SuggestedFileName = (string.IsNullOrWhiteSpace(ViewModel.ConnName) ? "Model" : ViewModel.ConnName) + ".xml",
                DefaultExtension = "xml",
                FileTypeChoices = [new FilePickerFileType("XCode XML 模型") { Patterns = ["*.xml"] }]
            });
            if (output == null) return;
            await using var stream = await output.OpenWriteAsync();
            await using var writer = new StreamWriter(stream, new UTF8Encoding(false));
            await writer.WriteAsync(xml);
            ViewModel.Status = $"已导出 {ViewModel.Tables.Count} 张表的 XML 模型";
        }
        catch (Exception ex) { ViewModel.Status = $"导出模型失败：{ex.Message}"; }
    }

    private async void ManageModel_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var editor = new MacModelManagerWindow(ViewModel.Tables);
            if (await editor.ShowDialog<bool>(Owner(sender)))
            {
                ReplaceModel(editor.Tables, "模型编辑已应用");
                ViewModel.Status += "（尚未修改数据库，可导出 XML 保存）";
            }
        }
        catch (Exception ex) { ViewModel.Status = $"模型管理失败：{ex.Message}"; }
    }

    private async void PreviewDdl_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.Tables.Count == 0) { ViewModel.Status = "请先连接数据库或导入 XML 模型"; return; }
        if (string.IsNullOrWhiteSpace(ViewModel.ConnName) ||
            !DAL.ConnStrs.ContainsKey(ViewModel.ConnName))
        {
            ViewModel.Status = "生成数据库专用建表 SQL 需要选择有效连接（不需要执行写入）";
            return;
        }
        try
        {
            var metadata = DAL.Create(ViewModel.ConnName).Db.CreateMetaData();
            var sql = MacDataToolsService.CreateAllSql(metadata, ViewModel.Tables);
            var preview = new MacSqlPreviewWindow($"建表 SQL 预览 · {ViewModel.ConnName}", sql);
            await preview.ShowDialog(Owner(sender));
            ViewModel.Status = $"已生成 {ViewModel.Tables.Count} 张表的建表 SQL 预览（未执行）";
        }
        catch (Exception ex) { ViewModel.Status = $"生成建表 SQL 失败：{ex.Message}"; }
    }

    private async void DatabaseWorkbench_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.ConnName) ||
            !DAL.ConnStrs.ContainsKey(ViewModel.ConnName))
        { ViewModel.Status = "请先选择有效数据库连接"; return; }
        try
        {
            var window = new MacSqlWorkbenchWindow(ViewModel.ConnName);
            await window.ShowDialog(Owner(sender));
        }
        catch (Exception ex) { ViewModel.Status = $"架构管理/SQL 查询失败：{ex.Message}"; }
    }

    private async void ApplyDatabaseSchema_Click(object? sender, RoutedEventArgs e)
    {
        var owner = Owner(sender);
        if (ViewModel.Tables.Count == 0 || !DAL.ConnStrs.ContainsKey(ViewModel.ConnName))
        { ViewModel.Status = "请先选择数据库连接并加载或导入数据模型"; return; }

        // 高风险 DDL：不是预览。必须二次明确确认，并手工输入完整连接名。
        var accepted = await ConfirmDialog.ShowAsync(owner, "高风险：修改数据库架构",
            $"将使用 XCode Migration.Full 更新数据库“{ViewModel.ConnName}”的 {ViewModel.Tables.Count} 张表。此操作可能创建表、修改字段或索引，无法自动撤销。请先备份数据库。", "我已备份，继续");
        if (!accepted) return;
        var confirmation = new Window
        {
            Title = "最终确认数据库目标", Width = 480, Height = 225,
            CanResize = false, WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Background = Avalonia.Media.Brushes.White
        };
        var nameInput = new TextBox { Watermark = "请输入完整数据库连接名称", Height = 39 };
        var execute = new Button { Content = "执行架构变更", IsEnabled = false, Padding = new Avalonia.Thickness(16, 9) };
        var cancel = new Button { Content = "取消", Padding = new Avalonia.Thickness(16, 9) };
        nameInput.TextChanged += (_, _) =>
            execute.IsEnabled = string.Equals(nameInput.Text, ViewModel.ConnName, StringComparison.Ordinal);
        execute.Click += (_, _) => confirmation.Close(true);
        cancel.Click += (_, _) => confirmation.Close(false);
        confirmation.Content = new StackPanel { Margin = new Avalonia.Thickness(22), Spacing = 13,
            Children =
            {
                new TextBlock { Text = $"输入“{ViewModel.ConnName}”确认操作目标", FontSize = 16 },
                nameInput,
                new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 12,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    Children = { cancel, execute } }
            } };
        if (!await confirmation.ShowDialog<bool>(owner)) return;
        try
        {
            MacDataToolsService.ValidateModel(ViewModel.Tables);
            var tables = ViewModel.Tables.ToArray();
            var name = ViewModel.ConnName;
            ViewModel.Status = "正在应用数据库架构变更，请勿关闭应用…";
            await Task.Run(() =>
                DAL.Create(name).Db.CreateMetaData().SetTables(Migration.Full, tables));
            ViewModel.Status = $"架构变更完成，共处理 {tables.Length} 张表；建议重新读取表结构核对";
        }
        catch (Exception ex) { ViewModel.Status = $"应用架构失败：{ex.Message}"; }
    }

}
