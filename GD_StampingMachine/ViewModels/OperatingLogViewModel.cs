using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class OperatingLogViewModel : BaseViewModelWithLog
    {
        public OperatingLogViewModel(OperatingLogModel OperatingLog)
        {
            _operatingLog = OperatingLog;
        }
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_OperatingLogViewModel");
        private readonly OperatingLogModel _operatingLog = new();


        public DateTime LogDatetime 
        { 
            get=> _operatingLog.LogDatetime; 
            set
            {
                _operatingLog.LogDatetime = value;
                OnPropertyChanged();
            }
        }
        public string LogSource
        {
            get => _operatingLog.LogSource;
            set
            {
                _operatingLog.LogSource = value;
                OnPropertyChanged();
            }
        }

        public string LogString
        {
            get => _operatingLog.LogString;
            set
            {
                _operatingLog.LogString = value; 
                OnPropertyChanged();
            }
        }
        public bool IsAlert
        {
            get => _operatingLog.IsAlert;
            set
            {
                _operatingLog.IsAlert = value;
                OnPropertyChanged();
            }
        }





    }
}
