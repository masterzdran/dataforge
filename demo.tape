# DataForger demo video — https://vhs.charm.sh
#
# Render with:  vhs demo.tape   (run from the repo root; outputs demo.gif there)
# Requires:     vhs >= 0.12.1 + ffmpeg on PATH
#   winget install charmbracelet.vhs   (may still ship v0.12.0, which on Windows
#   silently produces no output — fixed in v0.12.1, issue #788; grab the official
#   Windows zip from github.com/charmbracelet/vhs/releases if needed)
#   winget install -e --id Gyan.FFmpeg
# Shell: VHS drives ttyd, which on Windows runs cmd.exe — so use backslash
# paths and cmd built-ins (`type`, `cls`, `copy con`). The `Output` directive
# does not support Windows absolute paths (issue #741).
# The canvas capture on Windows runs at ~25fps, so Framerate must match it or
# the video plays back at 2x real time.

Output demo.gif
Set FontSize 20
Set Width 1400
Set Height 788
Set Padding 40
Set TypingSpeed 15ms
Set PlaybackSpeed 1
Set Framerate 25

# 1. Type a basic example live — `copy con` echoes every line as it is written.
Type "copy /y con samples\Basic.cs"
Enter
Sleep 600ms
Type "#:project ../src/DataForger"
Enter
Sleep 250ms
Type "using System;"
Enter
Sleep 250ms
Type "using DataForger;"
Enter
Sleep 250ms
Type "var person = Forge.ForCountry(Country.PT)"
Enter
Sleep 250ms
Type "    .WithSeed(1234)"
Enter
Sleep 250ms
Type "    .Create<Person>();"
Enter
Sleep 250ms
Type "Console.WriteLine(person.FullName);"
Enter
Sleep 250ms
Type "Console.WriteLine(person.Email);"
Enter
Sleep 250ms
# Ctrl+Z ends `copy con`; cmd writes the file.
Ctrl+Z
Enter
Sleep 800ms

# 2. Run the file-based app.
Type "dotnet run samples\Basic.cs"
Enter
Sleep 20s