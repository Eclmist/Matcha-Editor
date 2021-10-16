using Matcha_Editor.Core;
using System.Threading.Tasks;
using System.Windows;

namespace Matcha_Editor.MVVM.View
{
    public partial class EditorView : Window
    {
        public EditorView()
        {
            InitializeComponent();
            Show();
            Application.Current.MainWindow = this;
            SplashView.Instance.Close();
        }

        private void Window_Deactivated(object sender, System.EventArgs e)
        {
            LayoutRoot.AbortDockingPreview();
        }
    }
}
