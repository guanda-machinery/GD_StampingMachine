using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    /// <summary>
    /// 直接繼承NumberSettingModel
    /// </summary>
    public class QRSettingModel : NumberSettingModel
    {
        /// <summary>
        /// 目前模式
        /// </summary>
        //public string NumberSettingMode { get; set; }
        /// <summary>
        /// 單排數量
        /// </summary>
        public override int SequenceCount { get; set; } = 6;

        /// <summary>
        /// 特殊排序
        /// </summary>
        //public SpecialSequenceEnum SpecialSequence { get; set; }
        /// <summary>
        /// 水平對齊
        /// </summary>
        //public HorizontalAlignEnum HorizontalAlign { get; set; }
        /// <summary>
        /// 垂直對齊
        /// </summary>
        //public VerticalAlignEnum VerticalAlign { get; set; }

        /// <summary>
        /// Code設定 字元數量
        /// </summary>
        public int CharactersCount { get; set; }
        /// <summary>
        /// Code設定 字元型態
        /// </summary>
        public CharactersFormEnum CharactersForm { get; set; }

        public string ModelSize { get; set; }

        public new IronPlateMarginStruct IronPlateMargin { get; set; } =new IronPlateMarginStruct();
    }









}
