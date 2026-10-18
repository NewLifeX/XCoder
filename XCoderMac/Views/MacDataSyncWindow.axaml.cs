using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Collections.Specialized;
using XCoderAv.ViewModels;

namespace XCoderMac.Views;

/// <summary>
/// Mac 原生数据同步界面。复用 XCoderAv.DataSyncViewModel 的连接/表选择/同步业务，
/// 修复原窗口把布尔值 IsSourceConnected 显示为 True 的错误绑定。
/// </summary>
public partial class MacDataSyncWindow : Window
{
    public DataSyncViewModel ViewModel { get; }

    public MacDataSyncWindow()
    {
        InitializeComponent();
        ViewModel = new DataSyncViewModel();
        DataContext = ViewModel;

        // 源连接在打开时已加载；目标连接通常要等源数据库连接成功后才会出现。
        // 两组列表都支持首次选中第一项，同时保留用户已选的有效连接。
        ViewModel.Connections.CollectionChanged += ConnectionListChanged;
        ViewModel.TargetConnections.CollectionChanged += ConnectionListChanged;
        SelectFirstAvailableConnection();

        // 两组按钮状态统一以“实际连接成功”的 VM 状态为准。
        // 原本目标库按钮仅绑定 IsSourceConnected，目标已连上后仍可重复点击。
        ViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(DataSyncViewModel.IsSourceConnected)
                or nameof(DataSyncViewModel.IsTargetConnected))
                UpdateConnectionActions();
        };
        UpdateConnectionActions();
    }

    private bool _selectionUpdateQueued;

    private void ConnectionListChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // 目标连接通过 Clear/Add 逐个填入；等本轮操作结束再选首项，
        // 避免列表刷新过程中多次改变选中项。
        if (_selectionUpdateQueued) return;
        _selectionUpdateQueued = true;
        Dispatcher.UIThread.Post(() =>
        {
            _selectionUpdateQueued = false;
            SelectFirstAvailableConnection();
        }, DispatcherPriority.Background);
    }

    private void SelectFirstAvailableConnection()
    {
        var source = ViewModel.Connections;
        if (!source.Contains(ViewModel.SourceConn))
            ViewModel.SourceConn = source.FirstOrDefault() ?? "";

        var target = ViewModel.TargetConnections;
        if (!target.Contains(ViewModel.TargetConn))
            ViewModel.TargetConn = target.FirstOrDefault() ?? "";
    }

#if DEBUG
    /// <summary>只使用本地模拟列表验证首项选中及列表刷新，不会连接数据库。</summary>
    public async Task SmokeTestConnectionDefaultsAsync()
    {
        var initialSources = ViewModel.Connections.ToArray();
        var initialTargets = ViewModel.TargetConnections.ToArray();
        var initialSource = ViewModel.SourceConn;
        var initialTarget = ViewModel.TargetConn;

        try
        {
            ViewModel.Connections.Clear();
            ViewModel.TargetConnections.Clear();
            await Task.Delay(65);
            if (!string.IsNullOrEmpty(ViewModel.SourceConn) ||
                !string.IsNullOrEmpty(ViewModel.TargetConn))
                throw new InvalidOperationException("空列表下拉框不应显示旧选中项");

            ViewModel.Connections.Add("Source_A");
            ViewModel.Connections.Add("Source_B");
            await Task.Delay(65);
            if (ViewModel.SourceConn != "Source_A" ||
                SourceDatabaseCombo.SelectedItem?.ToString() != "Source_A")
                throw new InvalidOperationException("源数据库没有默认显示第一项");

            ViewModel.SourceConn = "Source_B";
            ViewModel.Connections.Add("Source_C");
            await Task.Delay(65);
            if (ViewModel.SourceConn != "Source_B")
                throw new InvalidOperationException("源数据库更新列表覆盖了用户选中的项目");

            ViewModel.TargetConnections.Add("Target_A");
            ViewModel.TargetConnections.Add("Target_B");
            await Task.Delay(65);
            if (ViewModel.TargetConn != "Target_A" ||
                TargetDatabaseCombo.SelectedItem?.ToString() != "Target_A")
                throw new InvalidOperationException("目标数据库加载后没有显示第一项");

            ViewModel.TargetConn = "Target_B";
            ViewModel.TargetConnections.Add("Target_C");
            await Task.Delay(65);
            if (ViewModel.TargetConn != "Target_B")
                throw new InvalidOperationException("目标列表更新覆盖了用户已选择的连接");

            ViewModel.TargetConnections.Clear();
            await Task.Delay(65);
            if (!string.IsNullOrEmpty(ViewModel.TargetConn))
                throw new InvalidOperationException("目标库列表清空后仍然显示旧连接");

            Console.WriteLine("SYNC DEFAULTS OK: source first, target first after load, preserve selection, clear on empty");
        }
        finally
        {
            ViewModel.Connections.Clear();
            foreach (var item in initialSources) ViewModel.Connections.Add(item);
            ViewModel.TargetConnections.Clear();
            foreach (var item in initialTargets) ViewModel.TargetConnections.Add(item);
            await Task.Delay(65);
            ViewModel.SourceConn = initialSources.Contains(initialSource)
                ? initialSource : initialSources.FirstOrDefault() ?? "";
            ViewModel.TargetConn = initialTargets.Contains(initialTarget)
                ? initialTarget : initialTargets.FirstOrDefault() ?? "";
        }
    }
#endif

    private void UpdateConnectionActions()
    {
        var sourceConnected = ViewModel.IsSourceConnected;
        var targetConnected = ViewModel.IsTargetConnected;

        ConnectSourceButton.IsEnabled = !sourceConnected;
        DisconnectSourceButton.IsEnabled = sourceConnected;

        // 目标库必须先连源库；连接成功后也必须禁止重复点击。
        ConnectTargetButton.IsEnabled = sourceConnected && !targetConnected;
        DisconnectTargetButton.IsEnabled = targetConnected;
    }

    // 数据同步窗口以内容形式嵌入主窗体，实际 SizeChanged 来源必须是可见视图。
    private void PageViewport_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        var available = e.NewSize.Width - 50;
        if (available <= 0) return;
        var wide = available >= 800;
        if (_groupsSideBySide == wide) return;
        _groupsSideBySide = wide;

        ConnectionGroups.ColumnDefinitions = wide
            ? new ColumnDefinitions("*,16,*")
            : new ColumnDefinitions("*");
        ConnectionGroups.RowDefinitions = wide
            ? new RowDefinitions("Auto")
            : new RowDefinitions("Auto,16,Auto");
        Grid.SetColumn(SourcePanel, 0);
        Grid.SetRow(SourcePanel, 0);
        Grid.SetColumn(TargetPanel, wide ? 2 : 0);
        Grid.SetRow(TargetPanel, wide ? 0 : 2);
    }

    private bool? _groupsSideBySide;
}
