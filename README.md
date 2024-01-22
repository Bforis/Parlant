# Parlant

A simple text-to-speech application on Windows, you simply leave the application open, **copy a text** and use the shortcut **CTRL+Shift+R** or click on the play button in the application to launch audio playback of the copied text.

You can change the language in relation to the languages installed on your computer. You can also change the volume and speed of the audio.

Notes: 
- Minimizing the window will display the application in the notification area (system tray), but closing the window will close the application. To close the application while it's in the notification area, double-click on the icon and simply close the window.
- The .ico doesn't fit properly.

*Error*:
- After pausing and changing the text, you need to launch the audio twice for it to work.
- You can't change speed, voice or volume during playback, so you'll have to change the settings and copy new text, then play again.

*NuGet Dependancies*: 
- WPF
- Forms
- NHotKey.WPF
- System.Speech
