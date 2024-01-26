using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GD_CommonLibrary
{
    public class BooleanToVisibilityConverter: BaseValueConverter
    {
        public bool Invert { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;
            if (parameter is string && (string)parameter == "HiddenOnFalse")
            {
                visibility = Visibility.Hidden;
            }

            if (parameter is Visibility.Hidden)
            {
                visibility = Visibility.Hidden;
            }



            if (value == null)
            {
                return (!Invert) ? visibility : Visibility.Visible;
            }

            if ((bool)value)
            {
                return Invert ? visibility : Visibility.Visible;
            }

            return (!Invert) ? visibility : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            if (visibility == Visibility.Collapsed)
            {
                return Invert;
            }

            return !Invert;
        }
    }
}
