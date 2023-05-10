using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model
{
    public class StampingPlateSettingModel
    {     
        /// <summary>
        /// 目前模式
        /// </summary>
        public string NumberSettingMode { get; set; }
        /// <summary>
        /// 單排數量
        /// </summary>
        public int SequenceCount { get; set; } = 6;

        /// <summary>
        /// 特殊排序
        /// </summary>
        public SpecialSequenceEnum SpecialSequence { get; set; } = SpecialSequenceEnum.TwoRow;
        /// <summary>
        /// 水平對齊
        /// </summary>
        public HorizontalAlignEnum HorizontalAlign { get; set; } = HorizontalAlignEnum.Left;
        /// <summary>
        /// 垂直對齊
        /// </summary>
        public VerticalAlignEnum VerticalAlign { get; set; } = VerticalAlignEnum.Top;


        /// <summary>
        /// Code設定 字元數量
        /// </summary>
        public int CharactersCount { get; set; }
        /// <summary>
        /// Code設定 字元型態
        /// </summary>
        public CharactersFormEnum CharactersForm { get; set; }
        public string ModelSize { get; set; }

        public PlateMarginStruct IronPlateMargin { get; set; } = new PlateMarginStruct();
    }

   /* public class NormalStampingPlateSettingModel: IStampingPlateSettingModel
    {     
        /// <summary>
        /// 單排數量
        /// </summary>
        public override int SequenceCount { get; set; } = 8;
    }

    public class QRStampingPlateSettingModel : IStampingPlateSettingModel
    {
        /// <summary>
        /// 單排數量
        /// </summary>
        public override int SequenceCount { get; set; } = 6;

    }*/

    public class PlateMarginStruct
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
