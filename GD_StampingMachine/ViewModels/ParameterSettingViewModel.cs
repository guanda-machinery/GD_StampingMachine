﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GD_StampingMachine.Method;
using GD_StampingMachine.UserControls.CustomControls;
using GD_StampingMachine.ViewModels.ParameterSetting;

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

    public class ParameterSettingViewModel : BaseViewModelWithLog
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_ParameterSettingViewModel");

        private bool _tbtnNumberSettingIsChecked;
        private bool _tbtnQRSettingIsChecked;
        private bool _tbtnAxisSettingSettingIsChecked;
        private bool _tbtnTimimgSettingIsChecked;
        private bool _tbtnSegregationSettingIsChecked;
        private bool _tbtnInputOutputIsChecked;
        private bool _tbtnSeEngineeringModeIsChecked;


        public bool TbtnNumberSettingIsChecked { get => _tbtnNumberSettingIsChecked; set { _tbtnNumberSettingIsChecked = value; OnPropertyChanged(); } }
        public bool TbtnQRSettingIsChecked { get => _tbtnQRSettingIsChecked; set { _tbtnQRSettingIsChecked = value; OnPropertyChanged(); } }
        public bool TbtnAxisSettingSettingIsChecked { get => _tbtnAxisSettingSettingIsChecked; set { _tbtnAxisSettingSettingIsChecked = value; OnPropertyChanged(); } }
        public bool TbtnTimimgSettingIsChecked { get => _tbtnTimimgSettingIsChecked; set { _tbtnTimimgSettingIsChecked = value; OnPropertyChanged(); } }
        public bool TbtnSegregationSettingIsChecked { get => _tbtnSegregationSettingIsChecked; set { _tbtnSegregationSettingIsChecked = value; OnPropertyChanged(); } }
        public bool TbtnInputOutputIsChecked { get => _tbtnInputOutputIsChecked; set { _tbtnInputOutputIsChecked = value; OnPropertyChanged(); } }
        public bool TbtnSeEngineeringModeIsChecked { get => _tbtnSeEngineeringModeIsChecked; set { _tbtnSeEngineeringModeIsChecked = value; OnPropertyChanged(); } }


        public ParameterSettingViewModel()
        {
            init(new());
        }


        public ParameterSettingViewModel(ParameterSettingModel ParameterSetting)
        {
            init(ParameterSetting);
        }

        private void init(ParameterSettingModel ParameterSetting)
        {
            _parameterSetting = ParameterSetting;
            NumberSettingPageVM = new NumberSettingPageViewModel();
            QRSettingPageVM = new QRSettingPageViewModel();
            AxisSettingVM = new AxisSettingViewModel(_parameterSetting.AxisSetting);
            TimingSettingVM = new TimingSettingViewModel(_parameterSetting.TimingSetting);
            SeparateSettingVM = new SeparateSettingViewModel(_parameterSetting.SeparateSetting);
            InputOutputVM = new InputOutputViewModel(_parameterSetting.InputOutput);
            EngineerSettingVM = new EngineerSettingViewModel(_parameterSetting.EngineerSetting);
        }




        private ParameterSettingModel _parameterSetting = new();

        /// <summary>
        /// 號碼設定
        /// </summary>
        public NumberSettingPageViewModel NumberSettingPageVM { get; set; }
        /// <summary>
        /// QR設定
        /// </summary>
        public QRSettingPageViewModel QRSettingPageVM { get; set; } 
        /// <summary>
        /// 軸向設定
        /// </summary>
        public AxisSettingViewModel AxisSettingVM { get; set; } 
        /// <summary>
        /// 計時設定
        /// </summary>
        public TimingSettingViewModel TimingSettingVM { get; set; } 
        /// <summary>
        /// 分料設定
        /// </summary>
        public SeparateSettingViewModel SeparateSettingVM { get; set; } 
        /// <summary>
        /// Inputoutput
        /// </summary>
        public InputOutputViewModel InputOutputVM { get; set; } 
        /// <summary>
        /// 工程模式
        /// </summary>
        public EngineerSettingViewModel EngineerSettingVM { get; set; } 
    }
}
