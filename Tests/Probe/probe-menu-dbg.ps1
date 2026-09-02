# 诊断：dump 主窗口所有后代元素的 ControlType + Name（前 120 个），定位菜单暴露形态
param(
    [string]$Exe = "C:\X\NewLife.XCoder\Bin\CrazyCoder\CrazyCoder.exe",
    [int]$WaitSec = 30,
    [switch]$KillFirst
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

if ($KillFirst) {
    Get-Process CrazyCoder -ErrorAction SilentlyContinue | Stop-Process -Force
    Start-Sleep -Milliseconds 600
}

$p = Start-Process -FilePath $Exe -PassThru
Write-Host "[dbg] PID=$($p.Id) 等待主窗口..."

$root = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition(
    [System.Windows.Automation.AutomationElement]::ProcessIdProperty, $p.Id)
$main = $null
$deadline = (Get-Date).AddSeconds($WaitSec)
while ((Get-Date) -lt $deadline) {
    Start-Sleep -Milliseconds 500
    if ($p.HasExited) { Write-Host "[dbg] 进程已退出 code=$($p.ExitCode)"; exit 1 }
    $wins = $root.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
    foreach ($w in $wins) {
        if ($w.Current.Name -like "*码神工具*") { $main = $w; break }
    }
    if ($main) { break }
}
if (-not $main) { Write-Host "[dbg] 未找到主窗口"; Get-Process CrazyCoder -ErrorAction SilentlyContinue | Stop-Process -Force; exit 2 }
Write-Host "[dbg] 主窗口: '$($main.Current.Name)' Class='$($main.Current.ClassName)'"
Write-Host "[dbg] 等待菜单动态加载 8s..."
Start-Sleep -Seconds 8

$all = $main.FindAll([System.Windows.Automation.TreeScope]::Descendants,
    [System.Windows.Automation.Condition]::TrueCondition)
Write-Host "[dbg] 后代元素总数: $($all.Count)"

# 统计 ControlType 直方图
$hist = @{}
foreach ($e in $all) {
    $ct = $e.Current.ControlType.ProgrammaticName
    $hist[$ct] = 1 + ($hist[$ct] ?? 0)
}
Write-Host "[dbg] === ControlType 直方图 ==="
$hist.GetEnumerator() | Sort-Object Value -Descending | ForEach-Object {
    Write-Host ("  {0,-40} {1}" -f $_.Key, $_.Value)
}

# 打印 Menu/MenuBar/MenuItem 及含'工具'的项
Write-Host "[dbg] === 菜单相关元素 ==="
$i = 0
foreach ($e in $all) {
    $ct = $e.Current.ControlType
    if ($ct -eq [System.Windows.Automation.ControlType]::MenuBar -or
        $ct -eq [System.Windows.Automation.ControlType]::Menu -or
        $ct -eq [System.Windows.Automation.ControlType]::MenuItem) {
        Write-Host ("  [{0}] {1} '{2}'" -f $ct.ProgrammaticName, $e.Current.ClassName, $e.Current.Name)
        $i++
        if ($i -ge 80) { break }
    }
}
if ($i -eq 0) {
    Write-Host "[dbg] 未发现任何菜单元素！打印前 60 个后代:"
    $j = 0
    foreach ($e in $all) {
        $nm = $e.Current.Name
        if ($nm) {
            Write-Host ("  [{0}] '{1}'" -f $e.Current.ControlType.ProgrammaticName, $nm)
            $j++
            if ($j -ge 60) { break }
        }
    }
}
Write-Host "[dbg] done. 进程保留 PID=$($p.Id)"
