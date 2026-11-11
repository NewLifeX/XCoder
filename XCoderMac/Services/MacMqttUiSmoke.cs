#if DEBUG
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using XCoderMac.Views;

namespace XCoderMac.Services;

/// <summary>MQTT 原窗口的 Mac UI 专项测试：不连接 Broker、不发送/订阅消息。</summary>
internal static class MacMqttUiSmoke
{
    public static int Run(string screenshotDirectory)
    {
        MacLegacyToolWindow? window = null;
        try
        {
            var tool = MacToolCatalog.Tools.Single(t => t.Title == "MQTT 客户端");
            window = new MacLegacyToolWindow(tool);
            window.Width = 1140;
            window.Height = 830;
            window.Show();
            window.UpdateLayout();

            var root = window.HostedToolContent
                ?? throw new InvalidOperationException("MQTT 未载入 Mac 宿主。");
            MacMqttLayout.AssertLayout(root);
            var actions = root.GetVisualDescendants().OfType<Button>()
                .Where(b => b.Classes.Contains("mqtt-action")).ToArray();
            if (actions.Length != 2)
                throw new InvalidOperationException($"MQTT 连接与订阅按钮数量异常：{actions.Length}");
            foreach (var action in actions)
                VerifyButton(action);

            var log = MacMqttLayout.LogBox(root);
            CheckLogStates(log);
            var inputs = root.GetVisualDescendants().OfType<TextBox>()
                .Where(t => t.AcceptsReturn && !t.IsReadOnly).ToArray();
            if (inputs.Length != 1 || inputs[0].CornerRadius.TopLeft < 8 ||
                log.CornerRadius.TopLeft < 8 || !log.ClipToBounds)
                throw new InvalidOperationException("MQTT 发布内容与日志文本框圆角未生效。");

            Directory.CreateDirectory(screenshotDirectory);
            Save(window, Path.Combine(screenshotDirectory, "10-MQTT客户端-布局与深色日志.png"));

            window.Width = 900;
            window.UpdateLayout();
            MacMqttLayout.AssertLayout(root);
            Save(window, Path.Combine(screenshotDirectory, "10-MQTT客户端-窄窗三列布局.png"));
            Console.WriteLine("MQTT LAYOUT OK: connection | publish | subscription on top, full-width taller log below, compact clear overlay; 1140/900 viewports");
            Console.WriteLine("MQTT BUTTON HOVER OK: background and text change; no truncated button text");
            Console.WriteLine("MQTT LOG OK: dark in normal/pointerover/focus/focus-within; rounded content and log text boxes");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("MQTT UI FAILED: " + ex);
            return 1;
        }
        finally
        {
            window?.Close();
        }
    }

    private static void VerifyButton(Button action)
    {
        if (action.Bounds.Width < 100 || action.Bounds.Height < 38 ||
            action.Content is not string text || string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException($"MQTT 按钮文字可能截断: {action.Content}, {action.Bounds}");

        var before = (ColorOf(action.Background), ColorOf(action.Foreground));
        SetPseudo(action, ":pointerover", true);
        try
        {
            var after = (ColorOf(action.Background), ColorOf(action.Foreground));
            if (before == after || before.Item2 == after.Item2 ||
                after.Item2.R < 230 || after.Item2.G < 230 || after.Item2.B < 230)
                throw new InvalidOperationException($"MQTT 按钮悬停未切换文字/背景色：{text}, {before} -> {after}");
        }
        finally { SetPseudo(action, ":pointerover", false); }
    }

    private static void CheckLogStates(TextBox log)
    {
        void Check(string state)
        {
            var bg = ColorOf(log.Background);
            var fg = ColorOf(log.Foreground);
            var borders = log.GetVisualDescendants().OfType<Border>()
                .Where(b => b.Name == "PART_BorderElement").ToArray();
            if (bg.R >= 45 || bg.G >= 45 || bg.B >= 60 ||
                fg.R < 180 || fg.G < 180 || fg.B < 180 ||
                borders.Any(b => ColorOf(b.Background).R >= 45 ||
                    ColorOf(b.Background).G >= 45 || ColorOf(b.Background).B >= 60))
                throw new InvalidOperationException($"MQTT 日志 {state} 失去黑底浅字：{bg}/{fg}");
        }

        Check("normal");
        foreach (var state in new[] { ":pointerover", ":focus", ":focus-within" })
        {
            SetPseudo(log, state, true);
            try { Check(state); }
            finally { SetPseudo(log, state, false); }
        }
    }

    private static Avalonia.Media.Color ColorOf(IBrush? brush) =>
        brush is ISolidColorBrush color ? color.Color : default;

    private static void SetPseudo(Control control, string state, bool on)
    {
        var property = typeof(StyledElement).GetProperty("PseudoClasses",
            BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("无法访问 Avalonia 的控件状态");
        var pseudo = property.GetValue(control)!;
        var interfaceType = pseudo.GetType().GetInterfaces()
            .First(i => i.Name == "IPseudoClasses");
        var method = interfaceType.GetMethod(on ? "Add" : "Remove")!;
        method.Invoke(pseudo, [state]);
    }

    private static void Save(Window window, string output)
    {
        window.UpdateLayout();
        using var picture = new RenderTargetBitmap(
            new PixelSize((int)Math.Ceiling(window.Bounds.Width * 2),
                           (int)Math.Ceiling(window.Bounds.Height * 2)),
            new Vector(192, 192));
        picture.Render(window);
        picture.Save(output);
        Console.WriteLine("MQTT SCREENSHOT: " + output);
    }
}
#endif
