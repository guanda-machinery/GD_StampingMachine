using GD_StampingMachine.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    public class NumberSettingModel
    {
        public NumberSettingModel()
        {

        }


        /// <summary>
        /// 目前模式
        /// </summary>
        public NumberSettingModeEnum NumberSettingMode { get; set; }

        /// <summary>
        /// 單排數量
        /// </summary>
        public int SequenceCount { get; set; } = 8;

        /// <summary>
        /// 特殊排序
        /// </summary>
        public SpecialSequenceEnum SpecialSequence { get; set; }

        public PlateMarginStruct PlateMargin { get; set; } = new PlateMarginStruct();
    }

    public class PlateMarginStruct
    {
        public double A_Margin { get; set; }
        public double B_Margin { get; set; }
        public double C_Margin { get; set; }
        public double D_Margin { get; set; }
        public double E_Margin { get; set; }
    }








}
