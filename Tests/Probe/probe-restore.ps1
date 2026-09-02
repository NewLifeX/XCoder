# 实证：恢复最小化窗口并验证 UIA 子树出现
param(
    [string]$Exe = "C:\X\NewLife.XCoder\Bin\CrazyCoder\CrazyCoder.exe"
)
$ErrorActionPreference = "Stop"
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Restore {
    [DllImport("user32.dll")] public static extern bool IsIconic(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left, Top, Right, Bottom; }
}
"@

Get-Process CrazyCoder -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Milliseconds 600
$p = Start-Process -FilePath $Exe -PassThru
Write-Host "[t] PID=$($p.Id) 等待主窗口..."
Start-Sleep -Seconds 15

$root = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition(
    [System.Windows.Automation.AutomationElement]::ProcessIdProperty, $p.Id)
$main = $null
foreach ($w in $root.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)) {
    if ($w.Current.Name -like "*码神工具*") { $main = $w; break }
}
if (-not $main) { Write-Host "[t] 未找到主窗口"; exit 2 }
$hwnd = [IntPtr]$main.Current.NativeWindowHandle
Write-Host ("[t] hwnd=0x{0:X} iconic={1}" -f $hwnd.ToInt64(), [Win32Restore]::IsIconic($hwnd))

[Win32Restore]::ShowWindow($hwnd, 9) | Out-Null   # SW_RESTORE
[Win32Restore]::SetForegroundWindow($hwnd) | Out-Null
Start-Sleep -Milliseconds 1500
$r = New-Object Win32Restore+RECT
[Win32Restore]::GetWindowRect($hwnd, [ref]$r) | Out-Null
Write-Host ("[t] restore 后 rect=({0},{1})-({2},{3}) iconic={4}" -f $r.Left, $r.Top, $r.Right, $r.Bottom, [Win32Restore]::IsIconic($hwnd))

# 重新获取 main 并统计后代
$main = $null
foreach ($w in $root.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)) {
    if ($w.Current.Name -like "*码神工具*") { $main = $w; break }
}
if ($main) {
    $all = $main.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
    Write-Host "[t] UIA 后代元素: $($all.Count)"
    $ctr = [System.Windows.Automation.ControlType]
    foreach ($e in $all) {
        if ($e.Current.ControlType -eq $ctr::MenuBar -or $e.Current.ControlType -eq $ctr::Menu -or $e.Current.ControlType -eq $ctr::MenuItem) {
            Write-Host ("  [{0}] '{1}'" -f $e.Current.ControlType.ProgrammaticName, $e.Current.Name)
        }
    }
}
Write-Host "[t] done. PID=$($p.Id) 保留运行"
