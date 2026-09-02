# 诊断：检查目标进程所有顶层窗口的可见性/位置/标题，判断 UI 是否真实渲染
param(
    [int]$ProcId = 0,
    [switch]$Launch = $false,
    [string]$Exe = "C:\X\NewLife.XCoder\Bin\CrazyCoder\CrazyCoder.exe"
)

Add-Type @"
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
public class EnumWin2 {
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll")] public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
    [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll")] public static extern int GetWindowText(IntPtr hWnd, StringBuilder s, int n);
    [DllImport("user32.dll")] public static extern int GetClassName(IntPtr hWnd, StringBuilder s, int n);
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left, Top, Right, Bottom; }

    static uint _pid;
    static List<string> _rows = new List<string>();
    static bool Callback(IntPtr hWnd, IntPtr lParam) {
        uint pid; GetWindowThreadProcessId(hWnd, out pid);
        if (pid == _pid) {
            var vis = IsWindowVisible(hWnd);
            RECT r; GetWindowRect(hWnd, out r);
            var t = new StringBuilder(512); GetWindowText(hWnd, t, 512);
            var c = new StringBuilder(256); GetClassName(hWnd, c, 256);
            _rows.Add(String.Format("hwnd=0x{0:X} visible={1} rect=({2},{3})-({4},{5}) class='{6}' title='{7}'",
                hWnd.ToInt64(), vis, r.Left, r.Top, r.Right, r.Bottom, c, t));
        }
        return true;
    }
    public static List<string> ListWindows(uint pid) {
        _pid = pid; _rows = new List<string>();
        EnumWindows(Callback, IntPtr.Zero);
        return _rows;
    }
}
"@

$procId = $ProcId
if ($Launch) {
    Get-Process CrazyCoder -ErrorAction SilentlyContinue | Stop-Process -Force
    Start-Sleep -Milliseconds 600
    $p = Start-Process -FilePath $Exe -PassThru
    $procId = $p.Id
    Write-Host "[diag] 已启动 PID=$procId，等待 12s 初始化"
    Start-Sleep -Seconds 12
}

Write-Host "[diag] 枚举 PID=$procId 的顶层窗口:"
$rows = [EnumWin2]::ListWindows([uint32]$procId)
if ($rows.Count -eq 0) { Write-Host "  (无顶层窗口)" }
foreach ($row in $rows) { Write-Host "  $row" }

Write-Host "[diag] done"
