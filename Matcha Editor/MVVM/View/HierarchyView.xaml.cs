using Matcha_Editor.Core;
using Matcha_Editor.Core.IPC;
using Matcha_Editor.Core.IPC.Command;
using Matcha_Editor.MVVM.Model;
using Matcha_Editor.MVVM.ViewModel;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Matcha_Editor.MVVM.View
{
    public partial class HierarchyView : ViewBase
    {
        readonly HierarchyTreeViewModel m_TreeViewModel;

        public HierarchyView()
        {
            InitializeComponent();

            HierarchyNode rootnode = new HierarchyNode { Name = "Default World" };
            //rootnode.Children.Add(new HierarchyNode { Name = "Dummy Nested Entity" });

            //rootnode.Children[0].Children.Add(new HierarchyNode { Name = "Deeply Nested Entity 1" });
            //rootnode.Children[0].Children.Add(new HierarchyNode { Name = "Deeply Nested Entity 2" });

            var result = new GetTopLevelEntitiesCommand.Response(IPCManager.Instance.Get(new GetTopLevelEntitiesCommand()));
            foreach (var entity in result.ResponseData.Args.Entities)
                rootnode.Children.Add(new HierarchyNode { 
                    Name = entity.Name, 
                    Guid = entity.Guid
                });

            m_TreeViewModel = new HierarchyTreeViewModel(rootnode);
            base.DataContext = m_TreeViewModel;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 5.0);
            e.Handled = true;
        }

        private void TreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            IList<UserControl> inspectors = EditorWindowManager.Instance.GetWindowsOfType<InspectorView>();
            foreach (UserControl inspector in inspectors)
            {
                HierarchyNodeViewModel selectedItem = e.NewValue as HierarchyNodeViewModel;
                InspectorView view = inspector as InspectorView;
                view.Show(selectedItem.Guid);
                view.ViewModel.SelectedItemName = selectedItem.Name;
                view.ViewModel.SelectedItemID = selectedItem.Guid;
                view.ViewModel.SelectedItemEnabled = selectedItem.Enabled;
            }
        }
    }
}
