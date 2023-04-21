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
    /// <summary>
    /// 計時設定
    /// </summary>
    public class TimingSettingViewModel : BaseViewModelWithLog
    {

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_TimingSettingViewModel");

        private TimingSettingModel _timingSetting = new TimingSettingModel();

        public TimingSettingModel TimingSetting
        {
            get 
            {
                return _timingSetting;
            } 
            set 
            {
                _timingSetting= value;
                OnPropertyChanged(nameof(TimingSetting));
            }

        }

       private DateTime? _cycleTimeTimePicker = null;
       public DateTime? CycleTimeTimePicker
        {
            get
            {
                return _cycleTimeTimePicker;
            }
            set
            {
                _cycleTimeTimePicker = value;
                if (value.HasValue)
                    TimingSetting.CycleTime = _cycleTimeTimePicker.Value.TimeOfDay;
                else
                    TimingSetting.CycleTime = new TimeSpan();
                OnPropertyChanged(nameof(CycleTimeTimePicker));
            }
        }


        private DateTime? _intervalsTimePicker = null;
        public DateTime? IntervalsTimePicker
        {
            get
            {
                return _intervalsTimePicker;
            }
            set
            {
                _intervalsTimePicker = value;
                if (value.HasValue)
                    TimingSetting.Intervals = _intervalsTimePicker.Value.TimeOfDay;
                else
                    TimingSetting.Intervals = new TimeSpan();
                OnPropertyChanged(nameof(IntervalsTimePicker));
            }
        }




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
                EnumList.Add(8);
                EnumList.Add(9);
                EnumList.Add(10);


                return EnumList;
            }
        }


        public ICommand LoadModeCommand
        {
            get => new RelayCommand(() =>
            {

            });
        }
        public ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                CycleTimeTimePicker = null;
                IntervalsTimePicker = null;
                TimingSetting = new TimingSettingModel();
            });
        }

        public ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {

            });
        }
    }










}
