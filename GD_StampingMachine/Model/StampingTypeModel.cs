using DevExpress.CodeParser;
using GD_StampingMachine.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    public class StampingTypeModel : ViewModelBase, ICloneable
    {
        /// <summary>
        /// 鋼印文字
        /// </summary>
        public string StampingTypeString { get; set; }

        /// <summary>
        /// No編號
        /// </summary>
        public int StampingTypeNumber { get; set; }
        /// <summary>
        /// 使用次數
        /// </summary>
        public int StampingTypeUseCount { get; set; }

        public bool IsNewAddStamping { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
