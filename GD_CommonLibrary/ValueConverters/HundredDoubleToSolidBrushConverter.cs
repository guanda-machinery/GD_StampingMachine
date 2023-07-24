using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using GD_CommonLibrary;

namespace GD_CommonLibrary.ValueConverters
{
    public class HundredDoubleToSolidBrushConverter : BaseValueConverter
    {
         public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if(double.TryParse(value.ToString() , out double result))
            {
                if (result >= 0 && result < 25)
                {
                //    return (SolidColorBrush)new BrushConverter().ConvertFrom(Brushes.OrangeRed.ToString());
                    return System.Windows.Media.Brushes.Red;
                }
                if (result >= 25 && result < 50)
                {
                    return System.Windows.Media.Brushes.OrangeRed;
                }
                if (result >= 50 && result < 75)
                {
                    return System.Windows.Media.Brushes.Orange;
                }
                if (result >= 75 && result <100)
                {
                    return System.Windows.Media.Brushes.DarkOrange;
                }
                if (result >=100)
                {
                    return System.Windows.Media.Brushes.Green;
                }

                return System.Windows.Media.Brushes.Violet;
            }

           return null;
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }
}
