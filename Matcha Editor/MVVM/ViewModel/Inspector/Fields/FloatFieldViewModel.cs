using Matcha_Editor.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class FloatFieldViewModel : FieldViewModel
    {
        public float value { get; set; }

        public bool hasRange { get; private set; }
        public float minValue { get; private set; }
        public float maxValue { get; private set; }

        public FloatFieldViewModel(InspectorComponentFieldModel model) : base(model)
        {
            value = (float)model.Values[0].GetDouble();

            hasRange = DoesPropertyExist("range");

            if (hasRange)
            {
                minValue = (float)model.Properties["range"]["min"].GetDouble();
                maxValue = (float)model.Properties["range"]["max"].GetDouble();
            }
        }
    }
}
