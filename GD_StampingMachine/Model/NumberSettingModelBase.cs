using DevExpress.Pdf;
using DevExpress.Xpf.PropertyGrid;
using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    public class NumberSettingModelBase
    {
        // public string NumberSettingName { get; set; }
        /// <summary>
        /// 目前模式
        /// </summary>
        public virtual string NumberSettingMode { get; set; }
        /// <summary>
        /// 單排數量
        /// </summary>
        public virtual int SequenceCount { get; set; } 

        /// <summary>
        /// 特殊排序
        /// </summary>
        public virtual SpecialSequenceEnum SpecialSequence { get; set; }
        /// <summary>
        /// 水平對齊
        /// </summary>
        public virtual HorizontalAlignEnum HorizontalAlign { get; set; }
        /// <summary>
        /// 垂直對齊
        /// </summary>
        public virtual VerticalAlignEnum VerticalAlign { get; set; }

        public abstract IronPlateMarginStruct IronPlateMargin { get; set; } 
    }

    public class IronPlateMarginBaseStruct
    {
        public virtual double A_Margin { get; set; }
        public virtual double B_Margin { get; set; }
        public virtual double C_Margin { get; set; }
        public virtual double D_Margin { get; set; }
        public virtual double E_Margin { get; set; }
        public virtual double F_Margin { get; set; }
        public virtual double G_Margin { get; set; }
        public virtual double H_Margin { get; set; }
        public virtual double I_Margin { get; set; }
    }


}
