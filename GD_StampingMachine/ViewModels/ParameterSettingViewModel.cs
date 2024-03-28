using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System.Collections.Generic;
using System.Linq;

namespace GD_StampingMachine.ViewModels
{

    public class ParameterSettingModel
    {
        //public GD_Model.StampingPlateSettingModel NumberSetting { get; set; } = new();
        /// <summary>
        /// QR設定
        /// </summary>
        //public GD_Model.StampingPlateSettingModel QRSetting { get; set; } = new();
        /// <summary>
        /// 軸向設定
        /// </summary>
        public GD_Model.AxisSettingModel AxisSetting { get; set; } = new();
        /// <summary>
        /// 計時設定
        /// </summary>
        public GD_Model.TimingSettingModel TimingSetting { get; set; } = new();
        /// <summary>
        /// 分料設定
        /// </summary>
        public GD_Model.SeparateSettingModel SeparateSetting { get; set; } = new();
        /// <summary>
        /// Inputoutput
        /// </summary>
        public GD_Model.InputOutputModel InputOutput { get; set; } = new();
        /// <summary>
        /// 工程模式
        /// </summary>
        public GD_Model.EngineerSettingModel EngineerSetting { get; set; } = new();
    }

    public class ParameterSettingViewModel : GD_CommonControlLibrary.BaseViewModel
    {

  
        readonly StampingMachineJsonHelper JsonHM = new();

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_ParameterSettingViewModel");

        private bool _tBtnNumberSettingIsChecked;
        private bool _tBtnDataMatrixSettingIsChecked;
        private bool _tBtnAxisSettingSettingIsChecked;
        private bool _tBtnTimingSettingIsChecked;
        private bool _tBtnSegregationSettingIsChecked;
        private bool _tBtnInputOutputIsChecked;
        private bool _tBtnSeEngineeringModeIsChecked;


        public bool TBtnNumberSettingIsChecked { get => _tBtnNumberSettingIsChecked; set { _tBtnNumberSettingIsChecked = value; OnPropertyChanged(); } }
        public bool TBtnDataMatrixSettingIsChecked { get => _tBtnDataMatrixSettingIsChecked; set { _tBtnDataMatrixSettingIsChecked = value; OnPropertyChanged(); } }
        public bool TBtnAxisSettingSettingIsChecked { get => _tBtnAxisSettingSettingIsChecked; set { _tBtnAxisSettingSettingIsChecked = value; OnPropertyChanged(); } }
        public bool TBtnTimingSettingIsChecked { get => _tBtnTimingSettingIsChecked; set { _tBtnTimingSettingIsChecked = value; OnPropertyChanged(); } }
        public bool TBtnSegregationSettingIsChecked { get => _tBtnSegregationSettingIsChecked; set { _tBtnSegregationSettingIsChecked = value; OnPropertyChanged(); } }
        public bool TBtnInputOutputIsChecked { get => _tBtnInputOutputIsChecked; set { _tBtnInputOutputIsChecked = value; OnPropertyChanged(); } }
        public bool TBtnSeEngineeringModeIsChecked { get => _tBtnSeEngineeringModeIsChecked; set { _tBtnSeEngineeringModeIsChecked = value; OnPropertyChanged(); } }


        public ParameterSettingViewModel()
        {
            _parameterSetting = new();
        }


        public ParameterSettingViewModel(ParameterSettingModel ParameterSetting)
        {
            _parameterSetting = ParameterSetting;
        }

        private ParameterSettingModel? _parameterSetting = new();


        public NumberSettingPageViewModel? _numberSettingPageVM;
        /// <summary>
        /// 號碼設定
        /// </summary>
        public NumberSettingPageViewModel NumberSettingPageVM
        {
            get
            {
                if (_numberSettingPageVM == null)
                {
                    JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.NumberSetting, out List<StampPlateSettingModel> nSavedCollection, false);
                    _numberSettingPageVM = new()
                    { 
                        NumberSettingModelCollection = new(nSavedCollection.Select(x => new NumberSettingViewModel(x)))
                    };
                }
                return _numberSettingPageVM;
            }
        }

        private QRSettingPageViewModel? _qrSettingPageVM;
        /// <summary>
        /// QR設定
        /// </summary>
        public QRSettingPageViewModel QRSettingPageVM
        {
            get
            {
                 if (_qrSettingPageVM == null)
                {
                    JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.QRSetting, out List<StampPlateSettingModel> qrSavedCollection, false);
                    _qrSettingPageVM = new()
                    {
                        QRSettingModelCollection = new(qrSavedCollection.Select(x => new QRSettingViewModel(x)))
                    };
                }
                return _qrSettingPageVM;
            }
        }

        private AxisSettingViewModel? _axisSettingVM;
        /// <summary>
        /// 軸向設定
        /// </summary>
        public AxisSettingViewModel AxisSettingVM =>_axisSettingVM ??= new AxisSettingViewModel(_parameterSetting.AxisSetting);
    
        private TimingSettingViewModel? _timingSettingVM;
        /// <summary>
        /// 計時設定
        /// </summary>
        public TimingSettingViewModel TimingSettingVM => _timingSettingVM ??= new TimingSettingViewModel(_parameterSetting.TimingSetting);
        /// <summary>
        /// 分料設定
        /// </summary>
        /// 
        private SeparateSettingViewModel? _separateSettingVM;
        public SeparateSettingViewModel SeparateSettingVM => _separateSettingVM ??= new SeparateSettingViewModel(_parameterSetting.SeparateSetting);


        private InputOutputViewModel? _inputOutputVM;
        /// <summary>
        /// Inputoutput
        /// </summary>
        public InputOutputViewModel InputOutputVM => _inputOutputVM ??= new InputOutputViewModel(_parameterSetting.InputOutput);


        private EngineerSettingViewModel? _engineerSettingVM;
        /// <summary>
        /// 工程模式
        /// </summary>
        public EngineerSettingViewModel EngineerSettingVM => _engineerSettingVM ??= new EngineerSettingViewModel(_parameterSetting.EngineerSetting);
    }
}
