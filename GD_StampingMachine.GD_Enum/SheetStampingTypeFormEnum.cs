using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Enum
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SheetStampingTypeFormEnum
    {
        //None = 0,
        // [LocalizedDescription("NormalSheetStamping", typeof(EnumResources))]
        normal = 1,
        // [LocalizedDescription("QRSheetStamping", typeof(EnumResources))]
        qrcode = 2,

    }
}
