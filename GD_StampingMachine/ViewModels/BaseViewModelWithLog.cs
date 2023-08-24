using DevExpress.Mvvm.Native;
using GD_StampingMachine.Method;
using GD_StampingMachine.GD_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GD_CommonLibrary;
using System.Diagnostics;
using DevExpress.CodeParser;
using GD_MachineConnect.Machine;
using GD_MachineConnect;
using System.Collections.ObjectModel;
using System.Threading;
using GD_CommonLibrary.Method;
using DevExpress.CodeParser.VB;
using Newtonsoft.Json;

namespace GD_StampingMachine.ViewModels
{
    public abstract class BaseViewModelWithLog : BaseViewModel
    {
        [JsonIgnore]
        public StampingMachineJsonHelper JsonHM = new ();

        [JsonIgnore]
        public abstract string ViewModelName { get; }

        [JsonIgnore]
        public int LogCollectionMax = 100;


        public void AddLogData(string LogString, bool IsAlarm = false)
        {
            var ResourceString = ((string)System.Windows.Application.Current.TryFindResource(LogString));
            if (string.IsNullOrEmpty(ResourceString))
                AddLogData(ViewModelName, ResourceString, IsAlarm);
            else
                AddLogData(ViewModelName, LogString, IsAlarm);
        }

        private static readonly object thisLock = new object();

        public void AddLogData(string LogSource , string LogString, bool IsAlarm = false)
        {
            Task.Run(() =>
            {
                try
                {
                    lock (thisLock)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            var OperatingLog = (new OperatingLogModel(DateTime.Now, LogSource, LogString, IsAlarm));

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
