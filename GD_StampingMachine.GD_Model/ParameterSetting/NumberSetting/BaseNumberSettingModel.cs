using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model
{



    public abstract class BaseNumberSettingModel: INumberSettingInterface
    {

    }

    public class IronPlateMarginStruct
    {
        public double A_Margin { get; set; }
        public double B_Margin { get; set; }
        public double C_Margin { get; set; }
        public double D_Margin { get; set; }
        public double E_Margin { get; set; }
    }

    public class QR_IronPlateMarginStruct : IronPlateMarginStruct
    {
        public double A_Margin { get; set; }
        public double B_Margin { get; set; }
        public double C_Margin { get; set; }
        public double D_Margin { get; set; }
        public double E_Margin { get; set; }
        public double F_Margin { get; set; }
        public double G_Margin { get; set; }
        public double H_Margin { get; set; }
        public double I_Margin { get; set; }
    }



}
