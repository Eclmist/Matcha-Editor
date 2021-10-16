﻿using System;
using System.Collections.Generic;
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
    public partial class DockingTabView : UserControl
    {
        public DockingPanelView ParentPanel { get; set; }

        public DockingTabView()
        {
            InitializeComponent();
        }
        
        public void ToggleVisibility(bool visible)
        {
            this.Opacity = visible ? 1 : 0;
        }
    }
}
