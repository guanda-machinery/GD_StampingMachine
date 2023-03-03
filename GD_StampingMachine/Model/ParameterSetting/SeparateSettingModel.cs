using GD_StampingMachine.Enum;
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
        /// <summary>
        /// 單一/統一設定
        /// </summary>
        public SettingTypeEnum SettingType { get; set; }

        public ObservableCollection<SeparateBoxModel> UnifiedSetting_SeparateBoxModel { get; set; } = new ObservableCollection<SeparateBoxModel>(); 
        public SeparateBoxModel SingleSetting_SeparateBoxModel { get; set; } = new SeparateBoxModel();
    }

    public class SeparateBoxModel
    {
        public int BoxNumber { get; set; }

        public double BoxSliderValue { get; set; }
    }


}
