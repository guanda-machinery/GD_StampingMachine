using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GD_CommonControlLibrary
{
    /// <summary>
    /// 聯集
    /// </summary>
    public class MultiBooleanArrayAndParameterArrayToBooleanConverter : BaseMultiValueConverter
    {
        public bool Invert { get; set; }
        /// <summary>
        /// 為true時 當逆轉換時value=false時強制回傳array(false)，否則依照parameter的值進行反轉
        /// </summary>
        public bool ConvertBackForceToFalse { get; set; }
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<bool> parameterList = null;
            if (parameter is IList<bool> pList)
            {
                parameterList = pList.ToList();
            }

            for (int i = 0; i < values.Length; i++)
            {
                object parameterObject = null;
                if (parameterList != null)
                {
                    try
                    {
                        parameterObject = parameterList[i];
                    }
                    catch
                    {

                    }
                }

                if (!Equals(values[i], parameterObject))
                {
                    return Invert;
                    // return false;
                }
            }
            // return true;
            return !Invert;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {


            if (ConvertBackForceToFalse)
            {
                if (value is false)
                {
                    object[] array = new object[targetTypes.Length];
                    for (int i = 0; i < targetTypes.Length; i++)
                    {
                        array[i] = false;
                    }
                    return array;
                }
            }



            List<bool> parameterList = null;
            if (parameter is IList<bool> pList)
            {
                parameterList = pList.ToList();
            }

            List<object> result = new List<object>();
            if (value is bool boolValue)
            {
                for (int i = 0; i < targetTypes.Length; i++)
                {
                    var parameterObject = true;
                    if (parameterList != null)
                    {
                        try
                        {
                            parameterObject = parameterList[i];
                        }
                        catch
                        {

                        }
                    }

                    if (boolValue)
                    {
                        result.Add(parameterObject);
                    }
                    else
                    {
                        result.Add(!parameterObject);
                    }

                }
            }
            return result.ToArray();
        }


    }
}
