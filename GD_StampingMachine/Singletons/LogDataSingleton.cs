using DevExpress.Mvvm.Native;
using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace GD_StampingMachine.Singletons
{
    internal class LogDataSingleton : GD_CommonLibrary.BaseSingleton<LogDataSingleton>
    {
        public LogDataSingleton()
        {
            LogCollectionMax = 100;
        }

        public int LogCollectionMax 
        {
            get => DataObservableCollection.Capacity;
            set => DataObservableCollection.Capacity = value;
        } 
        private LimitedSizeObservableCollection<OperatingLogViewModel>? _dataObservableCollection;
        public LimitedSizeObservableCollection<OperatingLogViewModel> DataObservableCollection=> _dataObservableCollection ??= new LimitedSizeObservableCollection<OperatingLogViewModel>();




        private Dictionary<string, List<GD_Model.OperatingLogModel>>? _tempOperatingLog;
        /// <summary>
        /// 當檔案被鎖定時 將資料先擺在這裡
        /// </summary>
        public Dictionary<string, List<GD_Model.OperatingLogModel>> TempOperatingLog
        {
            get => _tempOperatingLog ??= new Dictionary<string, List<GD_Model.OperatingLogModel>>();
        }
        //private static readonly object thisLock = new object();

        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);

        public async Task AddLogDataAsync(string SourceName, Exception ex, bool IsAlarm = false)
        {
            await AddLogDataAsync(SourceName, ex.Message , IsAlarm);
        }
        public async Task AddLogDataAsync(string SourceName, string LogString, bool IsAlarm = false)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var LogSource = ((string)System.Windows.Application.Current.TryFindResource(SourceName));
                    if (string.IsNullOrEmpty(LogSource))
                        LogSource = SourceName;

                    var ResourceString = ((string)System.Windows.Application.Current.TryFindResource(LogString));
                    if (string.IsNullOrEmpty(ResourceString))
                        ResourceString = LogString;

                    var OperatingLog = (new OperatingLogModel(DateTime.Now, LogSource, ResourceString, IsAlarm));

                    await System.Windows.Application.Current.Dispatcher.InvokeAsync((() =>
                    {
                        this.DataObservableCollection.Add(new OperatingLogViewModel(OperatingLog));
                    }));

                    try
                    {
                        await semaphoreSlim.WaitAsync(5000);

                        const string LogFileDirectory = "Logs";
                        string LogFileName = System.IO.Path.Combine(LogFileDirectory, $"Log-{DateTime.Now.ToString("yyyy-MM-dd")}");
                        LogFileName += ".csv";
                        CsvFileManager csvManager = new();
                        //嘗試寫入被鎖定的檔案
                        if (this.TempOperatingLog.Count > 0)
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
                            if (this.TempOperatingLog.TryGetValue(LogFileName, out var OperatingLogCsvList))
                            {
                                OperatingLogCsvList.Add(OperatingLog);
                            }
                            else
                            {
                                this.TempOperatingLog[LogFileName] = new List<OperatingLogModel>() { OperatingLog };
                            }
                        }
                        semaphoreSlim.Release();
                    }
                    catch (OperationCanceledException)
                    {

                    }
                    catch (Exception ex)
                    {
                        semaphoreSlim.Release();
                        Console.WriteLine(ex.ToString());
                    }

                });
            }
            catch
            {

            }
            //將錯誤資料記錄下來
        }





    }
}
