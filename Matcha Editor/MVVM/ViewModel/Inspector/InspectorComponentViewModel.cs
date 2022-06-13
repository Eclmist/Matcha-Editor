using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Matcha_Editor.MVVM.Model;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class InspectorComponentViewModel
    {
        public string Name { get; set; }
        public string Guid { get; set; }
        public bool Enabled { get; set; }

        public ObservableCollection<FieldViewModel> Fields { get; set; }

        private InspectorComponentModel Model { get; set; }

        public InspectorComponentViewModel(InspectorComponentModel component)
        {
            UpdateDataWithModel(component);
        }
        
        public void UpdateDataWithModel(InspectorComponentModel component)
        {
            Fields = new ObservableCollection<FieldViewModel>();
            Model = component;

            Name = component.Name;
            Guid = component.Guid;
            Enabled = component.Enabled;
            UpdateProperties(component.Fields);
        }

        public void UpdateProperties(InspectorComponentFieldModel[] properties)
        {
            if (properties == null)
                return;

            Fields.Clear();
            foreach (InspectorComponentFieldModel property in properties)
            {
                property.ComponentRef = Model;

                switch (property.Type)
                {
                    case "Vector3":
                        Fields.Add(new Vector3FieldViewModel(property));
                        break;
                    case "Vector4":
                        Fields.Add(new Vector4FieldViewModel(property));
                        break;
                    case "Float":
                        Fields.Add(new FloatFieldViewModel(property));
                        break;
                    case "Boolean":
                        Fields.Add(new BooleanFieldViewModel(property));
                        break;
                }
            }
        }
    }
}
