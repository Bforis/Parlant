using System;
using System.Text;
using System.Drawing;  
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Speech.Synthesis;
using NHotkey;
using NHotkey.Wpf;
using System.Windows.Controls;

namespace Parlant
{
    public partial class MainWindow : Window
    {
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        private NotifyIcon trayIcon;

        private bool isCurrentTextRead = false;

        private string lastText = "";

        // Initialize
        public MainWindow()
        {
            InitializeComponent();

            // Shortcut HotKey
            HotkeyManager.Current.AddOrReplace("ReadClipboard", Key.R, ModifierKeys.Control | ModifierKeys.Shift, HandleReadClipboardHotkey);

            // Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = new Icon("parlant_logo_ico.ico"),
                Visible = true
            };
            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            // Language
            foreach (InstalledVoice voice in synthesizer.GetInstalledVoices())
            {
                VoiceInfo info = voice.VoiceInfo;
                languageComboBox.Items.Add(info.Name);
            }
            // Actual default voice
            string currentVoice = synthesizer.Voice.Name;
            languageComboBox.SelectedItem = currentVoice;

            // Play
            synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
        }

        // Minify Tray Icon
        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        // Change language
        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? selectedVoice = languageComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedVoice))
            {
                synthesizer.SelectVoice(selectedVoice);
            }
        }

        // Synthetizer Logic
        private void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            isCurrentTextRead = false;
        }
        private void ReadTextFromClipboard()
        {
            Dispatcher.Invoke(() =>
            {
                // If app is not loaded
                if (!IsLoaded) return;

                try
                {
                    string text = System.Windows.Clipboard.GetText();

                    if (!string.IsNullOrWhiteSpace(text) && text != lastText)
                    {
                        // Cancel Speaking
                        synthesizer.SpeakAsyncCancelAll(); 
                        StartSpeaking(text);
                        // Update last copied text
                        lastText = text;
                        isCurrentTextRead = true;
                    }
                    else if (synthesizer.State == SynthesizerState.Paused)
                    {
                        // If paused, resume reading
                        synthesizer.Resume();
                        // Update state after resume
                        isCurrentTextRead = true; 
                    }
                    else if (synthesizer.State == SynthesizerState.Speaking)
                    {
                        // If speaking, pause
                        synthesizer.Pause();
                        // Update state after pause
                        isCurrentTextRead = false;
                    }
                }

                // Handle errors
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    System.Windows.MessageBox.Show($"Error trying to access clipboard: {ex.Message}");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"An unexpected error occurred: {ex.Message}");
                }
            });
        }


        // Play Synthetizer
        private void StartSpeaking(string text)
        {
            synthesizer.Volume = (int)volumeSlider.Value;
            synthesizer.Rate = (int)rateSlider.Value;
            synthesizer.SpeakAsync(text);
        }

        private void HandleReadClipboardHotkey(object sender, HotkeyEventArgs e)
        {
            ReadTextFromClipboard();
        }
        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            ReadTextFromClipboard();
        }

        // App State Open/Closed
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            trayIcon.Dispose();
            base.OnClosed(e);
        }
    }
}

/*
Error:
- After pausing and changing the text, you need to launch the audio twice for it to work.
- You can't change speed, voice or volume during playback, so you'll have to change the settings and copy new text, then play again.

Notes: 
- Minimizing the window will display the application in the notification area (system tray), but closing the window will close the application. To close the application while it's in the notification area, double-click on the icon and simply close the window.
- The .ico doesn't fit properly.

NuGet Dependancies: 
- NHotKey.WPF
- System.Speech
- Forms
- WPF
 */
