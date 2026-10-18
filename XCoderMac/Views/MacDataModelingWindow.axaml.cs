using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NewLife;
using XCoderAv.ViewModels;
using XCoderMac.Dialogs;

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
