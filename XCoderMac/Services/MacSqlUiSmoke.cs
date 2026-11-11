#if DEBUG
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using XCoderMac.Views;

namespace XCoderMac.Services;

/// <summary>只检查这次更改的导航、SQL 预览和 SQL 查询器外观；不访问任何数据库。</summary>
internal static class MacSqlUiSmoke
{
    public static async Task<int> RunAsync(MacShellWindow shell, string output)
    {
        Directory.CreateDirectory(output);
        MacSqlPreviewWindow? preview = null;
        MacSqlWorkbenchWindow? workbench = null;
        MacNetworkWindow? network = null;
        try
        {
            if (MacToolCatalog.Categories.Length != 4 ||
                MacToolCatalog.Categories.Any(c => c.Name == "数据工具") ||
                MacToolCatalog.Tools.First(t => t.Title == "数据建模").Category != "开发与数据" ||
                shell.GetVisualDescendants().OfType<Button>()
                    .Any(b => b.Name is "DataToolsNavButton" or "HomeDataToolsButton"))
                throw new InvalidOperationException("左侧独立数据工具入口尚未删除");
            Console.WriteLine("SQL UI NAV OK: original 4 categories, no extra sidebar/home entry");

            const string sample = """
                CREATE TABLE DemoTable (
                    Id INT PRIMARY KEY,
                    UserName VARCHAR(100)
                );

                CREATE INDEX IX_DemoTable_UserName ON DemoTable(UserName);
                """;
            preview = new MacSqlPreviewWindow("建表 SQL 预览 · 测试", sample);
            preview.Show(shell);
            await Task.Delay(210);
            preview.UpdateLayout();
            var text = preview.GetVisualDescendants().OfType<TextBox>()
                .SingleOrDefault(t => t.Name == "SqlPreviewText")
                ?? throw new InvalidOperationException("SQL 预览控件缺失");
            if (!text.IsReadOnly || text.Text != sample)
                throw new InvalidOperationException("SQL 预览不是只读或预览内容丢失");
            CheckDarkStates(text, "SQL 预览");
            Render(preview, Path.Combine(output, "01-建表SQL预览-固定深色.png"));
            Console.WriteLine("SQL PREVIEW OK: black background, white text, normal/hover/focus/focus-within");
            preview.Close();
            preview = null;

            workbench = new MacSqlWorkbenchWindow("测试连接-未访问数据库");
            workbench.Show(shell);
            await Task.Delay(180);
            workbench.UpdateLayout();
            var tabs = workbench.GetVisualDescendants().OfType<TabControl>()
                .Single(t => t.Name == "SqlWorkbenchTabs");
            var items = tabs.Items.Cast<TabItem>().ToArray();
            if (items.Length != 2 || items.Any(t => t.FontSize > 14.1 || t.FontSize < 12 ||
                t.Padding.Top > 10 || t.Padding.Bottom > 10))
                throw new InvalidOperationException(
                    $"SQL 查询器 TAB 字号/高度不正确: {string.Join(",", items.Select(i => $"{i.FontSize}/{i.Padding}"))}");
            Render(workbench, Path.Combine(output, "01-架构管理与SQL查询器-小字号.png"));
            tabs.SelectedIndex = 1;
            await Task.Delay(130);
            workbench.UpdateLayout();
            var result = workbench.GetVisualDescendants().OfType<TextBox>()
                .SingleOrDefault(t => t.Name == "SqlQueryResultText")
                ?? throw new InvalidOperationException("SQL 查询结果控件缺失");
            result.Text = "演示查询结果：Id=1, UserName=Test";
            CheckDarkStates(result, "SQL 查询结果");
            Render(workbench, Path.Combine(output, "01-SQL查询器-固定深色.png"));
            Console.WriteLine("SQL WORKBENCH OK: 2 tabs font=14, result black in hover/focus");
            workbench.Close();
            workbench = null;

            // 同一全局问题还影响网络调试日志。确认所有 Mac 原生深色文本框均得到修复。
            network = new MacNetworkWindow();
            network.Show(shell);
            await Task.Delay(110);
            network.UpdateLayout();
            var networkLog = network.GetVisualDescendants().OfType<TextBox>()
                .Single(t => t.Name == "txtLog");
            CheckDarkStates(networkLog, "网络通信日志");
            Console.WriteLine("NETWORK CONSOLE OK: hover/focus never turns black log white");
            network.Close();
            network = null;
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("SQL UI SMOKE FAILED: " + ex);
            return 1;
        }
        finally
        {
            preview?.Close();
            workbench?.Close();
            network?.Close();
        }
    }

    private static void CheckDarkStates(TextBox box, string name)
    {
        var pseudoProperty = typeof(Avalonia.StyledElement).GetProperty(
            "PseudoClasses", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("无法检查 Avalonia 的交互状态");
        var pseudo = pseudoProperty.GetValue(box)!;
        var face = pseudo.GetType().GetInterfaces().First(i => i.Name == "IPseudoClasses");
        var add = face.GetMethod("Add")!;
        var remove = face.GetMethod("Remove")!;
        void Check(string state)
        {
            var bg = box.Background as ISolidColorBrush;
            var fg = box.Foreground as ISolidColorBrush;
            var borders = box.GetVisualDescendants().OfType<Border>()
                .Where(b => b.Name == "PART_BorderElement").ToArray();
            if (!box.Classes.Contains("dark-console") ||
                bg == null || bg.Color.R >= 32 || bg.Color.G >= 32 || bg.Color.B >= 45 ||
                fg == null || fg.Color.R <= 190 || fg.Color.G <= 190 || fg.Color.B <= 190 ||
                borders.Any(b => b.Background is not ISolidColorBrush color ||
                    color.Color.R >= 32 || color.Color.G >= 32 || color.Color.B >= 45))
                throw new InvalidOperationException(
                    $"{name} 的 {state} 状态不再是黑底浅字：{box.Background}/{box.Foreground}");
        }
        Check("正常");
        foreach (var state in new[] { ":pointerover", ":focus", ":focus-within" })
        {
            add.Invoke(pseudo, [state]);
            try { Check(state); }
            finally { remove.Invoke(pseudo, [state]); }
        }
    }

    private static void Render(Window window, string output)
    {
        window.UpdateLayout();
        using var image = new RenderTargetBitmap(
            new PixelSize((int)Math.Ceiling(window.Bounds.Width * 2),
                          (int)Math.Ceiling(window.Bounds.Height * 2)),
            new Vector(192, 192));
        image.Render(window);
        image.Save(output);
        Console.WriteLine("SQL UI SCREENSHOT: " + output);
    }
}
#endif
