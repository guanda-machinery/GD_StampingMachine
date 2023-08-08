
using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model
{
    public class NumberSettingModel : BaseNumberSettingModel
    {
        public new IronPlateMarginStruct IronPlateMargin { get; set; } = new IronPlateMarginStruct();
       
        public class Normal_IronPlateMarginStruct
        {
            public double A_Margin { get; set; }
            public double B_Margin { get; set; }
            public double C_Margin { get; set; }
            public double D_Margin { get; set; }
            public double E_Margin { get; set; }
        }
    }

}
