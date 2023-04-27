using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using GD_CommonLibrary;
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
    public class SeparateSettingViewModel : BaseViewModelWithLog
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SeparateSettingViewModel");
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


        public ObservableCollection<SeparateBoxViewModel> UnifiedSetting_SeparateBoxModel
        {
            get
            {
                return SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection;
            }
            set
            {
                SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection = value;
                OnPropertyChanged();
            }
        }

        public SeparateBoxViewModel SingleSetting_SeparateBoxModel
        {
            get
            {
                return SeparateSetting.SingleSetting_SeparateBox;
            }
            set
            {
                SeparateSetting.SingleSetting_SeparateBox = value;
                OnPropertyChanged();
            }
        }


        public double SingleSetting_SeparateBoxValue
        {
            get
            {
                SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection.ForEach(x => x.BoxSliderValue = SeparateSetting.SingleSetting_SeparateBox.BoxSliderValue);
                return SeparateSetting.SingleSetting_SeparateBox.BoxSliderValue;
            }
            set
            {
                SeparateSetting.SingleSetting_SeparateBox.BoxSliderValue = value;
                OnPropertyChanged(nameof(SingleSetting_SeparateBoxValue));
            }
        }


        public bool SingleSettingBoolean
        {
            get
            {
                SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection.ForEach(x =>
                {
                    x.BoxSliderValue = SeparateSetting.SingleSetting_SeparateBox.BoxSliderValue;
                    x.BoxSliderIsEnabled = SeparateSetting.SettingType == SettingTypeEnum.SingleSetting;
                });
                return SeparateSetting.SettingType == SettingTypeEnum.SingleSetting;
            }
            set
            {
                if(value)
                {
                    SeparateSetting.SettingType = SettingTypeEnum.SingleSetting;
                }
                OnPropertyChanged();
            }
        }
        public bool UnifiedSettingBoolean
        {
            get
            {
                SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection.ForEach(x =>
                {
                    x.BoxSliderIsEnabled = SeparateSetting.SettingType == SettingTypeEnum.SingleSetting;
                });
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
            if (SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection.Count == 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection.Add(new SeparateBoxViewModel()
                    {
                        BoxIndex = i,
                        BoxSliderValue = 0,
                        BoxIsEnabled = true,
                    }) ;
                }
            }
        }


    }
}
