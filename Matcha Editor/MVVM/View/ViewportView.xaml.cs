using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Matcha_Editor.MVVM.View
{
    public partial class ViewportView : UserControl
    {
        public ViewportView()
        {
            InitializeComponent();
        }

        public IntPtr GetWindowsHandle()
        {
            Window window = Window.GetWindow(this);
            if (window == null)
                return IntPtr.Zero;

            return new WindowInteropHelper(window).EnsureHandle();
        }
    }
}
