using System;
using System.Collections.Generic;
using System.Text;
using Matcha_Editor.MVVM.Model;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class ComponentPropertyViewModel
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public FieldViewModel Field { get; set; }

        public ComponentPropertyViewModel(ComponentPropertyModel property)
        {
            Name = property.Name;
            Type = property.Type;

            switch (Type)
            {
                case "Vector3":
                    Field = new Vector3FieldViewModel(property.Values);
                    break;
                default:
                    Field = new BooleanFieldViewModel(property.Values);
                    break;
            }
        }
    }
}
