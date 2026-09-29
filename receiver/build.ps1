# Build UxPlay into receiver/uxplay/build/uxplay.exe (MSYS2 UCRT64).
param(
    [string]$MsysRoot = "C:\msys64",
    [string]$UxPlayRepo = "https://github.com/FDH2/UxPlay.git"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$uxplayDir = Join-Path $PSScriptRoot "uxplay"
$bash = Join-Path $MsysRoot "usr\bin\bash.exe"

if (-not (Test-Path $bash)) {
    throw "MSYS2 bash not found at $bash. Install MSYS2 or pass -MsysRoot."
}

if (-not (Test-Path (Join-Path $uxplayDir ".git"))) {
    Write-Host "Cloning UxPlay into receiver/uxplay ..."
    git clone --depth 1 $UxPlayRepo $uxplayDir
}

$uxplayUnix = ($uxplayDir -replace '\\', '/') -replace '^([A-Za-z]):', '/$1'
$uxplayUnix = $uxplayUnix.Substring(0, 2).ToLower() + $uxplayUnix.Substring(2)

$script = @"
export PATH=/ucrt64/bin:/usr/bin:`$PATH
cd '$uxplayUnix'
mkdir -p build
cd build
cmake -G Ninja -DNO_MARCH_NATIVE=ON ..
ninja
"@

& $bash -lc $script
if ($LASTEXITCODE -ne 0) { throw "UxPlay build failed (exit $LASTEXITCODE)" }

$exe = Join-Path $uxplayDir "build\uxplay.exe"
if (-not (Test-Path $exe)) { throw "Expected binary missing: $exe" }
Write-Host "OK: $exe"
