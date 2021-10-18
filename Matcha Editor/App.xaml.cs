using Matcha_Editor.Core.IPC;
using Matcha_Editor.Core.IPC.Command;
using Matcha_Editor.MVVM.View;
using System;
using System.Windows;

namespace Matcha_Editor
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Resources.Add(SystemParameters.VerticalScrollBarWidthKey, 10.0);
            Resources.Add(SystemParameters.HorizontalScrollBarHeightKey, 10.0);
            SplashView splashScreen = new SplashView();
            splashScreen.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            IPCManager.Instance.Post(new DetachCommand());
        }
    }
}
