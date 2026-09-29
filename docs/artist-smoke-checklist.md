# Artist smoke checklist

## U2 — Receive stack (dev machine)

- [x] MSYS2 installed (`C:\msys64`)
- [x] UCRT64 deps installed (cmake, gcc, gstreamer plugins, libplist)
- [x] `.\receiver\build.ps1` produces `receiver\uxplay\build\uxplay.exe`
- [x] `.\receiver\run.ps1` / `uxplay.exe` starts without immediate crash (dev smoke)
- [ ] iPad Screen Mirroring lists **artist-mirror** (same Wi‑Fi)
- [ ] Video appears in the UxPlay window
- [ ] Stop mirroring on iPad → receiver returns to waiting (no reboot)

## F1 — First successful mirror (app / packaging)

- [ ] Install/open app on Windows (no MSYS2 on cold machine once packaged)
- [ ] App shows Ready
- [ ] iPad Screen Mirroring lists this receiver (same Wi‑Fi)
- [ ] Mirror appears in shareable window
- [ ] Discord can pick the window and shows iPad content
- [ ] OBS Window Capture can target the same window

## F2 — Reconnect after drop

- [ ] Stop mirroring on iPad → clear disconnected/error or Ready state
- [ ] Restart mirroring → session resumes without terminal recovery

## Notes

- Date / Windows version / iPadOS version:
- Issues:
- Build note: UxPlay built with internal mDNS (`-DNO_MARCH_NATIVE=ON`); child-process integration for shell (U3).
