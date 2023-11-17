using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GD_CommonLibrary
{
    public class UnionBooleanMultiValueConverter : BaseMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Implement your logic to determine the final IsEnabled value based on multiple conditions
            // Example: return (bool)values[0] && (bool)values[1];
            // For simplicity, assuming all conditions must be met
            return values.All(value => value is bool && (bool)value);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
