using Matcha_Editor.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class BooleanFieldViewModel : FieldViewModel
    {
        public bool m_Enabled;
        public bool Enabled
        {
            get { return m_Enabled; }
            set
            {
                m_Enabled = value;
                NotifyPropertyChanged();
            }
        }

        public BooleanFieldViewModel(InspectorComponentFieldModel model) : base(model)
        {
            m_Enabled = model.Values[0].GetBoolean();
        }

        protected override void UpdateModelData()
        {
            Model.Values[0] = Enabled;
        }
    }
}
