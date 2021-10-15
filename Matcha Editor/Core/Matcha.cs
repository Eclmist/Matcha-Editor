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
                ExitWithError("Unable to establish connection with the engine", "Connection Error");
                return false;
            }

            return true;
        }

        private void ExitWithError(string error, string caption)
        {
            MessageBox.Show(error, caption, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            App.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }
    }
}
