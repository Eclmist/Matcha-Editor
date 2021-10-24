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

        private bool m_ThresholdCleared;
        private double m_Threshold = 60;

        public DockingManager()
        {
            m_NodeToSplitterMap = new Dictionary<DockingNode, DockingSplitterView>();
        }

        public void OnMouseDown(MouseButton button, Point mouseScreenPoint, DockingPanel panel)
        {
            if (button == MouseButton.Middle)
                ClosePanel(panel);
            if (button == MouseButton.Left)
                StartDrag(panel, mouseScreenPoint);
        }

        public DockingPanel AddPanel(DockingPanelDescriptor desc)
        {
            DockingPanel panel = new DockingPanel();
            panel.Title = desc.Title;
            panel.Node = DockingLayoutManager.Instance.AddNode(desc.Container, desc.Parent.Node, desc.Position);
            panel.PanelView = new DockingPanelView(panel.Node);
            panel.PanelView.Tab.TitleText = desc.Title;
            panel.PanelView.Container.Content = desc.Content;
            panel.PanelView.Tab.PreviewMouseDown += (s, e) => OnMouseDown(e.ChangedButton, e.GetScreenPoint(s), panel);
            panel.ContainerView = desc.Container as DockingContainerView;
            panel.Content = desc.Content;

            CreateSplitter(panel);

            if (panel.PanelView != null)
                panel.ContainerView.LayoutRoot.Children.Add(panel.PanelView);

            return panel;
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

        private void StartDrag(DockingPanel panel, Point mouseScreenPoint)
        {
            Debug.Assert(m_PreviewWindow == null);
            m_PreviewWindow = CreatePreviewWindow(panel);
            m_PreviewWindow.ShowAsWindow(mouseScreenPoint);
        }

        private void ContinueDrag(DockingPanel panel, Point mouseScreenPoint)
        {
            Debug.Assert(m_PreviewWindow != null);
            m_PreviewWindow.CaptureMouse();
            m_ThresholdCleared = (mouseScreenPoint - panel.PanelView.PointToScreen(new Point())).Length > m_Threshold;
            m_PreviewWindow.Opacity = m_ThresholdCleared ? 0.9 : 0;

            DockingContainerView topmostContainer = GetTopmostContainer(mouseScreenPoint);
            if (topmostContainer != null)
            {
                DockingNode subzoneNode = GetSubzone(topmostContainer, mouseScreenPoint);
                if (subzoneNode != null && subzoneNode.Parent != panel.Node)
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

        private void EndDrag(DockingPanel panel, Point mouseScreenPoint)
        {
            Debug.Assert(m_PreviewWindow != null);

            if (m_ThresholdCleared)
            {
                if (m_TargetNode == null)
                    DetachIntoExternalWindow(panel, mouseScreenPoint);
                else
                    DockIntoPanel(panel, m_TargetNode);
            }

            Reset();
        }

        private void Reset()
        {
            if (m_PreviewWindow != null)
                m_PreviewWindow.Close();

            m_PreviewWindow = null;
            m_TargetNode = null;
            m_ThresholdCleared = false;
        }

        private void DockIntoPanel(DockingPanel panel, DockingNode targetNode)
        {
            DockingContainerView targetContainer = DockingLayoutManager.Instance.GetRootContainer(targetNode) as DockingContainerView;
            ClosePanel(panel);
            targetContainer.TEMP_DockPanel(panel.Content, panel.Title, new DockingPanel{Node = targetNode.Parent}, m_TargetPosition);
        }

        private void DetachIntoExternalWindow(DockingPanel panel, Point screenPoint)
        {
            DockingExternalWindowView newWindow = new DockingExternalWindowView();
            newWindow.ShowInTaskbar = false;
            newWindow.Focusable = false;
            newWindow.Owner = App.Current.MainWindow;
            newWindow.Left = screenPoint.X - panel.Node.Width / 2;
            newWindow.Top = screenPoint.Y - panel.Node.Height / 2;
            newWindow.Width = panel.Node.Width;
            newWindow.Height = panel.Node.Height;
            newWindow.Show();
            newWindow.DockingContainer.TEMP_DockPanel(panel.Content, panel.Title);

            ClosePanel(panel);
        }

        private DockingPreviewWindowView CreatePreviewWindow(DockingPanel panel)
        {
            DockingPreviewWindowView window = new DockingPreviewWindowView();
            window.ShowActivated = false;
            window.Focusable = false;
            window.ShowInTaskbar = false;
            window.IsEnabled = true;
            window.Owner = App.Current.MainWindow;
            window.Tab.TitleText = panel.Title;
            window.PreviewMouseMove += (s, e) => ContinueDrag(panel, e.GetScreenPoint(s));
            window.PreviewMouseUp += (s, e) => { if (e.ChangedButton == MouseButton.Left) EndDrag(panel, e.GetScreenPoint(s)); };
            return window;
        }

        DockingContainerView GetTopmostContainer(Point mouseScreenPos)
        {

            foreach (var dockingContainer in DockingLayoutManager.Instance.GetRootContainers())
            {
                Point localPosition = dockingContainer.PointFromScreen(mouseScreenPos);
                var visual = VisualTreeHelper.HitTest(dockingContainer, localPosition)?.VisualHit;

                while (visual != null && !(visual is DockingContainerView))
                    visual = VisualTreeHelper.GetParent(visual);

                return visual as DockingContainerView;
            }
            return null;
        }

        private DockingNode GetSubzone(UIElement container, Point mouseScreenPos)
        {
            Point mouseLocalPosition = container.PointFromScreen(mouseScreenPos);
            return DockingLayoutManager.Instance.GetPreviewNode(mouseLocalPosition, container);
        }
    }
}
