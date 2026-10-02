# artist-mirror

Free Windows app that mirrors an iPad screen over the same Wi‑Fi into a clean, shareable window

## What it is

- Windows-only receiver (v1)
- Same home Wi‑Fi / local network
- Stock iPadOS Screen Mirroring (AirPlay-style) — no paid iPad companion
- Will be open source (GPLv3) once it works

## What it is not

- Not a Twitch/YouTube built-in streamer
- Not an iPad-as-drawing-tablet driver for PC apps
- Not Mac/Linux (yet)

## Quick intent

1. Download the zip (`artist-mirror-portable.zip` or the `artist-mirror` folder).
2. Unzip it somewhere easy, e.g. Desktop.
3. Open the folder.
4. Double-click **Start.vbs**  
   (or double-click **ArtistMirror.exe** if you prefer)
5. In the app, tap **Start**.
6. Windows may warn that it protected your PC or ask whether to run the app. That is normal for unsigned portable apps.
Click **More info**
Click **Run anyway**
If Windows Firewall asks to allow network access, click **Allow**. Without that, the iPad cannot find artist-mirror on Wi-Fi.

## License

GNU GPLv3 - see [LICENSE](LICENSE). Notices: [NOTICE](NOTICE).

## Maintainers

```powershell
.\receiver\build.ps1
.\packaging\package.ps1
```
