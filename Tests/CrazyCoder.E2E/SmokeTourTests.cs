using System.Text;
using Xunit.Abstractions;

namespace CrazyCoder.E2E;

/// <summary>全菜单冒烟巡检：从真实 UI 枚举全部工具菜单，逐个点击打开并验证窗体可加载</summary>
[Collection("CrazyCoder")]
public class SmokeTourTests
{
    readonly CrazyCoderSession _s;
    readonly ITestOutputHelper _log;

    public SmokeTourTests(CrazyCoderSession session, ITestOutputHelper log)
    {
        _s = session;
        _log = log;
    }

    sealed record ToolResult(ToolDef Tool, String Status, String Message, String Screenshot);

    [Fact]
    public void 全菜单冒烟巡检()
    {
        _s.Start();
        var tools = _s.DiscoverTools();
        Assert.True(tools.Count > 0, "未能从 UI 发现任何工具菜单");
        _log.WriteLine($"发现工具 {tools.Count} 个");

        var results = new List<ToolResult>();

        foreach (var tool in tools)
        {
            _log.WriteLine($"---- 巡检: {tool.Category}/{tool.Name}");
            results.Add(TourOne(tool));
            // 每个工具后清理现场
            try { _s.CloseAllChildren(); } catch { }
            try { _s.CloseDialogs(); } catch { }
        }

        var ok = results.Count(x => x.Status == "通过");
        var warn = results.Count(x => x.Status.StartsWith("通过("));
        var fail = results.Count(x => !x.Status.StartsWith("通过"));

        // 生成巡检报告
        var report = BuildReport(results, tools.Count, ok, warn, fail);
        var reportFile = Path.Combine(_s.OutDir, "巡检报告.md");
        File.WriteAllText(reportFile, report, new UTF8Encoding(false));
        _log.WriteLine($"报告已生成: {reportFile}");

        var sb = new StringBuilder();
        sb.AppendLine($"巡检结果：通过 {ok}，警告 {warn}，失败/异常 {fail}");
        foreach (var r in results.Where(x => !x.Status.StartsWith("通过")))
            sb.AppendLine($"  [{r.Status}] {r.Tool.Category}/{r.Tool.Name}：{r.Message}");

        _log.WriteLine(sb.ToString());
        Assert.True(fail == 0, "存在巡检失败项，详见巡检报告：\n" + sb);
    }

    ToolResult TourOne(ToolDef tool)
    {
        var shot = "";
        try
        {
            // 打开前先清理
            _s.CloseAllChildren();
            _s.CloseDialogs();
            _s.Start(); // 若已崩溃则重启

            var child = _s.OpenTool(tool);
            Thread.Sleep(1200); // 让窗体完成加载

            if (!_s.IsAlive)
                return new(tool, "崩溃", "打开后进程退出", shot);

            if (child == null)
            {
                shot = _s.Capture(_s.MainHandle, $"{(tool.Category)}-{tool.Name}-无窗体");
                // 检查是否弹了模态框
                var dlg = _s.TopWindows().Where(w => w.cls == "#32770").ToList();
                if (dlg.Count > 0)
                {
                    foreach (var d in dlg)
                        _s.Capture(d.hwnd, $"{(tool.Category)}-{tool.Name}-弹窗");
                    _s.CloseDialogs();
                    return new(tool, "异常弹窗", "打开后弹出对话框而非正常窗体，已自动关闭", shot);
                }
                return new(tool, "无窗体", "点击菜单后 10 秒内未出现子窗体", shot);
            }

            // 检测弹窗
            var dialogs = _s.TopWindows().Where(w => w.cls == "#32770").ToList();
            if (dialogs.Count > 0)
            {
                var dlgInfo = new StringBuilder();
                foreach (var d in dialogs)
                {
                    var f = _s.Capture(d.hwnd, $"{(tool.Category)}-{tool.Name}-弹窗");
                    dlgInfo.Append($"「{d.title}」");
                }
                _s.CloseDialogs();
                shot = _s.Capture(child, $"{tool.Category}-{tool.Name}");
                return new(tool, "通过(有弹窗)", $"窗体已打开但弹出对话框 {dlgInfo}", shot);
            }

            // 扫描窗体内容中的异常堆栈
            var content = _s.GetTexts(child);
            var marker = FindExceptionMarker(content);
            if (marker != null)
            {
                shot = _s.Capture(child, $"{tool.Category}-{tool.Name}");
                return new(tool, "通过(内容异常)", $"窗体内容出现异常「{marker}」：{Snippet(content, 120)}", shot);
            }

            shot = _s.Capture(child, $"{tool.Category}-{tool.Name}");
            return new(tool, "通过", $"子窗体「{child.Name}」正常加载", shot);
        }
        catch (Exception ex)
        {
            if (String.IsNullOrEmpty(shot)) shot = _s.Capture(_s.MainHandle, $"{tool.Category}-{tool.Name}-异常");
            return new(tool, "异常", ex.Message, shot);
        }
    }

