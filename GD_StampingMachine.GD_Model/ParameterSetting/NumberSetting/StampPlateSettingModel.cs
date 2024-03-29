﻿using GD_StampingMachine.GD_Enum;
using System.Collections.Generic;

namespace GD_StampingMachine.Model
{

    public class StampPlateSettingModel
    {

        public StampPlateSettingModel()
        {
            SequenceCount = 6;
            StampingMarginPos = new StampingMarginPosModel()
            {
                rXAxisPos1 = 10,
                rXAxisPos2 = 25,
                rYAxisPos1 = 14,
                rYAxisPos2 = 14,
            };
            IronPlateMargin = new IronPlateMarginStruct();

        }

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
        /// 遇到- 換行的功能
        /// </summary>
        public bool DashAutoWrapping { get; set; }
        /// <summary>
        /// 單排數量
        /// </summary>
        public int SequenceCount { get; set; }


        public string PlateNumber { get; set; }

        /// <summary>
        /// 可加工的位址
        /// </summary>
        public List<PlateFontModel> StampableList1 { get; set; } = new List<PlateFontModel>();
        public List<PlateFontModel> StampableList2 { get; set; } = new List<PlateFontModel>();
        //public List<PlateFontModel>, L>)  { get; set; } = (new List<PlateFontModel>(), new List<PlateFontModel>());
        /// <summary>
        /// 可加工的位址
        /// </summary>
        //public List<PlateFontModel> StampableList2 { get; set; } = new List<PlateFontModel>();

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
        /// QR參數
        /// </summary>
        public string QrCodeContent { get; set; }

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

        public StampingMarginPosModel StampingMarginPos { get; set; } = new StampingMarginPosModel();
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

    public class StampingMarginPosModel
    {



        public float rXAxisPos1 { get; set; }
        public float rYAxisPos1 { get; set; }
        public float rXAxisPos2 { get; set; }
        public float rYAxisPos2 { get; set; }
    }



    public class PlateFontModel
    {
        public ushort FontIndex { get; set; }
        public bool IsUsed { get; set; }

        public string FontString { get; set; }
        /// <summary>
        /// 是否可變更IsUsed
        /// </summary>
        public bool IsUsedEditedable { get; set; }

    }
}