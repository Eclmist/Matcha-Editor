using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Runtime.CompilerServices;
using Matcha_Editor.MVVM.Model;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class InspectorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string m_SelectedItemName;
        private string m_SelectedItemID;
        private bool m_SelectedItemEnabled;

        public string SelectedItemName
        {
            get
            {
                return m_SelectedItemName;
            }
            set
            {
                if (m_SelectedItemName != value)
                {
                    m_SelectedItemName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SelectedItemID
        {
            get
            {
                return m_SelectedItemID;
            }
            set
            {
                if (m_SelectedItemID != value)
                {
                    m_SelectedItemID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool SelectedItemEnabled
        {
            get
            {
                return m_SelectedItemEnabled;
            }
            set
            {
                if (m_SelectedItemEnabled != value)
                {
                    m_SelectedItemEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<InspectorComponentViewModel> Components { get; set; }

        public InspectorViewModel()
        {
            Components = new ObservableCollection<InspectorComponentViewModel>();
        }

        public void UpdateComponents(InspectorComponentModel[] components)
        {
            Components.Clear();

            for (int i = 0; i < components.Length; ++i)
                Components.Add(new InspectorComponentViewModel(components[i]));

            //for (int i = 0; i < components.Length; ++i)
            //{
            //    if (Components.Count > i)
            //    {
            //        Components[i].Name = components[i].Name;
            //        Components[i].Guid = components[i].Guid;
            //        Components[i].Enabled = components[i].Enabled;
            //        Components[i].IsFixed = components[i].IsFixed;
            //        Components[i].Mo = components[i].IsFixed;

            //        Components[i].UpdateProperties(components[i].Properties);
            //    }
            //    else
            //    {
            //        Components.Add(new InspectorComponentViewModel(components[i]));
            //    }
            //}

            //while (Components.Count > components.Length)
            //    Components.RemoveAt(Components.Count - 1);

            NotifyPropertyChanged();
        }
    }
}
