using Matcha_Editor.Core.IPC;
using Matcha_Editor.MVVM.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Matcha_Editor.MVVM.ViewModel
{
    public abstract class FieldViewModel : INotifyPropertyChanged
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        protected InspectorComponentFieldModel Model { get; private set; }

        public FieldViewModel(InspectorComponentFieldModel model)
        {
            Model = model;
            Name = model.Name;
            Type = model.Type;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            UpdateModelData();
            IPCManager.Instance.Post(new Core.IPC.Commands.DataSetCommands.SetComponentCommand(Model.ComponentRef));
        }

        protected bool DoesPropertyExist(string name)
        {
            if (Model.Properties == null)
                return false;

            return Model.Properties.ContainsKey(name);
        }

        protected abstract void UpdateModelData();
    }
}
