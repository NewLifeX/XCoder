<#
.SYNOPSIS
    把原生库打包为 zip，供以嵌入资源形式随程序集发布。

.DESCRIPTION
    原生 DLL 多为未压缩 PE 文件，zip 压缩率通常可达 3~8 倍。
    打包后由 ZipResourceExtractor 在运行时解压还原，可显著减小程序集体积。

    打包产物命名需与代码约定一致：
      · 单文件：{目录}.{文件名去扩展名}.zip，zip 内仅含该文件
               例：CAN\ZLG_CAN\zlgcan.dll -> CAN\ZLG_CAN\zlgcan.zip
      · 目录  ：{目录名}.zip，zip 内含目录内全部文件（保留相对结构）
               例：CAN\ZLG_CAN\kerneldlls -> CAN\ZLG_CAN\kerneldlls.zip

.PARAMETER Target
    可选，指定要打包的目标（相对本脚本目录）。缺省打包全部已配置项。

.EXAMPLE
    .\pack_native_res.ps1

.EXAMPLE
    .\pack_native_res.ps1 -Target 'CAN\ZLG_CAN\kerneldlls'
#>
[CmdletBinding()]
param(
    [string]$Target
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$root = $PSScriptRoot

# 待打包项：与 A1Lib.csproj 中的嵌入配置一一对应
$items = @(
    @{ Path = 'CAN\ZLG_CAN\zlgcan.dll';  Kind = 'file' }
    @{ Path = 'CAN\ZLG_CAN\kerneldlls';  Kind = 'dir'  }
    @{ Path = 'CAN\CX_CAN\ControlCAN.dll'; Kind = 'file' }
    @{ Path = 'CAN\PCAN\PCANBasic.dll';  Kind = 'file' }
)

function Get-ZipPath([string]$relPath, [string]$kind) {
    $full = Join-Path $root $relPath
    if ($kind -eq 'dir') {
        return "$full.zip"
    }
    # 单文件：同名但扩展名换成 .zip
    $dir  = Split-Path $full -Parent
    $base = [IO.Path]::GetFileNameWithoutExtension($full)
    return (Join-Path $dir "$base.zip")
}

function Write-Zip([string]$srcPath, [string]$kind, [string]$zipPath) {
    if (Test-Path $zipPath) { Remove-Item $zipPath -Force }

    if ($kind -eq 'dir') {
        # 目录：以目录内容为根，保留相对结构（解压后直接铺在目标目录下）
        Compress-Archive -Path (Join-Path $srcPath '*') -DestinationPath $zipPath -CompressionLevel Optimal
    }
    else {
        # 单文件：以文件名作为 zip 内的条目名
        Compress-Archive -Path $srcPath -DestinationPath $zipPath -CompressionLevel Optimal
    }
    return $zipPath
}

function Format-Size([long]$n) {
    if ($n -ge 1MB) { return ('{0:N2} MB' -f ($n / 1MB)) }
    if ($n -ge 1KB) { return ('{0:N1} KB' -f ($n / 1KB)) }
    return "$n B"
}

$results = @()

foreach ($it in $items) {
    $rel = $it.Path
    $kind = $it.Kind

    if ($Target -and ($rel -ne $Target)) { continue }

    $src = Join-Path $root $rel
    if (-not (Test-Path $src)) {
        Write-Warning "跳过（不存在）: $rel"
        continue
    }

    # 源大小
    if ($kind -eq 'dir') {
        $rawSize = (Get-ChildItem $src -Recurse -File | Measure-Object -Property Length -Sum).Sum
        $fileCount = (Get-ChildItem $src -Recurse -File).Count
    }
    else {
        $rawSize = (Get-Item $src).Length
        $fileCount = 1
    }

    $zipPath = Get-ZipPath $rel $kind
    Write-Zip $src $kind $zipPath | Out-Null

    $zipSize = (Get-Item $zipPath).Length
    $ratio = if ($rawSize -gt 0) { $zipSize / $rawSize } else { 0 }

    $results += [pscustomobject]@{
        目标     = $rel
        类型     = $kind
        文件数   = $fileCount
        原始     = Format-Size $rawSize
        压缩后   = Format-Size $zipSize
        压缩率   = ('{0:P0}' -f $ratio)
        Zip      = [IO.Path]::GetFileName($zipPath)
        RawBytes = [long]$rawSize
        ZipBytes = [long]$zipSize
    }
}

if ($results.Count -eq 0) {
    Write-Host '没有可打包的目标。' -ForegroundColor Yellow
    return
}

$results | Select-Object 目标, 类型, 文件数, 原始, 压缩后, 压缩率 | Format-Table -AutoSize

$sumRaw = ($results | Measure-Object -Property RawBytes -Sum).Sum
$sumZip = ($results | Measure-Object -Property ZipBytes -Sum).Sum
$sumRatio = if ($sumRaw -gt 0) { $sumZip / $sumRaw } else { 0 }

Write-Host ''
Write-Host ('合计：{0} -> {1}（压缩率 {2:P0}）' -f (Format-Size $sumRaw), (Format-Size $sumZip), $sumRatio) -ForegroundColor Green

Write-Host ''
Write-Host '说明：zip 与原始文件同时存在于源码目录时，A1Lib.csproj 只嵌入 zip。'
Write-Host '      原始文件保留供打包与排查使用，不参与编译。'
