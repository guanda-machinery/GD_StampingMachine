using GD_StampingMachine.GD_Enum;

namespace GD_StampingMachine.GD_Model.ProductionSetting
{
    public class StampingSteelBeltModel
    {
        public string BeltString { get; set; }
        public string BeltNumberString { get; set; }
        public SteelBeltStampingStatusEnum MachiningStatus { get; set; }
    }
}
