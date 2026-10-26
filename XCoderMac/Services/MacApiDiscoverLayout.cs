using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace XCoderMac.Services;

/// <summary>
/// 原 XCoderAv API 发现页的 Mac 适配：修正两个“清空”按钮的网格重叠，
/// 对 ListBox 服务列表及 TextBox 日志都加真实圆角容器。保留所有原始业务绑定。
/// </summary>
internal static class MacApiDiscoverLayout
{
    public static void Apply(Control content)
    {
        if (content is not Grid page)
            throw new InvalidOperationException("API 发现页面根布局发生变化。");

        var top = page.Children.OfType<Border>().SingleOrDefault(b => Grid.GetRow(b) == 0);
        if (top?.Child is not Grid actions)
            throw new InvalidOperationException("API 发现配置栏缺失。");

        var buttons = actions.Children.OfType<Button>().ToArray();
        if (buttons.Length != 3 ||
            buttons.Any(b => b.Content is not string))
            throw new InvalidOperationException("API 发现操作按钮数量/结构发生变化。");

        // 上游“清空服务”“清空日志”都在列 3，其中日志按钮有 90px 右 Margin，
        // 视觉叠加后导致文字裁切。拆为 3 个独立列，不更改 Command。
        actions.ColumnDefinitions = new ColumnDefinitions("Auto,92,94,112,112");
        actions.ColumnSpacing = 10;
        foreach (var button in buttons)
        {
            var label = button.Content?.ToString();
            Grid.SetColumn(button, label switch
            {
                "扫描" => 2,
                "清空服务" => 3,
                "清空日志" => 4,
                _ => throw new InvalidOperationException("未知 API 发现按钮: " + label)
            });
            button.Classes.Add("api-discover-action");
            button.Width = double.NaN;
            button.Height = 38;
            button.MinWidth = label == "扫描" ? 90 : 108;
            button.Margin = new Thickness(0);
            button.Padding = new Thickness(14, 0);
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            button.HorizontalContentAlignment = HorizontalAlignment.Center;
            button.VerticalContentAlignment = VerticalAlignment.Center;
        }

        var port = actions.Children.OfType<TextBox>().Single();
        port.Width = double.NaN;
        port.Height = 38;
        port.Margin = new Thickness(0);
        port.Padding = new Thickness(9, 0);

        // “发现的服务”使用 ListBox（而非 TextBox）；用裁剪圆角 Border 包裹，
        // 免受上游 ListBox 默认方形模板边框影响。
        var serviceArea = page.Children.OfType<Grid>().SingleOrDefault(g => Grid.GetRow(g) == 1)
            ?? throw new InvalidOperationException("API 发现服务列表区域缺失。");
        var services = serviceArea.Children.OfType<ListBox>().SingleOrDefault()
            ?? throw new InvalidOperationException("API 发现服务列表控件缺失。");
        var row = Grid.GetRow(services);
        serviceArea.Children.Remove(services);
        services.Margin = new Thickness(0);
        services.BorderThickness = new Thickness(0);
        services.Background = Brushes.Transparent;
        var serviceBorder = RoundedPanel(services, new Thickness(0, 4, 0, 4));
        Grid.SetRow(serviceBorder, row);
        serviceArea.Children.Add(serviceBorder);

        // 日志原来是 TextBox，但其默认模板外边框是方形：也包裹同款圆角 Border，
        // 并取消模板自己绘制边框。保留 Log 的原双向绑定与日志行为。
        var log = page.Children.OfType<TextBox>().SingleOrDefault(t => Grid.GetRow(t) == 2)
            ?? throw new InvalidOperationException("API 发现日志控件缺失。");
        page.Children.Remove(log);
        log.Margin = new Thickness(0);
        log.BorderThickness = new Thickness(0);
        log.CornerRadius = new CornerRadius(10);
        log.ClipToBounds = true;
        log.Padding = new Thickness(12);
        var logBorder = RoundedPanel(log, new Thickness(0, 4, 0, 0));
        Grid.SetRow(logBorder, 2);
        page.Children.Add(logBorder);
    }

    private static Border RoundedPanel(Control child, Thickness margin) => new()
    {
        Child = child,
        CornerRadius = new CornerRadius(11),
        BorderThickness = new Thickness(1),
        BorderBrush = new SolidColorBrush(Color.Parse("#D5E2F2")),
        Background = new SolidColorBrush(Color.Parse("#F8FAFE")),
        ClipToBounds = true,
        Margin = margin
    };

    public static (Grid Actions, Button[] Buttons, Border ServicePanel, ListBox Services,
        Border LogPanel, TextBox Log) GetControls(Control content)
    {
        if (content is not Grid page)
            throw new InvalidOperationException("API 发现页面根布局异常。");
        var actions = page.Children.OfType<Border>().Single(b => Grid.GetRow(b) == 0).Child as Grid
            ?? throw new InvalidOperationException("API 发现配置区域异常。");
        var serviceArea = page.Children.OfType<Grid>().Single(g => Grid.GetRow(g) == 1);
        var listPanel = serviceArea.Children.OfType<Border>().Single(b => Grid.GetRow(b) == 1);
        var logPanel = page.Children.OfType<Border>().Single(b => Grid.GetRow(b) == 2);
        return (actions, actions.Children.OfType<Button>().ToArray(),
            listPanel, (ListBox)listPanel.Child!, logPanel, (TextBox)logPanel.Child!);
    }

    public static void AssertLayout(Control content)
    {
        var (actions, buttons, listPanel, list, logPanel, log) = GetControls(content);
        if (actions.ColumnDefinitions.Count != 5 || actions.ColumnSpacing < 8 ||
            buttons.Length != 3 || !buttons.All(b => b.Classes.Contains("api-discover-action")) ||
            buttons.Select(Grid.GetColumn).Distinct().Count() != 3 ||
            buttons.Any(b => b.Bounds.Width < 88 || b.Bounds.Height < 36) ||
            listPanel.CornerRadius.TopLeft < 10 || logPanel.CornerRadius.TopLeft < 10 ||
            !listPanel.ClipToBounds || !logPanel.ClipToBounds ||
            list.BorderThickness.Left != 0 || log.BorderThickness.Left != 0 ||
            listPanel.Bounds.Width < 400 || logPanel.Bounds.Width < 400)
            throw new InvalidOperationException("API 发现按钮间距/文字或服务与日志圆角不符合要求。");

        var ordered = buttons.OrderBy(b => b.Bounds.X).ToArray();
        if (ordered.Zip(ordered.Skip(1), (a,b) => b.Bounds.Left - a.Bounds.Right)
            .Any(gap => gap < 7))
            throw new InvalidOperationException("API 发现按钮发生重叠或间距过小。");
    }
}
