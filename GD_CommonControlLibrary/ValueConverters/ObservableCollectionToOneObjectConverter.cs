using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GD_CommonControlLibrary
{
    public class ObservableCollectionToOneObjectConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<object>)
            {
                var ObservableCollectionValue = (value as IEnumerable<object>).ToList();

                int CollectionCount = 0;
                if (parameter is int)
                {
                    CollectionCount = (int)parameter;
                }

                if (parameter is string)
                {
                    int.TryParse((string)parameter, out CollectionCount);
                }





                if (ObservableCollectionValue.Count > CollectionCount)
                    return ObservableCollectionValue[CollectionCount];
                else
                    return null;
            }
            return null;
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}
