using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace XCoderMac.Services;

/// <summary>加密解密页面专用 Mac 控件留白：固定每行两枚功能按钮，分组与输入间距统一。</summary>
internal static class MacSecuritySpacing
{
    public static void Apply(Control content)
    {
        if (content is not Grid page)
            throw new InvalidOperationException("加密解密根网格结构已变化。");

        page.ColumnDefinitions = new ColumnDefinitions("296,16,*");
        page.Children.OfType<Grid>().Single(g => Grid.GetColumn(g) == 1)
            .SetValue(Grid.ColumnProperty, 2);
        var left = page.Children.OfType<Border>().Single(b => Grid.GetColumn(b) == 0);
        left.Margin = new Thickness(0);
        left.Padding = new Thickness(8);
        left.CornerRadius = new CornerRadius(12);
        if (left.Child is not ScrollViewer scroll || scroll.Content is not WrapPanel flow)
            throw new InvalidOperationException("加密解密按钮流布局结构已变化。");
        flow.Margin = new Thickness(0);
        flow.Orientation = Orientation.Horizontal;

        var buttons = flow.Children.OfType<Button>().ToArray();
        if (buttons.Length < 30)
            throw new InvalidOperationException("加密解密操作按钮数量异常。");

        foreach (var b in buttons)
        {
            b.Width = 120;
            b.MinWidth = 120;
            b.Height = 40;
            b.Margin = new Thickness(5);
            b.Padding = new Thickness(8, 0);
            b.FontSize = 13;
            b.HorizontalContentAlignment = HorizontalAlignment.Center;
            b.VerticalContentAlignment = VerticalAlignment.Center;
            // 不改变业务 Command，安全包装依旧由 MacSecurityCommandGuard 负责。
        }

        var right = page.Children.OfType<Grid>().Single(g => Grid.GetColumn(g) == 2);
        right.RowSpacing = 10;
        foreach (var group in right.Children.OfType<Border>()
                     .Where(b => b.Classes.Contains("GroupBorder")))
        {
            group.Margin = new Thickness(0);
            group.Padding = new Thickness(12);
            group.CornerRadius = new CornerRadius(12);
        }

        var sourceAndPassword = right.Children.OfType<Border>()
            .Where(b => Grid.GetRow(b) is 0 or 1)
            .Select(b => b.Child).OfType<Grid>().ToArray();
        if (sourceAndPassword.Length != 2)
            throw new InvalidOperationException("加密解密原文/密码的分组结构已变化。");
        foreach (var form in sourceAndPassword)
        {
            // 上游 Row 1 是空白占位，Row 2 才是 TextBox。改成显式两行减少
            // 无意义占位，真正用 RowSpacing=10 表示控件间距。
            var input = form.Children.OfType<TextBox>().Single();
            form.RowDefinitions = new RowDefinitions("Auto,Auto");
            Grid.SetRow(input, 1);
            form.RowSpacing = 10;
            input.Padding = new Thickness(12);
            input.CornerRadius = new CornerRadius(10);
            input.ClipToBounds = true;
            var header = form.Children.OfType<Grid>().Single();
            header.ColumnSpacing = 10;
            foreach (var label in header.Children.OfType<TextBlock>())
                label.Margin = new Thickness(0);
            foreach (var radio in header.Children.OfType<RadioButton>())
                radio.Margin = new Thickness(0);
            foreach (var button in header.Children.OfType<Button>())
            {
                button.Margin = new Thickness(0);
                button.Height = 34;
                button.Padding = new Thickness(12, 0);
                button.MinWidth = 92;
            }
        }

        var options = right.Children.OfType<Border>().Single(b => Grid.GetRow(b) == 2);
        if (options.Child is StackPanel choices)
        {
            choices.Spacing = 14;
            foreach (var child in choices.Children.OfType<Control>())
                child.Margin = new Thickness(0);
        }

        var result = right.Children.OfType<Border>().Single(b => Grid.GetRow(b) == 3);
        if (result.Child is Grid resultForm)
        {
            resultForm.RowSpacing = 10;
            var resultText = resultForm.Children.OfType<TextBox>().Single();
            resultText.Padding = new Thickness(12);
            resultText.CornerRadius = new CornerRadius(10);
            resultText.ClipToBounds = true;
        }
    }

    public static void AssertLayout(Control content)
    {
        if (content is not Grid page)
            throw new InvalidOperationException("加密解密根布局不匹配。");
        var border = page.Children.OfType<Border>().Single(b => Grid.GetColumn(b) == 0);
        var flow = ((ScrollViewer)border.Child!).Content as WrapPanel
            ?? throw new InvalidOperationException("操作按钮流面板缺失。");
        var buttons = flow.Children.OfType<Button>().ToArray();
        var right = page.Children.OfType<Grid>().Single(g => Grid.GetColumn(g) == 2);
        if (page.ColumnDefinitions.Count != 3 || page.ColumnDefinitions[0].Width.Value < 290 ||
            page.ColumnDefinitions[1].Width.Value < 12 ||
            buttons.Length < 30 || right.RowSpacing < 8 ||
            buttons.Any(b => b.Width < 115 || b.Margin.Left < 4 ||
                             b.Margin.Right < 4 || b.Margin.Top < 4 ||
                             b.Margin.Bottom < 4))
            throw new InvalidOperationException("加密解密流布局尺寸与分组间距不正确。");

        // 同排相邻按钮之间留出至少 8px；纵向按钮行相互留出至少 8px。
        var nonEmpty = buttons.Where(b => b.Bounds.Width > 1 && b.Bounds.Height > 1).ToArray();
        if (nonEmpty.Length < 30)
            throw new InvalidOperationException("加密解密按钮没有实际渲染。");
        foreach (var group in nonEmpty.GroupBy(b => Math.Round(b.Bounds.Y / 4.0)))
        {
            var ordered = group.OrderBy(b => b.Bounds.X).ToArray();
            if (ordered.Length > 2 ||
                ordered.Zip(ordered.Skip(1), (a, b) => b.Bounds.Left - a.Bounds.Right)
                    .Any(gap => gap < 8))
                throw new InvalidOperationException("加密解密按钮没有形成两列并保持水平间距。");
        }
        var rows = nonEmpty.Select(b => b.Bounds.Y).Distinct().OrderBy(y => y).ToArray();
        if (rows.Zip(rows.Skip(1), (a, b) => b - a).Any(gap => gap < 48))
            throw new InvalidOperationException("加密解密相邻按钮行之间的竖向间距不足。");
    }
}
