using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GD_StampingMachine.UserControls.CustomControls;
using GD_StampingMachine.ViewModels.ParameterSetting;

namespace GD_StampingMachine.ViewModels
{
    public class ParameterSettingViewModel : BaseViewModelWithLog
    {

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

        /// <summary>
        /// 號碼設定
        /// </summary>
        public NumberSettingViewModel NumberSettingVM { get; set; } = new NumberSettingViewModel(new Model.NumberSettingModelBase());
        /// <summary>
        /// QR設定
        /// </summary>
        public QRSettingViewModel QRSettingVM { get; set; } = new QRSettingViewModel(new Model.NumberSettingModelBase());        
        /// <summary>
        /// 軸向設定
        /// </summary>
        public AxisSettingViewModel AxisSettingVM { get; set; } = new AxisSettingViewModel();
        /// <summary>
        /// 計時設定
        /// </summary>
        public TimingSettingViewModel TimingSettingVM { get; set; } = new TimingSettingViewModel();
        /// <summary>
        /// 分料設定
        /// </summary>
        public SeparateSettingViewModel SeparateSettingVM { get; set; } = new SeparateSettingViewModel();
    }
}
