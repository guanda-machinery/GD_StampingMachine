using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model
{
    public class SeparateBoxModel
    {
        /// <summary>
        /// 盒子可用
        /// </summary>
        public bool BoxIsEnabled { get; set; }
        /// <summary>
        /// 盒子index
        /// </summary>
        public int BoxIndex { get; set; }
        /// <summary>
        /// 盒子在出料口
        /// </summary>
        public bool BoxIsUsing { get; set; }
        public double BoxSliderValue { get; set; }
    }
}
