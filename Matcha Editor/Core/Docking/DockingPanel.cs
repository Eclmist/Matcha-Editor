using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Matcha_Editor.MVVM.View;

namespace Matcha_Editor.Core.Docking
{
    public class DockingPanel
    {
        public string Title { get; set; }
        public DockingNode Node { get; set; }
        public DockingPanelView PanelView { get; set; }
        public DockingContainerView ContainerView { get; set; }
        public UIElement Content { get; set; }
    }

    public class DockingPanelDescriptor
    {
        public string Title { get; set; }
        public UIElement Content { get; set; }
        public UIElement Container { get; set; }
        public DockingPanel Parent { get; set; }
        public DockingLayoutManager.DockPosition Position { get; set; }
    }
}
