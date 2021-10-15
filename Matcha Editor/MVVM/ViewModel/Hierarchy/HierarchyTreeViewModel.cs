using Matcha_Editor.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class HierarchyTreeViewModel
    {

        public ReadOnlyCollection<HierarchyNodeViewModel> RootNode { get; set; }

        public HierarchyTreeViewModel(HierarchyNode rootNode)
        {
            RootNode = new ReadOnlyCollection<HierarchyNodeViewModel>(
                new HierarchyNodeViewModel[]
                    {
                        new HierarchyNodeViewModel(rootNode)
                    }
                );
        }
    }
}
