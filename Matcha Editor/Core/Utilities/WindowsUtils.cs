using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace Matcha_Editor.Core.Utilities
{
    public static class WindowsUtils
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        public static int GetWindowZ(Window window)
        {
            IntPtr hWnd = GetHwnd(window);
            var zOrder = -1;
            while ((hWnd = GetWindow(hWnd, 2 /* GW_HWNDNEXT */)) != IntPtr.Zero) zOrder++;
            return zOrder;
        }

        public static IntPtr GetHwnd(Window element)
        {
            var window = Window.GetWindow(element);
            return new WindowInteropHelper(window).EnsureHandle();
        }
    }
}
