using GD_StampingMachine.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model.ProductionSetting
{
    public class PartsParameterModel////:CloneableBase
    {
        //箱子種類/箱子編號
       
        public int BoxNumber { get; set; }
        public string Parametert_A { get; set; }
        public string Parametert_B{ get; set; }
        public string Parametert_C { get; set; }

        public double FinishProgress { get; set; }

        public SettingViewModelBase SettingVMBase { get; set; }




    }
}
