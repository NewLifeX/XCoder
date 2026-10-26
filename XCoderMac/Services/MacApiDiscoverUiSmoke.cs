#if DEBUG
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using XCoderMac.Views;
using XCoderAv.ViewModels;

namespace XCoderMac.Services;

/// <summary>API 发现专用 UI 冒烟：只打开视图，不做 UDP 扫描、不发任何网络请求。</summary>
internal static class MacApiDiscoverUiSmoke
{
    public static int Run(string screenshotDirectory)
    {
        MacLegacyToolWindow? window = null;
        try
        {
            var tool = MacToolCatalog.Tools.Single(t => t.Title == "API 发现");
            window = new MacLegacyToolWindow(tool)
            {
                Width = 1060,
                Height = 850
            };
            window.Show();
            window.UpdateLayout();
            var root = window.HostedToolContent
                ?? throw new InvalidOperationException("API 发现窗口未装载到 Mac 宿主。");
            MacApiDiscoverLayout.AssertLayout(root);
            var (actions, buttons, listBorder, list, logBorder, log) =
                MacApiDiscoverLayout.GetControls(root);
            var vm = window.HostedViewModel as ApiDiscoverViewModel
                ?? throw new InvalidOperationException("API 发现 ViewModel 丢失。");
            if (vm.Port != 5500 || !ReferenceEquals(list.ItemsSource, vm.Services) ||
                buttons.Any(b => b.Command == null))
                throw new InvalidOperationException("API 发现端口、服务数据或按钮命令丢失。");

            foreach (var button in buttons)
                CheckButtonHover(button);

            // 圆角容器和裁剪适用于服务列表 ListBox 及日志 TextBox 两种不同控件。
            CheckPanel(listBorder, list, "发现的服务");
            CheckPanel(logBorder, log, "日志");
            CheckLogStates(log);

            Directory.CreateDirectory(screenshotDirectory);
            var image = Path.Combine(screenshotDirectory, "04-API发现-按钮与圆角修复.png");
            Save(window, image);
            window.Width = 850;
            window.UpdateLayout();
            MacApiDiscoverLayout.AssertLayout(root);

            Console.WriteLine("API DISCOVER LAYOUT OK: 3 separated buttons, commands kept, 1060/850 widths");
            Console.WriteLine("API DISCOVER HOVER OK: all buttons change background and text to white on pointerover");
            Console.WriteLine("API DISCOVER CORNERS OK: ListBox and TextBox rounded/clipped in normal/pointerover/focus");
            Console.WriteLine("API DISCOVER SCREENSHOT: " + image);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("API DISCOVER UI FAILED: " + ex);
            return 1;
        }
        finally { window?.Close(); }
    }

    private static void CheckButtonHover(Button button)
    {
        if (button.Content is not string name || button.Bounds.Width < 90 ||
            button.Bounds.Height < 36)
            throw new InvalidOperationException($"API 发现按钮尺寸太小：{button.Content}/{button.Bounds}");

        var before = (ColorOf(button.Background), ColorOf(button.Foreground));
        SetPseudo(button, ":pointerover", true);
        try
        {
            var after = (ColorOf(button.Background), ColorOf(button.Foreground));
            if (before.Item1 == after.Item1 || before.Item2 == after.Item2 ||
                after.Item2.R < 225 || after.Item2.G < 225 || after.Item2.B < 225)
                throw new InvalidOperationException(
                    $"API 发现 {name} 按钮悬停背景/文字不变：{before} -> {after}");
        }
        finally { SetPseudo(button, ":pointerover", false); }
    }

    private static void CheckPanel(Border frame, Control child, string name)
    {
        if (frame.CornerRadius.TopLeft < 10 || frame.CornerRadius.TopRight < 10 ||
            frame.CornerRadius.BottomLeft < 10 || frame.CornerRadius.BottomRight < 10 ||
            frame.BorderThickness.Left < 1 || !frame.ClipToBounds ||
            frame.Child != child || child.Bounds.Width < 400)
            throw new InvalidOperationException($"API 发现 {name} 没有真实圆角及裁剪。");
    }

    private static void CheckLogStates(TextBox box)
    {
        foreach (var state in new[] { "normal", ":pointerover", ":focus", ":focus-within" })
        {
            if (state != "normal") SetPseudo(box, state, true);
            try
            {
                if (box.CornerRadius.TopLeft < 9 || !box.ClipToBounds ||
                    box.BorderThickness.Left != 0 ||
                    ColorOf(box.Background).R < 200)
                    throw new InvalidOperationException($"API 发现日志 {state} 丢失浅色背景或圆角。");
            }
            finally { if (state != "normal") SetPseudo(box, state, false); }
        }
    }

    private static Color ColorOf(IBrush? brush) =>
        brush is ISolidColorBrush solid ? solid.Color : default;

    private static void SetPseudo(Control control, string state, bool on)
    {
        var property = typeof(StyledElement).GetProperty("PseudoClasses",
            BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("无法访问 Avalonia 状态。");
        var pseudo = property.GetValue(control)!;
        var iface = pseudo.GetType().GetInterfaces().First(i => i.Name == "IPseudoClasses");
        iface.GetMethod(on ? "Add" : "Remove")!.Invoke(pseudo, [state]);
    }

    private static void Save(Window window, string output)
    {
        window.UpdateLayout();
        using var image = new RenderTargetBitmap(
            new PixelSize((int)Math.Ceiling(window.Bounds.Width * 2),
                           (int)Math.Ceiling(window.Bounds.Height * 2)),
            new Vector(192, 192));
        image.Render(window);
        image.Save(output);
    }
}
#endif
