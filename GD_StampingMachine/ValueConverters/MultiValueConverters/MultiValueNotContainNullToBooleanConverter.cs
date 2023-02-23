using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace GD_StampingMachine.ValueConverters
{
    public class MultiValueNotContainNullToBooleanConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
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

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
