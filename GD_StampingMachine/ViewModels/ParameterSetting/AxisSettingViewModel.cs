using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class AxisSettingViewModel : ViewModelBase
    {
        private AxisSettingModel _AxisSettingModel = new AxisSettingModel() {XAxisSpeed =50 };


        public AxisSettingModel AxisSetting
        {
            get
            {
               return _AxisSettingModel;
            }
            set
            {
                _AxisSettingModel = value;
                OnPropertyChanged(nameof(AxisSetting));
            }
        }



    }
}
