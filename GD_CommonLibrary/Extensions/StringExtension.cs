using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_CommonLibrary.Extensions
{
    public static class StringExtension
    {        /// <summary>
             /// 使用指定長度分割字串
             /// </summary>
             /// <param name="owner"></param>
             /// <param name="Length"></param>
             /// <returns></returns>
        public static List<string> SpiltByLength(this string owner, int Length)
        {
            return SpiltByLength(owner, 0, Length);
        }
        /// <summary>
        /// 使用指定長度分割字串 由startIndex開始
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="startIndex"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static List<string> SpiltByLength(this string owner, int startIndex, int Length)
        {
            var list = new List<string>();
            if (Length > startIndex)
            {
                for (int i = 0; startIndex + i * Length < owner.Length; i++)
                {
                    var subLength = Length;
                    if (subLength > owner.Length - (startIndex + Length * i))
                    {
                        subLength = owner.Length - (startIndex + Length * i);
                    }
                    list.Add(owner.Substring(startIndex + i * Length, subLength));
                }
            }
            return list;
        }
    }
}
