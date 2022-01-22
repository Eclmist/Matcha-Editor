using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Matcha_Editor.MVVM.View;

namespace Matcha_Editor.Core.Docking
{
    public class DockingPanel
    {
        public DockingNode Node { get; set; }
        public DockingPanelView PanelView { get; set; }
        public DockingContainerView ContainerView { get; set; }

        private List<DockingPanelTab> m_Tabs = new List<DockingPanelTab>();
        private int m_ActiveTabIndex = 0;

        public int GetNumTabs() { return m_Tabs.Count; }

        public int AddTab(DockingPanelTab tab, int atIndex = -1)
        {
            if (atIndex == -1)
                atIndex = m_Tabs.Count;

            m_Tabs.Insert(atIndex, tab);
            tab.Parent = this;
            return atIndex;
        }

        public void RemoveTab(DockingPanelTab tab)
        {
            m_Tabs.Remove(tab);
            if (m_Tabs.Count > 0)
                SetActiveTabIndex(m_ActiveTabIndex >= m_Tabs.Count ? m_ActiveTabIndex - 1 : m_ActiveTabIndex);
        }

        public void SetActiveTabIndex(int index)
        {
            SetActiveTab(m_Tabs[index]);
        }

        public void SetActiveTab(DockingPanelTab tab)
        {
            m_Tabs[m_ActiveTabIndex].TabView.SetActiveStyle(false);
            tab.TabView.SetActiveStyle(true);

            m_ActiveTabIndex = m_Tabs.IndexOf(tab);
            PanelView.Container.Content = tab.Content;
        }
    }

    public class DockingPanelDescriptor
    {
        public UIElement Container { get; set; }
        public DockingPanel RelativePanel { get; set; }
        public DockingLayoutManager.DockPosition RelativePosition { get; set; }
        public DockingPanelTab Tab { get; set; }
    }

    public class DockingPanelTab
    {
        public string Title { get; set; }
        public UIElement Content { get; set; }
        public DockingPanel Parent { get; set; }
        public DockingTabView TabView { get; set; }

        public void Select()
        {
            Parent.SetActiveTab(this);
        }
    }
}
