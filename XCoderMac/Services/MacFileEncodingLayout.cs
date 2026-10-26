using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XCoderMac.Services;

/// <summary>
/// Mac 文件编码视图：保留 XCoderAv 的原始控件与业务绑定，
/// 统一分组间距、输入控件高度及日志间距。
/// </summary>
internal static class MacFileEncodingLayout
{
    public static void Apply(Control content)
    {
        if (content is not Grid page)
            throw new InvalidOperationException("文件编码工具根布局已变化，需要复核 Mac 适配。");

        // 前两组与扫描结果/日志四个区域全部使用明确的行间距。
        // 不能仅设置 GroupBorder.Margin：原来两个边框几乎紧贴。
        page.RowSpacing = 12;

        var folderPanel = FindPanel(page, 0);
        var options = FindPanel(page, 1);
        foreach (var panel in new[] { folderPanel, options })
        {
            panel.Margin = new Thickness(0);
            panel.Padding = new Thickness(12);
            panel.CornerRadius = new CornerRadius(12);
            panel.ClipToBounds = true;
        }

        if (folderPanel.Child is not Grid folderGrid)
            throw new InvalidOperationException("文件编码文件夹配置栏结构已变化。");
        folderGrid.ColumnSpacing = 12;

        var actions = folderGrid.Children.OfType<Button>().ToArray();
        if (actions.Length != 3)
            throw new InvalidOperationException("文件编码操作按钮数量异常。");

        int[] widths = [88, 88, 124];
        for (var i = 0; i < actions.Length; i++)
        {
            actions[i].Width = widths[i];
            actions[i].Height = 40;
            actions[i].MinHeight = 40;
            actions[i].Margin = new Thickness(0);
            actions[i].Padding = new Thickness(12, 0);
            actions[i].HorizontalContentAlignment = HorizontalAlignment.Center;
            actions[i].VerticalContentAlignment = VerticalAlignment.Center;
        }

        var folderInput = folderGrid.Children.OfType<TextBox>().Single();
        folderInput.Height = 40;
        folderInput.Margin = new Thickness(0);
        folderInput.Padding = new Thickness(10, 0);
        folderInput.CornerRadius = new CornerRadius(9);
        var folderLabel = folderGrid.Children.OfType<TextBlock>().Single();
        folderLabel.Margin = new Thickness(0);
        folderLabel.VerticalAlignment = VerticalAlignment.Center;

        if (options.Child is not StackPanel oldPanel || oldPanel.Children.Count != 4)
            throw new InvalidOperationException("文件编码选项行结构已变化。");

        var controls = oldPanel.Children.ToArray();
        oldPanel.Children.Clear();

        // “后缀过滤”占弹性列，“目标编码”选项固定在右侧。
        var row = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,Auto"),
            ColumnSpacing = 12,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        for (var i = 0; i < controls.Length; i++)
        {
            Grid.SetColumn(controls[i], i);
            row.Children.Add(controls[i]);
        }

        if (controls[1] is not TextBox suffix || controls[3] is not ComboBox encoding)
            throw new InvalidOperationException("文件编码后缀或目标编码控件不匹配。");

        suffix.Width = double.NaN;
        suffix.MinWidth = 160;
        suffix.Height = 40;
        suffix.Margin = new Thickness(0);
        suffix.Padding = new Thickness(10, 0);
        suffix.CornerRadius = new CornerRadius(9);
        suffix.HorizontalAlignment = HorizontalAlignment.Stretch;
        encoding.Width = 140;
        encoding.Height = 40;
        encoding.Margin = new Thickness(0);
        encoding.VerticalContentAlignment = VerticalAlignment.Center;

        foreach (var label in row.Children.OfType<TextBlock>())
        {
            label.Margin = new Thickness(0);
            label.VerticalAlignment = VerticalAlignment.Center;
        }
        options.Child = row;

        var scan = page.Children.OfType<Grid>().SingleOrDefault(g => Grid.GetRow(g) == 2)
            ?? throw new InvalidOperationException("文件编码扫描结果区域缺失。");
        scan.Margin = new Thickness(0);
        scan.RowSpacing = 10;
        scan.Children.OfType<TextBlock>().Single().Margin = new Thickness(0);
        var scanText = scan.Children.OfType<TextBox>().Single();
        scanText.Margin = new Thickness(0);
        scanText.Padding = new Thickness(12);
        scanText.CornerRadius = new CornerRadius(10);
        scanText.ClipToBounds = true;

        var log = page.Children.OfType<TextBox>().SingleOrDefault(t => Grid.GetRow(t) == 3)
            ?? throw new InvalidOperationException("文件编码日志区域缺失。");
        log.Margin = new Thickness(0);
        log.MinHeight = 72;
        log.Padding = new Thickness(12);
        log.CornerRadius = new CornerRadius(10);
        log.ClipToBounds = true;
    }

