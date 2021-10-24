using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Matcha_Editor.MVVM.View
{
    public partial class WindowChrome : UserControl
    {
        public WindowChrome()
        {
            InitializeComponent();
        }

        public void RefreshButtons()
        {
            MaximizeBtn.Visibility = Application.Current.MainWindow.WindowState == WindowState.Maximized
                ? Visibility.Collapsed
                : Visibility.Visible;
            MinimizeBtn.Visibility = Application.Current.MainWindow.WindowState == WindowState.Maximized
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void Btn_Minimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void Btn_Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
