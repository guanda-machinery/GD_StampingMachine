using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.ViewModels.ParameterSetting;

namespace GD_StampingMachine.ViewModels.MachineMonitor
{
    public class PlateMonitorViewModel : BaseViewModel
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
            get => _stampingStatus;
            set
            {
                _stampingStatus = value;
                OnPropertyChanged();
            }

        }

        private bool _dataMatrixIsFinish;
        public bool DataMatrixIsFinish
        {
            get => _dataMatrixIsFinish; set { _dataMatrixIsFinish = value; OnPropertyChanged(); }
        }
        private bool _engravingIsFinish;
        public bool EngravingIsFinish
        {
            get => _engravingIsFinish; set { _engravingIsFinish = value; OnPropertyChanged(); }
        }

        private bool _shearingIsFinish;
        public bool ShearingIsFinish
        {
            get => _shearingIsFinish; set { _shearingIsFinish = value; OnPropertyChanged(); }
        }

    }
}
