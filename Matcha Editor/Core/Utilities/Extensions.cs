using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Matcha_Editor.Core.Utilities
{
    public static class Extensions
    {
        public static Point GetScreenPoint(this MouseEventArgs args, object sender)
        {
            return ((UIElement)sender).PointToScreen(args.GetPosition((UIElement)sender));
        }
    }
}
