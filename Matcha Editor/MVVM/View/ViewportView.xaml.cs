using Matcha_Editor.Core;
using Matcha_Editor.Core.IPC;
using Matcha_Editor.Core.IPC.Command;
using System.Windows.Controls;

namespace Matcha_Editor.MVVM.View
{
    public partial class ViewportView : ViewBase
    {
        public ViewportView()
        {
            InitializeComponent();
            EditorWindowManager.Instance.RegisterWindow(this);
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //TODO: MVVM This
            IPCManager.Instance.Post(new SetDrawModeCommand(((ComboBoxItem)e.AddedItems[0]).Content as string));
        }
    }
}
