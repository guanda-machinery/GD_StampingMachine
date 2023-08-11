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
    internal class LogDataSingleton
    {   
        // 多執行緒，lock 使用
        private static readonly object thisLock = new();

        // 將唯一實例設為 private static
        private static LogDataSingleton instance;


        // 設為 private，外界不能 new
        private LogDataSingleton()
        {

        }
        // 外界只能使用靜態方法取得實例
        public static LogDataSingleton Instance
        {
            //雙重鎖
            get
            {
                if (instance == null)
                    lock (thisLock)
                        if (instance == null)
                            instance = new LogDataSingleton();
                return instance;
            }
        }

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
