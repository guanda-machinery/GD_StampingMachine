using GD_StampingMachine.GD_Enum;

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
