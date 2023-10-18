using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model
{
    public class SendMachineCommandModel
    {
        public SteelBeltStampingStatusEnum SteelBeltStampingStatus { get; set; }

        /// <summary>
        /// 鐵片流水號
        /// </summary>
        public int WorkNumber { get; set; }

        
        /// <summary>
        /// 工作移動到下一個工站的相對距離(由計算得出 輪到他加工時他需移動的距離)
        /// </summary>
        public double RelativeMoveDistance { get; set; }
        /// <summary>
        /// 工作需要移動的絕對距離(目前位置離加工位置多遠)
        /// </summary>
        public double AbsoluteMoveDistance { get; set; }

        /// <summary>
        /// 鐵牌
        /// </summary>
        public StampPlateSettingModel StampPlateSetting { get; set; }

        /// <summary>
        /// 鐵牌寬度
        /// </summary>
        public  double StampWidth { get; set; }



        /// <summary>
        /// 加工需求 QR加工 true會進行加工
        /// </summary>
        public bool WorkScheduler_QRStamping { get; set; } = true;
        //public double WorkScheduler_QRStamping_XOffset { get; set; }


        /// <summary>
        /// 加工需求 鋼印加工 true會進行加工
        /// </summary>
        public bool WorkScheduler_FontStamping { get; set; } = true;
        //public double WorkScheduler_FontStamping_XOffset { get; set; }

        /// <summary>
        /// 加工需求 剪斷 true會進行加工
        /// </summary>
        public bool WorkScheduler_Shearing { get; set; } = true;
        //public double WorkScheduler_Shearing_XOffset { get; set; }when
        /// <summary>
        /// 標記為完成
        /// </summary>
        public bool IsFinish { get; set; }

        public bool IsSended { get; set; }

        /// <summary>
        /// 正在加工
        /// </summary>
        public bool IsWorking { get; set; }


        /// <summary>
        /// 加工順序
        /// </summary>
        public int WorkIndex { get; set; } = -1;



        /// <summary>
        /// 紀錄已經做過的工序
        /// </summary>
        public SteelBeltStampingStatusEnum MachiningStatus { get; set; }


    }




}
