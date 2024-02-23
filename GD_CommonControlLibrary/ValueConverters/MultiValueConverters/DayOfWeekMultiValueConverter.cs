using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_CommonControlLibrary
{
    public class DayOfWeekMultiValueConverter : BaseMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.FirstOrDefault(x => x is DayOfWeek) is DayOfWeek dayOfWeek)
            {
                CultureInfo ci;
                if (values.First(x => x is CultureInfo) is CultureInfo cultureInfo)
                {
                     ci = cultureInfo;
                }
                else
                {
                    ci = CultureInfo.CurrentCulture;
                }
                //CultureInfo ci = new CultureInfo("zh-tw");
                return ci.DateTimeFormat.GetShortestDayName(dayOfWeek);
                // return ci.DateTimeFormat.GetDayName(dayOfWeek);
            }
            return values;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