    static String Snippet(String text, Int32 len)
    {
        text = text.Trim();
        if (text.Length <= len) return text.Replace("\r", "").Replace("\n", " ⏎ ");
        return text[..len].Replace("\r", "").Replace("\n", " ⏎ ") + "…";
    }

    /// <summary>在文本中查找异常标记</summary>
    static String? FindExceptionMarker(String? text)
    {
        if (String.IsNullOrEmpty(text)) return null;
        String[] markers =
        [
            "ReflectionTypeLoadException", "TypeLoadException", "FileNotFoundException",
            "NullReferenceException", "未将对象引用设置到对象的实例",
            "InvalidOperationException", "ArgumentException", "NotSupportedException",
            "未能加载文件", "未能加载程序集", "at System.Reflection", "at NewLife.Reflection",
        ];
        return markers.FirstOrDefault(m => text.Contains(m));
    }

    static String BuildReport(List<ToolResult> results, Int32 total, Int32 ok, Int32 warn, Int32 fail)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# 码神工具 E2E 冒烟巡检报告");
        sb.AppendLine();
        sb.AppendLine($"- 巡检时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"- 发现工具：{total} 个");
        sb.AppendLine($"- 通过：{ok} ｜ 弹窗警告：{warn} ｜ 失败/异常：{fail}");
        sb.AppendLine();
        sb.AppendLine("## 明细");
        sb.AppendLine();
        sb.AppendLine("| 分类 | 工具 | 结果 | 说明 | 截图 |");
        sb.AppendLine("|------|------|------|------|------|");
        foreach (var r in results.OrderBy(x => x.Tool.Category).ThenBy(x => x.Tool.Name))
        {
            var img = String.IsNullOrEmpty(r.Screenshot) ? "-" : $"[图]({Path.GetFileName(r.Screenshot)})";
            sb.AppendLine($"| {r.Tool.Category} | {r.Tool.Name} | {r.Status} | {r.Message} | {img} |");
        }
        sb.AppendLine();
        sb.AppendLine("## 问题清单");
        sb.AppendLine();
        var warns = results.Where(x => x.Status.StartsWith("通过(")).ToList();
        var issues = results.Where(x => !x.Status.StartsWith("通过")).ToList();
        if (warns.Count == 0 && issues.Count == 0)
        {
            sb.AppendLine("本次巡检未发现问题 ✅");
        }
        else
        {
            var idx = 1;
            if (warns.Count > 0)
            {
                sb.AppendLine("### 警告（可运行但有疑点）");
                sb.AppendLine();
                foreach (var r in warns)
                {
                    sb.AppendLine($"{idx}. **[{r.Status}] {r.Tool.Category}/{r.Tool.Name}**：{r.Message}  {(String.IsNullOrEmpty(r.Screenshot) ? "" : $"（截图 {Path.GetFileName(r.Screenshot)}）")}");
                    idx++;
                }
            }
            if (issues.Count > 0)
            {
                sb.AppendLine("### 失败/异常");
                sb.AppendLine();
                foreach (var r in issues)
                {
                    sb.AppendLine($"{idx}. **[{r.Status}] {r.Tool.Category}/{r.Tool.Name}**：{r.Message}  {(String.IsNullOrEmpty(r.Screenshot) ? "" : $"（截图 {Path.GetFileName(r.Screenshot)}）")}");
                    idx++;
                }
            }
        }
        return sb.ToString();
    }
}
