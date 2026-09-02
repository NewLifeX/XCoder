using System.Diagnostics;
using System.Text;
using System.Xml;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using Application = FlaUI.Core.Application;

namespace CrazyCoder.E2E;

/// <summary>工具菜单定义：分类 + 菜单名</summary>
public sealed record ToolDef(String Category, String Name);

/// <summary>码神工具 E2E 会话：负责启动 CrazyCoder.exe、恢复窗口、通过真实菜单点击操作工具窗体</summary>
public sealed class CrazyCoderSession : IDisposable
{
    /// <summary>exe 路径</summary>
    public String ExePath { get; }

    /// <summary>仓库根目录</summary>
    public String RepoRoot { get; } = "";

    /// <summary>输出目录（巡检报告与截图）</summary>
    public String OutDir { get; }

    /// <summary>截图目录</summary>
    public String ScreenshotDir => Path.Combine(OutDir, "screenshots");

    public UIA3Automation Automation { get; } = new();
    public Application? App { get; private set; }
    public Window? MainWindow { get; private set; }
    public IntPtr MainHandle => App?.MainWindowHandle ?? IntPtr.Zero;

    /// <summary>进程是否存活</summary>
    public Boolean IsAlive
    {
        get
        {
            if (App == null) return false;
            try
            {
                using var p = Process.GetProcessById(App.ProcessId);
                return !p.HasExited;
            }
            catch { return false; }
        }
    }

    static readonly String[] _systemRoots = ["窗口", "帮助"];

    /// <summary>是否系统菜单根（窗口/帮助等，非工具分类）</summary>
    static Boolean IsSystemRoot(String name)
    {
        foreach (var r in _systemRoots)
        {
            if (name.Contains(r)) return true;
        }
        return false;
    }

    public CrazyCoderSession()
    {
        // 从测试输出目录向上找到仓库根（含 Bin\CrazyCoder\CrazyCoder.exe）
        var dir = AppContext.BaseDirectory;
        while (dir != null)
        {
            var exe = Path.Combine(dir, "Bin", "CrazyCoder", "CrazyCoder.exe");
            if (File.Exists(exe))
            {
                RepoRoot = dir;
                ExePath = exe;
                break;
            }
            dir = Path.GetDirectoryName(dir);
        }
        if (ExePath == null) throw new InvalidOperationException("找不到 Bin\\CrazyCoder\\CrazyCoder.exe，请先构建主项目");

        OutDir = Path.Combine(AppContext.BaseDirectory, "out");
        Directory.CreateDirectory(ScreenshotDir);
    }

    /// <summary>启动 app 并等待主窗口与菜单就绪（可重入：已运行则复用，进程已死则重启）</summary>
    public void Start()
    {
        if (IsAlive)
        {
            MainWindow = App!.GetMainWindow(Automation);
            return;
        }
        if (App != null)
        {
            try { App.Kill(); } catch { }
            try { App.Dispose(); } catch { }
            App = null;
            MainWindow = null;
        }

        PrepareConfig();
        App = Application.Launch(ExePath);

        if (!App.WaitWhileMainHandleIsMissing(TimeSpan.FromSeconds(40)))
            throw new InvalidOperationException("主窗口句柄 40 秒内未出现，启动失败！");

        // 激活窗口：ShowWindow(Restore) 后 UIA 子树才会完整暴露
        Win32.ForceForeground(App.MainWindowHandle);
        Thread.Sleep(800);
        MainWindow = App.GetMainWindow(Automation);

        // 等待菜单动态加载（反射 IXForm 填充菜单是异步的）
        WaitUntil(() => FindMenuBar() != null, TimeSpan.FromSeconds(15), "菜单栏 15 秒内未就绪");
        Thread.Sleep(1000);
        MainWindow = App.GetMainWindow(Automation);
    }

