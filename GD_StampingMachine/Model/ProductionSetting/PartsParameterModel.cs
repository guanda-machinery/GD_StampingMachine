using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.ViewModels;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model.ProductionSetting
{
    public class PartsParameterModel////:CloneableBase
    {
        /// <summary>
        /// 分配加工專案
        /// </summary>
        public string DistributeName { get; set; }
        /// <summary>
        /// 加工專案名稱
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 參數A
        /// </summary>
        public string Parametert_A { get; set; }
        /// <summary>
        /// 參數B
        /// </summary>
        public string Parametert_B{ get; set; }
        /// <summary>
        /// 參數C
        /// </summary>
        public string Parametert_C { get; set; }
        /// <summary>
        /// 進度條
        /// </summary>
        public double FinishProgress { get; set; }
        /// <summary>
        /// 加工狀態
        /// </summary>
        public MachiningStatusEnum MachiningStatus{ get; set; }


        /// <summary>
        /// 分料盒
        /// </summary>
        public int? BoxNumber { get; set; }
        /// <summary>
        /// 鐵牌樣式
        /// </summary>
        public SettingViewModelBase SettingVMBase { get; set; }




    }



}
