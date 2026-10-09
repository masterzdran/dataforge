# DataForger demo video — https://vhs.charm.sh
#
# Render with:  vhs demo.tape          (produces demo.mp4 next to where you run it)
# Requires:     vhs + ffmpeg on PATH
#   winget install charmbracelet.vhs
#   winget install -e --id Gyan.FFmpeg
# Note: needs vhs >= 0.12.1 — winget may still ship v0.12.0, which on
# Windows silently fails to spawn ffmpeg (fixed in v0.12.1, issue #788).
# If `vhs demo.tape` prints "Creating demo.mp4..." but writes no file,
# grab the official Windows zip from github.com/charmbracelet/vhs/releases.
# Shell: PowerShell — `type` is the Get-Content alias.

Output demo.mp4
Set FontSize 20
Set Width 1400
Set Height 788
Set Padding 40
Set TypingSpeed 20ms

Type "type samples/DataForger.Demo/Program.cs"
Enter
Sleep 4s
Ctrl+L
Type "dotnet run --project samples/DataForger.Demo"
Enter
Sleep 8s