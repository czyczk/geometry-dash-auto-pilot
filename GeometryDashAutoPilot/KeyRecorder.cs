using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Serilog;

namespace GeometryDashAutoPilot
{
    public class KeyRecorder
    {
        private readonly Stopwatch _stopwatch;

        private readonly LevelProfile _profile;

        private readonly IKeyboardMouseEvents _globalHook;

        private readonly LinkedList<(long, bool)> _tempOperations;

        private bool _isOperationKeyDown;

        public bool IsRecording { get; private set; }

        public void StartRecording()
        {
            if (IsRecording)
            {
                Log.Information("Recording is still running.");
                return;
            }

            _stopwatch.Start();
            IsRecording = true;
            _tempOperations.Clear();
            _globalHook.KeyDown += OnSpaceKeyDown;
            _globalHook.KeyUp += OnSpaceKeyUp;
            Log.Information("Recording started.");
        }

        public void StopRecording()
        {
            if (!IsRecording)
            {
                Log.Information("Recording is not running.");
                return;
            }

            IsRecording = false;

            // Unregister handlers
            _globalHook.KeyDown -= OnSpaceKeyDown;
            _globalHook.KeyUp -= OnSpaceKeyUp;

            _stopwatch.Reset();

            // Merge _tempOperations
            if (_tempOperations.Any())
            {
                _profile.RemoveOperationsSince(_tempOperations.First.Value.Item1);
//                // Fill the duration with key downs or it won't behave properly in the flight mode.
//                const long fillingStep = 170000L;
//                var lastTicks = 0L;
//                foreach (var operation in _tempOperations)
//                {
//                    if (operation.Item2)
//                    {
//                        lastTicks = operation.Item1;
//                    }
//                    else
//                    {
//                        var stack = new Stack<(long, bool)>();
//                        for (var tips = operation.Item1 - 1; tips > lastTicks; tips -= fillingStep)
//                        {
//                            stack.Push((tips, true));
//                        }
//                        
//                        try
//                        {
//                            while (true)
//                            {
//                                var (filledTips, filledIsKeyDown) = stack.Pop();
//                                _profile.AddOperation(filledTips, filledIsKeyDown);
//                            }
//                        }
//                        catch (InvalidOperationException)
//                        { }
//                    }
//                    _profile.AddOperation(operation.Item1, operation.Item2);
//                }
                _profile.AddOperations(_tempOperations);
            }

            _profile.ProcessOperations();

            Log.Information("Recording stopped and the position is reset.");
        }

        private void RecordOperation(long ticks, bool isKeyDown)
        {
            _tempOperations.AddLast((ticks, isKeyDown));
            Log.Debug($"Key operation recorded. IsKeyDown: {isKeyDown}. Ticks: {ticks}.");
        }

        private void OnSpaceKeyDown(object sender, KeyEventArgs e)
        {
            var elapsedTicks = _stopwatch.ElapsedTicks;

            if (e.KeyCode != Keys.Space && e.KeyCode != Keys.Up || e.Shift || e.Control || e.Alt)
                return;

            if (_isOperationKeyDown)
                return;

            _isOperationKeyDown = true;
            RecordOperation(elapsedTicks, true);
        }

        private void OnSpaceKeyUp(object sender, KeyEventArgs e)
        {
            var elapsedTicks = _stopwatch.ElapsedTicks;

            if (e.KeyCode != Keys.Space && e.KeyCode != Keys.Up || e.Shift || e.Control || e.Alt)
                return;

            // Fill the duration with key downs or it won't behave properly in the flight mode.
            if (_isOperationKeyDown)
            {
                _isOperationKeyDown = false;

                // Fetch the last operation (key down)
//                var lastOperation = _tempOperations.Last.Value;

//                const long fillingStep = 170000L;
//                for (var tips = lastOperation.Item1 + fillingStep; tips < elapsedTicks; tips += fillingStep)
//                {
//                    _tempOperations.AddLast((tips, true));
//                }

//                var stack = new Stack<(long, bool)>();
//                for (var tips = elapsedTicks - 1; tips > lastOperation.Item1; tips -= fillingStep)
//                {
//                    stack.Push((tips, true));
//                }
//
//                try
//                {
//                    while (true)
//                    {
//                        _tempOperations.AddLast(stack.Pop());
//                    }
//                }
//                catch (InvalidOperationException)
//                { }
            }

            RecordOperation(elapsedTicks, false);
        }

        public KeyRecorder(Stopwatch stopwatch, LevelProfile profile, IKeyboardMouseEvents globalHook)
        {
            _stopwatch = stopwatch;
            _profile = profile;
            _globalHook = globalHook;
            _tempOperations = new LinkedList<(long, bool)>();
        }
    }
}
