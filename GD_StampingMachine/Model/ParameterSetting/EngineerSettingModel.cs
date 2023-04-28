using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    public class EngineerSettingModel
    {
        /// <summary>
        /// 循環時間
        /// </summary>
        public TimeSpan CycleTime { get; set; }
        /// <summary>
        /// 間隔時間
        /// </summary>
        public TimeSpan Intervals { get; set; }


    }
}
