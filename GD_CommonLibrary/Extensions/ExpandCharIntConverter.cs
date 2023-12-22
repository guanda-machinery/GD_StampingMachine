
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GD_CommonLibrary.Extensions
{
    public static partial class CommonExtensions
    {
        public static char ToChar(this int Num)
        {
            if(Num <0)
                return '\0';
            char C = Convert.ToChar(Num);
            return C;
        }
        public static int ToInt(this char ch)
        {
            return Convert.ToInt32(ch);
        }

        public static int[] ToASC(this string S)
        {
          var N = new List<int>();
          foreach(var eachS in S)
            {
                 N.Add(Convert.ToInt32(eachS));
            }
            return N.ToArray();
        }
        public static int ToASC(this char C)
        {
            int N = Convert.ToInt32(C);
            return N;
        }
        public static List<char> ToCharList(this IList<int> intList)
        {
            List<char> charList = new List<char>();
            foreach (int num in intList)
            {
                charList.Add(num.ToChar());
            }
            return charList;
        }
        public static List<int> ToIntList(this IList<char> charList)
        {
            List<int> intList = new List<int>();
            foreach (char c in charList)
            {
                intList.Add(c.ToInt());
            }
            return intList;
        }









    }
}
