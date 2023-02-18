//using DevExpress.Mvvm.CodeGenerators;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    //[GenerateViewModel]
    public partial class StampingMainViewModel : ViewModelBase
    {
        public StampingMainViewModel()
        {
            Task.Run(() =>
             {
                 while(true)
                 {
                     DateTimeNow = DateTime.Now;
                     Thread.Sleep(100);
                 }
             });


            MechanicalSpecificationVM = new MechanicalSpecificationViewModel();
          

        }

        private DateTime _dateTimeNow = new DateTime();
        public DateTime DateTimeNow 
        { 
            get 
            {
                return _dateTimeNow;
            }
            set
            {
                _dateTimeNow = value;
                OnPropertyChanged(nameof(DateTimeNow));
            }
        }

        public MechanicalSpecificationViewModel MechanicalSpecificationVM { get; set; } = new MechanicalSpecificationViewModel();


        public RelayCommand Command
        {
            get
            {
                return new RelayCommand(()
                    => { });

            }
        }








    }
}
