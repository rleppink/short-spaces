using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ShortSpaces
{
    /*
     *  Hotkey abstraction kindly modified from:
     *  https://bloggablea.wordpress.com/2007/05/01/global-hotkeys-with-net/
     */

    /// <summary>
    /// Defines all Hotkey registering and unregistering functionality.
    /// </summary>
    public class Hotkey : IMessageFilter
    {
        private const uint ErrorHotkeyAlreadyRegistered = 1409;
        private const int MaximumId = 0xBFFF;
        private const uint ModAlt = 0x1;
        private const uint ModControl = 0x2;
        private const uint ModShift = 0x4;
        private const uint ModWin = 0x8;
        private const uint WmHotkey = 0x312;
        private static int _currentId;
        private bool _alt;
        private bool _control;

        [XmlIgnore]
        private int _id;

        private Keys _keyCode;

        [XmlIgnore]
        private bool _registered;

        private bool _shift;

        [XmlIgnore]
        private Control _windowControl;

        private bool _windows;

        public Hotkey() : this(Keys.None, false, false, false, false)
        {
        }

        public Hotkey(Keys keyCode, bool shift, bool control, bool alt, bool windows)
        {
            KeyCode = keyCode;
            Shift = shift;
            Control = control;
            Alt = alt;
            Windows = windows;

            Application.AddMessageFilter(this);
        }

        ~Hotkey()
        {
            if (Registered)
                Unregister();
        }

        public event HandledEventHandler Pressed;

        public bool Alt
        {
            get => _alt;

            set
            {
                _alt = value;
                Reregister();
            }
        }

        public bool Control
        {
            get => _control;

            set
            {
                _control = value;
                Reregister();
            }
        }

        public bool Empty => _keyCode == Keys.None;

        public Keys KeyCode
        {
            get => _keyCode;

            set
            {
                _keyCode = value;
                Reregister();
            }
        }

        public bool Registered => _registered;

        public bool Shift
        {
            get => _shift;

            set
            {
                _shift = value;
                Reregister();
            }
        }

        public bool Windows
        {
            get => _windows;

            set
            {
                _windows = value;
                Reregister();
            }
        }

        /// <summary>
        /// Returns whether this hotkey can be registered to the given control.
        /// </summary>
        /// <param name="controlToRegister">The control against which to check whether this hotkey can be registered</param>
        /// <returns>A boolean value stating whether the hotkey can be registered</returns>
        public bool GetCanRegister(Control controlToRegister)
        {
            try
            {
                if (!Register(controlToRegister))
                    return false;

                Unregister();
                return true;
            }
            catch (Win32Exception)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }

        /// <summary>
        /// Inherited from IMessageFilter.
        /// </summary>
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != WmHotkey)
                return false;

            if (_registered && m.WParam.ToInt32() == _id)
                return OnPressed();

            return false;
        }

        /// <summary>
        /// Registers this hotkey to the given control
        /// </summary>
        /// <param name="controlToRegister">The control to register this hotkey to</param>
        /// <returns>A value indicating whether the registering of the hotkey was successful</returns>
        public bool Register(Control controlToRegister)
        {
            if (controlToRegister == null)
                return false;

            if (_registered)
                return true;

            if (Empty)
                throw new NotSupportedException("You cannot register an empty hotkey");

            _id = _currentId;
            _currentId = (_currentId + 1) % MaximumId;

            var modifiers = (Alt ? ModAlt : 0) | (Control ? ModControl : 0) |
                            (Shift ? ModShift : 0) | (Windows ? ModWin : 0);

            if (NativeMethods.RegisterHotKey(controlToRegister.Handle, _id, modifiers, _keyCode) == 0)
            {
                if (Marshal.GetLastWin32Error() == ErrorHotkeyAlreadyRegistered)
                    return false;

                throw new Win32Exception();
            }

            _registered = true;
            _windowControl = controlToRegister;

            return true;
        }

        /// <summary>
        /// Removes the hotkey handler for this hotkey
        /// </summary>
        public void RemoveHandler()
        {
            Pressed = null;
        }

        /// <summary>
        /// Returns a nicely formatted string of this hotkey.
        /// </summary>
        /// <returns>A nicely formatted string of this hotkey</returns>
        public override string ToString()
        {
            var keys = new List<string>();

            if (Control)
                keys.Add("Ctrl");

            if (Alt)
                keys.Add("Alt");

            if (Shift)
                keys.Add("Shift");

            //// TODO: bool windowsPressed = (Control.ModifierKeys | Keys.LWin) == keyEventArgs.Modifiers;

            if (KeyCode != Keys.ShiftKey &&
                KeyCode != Keys.ControlKey &&
                KeyCode != Keys.Menu &&
                KeyCode != Keys.LWin &&
                KeyCode != Keys.RWin)
                keys.Add(KeyCode.ToString());

            return string.Join(" + ", keys.ToArray());
        }

        /// <summary>
        /// Unregisters this hotkey if it was registered. If not, just passes.
        /// </summary>
        public void Unregister()
        {
            if (!_registered)
                return;

            if (!_windowControl.IsDisposed)
                if (NativeMethods.UnregisterHotKey(_windowControl.Handle, _id) == 0)
                    throw new Win32Exception();

            _registered = false;
            _windowControl = null;
        }

        /// <summary>
        /// Function that is called when the hotkey was pressed.
        /// </summary>
        /// <returns>A boolean stating whether the event was handled</returns>
        private bool OnPressed()
        {
            var handledEventArgs = new HandledEventArgs(false);
            Pressed?.Invoke(this, handledEventArgs);

            return handledEventArgs.Handled;
        }

        /// <summary>
        /// Re-registers the current hotkey.
        /// </summary>
        private void Reregister()
        {
            if (!_registered)
                return;

            var currentWindowControl = _windowControl;

            Unregister();
            Register(currentWindowControl);
        }
    }
}