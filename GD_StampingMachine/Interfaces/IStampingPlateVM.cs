using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model.ParameterSetting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Interfaces
{
    public interface IStampingPlateVM
    {
        /// <summary>
        /// 目前模式
        /// </summary>
        public string NumberSettingMode { get; set; }
        /// <summary>
        /// 單排數量
        /// </summary>
        public int SequenceCount { get; set; }

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

        //public Model.PlateMarginStruct IronPlateMargin { get; set; }

    }
}
