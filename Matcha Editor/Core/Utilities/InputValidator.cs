using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Matcha_Editor.Core.Utilities
{
    public class InputValidator
    {
        public static void FloatingPointInputValidator(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox textbox = sender as TextBox;

            string input = textbox.Text.ToLower();
            // Only valid characters in floating point numbers are
            // +/-
            // .
            // 0-9
            // e
            // Remove all invalid characters
            input = Regex.Replace(input, @"[^+\-e.0-9]", "");

            // There can only be a single 'e' in the expression. Remove all but the first one
            input = Regex.Replace(input, @"(?<=e.*)e", "");

            var mantisaExpSplit = input.Split(new[] { 'e' }, 2);
            // Split the mantissa and exponent part of the string
            string mantissa = mantisaExpSplit[0];
            string exponent = (mantisaExpSplit.Length > 1 && mantisaExpSplit[1] != "") 
                ? "e" + mantisaExpSplit[1]
                : "";

            // Remove '.'s from exponent
            exponent = Regex.Replace(exponent, @"\.", "");

            // +/- can only immediately follow e (e+10, e-10). Remove all other occurances of +/-
            exponent = Regex.Replace(exponent, @"(?<!e)[+-]", "");


            // Remove 'e's from mantissa
            mantissa = Regex.Replace(mantissa, @"e", "");

            // There can only be a single '.' in the expression. Remove all but the first one
            mantissa = Regex.Replace(mantissa, @"(?<=\..*)\.", "");

            // +/- can only exist at the start of the string. Remove all other occurances of +/-
            mantissa = Regex.Replace(mantissa, @"(?!^-)-", "");


            // Known bugs: "-", "+", "12e+" "12e-"
            // Needs to handle no-digit checks better
            // The following fixes above bugs, but (TODO) need to come back and refactor this better
            if (mantissa == "" || mantissa == "-" || mantissa == "+")
                mantissa = "0";

            if (exponent == "e-" || exponent == "e+")
                exponent = "e0";

            textbox.Text = mantissa + exponent;
        }
    }
}
