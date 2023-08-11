using DevExpress.Mvvm.Native;
using DevExpress.Office.Utils;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Singletons
{
    internal class LogDataSingleton : GD_CommonLibrary.BaseSingleton<LogDataSingleton>
    {   
        public int LogCollectionMax = 100;
        private DXObservableCollection<OperatingLogViewModel> _dataObservableCollection;
        public DXObservableCollection<OperatingLogViewModel> DataObservableCollection
        {
            get
            {
                _dataObservableCollection ??= new DXObservableCollection<OperatingLogViewModel>();
                if (_dataObservableCollection.Count > LogCollectionMax)
                {
                   _dataObservableCollection.RemoveRange(0, _dataObservableCollection.Count - LogCollectionMax);
                }
                return _dataObservableCollection;
            }
            set => _dataObservableCollection = value;
        }



        private Dictionary<string, List<GD_Model.OperatingLogModel>> _tempOperatingLog;
        /// <summary>
        /// 當檔案被鎖定時 將資料先擺在這裡
        /// </summary>
        public Dictionary<string, List<GD_Model.OperatingLogModel>> TempOperatingLog
        {
            get=>_tempOperatingLog ??= new Dictionary<string, List<GD_Model.OperatingLogModel>>();
        }






    }
}
