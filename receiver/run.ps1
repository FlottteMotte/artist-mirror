# Run the local UxPlay build as AirPlay name "artist-mirror".
param(
    [string]$MsysRoot = "C:\msys64",
    [string]$Name = "artist-mirror"
)

$ErrorActionPreference = "Stop"
$exe = Join-Path $PSScriptRoot "uxplay\build\uxplay.exe"
if (-not (Test-Path $exe)) {
    throw "Missing $exe - run .\receiver\build.ps1 first."
}

$ucrtBin = Join-Path $MsysRoot "ucrt64\bin"
if (-not (Test-Path $ucrtBin)) {
    throw "UCRT64 bin not found: $ucrtBin"
}

$env:PATH = "$ucrtBin;" + $env:PATH
Write-Host "Starting $Name - Screen Mirror from iPad on the same Wi-Fi. Ctrl+C to stop."
& $exe -n $Name @args
