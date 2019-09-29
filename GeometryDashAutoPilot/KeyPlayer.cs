using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using Serilog;

namespace GeometryDashAutoPilot
{
    public class KeyPlayer
    {
        private readonly Stopwatch _stopwatch;

        private readonly LevelProfile _levelProfile;

        private readonly IKeyboardSimulator _keyboard;

        private readonly IMouseSimulator _mouse;

        private int _lastPlaybackPosition;

        public bool IsPlaying { get; private set; }

        protected void StartPressing(long ticks, VirtualKeyCode keyCode = VirtualKeyCode.SPACE)
        {
            Log.Debug($"Operation performed. IsKeyDown: {true}. Ticks: {ticks}.");
//            _keyboard.KeyDown(keyCode);
            _mouse.LeftButtonDown();
        }

        protected void EndPressing(long ticks, VirtualKeyCode keyCode = VirtualKeyCode.SPACE)
        {
            Log.Debug($"Operation performed. IsKeyDown: {false}. Ticks: {ticks}.");
            //            _keyboard.KeyUp(keyCode);
            _mouse.LeftButtonUp();
        }

        /**
         * Start or pause the playback according to the situation.
         */
        public void TogglePlayback()
        {
            if (IsPlaying)
            {
                // If it's playing, pause the stopwatch and the operation playback
                PausePlayback();
                return;
            }

            // Start the playback if it's not playing
            StartPlayback();
        }

        /**
         * Start the stopwatch and the operation playback.
         */
        public void StartPlayback()
        {
//            if (!_stopwatch.IsRunning)
//            {
//                // Playback has been reset before
//                Log.Verbose("[Stopwatch] The stopwatch is not running when the playback starts. The playback has been reset before.");
//                _lastPlaybackPosition = 0;
//            }

            IsPlaying = true;
            _stopwatch.Start();
            Log.Verbose("[Stopwatch] Stopwatch started.");

            var playingTask = new Task(() =>
            {
                while (IsPlaying)
                {
                    if (_lastPlaybackPosition == _levelProfile.Count)
                    {
                        IsPlaying = false; // Playback is now finished
                        Log.Information("Playback finished.");
                        break;
                    }

                    var currentTicks = _stopwatch.ElapsedTicks;
                    var (operationTicks, operationIsStartPressing) = _levelProfile[_lastPlaybackPosition];
                    if (operationTicks > currentTicks) continue;
                    if (operationIsStartPressing)
                        StartPressing(operationTicks);
                    else
                        EndPressing(operationTicks);
                    _lastPlaybackPosition++;
                }
            });
            playingTask.Start();
            Log.Information("Playback started.");
        }

        /**
         * Pause the stopwatch and the operation playback.
         */
        public void PausePlayback()
        {
            IsPlaying = false;
            _stopwatch.Stop();
            Log.Verbose("[Stopwatch] Stopwatch stopped.");
            Log.Information("Playback paused.");
        }

        /**
         * Stop the operation playback but keep the stopwatch running.
         * Should be triggered before entering recording mode.
         */
        public void StopPlaybackResumingStopwatch()
        {
            IsPlaying = false;
            _stopwatch.Start();
        }

        public void ResetPlayback()
        {
            Log.Information(IsPlaying ? "Playback stopped and the position is reset." : "The position is reset.");
            IsPlaying = false;
            _stopwatch.Reset();
            _lastPlaybackPosition = 0;
        }

        public KeyPlayer(Stopwatch stopwatch, LevelProfile levelProfile)
        {
            _stopwatch = stopwatch;
            _keyboard = new InputSimulator().Keyboard;
            _mouse = new InputSimulator().Mouse;
            _levelProfile = levelProfile;
        }
    }
}
