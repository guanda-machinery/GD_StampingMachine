using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml.Linq;

namespace GD_CommonLibrary.ValueConverters
{
    public class EnumGetNameConverter : BaseValueConverter
    {
         public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is System.Enum)
            {
               var description=((System.Enum)value).GetDescription();
               var CultResource = (string)System.Windows.Application.Current.TryFindResource("Enum_" + ((System.Enum)value).ToString());
                if (!string.IsNullOrEmpty(CultResource))
                {
                    return CultResource;
                }
                else
                {
                    if (!string.IsNullOrEmpty(description))
                    {
                        return description;
                     
                    }
                }
                return value.ToString() + "(找不到資源檔)";
            }
            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (System.Enum EnumA in System.Enum.GetValues(targetType))
            {
                if (EnumA.GetDescription() == value.ToString())
                {
                    return EnumA;
                }



            }

            return value;
        }

       
    }
}