    /// <summary>把测试运行所需的隔离配置写入 exe 同目录（语音关闭、不自动打开上次工具、更新检查置今天）</summary>
    void PrepareConfig()
    {
        var cfgFile = Path.Combine(Path.GetDirectoryName(ExePath)!, "Config", "XCoder.config");
        if (!File.Exists(cfgFile)) return;
        var xml = new XmlDocument();
        xml.Load(cfgFile);
        void Set(String key, String val)
        {
            var node = xml.SelectSingleNode($"/XConfig/{key}");
            if (node == null)
            {
                node = xml.CreateElement(key);
                xml.DocumentElement!.AppendChild(node);
            }
            node.InnerText = val;
        }
        Set("SpeechTip", "false");
        Set("LastTool", "");
        Set("LastUpdate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        xml.Save(cfgFile);
    }

    /// <summary>查找主窗体的菜单栏</summary>
    public AutomationElement? FindMenuBar()
        => MainWindow?.FindFirstDescendant(cf => cf.ByControlType(ControlType.MenuBar));

    /// <summary>从真实 UI 枚举全部工具菜单（分类 + 工具名）</summary>
    public List<ToolDef> DiscoverTools()
    {
        var list = new List<ToolDef>();
        var bar = FindMenuBar() ?? throw new InvalidOperationException("菜单栏不存在");
        Win32.ForceForeground(MainHandle);
        var roots = bar.FindAllChildren(cf => cf.ByControlType(ControlType.MenuItem));
        foreach (var root in roots)
        {
            var cat = root.Name;
            if (IsSystemRoot(cat)) continue;

            var menu = root.AsMenuItem();
            try { menu.Expand(); }
            catch { continue; }
            Thread.Sleep(300);

            var items = root.FindAllDescendants(cf => cf.ByControlType(ControlType.MenuItem));
            foreach (var item in items)
            {
                var name = item.Name;
                if (String.IsNullOrEmpty(name) || name == cat) continue;
                list.Add(new ToolDef(cat, name));
            }
            try { menu.Collapse(); } catch { }
        }
        return list;
    }

    /// <summary>点击菜单打开指定工具，返回新出现的子窗体元素（无则 null）</summary>
    public AutomationElement? OpenTool(ToolDef tool)
    {
        // 先关闭可能残留的子窗体
        CloseAllChildren();

        var bar = FindMenuBar();
        if (bar == null) return null;

        Win32.ForceForeground(MainHandle);
        var roots = bar.FindAllChildren(cf => cf.ByControlType(ControlType.MenuItem));
        var root = roots.FirstOrDefault(x => x.Name == tool.Category);
        if (root == null) return null;

        var menu = root.AsMenuItem();
        try { menu.Expand(); } catch { return null; }
        Thread.Sleep(300);

        var items = root.FindAllDescendants(cf => cf.ByControlType(ControlType.MenuItem));
        var item = items.FirstOrDefault(x => x.Name == tool.Name);
        if (item == null) return null;

        var before = CurrentChildNames();
        item.Click();
        Thread.Sleep(800);

        // 等待出现新子窗体
        AutomationElement? child = null;
        var deadline = DateTime.Now.AddSeconds(10);
        while (DateTime.Now < deadline)
        {
            child = ChildForms().FirstOrDefault(x => !before.Contains(x.Name));
            if (child != null) break;
            // 兜底：主窗体标题带 "- [工具名]"
            var title = MainWindow?.Name;
            if (!String.IsNullOrEmpty(title) && title.Contains(" - [" + tool.Name + "]")) break;
            Thread.Sleep(300);
        }
        return child;
    }

    /// <summary>判断元素是否 MDI 子窗体</summary>
    static Boolean IsChildForm(AutomationElement el)
    {
        if (el.ControlType != ControlType.Window && el.ControlType != ControlType.Pane) return false;
        if (String.IsNullOrEmpty(el.Name)) return false;
        var cls = el.ClassName ?? "";
        return cls.Contains("WindowsForms10.Window");
    }

    /// <summary>当前主窗体下所有 MDI 子窗体元素</summary>
    public AutomationElement[] ChildForms()
    {
        if (MainWindow == null) return [];
        var wins = MainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Window));
        var panes = MainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Pane));
        return wins.Concat(panes).Where(IsChildForm).ToArray();
    }

    /// <summary>当前已打开子窗体名称集合</summary>
    HashSet<String> CurrentChildNames()
    {
        var set = new HashSet<String>();
        foreach (var c in ChildForms()) set.Add(c.Name);
        return set;
    }

    /// <summary>关闭全部子窗体</summary>
    public void CloseAllChildren()
    {
        var children = ChildForms();
        foreach (var c in children)
        {
            var hwnd = c.Properties.NativeWindowHandle.ValueOrDefault;
            if (hwnd != IntPtr.Zero) Win32.CloseWindow(hwnd);
        }
        if (children.Length > 0)
        {
            // 等待窗体真正关闭
            var deadline = DateTime.Now.AddSeconds(5);
            while (DateTime.Now < deadline && ChildForms().Length > 0)
                Thread.Sleep(200);
        }
    }

    /// <summary>枚举进程顶层窗口（不含主窗口）</summary>
    public List<(IntPtr hwnd, String title, String cls)> TopWindows()
    {
        if (App == null) return [];
        return Win32.EnumTopWindows(App.ProcessId).Where(w => w.hwnd != MainHandle).ToList();
    }

    /// <summary>关闭进程顶层对话框（标准 MessageBox #32770）</summary>
    public void CloseDialogs()
    {
        foreach (var w in TopWindows())
        {
            if (w.cls == "#32770") Win32.CloseWindow(w.hwnd);
        }
    }

    /// <summary>收集元素内全部可见文本（Value/Text/Document 模式），用于内容异常检测</summary>
    public String GetTexts(AutomationElement root)
    {
        var sb = new StringBuilder();
        try
        {
            var count = 0;
            foreach (var el in root.FindAllDescendants())
            {
                if (count++ > 400 || sb.Length > 40000) break;
                try
                {
                    if (el.Patterns.Value.IsSupported)
                    {
                        var v = el.Patterns.Value.PatternOrDefault?.Value?.ValueOrDefault;
                        if (!String.IsNullOrEmpty(v)) sb.AppendLine(v);
                    }
                    else if (el.Patterns.Text.IsSupported)
                    {
                        var range = el.Patterns.Text.PatternOrDefault?.DocumentRange;
                        var v = range?.GetText(20000);
                        if (!String.IsNullOrEmpty(v)) sb.AppendLine(v);
                    }
                    else if (el.ControlType == ControlType.Text && !String.IsNullOrEmpty(el.Name))
                        sb.AppendLine(el.Name);
                }
                catch { }
            }
        }
        catch { }
        return sb.ToString();
    }

    /// <summary>等待条件成立</summary>
    public static Boolean WaitUntil(Func<Boolean> condition, TimeSpan timeout, String? msg = null)
    {
        var deadline = DateTime.Now + timeout;
        while (DateTime.Now < deadline)
        {
            if (condition()) return true;
            Thread.Sleep(200);
        }
        if (msg != null) Console.WriteLine($"[wait] 超时：{msg}");
        return false;
    }

    /// <summary>对窗口句柄截图</summary>
    public String Capture(IntPtr hwnd, String name)
    {
        var file = Path.Combine(ScreenshotDir, Sanitize(name) + ".png");
        if (hwnd == IntPtr.Zero) return "";
        Win32.CaptureWindow(hwnd, file);
        return File.Exists(file) ? file : "";
    }

    /// <summary>对元素截图</summary>
    public String Capture(AutomationElement el, String name)
        => Capture(el.Properties.NativeWindowHandle.ValueOrDefault, name);

    static String Sanitize(String name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }

    public void Dispose()
    {
        try { App?.Close(); } catch { }
        try { App?.Kill(); } catch { }
        try { Automation.Dispose(); } catch { }
        try { App?.Dispose(); } catch { }
    }
}
