using CommunityToolkit.Mvvm.Input;
using GD_StampingMachine.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class MachineFunctionTest2ViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => nameof(MachineFunctionTest2ViewModel);

        public ICommand ResetAlarmCommand
        {
            get => new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StampMachineDataSingleton.Instance.AlarmMessageCollection.Clear();
                });
            });
        }
        public ICommand AddAlarmCommand
        {
            get => new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StampMachineDataSingleton.Instance.AlarmMessageCollection.Add("NewAlarm");
                });
            });
        }





    }
}
