using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace XCoderMac.Services;

/// <summary>
/// MQTT 的 macOS 专用布局。顶部连接/发布/订阅三卡并排，底部日志跨全宽；
/// 移动原有控件而非复制 ViewModel/Command，不修改 XCoderAv。
/// </summary>
internal static class MacMqttLayout
{
    public static void Apply(Control content)
    {
        var root = Root(content);
        var leftScroll = root.Children.OfType<ScrollViewer>().SingleOrDefault()
            ?? throw new InvalidOperationException("MQTT 左侧配置区域缺失。");
        var leftGroups = leftScroll.Content as StackPanel
            ?? throw new InvalidOperationException("MQTT 左侧配置组结构已变化。");
        var oldRight = root.Children.OfType<Grid>().SingleOrDefault()
            ?? throw new InvalidOperationException("MQTT 右侧发布/日志布局缺失。");

        var leftCards = leftGroups.Children.OfType<Border>().ToArray();
        var rightCards = oldRight.Children.OfType<Border>().OrderBy(Grid.GetRow).ToArray();
        if (leftCards.Length != 2 || rightCards.Length != 2)
            throw new InvalidOperationException("MQTT 连接/订阅/发布/日志卡片数量已变化。");

        var connectCard = leftCards[0];
        var subscribeCard = leftCards[1];
        var publishCard = rightCards[0];
        var logCard = rightCards[1];

        // 重新使用原卡片与控件。先解除逻辑父级，防止出现重复父容器。
        leftGroups.Children.Remove(connectCard);
        leftGroups.Children.Remove(subscribeCard);
        oldRight.Children.Remove(publishCard);
        oldRight.Children.Remove(logCard);
        root.Children.Remove(leftScroll);
        root.Children.Remove(oldRight);

        // 900 宽仍确保连接地址和发布主题有足够空间；日志横跨整个区域。
        root.ColumnDefinitions = new ColumnDefinitions("295,14,*,14,232");
        root.RowDefinitions = new RowDefinitions("Auto,14,*");
        // 窗口缩窄时只缩小左右卡片，而不是把发布主题输入压成一条细缝。
        // 根网格在同一页内按宽度响应式调整，日志仍始终跨全宽。
        var compactColumns = false;
        root.SizeChanged += (_, e) =>
        {
            var compact = e.NewSize.Width < 960;
            if (compact == compactColumns) return;
            compactColumns = compact;
            root.ColumnDefinitions = new ColumnDefinitions(
                compact ? "258,12,*,12,204" : "295,14,*,14,232");
        };
        Place(root, connectCard, 0, 0);
        Place(root, publishCard, 0, 2);
        Place(root, subscribeCard, 0, 4);
        Place(root, logCard, 2, 0, 5);

        foreach (var card in new[] { connectCard, publishCard, subscribeCard, logCard })
        {
            card.Margin = new Thickness(0);
            card.Padding = new Thickness(14);
            card.CornerRadius = new CornerRadius(12);
            card.ClipToBounds = true;
        }

        // 连接：保留原服务地址、客户端 ID、用户/密码及连接命令。
        var connect = Form(connectCard);
        TuneForm(connect, 9, 8, "48,*,0");
        foreach (var b in connect.Children.OfType<Button>())
            TuneAction(b, 108);
        foreach (var input in connect.Children.OfType<TextBox>())
            TuneSingleLine(input, 36);

        // 订阅：放到发布的右边。窄卡片改为主题、QoS、订阅三行，
        // 防止原 QoS 挤压主题输入框。
        var subscribe = Form(subscribeCard);
        subscribe.ColumnDefinitions = new ColumnDefinitions("48,*");
        subscribe.RowDefinitions = new RowDefinitions("Auto,Auto,Auto");
        subscribe.ColumnSpacing = 8;
        subscribe.RowSpacing = 10;
        var topicText = subscribe.Children.OfType<TextBox>().Single();
        TuneSingleLine(topicText, 36);
        var qos = subscribe.Children.OfType<ComboBox>().Single();
        qos.Width = 80;
        qos.Height = 36;
        qos.Margin = new Thickness(0);
        Grid.SetRow(qos, 1);
        Grid.SetColumn(qos, 1);
        var subscribeButton = subscribe.Children.OfType<Button>().Single();
        Grid.SetRow(subscribeButton, 2);
        Grid.SetColumn(subscribeButton, 0);
        Grid.SetColumnSpan(subscribeButton, 2);
        TuneAction(subscribeButton, 108);
        subscribe.Children.Add(new TextBlock
        {
            Text = "QoS",
            Foreground = new SolidColorBrush(Color.Parse("#294563")),
            VerticalAlignment = VerticalAlignment.Center
        }.At(1, 0));
        foreach (var label in subscribe.Children.OfType<TextBlock>())
            label.Margin = new Thickness(0);

        // 发布：缩短多行消息区，减少卡片高度，仍保留主题 / QoS / 保留 / 发布。
        if (publishCard.Child is not Grid publish || publish.Children.Count != 3)
            throw new InvalidOperationException("MQTT 发布区结构已变化。");
        var topic = publish.Children.OfType<Grid>().Single();
        var message = publish.Children.OfType<TextBox>().Single();
        var send = publish.Children.OfType<Button>().Single();

        publish.RowDefinitions = new RowDefinitions("Auto,Auto,82,Auto");
        publish.RowSpacing = 9;
        Grid.SetRow(topic, 1);
        Grid.SetRow(message, 2);
        Grid.SetRow(send, 3);
        publish.Children.Add(new TextBlock
        {
            Text = "发布配置",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Foreground = new SolidColorBrush(Color.Parse("#294563")),
            Margin = new Thickness(0, 0, 0, 2)
        }.At(0, 0));

        topic.ColumnDefinitions = new ColumnDefinitions("42,*,66,Auto");
        topic.ColumnSpacing = 7;
        foreach (var input in topic.Children.OfType<TextBox>())
            TuneSingleLine(input, 36);
        foreach (var combo in topic.Children.OfType<ComboBox>())
        {
            combo.Width = 66;
            combo.Height = 36;
            combo.Margin = new Thickness(0);
        }
        foreach (var label in topic.Children.OfType<TextBlock>())
            label.Margin = new Thickness(0);
        foreach (var check in topic.Children.OfType<CheckBox>())
            check.Margin = new Thickness(0);

        message.Height = double.NaN;
        message.MinHeight = 70;
        message.Margin = new Thickness(0);
        message.Padding = new Thickness(10);
        message.CornerRadius = new CornerRadius(10);
        message.ClipToBounds = true;
        message.TextWrapping = TextWrapping.Wrap;
        send.Margin = new Thickness(0);
        send.Width = 100;
        send.Height = 38;
        send.HorizontalAlignment = HorizontalAlignment.Left;
        send.HorizontalContentAlignment = HorizontalAlignment.Center;

        // 日志：跨全宽且占据剩余高度。清空移动到日志内容右下角的小悬浮按钮；
        // 继续使用原 ClearLogCommand，不占用日志标题工具栏的高度。
        if (logCard.Child is not Grid logs ||
            logs.Children.OfType<TextBox>().SingleOrDefault() is not TextBox log ||
            logs.Children.OfType<Grid>().SingleOrDefault() is not Grid header)
            throw new InvalidOperationException("MQTT 日志区结构已变化。");

        logs.RowDefinitions = new RowDefinitions("Auto,*");
        logs.RowSpacing = 8;
        var clear = header.Children.OfType<Button>().Single();
        header.Children.Remove(clear);
        var heading = header.Children.OfType<TextBlock>().Single();
        heading.Margin = new Thickness(0);
        clear.Width = 64;
        clear.Height = 28;
        clear.MinHeight = 28;
        clear.Padding = new Thickness(8, 0);
        clear.Margin = new Thickness(0, 0, 8, 8);
        clear.HorizontalAlignment = HorizontalAlignment.Right;
        clear.VerticalAlignment = VerticalAlignment.Bottom;
        Grid.SetRow(clear, 1);
        Grid.SetColumn(clear, 0);
        logs.Children.Add(clear);
        clear.SetValue(Panel.ZIndexProperty, 1);

        log.Classes.Add("dark-console");
        log.ClearValue(TemplatedControl.BackgroundProperty);
        log.ClearValue(TemplatedControl.ForegroundProperty);
        log.Margin = new Thickness(0);
        log.Padding = new Thickness(12, 12, 12, 42);
        log.CornerRadius = new CornerRadius(10);
        log.ClipToBounds = true;
        log.TextWrapping = TextWrapping.Wrap;
        log.BorderThickness = new Thickness(1);
    }

