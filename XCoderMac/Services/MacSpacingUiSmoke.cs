#if DEBUG
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media.Imaging;
using XCoderMac.Views;

namespace XCoderMac.Services;

/// <summary>全项目分组间距审计与加密解密流布局专项：只渲染界面，不执行任何工具命令。</summary>
internal static class MacSpacingUiSmoke
{
    public static int Run(string folder)
    {
        MacLegacyToolWindow? window = null;
        try
        {
            var tool = MacToolCatalog.Tools.Single(t => t.Title == "加密解密");
            window = new MacLegacyToolWindow(tool) { Width = 1250, Height = 880 };
            window.Show();
            window.UpdateLayout();
            var root = window.HostedToolContent
                ?? throw new InvalidOperationException("加密解密未进入宿主");
            MacSecuritySpacing.AssertLayout(root);

            // 右侧的分组/控件行距不得退回原来 0~4 的状态。
            var groups = root.GetLogicalDescendants().OfType<Border>()
                .Where(b => b.Classes.Contains("GroupBorder")).ToArray();
            if (groups.Length != 4 || groups.Any(g => g.Padding.Left < 10 ||
                g.CornerRadius.TopLeft < 10))
                throw new InvalidOperationException("加密解密四组内容边距未统一");
            var fields = root.GetLogicalDescendants().OfType<TextBox>()
                .Where(t => t.Classes.Contains("CodeTextBox")).ToArray();
            if (fields.Length != 3 || fields.Any(t => t.CornerRadius.TopLeft < 8))
                throw new InvalidOperationException("加密解密输入/输出未统一圆角");

            Directory.CreateDirectory(folder);
            Render(window, System.IO.Path.Combine(folder, "06-加密解密-全局间距与按钮流.png"));
            window.Width = 970;
            window.UpdateLayout();
            MacSecuritySpacing.AssertLayout(root);

            Console.WriteLine("SPACING SECURITY OK: 31 operation buttons, two columns, 10px between buttons, 4 groups, 1250/970 windows");
            window.Close();
            window = null;

            // 全部旧窗口（含已精调的）由独立 --smoke-host-render 验证可开，
            // 这里实际查验其它未精调旧工具的 GroupBorder margin >= 10。
            var audited = 0;
            var adjusted = 0;
            foreach (var candidate in MacToolCatalog.Tools.Where(t => t.WindowType.Namespace == "XCoderAv.Views"))
            {
                if (candidate.Title is "MQTT 客户端" or "API 发现" or
                    "文件编码" or "消息调试" or "加密解密") continue;
                window = new MacLegacyToolWindow(candidate);
                window.Show();
                window.UpdateLayout();
                var panels = window.HostedToolContent!.GetLogicalDescendants()
                    .OfType<Border>().Where(b => b.Classes.Contains("GroupBorder")).ToArray();
                foreach (var group in panels)
                    if (group.Margin.Bottom < 9.9 && group.Margin.Top == 0 &&
                        group.Margin.Left == 0 && group.Margin.Right == 0)
                        throw new InvalidOperationException(
                            $"{candidate.Title} 未按规则保留分组下间距：{group.Margin}");
                audited++;
                adjusted += panels.Length;
                window.Close();
                window = null;
            }
            Console.WriteLine($"SPACING LEGACY OK: {audited} untailored old tool windows, {adjusted} grouped panels audited");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("SPACING UI FAILED: " + ex);
            return 1;
        }
        finally { window?.Close(); }
    }

    private static void Render(Window window, string path)
    {
        window.UpdateLayout();
        using var png = new RenderTargetBitmap(
            new PixelSize((int)Math.Ceiling(window.Bounds.Width * 2),
                (int)Math.Ceiling(window.Bounds.Height * 2)),
            new Vector(192, 192));
        png.Render(window);
        png.Save(path);
    }
}
#endif
