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
        public NumberSettingViewModel NumberSettingVM { get; set; } = new NumberSettingViewModel();
        public QRSettingViewModel QRSettingVM { get; set; } = new QRSettingViewModel();
        public AxisSettingViewModel AxisSettingVM { get; set; } = new AxisSettingViewModel();
        public TimingSettingViewModel TimingSettingVM { get; set; } = new TimingSettingViewModel();
        public SeparateSettingViewModel SeparateSettingVM { get; set; } = new SeparateSettingViewModel();
    }
}
