using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_CommonLibrary.Extensions
{
    public static partial class CommonExtensions
    {
        public static string ExpandToString<T>(this IEnumerable<T> IEnumerableValue)
        {
            var RString = "";
            IEnumerableValue.ForEach(obj =>
            {
                RString += obj.ToString() + ",";
            });
           if( RString.Length>0)
                RString =  RString.Remove(RString.Length);
            return RString;

        }
    }
}