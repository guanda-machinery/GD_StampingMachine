﻿using System;
using System.Globalization;
using System.Windows;

namespace GD_CommonControlLibrary
{
    public class ObjectIsNullToVisibilityConverter : BaseValueConverter
    {
        public bool Invert { get; set; }

        public bool IsHidden { get; set; }


        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility _visibility = IsHidden? Visibility.Hidden:Visibility.Collapsed;
            if (value == null)
            {
                return (Invert) ? Visibility.Visible : _visibility;
            }

            if (value is string)
            {
                if (string.IsNullOrEmpty(value as string))
                {
                    return (Invert) ? Visibility.Visible : _visibility;
                }
                else
                {
                    return (!Invert) ? Visibility.Visible : _visibility;
                }
            }

            return (!Invert) ? Visibility.Visible : _visibility;

        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility ValueVisibility)
            {
                if (ValueVisibility is Visibility.Visible)
                    return (Invert);
                else
                    return (!Invert);
            }
            throw new Exception();
        }


    }
}
