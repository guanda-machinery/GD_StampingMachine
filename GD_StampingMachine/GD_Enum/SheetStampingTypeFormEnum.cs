using DevExpress.Mvvm.DataAnnotations;
//using GD_StampingMachine.Cultures;
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
       // [LocalizedDescription("NormalSheetStamping", typeof(EnumResources))]
        NormalSheetStamping, 
       // [LocalizedDescription("QRSheetStamping", typeof(EnumResources))]
        QRSheetStamping,

    }
}
