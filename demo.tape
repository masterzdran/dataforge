# DataForger demo video — https://vhs.charm.sh
#
# Render with:  vhs demo.tape   (run from the repo root; outputs demo.mp4 there)
# Requires:     vhs >= 0.12.1 + ffmpeg on PATH
#   winget install charmbracelet.vhs   (may still ship v0.12.0, which on Windows
#   silently produces no output — fixed in v0.12.1, issue #788; grab the official
#   Windows zip from github.com/charmbracelet/vhs/releases if needed)
#   winget install -e --id Gyan.FFmpeg
# Shell: VHS drives ttyd, which on Windows runs cmd.exe — so use backslash
# paths and cmd built-ins (`type`, `cls`). The `Output` directive does not
# support Windows absolute paths (issue #741).

Output demo.mp4
Set FontSize 20
Set Width 1400
Set Height 788
Set Padding 40
Set TypingSpeed 15ms
Set PlaybackSpeed 1
# The canvas capture on Windows runs at ~25fps, so timestamps must match or
# the video plays back at 2x real time.
Set Framerate 25

# 1. Show the demo source: the fluent, per-country API.
Type "type samples\DataForger.Demo\Program.cs"
Enter
Sleep 4s

# 2. Clear the screen and run the demo.
Type "cls"
Enter
Sleep 500ms
Type "dotnet run --project samples\DataForger.Demo\DataForger.Demo.csproj"
Enter
Sleep 25s