    public static TextBox LogBox(Control content)
    {
        var root = Root(content);
        var logs = root.Children.OfType<Border>().Single(b => Grid.GetRow(b) == 2);
        return ((Grid)logs.Child!).Children.OfType<TextBox>().Single();
    }

    public static void AssertLayout(Control content)
    {
        var root = Root(content);
        var cards = root.Children.OfType<Border>().ToArray();
        if (cards.Length != 4 || root.ColumnDefinitions.Count != 5 || root.RowDefinitions.Count != 3)
            throw new InvalidOperationException("MQTT 主布局不是顶部三卡、底部日志。");

        var connectCard = cards.Single(c => Grid.GetRow(c) == 0 && Grid.GetColumn(c) == 0);
        var publishCard = cards.Single(c => Grid.GetRow(c) == 0 && Grid.GetColumn(c) == 2);
        var subscribeCard = cards.Single(c => Grid.GetRow(c) == 0 && Grid.GetColumn(c) == 4);
        var logCard = cards.Single(c => Grid.GetRow(c) == 2);
        var logForm = (Grid)logCard.Child!;
        var publishForm = (Grid)publishCard.Child!;
        var log = LogBox(content);
        var clear = logForm.Children.OfType<Button>().SingleOrDefault();
        var subscribe = Form(subscribeCard);
        var connect = Form(connectCard);

        if (Grid.GetColumnSpan(logCard) != 5 ||
            connectCard.Bounds.Right + 10 > publishCard.Bounds.Left ||
            publishCard.Bounds.Right + 10 > subscribeCard.Bounds.Left ||
            logCard.Bounds.Top - connectCard.Bounds.Bottom < 10 ||
            logCard.Bounds.Width < root.Bounds.Width - 3 ||
            subscribe.RowDefinitions.Count != 3 ||
            subscribe.RowSpacing < 9 ||
            publishForm.RowDefinitions.Count != 4 ||
            publishForm.RowDefinitions[2].Height.Value > 90 ||
            publishForm.RowSpacing < 8 ||
            clear == null || clear.Command == null ||
            Grid.GetRow(clear) != 1 || clear.Bounds.Height > 30 ||
            clear.Bounds.Right > log.Bounds.Right || clear.Bounds.Right < log.Bounds.Right - 20 ||
            !log.Classes.Contains("dark-console") ||
            log.Background is not ISolidColorBrush bg ||
            bg.Color.R > 35 || bg.Color.G > 35 || bg.Color.B > 50)
            throw new InvalidOperationException("MQTT 布局、清空按钮或日志颜色不符合要求。");

        var compact = root.Bounds.Width < 960;
        if (root.Bounds.Width > 780 &&
            (connectCard.Bounds.Width < (compact ? 250 : 285) ||
             subscribeCard.Bounds.Width < (compact ? 200 : 225) ||
             publishCard.Bounds.Width < 300 || log.Bounds.Height < 230 ||
             log.Bounds.Height <= publishCard.Bounds.Height))
            throw new InvalidOperationException(
                $"MQTT 页面拥挤：connect={connectCard.Bounds}, publish={publishCard.Bounds}, subscribe={subscribeCard.Bounds}, log={log.Bounds}");

        if (connect.Children.OfType<Button>().Count() != 1 ||
            subscribe.Children.OfType<Button>().Count() != 1 ||
            publishForm.Children.OfType<Button>().Count() != 1 ||
            connect.Children.OfType<Button>().Any(b => b.Command == null) ||
            subscribe.Children.OfType<Button>().Any(b => b.Command == null) ||
            publishForm.Children.OfType<Button>().Any(b => b.Command == null))
            throw new InvalidOperationException("MQTT 连接/订阅/发布命令按钮丢失。");
    }

