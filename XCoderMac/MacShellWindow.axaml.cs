using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Threading;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using XCoderAv.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;
using System.IO;
using XCoderMac.Services;

namespace XCoderMac;

/// <summary>
/// macOS 专用工具中心：只负责分类、检索与启动现有工具。
/// 工具业务实现仍完全复用 XCoderAv。
/// </summary>
public partial class MacShellWindow : Window
{
    private string? _selectedCategory;
    private Button? _activeNavigationButton;

    // 与 ToolCards 实际宽度绑定的响应式列数，使用 Avalonia 属性通知 UI 自动重排。
    public static readonly StyledProperty<int> ToolColumnsProperty =
        AvaloniaProperty.Register<MacShellWindow, int>(nameof(ToolColumns), 4);

    public int ToolColumns
    {
        get => GetValue(ToolColumnsProperty);
        private set => SetValue(ToolColumnsProperty, value);
    }

    private static int GetToolColumnCount(double availableWidth) =>
        availableWidth >= 940 ? 4 :
        availableWidth >= 690 ? 3 :
        availableWidth >= 460 ? 2 : 1;

    private void ToolCards_SizeChanged(object? sender, SizeChangedEventArgs e) =>
        UpdateToolColumns(e.NewSize.Width);

    private void UpdateToolColumns(double availableWidth)
    {
        if (availableWidth <= 0 || double.IsInfinity(availableWidth)) return;
        ToolColumns = GetToolColumnCount(availableWidth);
    }

    private void SelectNavigation(Button? button)
    {
        if (button == null) return;
        _activeNavigationButton?.Classes.Remove("selected");
        button.Classes.Add("selected");
        _activeNavigationButton = button;
    }

    public ObservableCollection<MacTool> VisibleTools { get; } = new();
    public ObservableCollection<MacTool> QuickTools { get; } = new();
    private INotifyPropertyChanged? _activeStatusNotifier;
    private object? _activeStatusSource;
    public ObservableCollection<MacToolTab> OpenTabs { get; } = new();
    private MacToolTab? _activeTab;
    public MacCategory[] Categories => MacToolCatalog.Categories;
    public int CategoryCount => MacToolCatalog.Categories.Length;

