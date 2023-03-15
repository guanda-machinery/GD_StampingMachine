using GD_StampingMachine.Cultures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Enum
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum VerticalAlignEnum
    {
        [LocalizedDescription("Top", typeof(EnumResources))]
        Top, 
        [LocalizedDescription("Center", typeof(EnumResources))]
        Center,
        [LocalizedDescription("Bottom", typeof(EnumResources))]
        Bottom,

    }
}