    private static Grid Root(Control content) =>
        content as Grid ?? throw new InvalidOperationException("MQTT 源窗口根元素已变化。");

    private static Grid Form(Border card)
    {
        if (card.Child is not StackPanel stack || stack.Children.Count != 2 ||
            stack.Children[1] is not Grid form)
            throw new InvalidOperationException("MQTT 连接/订阅分组结构已变化。");
        stack.Spacing = 10;
        return form;
    }

    private static void TuneForm(Grid form, double rowSpacing, double colSpacing, string columns)
    {
        form.RowSpacing = rowSpacing;
        form.ColumnSpacing = colSpacing;
        form.ColumnDefinitions = new ColumnDefinitions(columns);
        foreach (var label in form.Children.OfType<TextBlock>())
            label.Margin = new Thickness(0);
        foreach (var combo in form.Children.OfType<ComboBox>())
        {
            combo.Width = 76;
            combo.Height = 36;
            combo.Margin = new Thickness(0);
        }
    }

    private static void TuneSingleLine(TextBox input, double height)
    {
        input.Height = height;
        input.Margin = new Thickness(0);
        input.Padding = new Thickness(9, 0);
        input.VerticalContentAlignment = VerticalAlignment.Center;
    }

    private static void TuneAction(Button b, double width)
    {
        b.Classes.Add("mqtt-action");
        b.Width = width;
        b.MinWidth = width;
        b.Height = 40;
        b.Padding = new Thickness(12, 0);
        b.Margin = new Thickness(0, 2, 0, 0);
        b.HorizontalAlignment = HorizontalAlignment.Left;
        b.HorizontalContentAlignment = HorizontalAlignment.Center;
        b.VerticalContentAlignment = VerticalAlignment.Center;
    }

    private static void Place(Grid parent, Control child, int row, int col, int colSpan = 1)
    {
        Grid.SetRow(child, row);
        Grid.SetColumn(child, col);
        Grid.SetColumnSpan(child, colSpan);
        parent.Children.Add(child);
    }

    private static T At<T>(this T control, int row, int col) where T : Control
    {
        Grid.SetRow(control, row);
        Grid.SetColumn(control, col);
        return control;
    }
}
