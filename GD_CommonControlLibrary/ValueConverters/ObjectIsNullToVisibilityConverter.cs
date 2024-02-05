using System;
using System.Globalization;

namespace GD_CommonControlLibrary
{
    public class ObjectIsNullToBooleanConverter : BaseValueConverter
    {
        public bool Invert { get; set; }
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
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

            return (Invert);

        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is false)
            {
                return null;
            }


            return parameter;
        }


    }
}
