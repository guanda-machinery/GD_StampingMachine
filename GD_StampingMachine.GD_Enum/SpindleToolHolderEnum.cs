using System.ComponentModel.DataAnnotations;

namespace GD_StampingMachine.GD_Enum
{
    public enum SpindleToolHolderEnum
    {
        // [Image(imageUri: @"pack://application:,,,/GD_STD.Enum;component/ImageSVG/SelectPlate_Top.svg"), Display(Name = "上面", Description = "右翼板")]
        [Display(Name = "None", Description = "")]
        None,
        [Display(Name = "BT30", Description = "")]
        BT30,
        [Display(Name = "BT40", Description = "")]
        BT40,
        [Display(Name = "BT50", Description = "")]
        BT50,
        HSK_A,
        HSK_B,
        HSK_C,
        HSK_D,
        HSK_E,
        HSK_F

    }
}
