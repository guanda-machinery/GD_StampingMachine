using DevExpress.Mvvm.DataAnnotations;
using GD_StampingMachine.Cultures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GD_StampingMachine.GD_Enum
{

    /// <summary>
    /// 特殊排序
    /// </summary>
    public enum SpecialSequenceEnum
    {
        //[Image(imageUri: @"pack://application:,,,/GD_STD.Enum;component/ImageSVG/SelectPlate_Front.svg"), Description("前面", Description = "腹板")]
        [LocalizedDescription("OneRow", typeof(EnumResources))]
        OneRow,
        [LocalizedDescription("TwoRow", typeof(EnumResources))]
        TwoRow
    }


}
