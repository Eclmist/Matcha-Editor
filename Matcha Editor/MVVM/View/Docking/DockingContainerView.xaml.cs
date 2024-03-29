﻿using Matcha_Editor.Core.Docking;
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
            DockingLayoutManager.Instance.Resize(e.NewSize, this);

            if (DockingLayoutManager.Instance.HasNodes(this))
            {
                Size minSize = DockingLayoutManager.Instance.GetRootNode(this).GetMinimumSize();
                Window.GetWindow(this).MinWidth = Math.Max(DockingNode.MinimumSize, minSize.Width + 16); //TODO: Remove padding hack
                Window.GetWindow(this).MinHeight = Math.Max(DockingNode.MinimumSize, minSize.Height + 59);
            }
        }

        public DockingPanel TEMP_DockPanel(
            UIElement content, string title, DockingPanel relativeTo = null,
            DockingLayoutManager.DockPosition relativePos = DockingLayoutManager.DockPosition.Left)
        {

            DockingPanelTab tab = new DockingPanelTab();
            tab.Content = content;
            tab.Title = title;

            return m_DockingManager.AddTab(tab, relativeTo, relativePos, this);
        }

        public void Close()
        {
            DockingNode rootNode = DockingLayoutManager.Instance.GetRootNode(this);
            DockingLayoutManager.Instance.RemoveNode(rootNode);
        }
    }
}
