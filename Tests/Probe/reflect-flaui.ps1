$ErrorActionPreference = "Stop"
$core = "C:\Users\Stone\.nuget\packages\flaui.core\5.0.0\lib\net8.0-windows7.0\FlaUI.Core.dll"
[System.Reflection.Assembly]::LoadFrom($core) | Out-Null
$asm = [System.Reflection.Assembly]::LoadFrom($core)

Write-Host "== 含 Text 的类型 =="
$asm.GetTypes() | Where-Object { $_.Name -match 'Text' -and $_.IsInterface } | ForEach-Object { Write-Host ("  {0}" -f $_.FullName) }

Write-Host "== IAutomationPattern 成员 =="
$ia = $asm.GetType("FlaUI.Core.IAutomationPattern``1")
if ($ia) { $ia.GetProperties() | ForEach-Object { Write-Host ("  {0} : {1}" -f $_.Name, $_.PropertyType.Name) } }

Write-Host "== Patterns.Value 包装类型成员 =="
$el = [FlaUI.Core.AutomationElements.AutomationElement]
$patternsType = $el.GetProperty("Patterns").PropertyType
$valProp = $patternsType.GetProperty("Value")
Write-Host ("  Value 属性类型: {0}" -f $valProp.PropertyType.FullName)
$valProp.PropertyType.GetProperties() | ForEach-Object { Write-Host ("  P {0} : {1}" -f $_.Name, $_.PropertyType.Name) }
$valProp.PropertyType.GetMethods() | Where-Object { $_.Name -match 'Get|Try' } | ForEach-Object { Write-Host ("  M {0}" -f $_.Name) }

Write-Host "== IValuePattern =="
$vp = $asm.GetType("FlaUI.Core.Patterns.IValuePattern")
if ($vp) { $vp.GetProperties() | ForEach-Object { Write-Host ("  {0} : {1}" -f $_.Name, $_.PropertyType.Name) } }

Write-Host "== TextPattern 相关（UIA3） =="
$u3 = "C:\Users\Stone\.nuget\packages\flaui.uia3\5.0.0\lib\net8.0-windows7.0\FlaUI.UIA3.dll"
[System.Reflection.Assembly]::LoadFrom($u3) | Out-Null
$u3asm = [System.Reflection.Assembly]::LoadFrom($u3)
$u3asm.GetTypes() | Where-Object { $_.Name -match 'Text' } | ForEach-Object { Write-Host ("  {0}" -f $_.FullName) }
