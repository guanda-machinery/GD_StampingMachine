using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace GD_CommonControlLibrary
{
    public class ToggleButtonMultiBooleanToBooleanConverter : BaseMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue))
            {
            }
            return values.Contains(true);
        }


        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is bool ConvertBoolean)
            {
                var FalseList = new List<object>();
                foreach (var obj in targetTypes)
                {
                    FalseList.Add(false);
                }

                if (ConvertBoolean)
                {
                    FalseList[0] = true;
                }
                return FalseList.ToArray();
            }

            throw new NotImplementedException();
        }
    }
}
