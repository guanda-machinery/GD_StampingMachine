using System;
using System.Globalization;
using System.Windows;


namespace GD_CommonLibrary
{
    public class ObjectIsEqualToVisibilityConverter : BaseValueConverter
    {
        public bool Invert { get; set; }
        public bool IsNullable { get; set; }
        public bool IsTypeJudge { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (IsNullable && value is null)
            {
                if (object.Equals(value, null))
                {
                    return (!Invert) ? Visibility.Visible : Visibility.Collapsed;
                }
            }


            if (IsTypeJudge)
            {
                if (value != null)
                {
                    if (Object.ReferenceEquals(value, parameter))
                    {
                        return (!Invert) ? Visibility.Visible : Visibility.Collapsed;
                    }


                    if (value.GetType() == parameter as Type)
                    {
                        return (!Invert) ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                    {
                        return (Invert) ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }


            if (object.Equals(value, parameter))
            {
                return (!Invert) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return (Invert) ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}
