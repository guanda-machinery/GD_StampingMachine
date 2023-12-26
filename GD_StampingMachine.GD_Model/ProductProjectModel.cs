
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model.ProductionSetting;
using System;
using System.Collections.Generic;

namespace GD_StampingMachine.GD_Model
{
    public class ProductProjectModel
    {
        /// <summary>
        /// 路徑
        /// </summary
        public string ProjectPath { get; set; }
        /// <summary>
        /// 工程編號
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 型態
        /// </summary>
        public SheetStampingTypeFormEnum SheetStampingTypeForm { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改時間
        /// </summary>
        public DateTime? EditTime { get; set; }

        /// <summary>
        /// 完成進度
        /// </summary>
        public double FinishProgress { get; set; }

        /// <summary>
        /// 是否為已完成專案
        /// </summary>
        public bool ProductProjectIsFinish { get; set; }

        /// <summary>
        /// 專案不存在
        /// </summary>
        //public bool FileIsNotExisted { get; set; }
        /// <summary>
        /// 參數設定表
        /// </summary>
        //public ObservableCollection

        // public SheetStampingTypeFormEnum SheetStampingType { get; set; }

        /// <summary>
        /// 加工參數
        /// </summary>

        public List<PartsParameterModel> PartsParameterObservableCollection { get; set; } = new List<PartsParameterModel>();

    }
}
