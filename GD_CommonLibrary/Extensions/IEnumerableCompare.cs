using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_CommonLibrary.Extensions
{
    public static class IEnumerableCompare
    {
        /// <summary>
        /// 可比較兩個陣列的長度 並回傳較長的值
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static int CountCompare<T1,T2>(this IEnumerable<T1> list1,  IEnumerable<T2> list2)
        {
            if (list1 == null)
            {
                return list2?.Count() ?? 0; 
            }
            else if (list2 == null)
            {
                return list1?.Count() ?? 0; 
            }
            else
            {
                return Math.Max(list1.Count(), list2.Count()); 
            }
        }

    }
}
