using System;
using System.Globalization;

namespace GD_CommonControlLibrary
{
    public class DayOfWeekConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DayOfWeek dayOfWeek)
            {
                var ci = CultureInfo.CurrentCulture;
                //CultureInfo ci = new CultureInfo("zh-tw");
                return ci.DateTimeFormat.GetDayName(dayOfWeek);
            }
            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
