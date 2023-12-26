using System.ComponentModel;

namespace GD_StampingMachine.GD_Enum
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum VerticalAlignEnum
    {
        //[LocalizedDescription("Top", typeof(EnumResources))]
        Top,
        // [LocalizedDescription("Center", typeof(EnumResources))]
        Center,
        //[LocalizedDescription("Bottom", typeof(EnumResources))]
        Bottom,

    }
}
