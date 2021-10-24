using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class DockingTabView : UserControl
    {
        public DockingPanelView ParentPanel { get; set; }
        public string TitleText { get; set; }

        public DockingTabView()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        
        public void ToggleVisibility(bool visible)
        {
            this.Opacity = visible ? 1 : 0;
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
        }
    }
}
