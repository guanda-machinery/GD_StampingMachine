using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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
        /// 加工UID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// QR陣列已完成
        /// </summary>
        public bool DataMatrixIsFinish { get; set; }
        /// <summary>
        /// 鋼印已完成
        /// </summary>
        public bool EngravingIsFinish { get; set; }
        /// <summary>
        /// 已剪斷
        /// </summary>
        public bool ShearingIsFinish { get; set; }

        /// <summary>
        /// 被完成
        /// </summary>
        public bool IsFinish { get; set; }


        /// <summary>
        /// 鐵牌字串
        /// </summary>
        public string IronPlateString { get; set; }
        /// <summary>
        /// QR內容
        /// </summary>
        public string QrCodeContent { get; set; }
        /// <summary>
        /// 側邊字串(橫著打)
        /// </summary>
        public string QR_Special_Text { get; set; }


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
        /// <summary>
        /// 鐵牌參數
        /// </summary>
        public StampPlateSettingModel StampingPlate { get; set; } = new StampPlateSettingModel();
   
        /// <summary>
        /// 機台命令
        /// </summary>
        public SendMachineCommandModel SendMachineCommand { get; set; } = new SendMachineCommandModel();




    }






}
