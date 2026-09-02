using System.Runtime.InteropServices;

namespace CrazyCoder.E2E;

/// <summary>Win32 辅助：窗口激活、关闭、截屏等</summary>
public static class Win32
{
    const Int32 SW_RESTORE = 9;
    const Int32 WM_CLOSE = 0x0010;

    [DllImport("user32.dll")] static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);
    [DllImport("user32.dll")] static extern Boolean SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")] static extern Boolean BringWindowToTop(IntPtr hWnd);
    [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out UInt32 processId);
    [DllImport("user32.dll")] static extern Boolean AttachThreadInput(UInt32 idAttach, UInt32 idAttachTo, Boolean fAttach);
    [DllImport("kernel32.dll")] static extern UInt32 GetCurrentThreadId();
    [DllImport("user32.dll")] static extern Boolean PostMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")] static extern Boolean IsWindow(IntPtr hWnd);
    [DllImport("user32.dll")] static extern Boolean GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll")] static extern Boolean PrintWindow(IntPtr hWnd, IntPtr hdcBlt, UInt32 nFlags);
    [DllImport("user32.dll")] static extern Int32 GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, Int32 count);
    [DllImport("user32.dll")] static extern Int32 GetClassName(IntPtr hWnd, System.Text.StringBuilder text, Int32 count);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, String lParam);
    [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

    const UInt32 EM_SETSEL = 0x00B1;
    const UInt32 EM_REPLACESEL = 0x00C2;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public Int32 Left, Top, Right, Bottom; }

    /// <summary>把窗口恢复到前台（含线程输入附加技巧，绕过前台锁）</summary>
    public static void ForceForeground(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero || !IsWindow(hWnd)) return;
        if (GetForegroundWindow() == hWnd) return;

        UInt32 fgThread = 0;
        GetWindowThreadProcessId(GetForegroundWindow(), out fgThread);
        var cur = GetCurrentThreadId();
        var attach = fgThread != 0 && fgThread != cur;
        if (attach) AttachThreadInput(cur, fgThread, true);

        try
        {
            ShowWindow(hWnd, SW_RESTORE);
            BringWindowToTop(hWnd);
            SetForegroundWindow(hWnd);
        }
        finally
        {
            if (attach) AttachThreadInput(cur, fgThread, false);
        }
    }

    /// <summary>关闭窗口（发送 WM_CLOSE）</summary>
    public static Boolean CloseWindow(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero || !IsWindow(hWnd)) return false;
        return PostMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
    }

    /// <summary>用 EM_SETSEL + EM_REPLACESEL 替换 RichTextBox 全部内容（UIA 对 RichEdit 不支持写值）</summary>
    public static void ReplaceText(IntPtr hWnd, String text)
    {
        if (hWnd == IntPtr.Zero || !IsWindow(hWnd)) return;
        // 全选：wParam=起点0，lParam=终点-1
        SendMessage(hWnd, EM_SETSEL, IntPtr.Zero, (IntPtr)(-1));
        // 替换：wParam=是否可撤销(false)，lParam=文本
        SendMessage(hWnd, EM_REPLACESEL, IntPtr.Zero, text);
    }

    delegate Boolean EnumProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")] static extern Boolean EnumWindows(EnumProc lpEnumFunc, IntPtr lParam);

    /// <summary>枚举指定进程的全部顶层窗口</summary>
    public static List<(IntPtr hwnd, String title, String cls)> EnumTopWindows(Int32 pid)
    {
        var list = new List<(IntPtr, String, String)>();
        EnumWindows((h, l) =>
        {
            UInt32 p;
            GetWindowThreadProcessId(h, out p);
            if (p == (UInt32)pid)
            {
                var t = new System.Text.StringBuilder(512);
                GetWindowText(h, t, 512);
                var c = new System.Text.StringBuilder(256);
                GetClassName(h, c, 256);
                list.Add((h, t.ToString(), c.ToString()));
            }
            return true;
        }, IntPtr.Zero);
        return list;
    }

    /// <summary>获取窗口矩形（屏幕坐标）</summary>
    public static RECT? GetRect(IntPtr hWnd)
    {
        RECT r;
        if (!GetWindowRect(hWnd, out r)) return null;
        return r;
    }

    /// <summary>PrintWindow 截屏窗口内容到文件</summary>
    public static Boolean CaptureWindow(IntPtr hWnd, String file)
    {
        var rect = GetRect(hWnd);
        if (rect == null) return false;
        var r = rect.Value;
        var w = r.Right - r.Left;
        var h = r.Bottom - r.Top;
        if (w <= 0 || h <= 0) return false;

        using var bmp = new Bitmap(w, h);
        using (var g = Graphics.FromImage(bmp))
        {
            var hdc = g.GetHdc();
            try
            {
                // PW_RENDERFULLCONTENT=2：捕获 DirectX/较新渲染的完整内容
                if (!PrintWindow(hWnd, hdc, 2))
                    PrintWindow(hWnd, hdc, 0);
            }
            finally
            {
                g.ReleaseHdc(hdc);
            }
        }
        var dir = Path.GetDirectoryName(file);
        if (dir != null) Directory.CreateDirectory(dir);
        bmp.Save(file);
        return true;
    }
}
