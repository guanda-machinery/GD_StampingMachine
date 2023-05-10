using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model
{
    public class OperatingLogModel
    {
        public OperatingLogModel()
        {

        }
        public OperatingLogModel(DateTime _logDatetime, string _logSource, string _logString, bool _isAlert = false)
        {
            LogDatetime = _logDatetime;
            LogSource = _logSource;
            LogString = _logString;
            IsAlert = _isAlert;
        }
        public DateTime LogDatetime { get; set; }
        public string LogString { get; set; }
        public string LogSource { get; set; }
        public bool IsAlert { get; set; }
    }
}
