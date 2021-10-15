using Matcha_Editor.Core.IPC;
using Matcha_Editor.Core.IPC.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;

namespace Matcha_Editor.Core.RenderSurface
{
    public class RenderSurfaceHost : HwndHost
    {
        public static RenderSurfaceHost Instance;

        public RenderSurfaceHost()
        {
            Instance = this;
        }

        // Hwnd of the client is owned by the engine.
        private IntPtr m_ClientHwnd = IntPtr.Zero;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            InitializeCommand initializeCommand = new InitializeCommand(hwndParent.Handle);
            var response = new InitializeCommand.Response(IPCManager.Instance.Get(initializeCommand));
            m_ClientHwnd = (IntPtr)response.ResponseData.args.childhwnd;
            Debug.Assert(m_ClientHwnd != IntPtr.Zero);
            return new HandleRef(this, m_ClientHwnd);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            m_ClientHwnd = IntPtr.Zero;
        }


        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            base.WndProc(hwnd, msg, wParam, lParam, ref handled);

            return IntPtr.Zero;
        }
        //public IntPtr WndProcMsgFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{

        //    if (msg.Equals(0x0021))
        //    {
        //        RenderSurfaceHost.SendMessage(RenderSurfaceHost.Instance.m_ClientHwnd, 0x0006, new IntPtr(1), new IntPtr(0));
        //    }
        //    handled = false;
        //    return IntPtr.Zero;
        //}

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        internal static extern int SendMessage(IntPtr hwnd,
    int msg,
    IntPtr wParam,
    IntPtr lParam);
    }
}
