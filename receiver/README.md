# Receiver (UxPlay lineage)

Windows AirPlay mirror receive stack for artist-mirror.

**Integration mode (v1):** managed **child process** — the future artist shell starts/stops `uxplay.exe`. We do not reimplement AirPlay.

**Upstream:** [FDH2/UxPlay](https://github.com/FDH2/UxPlay) (GPLv3). UxPlay ≥ 1.74 uses a built-in minimal mDNSResponder, so Apple Bonjour SDK is **not** required for the default build.

## Prerequisites

1. [MSYS2](https://www.msys2.org/) installed (default: `C:\msys64`)
2. UCRT64 packages (installed once):

```bash
# in MSYS2 UCRT64 or via bash -lc
pacman -S --noconfirm \
  mingw-w64-ucrt-x86_64-cmake \
  mingw-w64-ucrt-x86_64-gcc \
  mingw-w64-ucrt-x86_64-ninja \
  mingw-w64-ucrt-x86_64-pkgconf \
  mingw-w64-ucrt-x86_64-libplist \
  mingw-w64-ucrt-x86_64-gstreamer \
  mingw-w64-ucrt-x86_64-gst-plugins-base \
  mingw-w64-ucrt-x86_64-gst-plugins-good \
  mingw-w64-ucrt-x86_64-gst-plugins-bad \
  mingw-w64-ucrt-x86_64-gst-libav
```

## Build

From the repo root in PowerShell:

```powershell
.\receiver\build.ps1
```

This clones UxPlay into `receiver/uxplay` (if missing) and builds `receiver/uxplay/build/uxplay.exe`.

## Run (smoke)

Same Wi‑Fi as the iPad. Allow Windows Firewall when prompted.

```powershell
.\receiver\run.ps1
```

On the iPad: Control Center → Screen Mirroring → pick **artist-mirror**.

If the iPad does not see the receiver, add an inbound firewall allow for `uxplay.exe` (Private network).

## Layout

| Path | Role |
|------|------|
| `receiver/uxplay/` | Upstream sources (gitignored; fetched by `build.ps1`) |
| `receiver/uxplay/build/uxplay.exe` | Built binary (gitignored `*.exe`) |
| `receiver/build.ps1` | Clone + cmake/ninja |
| `receiver/run.ps1` | Launch with UCRT64 PATH + display name |

## Notes

- Rebuild after `git pull` inside `receiver/uxplay` or delete `build/` and re-run `build.ps1`.
- Cold-machine packaging (no MSYS2) is **U5**, not this unit.
