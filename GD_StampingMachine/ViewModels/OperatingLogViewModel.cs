using GD_StampingMachine.GD_Model;
using System;

namespace GD_StampingMachine.ViewModels
{
    public class OperatingLogViewModel : GD_CommonLibrary.BaseViewModel
    {
        public OperatingLogViewModel(OperatingLogModel OperatingLog)
        {
            _operatingLog = OperatingLog;
        }
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_OperatingLogViewModel");
        private readonly OperatingLogModel _operatingLog = new();


        public DateTime LogDatetime
        {
            get => _operatingLog.LogDatetime;
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
