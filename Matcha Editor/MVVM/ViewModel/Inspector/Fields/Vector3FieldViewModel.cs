using Matcha_Editor.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class Vector3FieldViewModel : FieldViewModel
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3FieldViewModel(InspectorComponentFieldModel model) : base(model)
        {
            X = model.Values[0].GetDouble();
            Y = model.Values[1].GetDouble();
            Z = model.Values[2].GetDouble();
        }
    }
}
