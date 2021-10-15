using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace Matcha_Editor.Core.Docking
{
    public class DockingNode
    {
        public Rect Rect { get; set; }

        public DockingNode Parent { get; set; }
        public DockingNode LeftChild { get; set; }
        public DockingNode RightChild { get; set; }

        public bool HasChildren()
        {
            Debug.Assert(
                (LeftChild == null && RightChild == null) ||
                (LeftChild != null && RightChild != null));
            return LeftChild != null;
        }
    }
}
