using System;
using System.Collections.Generic;
using System.Text;

namespace Matcha_Editor.MVVM.Model
{
    public class HierarchyNode
    {
        public HierarchyNode Parent { get; set; }
        public IList<HierarchyNode> Children { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }
        public bool Enabled { get; set; }

        public HierarchyNode()
        {
            Children = new List<HierarchyNode>();
        }
    }
}
