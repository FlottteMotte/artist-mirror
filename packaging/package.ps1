# Build a portable folder for artists: double-click ArtistMirror.exe, no PowerShell / MSYS2.
param(
    [string]$MsysRoot = "C:\msys64",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$repo = Split-Path -Parent $PSScriptRoot
$dist = Join-Path $repo "dist\artist-mirror"
$engine = Join-Path $dist "engine"
$uxplaySrc = Join-Path $repo "receiver\uxplay\build\uxplay.exe"
$ucrtBin = Join-Path $MsysRoot "ucrt64\bin"
$gstPluginsSrc = Join-Path $MsysRoot "ucrt64\lib\gstreamer-1.0"
$ntldd = Join-Path $ucrtBin "ntldd.exe"

# Plugins UxPlay needs for AirPlay mirror/audio on Windows (not the whole GStreamer zoo).
$pluginNames = @(
    "libgstapp.dll",
    "libgstaudioconvert.dll",
    "libgstaudiomixer.dll",
    "libgstaudioparsers.dll",
    "libgstaudioresample.dll",
    "libgstautodetect.dll",
    "libgstcoreelements.dll",
    "libgstd3d.dll",
    "libgstd3d11.dll",
    "libgstd3d12.dll",
    "libgstdirectsound.dll",
    "libgstlibav.dll",
    "libgstlevel.dll",
    "libgstmediafoundation.dll",
    "libgstopenh264.dll",
    "libgstplayback.dll",
    "libgsttypefindfunctions.dll",
    "libgstvideobox.dll",
    "libgstvideoconvertscale.dll",
    "libgstvideofilter.dll",
    "libgstvideofiltersbad.dll",
    "libgstvideoparsersbad.dll",
    "libgstvideorate.dll",
    "libgstvolume.dll",
    "libgstwasapi.dll",
    "libgstwasapi2.dll"
)

Write-Host "== artist-mirror portable package =="

if (-not (Test-Path $uxplaySrc)) {
    Write-Host "Building UxPlay first..."
    & (Join-Path $repo "receiver\build.ps1") -MsysRoot $MsysRoot
}

if (-not (Test-Path $uxplaySrc)) { throw "Missing $uxplaySrc" }
if (-not (Test-Path $gstPluginsSrc)) { throw "Missing GStreamer plugins at $gstPluginsSrc" }

if (Test-Path $dist) {
    Write-Host "Stopping any running artist-mirror / uxplay processes..."
    Get-Process -Name "ArtistMirror","uxplay" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    Start-Sleep -Milliseconds 500
    Remove-Item $dist -Recurse -Force -ErrorAction SilentlyContinue
    if (Test-Path $dist) {
        $bak = "$dist.old-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
        Rename-Item $dist $bak -Force
        Write-Host "Old dist was locked; moved aside to $bak"
    }
}
New-Item -ItemType Directory -Force -Path $engine | Out-Null

Write-Host "Publishing self-contained UI..."
dotnet publish (Join-Path $repo "app\ArtistMirror\ArtistMirror.csproj") `
    -c $Configuration `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=false `
    -o $dist
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }

Write-Host "Copying uxplay.exe..."
Copy-Item $uxplaySrc (Join-Path $engine "uxplay.exe") -Force

Write-Host "Copying all UCRT64 runtime DLLs into engine/ (plugin deps)..."
$binDlls = Get-ChildItem $ucrtBin -Filter "*.dll"
foreach ($dll in $binDlls) {
    Copy-Item $dll.FullName $engine -Force
}
Write-Host ("Copied {0} runtime DLLs" -f $binDlls.Count)

Write-Host "Copying essential GStreamer plugins..."
$gstOut = Join-Path $engine "gstreamer-1.0"
New-Item -ItemType Directory -Force -Path $gstOut | Out-Null
$copiedPlugins = 0
foreach ($name in $pluginNames) {
    $src = Join-Path $gstPluginsSrc $name
    if (Test-Path $src) {
        Copy-Item $src $gstOut -Force
        $copiedPlugins++

        # Pull any extra deps for this plugin into engine/
        if (Test-Path $ntldd) {
            $env:PATH = "$ucrtBin;" + $env:PATH
            $out = & $ntldd -R $src 2>&1 | Out-String
            foreach ($line in ($out -split "`r?`n")) {
                if ($line -match '=>\s+(.+\.dll)\s+\(') {
                    $p = $Matches[1].Trim()
                    if ((Test-Path $p) -and ($p -like "*\ucrt64\bin\*")) {
                        Copy-Item $p $engine -Force -ErrorAction SilentlyContinue
                    }
                }
            }
        }
    }
    else {
        Write-Host "  skip missing plugin: $name"
    }
}
Write-Host ("Copied {0} plugins" -f $copiedPlugins)

$gio = Join-Path $MsysRoot "ucrt64\lib\gio\modules"
if (Test-Path $gio) {
    $gioOut = Join-Path $engine "gio\modules"
    New-Item -ItemType Directory -Force -Path $gioOut | Out-Null
    Copy-Item (Join-Path $gio "*") $gioOut -Force -ErrorAction SilentlyContinue
}

$startVbs = @"
' Silent launcher - no console window.
Option Explicit
Dim sh, fso, root, exe
Set sh = CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")
root = fso.GetParentFolderName(WScript.ScriptFullName)
exe = root & "\ArtistMirror.exe"
If fso.FileExists(exe) Then
  sh.Run """" & exe & """", 1, False
Else
  MsgBox "ArtistMirror.exe missing in this folder.", vbExclamation, "artist-mirror"
End If
"@
Set-Content -Path (Join-Path $dist "Start.vbs") -Value $startVbs -Encoding ASCII

$readme = @"
artist-mirror - portable

1. Double-click Start.vbs (or ArtistMirror.exe)
2. Tap Start
3. iPad (same Wi-Fi): Screen Mirroring -> artist-mirror
4. Share that window in Discord / OBS

Allow the firewall prompt if Windows asks.
No PowerShell. No MSYS2 required. No terminal window.
"@
Set-Content -Path (Join-Path $dist "README.txt") -Value $readme -Encoding UTF8

Write-Host ""
Write-Host "OK: $dist"
Write-Host "Zip this folder and send it to artists."
