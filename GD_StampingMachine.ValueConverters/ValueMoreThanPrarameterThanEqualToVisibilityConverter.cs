using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using GD_CommonLibrary;


namespace GD_StampingMachine.ValueConverters
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
