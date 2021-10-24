using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Matcha_Editor.Core;

namespace Matcha_Editor.MVVM.View
{
    public abstract class ViewBase : UserControl
    {
        public ViewBase()
        {
            EditorWindowManager.Instance.RegisterWindow(this);
        }

        ~ViewBase()
        {
            EditorWindowManager.Instance.DeregisterWindow(this);
        }
    }
}
