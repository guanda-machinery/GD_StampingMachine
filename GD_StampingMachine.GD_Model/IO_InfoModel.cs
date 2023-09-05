using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model
{
    public class IO_InfoModel
    {
        /// <summary>
        /// 功能或狀態說明
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// OPCUA 對應ID
        /// </summary>
        public string NodeID { get; set; }

        /// <summary>
        /// 端子
        /// </summary>
        public string BondingCableTerminal { get; set; }

        /// <summary>
        /// KEBA 定義
        /// </summary>
        public string KEBA_Definition { get; set; }


        /// <summary>
        /// i/o類別
        /// </summary>
        public GD_Enum.ioSensorType SensorType { get; set; }
        
        [JsonIgnore]
        public double? IO_Value { get; set; }


    }


}
