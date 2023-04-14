using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model.ProductionSetting
{
    public class StampingSteelBeltModel
    {
        public string BeltString { get; set; }
        public string BeltNumberString { get; set; }
        public SteelBeltStampingStatusEnum MachiningStatus { get; set; }
    }
}
