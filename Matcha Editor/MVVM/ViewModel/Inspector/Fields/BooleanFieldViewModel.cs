using System;
using System.Collections.Generic;
using System.Text;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class BooleanFieldViewModel : FieldViewModel
    {
        public bool Enabled { get; set; }

        public BooleanFieldViewModel(dynamic values)
        {
            Enabled = values[0].GetBoolean();
        }
    }
}
