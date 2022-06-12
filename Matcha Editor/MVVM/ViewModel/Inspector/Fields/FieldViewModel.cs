using Matcha_Editor.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class FieldViewModel
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

        protected bool DoesPropertyExist(string name)
        {
            if (Model.Properties == null)
                return false;

            return Model.Properties.ContainsKey(name);
        }
    }
}
