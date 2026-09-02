using FlaUI.Core.AutomationElements;

namespace CrazyCoder.E2E;

/// <summary>UI 操作辅助：按 AutomationId 定位控件并读写（兼容 TextBox 与 RichTextBox）</summary>
public sealed class UiOps
{
    readonly CrazyCoderSession _s;

    public UiOps(CrazyCoderSession session) => _s = session;

    /// <summary>在窗体中按 AutomationId 查找控件</summary>
    public AutomationElement? Find(AutomationElement root, String automationId)
        => root.FindFirstDescendant(cf => cf.ByAutomationId(automationId));

    /// <summary>读取控件文本（Value 模式优先，富文本框走 Text 模式）</summary>
    public String? Read(AutomationElement root, String automationId)
    {
        var el = Find(root, automationId);
        return el == null ? null : ReadElement(el);
    }

    public static String? ReadElement(AutomationElement el)
    {
        try
        {
            if (el.Patterns.Value.IsSupported)
                return el.Patterns.Value.Pattern.Value.ValueOrDefault;
        }
        catch { }
        try
        {
            var range = el.Patterns.Text.PatternOrDefault?.DocumentRange;
            if (range != null) return range.GetText(200000);
        }
        catch { }
        return el.Name;
    }

    /// <summary>写入控件文本（TextBox 用 Value 模式，RichTextBox 用 EM 消息）</summary>
    public void SetText(AutomationElement root, String automationId, String value)
    {
        var el = Find(root, automationId)
            ?? throw new InvalidOperationException($"找不到控件 {automationId}");
        try
        {
            if (el.Patterns.Value.IsSupported)
            {
                el.Patterns.Value.Pattern.SetValue(value);
                return;
            }
        }
        catch { }
        // RichTextBox 等不支持 Value 模式：用 EM 消息全选替换
        var hwnd = el.Properties.NativeWindowHandle.ValueOrDefault;
        if (hwnd == IntPtr.Zero) throw new InvalidOperationException($"控件 {automationId} 无窗口句柄");
        Win32.ReplaceText(hwnd, value);
    }

    /// <summary>点击按钮（Invoke 优先）</summary>
    public void Click(AutomationElement root, String automationId)
    {
        var el = Find(root, automationId)
            ?? throw new InvalidOperationException($"找不到按钮 {automationId}");
        Win32.ForceForeground(_s.MainHandle);
        try
        {
            el.AsButton()?.Invoke();
            return;
        }
        catch { }
        el.Click();
    }

    /// <summary>选择单选按钮（模拟点击）</summary>
    public void SelectRadio(AutomationElement root, String automationId)
    {
        var el = Find(root, automationId)
            ?? throw new InvalidOperationException($"找不到单选按钮 {automationId}");
        Win32.ForceForeground(_s.MainHandle);
        el.Click();
        Thread.Sleep(120);
    }

    /// <summary>设置复选框状态（Toggle 模式，无需鼠标）</summary>
    public void SetCheck(AutomationElement root, String automationId, Boolean on)
    {
        var el = Find(root, automationId)
            ?? throw new InvalidOperationException($"找不到复选框 {automationId}");
        var cb = el.AsCheckBox();
        if (cb.IsChecked == on) return;
        try
        {
            cb.IsChecked = on;
        }
        catch
        {
            cb.Toggle();
        }
        Thread.Sleep(120);
    }
}
