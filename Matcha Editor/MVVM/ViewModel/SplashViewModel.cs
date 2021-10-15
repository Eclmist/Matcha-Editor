using Matcha_Editor.MVVM.Model;
using System;
using System.Configuration;
using System.Timers;
using System.Windows;

namespace Matcha_Editor.MVVM.ViewModel
{
    public class SplashViewModel
    {
        public SplashModel Splash { get; set; }

        public SplashViewModel()
        {
            Splash = new SplashModel
            {
                Version = ConfigurationManager.AppSettings.Get("EditorVersion"),
                Status = "Starting Editor"
            };
        }
    }
}
