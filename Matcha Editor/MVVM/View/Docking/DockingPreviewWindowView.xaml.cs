using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Matcha_Editor.MVVM.View
{
    public partial class DockingPreviewWindowView : Window
    {
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);

        public const int DefaultWidth = 200;
        public const int DefaultHeight = 160;


        public DockingPreviewWindowView()
        {
            InitializeComponent();
            //IntPtr hWnd = new WindowInteropHelper(this).EnsureHandle();
            //int extendedStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            //SetWindowLong(hWnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

        public void ShowAsWindow(Point pos)
        {
            pos.X -= Width / 2;
            pos.Y -= Height / 2;
            ShowAsWindow(pos, new Size(DefaultWidth, DefaultHeight));
        }

        public void ShowAsWindow(Point pos, Size size)
        {
            Width = size.Width;
            Height = size.Height;
            Left = pos.X;
            Top = pos.Y;
            Show();
        }

        public void ShowAsTab()
        {
            Show();
        }

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
    }
}
