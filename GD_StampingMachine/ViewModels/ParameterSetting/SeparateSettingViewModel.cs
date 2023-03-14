using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class SeparateSettingViewModel : ViewModelBase
    {
        public SeparateSettingViewModel()
        {
            initSeparateSetting();
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
                _separateSetting = value;   
                OnPropertyChanged(nameof(SeparateSetting));
            }
        }

        //private double _ss_SeparateBoxValue = 0;
        
        public double SingleSetting_SeparateBoxValue
        {
            get 
            {
                return SeparateSetting.SingleSetting_SeparateBoxModel.BoxSliderValue;
                //return _ss_SeparateBoxValue;
            }
            set
            {
                //_ss_SeparateBoxValue = value;
                SeparateSetting.SingleSetting_SeparateBoxModel.BoxSliderValue = value;
                //SeparateSetting.UnifiedSetting_SeparateBoxModel.ForEach(x => x.BoxSliderValue = _ss_SeparateBoxValue);
                OnPropertyChanged(nameof(SingleSetting_SeparateBoxValue));
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

        public ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                SeparateSetting = new SeparateSettingModel();
                initSeparateSetting();
            });
        }

        public ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {

            });
        }

        private void initSeparateSetting()
        {
            SingleSetting_SeparateBoxValue = 0;
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


    }
}
