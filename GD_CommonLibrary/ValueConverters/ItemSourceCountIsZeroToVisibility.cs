using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GD_CommonLibrary;

namespace GD_CommonLibrary.ValueConverters
{
    public class ItemSourceCountIsZeroToVisibility : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value!=null)
            {
                if(value is IEnumerable<object> ValueArray)
                {
                    return ValueArray.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;
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