    public MacShellWindow()
    {
        InitializeComponent();
        DataContext = this;
        _activeNavigationButton = AllToolsNavButton;
        SetQuickEntries(MacQuickEntryStore.Load());
        OpenTabs.CollectionChanged += (_, _) => UpdateSidebarOpenTabs();
        // ToolCards.SizeChanged 会在首次布局和后续缩放时计算列数，无需持续监听每一帧布局。
        // 即使焦点处于搜索文本框，也能响应 ⌘F。
        AddHandler(InputElement.KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
        RefreshTools();
        Closed += (_, _) =>
        {
            foreach (var tab in OpenTabs.ToArray()) CloseToolTab(tab);
        };
    }

    private void ShowHome()
    {
        if (HomeView == null || TabbedWorkspace == null) return;
        HomeView.IsVisible = true;
        TabbedWorkspace.IsVisible = false;
        UpdateSidebarOpenTabs();
        UpdateToolStatus();
    }

    private void ShowWorkspace()
    {
        if (OpenTabs.Count == 0) { ShowHome(); return; }
        HomeView.IsVisible = false;
        TabbedWorkspace.IsVisible = true;
        UpdateSidebarOpenTabs();
        UpdateToolStatus();
    }

    private void UpdateSidebarOpenTabs()
    {
        if (SidebarOpenTabsSection == null) return;
        SidebarOpenTabsSection.IsVisible = OpenTabs.Count > 0;
        SidebarOpenTabsTitle.Text = $"已打开工具 ({OpenTabs.Count})";
    }


    private void AllTools_Click(object? sender, RoutedEventArgs e)
    {
        _selectedCategory = null;
        SelectNavigation(AllToolsNavButton);
        RefreshTools();
        ShowHome();
    }

    private void Category_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: MacCategory category }) return;
        ShowCategory(category.Name, sender as Button);
    }

    private void ShowCategory(string categoryName, Button? selected)
    {
        _selectedCategory = categoryName;
        // 旧搜索条件不能悄悄过滤分类结果，否则看起来像“数据工具没有内容”。
        if (SearchBox != null && !string.IsNullOrEmpty(SearchBox.Text))
            SearchBox.Text = "";
        SelectNavigation(selected);
        RefreshTools();
        ShowHome();
        // 选择分类时直接展示其工具卡片，不再让用户先滚过首页横幅。
        if (HomeView != null) HomeView.Offset = Vector.Zero;
    }

    private void SearchBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        RefreshTools();
        if (SearchBox != null && !string.IsNullOrWhiteSpace(SearchBox.Text))
            ShowHome();
    }

    private void RefreshTools()
    {
        // 初始化时 TextChanged 可能先触发；常用工具仅在真正“所有工具”首页显示。
        if (ToolCount == null || SectionTitle == null || SearchBox == null) return;

        var query = SearchBox.Text?.Trim();
        var tools = MacToolCatalog.Tools.AsEnumerable();

        if (_selectedCategory != null)
            tools = tools.Where(t => t.Category == _selectedCategory);
        if (!string.IsNullOrWhiteSpace(query))
            tools = tools.Where(t =>
                t.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Category.Contains(query, StringComparison.OrdinalIgnoreCase));

        VisibleTools.Clear();
        foreach (var tool in tools) VisibleTools.Add(tool);

        var isAllToolsHome = _selectedCategory == null && string.IsNullOrWhiteSpace(query);
        QuickToolsSection.IsVisible = isAllToolsHome;
        HomeHeroSection.IsVisible = isAllToolsHome;
        HomeStatsSection.IsVisible = isAllToolsHome;
        SectionTitle.Text = !string.IsNullOrWhiteSpace(query) ? "工具搜索" :
            _selectedCategory ?? "探索全部工具";
        SectionDescription.Text = !string.IsNullOrWhiteSpace(query) ?
            $"与“{query}”相关的工具" :
            _selectedCategory != null ? "此分类下的工具都可直接打开使用。" :
            "选择一个工具开始工作，已有功能直接复用原项目。";

        ToolCount.Text = $"{VisibleTools.Count} 个工具";
        EmptyMessage.IsVisible = VisibleTools.Count == 0;
    }

    private void SetQuickEntries(IEnumerable<string> names)
    {
        QuickTools.Clear();
        foreach (var title in names.Distinct(StringComparer.Ordinal))
        {
            var entry = MacToolCatalog.Tools.FirstOrDefault(tool => tool.Title == title);
            if (entry != null) QuickTools.Add(entry);
        }
        if (QuickEntryEmptyMessage != null)
            QuickEntryEmptyMessage.IsVisible = QuickTools.Count == 0;
    }

    private async void ManageQuickEntries_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new Views.MacQuickEntrySettingsWindow(QuickTools.Select(entry => entry.Title));
        var confirmed = await dialog.ShowDialog<bool>(this);
        if (!confirmed) return;
        try
        {
            var selection = dialog.ResultTitles;
            MacQuickEntryStore.Save(selection);
            SetQuickEntries(selection);
            WorkspaceStatusTitle.Text = "快捷入口";
            WorkspaceStatusText.Text = $"已保存 {selection.Length} 个快捷入口";
        }
        catch (Exception ex)
        {
            WorkspaceStatusTitle.Text = "快捷入口";
            WorkspaceStatusText.Text = $"保存快捷入口失败：{ex.Message}";
            Console.Error.WriteLine($"保存快捷入口失败：{ex}");
        }
    }

    private void ToolCard_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: MacTool tool }) OpenTool(tool);
    }

    private void StartModeling_Click(object? sender, RoutedEventArgs e)
    {
        OpenTool(MacToolCatalog.Tools[0]);
    }

    private void OpenTool(MacTool tool)
    {
        var existing = OpenTabs.FirstOrDefault(t => t.Tool.WindowType == tool.WindowType);
        if (existing != null) { ActivateToolTab(existing); return; }

        try
        {
            // 业务 Window 仅作为 ViewModel 和事件源；不会 Show/ShowDialog。
            var source = tool.WindowType.Namespace == "XCoderAv.Views"
                ? new Views.MacLegacyToolWindow(tool)
                : Activator.CreateInstance(tool.WindowType) as Window;
            if (source == null)
                throw new InvalidOperationException("无法初始化工具窗口");
            if (source.Content is not Control view)
                throw new InvalidOperationException("工具未提供可嵌入标签页的 UI");

            source.Content = null;
            if (source.DataContext != null) view.DataContext = source.DataContext;

            // Avalonia 样式只能有一个父级；从源 Window 迁移到嵌入式视图。
            var styles = source.Styles.ToArray();
            source.Styles.Clear();
            foreach (var style in styles) view.Styles.Add(style);

            var tab = new MacToolTab(tool, source, view);
            OpenTabs.Add(tab);
            ActivateToolTab(tab);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"打开工具标签失败 {tool.Title}: {ex}");
            var errorWindow = new Window
            {
                Title = "工具启动失败",
                Width = 465, Height = 220,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new TextBlock
                {
                    Margin = new Thickness(24),
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    Text = $"{tool.Title} 暂时无法启动。\n\n{ex.Message}"
                }
            };
            _ = errorWindow.ShowDialog(this);
        }
    }

    private void ActivateToolTab(MacToolTab tab)
    {
        if (!OpenTabs.Contains(tab)) return;
        if (!ReferenceEquals(_activeTab, tab))
        {
            ToolTabContent.Content = null;
            _activeTab = tab;
            ToolTabContent.Content = tab.Content;
        }
        foreach (var item in OpenTabs)
            item.IsActive = ReferenceEquals(item, tab);
        ShowWorkspace();
        ObserveActiveToolStatus();
        RevealActiveTab(tab);
        if (tab.SourceWindow is Views.MacSpeechWindow speech)
            _ = speech.EnsureTabVoicesAsync();
    }

    private bool QuickToolsNavExists() => this.GetVisualDescendants()
        .OfType<Button>().Any(button => button.Name == "QuickToolsNavButton");

    private void SidebarTab_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: MacToolTab tab })
            ActivateToolTab(tab);
        e.Handled = true;
    }

    private void SidebarCloseTab_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: MacToolTab tab })
            CloseToolTab(tab);
        e.Handled = true;
    }

    /// <summary>
    /// 主状态栏统一监听活动工具的 Status/StatusText，切换标签即切换数据源。
    /// 无状态属性的旧工具只显示“已打开”，不凭空制造业务状态。
    /// </summary>
    private void ObserveActiveToolStatus()
    {
        ClearStatusSubscriptions();
        if (_activeTab == null) { UpdateToolStatus(); return; }

        var source = _activeTab.SourceWindow.DataContext;
        if (_activeTab.SourceWindow is Views.MacLegacyToolWindow legacy)
            source = legacy.HostedViewModel;

        // Redis 等少数窗口使用 DataContext=this，并把真实状态存放在 Vm.Status。
        // 优先选择真正具有 Status/StatusText 属性的对象，不能只观察其外层 Window。
        if (!HasStatusProperty(source))
        {
            var nested = source?.GetType().GetProperty("Vm")?.GetValue(source) ??
                         source?.GetType().GetProperty("ViewModel")?.GetValue(source);
            if (HasStatusProperty(nested)) source = nested;
        }
        _activeStatusSource = source;
        if (source is INotifyPropertyChanged notifier)
        {
            _activeStatusNotifier = notifier;
            notifier.PropertyChanged += OnActiveToolStatusChanged;
        }

        // 有些 macOS 原生工具使用 View 自身的 StatusText（例如语音助手）。
        // 源窗口仍存活，因此可以通过 Window 的通知订阅更新。
        if (_activeTab.SourceWindow is INotifyPropertyChanged windowNotifier &&
            !ReferenceEquals(windowNotifier, source) &&
            HasStatusProperty(_activeTab.SourceWindow))
        {
            _activeStatusWindowNotifier = windowNotifier;
            windowNotifier.PropertyChanged += OnActiveToolStatusChanged;
        }
        UpdateToolStatus();
    }

    private INotifyPropertyChanged? _activeStatusWindowNotifier;

    private static bool HasStatusProperty(object? source) =>
        source?.GetType().GetProperty("Status", BindingFlags.Public | BindingFlags.Instance) != null ||
        source?.GetType().GetProperty("StatusText", BindingFlags.Public | BindingFlags.Instance) != null;

    private static string? ReadStatus(object? source)
    {
        if (source == null) return null;
        foreach (var name in new[] { "Status", "StatusText" })
        {
            var prop = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (prop?.PropertyType == typeof(string) && prop.GetValue(source) is string value &&
                !string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }
        // 原生协议工具没有文本 Status 时，使用它们的真实连接属性。
        var isConnected = source.GetType().GetProperty("IsConnected",
            BindingFlags.Instance | BindingFlags.Public);
        if (isConnected?.PropertyType == typeof(bool) &&
            isConnected.GetValue(source) is bool connected)
            return connected ? "已连接" : "未连接";
        return null;
    }

    private void OnActiveToolStatusChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.PropertyName) &&
            e.PropertyName != "Status" && e.PropertyName != "StatusText" &&
            e.PropertyName != "IsConnected" && e.PropertyName != "ProgressText")
            return;
        if (Dispatcher.UIThread.CheckAccess()) UpdateToolStatus();
        else Dispatcher.UIThread.Post(UpdateToolStatus);
    }

    private void UpdateToolStatus()
    {
        if (WorkspaceStatusTitle == null || WorkspaceStatusText == null) return;
        if (HomeView.IsVisible || _activeTab == null)
        {
            WorkspaceStatusTitle.Text = "工作台";
            WorkspaceStatusText.Text = OpenTabs.Count == 0
                ? "就绪 · 请选择一个工具" : $"就绪 · 已打开 {OpenTabs.Count} 个工具";
            return;
        }

        WorkspaceStatusTitle.Text = $"◉ {_activeTab.Title}";
        WorkspaceStatusText.Text = ReadStatus(_activeStatusSource) ??
            ReadStatus(_activeTab.SourceWindow) ?? "已打开 · 等待操作";
    }

    private void ClearStatusSubscriptions()
    {
        if (_activeStatusNotifier != null)
            _activeStatusNotifier.PropertyChanged -= OnActiveToolStatusChanged;
        if (_activeStatusWindowNotifier != null)
            _activeStatusWindowNotifier.PropertyChanged -= OnActiveToolStatusChanged;
        _activeStatusNotifier = null;
        _activeStatusWindowNotifier = null;
        _activeStatusSource = null;
    }

    private void TabHeader_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // 标签本体不是 Button，因此不存在按下后盖住标题的背景。
        if (sender is Border { Tag: MacToolTab tab } &&
            e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            ActivateToolTab(tab);
            e.Handled = true;
        }
    }

    private void ScrollTabsLeft_Click(object? sender, RoutedEventArgs e) => ScrollTabsBy(-240);
    private void ScrollTabsRight_Click(object? sender, RoutedEventArgs e) => ScrollTabsBy(240);

    private void ScrollTabsBy(double delta)
    {
        if (TabsScrollViewer == null) return;
        var max = Math.Max(0, TabsScrollViewer.Extent.Width - TabsScrollViewer.Viewport.Width);
        var x = Math.Clamp(TabsScrollViewer.Offset.X + delta, 0, max);
        TabsScrollViewer.Offset = new Vector(x, TabsScrollViewer.Offset.Y);
    }

    private void RevealActiveTab(MacToolTab tab)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (!OpenTabs.Contains(tab) || !TabbedWorkspace.IsVisible) return;
            var viewport = TabsScrollViewer.Viewport.Width;
            if (viewport <= 0) return;
            var index = OpenTabs.IndexOf(tab);
            const double tabPitch = 181; // 174px 标签 + 6px Spacing + 1px Margin
            var left = index * tabPitch;
            var right = left + 174;
            var current = TabsScrollViewer.Offset.X;
            var target = left < current ? left :
                right > current + viewport ? right - viewport : current;
            var max = Math.Max(0, TabsScrollViewer.Extent.Width - viewport);
            TabsScrollViewer.Offset = new Vector(Math.Clamp(target, 0, max), TabsScrollViewer.Offset.Y);
        });
    }

    private void CloseTab_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: MacToolTab tab }) CloseToolTab(tab);
        e.Handled = true;
    }

    private void CloseToolTab(MacToolTab tab)
    {
        var index = OpenTabs.IndexOf(tab);
        if (index < 0) return;
        var wasActive = ReferenceEquals(_activeTab, tab);
        if (wasActive)
        {
            ClearStatusSubscriptions();
            ToolTabContent.Content = null;
            _activeTab = null;
        }
        OpenTabs.RemoveAt(index);

        try
        {
            var vm = tab.SourceWindow is Views.MacLegacyToolWindow legacy
                ? legacy.HostedViewModel
                : tab.SourceWindow.DataContext;

            // 对会话类工具主动断开，避免隐藏窗口后端口被占用。
            switch (vm)
            {
                case SshViewModel ssh when ssh.IsConnected:
                    ssh.ToggleConnectCommand.Execute(null);
                    break;
                case NetworkViewModel network when network.IsConnected:
                    network.ToggleConnectCommand.Execute(null);
                    break;
                case ApiDebugViewModel api when api.IsConnected:
                    api.ToggleConnectCommand.Execute(null);
                    break;
                case MessageDebugViewModel message when message.IsConnected:
                    message.ToggleConnectCommand.Execute(null);
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }

            if (tab.SourceWindow is Views.MacSpeechWindow speech)
                speech.StopForTab();
            tab.SourceWindow.Close();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"关闭 {tab.Title} 时释放资源失败：{ex}");
        }

        if (wasActive && OpenTabs.Count > 0)
            ActivateToolTab(OpenTabs[Math.Min(index, OpenTabs.Count - 1)]);
        else if (OpenTabs.Count == 0)
            ShowHome();

        UpdateSidebarOpenTabs();
        if (wasActive && OpenTabs.Count == 0)
            UpdateToolStatus();
    }

