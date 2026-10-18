using System;
using System.Diagnostics;
using System.IO;
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
}
