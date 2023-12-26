using System;
using System.Globalization;
using System.Linq;

namespace GD_CommonLibrary
{
    public class HourToDateTimeConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double Hour)
            {
                var ts = TimeSpan.FromHours(Hour);
                return $"{ts.Hours}:{ts.Minutes:D2}";
            }
            return "00:00";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeString = value.ToString();

            DateTime result = default(DateTime);
            if (DateTime.TryParseExact(timeString, "HH:mm:ss", null, DateTimeStyles.None, out result))
            {

            }
            else if (DateTime.TryParseExact(timeString, "HH:mm", null, DateTimeStyles.None, out result))
            {

            }
            else if (DateTime.TryParseExact(timeString, "H:mm", null, DateTimeStyles.None, out result))
            {

            }
            else if (DateTime.TryParseExact(timeString, "HH", null, DateTimeStyles.None, out result))
            {

            }
            else if (DateTime.TryParseExact(timeString, "H", null, DateTimeStyles.None, out result))
            {

            }

            var timeDifference = result - result.Date;

            return timeDifference.TotalHours.ToString();
        }
    }


    public class DateTimeToHourConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime time)
            {
                var ts = (time - time.Date).TotalHours;
                return ts;
            }

            return 0d;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double dValue)
            {
                return new DateTime().AddHours(dValue);
            }

            return new DateTime();

        }
    }


    public class HourDiffToDateTimeMultiConverter : BaseMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 3)
                throw new NotImplementedException();

            var first = values.FirstOrDefault();
            var last = values.LastOrDefault();
            if (first is double firstHour && last is double lastHour)
            {
                var diff = lastHour - firstHour;
                if (diff < 0)
                    diff += 24;
                var ts = TimeSpan.FromHours(diff);

                var hourString = (string)System.Windows.Application.Current.TryFindResource("Hour");
                var minuteString = (string)System.Windows.Application.Current.TryFindResource("Minute");

                if (ts.Minutes < 1)
                {
                    return $"{ts.Hours}{hourString}";
                }

                if (hourString == null)
                    return $"{ts.Hours}:{ts.Minutes:D2}";
                else
                    return $"{ts.Hours}{hourString}{ts.Minutes:D2}{minuteString}";
            }
            return "00:00";
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class DateTimeDiffToDateTimeMultiConverter : BaseMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 3)
                throw new NotImplementedException();

            var first = values.FirstOrDefault();
            var last = values.LastOrDefault();
            if (first is DateTime firstHour && last is DateTime lastHour)
            {
                var diff = lastHour - firstHour;
                if (diff.TotalHours < 0)
                    diff = diff.Add(TimeSpan.FromHours(24));

                var ts = diff;

                var hourString = (string)System.Windows.Application.Current.TryFindResource("Hour");
                var minuteString = (string)System.Windows.Application.Current.TryFindResource("Minute");

                if (ts.Minutes < 1)
                {
                    return $"{ts.Hours}{hourString}";
                }

                if (hourString == null)
                    return $"{ts.Hours}:{ts.Minutes:D2}";
                else
                    return $"{ts.Hours}{hourString}{ts.Minutes:D2}{minuteString}";
            }
            return "00:00";
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
