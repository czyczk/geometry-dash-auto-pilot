using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Serilog;

namespace GeometryDashAutoPilot
{
    public class GlobalController
    {
        private Stopwatch _stopwatch;

        private LevelProfile _levelProfile;

        private KeyPlayer _keyPlayer;

        private KeyRecorder _keyRecorder;

        private IKeyboardMouseEvents _globalHook;

        public void SubscribeHandlers()
        {
            _globalHook.KeyDown += GlobalHookKeyDown;
            _globalHook.KeyUp += GlobalHookKeyUp;
            _globalHook.KeyPress += GlobalHookKeyPress;
            Log.Information("Global key listeners started.");
        }

        private void GlobalHookKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F6)
            {
                Log.Debug("Key down: F6");
                // F6: Stop recording if it's recording, otherwise stop the operation playback and reset the stopwatch
                if (_keyRecorder.IsRecording)
                    _keyRecorder.StopRecording();
                else
                    _keyPlayer.ResetPlayback();
            }
            else if (e.KeyCode == Keys.S && e.Control)
            {
                Log.Debug("Key down: Ctrl + S");
                // Ctrl + S: Save the operations to the level profile
                _levelProfile.SaveOperationsToFile();
            }
            else if (e.KeyCode == Keys.R && e.Control)
            {
                Log.Debug("Key down: Ctrl + R");
                // Ctrl + R: Reload the operations from the level profile
                _levelProfile.LoadOperationsFromFile();
            }
        }

        private void GlobalHookKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                Log.Debug("Key up: F5");
                e.SuppressKeyPress = true;
                // F5: Start or pause the stopwatch and operation playback
                // If the recorder is running, ignore the request
                if (_keyRecorder.IsRecording)
                    return;

                // Toggle playback only if the profile is not empty. Otherwise, start recording.
                if (_levelProfile.Count > 0)
                {
                    _keyPlayer.TogglePlayback();
                }
                else
                {
                    Log.Information("There is nothing to play in the profile.");
                    _keyRecorder.StartRecording();
                }
            }
            else if (e.KeyCode == Keys.F10)
            {
                Log.Debug("Key up: F10");
                // F12: Stop the operation playback, keep the stopwatch running and enter recording mode
                _keyPlayer.StopPlaybackResumingStopwatch();
                _keyRecorder.StartRecording();
            }
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
        }

        public GlobalController(LevelProfile levelProfile)
        {
            _globalHook = Hook.GlobalEvents();
            Log.Information($"Using high-resolution stopwatch: {Stopwatch.IsHighResolution}.");
            _stopwatch = new Stopwatch();
            _levelProfile = levelProfile;
            _keyPlayer = new KeyPlayer(_stopwatch, levelProfile);
            _keyRecorder = new KeyRecorder(_stopwatch, _levelProfile, _globalHook);
        }
    }
}
