using System.Collections.ObjectModel;
using Matcha_Editor.MVVM.Model;

namespace Matcha_Editor.MVVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<ComponentModel> PropertyComponents { get; set; }

        public MainViewModel()
        {
            PropertyComponents = new ObservableCollection<ComponentModel>();
            PropertyComponents.Add(new ComponentModel
            {
                Name = "Transform",
                Guid = "2g9201-12011da1-1as-14"
            });
            PropertyComponents.Add(new ComponentModel
            {
                Name = "Visual",
                Guid = "9ge201-12011da1-1as-14"
            });
            PropertyComponents.Add(new ComponentModel
            {
                Name = "Mesh",
                Guid = "cg2201-12011da1-1as-14"
            });
        }
    }
}
