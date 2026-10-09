# DataForger demo video — https://vhs.charm.sh
#
# Render with:  vhs demo.tape          (produces demo.mp4)
# Requires:     vhs + ffmpeg on PATH
#   winget install charmbracelet.vhs
#   winget install -e --id Gyan.FFmpeg
# Tip: run from a bash-like shell (Git Bash / WSL) so `cat` works.

Output demo.mp4
Set FontSize 20
Set Width 1400
Set Height 788
Set Padding 40
Set TypingSpeed 20ms

Type "cat samples/DataForger.Demo/Program.cs"
Enter
Sleep 4s
Ctrl+L
Type "dotnet run --project samples/DataForger.Demo"
Enter
Sleep 8s