using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model.ProductionSetting
{
    public class PartsParameterModel
    {
        /// <summary>
        /// 分配加工專案
        /// </summary>
        public string DistributeName { get; set; }
        /// <summary>
        /// 加工專案名稱
        /// </summary>
        public string ProjectID { get; set; }


        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }


        /// <summary>
        /// 參數A
        /// </summary>
        public string ParamA { get; set; }
        /// <summary>
        /// 參數B
        /// </summary>
        public string ParamB { get; set; }
        /// <summary>
        /// 參數C
        /// </summary>
        public string ParamC { get; set; }
        /// <summary>
        /// 進度條
        /// </summary>
        public float Processing { get; set; }
        /// <summary>
        /// 加工狀態
        /// </summary>
        public MachiningStatusEnum MachiningStatus { get; set; }
        /// <summary>
        /// 分料盒
        /// </summary>
        public int? BoxIndex { get; set; }

        

        public StampingPlateSettingModel StampingPlate { get; set; } 
    }






}
