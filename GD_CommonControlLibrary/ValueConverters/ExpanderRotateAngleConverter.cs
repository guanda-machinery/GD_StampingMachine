using System;
using System.Globalization;
using System.Windows.Controls;

namespace GD_CommonControlLibrary
{
    public class ExpanderRotateAngleConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double factor = 1.0;
            if (parameter is { } parameterValue)
            {
                if (!double.TryParse(parameterValue.ToString(), out factor))
                {
                    factor = 1.0;
                }
            }
            return value switch
            {
                ExpandDirection.Left => 90 * factor,
                ExpandDirection.Right => -90 * factor,
                _ => 0
            };
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
