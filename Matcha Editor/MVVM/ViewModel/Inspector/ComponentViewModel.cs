using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Matcha_Editor.MVVM.Model;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class ComponentViewModel
    {
        public string Name { get; set; }
        public string Guid { get; set; }
        public bool Enabled { get; set; }
        public bool IsFixed { get; set; }

        //public ObservableCollection<>
        //public ComponentPropertyModel[] Properties { get; set; }

        public ComponentViewModel(ComponentModel component)
        {
            Name = component.Name;
            Guid = component.Guid;
            Enabled = component.Enabled;
            IsFixed = component.IsFixed;
        }
    }
}
