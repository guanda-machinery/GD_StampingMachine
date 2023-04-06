using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GD_StampingMachine.ViewModels.ParameterSetting;

namespace GD_StampingMachine.ViewModels
{
    public class ParameterSettingViewModel : ViewModelBase
    {
        public NumberSettingViewModel NumberSettingVM { get; set; } = new NumberSettingViewModel(new Model.NumberSettingModelBase());
        public QRSettingViewModel QRSettingVM { get; set; } = new QRSettingViewModel(new Model.NumberSettingModelBase());
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
