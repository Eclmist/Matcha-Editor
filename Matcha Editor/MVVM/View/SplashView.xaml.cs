using Matcha_Editor.Core;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Matcha_Editor.MVVM.View
{
    public partial class SplashView : Window
    {
        public static SplashView Instance;

        public SplashView()
        {
            Instance = this;
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            Task.Run(MatchaEditor.Instance.PreInitialize);
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
        }
    }
}
