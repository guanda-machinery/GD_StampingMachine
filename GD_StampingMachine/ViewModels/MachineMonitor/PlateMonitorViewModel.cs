using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels.MachineMonitor
{
    public class PlateMonitorViewModel: BaseViewModel
    {
        public override string ViewModelName => nameof(PlateMonitorViewModel);

        private SettingBaseViewModel _settingBaseVM;
        public SettingBaseViewModel SettingBaseVM
        {
            get => _settingBaseVM;
            set { _settingBaseVM = value; OnPropertyChanged(); }
        }

        private SteelBeltStampingStatusEnum _stampingStatus;
        public SteelBeltStampingStatusEnum StampingStatus
        {
            get => _stampingStatus; set { _stampingStatus = value; OnPropertyChanged(); }

        }




        }
}
