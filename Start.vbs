' Silent launcher - no console window.
Option Explicit
Dim sh, fso, root, exe
Set sh = CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")
root = fso.GetParentFolderName(WScript.ScriptFullName)

exe = root & "\dist\artist-mirror\ArtistMirror.exe"
If Not fso.FileExists(exe) Then exe = root & "\dist\ArtistMirror.exe"
If Not fso.FileExists(exe) Then exe = root & "\app\ArtistMirror\bin\Release\net8.0-windows\win-x64\ArtistMirror.exe"
If Not fso.FileExists(exe) Then exe = root & "\app\ArtistMirror\bin\Release\net8.0-windows\ArtistMirror.exe"

If fso.FileExists(exe) Then
  sh.Run """" & exe & """", 1, False
Else
  MsgBox "artist-mirror is not built yet." & vbCrLf & "Maintainers: run packaging\package.ps1", vbExclamation, "artist-mirror"
End If
