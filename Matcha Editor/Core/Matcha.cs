using Matcha_Editor.Core.IPC;
using Matcha_Editor.Core.IPC.Command;
using Matcha_Editor.MVVM.View;
using Matcha_Editor.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Matcha_Editor.Core
{
    class MatchaEditor
    {
        private static MatchaEditor m_Instance;

        public static MatchaEditor Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new MatchaEditor();

                return m_Instance;
            }
        }

        private MatchaEditor() { }

        public void PreInitialize()
        {
            if (!ConnectToEngine())
                return;

            CreateEditorView();
        }

        private void CreateEditorView()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ((SplashViewModel)SplashView.Instance.DataContext).Splash.Status = "Initializing engine...";
            });

            System.Threading.Thread.Sleep(500);
            App.Current.Dispatcher.Invoke(() =>
            {
                new EditorView();
            });
        }

        private bool ConnectToEngine()
        {
            App.Current.Dispatcher.Invoke(() => 
                ((SplashViewModel)SplashView.Instance.DataContext).Splash.Status = "Establishing connection with engine...");

            try
            {
                IPCManager.Instance.Initialize();
            }
            catch (Exception)
            {
                var res = MessageBox.Show(
                    "Unable to establish connection with the engine. Try again?", 
                    "Connection Error", 
                    MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No);

                if (res == MessageBoxResult.Yes) // Retry
                    return ConnectToEngine();

                App.Current.Dispatcher.Invoke(Application.Current.Shutdown);
                return false;
            }

            return true;
        }
    }
}
