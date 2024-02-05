using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace GD_CommonControlLibrary
{
    public class MultiBooleanToVisibilityConverter : BaseMultiValueConverter
    {
        /// <summary>
        /// 邏輯
        /// </summary>
        public enum SetOperationEnum
        {
            /// <summary>
            /// 交集
            /// </summary>
            Intersection,
            /// <summary>
            /// 聯集
            /// </summary>
            Union
        }

        public SetOperationEnum SetOperation { get; set; }
        public bool Invert { get; set; }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var ValueList = values.ToList();

            switch (SetOperation)
            {
                case SetOperationEnum.Intersection:
                    {
                        if (ValueList.Contains(false))
                            return (!Invert) ? Visibility.Collapsed : Visibility.Visible;
                        else
                            return (!Invert) ? Visibility.Visible : Visibility.Collapsed;
                    }
                case SetOperationEnum.Union:
                    {
                        if (ValueList.Contains(true))
                            return (!Invert) ? Visibility.Visible : Visibility.Collapsed;
                        else
                            return (!Invert) ? Visibility.Collapsed : Visibility.Visible;
                    }
            }



            return (!Invert) ? Visibility.Collapsed : Visibility.Visible;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}
