using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    public class OperatingLogModel
    {
        public OperatingLogModel()
        {

        }
        public OperatingLogModel(DateTime _logDatetime, string _logString, bool _isAlert = false)
        {
            LogDatetime = _logDatetime;
            LogString = _logString;
            IsAlert = _isAlert;
        }
        //public LogSourceEnum LogSource { get; set; }
        public DateTime LogDatetime { get; set; }
        public string LogString { get; set; }
        public bool IsAlert { get; set; }
    }
}