#if DEBUG
    /// <summary>Windows 数据工具迁移验收：离线 XML/模型编辑/SQL 工具窗口，不访问任何数据库。</summary>
    public async Task<int> SmokeTestDataToolsAsync(string screenshots)
    {
        Directory.CreateDirectory(screenshots);
        Views.MacModelManagerWindow? editor = null;
        Views.MacSqlWorkbenchWindow? sql = null;
        try
        {
            Width = 1390;
            Height = 940;
            WindowState = WindowState.Normal;

            // 没有用户要求的独立“数据工具”侧边入口；模型/同步/Redis 回归原来的分类。
            if (CategoryCount != 4 ||
                this.GetVisualDescendants().OfType<Button>()
                    .Any(b => b.Name == "DataToolsNavButton" || b.Name == "HomeDataToolsButton") ||
                MacToolCatalog.Tools.First(t => t.Title == "数据建模").Category != "开发与数据" ||
                MacToolCatalog.Tools.First(t => t.Title == "数据同步").Category != "开发与数据" ||
                MacToolCatalog.Tools.First(t => t.Title == "Redis 管理").Category != "设备与运维")
                throw new InvalidOperationException("首页出现未经要求的数据工具入口或分类未恢复");
            Console.WriteLine("DATA TOOLS NAV OK: no redundant sidebar/hero entry; 4 original categories retained");
            var sample = Path.Combine(Services.MacUserPaths.InitialWorkingDirectory,
                "XCoder-CLI", "Model.xml");
            var original = Services.MacDataToolsService.ImportFile(sample);
            if (original.Count < 5) throw new InvalidOperationException("XCode 示例 XML 导入后表数异常");
            var xml = Services.MacDataToolsService.ExportXml(original);
            var roundtrip = Services.MacDataToolsService.ImportXml(xml);
            if (roundtrip.Count != original.Count ||
                roundtrip[0].Columns.Count != original[0].Columns.Count ||
                roundtrip[0].Indexes.Count != original[0].Indexes.Count)
                throw new InvalidOperationException("XCode XML 导出/导入往返后丢失表/字段/索引");
            Services.MacDataToolsService.ValidateModel(roundtrip);
            Console.WriteLine($"DATA TOOLS XML OK: {roundtrip.Count} tables; columns and indexes preserved");

            // 仅向随机临时目录中的本地 SQLite 数据库操作，绝不触及现有连接。
            var localDbDir = Path.Combine(Path.GetTempPath(),
                "XCoder-DataTools-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(localDbDir);
            var connName = "LocalSqliteSmoke_" + Guid.NewGuid().ToString("N");
            try
            {
                var sqliteFile = Path.Combine(localDbDir, "test.sqlite");
                XCode.DataAccessLayer.DAL.AddConnStr(connName,
                    $"Data Source={sqliteFile}", null, "SQLite");
                var md = XCode.DataAccessLayer.DAL.Create(connName).Db.CreateMetaData();
                var ddl = Services.MacDataToolsService.CreateTableSql(md, roundtrip[0]);
                if (!ddl.Contains("CREATE TABLE", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"SQL 生成没有创建表语句：{ddl[..Math.Min(200, ddl.Length)]}");
                var complete = Services.MacDataToolsService.CreateAllSql(md, roundtrip.Take(2));
                if (complete.Length <= ddl.Length)
                    throw new InvalidOperationException("多表建表 SQL 生成不完整");
                Console.WriteLine($"DATA TOOLS DDL OK: SQLite offline {roundtrip[0].Name}; two tables generated; no DDL executed");

                // 与 Windows / Mac 原 DataModelingViewModel 使用同一代码生成器，
                // 证明“导入 XML -> 离线生成实体”不依赖实时数据库连接。
                var generatedDir = Path.Combine(localDbDir, "generated");
                var option = new XCode.Code.EntityBuilderOption
                {
                    BaseClass = "Entity",
                    ConnName = "OfflineXcoderSample",
                    Namespace = "XCoder.MacSmoke",
                    Output = generatedDir
                };
                _ = XCode.Code.EntityBuilder.BuildTables([roundtrip[0]], option);
                if (!Directory.Exists(generatedDir) ||
                    !Directory.EnumerateFiles(generatedDir, "*.cs", SearchOption.AllDirectories).Any())
                    throw new InvalidOperationException("离线 XML 模型未生成任何实体 C# 文件");
                Console.WriteLine("DATA TOOLS CODEGEN OK: imported XML model -> generated C# entities in temp directory");
            }
            finally
            {
                XCode.DataAccessLayer.DAL.ConnStrs.TryRemove(connName, out _);
                try { Directory.Delete(localDbDir, recursive: true); }
                catch (IOException) { /* DAL may keep an SQLite file handle until process exit. */ }
            }

            OpenTool(MacToolCatalog.Tools.First(x => x.Title == "数据建模"));
            await Task.Delay(130);
            UpdateLayout();
            var controls = _activeTab!.Content.GetVisualDescendants().OfType<Button>().ToArray();
            foreach (var id in new[]
            {
                "ImportModelButton", "ExportModelButton", "ManageModelButton",
                "PreviewDdlButton", "DatabaseWorkbenchButton", "ApplyDatabaseSchemaButton"
            })
                if (!controls.Any(button => button.Name == id))
                    throw new InvalidOperationException($"Windows 数据工具对应操作入口缺失：{id}");
            CaptureModelingScreenshot(screenshots, "01-数据建模-全部数据工具.png");

            editor = new Views.MacModelManagerWindow(roundtrip.Take(1));
            editor.Show(this);
            await Task.Delay(140);
            editor.UpdateLayout();
            editor.SmokeTestOfflineEditor();
            await Task.Delay(240);
            editor.UpdateLayout();
            editor.AssertVisibleLists();
            if (editor.Tables.Count < 2) throw new InvalidOperationException("模型管理没有新增第二张表");
            using (var bmp = new RenderTargetBitmap(
                new PixelSize((int)Math.Ceiling(editor.Bounds.Width * 2),
                              (int)Math.Ceiling(editor.Bounds.Height * 2)),
                new Vector(192, 192)))
            {
                bmp.Render(editor);
                bmp.Save(Path.Combine(screenshots, "01-模型管理-表字段索引.png"));
            }
            editor.Close();
            editor = null;

            sql = new Views.MacSqlWorkbenchWindow("仅界面测试-未连接");
            sql.Show(this);
            await Task.Delay(150);
            sql.UpdateLayout();
            sql.SmokeTestOffline();
            using (var bmp = new RenderTargetBitmap(
                new PixelSize((int)Math.Ceiling(sql.Bounds.Width * 2),
                              (int)Math.Ceiling(sql.Bounds.Height * 2)),
                new Vector(192, 192)))
            {
                bmp.Render(sql);
                bmp.Save(Path.Combine(screenshots, "01-架构管理与SQL查询器.png"));
            }
            sql.Close();
            sql = null;
            if (MacToolCatalog.Categories.Length != 4 ||
                MacToolCatalog.Tools.Length != 26)
                throw new InvalidOperationException("工具数或原有分类发生变化");

            Console.WriteLine("DATA TOOLS UI OK: 6 model actions; model editor CRUD; SQL/schema windows; category 3 tools");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("DATA TOOLS OFFLINE FAILED: " + ex);
            return 1;
        }
        finally
        {
            editor?.Close();
            sql?.Close();
            foreach (var tab in OpenTabs.ToArray()) CloseToolTab(tab);
        }
    }

#endif

    /// <summary>数据同步在主窗口标签页中的布局与命令绑定检查，不连接实际数据库。</summary>
    public async System.Threading.Tasks.Task<int> SmokeTestDataSyncLayoutAsync(string screenshotDirectory)
    {
        Directory.CreateDirectory(screenshotDirectory);
        var errors = 0;
        var tool = MacToolCatalog.Tools.First(x => x.Title == "数据同步");

        foreach (var (width, height, sideBySide) in new[]
        {
            (1540, 980, true),
            (1320, 850, true),
            (1000, 690, false)
        })
        {
            try
            {
                Width = width;
                Height = height;
                WindowState = WindowState.Normal;
                OpenTool(tool);
                await System.Threading.Tasks.Task.Delay(170);
                UpdateLayout();

                var root = _activeTab?.Content as ScrollViewer
                    ?? throw new InvalidOperationException("数据同步未在主窗口标签中打开");
                var elements = root.GetVisualDescendants().OfType<Control>().ToArray();
                T? Named<T>(string name) where T : Control =>
                    elements.OfType<T>().FirstOrDefault(e => e.Name == name);

                var groups = Named<Grid>("ConnectionGroups")
                    ?? throw new InvalidOperationException("缺少数据库连接分组");
                var source = Named<Border>("SourcePanel")
                    ?? throw new InvalidOperationException("缺少源数据库卡片");
                var target = Named<Border>("TargetPanel")
                    ?? throw new InvalidOperationException("缺少目标数据库卡片");
                var sourceCombo = Named<ComboBox>("SourceDatabaseCombo")
                    ?? throw new InvalidOperationException("缺少源数据库下拉框");
                var targetCombo = Named<ComboBox>("TargetDatabaseCombo")
                    ?? throw new InvalidOperationException("缺少目标数据库下拉框");

                var actualSideBySide = Grid.GetColumn(target) == 2 && Grid.GetRow(target) == 0;
                if (actualSideBySide != sideBySide ||
                    sourceCombo.Bounds.Width < 240 || targetCombo.Bounds.Width < 240 ||
                    Math.Abs(sourceCombo.Bounds.Width - targetCombo.Bounds.Width) > 5)
                    throw new InvalidOperationException(
                        $"数据库分组或下拉框布局错误: side={actualSideBySide}, source={sourceCombo.Bounds.Width:F0}, target={targetCombo.Bounds.Width:F0}");

                // 校验原来错误显示的布尔 True 已被明确操作文案替代。
                var contents = elements.OfType<Button>().Select(x => x.Content?.ToString()).ToArray();
                foreach (var expected in new[] { "连接源数据库", "断开源数据库", "连接目标数据库", "断开目标数据库" })
                    if (!contents.Contains(expected))
                        throw new InvalidOperationException($"缺少数据库操作按钮：{expected}");
                if (contents.Contains("True") || contents.Contains("False"))
                    throw new InvalidOperationException("布尔状态被当作按钮文字显示");

                if (root.DataContext is not DataSyncViewModel vm)
                    throw new InvalidOperationException("没有复用上游 DataSyncViewModel");
                if (!ReferenceEquals(sourceCombo.ItemsSource, vm.Connections) ||
                    !ReferenceEquals(targetCombo.ItemsSource, vm.TargetConnections))
                    throw new InvalidOperationException("连接下拉框的原有数据绑定丢失");

                // 不访问真实数据库，直接改变连接成功/断开状态，验证四个按钮是否即时切换。
                var sourceConnect = Named<Button>("ConnectSourceButton")!;
                var sourceDisconnect = Named<Button>("DisconnectSourceButton")!;
                var targetConnect = Named<Button>("ConnectTargetButton")!;
                var targetDisconnect = Named<Button>("DisconnectTargetButton")!;
                if (sourceConnect == null || sourceDisconnect == null ||
                    targetConnect == null || targetDisconnect == null)
                    throw new InvalidOperationException("缺少连接/断开按钮的状态绑定");

                void AssertActions(bool source, bool target,
                    bool connectSource, bool disconnectSource,
                    bool connectTarget, bool disconnectTarget)
                {
                    vm.IsSourceConnected = source;
                    vm.IsTargetConnected = target;
                    UpdateLayout();
                    if (sourceConnect.IsEnabled != connectSource ||
                        sourceDisconnect.IsEnabled != disconnectSource ||
                        targetConnect.IsEnabled != connectTarget ||
                        targetDisconnect.IsEnabled != disconnectTarget)
                        throw new InvalidOperationException(
                            $"连接按钮状态错误：源={source} 目标={target}; " +
                            $"实际={sourceConnect.IsEnabled}/{sourceDisconnect.IsEnabled}/" +
                            $"{targetConnect.IsEnabled}/{targetDisconnect.IsEnabled}");
                }

                AssertActions(false, false, true, false, false, false); // 初始状态
                AssertActions(true, false, false, true, true, false);  // 源库成功
                AssertActions(true, true, false, true, false, true);   // 目标库成功
                AssertActions(true, false, false, true, true, false); // 目标断开
                AssertActions(false, false, true, false, false, false); // 源库断开
                Console.WriteLine("SYNC BUTTONS OK: initial, source connected, target connected, target disconnected, source disconnected");

#if DEBUG
                if (width == 1540)
                {
                    if (_activeTab?.SourceWindow is not Views.MacDataSyncWindow syncWindow)
                        throw new InvalidOperationException("无法获取 Mac 数据同步窗口执行首项选中测试");
                    await syncWindow.SmokeTestConnectionDefaultsAsync();
                }
#endif

                Console.WriteLine($"SYNC LAYOUT OK: {width}x{height}, sideBySide={actualSideBySide}, " +
                    $"sourceCombo={sourceCombo.Bounds.Width:F0}, targetCombo={targetCombo.Bounds.Width:F0}, " +
                    $"rootViewport={root.Viewport.Width:F0}, groups={groups.Bounds.Width:F0}");

                if (width is 1540 or 1000)
                {
                    var filename = width == 1540 ? "02-数据同步.png" : "02-数据同步-小窗口.png";
                    using var bitmap = new RenderTargetBitmap(
                        new PixelSize((int)Math.Ceiling(Bounds.Width * 2),
                                      (int)Math.Ceiling(Bounds.Height * 2)),
                        new Vector(192, 192));
                    bitmap.Render(this);
                    var screenshot = Path.Combine(screenshotDirectory, filename);
                    bitmap.Save(screenshot);
                    Console.WriteLine($"SYNC SCREENSHOT: {screenshot}");
                }
            }
            catch (Exception ex)
            {
                errors++;
                Console.Error.WriteLine($"SYNC LAYOUT FAILED ({width}): {ex}");
            }
        }

        foreach (var tab in OpenTabs.ToArray()) CloseToolTab(tab);
        Console.WriteLine($"SYNC LAYOUT SUMMARY: 3 sizes, ERRORS={errors}");
        return errors;
    }

    /// <summary>API 交互日志必须是黑色控制台，悬停/聚焦也不允许继承浅色输入框背景。</summary>
    public async System.Threading.Tasks.Task<int> SmokeTestApiConsoleAsync(string screenshotDirectory)
    {
        Directory.CreateDirectory(screenshotDirectory);
        try
        {
            Width = 1320;
            Height = 850;
            WindowState = WindowState.Normal;
            OpenTool(MacToolCatalog.Tools.First(x => x.Title == "API 调试"));
            await System.Threading.Tasks.Task.Delay(180);
            UpdateLayout();

            if (_activeTab?.SourceWindow.DataContext is not ApiDebugViewModel vm)
                throw new InvalidOperationException("API 调试没有绑定原 ViewModel");
            var boxes = _activeTab.Content.GetVisualDescendants().OfType<TextBox>().ToArray();
            var console = boxes.SingleOrDefault(box => box.Name == "RpcInteractionLogBox")
                ?? throw new InvalidOperationException("交互日志控件没有找到");
            var ordinary = boxes.FirstOrDefault(box => box.Text == vm.Address)
                ?? throw new InvalidOperationException("普通地址输入框没有找到");

            // 顶部不再重复显示“打开”；连接设置中的真实操作按钮仍必须存在。
            var rpcContent = _activeTab.Content.GetVisualDescendants().OfType<Control>().ToArray();
            var operationButtons = rpcContent.OfType<Button>()
                .Where(button => button.Command == vm.ToggleConnectCommand).ToArray();
            var duplicateLabels = rpcContent.OfType<TextBlock>()
                .Where(text => text.Text == vm.ConnectButtonText &&
                               !text.GetVisualAncestors().OfType<Button>().Any()).ToArray();
            if (operationButtons.Length != 1 || duplicateLabels.Length != 0)
                throw new InvalidOperationException(
                    $"RPC 顶部状态标签未删除或真正连接按钮丢失：按钮={operationButtons.Length}, 重复标签={duplicateLabels.Length}");

            var actionSelect = rpcContent.OfType<ComboBox>()
                .FirstOrDefault(box => box.Name == "RpcActionSelect")
                ?? throw new InvalidOperationException("API Action 下拉框丢失");
            var actionInput = boxes.FirstOrDefault(box => box.Name == "RpcActionManualInput")
                ?? throw new InvalidOperationException("手动 Action 输入框丢失");
            var sendButton = rpcContent.OfType<Button>()
                .FirstOrDefault(button => button.Name == "RpcSendRequestButton")
                ?? throw new InvalidOperationException("发送按钮丢失");
            var parameters = boxes.FirstOrDefault(box => box.Name == "RpcRequestParameterInput")
                ?? throw new InvalidOperationException("请求参数输入框丢失");

            if (Grid.GetColumn(actionSelect) != 0 || Grid.GetColumn(actionInput) != 2 ||
                Grid.GetColumn(sendButton) != 4 || sendButton.Command != vm.SendCommand ||
                !ReferenceEquals(actionSelect.ItemsSource, vm.ApiActions) ||
                !parameters.AcceptsReturn)
                throw new InvalidOperationException("API Action、手动输入和发送命令布局/绑定异常");

            if (Math.Abs(actionSelect.Bounds.Y - actionInput.Bounds.Y) > 3 ||
                Math.Abs(actionInput.Bounds.Y - sendButton.Bounds.Y) > 3 ||
                !(actionSelect.Bounds.Width >= 90 &&
                  actionInput.Bounds.Width >= 105 && sendButton.Bounds.Width >= 85))
                throw new InvalidOperationException(
                    $"API Action/手动输入/发送未保持同一行: " +
                    $"combo={actionSelect.Bounds}, manual={actionInput.Bounds}, send={sendButton.Bounds}");

            // Bounds.Y 是相对于各自父 Grid 的局部坐标，不可直接比较不同 Grid 的 Y。
            // 检查父 Grid 在外层的行顺序，并验证多行编辑区真实可见高度。
            if (parameters.Bounds.Height < 144 ||
                actionSelect.Parent is not Grid actionRow ||
                parameters.Parent is not Grid parameterArea ||
                Grid.GetRow(actionRow) != 2 ||
                Grid.GetRow(parameterArea) != 3)
                throw new InvalidOperationException(
                    $"请求参数输入框高度不足或布局顺序错误：{parameters.Bounds}");

            var originalAction = vm.SelectedAction;
            var originalParams = vm.SendText;
            try
            {
                vm.SelectedAction = "Demo.Action";
                vm.SendText = "{\"id\":123,\"name\":\"测试\"}";
                await System.Threading.Tasks.Task.Delay(50);
                UpdateLayout();
                if (actionInput.Text != vm.SelectedAction || parameters.Text != vm.SendText)
                    throw new InvalidOperationException("手动 Action 或请求参数的数据绑定没有回填");
            }
            finally
            {
                vm.SelectedAction = originalAction;
                vm.SendText = originalParams;
            }
            Console.WriteLine(
                $"RPC REQUEST FORM OK: combo={actionSelect.Bounds.Width:F0}, " +
                $"manual={actionInput.Bounds.Width:F0}, send={sendButton.Bounds.Width:F0}, " +
                $"parameters={parameters.Bounds.Height:F0} high, command + bindings correct");

            static bool IsBlack(Avalonia.Media.IBrush? brush) =>
                brush is Avalonia.Media.ISolidColorBrush solid &&
                solid.Color.R < 32 && solid.Color.G < 32 && solid.Color.B < 45;
            static bool IsLight(Avalonia.Media.IBrush? brush) =>
                brush is Avalonia.Media.ISolidColorBrush solid &&
                solid.Color.R > 195 && solid.Color.G > 195 && solid.Color.B > 195;

            void AssertConsole(string state)
            {
                UpdateLayout();
                var backgrounds = console.GetVisualDescendants()
                    .OfType<Border>().Where(border => border.Name == "PART_BorderElement").ToArray();
                if (!IsBlack(console.Background) || !IsLight(console.Foreground) ||
                    backgrounds.Any(border => !IsBlack(border.Background)) ||
                    !console.IsReadOnly)
                    throw new InvalidOperationException(
                        $"RPC Console {state} 颜色错误: background={console.Background}, " +
                        $"foreground={console.Foreground}, templateBorder=" +
                        $"{string.Join(",", backgrounds.Select(border => border.Background?.ToString()))}");
                Console.WriteLine($"RPC CONSOLE {state} OK: black background, light text, templateBorders={backgrounds.Length}");
            }

            AssertConsole("normal");
            var pseudoProp = typeof(Avalonia.StyledElement).GetProperty("PseudoClasses",
                BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("缺少 Avalonia 伪类接口");
            var pseudo = pseudoProp.GetValue(console)
                ?? throw new InvalidOperationException("无法获取伪类状态");
            var pseudoInterface = pseudo.GetType().GetInterfaces().First(i => i.Name == "IPseudoClasses");
            var add = pseudoInterface.GetMethod("Add")!;
            var remove = pseudoInterface.GetMethod("Remove")!;
            foreach (var state in new[] { ":pointerover", ":focus", ":focus-within" })
            {
                add.Invoke(pseudo, [state]);
                try { AssertConsole(state); }
                finally { remove.Invoke(pseudo, [state]); }
            }

            if (!IsLight(ordinary.Background))
                throw new InvalidOperationException("RPC Console 黑色样式污染了普通请求输入框");

            CaptureModelingScreenshot(screenshotDirectory, "03-API 调试.png");
            vm.ReceiveLog = "[Info] NewLife.Remoting RPC 控制台\n[Info] 连接与请求日志显示在此区域\n[Info] 黑色底色不会随焦点改变";
            await System.Threading.Tasks.Task.Delay(85);
            UpdateLayout();
            if (console.Text != vm.ReceiveLog) throw new InvalidOperationException("原有 ReceiveLog 绑定丢失");
            CaptureModelingScreenshot(screenshotDirectory, "03-API调试-黑色交互日志.png");
            Console.WriteLine("RPC CONSOLE SUMMARY: normal, hover, focus, focus-within, receive log binding all passed");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("RPC CONSOLE FAILED: " + ex);
            return 1;
        }
        finally
        {
            foreach (var tab in OpenTabs.ToArray()) CloseToolTab(tab);
        }
    }

    /// <summary>以渲染完成后的真实工作台检查侧栏、标签与统一状态栏，并保存截图。</summary>
    public async System.Threading.Tasks.Task<int> SmokeTestWorkspaceLayoutAsync(string screenshotDirectory)
    {
        Directory.CreateDirectory(screenshotDirectory);
        try
        {
            Width = 1320;
            Height = 850;
            WindowState = WindowState.Normal;

            OpenTool(MacToolCatalog.Tools.First(x => x.Title == "数据建模"));
            await System.Threading.Tasks.Task.Delay(140);
            OpenTool(MacToolCatalog.Tools.First(x => x.Title == "数据同步"));
            var sync = (DataSyncViewModel)_activeTab!.SourceWindow.DataContext!;
            sync.Status = "数据同步已就绪";
            await System.Threading.Tasks.Task.Delay(240);
            UpdateLayout();

            if (OpenTabs.Count != 2 || !TabbedWorkspace.IsVisible ||
                !SidebarOpenTabsSection.IsVisible ||
                SidebarOpenTabs.ItemsSource != OpenTabs ||
                WorkspaceStatusText.Text != sync.Status ||
                !WorkspaceStatusTitle.Text!.Contains("数据同步"))
                throw new InvalidOperationException("工作台多窗口与底部状态栏状态不一致");

            var sidebarEntries = SidebarOpenTabs.GetVisualDescendants().OfType<Button>()
                .Count(b => b.Tag is MacToolTab);
            var headerEntries = TabsScrollViewer.GetVisualDescendants().OfType<Button>()
                .Count(b => b.Tag is MacToolTab);
            var contentControls = ToolTabContent.GetVisualDescendants().OfType<Control>().Count();
            if (sidebarEntries < 4 || headerEntries < 2 || contentControls < 8)
                throw new InvalidOperationException(
                    $"工作台渲染尚未完成：侧栏按钮{sidebarEntries}，顶部标签{headerEntries}，内容控件{contentControls}");

            if (SidebarOpenTabsTitle.Text != "已打开工具 (2)" ||
                this.GetVisualDescendants().OfType<Button>()
                    .Any(b => b.Name == "WorkspaceReturnButton"))
                throw new InvalidOperationException("左侧已打开工具标题错误，或顶部冗余按钮仍存在");

            var hoveredSidebar = SidebarOpenTabs.GetVisualDescendants().OfType<Button>()
                .FirstOrDefault(b => b.Classes.Contains("sidebar-tool-tab"))
                ?? throw new InvalidOperationException("左侧打开工具项无法点击");
            var pseudoProperty = typeof(Avalonia.StyledElement).GetProperty("PseudoClasses",
                BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("无法模拟鼠标悬停状态");
            var pseudo = pseudoProperty.GetValue(hoveredSidebar)!;
            var iface = pseudo.GetType().GetInterfaces().First(i => i.Name == "IPseudoClasses");
            var add = iface.GetMethod("Add")!;
            var remove = iface.GetMethod("Remove")!;
            add.Invoke(pseudo, [":pointerover"]);
            try
            {
                UpdateLayout();
                var fill = hoveredSidebar.Background as Avalonia.Media.ISolidColorBrush;
                if (fill == null || fill.Color.A != 0)
                    throw new InvalidOperationException(
                        $"鼠标悬停时左侧内层按钮出现多余的色条：{hoveredSidebar.Background}");

                var presenterFills = hoveredSidebar.GetVisualDescendants()
                    .OfType<Avalonia.Controls.Presenters.ContentPresenter>()
                    .Select(x => x.Background).OfType<Avalonia.Media.ISolidColorBrush>().ToArray();
                if (presenterFills.Any(x => x.Color.A > 0))
                    throw new InvalidOperationException("鼠标悬停时左侧按钮内部模板仍有独立色块");

                CaptureModelingScreenshot(screenshotDirectory, "00-工作台-已打开工具悬停.png");
                Console.WriteLine("WORKSPACE HOVER OK: no inner color strip or duplicate tooltip; sidebar title=已打开工具");
            }
            finally { remove.Invoke(pseudo, [":pointerover"]); }

            CaptureModelingScreenshot(screenshotDirectory, "00-工作台-已打开工具与统一状态栏.png");
            Console.WriteLine($"WORKSPACE LAYOUT OK: tabs={OpenTabs.Count}, sidebarButtons={sidebarEntries}, headerButtons={headerEntries}, contentControls={contentControls}, status={WorkspaceStatusText.Text}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("WORKSPACE LAYOUT FAILED: " + ex);
            return 1;
        }
        finally
        {
            foreach (var tab in OpenTabs.ToArray()) CloseToolTab(tab);
        }
    }

    /// <summary>
    /// 实际按 Button.OnClick 路径验证上游加密命令在非法输入时不会杀死整个进程。
    /// 仅使用测试字符串，不读写真实密码、文件或网络。
    /// </summary>
    public async System.Threading.Tasks.Task<int> SmokeTestSecurityButtonsAsync(string screenshotDirectory)
    {
        Directory.CreateDirectory(screenshotDirectory);
        try
        {
            Width = 1320;
            Height = 850;
            WindowState = WindowState.Normal;
            OpenTool(MacToolCatalog.Tools.First(x => x.Title == "加密解密"));
            await System.Threading.Tasks.Task.Delay(180);
            UpdateLayout();

            if (_activeTab?.SourceWindow is not Views.MacLegacyToolWindow legacy ||
                legacy.HostedViewModel is not SecurityViewModel vm)
                throw new InvalidOperationException("加密解密没有绑定原 SecurityViewModel");

            var buttons = _activeTab.Content.GetVisualDescendants().OfType<Button>()
                .Where(b => b.Command != null).ToArray();
            var unprotected = buttons.Where(b =>
                b.Command is not Services.MacSecurityCommandGuard.SafeSecurityCommand).ToArray();
            if (buttons.Length < 31 || unprotected.Length != 0)
                throw new InvalidOperationException(
                    $"命令保护不完整：按钮={buttons.Length}，未保护={string.Join(',', unprotected.Select(b => b.Content))}");

            Button Find(string content) => buttons.FirstOrDefault(b => b.Content?.ToString() == content)
                ?? throw new InvalidOperationException($"找不到 {content} 按钮");

            var onClick = typeof(Button).GetMethod("OnClick",
                BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("无法找到 Button.OnClick");
            void Click(string name)
            {
                try { onClick.Invoke(Find(name), null); }
                catch (TargetInvocationException ex)
                {
                    throw new InvalidOperationException($"实际点击 {name} 触发未处理异常", ex.InnerException);
                }
            }

            vm.SourceText = "Hello World";
            Click("HEX编码");
            if (!vm.ResultText.Contains("48656C6C6F"))
                throw new InvalidOperationException("有效的 HEX 编码发生回归");
            Click("Base64编码");
            if (!vm.ResultText.Contains("SGVsbG8gV29ybGQ="))
                throw new InvalidOperationException("有效的 Base64 编码发生回归");

            vm.SourceText = "学无先后达者为师";
            Click("HEX解码");
            if (!vm.ResultText.Contains("HEX解码失败") ||
                !vm.ResultText.Contains("输入格式不正确"))
                throw new InvalidOperationException("非法 HEX 输入没有被转换成友好的错误提示");
            Console.WriteLine("SECURITY BUTTON OK: invalid HEX decoder guarded");

            Click("Base64解码");
            if (!vm.ResultText.Contains("Base64解码失败") ||
                !vm.ResultText.Contains("输入格式不正确"))
                throw new InvalidOperationException("非法 Base64 输入没有被转换成友好的错误提示");
            Console.WriteLine("SECURITY BUTTON OK: invalid Base64 decoder guarded");

            vm.SourceText = "abc";
            vm.PassText = "not-an-rsa-key";
            Click("RSA解密");
            if (!vm.ResultText.Contains("RSA解密失败"))
                throw new InvalidOperationException("RSA 解密错误没有在页面显示");
            Console.WriteLine("SECURITY BUTTON OK: bad RSA key guarded");

            vm.SourceText = "Hello";
            Click("SHA256");
            if (string.IsNullOrWhiteSpace(vm.ResultText) || vm.ResultText.Contains("失败"))
                throw new InvalidOperationException("正常 SHA256 命令没有成功");
            vm.SourceText = "https://newlifex.com/?x=hello world";
            Click("Url编码");
            if (!vm.ResultText.Contains("%"))
                throw new InvalidOperationException("正常 UrlEncode 命令没有成功");

            vm.SourceText = "not valid HEX or Base64";
            Click("HEX解码");
            UpdateLayout();
            CaptureModelingScreenshot(screenshotDirectory, "06-加密解密-非法输入提示.png");

            Console.WriteLine($"SECURITY BUTTONS OK: {buttons.Length} protected commands; simulated real OnClick, valid HEX/Base64/SHA/URL, invalid HEX/Base64/RSA");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("SECURITY BUTTONS FAILED: " + ex);
            return 1;
        }
        finally
        {
            foreach (var tab in OpenTabs.ToArray()) CloseToolTab(tab);
        }
    }

    /// <summary>快捷入口管理与独立用户配置回归；严格使用临时文件，绝不更改实际偏好。</summary>
    public async System.Threading.Tasks.Task<int> SmokeTestQuickEntriesAsync(string screenshotDirectory)
    {
        Directory.CreateDirectory(screenshotDirectory);
        var testFolder = Path.Combine(Path.GetTempPath(), "XCoder-QuickEntries-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(testFolder);
        var testFile = Path.Combine(testFolder, "quick-entries.json");
        Views.MacQuickEntrySettingsWindow? dialog = null;
        try
        {
            var defaults = MacQuickEntryStore.Load(testFile);
            if (!defaults.SequenceEqual(MacQuickEntryStore.Defaults))
                throw new InvalidOperationException("初次运行应使用默认快捷入口");

            var preferred = new[] { "SSH 工具", "数据同步", "数据建模" };
            MacQuickEntryStore.Save(preferred, testFile);
            if (!MacQuickEntryStore.Load(testFile).SequenceEqual(preferred))
                throw new InvalidOperationException("快捷入口保存/重新打开后顺序改变");

            MacQuickEntryStore.Save(["SSH 工具", "SSH 工具", "不存在的工具", "API 调试"], testFile);
            if (!MacQuickEntryStore.Load(testFile).SequenceEqual(["SSH 工具", "API 调试"]))
                throw new InvalidOperationException("未知或重复入口过滤失败");

            MacQuickEntryStore.Save([], testFile);
            if (MacQuickEntryStore.Load(testFile).Length != 0)
                throw new InvalidOperationException("全部清空后错误恢复默认列表");

            File.WriteAllText(testFile, "{ invalid json");
            if (!MacQuickEntryStore.Load(testFile).SequenceEqual(MacQuickEntryStore.Defaults) ||
                File.ReadAllText(testFile) != "{ invalid json")
                throw new InvalidOperationException("配置损坏时覆盖了用户文件或错误恢复默认");

            MacQuickEntryStore.Save(preferred, testFile);
            if (OperatingSystem.IsMacOS() &&
                (File.GetUnixFileMode(testFile) &
                 (UnixFileMode.GroupRead | UnixFileMode.OtherRead)) != 0)
                throw new InvalidOperationException("快捷入口配置权限不符合用户私有原则");

            dialog = new Views.MacQuickEntrySettingsWindow(preferred);
            dialog.Show(this);
            await System.Threading.Tasks.Task.Delay(190);
            dialog.UpdateLayout();
#if DEBUG
            dialog.SmokeTestEditor();
#endif
            await System.Threading.Tasks.Task.Delay(220);
            dialog.UpdateLayout();
            var list = dialog.FindControl<ListBox>("SelectedEntriesList")
                ?? throw new InvalidOperationException("未找到快捷入口排序列表");
            var renderedItems = list.GetVisualDescendants().OfType<ListBoxItem>().Count();
            Console.WriteLine($"QUICK ENTRY RENDER: source={dialog.SelectedEntries.Count}, " +
                $"itemsSource={(ReferenceEquals(list.ItemsSource, dialog.SelectedEntries))}, " +
                $"rendered={renderedItems}, listHeight={list.Bounds.Height:F0}, " +
                $"dataContext={list.DataContext?.GetType().Name ?? "null"}");
            if (!ReferenceEquals(list.ItemsSource, dialog.SelectedEntries) || renderedItems < 6)
                throw new InvalidOperationException("快捷入口排序列表未正常渲染");

            if (dialog.Bounds.Width < 750 ||
                dialog.Options.Count != MacToolCatalog.Tools.Length ||
                dialog.SelectedEntries.Count != MacQuickEntryStore.Defaults.Length)
                throw new InvalidOperationException("快捷入口管理界面未渲染全部工具");

            using var screenshot = new RenderTargetBitmap(
                new PixelSize((int)Math.Ceiling(dialog.Bounds.Width * 2),
                              (int)Math.Ceiling(dialog.Bounds.Height * 2)),
                new Vector(192, 192));
            screenshot.Render(dialog);
            var output = Path.Combine(screenshotDirectory, "00-管理快捷入口.png");
            screenshot.Save(output);
            Console.WriteLine($"QUICK ENTRY SCREENSHOT: {output}");
            Console.WriteLine("QUICK ENTRIES OK: defaults, custom ordering, reload, dedup, empty, malformed config, privacy; editor controls");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("QUICK ENTRIES FAILED: " + ex);
            return 1;
        }
        finally
        {
            dialog?.Close();
            Directory.Delete(testFolder, recursive: true);
        }
    }

    /// <summary>开发测试：验证窗口改变宽度时的布局与导航选中项实际尺寸。</summary>
    public async System.Threading.Tasks.Task<int> SmokeTestHomeLayoutAsync(string screenshotDirectory)
    {
        Directory.CreateDirectory(screenshotDirectory);
        var errors = 0;
        foreach (var (width, expected) in new[] { (1320, 4), (1000, 3), (1540, 4) })
        {
            try
            {
                Width = width;
                Height = 980;
                WindowState = WindowState.Normal;
                ShowHome();
                await System.Threading.Tasks.Task.Delay(180);
                UpdateLayout();

                var grid = ToolCards.GetVisualDescendants().OfType<UniformGrid>().FirstOrDefault();
                var navWidth = AllToolsNavButton.Bounds.Width;
                var cardsWidth = ToolCards.Bounds.Width;
                var firstCard = ToolCards.GetVisualDescendants().OfType<Button>()
                    .FirstOrDefault(b => b.Classes.Contains("tool-card"));
                Console.WriteLine($"HOME LAYOUT: window={width}, cards={cardsWidth:F1}, cols={ToolColumns}, actualGrid={grid?.Columns}, nav={navWidth:F1}, firstCard={firstCard?.Bounds.Width:F1}");

                if (ToolColumns != expected || grid?.Columns != expected)
                    throw new InvalidOperationException($"工具卡片应为 {expected} 列");
                if (navWidth < 175 || navWidth > 205)
                    throw new InvalidOperationException($"导航按钮没有铺满左栏内部：{navWidth}");
                if (firstCard == null || firstCard.Bounds.Width < 175)
                    throw new InvalidOperationException("工具卡片未正常伸展");
                if (cardsWidth <= 0 || cardsWidth > width - 180)
                    throw new InvalidOperationException($"工具区宽度不正常：{cardsWidth}");

                if (width == 1540)
                {
                    // 保持与其他 26 张工具截图一致的 3080px 原始宽度。
                    var screenshot = Path.Combine(screenshotDirectory, "00-首页.png");
                    var pixels = new PixelSize((int)Math.Ceiling(Bounds.Width * 2),
                                               (int)Math.Ceiling(Bounds.Height * 2));
                    using var bitmap = new RenderTargetBitmap(pixels, new Vector(192, 192));
                    bitmap.Render(this);
                    bitmap.Save(screenshot);
                    Console.WriteLine($"HOME SCREENSHOT: {screenshot} ({pixels.Width}x{pixels.Height})");
                }
            }
            catch (Exception ex)
            {
                errors++;
                Console.Error.WriteLine($"HOME LAYOUT FAILED ({width}): {ex}");
            }
        }
        Console.WriteLine($"HOME LAYOUT SUMMARY: 3 sizes, ERRORS={errors}");
        return errors;
    }

    private void CaptureModelingScreenshot(string screenshotDirectory, string filename)
    {
        using var bitmap = new RenderTargetBitmap(
            new PixelSize((int)Math.Ceiling(Bounds.Width * 2),
                          (int)Math.Ceiling(Bounds.Height * 2)),
            new Vector(192, 192));
        bitmap.Render(this);
        var path = Path.Combine(screenshotDirectory, filename);
        bitmap.Save(path);
        Console.WriteLine($"MODEL SCREENSHOT: {path}");
    }

    /// <summary>验证数据建模在主窗口标签页中的响应式布局，生成真实截图供审阅。</summary>
    public async System.Threading.Tasks.Task<int> SmokeTestModelingLayoutAsync(string screenshotDirectory)
    {
        Directory.CreateDirectory(screenshotDirectory);
        var errors = 0;
        var tool = MacToolCatalog.Tools.First(t => t.Title == "数据建模");
        foreach (var (windowWidth, windowHeight, expectedTop, expectedBottom) in new[]
        {
            (1540, 980, true, true),
            (1320, 850, true, true),
            (1000, 690, false, false)
        })
        {
            try
            {
                Width = windowWidth;
                Height = windowHeight;
                WindowState = WindowState.Normal;
                OpenTool(tool);
                await System.Threading.Tasks.Task.Delay(170);
                UpdateLayout();

                if (_activeTab?.Tool != tool || ToolTabContent.Content == null)
                    throw new InvalidOperationException("数据建模没有在主窗口中显示");

                var root = _activeTab.Content;
                var children = root.GetVisualDescendants().OfType<Control>().ToArray();
                T? Named<T>(string name) where T : Control =>
                    children.OfType<T>().FirstOrDefault(x => x.Name == name);

                var topGrid = Named<Grid>("ConfigurationGrid") ??
                    throw new InvalidOperationException("缺少上部配置区");
                var bottomGrid = Named<Grid>("StructureGrid") ??
                    throw new InvalidOperationException("缺少下部表结构区");
                var toolsPanel = Named<Border>("ModelToolsPanel") ??
                    throw new InvalidOperationException("缺少模型与数据库工具区");
                var toolActions = Named<WrapPanel>("ModelToolsActions") ??
                    throw new InvalidOperationException("缺少模型工具按钮容器");
                if (topGrid.Parent is not StackPanel contentStack ||
                    !ReferenceEquals(contentStack, toolsPanel.Parent) ||
                    !ReferenceEquals(contentStack, bottomGrid.Parent) ||
                    !(contentStack.Children.IndexOf(topGrid) <
                      contentStack.Children.IndexOf(toolsPanel) &&
                      contentStack.Children.IndexOf(toolsPanel) <
                      contentStack.Children.IndexOf(bottomGrid)))
                    throw new InvalidOperationException(
                        "模型与数据库工具应位于连接/代码生成设置下方、数据表结构上方");

                var requiredActions = new[]
                {
                    "ImportModelButton", "ExportModelButton", "ManageModelButton",
                    "PreviewDdlButton", "DatabaseWorkbenchButton", "ApplyDatabaseSchemaButton"
                };
                foreach (var name in requiredActions)
                {
                    var action = Named<Button>(name) ??
                        throw new InvalidOperationException($"缺少数据工具按钮 {name}");
                    var minWidth = name == "ManageModelButton" ? 78 : 90;
                    if (action.Parent != toolActions || action.Bounds.Height < 43 ||
                        action.Bounds.Width < minWidth || action.Content?.ToString()?.Length < 4)
                        throw new InvalidOperationException(
                            $"数据工具按钮文字可能被挤压或布局不正确：{name}，{action.Bounds}");
                }
                Console.WriteLine($"MODEL TOOLS OK: 6 readable buttons, minHeight>=44, placed below connection and above table structure");
                var connection = Named<Border>("ConnectionPanel")!;
                var generation = Named<Border>("GenerationPanel")!;
                var tables = Named<Border>("TablesPanel")!;
                var fields = Named<Border>("FieldsPanel")!;
                var scroll = root as ScrollViewer ??
                    throw new InvalidOperationException("内容最外层缺少可滚动容器");
                var outputInput = Named<TextBox>("CodeOutputDirectoryInput") ??
                    throw new InvalidOperationException("代码输出目录文本框未找到");
                var browseButton = Named<Button>("BrowseOutputDirectoryButton") ??
                    throw new InvalidOperationException("选择目录按钮未找到");
                if (browseButton.Content?.ToString()?.Contains("选择目录") != true ||
                    Grid.GetColumn(browseButton) != 2 ||
                    !ReferenceEquals(browseButton.Parent, outputInput.Parent) ||
                    !ReferenceEquals(TopLevel.GetTopLevel(browseButton), this) ||
                    browseButton.Bounds.Width < 100)
                    throw new InvalidOperationException("选择目录按钮未靠在文本框右侧或未挂载主窗口");

                // 改写 ViewModel 验证路径自动回填绑定；不触发真实代码生成。
                if (root.DataContext is not DataModelingViewModel modelingVm)
                    throw new InvalidOperationException("数据建模的 ViewModel 丢失");
                var originalPath = modelingVm.OutputPath;
                var testPath = "/tmp/Newlife.XCoder-output-binding-test";
                try
                {
                    modelingVm.OutputPath = testPath;
                    await System.Threading.Tasks.Task.Delay(35);
                    UpdateLayout();
                    if (outputInput.Text != testPath)
                        throw new InvalidOperationException("输出目录选择后未反映到绑定文本框");
                }
                finally
                {
                    modelingVm.OutputPath = originalPath;
                    UpdateLayout();
                }

                var actualTop = Grid.GetColumn(generation) > Grid.GetColumn(connection);
                var actualBottom = Grid.GetColumn(fields) > Grid.GetColumn(tables);
                if (actualTop != expectedTop || actualBottom != expectedBottom)
                    throw new InvalidOperationException(
                        $"期望上面并排={expectedTop}/下面并排={expectedBottom}，实际={actualTop}/{actualBottom}");
                if (scroll.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled ||
                    scroll.VerticalScrollBarVisibility != ScrollBarVisibility.Auto)
                    throw new InvalidOperationException("窗口滚动策略不正确");
                if (tables.Bounds.Height < 475 || fields.Bounds.Height < 475 ||
                    Math.Abs(tables.Bounds.Height - fields.Bounds.Height) > 1 ||
                    connection.Bounds.Width < 200 || generation.Bounds.Width < 200)
                    throw new InvalidOperationException("面板尺寸过小或未完成布局");
                if (windowHeight <= 690 && scroll.Extent.Height <= scroll.Viewport.Height)
                    throw new InvalidOperationException("小窗口内容未启用垂直滚动");

                Console.WriteLine($"MODEL LAYOUT OK: window={windowWidth}x{windowHeight}, " +
                    $"topSideBySide={actualTop}, bottomSideBySide={actualBottom}, " +
                    $"viewport={scroll.Viewport.Width:F0}x{scroll.Viewport.Height:F0}, " +
                    $"extent={scroll.Extent.Height:F0}, panels={connection.Bounds.Width:F0}/{generation.Bounds.Width:F0}");

                if (windowWidth is 1540 or 1000)
                {
                    CaptureModelingScreenshot(screenshotDirectory, windowWidth == 1540
                        ? "01-数据建模.png"
                        : "01-数据建模-小窗口.png");
                }

                if (windowWidth == 1000)
                {
                    // 模拟实际滚到底部，确保小窗口的字段结构和数据表仍然可用。
                    var maxScrollY = Math.Max(0, scroll.Extent.Height - scroll.Viewport.Height);
                    scroll.Offset = new Vector(0, maxScrollY);
                    await System.Threading.Tasks.Task.Delay(80);
                    UpdateLayout();
                    if (scroll.Offset.Y < maxScrollY - 2)
                        throw new InvalidOperationException("小窗口无法滚动到底部");
                    if (tables.Bounds.Height < 475 || fields.Bounds.Height < 475)
                        throw new InvalidOperationException("底部结构区域大小异常");
                    CaptureModelingScreenshot(screenshotDirectory,
                        "01-数据建模-小窗口-表结构.png");
                    Console.WriteLine($"MODEL SCROLL OK: reached bottom at {scroll.Offset.Y:F0}");
                }
            }
            catch (Exception ex)
            {
                errors++;
                Console.Error.WriteLine($"MODEL LAYOUT FAILED ({windowWidth}): {ex}");
            }
        }

        foreach (var tab in OpenTabs.ToArray()) CloseToolTab(tab);
        Console.WriteLine($"MODEL LAYOUT SUMMARY: 3 sizes, ERRORS={errors}");
        return errors;
    }

    // 仅测试工具窗口的初始化、标签复用、导航与关闭，不执行外部业务命令。
    public int SmokeTestTabs()
    {
        var failures = 0;
        var completed = 0;

        foreach (var tool in MacToolCatalog.Tools)
        {
            try
            {
                OpenTool(tool);
                if (OpenTabs.Count != 1 || _activeTab?.Tool != tool ||
                    ToolTabContent.Content == null)
                    throw new InvalidOperationException("标签未成功打开");
                OpenTool(tool);
                if (OpenTabs.Count != 1) throw new InvalidOperationException("重复创建标签");
                ShowHome();
                if (!HomeView.IsVisible || TabbedWorkspace.IsVisible || OpenTabs.Count != 1)
                    throw new InvalidOperationException("返回主页导致标签丢失");
                ActivateToolTab(OpenTabs[0]);
                CloseToolTab(OpenTabs[0]);
                if (OpenTabs.Count != 0) throw new InvalidOperationException("关闭标签失败");
                completed++;
                Console.WriteLine($"TAB OK: {tool.Title}");
            }
            catch (Exception ex)
            {
                failures++;
                Console.Error.WriteLine($"TAB FAILED: {tool.Title}: {ex}");
                foreach (var tab in OpenTabs.ToArray()) CloseToolTab(tab);
            }
        }

        try
        {
            var persisted = MacQuickEntryStore.Load();
            if (!QuickTools.Select(tool => tool.Title).SequenceEqual(persisted) ||
                !ManageQuickEntriesButton.IsVisible ||
                QuickEntryEmptyMessage.IsVisible != (QuickTools.Count == 0))
                throw new InvalidOperationException("首页快捷入口未按用户配置显示");
            AllTools_Click(null, new RoutedEventArgs());
            if (!QuickToolsSection.IsVisible || QuickToolsNavExists())
                throw new InvalidOperationException("快捷入口没有显示在首页或侧栏仍保留旧入口");
            foreach (var tool in MacToolCatalog.Tools.Take(10)) OpenTool(tool);
            if (OpenTabs.Count != 10) throw new InvalidOperationException("多标签数量异常");
            UpdateLayout();
            if (!SidebarOpenTabsSection.IsVisible ||
                SidebarOpenTabs.ItemsSource != OpenTabs ||
                !SidebarOpenTabsTitle.Text!.Contains("(10)"))
                throw new InvalidOperationException("工作台侧栏的已打开列表未同步");
            var firstTab = OpenTabs[0];
            var thirdTab = OpenTabs[2];
            SidebarTab_Click(new Button { Tag = firstTab }, new RoutedEventArgs());
            if (_activeTab != firstTab || !firstTab.IsActive || !TabbedWorkspace.IsVisible)
                throw new InvalidOperationException("侧栏切换未激活正确标签");
            SidebarCloseTab_Click(new Button { Tag = thirdTab }, new RoutedEventArgs());
            if (OpenTabs.Contains(thirdTab) || OpenTabs.Count != 9 || _activeTab != firstTab)
                throw new InvalidOperationException("侧栏关闭后台标签失败");
            if (!SidebarOpenTabsTitle.Text!.Contains("(9)"))
                throw new InvalidOperationException("侧栏关闭后数量未更新");
            ActivateToolTab(OpenTabs[0]);
            CloseToolTab(OpenTabs[1]);
            if (OpenTabs.Count != 8 || _activeTab != OpenTabs[0])
                throw new InvalidOperationException("关闭后台标签影响当前标签");
            CloseToolTab(OpenTabs[0]);
            if (OpenTabs.Count != 7 || _activeTab == null)
                throw new InvalidOperationException("关闭活动标签未激活邻近标签");

            UpdateLayout();
            if (TabsScrollViewer.HorizontalScrollBarVisibility.ToString() != "Hidden")
                throw new InvalidOperationException("横向滚动条可见");
            if (TabsScrollViewer.Extent.Width > TabsScrollViewer.Viewport.Width + 1)
            {
                TabsScrollViewer.Offset = new Vector(0, 0);
                ScrollTabsBy(240);
                if (TabsScrollViewer.Offset.X <= 0)
                    throw new InvalidOperationException("标签右箭头未生效");
                ScrollTabsBy(-240);
            }

            // 活动窗口状态实时同步，不需要实际连接数据库。
            foreach (var tab in OpenTabs.ToArray()) CloseToolTab(tab);
            var modelingTool = MacToolCatalog.Tools.First(x => x.Title == "数据建模");
            var syncTool = MacToolCatalog.Tools.First(x => x.Title == "数据同步");
            OpenTool(modelingTool);
            var modeling = (DataModelingViewModel)_activeTab!.SourceWindow.DataContext!;
            modeling.Status = "测试状态：数据建模已就绪";
            UpdateLayout();
            if (WorkspaceStatusText.Text != modeling.Status ||
                !WorkspaceStatusTitle.Text!.Contains("数据建模"))
                throw new InvalidOperationException("数据建模状态没有传到底部状态栏");
            OpenTool(syncTool);
            var sync = (DataSyncViewModel)_activeTab!.SourceWindow.DataContext!;
            sync.Status = "测试状态：数据同步已就绪";
            if (WorkspaceStatusText.Text != sync.Status ||
                !WorkspaceStatusTitle.Text!.Contains("数据同步"))
                throw new InvalidOperationException("切换到数据同步后状态栏未变更");
            modeling.Status = "后台窗口改变不应覆盖前台";
            if (WorkspaceStatusText.Text != sync.Status)
                throw new InvalidOperationException("后台工具覆盖前台状态");
            ActivateToolTab(OpenTabs.First(x => x.Tool.Title == "数据建模"));
            if (WorkspaceStatusText.Text != modeling.Status)
                throw new InvalidOperationException("从侧栏切回工具后状态没有恢复");
            CloseToolTab(_activeTab!);
            if (WorkspaceStatusText.Text != sync.Status)
                throw new InvalidOperationException("关闭活动窗口后状态栏没有切换到剩余窗口");
            CloseToolTab(_activeTab!);
            if (SidebarOpenTabsSection.IsVisible ||
                WorkspaceStatusText.Text != "就绪 · 请选择一个工具")
                throw new InvalidOperationException("关闭全部窗口后状态栏没有清空");

            // Redis 等工具把真正状态放在内部 Vm，而不是根 DataContext。
            OpenTool(MacToolCatalog.Tools.First(x => x.Title == "Redis 管理"));
            if (_activeTab?.SourceWindow is not Views.MacRedisWindow redis)
                throw new InvalidOperationException("无法取得 Redis 工具窗口");
            redis.Vm.Status = "Redis 模拟状态同步成功";
            if (WorkspaceStatusText.Text != redis.Vm.Status)
                throw new InvalidOperationException("内部 Vm.Status 没有显示在状态栏");
            CloseToolTab(_activeTab!);

            OpenTool(MacToolCatalog.Tools.First(x => x.Title == "USB 设备"));
            if (_activeTab?.SourceWindow is not Views.MacSystemInfoWindow usb)
                throw new InvalidOperationException("无法取得 USB 工具窗口");
            usb.StatusText = "USB 模拟读取完成";
            if (WorkspaceStatusText.Text != usb.StatusText)
                throw new InvalidOperationException("硬件信息窗口状态没有实时反映在状态栏");
            CloseToolTab(_activeTab!);
            Console.WriteLine("TAB STATUS SOURCES OK: direct Status, nested Redis Vm.Status, USB StatusText");

            Console.WriteLine("TAB SIDEBAR/STATUS OK: open, switch, close, clear, VM Status live update");
            Console.WriteLine("TAB MULTI/SCROLL OK");
        }
        catch (Exception ex)
        {
            failures++;
            Console.Error.WriteLine($"TAB MULTI FAILED: {ex}");
        }
        finally
        {
            foreach (var tab in OpenTabs.ToArray()) CloseToolTab(tab);
        }

        Console.WriteLine($"SMOKE TABS: {completed}/{MacToolCatalog.Tools.Length}, ERRORS={failures}");
        return failures;
    }

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Meta) && e.Key == Key.W &&
            TabbedWorkspace.IsVisible && _activeTab != null)
        {
            CloseToolTab(_activeTab);
            e.Handled = true;
            return;
        }

        if (e.KeyModifiers.HasFlag(KeyModifiers.Meta) &&
            (e.Key == Key.Return || e.Key == Key.Enter) &&
            TabbedWorkspace.IsVisible &&
            _activeTab?.SourceWindow.DataContext is RegexViewModel regex)
        {
            if (regex.ExecuteCommand.CanExecute(null)) regex.ExecuteCommand.Execute(null);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.F && e.KeyModifiers.HasFlag(KeyModifiers.Meta))
        {
            SearchBox.Focus();
            SearchBox.SelectAll();
            e.Handled = true;
        }
    }
}
