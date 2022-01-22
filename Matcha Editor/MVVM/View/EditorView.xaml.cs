using System;
using Matcha_Editor.Core;
using System.Threading.Tasks;
using System.Windows;
using Matcha_Editor.Core.Docking;

namespace Matcha_Editor.MVVM.View
{
    public partial class EditorView : Window
    {
        public EditorView()
        {
            InitializeComponent();
            Show();
            Application.Current.MainWindow = this;
            WindowChrome.RefreshButtons();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DockingLayoutRoot.IsPrimaryContainer = true;
            //LayoutRoot.AddPanel(new InspectorView(), "Inspector", new Point(1800, 200));
            //LayoutRoot.AddPanel(new ConsoleView(), "Debug Console", new Point(900, 800));
            //LayoutRoot.AddPanel(new HierarchyView(), "Hierarchy", new Point(100, 500));
            DockingPanel viewport = DockingLayoutRoot.TEMP_DockPanel(new ViewportView(), "World Viewer");
            //DockingPanel viewport = DockingLayoutRoot.TEMP_DockPanel(new InspectorView(), "Inspector");
            DockingPanel inspectorPanel = DockingLayoutRoot.TEMP_DockPanel(new InspectorView(), "Inspector", viewport, DockingLayoutManager.DockPosition.Right);
            DockingPanel hierarchyPanel = DockingLayoutRoot.TEMP_DockPanel(new HierarchyView(), "Hierarchy", viewport, DockingLayoutManager.DockPosition.Left);
            DockingPanel contentBrowser = DockingLayoutRoot.TEMP_DockPanel(new ContentBrowserView(), "Content Browser", viewport, DockingLayoutManager.DockPosition.Bottom);
            DockingLayoutRoot.TEMP_DockPanel(new InspectorView(), "Inspector", contentBrowser, DockingLayoutManager.DockPosition.Stacked);
            DockingLayoutRoot.TEMP_DockPanel(new HierarchyView(), "Hierarchy", contentBrowser, DockingLayoutManager.DockPosition.Stacked);
            SplashView.Instance.Close();
        }

        private void MenuItem_Window_Layout_Default_Click(object sender, RoutedEventArgs e)
        {
            //DockingPanel viewport = DockingLayoutRoot.TEMP_DockPanel(new ViewportView(), "World Viewer");
            DockingPanel viewport = DockingLayoutRoot.TEMP_DockPanel(new InspectorView(), "Inspector");
            DockingPanel inspectorPanel = DockingLayoutRoot.TEMP_DockPanel(new InspectorView(), "Inspector", viewport, DockingLayoutManager.DockPosition.Right);
            DockingPanel hierarchyPanel = DockingLayoutRoot.TEMP_DockPanel(new HierarchyView(), "Hierarchy", viewport, DockingLayoutManager.DockPosition.Left);
            DockingPanel contentBrowser = DockingLayoutRoot.TEMP_DockPanel(new ContentBrowserView(), "Content Browser", viewport, DockingLayoutManager.DockPosition.Bottom);
            //DockingPanel debugPanel = DockingLayoutRoot.DockPanel(new ConsoleView(), "Debug Console", inspectorPanel);
            //DockingLayoutRoot.DockPanel(new HierarchyView(), "Hierarchy", debugPanel);
        }

        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            WindowChrome.RefreshButtons();
        }
    }
}
