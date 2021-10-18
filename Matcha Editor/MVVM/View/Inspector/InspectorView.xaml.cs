using Matcha_Editor.Core.IPC;
using Matcha_Editor.Core.IPC.Command;
using Matcha_Editor.MVVM.ViewModel;
using System.Windows.Controls;
using Matcha_Editor.MVVM.Model;

namespace Matcha_Editor.MVVM.View
{
    public partial class InspectorView : UserControl
    {

        public InspectorViewModel ViewModel { get; set; }

        public InspectorView()
        {
            InitializeComponent();
            ViewModel = new InspectorViewModel();
            Show("");
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 5.0);
            e.Handled = true;
        }

        public void Show(string guid)
        {
            if (guid == null) // TODO maybe do a GUID lookup instead?
            {
                HeaderBar.Visibility = System.Windows.Visibility.Collapsed;
                ViewModel.UpdateComponents(new ComponentModel[]{});
                return;
            }

            HeaderBar.Visibility = System.Windows.Visibility.Visible;

            GetComponentsCommand command = new GetComponentsCommand(guid);
            var response = new GetComponentsCommand.Response(IPCManager.Instance.Get(command));
            ViewModel.UpdateComponents(response.ResponseData.Args.Components);
            DataContext = ViewModel;
        }
    }
}
