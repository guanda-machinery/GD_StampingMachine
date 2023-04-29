using GD_CommonLibrary;
using GD_StampingMachine.Interfaces;
using Newtonsoft.Json;
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
        [JsonIgnore]
        public virtual ICommand RecoverSettingCommand { get; }
        [JsonIgnore]
        public virtual ICommand SaveSettingCommand { get; }
        [JsonIgnore]
        public virtual ICommand LoadSettingCommand { get; }
        [JsonIgnore]
        public virtual ICommand DeleteSettingCommand { get; }

    }
}
