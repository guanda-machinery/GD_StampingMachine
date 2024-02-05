using Newtonsoft.Json;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class ParameterSettingBaseViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_PartsParameterViewModel");

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
