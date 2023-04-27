using System;
using System.Globalization;
using GD_CommonLibrary;

namespace GD_StampingMachine.ValueConverters
{
    public class ObjectIsEqualToAutoOrNaNValueConverter : BaseValueConverter
    {
        public bool Invert { get; set; }
         public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

             if (object.Equals(value, parameter))
            {
                //auto
                return Double.NaN;
            }
            else
            {
                return null;
            }
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }

    }
}
