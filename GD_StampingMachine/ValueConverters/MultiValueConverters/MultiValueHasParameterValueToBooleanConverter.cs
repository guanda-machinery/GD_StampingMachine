using DevExpress.Xpf.Core.Native;
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
    public class MultiValueHasParameterValueToBooleanConverter : BaseMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
           var  ValueList = values.ToList();

            if (parameter != null)
            {
                foreach(var item in ValueList)
                {
                    if(Equals(item, parameter))
                        return true;
                }
                return false;
            }
            return false;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if (!(bool)value)
                {
                    if(parameter is bool)
                    {
                        var FalseList = new List<object> ();
                        for(int i=0;i<   targetTypes.Count();i++)
                        {
                            FalseList.Add(false);
                        }
                        return FalseList.ToArray();
                    }
                }
            }

            throw new NotImplementedException();
        }

       
    }
}
