using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace ShortSpaces
{
    /// <summary>
    /// A rect struct for usage in Windows native methods.
    /// </summary>
    public struct Rect
    {
        public int Left { get; set; }

        public int Top { get; set; }

        public int Right { get; set; }

        public int Bottom { get; set; }

        /// <summary>
        /// Overloaded operator to use to check whether the two given rects are _not_ equal.
        /// </summary>
        /// <param name="rectA">The first rect to check</param>
        /// <param name="rectB">The second rect to check</param>
        /// <returns>Whether the two given rects are _not_ equal</returns>
        public static bool operator !=(Rect rectA, Rect rectB)
        {
            return !(rectA == rectB);
        }

        /// <summary>
        /// Overloaded operator to use to check whether the two given rects are equal.
        /// </summary>
        /// <param name="rectA">The first rect to check</param>
        /// <param name="rectB">The second rect to check</param>
        /// <returns>Whether the two given rects are equal</returns>
        public static bool operator ==(Rect rectA, Rect rectB)
        {
            if (Equals(rectA, rectB))
                return true;

            return (rectA.Left == rectB.Left) && (rectA.Top == rectB.Top) && (rectA.Right == rectB.Right) && (rectA.Bottom == rectB.Bottom);
        }

        /// <summary>
        /// Overloaded Equals function to check whether the given obj equals this rect.
        /// </summary>
        /// <param name="obj">The object to check this rect against</param>
        /// <returns>Whether this rect equals the given obj</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Rect rectObj;
            try
            {
                rectObj = (Rect)obj;
            }
            catch (InvalidCastException)
            {
                return false;
            }

            return (rectObj.Left == Left) && (rectObj.Top == Top) && (rectObj.Right == Right) && (rectObj.Bottom == Bottom);
        }

        /// <summary>
        /// Returns a unique hashcode for this rect.
        /// </summary>
        /// <returns>A unique hashcode for this rect</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
                // http://stackoverflow.com/questions/1835976/what-is-a-sensible-prime-for-hashcode-calculation/2816747#2816747
                const int prime = 92821;
                var hash = (int)2166136261;
                hash = hash * prime ^ Left;
                hash = hash * prime ^ Top;
                hash = hash * prime ^ Right;
                hash = hash * prime ^ Bottom;
                return hash;
            }
        }

        /// <summary>
        /// Returns a string representation of this rect.
        /// </summary>
        /// <returns>A string representation of this rect</returns>
        public override string ToString()
        {
            string[] values = {
                    Left.ToString(CultureInfo.CurrentCulture),
                    Top.ToString(CultureInfo.CurrentCulture),
                    Right.ToString(CultureInfo.CurrentCulture),
                    Bottom.ToString(CultureInfo.CurrentCulture)
            };
            return "Rect: (" + string.Join(", ", values) + ")";
        }
    }

    internal static class NativeMethods
    {
        /// <summary>
        /// Returns a handle to the currently focused/active/foreground window.
        /// </summary>
        /// <returns>A handle to the currently active window</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Returns a rect containing the size of the window with the given window handler.
        /// </summary>
        /// <param name="hWnd">The window handler</param>
        /// <param name="lpRect">The rect containing the size of the window</param>
        /// <returns>Whether getting the window rect size was a succes</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

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
        /// Sets the window with the given window handler to an x,y position, and resizes it to the given width & height.
        /// </summary>
        /// <param name="hWnd">The window handler</param>
        /// <param name="hWndInsertAfter">A window handler to 'insert' this window after</param>
        /// <param name="xPosition">The x position of the window</param>
        /// <param name="yPosition">The y position of the window</param>
        /// <param name="width">The width to set the window to</param>
        /// <param name="height">The height to set the window to</param>
        /// <param name="uFlags">Any flags to give when resizing - only using WindowResizer.ShowWindowFlag currently</param>
        /// <returns>An int value describing whether the window position was succesfully set. If the function succeeds, this is nonzero. If the function fails, this is zero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int xPosition, int yPosition, int width, int height, uint uFlags);

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