# 码神工具 E2E 探测脚本：启动 CrazyCoder.exe 并 dump 菜单树（验证 UIA 可达性）
param(
    [string]$Exe = "C:\X\NewLife.XCoder\Bin\CrazyCoder\CrazyCoder.exe",
    [int]$WaitSec = 25,
    [switch]$KillFirst
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

if ($KillFirst) {
    Get-Process CrazyCoder -ErrorAction SilentlyContinue | Stop-Process -Force
    Start-Sleep -Milliseconds 500
}

# 启动 app
$p = Start-Process -FilePath $Exe -PassThru
Write-Host "[probe] PID=$($p.Id) 等待主窗口..."

$root = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition(
    [System.Windows.Automation.AutomationElement]::ProcessIdProperty, $p.Id)
$main = $null
$deadline = (Get-Date).AddSeconds($WaitSec)
while ((Get-Date) -lt $deadline) {
    Start-Sleep -Milliseconds 500
    if ($p.HasExited) { Write-Host "[probe] 进程已退出 code=$($p.ExitCode)"; exit 1 }
    $wins = $root.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
    foreach ($w in $wins) {
        $name = $w.Current.Name
        if ($name -like "*码神工具*" -or $name -like "*NewLife*") { $main = $w; break }
    }
    if ($main) { break }
}
if (-not $main) { Write-Host "[probe] 未找到主窗口（25s 内）"; Get-Process CrazyCoder -ErrorAction SilentlyContinue | Stop-Process -Force; exit 2 }

Write-Host "[probe] 主窗口: '$($main.Current.Name)' Class='$($main.Current.ClassName)'"

# 等菜单动态加载
Start-Sleep -Seconds 3

function Dump-Menu([System.Windows.Automation.AutomationElement]$el, [int]$depth) {
    $indent = "  " * $depth
    $ct = $el.Current.ControlType.ProgrammaticName
    $name = $el.Current.Name
    $ai = $el.TryGetCurrentPattern([System.Windows.Automation.SelectionItemPattern]::Pattern, [ref]$null)
    Write-Host "$indent[$ct] '$name'"
    $children = $el.FindAll([System.Windows.Automation.TreeScope]::Children, [System.Windows.Automation.Condition]::TrueCondition)
    foreach ($c in $children) {
        if ($c.Current.ControlType -eq [System.Windows.Automation.ControlType]::MenuBar -or
            $c.Current.ControlType -eq [System.Windows.Automation.ControlType]::MenuItem -or
            $c.Current.ControlType -eq [System.Windows.Automation.ControlType]::Menu) {
            Dump-Menu $c ($depth + 1)
        }
    }
}

# 从主窗口找 MenuBar
$menuBar = $main.FindFirst([System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
        [System.Windows.Automation.ControlType]::MenuBar)))
if (-not $menuBar) { Write-Host "[probe] 未找到 MenuBar"; Get-Process CrazyCoder -ErrorAction SilentlyContinue | Stop-Process -Force; exit 3 }

Write-Host "[probe] ===== 菜单树 ====="
Dump-Menu $menuBar 0

Write-Host "[probe] done. 进程保留运行 PID=$($p.Id)"
