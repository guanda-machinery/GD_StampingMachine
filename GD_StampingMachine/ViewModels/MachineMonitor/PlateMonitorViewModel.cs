using GD_CommonControlLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.ViewModels.ParameterSetting;

namespace GD_StampingMachine.ViewModels.MachineMonitor
{

    public class PlateMonitorModel 
    {
        public SettingBaseViewModel? SettingBaseVM { get; set; }

        public SteelBeltStampingStatusEnum StampingStatus { get; set; }

        public int ID { get; set; }
        public string? ProductProjectName { get; set; }
        public bool DataMatrixIsFinish { get; set; }
        public bool EngravingIsFinish { get; set; }
        public bool ShearingIsFinish { get; set; }
    }


    public class PlateMonitorViewModel : BaseViewModel
    {
        public override string ViewModelName => nameof(PlateMonitorViewModel);

        public PlateMonitorViewModel() 
        {
            PlateMonitor = new();
        }
        readonly PlateMonitorModel PlateMonitor;



        public SettingBaseViewModel? SettingBaseVM
        {
            get => PlateMonitor.SettingBaseVM;
            set { PlateMonitor.SettingBaseVM = value; OnPropertyChanged(); }
        }

        public SteelBeltStampingStatusEnum StampingStatus
        {
            get => PlateMonitor.StampingStatus;
            set
            {
                PlateMonitor.StampingStatus = value;
                OnPropertyChanged();
            }

        }

        public int ID
        {
            get => PlateMonitor.ID; 
            set
            {
                PlateMonitor.ID = value; OnPropertyChanged();
            }
        }

        public string? ProductProjectName
        {
            get => PlateMonitor.ProductProjectName;
            set
            {
                PlateMonitor.ProductProjectName = value; OnPropertyChanged();
            }
        }


        public bool DataMatrixIsFinish
        {
            get => PlateMonitor.DataMatrixIsFinish; set { PlateMonitor.DataMatrixIsFinish = value; OnPropertyChanged(); }
        }

        public bool EngravingIsFinish
        {
            get => PlateMonitor.EngravingIsFinish; set { PlateMonitor.EngravingIsFinish = value; OnPropertyChanged(); }
        }


        public bool ShearingIsFinish
        {
            get => PlateMonitor.ShearingIsFinish; set { PlateMonitor.ShearingIsFinish = value; OnPropertyChanged(); }
        }

    }
}
