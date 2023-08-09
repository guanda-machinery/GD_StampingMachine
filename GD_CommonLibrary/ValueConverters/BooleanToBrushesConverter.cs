﻿using GD_CommonLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GD_CommonLibrary
{
    public class BooleanToBrushesConverter : BaseValueConverter
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool BooleanValue)
            {
                if (BooleanValue)
                {
                    return System.Windows.Media.Brushes.Red;
                }
            }

            var Brush = Application.Current.TryFindResource("PrimaryHueDarkForegroundBrush");
            if(Brush is SolidColorBrush SolidColor)
            {
                return SolidColor;
            }

            return System.Windows.Media.Brushes.White;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}