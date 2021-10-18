using Matcha_Editor.MVVM.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class HierarchyNodeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private HierarchyNode m_Current;
        private HierarchyNodeViewModel m_Parent;
        public ReadOnlyCollection<HierarchyNodeViewModel> Children { get; set; }


        public HierarchyNodeViewModel(HierarchyNode nodeModel, HierarchyNodeViewModel parent = null)
        {
            m_Current = nodeModel;
            m_Parent = parent;

            Children = new ReadOnlyCollection<HierarchyNodeViewModel>(
                (from child in nodeModel.Children
                 select new HierarchyNodeViewModel(child, this))
                 .ToList<HierarchyNodeViewModel>());

        }

        public string Name
        {
            get { return m_Current.Name; }
        }
        public string Guid
        {
            get { return m_Current.Guid; }
        }
        public bool Enabled
        {
            get { return m_Current.Enabled; }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
