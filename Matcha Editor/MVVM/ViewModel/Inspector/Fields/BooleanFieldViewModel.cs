using Matcha_Editor.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class BooleanFieldViewModel : FieldViewModel
    {
        public bool Enabled { get; set; }

        public BooleanFieldViewModel(InspectorComponentFieldModel model) : base(model)
        {
            Enabled = model.Values[0].GetBoolean();
        }
    }
}
