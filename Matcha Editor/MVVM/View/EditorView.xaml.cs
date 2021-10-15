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

        private void InspectorView_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
