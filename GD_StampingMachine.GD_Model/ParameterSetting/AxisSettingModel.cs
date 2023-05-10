using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.GD_Model
{
    /// <summary>
    /// 軸向
    /// </summary>
    public class AxisSettingModel
    {
        public double XAxisSpeed { get; set; } 
        public double YAxisSpeed { get; set; }
        public double FontDepth { get; set; }
        public double RouletteSpeed { get; set; }

        public double ZAxisPressure { get; set; }
        public double ZAxisOrigin { get; set; }
        public double ZAxisPreparationPoint { get; set; }

        public double FeedDistance { get; set; }
    }
}
