using Matcha_Editor.Core.Docking;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using Matcha_Editor.Core;

namespace Matcha_Editor.MVVM.View
{
    public partial class DockingContainerView : UserControl
    {
        private DockingLayoutManager m_LayoutManager;

        private Dictionary<DockingNode, DockingSplitterView> m_NodeToSplitterMap;

        public DockingContainerView()
        {
            InitializeComponent();
        }

        private void AddDefaultPanels()
        {
            AddPanel(new ViewportView(), "World Viewer");
            AddPanel(new HierarchyView(), "Hierarchy");
            AddPanel(new InspectorView(), "Inspector");
            AddPanel(new ConsoleView(), "DebugConsole");
        }

        private void AddPanel<T>(T contentView, string tabtitle) where T : UserControl
        {
            EditorWindowManager.Instance.RegisterWindow(contentView);
            DockingNode dockingNode = m_LayoutManager.AddNewNode();
            DockingPanelView panelView = new DockingPanelView(dockingNode);
            panelView.Tab.TitleText = tabtitle;
            panelView.Tab.PreviewMouseDown += OnTabMouseDown;
            panelView.Container.Content = contentView;
            LayoutRoot.Children.Add(panelView);
            CreateSplitter(dockingNode);
        }

        private void CreateSplitter(DockingNode node)
        {
            if (node.IsAncestor())
                return;

            DockingSplitterView splitter = new DockingSplitterView(node.Parent);
            m_NodeToSplitterMap[node.Parent] = splitter;
            LayoutRoot.Children.Add(splitter);
        }

        private void DestroySplitter(DockingNode node)
        {
            if (node.IsAncestor())
                return;

            LayoutRoot.Children.Remove(m_NodeToSplitterMap[node.Parent]);
            m_NodeToSplitterMap.Remove(node.Parent);
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            m_LayoutManager = new DockingLayoutManager(new Size(ActualWidth, ActualHeight));
            m_NodeToSplitterMap = new Dictionary<DockingNode, DockingSplitterView>();

            AddDefaultPanels();
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_LayoutManager?.Resize(e.NewSize);
        }

        #region Drag-docking of Panels
        private DockingPreviewWindowView m_PreviewWindow;
        private DockingTabView m_SelectedTab;
        private Point m_TabClickStartPoint;
        private DockingNode m_PreviewNode;
        private DockingNode m_SelectedNode;
        private double m_DetachThreshold = 60;
        private double m_SlidingThreshold = 40;
        private bool m_SlidingThresholdCleared;
        private bool m_DetachThresholdCleared;

        private void OnTabMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed && m_SelectedTab == null)
            {
                DockingTabView tabToRemove = (DockingTabView) sender;
                DockingNode nodeToRemove = tabToRemove.ParentPanel.DockingNode;

                m_LayoutManager.RemoveNode(nodeToRemove);
                LayoutRoot.Children.Remove(tabToRemove.ParentPanel);
                EditorWindowManager.Instance.DeregisterWindow(tabToRemove.ParentPanel.Content as UserControl);

                DestroySplitter(nodeToRemove);
                return;
            }

