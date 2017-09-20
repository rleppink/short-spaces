using System;
using System.Runtime.InteropServices;

namespace ShortSpaces
{

    internal static class NativeMethods
    {
        /// <summary>
        /// Returns a handle to the currently focused/active/foreground window.
        /// </summary>
        /// <returns>A handle to the currently active window</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Registers a hotkey (modifiers+keys) to the window with the given window handler.
        /// </summary>
        /// <param name="hWnd">The window handler</param>
        /// <param name="id">The hotkey id</param>
        /// <param name="fsModifiers">The hotkey modifiers</param>
        /// <param name="vk">The keys for the hotkey</param>
        /// <returns>An int value describing whether the hotkey was succesfully registered. If the function succeeds, this is nonzero. If the function fails, this is zero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, System.Windows.Forms.Keys vk);

        /// <summary>
        /// Unregisters the hotkey with the given id from the window with the given window handler.
        /// </summary>
        /// <param name="hWnd">The window handler</param>
        /// <param name="id">The hotkey id</param>
        /// <returns>An int value describing whether the hotkey was succesfully unregistered. If the function succeeds, this is nonzero. If the function fails, this is zero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int UnregisterHotKey(IntPtr hWnd, int id);
    }
}