using DevExpress.Mvvm.Native;
using DevExpress.Office.Utils;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            private set => _dataObservableCollection = value;
        }



        private Dictionary<string, List<GD_Model.OperatingLogModel>> _tempOperatingLog;
        /// <summary>
        /// 當檔案被鎖定時 將資料先擺在這裡
        /// </summary>
        public Dictionary<string, List<GD_Model.OperatingLogModel>> TempOperatingLog
        {
            get=>_tempOperatingLog ??= new Dictionary<string, List<GD_Model.OperatingLogModel>>();
        }





        private static readonly object thisLock = new object();

        public void AddLogData(string SourceName, string LogString, bool IsAlarm = false)
        {
            Task.Run(() =>
            {
                try
                {
                    lock (thisLock)
                    {
                       System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            var LogSource = ((string)System.Windows.Application.Current.TryFindResource(SourceName));
                            if (string.IsNullOrEmpty(LogSource))
                                LogSource = SourceName;

                            var ResourceString = ((string)System.Windows.Application.Current.TryFindResource(LogString));
                            if (string.IsNullOrEmpty(ResourceString))
                                ResourceString = LogString;


                            var OperatingLog = (new OperatingLogModel(DateTime.Now, LogSource, ResourceString, IsAlarm));
                            Singletons.LogDataSingleton.Instance.DataObservableCollection.Add(new OperatingLogViewModel(OperatingLog));

                            const string LogFileDirectory = "Logs";
                            string LogFileName = System.IO.Path.Combine(LogFileDirectory, $"Log-{DateTime.Now.ToString("yyyy-MM-dd")}");
                            LogFileName += ".csv";

                            CsvFileManager csvManager = new CsvFileManager();


                            //嘗試寫入被鎖定的檔案
                            if (Singletons.LogDataSingleton.Instance.TempOperatingLog.Count > 0)
                            {

                                var SuccessfulWritedFile = new List<string>();
                                foreach (var pair in Singletons.LogDataSingleton.Instance.TempOperatingLog)
                                {
                                    if (!GD_CommonLibrary.Extensions.CommonExtensions.IsFileLocked(pair.Key))
                                    {
                                        csvManager.WriteCSVFileIEnumerable(pair.Key, pair.Value, true);
                                        SuccessfulWritedFile.Add(pair.Key);
                                    }
                                }
                                SuccessfulWritedFile.ForEach(_file =>
                                {
                                    Singletons.LogDataSingleton.Instance.TempOperatingLog.Remove(_file);
                                });

                            }


                            if (!GD_CommonLibrary.Extensions.CommonExtensions.IsFileLocked(LogFileName))
                            {
                                csvManager.WriteCSVFile(LogFileName, OperatingLog, true); ;
                            }
                            else
                            {
                                if (Singletons.LogDataSingleton.Instance.TempOperatingLog.TryGetValue(LogFileName, out var OperatingLogCsvList))
                                {
                                    OperatingLogCsvList.Add(OperatingLog);
                                }
                                else
                                {
                                    Singletons.LogDataSingleton.Instance.TempOperatingLog[LogFileName] = new List<OperatingLogModel>() { OperatingLog };
                                }
                            }
                            //將錯誤資料記錄下來
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }
            });
        }





    }
}
