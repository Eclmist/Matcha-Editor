using Matcha_Editor.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class VectorFieldViewModel : FieldViewModel
    {
        private float[] m_Vector;

        public float X
        {
            get { return m_Vector[0]; }
            set
            {
                m_Vector[0] = value;
                NotifyPropertyChanged();
            }
        }

        public float Y
        {
            get { return m_Vector[1]; }
            set
            {
                m_Vector[1] = value;
                NotifyPropertyChanged();
            }
        }

        public float Z
        {
            get { return m_Vector[2]; }
            set
            {
                m_Vector[2] = value;
                NotifyPropertyChanged();
            }
        }

        public float W
        {
            get { return m_Vector[3]; }
            set
            {
                m_Vector[3] = value;
                NotifyPropertyChanged();
            }
        }

        public VectorFieldViewModel(InspectorComponentFieldModel model) : base(model)
        {
            m_Vector = new float[model.Values.Count];
            for (int i = 0; i < m_Vector.Length; ++i)
                m_Vector[i] = (float)model.Values[i].GetDouble();
        }

        protected override void UpdateModelData()
        {
            for (int i = 0; i < m_Vector.Length; ++i)
                Model.Values[i] = m_Vector[i];
        }
    }
}
