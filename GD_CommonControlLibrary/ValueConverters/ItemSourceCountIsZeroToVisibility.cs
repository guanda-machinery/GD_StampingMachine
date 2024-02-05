using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace GD_CommonControlLibrary
{
    public class ItemSourceCountIsZeroToVisibility : BaseValueConverter
    {
        public bool Invert { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is IEnumerable<object> ValueArray)
                {
                    if (!Invert)
                        return ValueArray.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;
                    else
                        return ValueArray.Count() == 0 ? Visibility.Collapsed : Visibility.Visible;
                }

                if(value is int IntValue)
                {
                    if (!Invert)
                        return IntValue == 0 ? Visibility.Visible : Visibility.Collapsed;
                    else
                        return IntValue == 0 ? Visibility.Collapsed : Visibility.Visible;
                }


            }


            return Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
