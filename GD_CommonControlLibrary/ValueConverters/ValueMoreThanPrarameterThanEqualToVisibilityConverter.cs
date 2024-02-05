using System;
using System.Globalization;
using System.Windows;


namespace GD_CommonControlLibrary
{
    public class ValueMoreThanPrarameterThanEqualToVisibilityConverter : BaseValueConverter
    {
        public bool Invert { get; set; }
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter != null)
            {
                if (int.TryParse(value.ToString(), out var Value_Int) && int.TryParse(parameter.ToString(), out var Parameter_Int))
                {
                    if (Value_Int >= Parameter_Int)
                    {
                        return (!Invert) ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                    {
                        return (Invert) ? Visibility.Visible : Visibility.Collapsed;
                    }


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
