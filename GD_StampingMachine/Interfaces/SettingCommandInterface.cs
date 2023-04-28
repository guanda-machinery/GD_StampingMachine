using GD_CommonLibrary;
using GD_StampingMachine.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.Interfaces
{
    public interface SettingCommandInterface
    {
        public ICommand RecoverSettingCommand { get; }
        public ICommand SaveSettingCommand { get; }
        public ICommand LoadSettingCommand { get; }
        public ICommand DeleteSettingCommand { get; }
    }
}
