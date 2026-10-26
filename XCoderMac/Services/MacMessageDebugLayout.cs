using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XCoderMac.Services;

/// <summary>
/// Mac 版“消息调试”布局适配。只调整上游原控件的列宽、分组及控件间距，
/// 保留命令、数据绑定、日志记录和发送行为，不建立网络连接。
/// </summary>
internal static class MacMessageDebugLayout
{
    public static void Apply(Control content)
    {
        if (content is not Grid page)
            throw new InvalidOperationException("消息调试根网格结构已变化。");

        // 四段：连接配置、选项、日志、发送。原本不同分组间仅靠 Margin=6，
        // 视觉上边框相贴。改为统一的真实网格行间距。
        page.RowSpacing = 12;

        var connectPanel = FindPanel(page, 0);
        var optionsPanel = FindPanel(page, 1);
        var sendPanel = FindPanel(page, 3);
        foreach (var panel in new[] { connectPanel, optionsPanel, sendPanel })
        {
            panel.Margin = new Thickness(0);
            panel.Padding = new Thickness(12);
            panel.CornerRadius = new CornerRadius(12);
            panel.ClipToBounds = true;
        }

        if (connectPanel.Child is not Grid connect || connect.Children.Count < 5)
            throw new InvalidOperationException("消息调试连接栏结构已变化。");

        var mode = connect.Children.OfType<ComboBox>().Single();
        var address = connect.Children.OfType<TextBox>().Single();
        var open = connect.Children.OfType<Button>().Single();

        // 连接行模式固定、地址占据剩余宽度、“打开”永远在最右。
        connect.ColumnDefinitions = new ColumnDefinitions("Auto,150,Auto,*,110");
        connect.ColumnSpacing = 12;
        foreach (var child in connect.Children)
        {
            if (Grid.GetColumn(child) == 4) Grid.SetColumn(child, 4);
        }

        foreach (var label in connect.Children.OfType<TextBlock>())
        {
            label.Margin = new Thickness(0);
            label.VerticalAlignment = VerticalAlignment.Center;
        }
        mode.Width = double.NaN;
        mode.Height = 40;
        mode.Margin = new Thickness(0);
        mode.HorizontalAlignment = HorizontalAlignment.Stretch;

        address.Width = double.NaN;
        address.MinWidth = 180;
        address.Height = 40;
        address.Margin = new Thickness(0);
        address.Padding = new Thickness(10, 0);
        address.HorizontalAlignment = HorizontalAlignment.Stretch;
        address.VerticalContentAlignment = VerticalAlignment.Center;

        open.Width = double.NaN;
        open.Height = 40;
        open.MinHeight = 40;
        open.Margin = new Thickness(0);
        open.Padding = new Thickness(12, 0);
        open.HorizontalAlignment = HorizontalAlignment.Stretch;
        open.HorizontalContentAlignment = HorizontalAlignment.Center;
        open.VerticalContentAlignment = VerticalAlignment.Center;

        if (optionsPanel.Child is not WrapPanel options)
            throw new InvalidOperationException("消息调试选项栏不再是 WrapPanel。");

        // 复选框/计数参数都在同一个流面板；原始控件各自还有左右 Margin。
        // 统一去掉重复 Margin，显式给出横向和换行行距。
        options.ItemSpacing = 12;
        options.LineSpacing = 10;
        foreach (var control in options.Children.OfType<Control>())
        {
            control.Margin = new Thickness(0);
            control.VerticalAlignment = VerticalAlignment.Center;
            if (control is TextBox number)
            {
                number.Height = 38;
                number.Padding = new Thickness(8, 0);
                number.VerticalContentAlignment = VerticalAlignment.Center;
            }
        }

        var log = page.Children.OfType<TextBox>().SingleOrDefault(t => Grid.GetRow(t) == 2)
            ?? throw new InvalidOperationException("消息调试日志框缺失。");
        log.Margin = new Thickness(0);
        log.Padding = new Thickness(12);
        log.CornerRadius = new CornerRadius(10);
        log.ClipToBounds = true;

        if (sendPanel.Child is not Grid send)
            throw new InvalidOperationException("消息调试发送栏结构已变化。");
        send.ColumnDefinitions = new ColumnDefinitions("*,100,88");
        send.ColumnSpacing = 12;

        var sendText = send.Children.OfType<TextBox>().Single();
        sendText.Height = 40;
        sendText.Margin = new Thickness(0);
        sendText.Padding = new Thickness(10, 0);
        sendText.VerticalContentAlignment = VerticalAlignment.Center;
        sendText.HorizontalAlignment = HorizontalAlignment.Stretch;

        var sendButtons = send.Children.OfType<Button>().OrderBy(Grid.GetColumn).ToArray();
        if (sendButtons.Length != 2)
            throw new InvalidOperationException("消息调试发送/清空按钮数量异常。");
        for (var i = 0; i < sendButtons.Length; i++)
        {
            var button = sendButtons[i];
            button.Width = double.NaN;
            button.Height = 40;
            button.Margin = new Thickness(0);
            button.Padding = new Thickness(12, 0);
            button.HorizontalContentAlignment = HorizontalAlignment.Center;
            button.VerticalContentAlignment = VerticalAlignment.Center;
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
        }
    }

