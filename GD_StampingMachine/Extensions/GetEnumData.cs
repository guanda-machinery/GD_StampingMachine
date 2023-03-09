using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this System.Enum enumValue)
        {
            var name = enumValue.ToString();
            var field = enumValue.GetType().GetField(name);
            var description = name;

            if (field != null)
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                description = attribute?.Description ?? name;
            }
            return description;
        }

        public static TAttribute GetAttribute<TAttribute>(this System.Enum enumValue)
        where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();

        }
    }
}
