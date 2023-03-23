using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    public class NumberSettingModelBase: CloneableBase
    {
        // public string NumberSettingName { get; set; }
        /// <summary>
        /// 目前模式
        /// </summary>
        public string NumberSettingMode { get; set; }
        /// <summary>
        /// 單排數量
        /// </summary>
        public virtual int SequenceCount { get; set; } = 8;

        /// <summary>
        /// 特殊排序
        /// </summary>
        public SpecialSequenceEnum SpecialSequence { get; set; }
        /// <summary>
        /// 水平對齊
        /// </summary>
        public HorizontalAlignEnum HorizontalAlign { get; set; }
        /// <summary>
        /// 垂直對齊
        /// </summary>
        public VerticalAlignEnum VerticalAlign { get; set; }

        public PlateMarginStruct IronPlateMargin { get; set; } = new PlateMarginStruct();
    }

    public class PlateMarginStruct
    {
        public double A_Margin { get; set; }
        public double B_Margin { get; set; }
        public double C_Margin { get; set; }
        public double D_Margin { get; set; }
        public double E_Margin { get; set; }
    }
    public class IronPlateMarginStruct: PlateMarginStruct
    {
        public double F_Margin { get; set; }
        public double G_Margin { get; set; }
        public double H_Margin { get; set; }
        public double I_Margin { get; set; }
    }


}
