using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace GD_StampingMachine.ValueConverters
{
    public class ObjectIsEqualToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public bool Invert { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (object.Equals(value, parameter))
            {
                return (!Invert) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return (!Invert) ? Visibility.Collapsed : Visibility.Visible;
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

    }
}
