using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{

    public class StampPlateSettingModel
    {

        /// <summary>
        /// 自動換行
        /// </summary>
        //public bool DashAutoWrapping { get; set; }

    /// <summary>
    /// 型態
    /// </summary>
    public SheetStampingTypeFormEnum SheetStampingTypeForm { get; set; }
        /// <summary>
        /// 目前模式
        /// </summary>
        public string NumberSettingMode { get; set; }
        /// <summary>
        /// 單排數量
        /// </summary>
        public int SequenceCount { get; set; } = 6;

        /// <summary>
        /// 可加工的位址
        /// </summary>
        public List<PlateFontModel> StampableList { get; set; } = new List<PlateFontModel>();

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

        /// <summary>
        /// 側邊字串
        /// </summary>
        public string QR_Special_Text { get; set; }

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

    public class PlateFontModel
    {
        public bool IsUsed { get; set; }

        public string FontString { get; set; }
        /// <summary>
        /// 是否可變更IsUsed
        /// </summary>
        public bool IsUsedEditedable { get; set; }

    }
}