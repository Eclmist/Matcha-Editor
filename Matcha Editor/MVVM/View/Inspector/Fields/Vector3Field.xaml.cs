using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Matcha_Editor.Core.Utilities;

namespace Matcha_Editor.MVVM.View
{
    public partial class Vector3Field : UserControl
    {
        public Vector3Field()
        {
            InitializeComponent();

            TextBox_X.LostKeyboardFocus += InputValidator.FloatingPointInputValidator;
            TextBox_Y.LostKeyboardFocus += InputValidator.FloatingPointInputValidator;
            TextBox_Z.LostKeyboardFocus += InputValidator.FloatingPointInputValidator;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var parseOk = float.TryParse(e.Text, out _);
            e.Handled = parseOk;
        }
    }
}