            m_SelectedTab = sender as DockingTabView;
            m_SelectedTab.CaptureMouse();
            m_SelectedNode = m_SelectedTab.ParentPanel.DockingNode;
            m_TabClickStartPoint = e.GetPosition(this.LayoutRoot);
        }

        private void LayoutRoot_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (m_SelectedTab == null)
                return;

            Point position = e.GetPosition(sender as IInputElement);

            if ((position - m_TabClickStartPoint).Length > m_SlidingThreshold)
            {
                m_SlidingThresholdCleared = true;
                m_SelectedTab.Margin = new Thickness(position.X - m_TabClickStartPoint.X, 0,0,0);
            }
            else
            {
                m_SlidingThresholdCleared = false;
                m_SelectedTab.Margin = new Thickness(0);
            }

            if (Math.Abs(position.Y - m_TabClickStartPoint.Y) > m_DetachThreshold ||
                m_SelectedTab.Margin.Left < 0 || // TODO: Remove this hack! We should be looking for the position of the tab instead.
                m_SelectedTab.Margin.Left + 100 > m_SelectedTab.ParentPanel.ActualWidth)
            {
                m_DetachThresholdCleared = true;
                m_SelectedTab.ToggleVisibility(false);
                ShowDockingPreview(position);
            }
            else
            {
                m_DetachThresholdCleared = false;
                m_SelectedTab.ToggleVisibility(true);
                CloseDockingPreview();
            }
        }

        public void LayoutRoot_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(sender as IInputElement);

            if (m_DetachThresholdCleared && m_PreviewNode != null)
            {
                // Can't dock to itself. Doesn't make sense to, and it breaks the tree logic
                if (m_PreviewNode.Parent != m_SelectedNode)
                {
                    Debug.Assert(m_SelectedNode != null);
                    DestroySplitter(m_SelectedNode);
                    m_LayoutManager.MoveNode(m_SelectedNode, m_PreviewNode);
                    CreateSplitter(m_SelectedNode);
                }
            }

            // if (m_DetachThresholdCleared) HandleDetachWindow
            // if (m_SlidingThresholdCleared) HandleRearrangeTab

            AbortDockingPreview();
        }

        public void AbortDockingPreview()
        {
            if (m_SelectedTab == null)
                return;

            Debug.WriteLine("Released");
            m_SelectedTab.ReleaseMouseCapture();
            m_SelectedTab.ToggleVisibility(true);
            m_SelectedTab.Margin = new Thickness(0);
            m_SelectedTab = null;

            m_SelectedNode = null;

            CloseDockingPreview();

            m_SlidingThresholdCleared = false;
            m_DetachThresholdCleared = false;
        }

        private void CreatePreviewWindow()
        {
            m_PreviewWindow = new DockingPreviewWindowView();
            m_PreviewWindow.ShowActivated = false;
            m_PreviewWindow.Focusable = false;
            m_PreviewWindow.ShowInTaskbar = false;
            m_PreviewWindow.IsEnabled = false;
            m_PreviewWindow.Owner = App.Current.MainWindow;
            m_PreviewWindow.Tab.TitleText = m_SelectedTab.TitleText;
        }

        private void CloseDockingPreview()
        {
            if (m_PreviewWindow != null)
            {
                m_PreviewWindow.Close();
                m_PreviewWindow = null;
            }
        }

        private void ShowCursorPreview(Point mouseLocalPosition)
        {
            Point mouseScreenPos = LayoutRoot.PointToScreen(mouseLocalPosition);
            m_PreviewWindow.Left = mouseScreenPos.X - (m_PreviewWindow.Width / 2);
            m_PreviewWindow.Top = mouseScreenPos.Y - (m_PreviewWindow.Height / 2);
            m_PreviewWindow.Width = 200;
            m_PreviewWindow.Height = 160;
            m_PreviewWindow.Tab.Title = m_SelectedTab.Title;
            m_PreviewWindow.Show();
        }

        private void ShowSubzonePreview(DockingNode previewSubzone)
        {
            Point previewNodeScreenPos = LayoutRoot.PointToScreen(previewSubzone.Rect.TopLeft);
            m_PreviewWindow.Left = previewNodeScreenPos.X;
            m_PreviewWindow.Top = previewNodeScreenPos.Y;
            m_PreviewWindow.Width = previewSubzone.Rect.Width;
            m_PreviewWindow.Height = previewSubzone.Rect.Height;
            m_PreviewWindow.Tab.Title = m_SelectedTab.Title;
            m_PreviewWindow.Show();
        }

        private void ShowDockingPreview(Point mouseLocalPosition)
        {
            if (m_PreviewWindow == null)
                CreatePreviewWindow();

            m_PreviewNode = m_LayoutManager.GetPreviewNode(mouseLocalPosition);

            if (m_PreviewNode == null || m_PreviewNode.Parent == m_SelectedNode)
            {
                ShowCursorPreview(mouseLocalPosition);
                return;
            }

            ShowSubzonePreview(m_PreviewNode);
        }

        #endregion
    }
}
