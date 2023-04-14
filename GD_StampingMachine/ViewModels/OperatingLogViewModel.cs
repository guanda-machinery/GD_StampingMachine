using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class OperatingLogViewModel:ViewModelBase
    {
        public OperatingLogViewModel(OperatingLogModel _operatingLog)
        {
            OperatingLog = _operatingLog;
        }
        public readonly OperatingLogModel OperatingLog = new();


        /*public LogSourceEnum LogSource 
        {
            get => OperatingLog.LogSource;
            set 
            { 
                OperatingLog.LogSource = value; 
                OnPropertyChanged(); 
            } 
        }*/

        public DateTime LogDatetime 
        { 
            get=> OperatingLog.LogDatetime; 
            set
            { 
                OperatingLog.LogDatetime = value;
                OnPropertyChanged();
            } 
        }

        public string LogString
        {
            get => OperatingLog.LogString;
            set
            {
                OperatingLog.LogString = value; 
                OnPropertyChanged();
            }
        }
        public bool IsAlert
        {
            get => OperatingLog.IsAlert;
            set
            {
                OperatingLog.IsAlert = value;
                OnPropertyChanged();
            }
        }





    }
}
