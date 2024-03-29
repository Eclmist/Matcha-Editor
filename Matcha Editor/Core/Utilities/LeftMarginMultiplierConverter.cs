﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Matcha_Editor.Core.Utilities
{

    public class LeftMarginMultiplierConverter : IValueConverter
    {
        public double Length { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TreeViewItem;
            if (item == null)
                return new Thickness(0);

            return new Thickness(Length * item.GetDepth(), 2, 0, 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
