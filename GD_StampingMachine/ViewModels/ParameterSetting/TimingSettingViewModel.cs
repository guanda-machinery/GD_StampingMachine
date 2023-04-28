using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GD_CommonLibrary;
using GD_StampingMachine.Method;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 計時設定
    /// </summary>
    public class TimingSettingViewModel : ParameterSettingBaseViewModel
    {

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_TimingSettingViewModel");

  
        public TimingSettingViewModel(TimingSettingModel _TimingSetting)
        {
            this.TimingSetting = _TimingSetting;
        }

        private TimingSettingModel _timingSetting;
        public TimingSettingModel TimingSetting { get => _timingSetting; set { _timingSetting = value; OnPropertyChanged(); } }


        public ObservableCollection<int> CharactersCountCollection
        {
            get
            {
                var EnumList = new ObservableCollection<int>();
                EnumList.Add(1);
                EnumList.Add(2);
                EnumList.Add(3);
                EnumList.Add(4);
                EnumList.Add(5); 
                EnumList.Add(6);
                EnumList.Add(7);


                return EnumList;
            }
        }


        public override ICommand LoadSettingCommand 
        {
            get => new RelayCommand(() =>
            {
                if (JsonHM.ReadParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.TimingSetting, out TimingSettingModel NewTiming ,true))
                {
                    TimingSetting = NewTiming;
                }
            });
        }
        public override ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                TimingSetting = new TimingSettingModel();
            });
        }

        public override ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {
                if (JsonHM.WriteParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.TimingSetting, TimingSetting, true))
                {
                }
            });
        }

        public override ICommand DeleteSettingCommand => throw new NotImplementedException();
    }










}
