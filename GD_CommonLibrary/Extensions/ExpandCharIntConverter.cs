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
        public static char ToChar(this int Num)
        {
            char C = Convert.ToChar(Num);
            return C;
        }
        public static int[] ToASC(this string S)
        {
          var N = new List<int>();
            S.ForEach(eachS =>
            {
                 N.Add(Convert.ToInt32(eachS));
            });
            return N.ToArray();
        }
        public static int ToASC(this char C)
        {
            int N = Convert.ToInt32(C);
            return N;
        }






    }
}
