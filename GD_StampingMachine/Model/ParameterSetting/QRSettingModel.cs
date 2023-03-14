using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    public class QRSettingModel
    {
        public QRSettingModel()
        {

        }


        /// <summary>
        /// 目前模式
        /// </summary>
        public NumberSettingModeEnum NumberSettingMode { get; set; }

        /// <summary>
        /// 單排數量
        /// </summary>
        public int SequenceCount { get; set; }

        /// <summary>
        /// 特殊排序
        /// </summary>
        public SpecialSequenceEnum SpecialSequence { get; set; }


        /// <summary>
        /// Code設定 字元數量
        /// </summary>
        public int CharactersCount { get; set; }

        /// <summary>
        /// Code設定 字元型態
        /// </summary>
        public CharactersFormEnum CharactersForm { get; set; }

        public string ModelSize { get; set; }






        public IronPlateMarginStruct IronPlateMargin { get; set; } = new IronPlateMarginStruct();
    }

    public class IronPlateMarginStruct
    {
        public double A_Margin { get; set; }
        public double B_Margin { get; set; }
        public double C_Margin { get; set; }
        public double D_Margin { get; set; }
        public double E_Margin { get; set; }
        public double F_Margin { get; set; }
        public double G_Margin { get; set; }
        public double H_Margin { get; set; }
        public double I_Margin { get; set; }
    }








}
