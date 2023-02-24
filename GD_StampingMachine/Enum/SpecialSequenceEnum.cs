using DevExpress.Mvvm.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GD_StampingMachine.Enum
{
    public enum SpecialSequenceEnum
    {
        //[Image(imageUri: @"pack://application:,,,/GD_STD.Enum;component/ImageSVG/SelectPlate_Front.svg"), Description("前面", Description = "腹板")]
        [Description("雙排")]
        TwoRow,
        [Description("單排靠上")]
        OneTop,
        [Description("單排置中")]
        OneCenter,
        [Description("單排靠下")]
        OneBottom,
    }


}
