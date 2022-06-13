using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Matcha_Editor.Core.Utilities;

namespace Matcha_Editor.MVVM.View
{
    public partial class Vector4Field : UserControl
    {
        public Vector4Field()
        {
            InitializeComponent();

            TextBox_X.LostKeyboardFocus += InputValidator.FloatingPointInputValidator;
            TextBox_Y.LostKeyboardFocus += InputValidator.FloatingPointInputValidator;
            TextBox_Z.LostKeyboardFocus += InputValidator.FloatingPointInputValidator;
            TextBox_W.LostKeyboardFocus += InputValidator.FloatingPointInputValidator;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
