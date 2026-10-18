using System;
using System.Collections.ObjectModel;
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

namespace XCoderMac;

/// <summary>
/// macOS 专用工具中心：只负责分类、检索与启动现有工具。
/// 工具业务实现仍完全复用 XCoderAv。
/// </summary>
public partial class MacShellWindow : Window
{
    private static readonly string[] QuickToolTitles =
    [
        "数据建模", "正则表达式", "API 调试", "Redis 管理",
        "MQTT 客户端", "网络工具", "SSH 工具", "文件编码"
    ];

    private string? _selectedCategory;
    private bool _quickOnly;
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
    public ObservableCollection<MacToolTab> OpenTabs { get; } = new();
    private MacToolTab? _activeTab;
    public MacCategory[] Categories => MacToolCatalog.Categories;

    public MacShellWindow()
    {
        InitializeComponent();
        DataContext = this;
        _activeNavigationButton = AllToolsNavButton;
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
        UpdateWorkspaceButton();
    }

    private void ShowWorkspace()
    {
        if (OpenTabs.Count == 0) { ShowHome(); return; }
        HomeView.IsVisible = false;
        TabbedWorkspace.IsVisible = true;
        UpdateWorkspaceButton();
    }

    private void UpdateWorkspaceButton()
    {
        if (WorkspaceReturnButton == null) return;
        WorkspaceReturnButton.IsVisible = OpenTabs.Count > 0;
        WorkspaceReturnButton.Content = $"▤  已打开的标签 ({OpenTabs.Count})";
    }

    private void ShowWorkspace_Click(object? sender, RoutedEventArgs e) => ShowWorkspace();

    private void AllTools_Click(object? sender, RoutedEventArgs e)
    {
        _selectedCategory = null;
        _quickOnly = false;
        SelectNavigation(AllToolsNavButton);
        RefreshTools();
        ShowHome();
    }

    private void QuickTools_Click(object? sender, RoutedEventArgs e)
    {
        _selectedCategory = null;
        _quickOnly = true;
        SelectNavigation(QuickToolsNavButton);
        RefreshTools();
        ShowHome();
    }

    private void Category_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: MacCategory category }) return;
        _selectedCategory = category.Name;
        _quickOnly = false;
        SelectNavigation(sender as Button);
        RefreshTools();
        ShowHome();
    }

    private void SearchBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        RefreshTools();
        if (SearchBox != null && !string.IsNullOrWhiteSpace(SearchBox.Text))
            ShowHome();
    }

    private void RefreshTools()
    {
        // 初始化 XAML 时 TextChanged 可能先触发，保护尚未完成的控件。
        if (ToolCount == null || SectionTitle == null || SearchBox == null) return;

        var query = SearchBox.Text?.Trim();
        var tools = MacToolCatalog.Tools.AsEnumerable();

        if (_selectedCategory != null)
            tools = tools.Where(t => t.Category == _selectedCategory);
        else if (_quickOnly)
            tools = tools.Where(t => QuickToolTitles.Contains(t.Title));

        if (!string.IsNullOrWhiteSpace(query))
            tools = tools.Where(t =>
                t.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Category.Contains(query, StringComparison.OrdinalIgnoreCase));

        VisibleTools.Clear();
        foreach (var tool in tools) VisibleTools.Add(tool);

        SectionTitle.Text = !string.IsNullOrWhiteSpace(query) ? "工具搜索" :
            _selectedCategory ?? (_quickOnly ? "快捷工具" : "探索全部工具");
        SectionDescription.Text = !string.IsNullOrWhiteSpace(query) ?
            $"与“{query}”相关的工具" :
            _selectedCategory != null ? "此分类下的工具都可直接打开使用。" :
            _quickOnly ? "开发与调试时最常用的一组工具。" :
            "选择一个工具开始工作，已有功能直接复用原项目。";

        ToolCount.Text = $"{VisibleTools.Count} 个工具";
        EmptyMessage.IsVisible = VisibleTools.Count == 0;
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
        RevealActiveTab(tab);
        if (tab.SourceWindow is Views.MacSpeechWindow speech)
            _ = speech.EnsureTabVoicesAsync();
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
        if (wasActive) { ToolTabContent.Content = null; _activeTab = null; }
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

        UpdateWorkspaceButton();
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
            foreach (var tool in MacToolCatalog.Tools.Take(10)) OpenTool(tool);
            if (OpenTabs.Count != 10) throw new InvalidOperationException("多标签数量异常");
            ActivateToolTab(OpenTabs[0]);
            CloseToolTab(OpenTabs[1]);
            if (OpenTabs.Count != 9 || _activeTab != OpenTabs[0])
                throw new InvalidOperationException("关闭后台标签影响当前标签");
            CloseToolTab(OpenTabs[0]);
            if (OpenTabs.Count != 8 || _activeTab == null)
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
