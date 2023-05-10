using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model
{
    /// <summary>
    /// 機台設定
    /// </summary>
    public class MachanicalSpecificationModel
    {
        public AllowMachiningSizeModel AllowMachiningSize { get; set; } = new AllowMachiningSizeModel();

        public MachiningPropertyModel MachiningProperty { get; set; } = new MachiningPropertyModel();

        public MachineSizeModel MachineSize { get; set; } = new MachineSizeModel();

    }


    public class AllowMachiningSizeModel
    {
        /// <summary>
        /// 腹板高度上限
        /// </summary>
        public int WebHeightUpperLimited { get; set; }

        /// <summary>
        /// 腹板高度下限
        /// </summary>
        public int WebHeightLowerLimited { get; set; }

        /// <summary>
        /// 翼板寬度上限
        /// </summary>
        public int FlangeWidthUpperLimited { get; set; }

        /// <summary>
        /// 翼板寬度下限
        /// </summary>
        public int FlangeWidthLowerLimited { get; set; }


        /// <summary>
        /// 最大加工長度
        /// </summary>
        public int MachiningMaxLength { get; set; }
        /// <summary>
        /// 最短加工長度
        /// </summary>
        public int MachiningMinLength { get; set; }
    }

    public class MachiningPropertyModel
    {
        /// <summary>
        /// 垂直鑽頭數量
        /// </summary>
        public int VerticalDrillCount { get; set; }
        /// <summary>
        /// 水平鑽頭數量
        /// </summary>
        public int HorizontalDrillCount { get; set; }

        /// <summary>
        /// 每個垂直鑽孔頭的主軸數
        /// </summary>
        public int Each_VerticalDrill_SpindleCount { get; set; }

        /// <summary>
        /// 每個水平鑽孔頭的主軸數
        /// </summary>
        public int Each_HorizontalDrill_SpindleCount { get; set; }

        /// <summary>
        /// 輔助軸最大行程
        /// </summary>
        public int AuxiliaryAxisEffectiveTravelMax { get; set; }

        /// <summary>
        /// 最大孔徑
        /// </summary>
        public double MaxDrillDiameter { get; set; }

        /// <summary>
        /// 最大板厚
        /// </summary>
        public double MaxDrillThickness { get; set; }

        /// <summary>
        /// 主軸最大功率
        /// </summary>
        public double SpindleMaxPower { get; set; }

        /// <summary>
        /// 刀把規格
        /// </summary>
        public SpindleToolHolderEnum SpindleToolHolder { get; set; }

        /// <summary>
        /// 主軸轉速上限
        /// </summary>
        public double SpindleRotationalFrequencyMax { get; set; }
        /// <summary>
        /// 主軸轉速下限
        /// </summary>
        public double SpindleRotationalFrequencyMin { get; set; }

        /// <summary>
        /// 主軸進給速度上限
        /// </summary>
        public double SpindleFeedSpeedMax { get; set; }
        /// <summary>
        /// 主軸進給速度下限
        /// </summary>
        public double SpindleFeedSpeedMin { get; set; }


        /// <summary>
        /// 鑽孔軸軸向移動速度
        /// </summary>
        public double SpindleMoveSpeed { get; set; }

    }

    /// <summary>
    /// 機台尺寸
    /// </summary>
    public class MachineSizeModel
    {
        /// <summary>
        /// 長
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// 寬
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public double Weight { get; set; }
    }



}