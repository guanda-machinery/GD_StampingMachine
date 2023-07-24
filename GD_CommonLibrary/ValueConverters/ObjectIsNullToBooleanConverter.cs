using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using GD_CommonLibrary;

namespace GD_CommonLibrary.ValueConverters
{
    public class ObjectIsNullToVisibilityConverter : BaseValueConverter
    {
        public bool Invert { get; set; }

         public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility _visibility = Visibility.Collapsed;
            if (value == null)
            {
                return (Invert)? Visibility.Visible : _visibility;
            }

            if (value is string)
            {
                if (string.IsNullOrEmpty(value as string))
                {
                    return (Invert)? Visibility.Visible : _visibility;
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
            if(value is Visibility ValueVisibility)
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
