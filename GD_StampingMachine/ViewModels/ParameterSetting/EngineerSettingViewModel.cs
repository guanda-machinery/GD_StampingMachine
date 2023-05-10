using GD_StampingMachine.GD_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class EngineerSettingViewModel : ParameterSettingBaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_EngineerSettingViewModel");
        public EngineerSettingViewModel(EngineerSettingModel _EngineerSetting)
        {
            EngineerSetting = _EngineerSetting;
        }
        public EngineerSettingModel EngineerSetting;

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
                    EngineerSetting.CycleTime = _cycleTimeTimePicker.Value.TimeOfDay;
                else
                    EngineerSetting.CycleTime = new TimeSpan();
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
                    EngineerSetting.Intervals = _intervalsTimePicker.Value.TimeOfDay;
                else
                    EngineerSetting.Intervals = new TimeSpan();
                OnPropertyChanged(nameof(IntervalsTimePicker));
            }
        }

        public override ICommand RecoverSettingCommand => throw new NotImplementedException();

        public override ICommand SaveSettingCommand => throw new NotImplementedException();

        public override ICommand LoadSettingCommand => throw new NotImplementedException();

        public override ICommand DeleteSettingCommand => throw new NotImplementedException();
    }
}
