using System;
using System.Globalization;

namespace GD_CommonControlLibrary
{
    public class ObjectIsEqualToBooleanConverter : BaseValueConverter
    {

        public bool IsTypeJudge { get; set; }
        public bool Invert { get; set; }
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (IsTypeJudge)
            {
                if (Object.ReferenceEquals(value, parameter))
                {
                    return (!Invert);
                }

                if (value != null)
                {
                    if (value.GetType() == parameter as Type)
                    {
                        return (!Invert);
                    }
                }
                else
                {
                    return (Invert);
                }
            }

            if (object.Equals(value, parameter))
            {
                return !Invert;
            }
            else
            {
                return Invert;
            }





        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }

    }
}
