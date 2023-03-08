using GD_StampingMachine.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml.Linq;

namespace GD_StampingMachine.ValueConverters
{
    internal class EnumGetNameConverter : BaseValueConverter
    {
         public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is System.Enum)
            {
               return ((System.Enum)value).GetDescription();
            }
            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            foreach (System.Enum A in System.Enum.GetValues(targetType))
            {
                if (A.GetDescription() == value.ToString())
                {
                    return A;
                }
            }

            return value;
        }

       
    }
}
