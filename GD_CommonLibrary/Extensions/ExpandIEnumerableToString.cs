using System.Collections.Generic;

namespace GD_CommonLibrary.Extensions
{
    public static partial class CommonExtensions
    {
        public static string ExpandToString<T>(this IEnumerable<T> IEnumerableValue)
        {
            return IEnumerableValue.ExpandToString(',');
        }

        public static string ExpandToString<T>(this IEnumerable<T> IEnumerableValue, char splitCharacter)
        {
            var RString = "";
            foreach (var obj in IEnumerableValue)
            {
                RString += obj.ToString() + splitCharacter;
            }

            RString = RString.TrimEnd(splitCharacter);
            return RString;
        }

    }
}