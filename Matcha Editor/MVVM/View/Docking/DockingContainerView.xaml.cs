using Matcha_Editor.Core.Docking;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Matcha_Editor.Core;
using Matcha_Editor.CoreDocking;

namespace Matcha_Editor.MVVM.View
{
    public partial class DockingContainerView : UserControl
    {
        public bool IsPrimaryContainer { get; set; }
        private DockingManager m_DockingManager;

        public DockingContainerView()
        {
            InitializeComponent();
            m_DockingManager = new DockingManager();
        }

        private void DockingContainerView_OnLoaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Deactivated += (sender, args) => m_DockingManager.Reset();
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DockingNode rootNode = DockingLayoutManager.Instance.GetRootNode(this);
            if (rootNode == null)
                return;

            rootNode.Rect = new Rect(e.NewSize);
            rootNode.RecursiveResize();
            Size minSize = rootNode.GetMinimumSize();
            Window.GetWindow(this).MinWidth = Math.Max(DockingNode.MinimumSize, minSize.Width + 16); //TODO: Remove padding hack
            Window.GetWindow(this).MinHeight = Math.Max(DockingNode.MinimumSize, minSize.Height + 59);
        }

        public DockingPanel TEMP_DockPanel(
            UIElement content, string title, DockingPanel parent = null,
            DockingLayoutManager.DockPosition pos = DockingLayoutManager.DockPosition.Left)
        {
            DockingPanelDescriptor desc = new DockingPanelDescriptor();
            desc.Content = content;
            desc.Title = title;
            desc.Container = this;
            desc.Parent = parent ?? new DockingPanel();
            desc.Position = pos;

            DockingPanel panel = m_DockingManager.AddPanel(desc);

            return panel;
        }

        public void Close()
        {
            DockingNode rootNode = DockingLayoutManager.Instance.GetRootNode(this);
            DockingLayoutManager.Instance.RemoveRootNode(rootNode);
        }
    }
}
