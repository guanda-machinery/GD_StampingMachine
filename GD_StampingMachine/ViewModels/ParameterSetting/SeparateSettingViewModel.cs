using GD_StampingMachine.Enum;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class SeparateSettingViewModel : ViewModelBase
    {
        public SeparateSettingViewModel()
        {

            if (SeparateSetting.UnifiedSetting_SeparateBoxModel.Count == 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    SeparateSetting.UnifiedSetting_SeparateBoxModel.Add(new SeparateBoxModel()
                    {
                        BoxNumber = i,
                        BoxSliderValue = 0
                    });
                }
            }
        }

        private SeparateSettingModel _separateSetting = new SeparateSettingModel();
        public SeparateSettingModel SeparateSetting
        {
            get
            {
              return _separateSetting;
            }
            set
            {
                if (_separateSetting != value)
                {
                    _separateSetting = value;
                }
                OnPropertyChanged(nameof(SeparateSetting));
            }



        }
        
        public bool SingleSettingBoolean
        {
            get
            {
                return SeparateSetting.SettingType == SettingTypeEnum.SingleSetting;
            }
            set
            {
                if(value)
                {
                    SeparateSetting.SettingType = SettingTypeEnum.SingleSetting;
                }
                OnPropertyChanged(nameof(SingleSettingBoolean));
            }
        }
        public bool UnifiedSettingBoolean
        {
            get
            {
                return SeparateSetting.SettingType == SettingTypeEnum.UnifiedSetting;
            }
            set
            {
                if (value)
                {
                    SeparateSetting.SettingType = SettingTypeEnum.UnifiedSetting;
                }
                OnPropertyChanged(nameof(UnifiedSettingBoolean));
            }
        }



    }
}
