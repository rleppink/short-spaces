using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms;
using WindowsDesktop;

namespace ShortSpaces
{
    internal class Program
    {
        private static void Main()
        {
            var desktops = VirtualDesktop.GetDesktops(); 

            var inviForm = new Form { Visible = false };
            var keys = new List<HotkeyWorkspace>
            {
                new HotkeyWorkspace { Workspace = 1, Hotkey = new Hotkey { KeyCode = Keys.D1, Alt = true }},
                new HotkeyWorkspace { Workspace = 2, Hotkey = new Hotkey { KeyCode = Keys.D2, Alt = true }},
                new HotkeyWorkspace { Workspace = 3, Hotkey = new Hotkey { KeyCode = Keys.D3, Alt = true }},
                new HotkeyWorkspace { Workspace = 4, Hotkey = new Hotkey { KeyCode = Keys.D4, Alt = true }},
                new HotkeyWorkspace { Workspace = 5, Hotkey = new Hotkey { KeyCode = Keys.D5, Alt = true }},
            };

            foreach (var hotkeyWorkspace in keys) 
            {
                hotkeyWorkspace.Hotkey.Pressed += delegate 
                {
                    desktops[hotkeyWorkspace.Workspace - 1].Switch();

                    // Windows doesn't automatically focus the front window after switching
                    // This forces focus to the frontmost window
                    var activeAppWindow = GetForegroundWindow();
                    SetForegroundWindow(activeAppWindow);
                };
                hotkeyWorkspace.Hotkey.Register(inviForm);
            }

            Application.Run();
        }

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private class HotkeyWorkspace
        {
            public Hotkey Hotkey { get; set; }

            public int Workspace { get; set; }
        }
    }
}
