using GD_StampingMachine.Model;
using GD_StampingMachine.Model.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class AxisSettingViewModel : ParameterSettingBaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_AxisSettingViewModel");
        
        public AxisSettingViewModel(AxisSettingModel AxisSettingModel)
        {
            _axisSettingModel = AxisSettingModel;
        }

        public AxisSettingViewModel()
        {
        }

        private AxisSettingModel _axisSettingModel;

        public AxisSettingModel AxisSetting
        {
            get
            {
                //存檔
               return _axisSettingModel;
            }
            set
            {
                _axisSettingModel = value;
                OnPropertyChanged(nameof(AxisSetting));
            }
        }

        public override ICommand RecoverSettingCommand => throw new NotImplementedException();

        public override ICommand SaveSettingCommand => throw new NotImplementedException();

        public override ICommand LoadSettingCommand => throw new NotImplementedException();

        public override ICommand DeleteSettingCommand => throw new NotImplementedException();
    }
}
