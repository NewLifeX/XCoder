using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using Xunit.Abstractions;

namespace CrazyCoder.E2E;

/// <summary>E2E 探测：验证 UIA 结构假设（非正式测试）</summary>
[Collection("CrazyCoder")]
public class ProbeTests
{
    readonly CrazyCoderSession _s;
    readonly ITestOutputHelper _log;

    public ProbeTests(CrazyCoderSession session, ITestOutputHelper log)
    {
        _s = session;
        _log = log;
    }

    [Fact]
    public void 探测菜单与子窗体结构()
    {
        _s.Start();
        _log.WriteLine($"Exe: {_s.ExePath}");
        _log.WriteLine($"主窗口: '{_s.MainWindow?.Name}' hwnd=0x{_s.MainHandle.ToInt64():X}");

        var bar = _s.FindMenuBar();
        Assert.NotNull(bar);
        _log.WriteLine("===== 菜单结构 =====");
        var roots = bar!.FindAllChildren(cf => cf.ByControlType(ControlType.MenuItem));
        foreach (var root in roots)
        {
            _log.WriteLine($"[根] '{root.Name}'");
            try
            {
                var mi = root.AsMenuItem();
                mi.Expand();
                Thread.Sleep(300);
                var items = root.FindAllDescendants(cf => cf.ByControlType(ControlType.MenuItem));
                foreach (var it in items)
                    _log.WriteLine($"    - '{it.Name}'");
                try { mi.Collapse(); } catch { }
            }
            catch (Exception ex)
            {
                _log.WriteLine($"    展开失败: {ex.Message}");
            }
        }

        // 打开 安全工具 -> 第一个工具，验证子窗体检测
        _log.WriteLine("===== 打开第一个 安全工具 =====");
        var tools = _s.DiscoverTools();
        var t = tools.FirstOrDefault(x => x.Category == "安全工具");
        Assert.NotNull(t);
        _log.WriteLine($"打开: {t!.Category}/{t.Name}");

        var child = _s.OpenTool(t);
        Thread.Sleep(3000);
        _log.WriteLine($"OpenTool 返回子窗体: {(child == null ? "null" : $"'{child.Name}' class='{Cls(child)}'")}");
        _log.WriteLine($"主窗口标题: '{_s.MainWindow?.Name}'");
        _log.WriteLine($"ChildForms(): {_s.ChildForms().Length} 个");

        // dump 主窗口下所有 Window/Pane 后代
        _log.WriteLine("===== 主窗口 Window/Pane 后代 =====");
        foreach (var w in _s.ChildForms())
            _log.WriteLine($"  [{w.ControlType}] name='{w.Name}' class='{Cls(w)}' offscreen={w.IsOffscreen}");

        _log.WriteLine("===== 主窗口所有后代前 50 =====");
        var all = _s.MainWindow!.FindAllDescendants();
        for (var i = 0; i < Math.Min(50, all.Length); i++)
        {
            var el = all[i];
            var cls = "";
            try { cls = el.ClassName ?? ""; } catch { }
            _log.WriteLine($"  [{el.ControlType}] name='{el.Name}' class='{cls}'");
        }

        _s.CloseAllChildren();
    }

    [Fact]
    public void 探测对称加密控件结构()
    {
        _s.Start();
        var tools = _s.DiscoverTools();
        var t = tools.FirstOrDefault(x => x.Name == "对称加密");
        Assert.NotNull(t);
        var child = _s.OpenTool(t!);
        Assert.NotNull(child);
        Thread.Sleep(1500);

        _log.WriteLine($"子窗体: '{child!.Name}'");
        var cts = new[]
        {
            ControlType.Button, ControlType.Edit, ControlType.RadioButton,
            ControlType.CheckBox, ControlType.ComboBox, ControlType.Text,
        };
        foreach (var ct in cts)
        {
            _log.WriteLine($"===== {ct} =====");
            var els = child.FindAllDescendants(cf => cf.ByControlType(ct));
            foreach (var el in els.Take(80))
            {
                String? aid = null;
                try { aid = el.Properties.AutomationId.ValueOrDefault; } catch { }
                String? name = null;
                try { name = el.Name; } catch { }
                _log.WriteLine($"  aid='{aid}' name='{name}' cls='{Cls(el)}'");
            }
        }
        _s.CloseAllChildren();
    }

    /// <summary>安全读取 ClassName（部分控件不支持该属性）</summary>
    static String Cls(FlaUI.Core.AutomationElements.AutomationElement el)
    {
        try { return el.ClassName ?? ""; } catch { return ""; }
    }
}
