using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class AxisSettingViewModel : BaseViewModelWithLog
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
    }
}
