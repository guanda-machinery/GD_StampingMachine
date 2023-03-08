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
    public class ObjectIsNullToBooleanConverter : BaseValueConverter
    {
        public bool Invert { get; set; }
         public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return (!Invert);
            }

            if (value is string)
            {
                if (string.IsNullOrEmpty(value as string))
                {
                    return (!Invert);
                }
                else
                {
                    return (Invert);
                }
            }

            return (!Invert);

        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }

       
    }
}
