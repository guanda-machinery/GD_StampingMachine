
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GD_CommonLibrary.Extensions
{
    public static partial class CommonExtensions
    {
        /// <summary>
        /// 是否包含檔案路徑中不允許的字元
        /// </summary>
        /// <param name="ValueString">路徑</param>
        /// <returns></returns>
        public static bool IsContain_illegalPathChars(this string ValueString, out List<char> illegalChar)
        {
            illegalChar = new List<char>();
            var illegal = false;
            foreach (var c in Path.GetInvalidPathChars())
            {
                if (ValueString.Contains(c))
                {
                    illegalChar.Add(c);
                    illegal = true;
                }
            }
            return illegal;
        }
        /// <summary>
        /// 是否包含檔案名稱中不允許的字元
        /// </summary>
        /// <param name="ValueString">路徑</param>
        /// <returns></returns>
        public static bool IsContain_illegalFileNameChars(this string ValueString, out List<char> illegalChar)
        {
            illegalChar = new List<char>();
            var illegal = false;
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                if (ValueString.Contains(c))
                {
                    illegal = true;
                    illegalChar.Add(c);
                }
            }
            return illegal;
        }
        /// <summary>
        /// 是否包含根目錄
        /// </summary>
        /// <param name="ValueString"></param>
        /// <returns></returns>
        public static bool IsPathRooted(this string ValueString)
        {
            return Path.IsPathRooted(ValueString);
        }
        /// <summary>
        /// 包含根目錄的合法檔案路徑
        /// </summary>
        /// <param name="ValueString"></param>
        /// <returns></returns>
        public static bool IsLegalPath(this string ValueString, out List<char> illegalChar)
        {
            illegalChar = new List<char>();
            return Path.IsPathRooted(ValueString) && !ValueString.IsContain_illegalPathChars(out illegalChar);
        }
        /// <summary>
        /// 包含根目錄的合法檔案名稱
        /// </summary>
        /// <param name="ValueString"></param>
        /// <returns></returns>
        public static bool IsLegalFileName(this string ValueString, out List<char> illegalChar)
        {
            illegalChar = new List<char>();
            return Path.IsPathRooted(ValueString) && !ValueString.IsContain_illegalFileNameChars(out illegalChar);
        }

    }
}
