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
        private IntPtr m_ClientHwnd = IntPtr.Zero;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            InitializeCommand initializeCommand = new InitializeCommand(hwndParent.Handle);
            var response = new InitializeCommand.Response(IPCManager.Instance.Get(initializeCommand));
            m_ClientHwnd = (IntPtr)response.ResponseData.Args.ChildHwnd;
            Debug.Assert(m_ClientHwnd != IntPtr.Zero);
            return new HandleRef(this, m_ClientHwnd);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            m_ClientHwnd = IntPtr.Zero;
        }
    }
}