    public static void AssertLayout(Control content)
    {
        if (content is not Grid page)
            throw new InvalidOperationException("消息调试根网格不匹配。");

        var connect = (Grid)FindPanel(page, 0).Child!;
        var mode = connect.Children.OfType<ComboBox>().Single();
        var address = connect.Children.OfType<TextBox>().Single();
        var open = connect.Children.OfType<Button>().Single();
        var options = FindPanel(page, 1).Child as WrapPanel;
        var send = (Grid)FindPanel(page, 3).Child!;
        var sendButtons = send.Children.OfType<Button>().ToArray();
        var log = page.Children.OfType<TextBox>().Single(t => Grid.GetRow(t) == 2);

        if (page.RowSpacing < 11 ||
            connect.ColumnDefinitions.Count != 5 || connect.ColumnSpacing < 11 ||
            Grid.GetColumn(mode) != 1 || Grid.GetColumn(address) != 3 ||
            Grid.GetColumn(open) != 4 || !double.IsNaN(address.Width) ||
            mode.Bounds.Height < 38 || address.Bounds.Height < 38 ||
            open.Bounds.Height < 38 ||
            options == null || options.ItemSpacing < 11 || options.LineSpacing < 9 ||
            options.Children.OfType<TextBox>().Any(t => t.Bounds.Height < 36) ||
            send.ColumnSpacing < 11 || sendButtons.Length != 2 ||
            sendButtons.Any(b => b.Bounds.Height < 38) ||
            log.CornerRadius.TopLeft < 8 || !log.ClipToBounds)
            throw new InvalidOperationException("消息调试分组/控件间距或尺寸不符合要求。");

        foreach (var panel in new[] { FindPanel(page, 0), FindPanel(page, 1), FindPanel(page, 3) })
            if (panel.Padding.Left < 11 || panel.CornerRadius.TopLeft < 10)
                throw new InvalidOperationException("消息调试分组内边距或圆角不足。");

        // 不只检查属性，还检查实际渲染后的 Bounds，防止视觉上仍紧贴。
        var g0 = FindPanel(page, 0);
        var g1 = FindPanel(page, 1);
        var g3 = FindPanel(page, 3);
        var gap1 = g1.Bounds.Top - g0.Bounds.Bottom;
        var gap2 = log.Bounds.Top - g1.Bounds.Bottom;
        var gap3 = g3.Bounds.Top - log.Bounds.Bottom;
        if (gap1 < 10 || gap2 < 10 || gap3 < 10)
            throw new InvalidOperationException(
                $"消息调试四区间距不足：{gap1:0.0} / {gap2:0.0} / {gap3:0.0}");

        if (connect.Bounds.Width > 650 &&
            (address.Bounds.Width < 220 || open.Bounds.Right < connect.Bounds.Width - 6 ||
             address.Bounds.X <= mode.Bounds.Right))
            throw new InvalidOperationException(
                $"消息调试地址或打开按钮错位：address={address.Bounds}, open={open.Bounds}, connect={connect.Bounds.Width}");
    }

    private static Border FindPanel(Grid page, int row) =>
        page.Children.OfType<Border>().FirstOrDefault(child => Grid.GetRow(child) == row)
        ?? throw new InvalidOperationException($"消息调试缺少第 {row} 行分组。");
}
