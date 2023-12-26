//using GD_StampingMachine.Cultures;
using System.ComponentModel;

namespace GD_StampingMachine.GD_Enum
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum HorizontalAlignEnum
    {
        //[LocalizedDescription("Left", typeof(EnumResources))]
        Left,
        // [LocalizedDescription("Center", typeof(EnumResources))]
        Center,
        // [LocalizedDescription("Right", typeof(EnumResources))]
        Right,
    }
}
