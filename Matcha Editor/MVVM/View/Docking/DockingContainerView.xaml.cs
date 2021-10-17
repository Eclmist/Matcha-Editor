using Matcha_Editor.Core.Docking;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System;

namespace Matcha_Editor.MVVM.View
{
    public partial class DockingContainerView : UserControl
    {
        private DockingLayoutManager m_LayoutManager;

        public DockingContainerView()
        {
            InitializeComponent();
        }

        private void AddDefaultPanels()
        {

            DockingPanelView hierarchyPanel = new DockingPanelView();
            hierarchyPanel.Tab.TitleText = "Hierarchy";
            hierarchyPanel.Tab.PreviewMouseDown += OnTabMouseDown;
            hierarchyPanel.Container.Content = new HierarchyView();
            LayoutRoot.Children.Add(hierarchyPanel);

            DockingPanelView viewportPanel = new DockingPanelView();
            viewportPanel.Tab.TitleText = "World Viewer";
            viewportPanel.Tab.PreviewMouseDown += OnTabMouseDown;
            viewportPanel.Container.Content = new ViewportView();
            LayoutRoot.Children.Add(viewportPanel);

            DockingPanelView componentPanel = new DockingPanelView();
            componentPanel.Tab.TitleText = "Inspector";
            componentPanel.Tab.PreviewMouseDown += OnTabMouseDown;
            componentPanel.Container.Content = new InspectorView();
            LayoutRoot.Children.Add(componentPanel);

            DockingPanelView consolePanel = new DockingPanelView();
            consolePanel.Tab.TitleText = "Debug Console";
            consolePanel.Tab.PreviewMouseDown += OnTabMouseDown;
            consolePanel.Container.Content = new ConsoleView();
            LayoutRoot.Children.Add(consolePanel);

            m_LayoutManager.AddNewNode(viewportPanel);
            m_LayoutManager.AddNewNode(hierarchyPanel);
            m_LayoutManager.AddNewNode(componentPanel);
            m_LayoutManager.AddNewNode(consolePanel);
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            m_LayoutManager = new DockingLayoutManager(new Size(ActualWidth, ActualHeight));
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
        private DockingNode m_TargetDockingNode;
        private DockingNode m_SelectedDockingNode;
        private double m_DetachThreshold = 60;
        private double m_SlidingThreshold = 40;
        private bool m_SlidingThresholdCleared;
        private bool m_DetachThresholdCleared;


        private void OnTabMouseDown(object sender, MouseButtonEventArgs e)
        {
            m_SelectedTab = sender as DockingTabView;
            m_SelectedDockingNode = m_LayoutManager.FindNode(m_SelectedTab.ParentPanel);

            //if (e.MiddleButton == MouseButtonState.Pressed)
            //{
            //    m_LayoutManager.RemoveNode(m_SelectedDockingNode);
            //    return;
            //}

            m_SelectedTab.CaptureMouse();
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
                m_SelectedTab.Margin.Left + 100 > m_SelectedTab.ParentPanel.ActualWidth
                )
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

            if (m_DetachThresholdCleared && m_TargetDockingNode != null)
            {
                // Can't dock to itself. Doesn't make sense to, and it breaks the tree logic
                if (m_TargetDockingNode.Parent != m_SelectedDockingNode)
                {
                    Debug.Assert(m_SelectedDockingNode != null);
                    m_LayoutManager.AddNewNode(m_SelectedTab.ParentPanel, position);
                    m_LayoutManager.RemoveNode(m_SelectedDockingNode);
                }
            }


            // if (m_DetachThresholdCleared) HandleDetachWindow
            // if (m_SlidingThresholdCleared) HandleRearrangeTab

            AbortDockingPreview();
        }

        public void AbortDockingPreview()
        {
            if (m_SelectedTab != null)
            {
                Debug.WriteLine("Released");
                m_SelectedTab.ReleaseMouseCapture();
                m_SelectedTab.ToggleVisibility(true);
                m_SelectedTab.Margin = new Thickness(0);
                m_SelectedTab = null;

                m_SelectedDockingNode = null;

                CloseDockingPreview();

                m_SlidingThresholdCleared = false;
                m_DetachThresholdCleared = false;
            }
        }

        private void CreateDockingWindow()
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
                CreateDockingWindow();

            m_TargetDockingNode = m_LayoutManager.GetPreviewNode(mouseLocalPosition);

            if (m_TargetDockingNode == null || m_TargetDockingNode.Parent == m_SelectedDockingNode)
            {
                ShowCursorPreview(mouseLocalPosition);
                return;
            }

            ShowSubzonePreview(m_TargetDockingNode);
        }

        #endregion

    }
}
