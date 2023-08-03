using GD_CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class MachineFunctionViewModel : BaseViewModelWithLog
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("btnDescription_MachineFunction");

        private bool _isSelected;

        public ICommand Cylinder_1_Up
        {
            get => new RelayCommand(() =>
            {


            });
        }
        public ICommand Cylinder_1_Mid
        {
            get => new RelayCommand(() =>
            {


            });
        }
        public ICommand Cylinder_1_Down
        {
            get => new RelayCommand(() =>
            {


            });
        }






    }
}
