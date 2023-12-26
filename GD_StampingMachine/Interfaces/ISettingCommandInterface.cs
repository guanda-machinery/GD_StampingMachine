using System.Windows.Input;

namespace GD_StampingMachine.Interfaces
{
    public interface ISettingCommandInterface
    {
        public ICommand RecoverSettingCommand { get; }
        public ICommand SaveSettingCommand { get; }
        public ICommand LoadSettingCommand { get; }
        public ICommand DeleteSettingCommand { get; }
    }
}
