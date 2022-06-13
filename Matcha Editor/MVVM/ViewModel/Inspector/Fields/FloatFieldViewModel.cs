using Matcha_Editor.MVVM.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class FloatFieldViewModel : FieldViewModel
    {
        private float m_Value;

        public float Value 
        {
            get { return m_Value; }
            set 
            {
                m_Value = value;

                if (HasRange)
                    m_Value = Math.Clamp(m_Value, MinValue, MaxValue);

                NotifyPropertyChanged();
            }
        }

        public bool HasRange { get; private set; }
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public FloatFieldViewModel(InspectorComponentFieldModel model) : base(model)
        {
            m_Value = (float)model.Values[0].GetDouble();

            HasRange = DoesPropertyExist("range");

            if (HasRange)
            {
                MinValue = (float)model.Properties["range"]["min"].GetDouble();
                MaxValue = (float)model.Properties["range"]["max"].GetDouble();
            }
        }

        protected override void UpdateModelData()
        {
            Model.Values[0] = m_Value;
        }
    }
}
