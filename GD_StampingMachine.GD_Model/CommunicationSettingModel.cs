using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model
{
    public class CommunicationSettingModel
    {
        public string MachineName { get; set; }
        public string HostString { get; set; } 
        public int? Port { get; set; } 
        public string ServerDataPath { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public CommunicationProtocolEnum Protocol { get; set; }

    }


}
