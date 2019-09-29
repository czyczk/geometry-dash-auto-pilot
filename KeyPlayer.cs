using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WindowsInput;
using WindowsInput.Native;

namespace GeometryDashAutoPilot
{
    public class KeyPlayer
    {
        private Stopwatch _stopwatch;

        private readonly IKeyboardSimulator _keyboard;

        public void StartPressing(VirtualKeyCode keyCode = VirtualKeyCode.SPACE) => _keyboard.KeyDown(keyCode);

        public void EndPressing(VirtualKeyCode keyCode = VirtualKeyCode.SPACE) => _keyboard.KeyUp(keyCode);

        //        public void TestSend()
        //        {
        //            var inputSim = new InputSimulator();
        //            inputSim.Keyboard.KeyDown(VirtualKeyCode.VK_A);
        //            inputSim.Keyboard.KeyUp(VirtualKeyCode.VK_A);
        //            inputSim.Keyboard.KeyDown(VirtualKeyCode.VK_B);
        //            inputSim.Keyboard.KeyUp(VirtualKeyCode.VK_B);
        //        }

        public KeyPlayer()
        {
            _stopwatch = new Stopwatch();
            _keyboard = new InputSimulator().Keyboard;
        }
    }
}
