using Matcha_Editor.Core;
using Matcha_Editor.Core.IPC;
using Matcha_Editor.MVVM.View;
using Microsoft.Win32;
using System;
using System.Windows.Input;

namespace Matcha_Editor.MVVM.ViewModel.Commands
{
    internal class ConnectToEngineCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return IPCManager.Instance.HasActiveConnection;
        }

        public void Execute(object parameter)
        {
            if (IPCManager.Instance.HasActiveConnection)
                return;

            if (!MatchaEditor.Instance.ConnectToEngine())
                return;

            foreach (ViewportView v in EditorWindowManager.Instance.GetWindowsOfType<ViewportView>())
                v.RenderSurface.Initialize();
        }
    }
}
