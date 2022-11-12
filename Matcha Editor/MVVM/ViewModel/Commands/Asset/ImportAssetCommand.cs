using Matcha_Editor.Core.IPC;
using Microsoft.Win32;
using System;
using System.Windows.Input;

namespace Matcha_Editor.MVVM.ViewModel.Commands
{
    internal class ImportAssetCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Mesh (*.obj)|*.obj|Textures(*.png;*.jpg;*.jpeg;*.hdri)|*.png;*.jpg;*.jpeg;*.hdri|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    IPCManager.Instance.Post(new Core.IPC.Commands.ImportAssetCommand(filename));
                }
            }
        }
    }
}
