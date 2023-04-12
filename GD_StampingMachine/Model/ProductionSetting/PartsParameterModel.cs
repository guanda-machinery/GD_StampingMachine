using GD_StampingMachine.ViewModels;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model.ProductionSetting
{
    public class PartsParameterModel////:CloneableBase
    {
        /// <summary>
        /// 分配加工名稱
        /// </summary>
        public string DistributeName { get; set; }
        /// <summary>
        /// 加工專案名稱
        /// </summary>
        public string ProjectName { get; set; }

        public string Parametert_A { get; set; }
        public string Parametert_B{ get; set; }
        public string Parametert_C { get; set; }

        public double FinishProgress { get; set; }

        /// <summary>
        /// 分料盒
        /// </summary>
        public int? BoxNumber { get; set; }
        public SettingViewModelBase SettingVMBase { get; set; }




    }



}
