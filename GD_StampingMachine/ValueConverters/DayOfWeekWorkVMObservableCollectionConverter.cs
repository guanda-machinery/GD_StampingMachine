
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GD_StampingMachine
{
    public class DayOfWeekWorkVMObservableCollectionConverter : GD_CommonControlLibrary.BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IList<GD_StampingMachine.ViewModels.ParameterSetting.DayOfWeekWorkViewModel> collection)
            {
                var list = collection.Where(x => x.IsWork).Select(x => x.DayOfWeek).ToList();
                var ci = CultureInfo.CurrentCulture;

                var weekday = new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
                var weekend = new List<DayOfWeek>() { DayOfWeek.Saturday, DayOfWeek.Sunday };


                if (IsWeekday(list) && (IsWeekend(list)))
                {
                    return (string)System.Windows.Application.Current.TryFindResource("Text_Everyday");
                }
                else if (IsWeekday(list) && !IsAnyWeekend(list))
                {
                    return (string)System.Windows.Application.Current.TryFindResource("Text_EveryWeekday");
                }
                else if (IsWeekend(list) && !IsAnyWeekday(list))
                {
                    return (string)System.Windows.Application.Current.TryFindResource("Text_EveryWeekend");
                }
                else if (list.Count > 0)
                {
                    const char cut = '，';
                    var retrunString = "";
                    foreach (var dayOfWeek in list)
                    {
                        retrunString += ci.DateTimeFormat.GetDayName(dayOfWeek);
                        retrunString += cut;
                    }

                    retrunString = retrunString.TrimEnd(cut);
                    return retrunString;
                }
                else
                {
                    return (string)System.Windows.Application.Current.TryFindResource("Text_Never");
                }
            }
            return null;
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 陣列是否包含所有平日
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool IsWeekday(List<DayOfWeek> list)
        {
            var weekday = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            return weekday.All(element => list.Contains(element));
        }

        /// <summary>
        /// 陣列是否包含任一平日
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool IsAnyWeekday(List<DayOfWeek> list)
        {
            var weekday = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            return weekday.Any(element => list.Contains(element));
        }


        /// <summary>
        /// 陣列是否包含所有周末
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool IsWeekend(List<DayOfWeek> list)
        {
            var weekend = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            return weekend.All(element => list.Contains(element));
        }

        /// <summary>
        /// 陣列是否包含任一周末
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool IsAnyWeekend(List<DayOfWeek> list)
        {
            var weekend = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            return weekend.Any(element => list.Contains(element));
        }


    }
}
