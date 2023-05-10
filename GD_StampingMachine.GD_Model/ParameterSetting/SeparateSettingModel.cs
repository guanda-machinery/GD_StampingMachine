using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model
{
    public class SeparateSettingModel
    {
        public SeparateSettingModel()
        {

        }
        /// <summary>
        /// 單一/統一設定
        /// </summary>
        public SettingTypeEnum SettingType { get; set; }
        public ObservableCollection<SeparateBoxModel> UnifiedSetting_SeparateBoxObservableCollection { get; set; } = new ObservableCollection<SeparateBoxModel>();
        public SeparateBoxModel SingleSetting_SeparateBox { get; set; } = new SeparateBoxModel();
    }
}
