using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System;
using Matcha_Editor.Core.Utilities;

namespace Matcha_Editor.MVVM.View
{
    public partial class FloatField : UserControl
    {
        public FloatField()
        {
            InitializeComponent();
            valueSlider.ApplyTemplate();
            Thumb thumb = ((Track)valueSlider.Template.FindName("PART_Track", valueSlider)).Thumb;
            thumb.MouseEnter += new MouseEventHandler(thumb_MouseEnter);

            Value.LostKeyboardFocus += InputValidator.FloatingPointInputValidator;
        }

        private void thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.MouseDevice.Captured == null)
            {
                MouseButtonEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left);
                args.RoutedEvent = MouseLeftButtonDownEvent;
                (sender as Thumb).RaiseEvent(args);
            }
        }
    }
}
