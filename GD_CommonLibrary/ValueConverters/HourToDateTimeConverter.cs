using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_CommonLibrary
{
    public class HourToDateTimeConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double Hour)
            {
                var ts = TimeSpan.FromHours(Hour);
                return $"{ts.Hours}:{ts.Minutes:D2}";
            }
            return "00:00";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeString = value.ToString();

            DateTime result=default(DateTime);
            if (DateTime.TryParseExact(timeString , "HH:mm:ss",null, DateTimeStyles.None, out result))
            {

            }
            else if (DateTime.TryParseExact(timeString, "HH:mm", null, DateTimeStyles.None, out result))
            {

            }
            else if (DateTime.TryParseExact(timeString, "HH", null, DateTimeStyles.None, out result))
            {

            }


            var timeDifference = result - result.Date;

            return timeDifference.TotalHours.ToString();
        }
    }
}
