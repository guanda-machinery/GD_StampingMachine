using GD_StampingMachine.GD_Enum;
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
            UnifiedSetting_SeparateBoxModel = new ObservableCollection<SeparateBoxModel>();
            SingleSetting_SeparateBoxModel = new SeparateBoxModel();
        }
        /// <summary>
        /// 單一/統一設定
        /// </summary>
        public SettingTypeEnum SettingType { get; set; }

        public ObservableCollection<SeparateBoxModel> UnifiedSetting_SeparateBoxModel { get; set; }
        public SeparateBoxModel SingleSetting_SeparateBoxModel { get; set; }
    }

    public class SeparateBoxModel
    {
        public int BoxNumber { get; set; }

        public double BoxSliderValue { get; set; }
    }


}
