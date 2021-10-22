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

        public Vector3FieldViewModel(dynamic values)
        {
            X = values[0].GetDouble();
            Y = values[1].GetDouble();
            Z = values[2].GetDouble();
        }
    }
}