    /// <summary>实际界面布局后验证分组之间的几何间距，防止只改静态样式却仍然贴合。</summary>
    public static void AssertLayout(Control content)
    {
        if (content is not Grid page || FindPanel(page, 0).Child is not Grid folder)
            throw new InvalidOperationException("找不到文件编码配置行。");

        var top = FindPanel(page, 0);
        var second = FindPanel(page, 1);
        var actions = folder.Children.OfType<Button>().ToArray();
        var input = folder.Children.OfType<TextBox>().Single();
        var options = second.Child as Grid;
        var suffix = options?.Children.OfType<TextBox>().SingleOrDefault();
        var encoding = options?.Children.OfType<ComboBox>().SingleOrDefault();
        var scan = page.Children.OfType<Grid>().Single(g => Grid.GetRow(g) == 2);
        var scanText = scan.Children.OfType<TextBox>().Single();
        var log = page.Children.OfType<TextBox>().Single(t => Grid.GetRow(t) == 3);

        if (page.RowSpacing < 11 || folder.ColumnSpacing < 11 ||
            top.Padding.Left < 11 || second.Padding.Left < 11 ||
            top.CornerRadius.TopLeft < 10 || second.CornerRadius.TopLeft < 10 ||
            actions.Length != 3 || actions.Any(b => b.Bounds.Height < 38) ||
            actions[2].Bounds.Width < 110 || input.Bounds.Height < 38 ||
            options?.ColumnDefinitions.Count != 4 || options.ColumnSpacing < 11 ||
            suffix == null || encoding == null || !double.IsNaN(suffix.Width) ||
            Grid.GetColumn(suffix) != 1 || Grid.GetColumn(encoding) != 3 ||
            suffix.Bounds.Height < 38 || encoding.Bounds.Height < 38 ||
            scan.RowSpacing < 9 || scanText.CornerRadius.TopLeft < 8 ||
            log.CornerRadius.TopLeft < 8)
            throw new InvalidOperationException("文件编码分组间距、控件尺寸或圆角不符合要求。");

        var gap = second.Bounds.Top - top.Bounds.Bottom;
        var scanGap = scan.Bounds.Top - second.Bounds.Bottom;
        var logGap = log.Bounds.Top - scan.Bounds.Bottom;
        if (gap < 10 || scanGap < 10 || logGap < 10)
            throw new InvalidOperationException(
                $"文件编码实际分组贴合：groupGap={gap:0.0}, scanGap={scanGap:0.0}, logGap={logGap:0.0}");

        if (options.Bounds.Width > 650 &&
            (suffix.Bounds.Width < 300 ||
             encoding.Bounds.Right < options.Bounds.Width - 10))
            throw new InvalidOperationException(
                $"文件编码选项行未靠右：suffix={suffix.Bounds.Width:0}, encodingRight={encoding.Bounds.Right:0}, row={options.Bounds.Width:0}");
    }

    private static Border FindPanel(Grid page, int row) =>
        page.Children.OfType<Border>().FirstOrDefault(child => Grid.GetRow(child) == row)
        ?? throw new InvalidOperationException($"文件编码工具缺少第 {row} 行配置。");
}
