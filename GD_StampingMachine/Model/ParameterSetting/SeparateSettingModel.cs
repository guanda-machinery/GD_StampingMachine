using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    public class SeparateSettingModel
    {
        public SeparateSettingModel()
        {
            UnifiedSetting_SeparateBoxObservableCollection = new ObservableCollection<SeparateBoxViewModel>();
            SingleSetting_SeparateBox = new SeparateBoxViewModel();
        }
        /// <summary>
        /// 單一/統一設定
        /// </summary>
        public SettingTypeEnum SettingType { get; set; }
        public ObservableCollection<SeparateBoxViewModel> UnifiedSetting_SeparateBoxObservableCollection { get; set; }
        public SeparateBoxViewModel SingleSetting_SeparateBox { get; set; }
    }


}
