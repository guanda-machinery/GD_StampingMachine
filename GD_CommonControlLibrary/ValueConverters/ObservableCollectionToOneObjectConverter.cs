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

                int CollectionCount;
                if (parameter is int para)
                    CollectionCount = para;
                else if (parameter is string paras && int.TryParse(paras, out CollectionCount))
                {

                }
                else
                    CollectionCount = 0;



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
