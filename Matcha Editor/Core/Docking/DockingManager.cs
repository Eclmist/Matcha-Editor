using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Matcha_Editor.Core.Docking;
using Matcha_Editor.Core.Utilities;
using Matcha_Editor.MVVM.View;

namespace Matcha_Editor.CoreDocking
{
    public class DockingManager
    {
        private Dictionary<DockingNode, DockingSplitterView> m_NodeToSplitterMap;
        private DockingPreviewWindowView m_PreviewWindow;
        private DockingNode m_TargetNode;
        private DockingLayoutManager.DockPosition m_TargetPosition;


        #region User Controls
        private Point m_TabDragStartPoint;

        private bool m_TabReordered;
        private bool m_TabDetached;

        private double m_TabReorderThreshold = 30;
        private double m_TabDetachThreshold = 30;
        #endregion

        public DockingManager()
        {
            m_NodeToSplitterMap = new Dictionary<DockingNode, DockingSplitterView>();
        }

        public void OnMouseDown(MouseButton button, Point mouseScreenPoint, DockingPanelTab tab)
        {
            if (button == MouseButton.Middle)
                CloseTab(tab);
            if (button == MouseButton.Left)
                StartDrag(tab, mouseScreenPoint);
        }

        public DockingPanel AddTab(DockingPanelTab tab, DockingPanel relativeTo, DockingLayoutManager.DockPosition relativePos, UIElement rootContainer)
        {
            if (relativePos == DockingLayoutManager.DockPosition.Stacked)
            {
                Debug.Assert(relativeTo != null);
                relativeTo.AddTab(tab);
                DockingTabView tabView = relativeTo.PanelView.AddTab(tab);
                tabView.PreviewMouseDown += (s, e) => OnMouseDown(e.ChangedButton, e.GetScreenPoint(s), tab);
                return relativeTo;
            }

            DockingPanelDescriptor desc = new DockingPanelDescriptor();
            desc.Container = rootContainer;
            desc.RelativePanel = relativeTo ?? new DockingPanel();
            desc.RelativePosition = relativePos;
            desc.Tab = tab;
            return AddPanel(desc);
        }

        public void Reset()
        {
            if (m_PreviewWindow != null)
                m_PreviewWindow.Close();

            m_PreviewWindow = null;
            m_TargetNode = null;
            m_TabDetached = false;
        }

        private DockingPanel AddPanel(DockingPanelDescriptor desc)
        {
            DockingPanel panel = new DockingPanel();
            panel.Node = DockingLayoutManager.Instance.AddNode(desc.Container, desc.RelativePanel.Node, desc.RelativePosition);
            panel.PanelView = new DockingPanelView(panel.Node);
            panel.PanelView.Container.Content = desc.Tab.Content;
            panel.ContainerView = desc.Container as DockingContainerView;

            panel.AddTab(desc.Tab);
            DockingTabView tabView = panel.PanelView.AddTab(desc.Tab);
            tabView.PreviewMouseDown += (s, e) => OnMouseDown(e.ChangedButton, e.GetScreenPoint(s), desc.Tab);

            CreateSplitter(panel);

            if (panel.PanelView != null)
                panel.ContainerView.LayoutRoot.Children.Add(panel.PanelView);

            return panel;
        }

        private void CloseTab(DockingPanelTab tab)
        {
            tab.Parent.PanelView.RemoveTab(tab);
            tab.Parent.RemoveTab(tab);

            if (tab.Parent.GetNumTabs() == 0)
                ClosePanel(tab.Parent);
        }

        private void ClosePanel(DockingPanel panel)
        {
            DockingLayoutManager.Instance.RemoveNode(panel.Node);
            DestroySplitter(panel);
            CloseWindowIfEmpty(panel);
            panel.ContainerView.LayoutRoot.Children.Remove(panel.PanelView);
        }

        private void CloseWindowIfEmpty(DockingPanel panel)
        {
            if (!DockingLayoutManager.Instance.HasNodes(panel.ContainerView))
                if (!panel.ContainerView.IsPrimaryContainer)
                    Window.GetWindow(panel.ContainerView)?.Close();
        }

        private void CreateSplitter(DockingPanel panel)
        {
            if (panel.Node.IsAncestor())
                return;

            DockingSplitterView splitter = new DockingSplitterView(panel.Node.Parent);
            m_NodeToSplitterMap[panel.Node.Parent] = splitter;
            panel.ContainerView.LayoutRoot.Children.Add(splitter);
        }

        private void DestroySplitter(DockingPanel panel)
        {
            if (panel.Node.IsAncestor()) 
                return;

            panel.ContainerView.LayoutRoot.Children.Remove(m_NodeToSplitterMap[panel.Node.Parent]);
            m_NodeToSplitterMap.Remove(panel.Node.Parent);
        }

        private void StartDrag(DockingPanelTab tab, Point mouseScreenPoint)
        {
            Debug.Assert(m_PreviewWindow == null);
            m_TabDragStartPoint = mouseScreenPoint;
            m_PreviewWindow = CreatePreviewWindow(tab);
            m_PreviewWindow.Opacity = 0;
            m_PreviewWindow.ShowAsTab(mouseScreenPoint - (m_TabDragStartPoint - tab.TabView.PointToScreen(new Point())));
            m_PreviewWindow.CaptureMouse();
        }

