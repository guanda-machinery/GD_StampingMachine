using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace GD_CommonLibrary.Extensions
{
    public static partial class CommonExtensions
    {
        /// <summary>
        /// 深層複製
        /// </summary>
        /// <typeparam name="T">複製對象類別</typeparam>
        /// <param name="source">複製對象
        /// <returns>複製出的物件</returns>
        public static T DeepClone<T>(this T from)
        {
            using (MemoryStream s = new MemoryStream())
            {
                BinaryFormatter f = new BinaryFormatter();
                f.Serialize(s, from);
                s.Position = 0;
                object clone = f.Deserialize(s);

                return (T)clone;
            }
        }

    }
}
