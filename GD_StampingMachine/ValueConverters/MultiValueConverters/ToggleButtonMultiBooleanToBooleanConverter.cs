using DevExpress.Utils.Animation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GD_StampingMachine.ValueConverters
{
    public class ToggleButtonMultiBooleanToBooleanConverter : BaseMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var ValueList = values.ToList();

            if (ValueList.Contains(DependencyProperty.UnsetValue))
            {
                Debugger.Break();
            }

            return ValueList.Contains(true);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is bool ConvertBoolean)
            {
                var FalseList = new List<object>();
                for (int i = 0; i < targetTypes.Count(); i++)
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
