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
            DockingPanel viewport = DockingLayoutRoot.TEMP_DockPanel(new ViewportView(), "World Viewer");
            DockingPanel inspectorPanel = DockingLayoutRoot.TEMP_DockPanel(new InspectorView(), "Inspector", viewport, DockingLayoutManager.DockPosition.Right);
            DockingPanel hierarchyPanel = DockingLayoutRoot.TEMP_DockPanel(new HierarchyView(), "Hierarchy", inspectorPanel, DockingLayoutManager.DockPosition.Bottom);
            DockingPanel contentBrowser = DockingLayoutRoot.TEMP_DockPanel(new ContentBrowserView(), "Content Browser", viewport, DockingLayoutManager.DockPosition.Bottom);
            SplashView.Instance.Close();
        }

        private void MenuItem_Window_Layout_Default_Click(object sender, RoutedEventArgs e)
        {
            DockingPanel viewport = DockingLayoutRoot.TEMP_DockPanel(new ViewportView(), "World Viewer");
            DockingPanel inspectorPanel = DockingLayoutRoot.TEMP_DockPanel(new InspectorView(), "Inspector", viewport, DockingLayoutManager.DockPosition.Right);
            DockingPanel hierarchyPanel = DockingLayoutRoot.TEMP_DockPanel(new HierarchyView(), "Hierarchy", inspectorPanel, DockingLayoutManager.DockPosition.Bottom);
            DockingPanel contentBrowser = DockingLayoutRoot.TEMP_DockPanel(new ContentBrowserView(), "Content Browser", viewport, DockingLayoutManager.DockPosition.Bottom);
        }

        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            WindowChrome.RefreshButtons();
        }
    }
}
