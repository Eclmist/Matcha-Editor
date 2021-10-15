using Matcha_Editor.MVVM.Model;
using Matcha_Editor.MVVM.ViewModel;
using System.Windows.Controls;

namespace Matcha_Editor.MVVM.View
{
    public partial class HierarchyView : UserControl
    {
        readonly HierarchyTreeViewModel m_TreeViewModel;

        public HierarchyView()
        {
            InitializeComponent();

            HierarchyNode rootnode = new HierarchyNode { Name = "Dummy Nested Entity" };
            rootnode.Children.Add(new HierarchyNode { Name = "Nested Entity 1" });
            rootnode.Children.Add(new HierarchyNode { Name = "Nested Entity 2" });
            rootnode.Children.Add(new HierarchyNode { Name = "Nested Entity 3" });

            rootnode.Children[2].Children.Add(new HierarchyNode { Name = "Deeply Nested Entity 1" });
            rootnode.Children[2].Children.Add(new HierarchyNode { Name = "Deeply Nested Entity 2" });

            m_TreeViewModel = new HierarchyTreeViewModel(rootnode);
            base.DataContext = m_TreeViewModel;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 5.0);
            e.Handled = true;
        }
    }
}
