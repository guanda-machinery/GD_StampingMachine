using DevExpress.Mvvm.Native;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GD_CommonLibrary;

namespace GD_StampingMachine.ViewModels
{
    public abstract class BaseViewModelWithLog : BaseViewModel
    {
        protected GD_JsonHelperMethod JsonHM = new GD_JsonHelperMethod();

        public abstract string ViewModelName { get; } 

        public static int LogCollectionMax = 100;
        private static DXObservableCollection<OperatingLogViewModel> _logDataObservableCollection;
        public static DXObservableCollection<OperatingLogViewModel> LogDataObservableCollection
        {
            get
            {
                if (_logDataObservableCollection == null)
                    _logDataObservableCollection = new DXObservableCollection<OperatingLogViewModel>();
                if (_logDataObservableCollection.Count > LogCollectionMax)
                {
                    _logDataObservableCollection.RemoveRange(0, _logDataObservableCollection.Count - LogCollectionMax);
                }
                return _logDataObservableCollection;
            }

            set
            {
                _logDataObservableCollection = value;
            }
        }
        public async void AddLogData(string LogString, bool IsAlarm = false)
        {
            await Task.Run(() =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    var ResourceString = ((string)System.Windows.Application.Current.TryFindResource(LogString));
                    if(ResourceString != null)
                        LogDataObservableCollection.Add(new OperatingLogViewModel(new OperatingLogModel(DateTime.Now, ViewModelName, ResourceString, IsAlarm)));
                    else
                        LogDataObservableCollection.Add(new OperatingLogViewModel(new OperatingLogModel(DateTime.Now, ViewModelName, LogString, IsAlarm)));

                });
            });
        }

        public async void AddLogData(string LogSource , string LogString, bool IsAlarm = false)
        {
            await Task.Run(() =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    LogDataObservableCollection.Add(new OperatingLogViewModel(new OperatingLogModel(DateTime.Now, LogSource, LogString, IsAlarm)));
                });
            });
        }

        





    }
}
