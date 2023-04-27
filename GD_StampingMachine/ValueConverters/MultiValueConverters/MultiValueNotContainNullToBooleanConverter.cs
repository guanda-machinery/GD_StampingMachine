using DevExpress.Xpf.Core;
using GD_CommonLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using GD_CommonLibrary;

namespace GD_StampingMachine.ValueConverters
{
    /// <summary>
    /// 當多重綁定的值有空值時 回傳
    /// </summary>
    public class MultiValueNotContainNullToBooleanConverter : BaseMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var EachValue in values)
            {
                if (EachValue is null)
                    return false;

                if (EachValue == DependencyProperty.UnsetValue)
                    return false;

                if (EachValue is string)
                {
                    if (string.IsNullOrEmpty(EachValue as string) || (string.IsNullOrWhiteSpace(EachValue as string)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
