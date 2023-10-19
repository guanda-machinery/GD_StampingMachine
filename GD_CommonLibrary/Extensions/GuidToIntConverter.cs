using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_CommonLibrary.Extensions
{
    public static class GuidToIntConverter
    {
        public static int[] GuidToInt(this Guid value)
        {
            byte[] b = value.ToByteArray();
            int bint = BitConverter.ToInt32(b, 0);
            var bint1 = BitConverter.ToInt32(b, 4);
            var bint2 = BitConverter.ToInt32(b, 8);
            var bint3 = BitConverter.ToInt32(b, 12);
            return new[] { bint, bint1, bint2, bint3 };
        }

        public static Guid IntToGuid(int value, int value1, int value2, int value3)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            BitConverter.GetBytes(value1).CopyTo(bytes, 4);
            BitConverter.GetBytes(value2).CopyTo(bytes, 8);
            BitConverter.GetBytes(value3).CopyTo(bytes, 12);
            return new Guid(bytes);
        }
    }
}