        private void ContinueDrag(DockingPanelTab tab, Point mouseScreenPoint)
        {
            Debug.Assert(m_PreviewWindow != null);

            Vector dragDelta = mouseScreenPoint - m_TabDragStartPoint;

            m_TabDetached = Math.Abs(dragDelta.Y) > m_TabDetachThreshold;
            m_TabReordered = dragDelta.Length > m_TabReorderThreshold;

            if (!m_TabReordered && !m_TabDetached)
            {
                tab.TabView.Opacity = 1;
                m_PreviewWindow.Opacity = 0;
                return;
            }

            m_PreviewWindow.Opacity = 0.9;
            tab.TabView.Opacity = 0;

            if (m_TabDetached)
            {
                DockingContainerView topmostContainer = GetTopmostContainer(mouseScreenPoint);
                if (topmostContainer != null)
                {
                    DockingNode subzoneNode = GetSubzone(topmostContainer, mouseScreenPoint);
                    if (subzoneNode != null && subzoneNode.Parent != tab.Parent.Node)
                    {
                        m_PreviewWindow.ShowAsWindow(topmostContainer.PointToScreen(subzoneNode.Rect.TopLeft), subzoneNode.Rect.Size);
                        m_TargetNode = subzoneNode;
                        if (subzoneNode.Rect == subzoneNode.Parent.GetTopSubzone().Rect)
                            m_TargetPosition = DockingLayoutManager.DockPosition.Top;
                        if (subzoneNode.Rect == subzoneNode.Parent.GetRightSubzone().Rect)
                            m_TargetPosition = DockingLayoutManager.DockPosition.Right;
                        if (subzoneNode.Rect == subzoneNode.Parent.GetBottomSubzone().Rect)
                            m_TargetPosition = DockingLayoutManager.DockPosition.Bottom;
                        if (subzoneNode.Rect == subzoneNode.Parent.GetLeftSubzone().Rect)
                            m_TargetPosition = DockingLayoutManager.DockPosition.Left;

                        return;
                    }
                }

                m_TargetNode = null;
                m_PreviewWindow.ShowAsWindow(mouseScreenPoint);
            }
            else
            {
                //m_PreviewWindow.ShowAsTab(mouseScreenPoint - (m_TabDragStartPoint - tab.TabView.PointToScreen(new Point())));
                Point pos = tab.TabView.PointToScreen(new Point());
                pos.X += dragDelta.X;
                m_PreviewWindow.ShowAsTab(pos);
            }

        }

        private void EndDrag(DockingPanelTab tab, Point mouseScreenPoint)
        {
            Debug.Assert(m_PreviewWindow != null);
            tab.TabView.Opacity = 1;

            if (!m_TabReordered)
                tab.Select();

            if (m_TabDetached)
            {
                if (m_TargetNode == null)
                    DetachIntoExternalWindow(tab, mouseScreenPoint);
                else
                    DockIntoPanel(tab, m_TargetNode);
            }

            Reset();
        }

        private void DockIntoPanel(DockingPanelTab tab, DockingNode targetNode)
        {
            CloseTab(tab);
            DockingContainerView targetContainer = DockingLayoutManager.Instance.GetRootContainer(targetNode) as DockingContainerView;
            targetContainer.TEMP_DockPanel(tab.Content, tab.Title, new DockingPanel{Node = targetNode.Parent}, m_TargetPosition);
        }

        private void DetachIntoExternalWindow(DockingPanelTab tab, Point screenPoint)
        {
            CloseTab(tab);
            DockingExternalWindowView newWindow = new DockingExternalWindowView();
            newWindow.ShowInTaskbar = false;
            newWindow.Focusable = false;
            newWindow.Owner = App.Current.MainWindow;
            newWindow.Left = screenPoint.X - DockingPreviewWindowView.DefaultWidth / 2;
            newWindow.Top = screenPoint.Y - DockingPreviewWindowView.DefaultHeight / 2;
            newWindow.Width = tab.Parent.Node.Width;
            newWindow.Height = tab.Parent.Node.Height;
            newWindow.Show();
            newWindow.DockingContainer.TEMP_DockPanel(tab.Content, tab.Title);
        }

        private DockingPreviewWindowView CreatePreviewWindow(DockingPanelTab tab)
        {
            DockingPreviewWindowView window = new DockingPreviewWindowView();
            window.ShowActivated = false;
            window.Focusable = false;
            window.ShowInTaskbar = false;
            window.IsEnabled = true;
            window.Owner = App.Current.MainWindow;
            window.Tab.TitleText = tab.Title;
            window.PreviewMouseMove += (s, e) => { if (e.LeftButton == MouseButtonState.Pressed) ContinueDrag(tab, e.GetScreenPoint(s)); };
            window.PreviewMouseUp += (s, e) => { if (e.ChangedButton == MouseButton.Left) EndDrag(tab, e.GetScreenPoint(s)); };
            return window;
        }

        private DockingContainerView GetTopmostContainer(Point mouseScreenPos)
        {
            int topmostZ = 0;
            DockingContainerView topmostContainer = null;

            foreach (var dockingContainer in DockingLayoutManager.Instance.GetRootContainers())
            {
                Point localPosition = dockingContainer.PointFromScreen(mouseScreenPos);
                var visual = VisualTreeHelper.HitTest(dockingContainer, localPosition)?.VisualHit;

                while (visual != null && !(visual is DockingContainerView))
                    visual = VisualTreeHelper.GetParent(visual);

                if (visual != null)
                {
                    int zOrder = WindowsUtils.GetWindowZ(Window.GetWindow(visual));
                    if (zOrder > topmostZ)
                    {
                        topmostContainer = visual as DockingContainerView;
                        topmostZ = zOrder;
                    }
                }
            }

            return topmostContainer;
        }

        private DockingNode GetSubzone(UIElement container, Point mouseScreenPos)
        {
            Point mouseLocalPosition = container.PointFromScreen(mouseScreenPos);
            return DockingLayoutManager.Instance.GetPreviewNode(mouseLocalPosition, container);
        }
    }
}
