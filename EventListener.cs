using System;
using System.Collections.Generic;
using System.Text;
using Gma.System.MouseKeyHook;

namespace GeometryDashAutoPilot
{
    public class EventListener
    {
        private IKeyboardMouseEvents _globalHook;

        public void Subscribe()
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyPress += (sender, e) => { };
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {

        }

        public void StartListening()
        {
            
        }
    }
}
