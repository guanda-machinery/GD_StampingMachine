using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class ParameterSettingViewModel : ViewModelBase
    {
        public NumberSettingViewModel NumberSettingVM { get; set; } = new NumberSettingViewModel();


    }
}
