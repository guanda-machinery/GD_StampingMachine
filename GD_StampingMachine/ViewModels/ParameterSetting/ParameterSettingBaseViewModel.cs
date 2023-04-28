using GD_CommonLibrary;
using GD_StampingMachine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public abstract class ParameterSettingBaseViewModel : BaseViewModelWithLog
    {
        public virtual ICommand RecoverSettingCommand { get; }
        public virtual ICommand SaveSettingCommand { get; }
        public virtual ICommand LoadSettingCommand { get; }
        public virtual ICommand DeleteSettingCommand { get; }

    }
}
