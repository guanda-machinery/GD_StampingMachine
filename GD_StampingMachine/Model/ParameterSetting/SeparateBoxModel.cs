using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model.ParameterSetting
{
    public class SeparateBoxModel
    {
        public bool BoxIsEnabled { get; set; }
        public int BoxIndex { get; set; }
        public double BoxSliderValue { get; set; }
    }
}
