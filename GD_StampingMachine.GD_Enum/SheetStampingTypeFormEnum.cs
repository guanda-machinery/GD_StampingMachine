using System.ComponentModel;

namespace GD_StampingMachine.GD_Enum
{

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SheetStampingTypeFormEnum
    {
        //None = 0,
        NormalSheetStamping = 1,
        QRSheetStamping = 2,
    }



}
