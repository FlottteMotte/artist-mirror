# artist-mirror

Free Windows app that mirrors an iPad screen over the same Wi-Fi - for art progress in Discord / OBS.

**Status:** private / early development.

## For artists (simple)

1. Unzip `artist-mirror-portable.zip`
2. Double-click **Start.vbs** (or `ArtistMirror.exe`)
3. Tap **Start**
4. iPad (same Wi-Fi): Screen Mirroring -> **artist-mirror**
5. Share that window in Discord / OBS

No PowerShell. No MSYS2. No terminal. Allow firewall if Windows asks.

## What it is / is not

- Windows mirror receiver only
- Not a Twitch login app, not an iPad drawing tablet driver

## License

GNU GPLv3 - see [LICENSE](LICENSE). Notices: [NOTICE](NOTICE).

## Maintainers

```powershell
.\receiver\build.ps1
.\packaging\package.ps1
```

From the repo root you can also use `Start.vbs` after packaging.

Output: `dist\artist-mirror\`  
Docs: [receiver/README.md](receiver/README.md), [docs/plans/...](docs/plans/2026-09-05-001-feat-ipad-windows-mirror-plan.md)
