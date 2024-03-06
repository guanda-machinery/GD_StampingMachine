using CommunityToolkit.Mvvm.Input;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Singletons;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class EngineerSettingViewModel : ParameterSettingBaseViewModel, IAsyncDisposable
    {

        private bool isDisposed = false;
        public async ValueTask DisposeAsync()
        {
            if (!isDisposed)
            {
                await GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance.StopScanOpcuaAsync();
                // 标记为已释放
                isDisposed = true;

                Debug.WriteLine("Async resources disposed.");
            }
        }



        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_EngineerSettingViewModel");
        public EngineerSettingViewModel(EngineerSettingModel engineerSetting)
        {
            EngineerSetting = engineerSetting;
        }
        public EngineerSettingModel EngineerSetting;

        private DateTime? _cycleTimeTimePicker = null;
        public DateTime? CycleTimeTimePicker
        {
            get
            {
                return _cycleTimeTimePicker;
            }
            set
            {
                _cycleTimeTimePicker = value;
                if (value.HasValue)
                    EngineerSetting.CycleTime = _cycleTimeTimePicker.Value.TimeOfDay;
                else
                    EngineerSetting.CycleTime = new TimeSpan();
                OnPropertyChanged(nameof(CycleTimeTimePicker));
            }
        }


        private DateTime? _intervalsTimePicker = null;
        public DateTime? IntervalsTimePicker
        {
            get
            {
                return _intervalsTimePicker;
            }
            set
            {
                _intervalsTimePicker = value;
                if (value.HasValue)
                    EngineerSetting.Intervals = _intervalsTimePicker.Value.TimeOfDay;
                else
                    EngineerSetting.Intervals = new TimeSpan();
                OnPropertyChanged(nameof(IntervalsTimePicker));
            }
        }

        public override ICommand RecoverSettingCommand => throw new NotImplementedException();

        public override ICommand SaveSettingCommand => throw new NotImplementedException();

        public override ICommand LoadSettingCommand => throw new NotImplementedException();

        public override ICommand DeleteSettingCommand => throw new NotImplementedException();



        public StampMachineDataSingleton StampMachineData
        {
            get => GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance;
        }



        private AsyncRelayCommand? _opcuaFormBrowseServerOpenCommand;
        public AsyncRelayCommand OpcuaFormBrowseServerOpenCommand
        {
            get => _opcuaFormBrowseServerOpenCommand ??= new (async (CancellationToken token) =>
            {
                try
                {
                    await Task.Delay(1000);
                    //    await GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance.TestConnect();

                }
                catch (Exception)
                {

                }
            }, () => !OpcuaFormBrowseServerOpenCommand.IsRunning);
        }




        private AsyncRelayCommand? _opcuaStartScanCommand;
        public AsyncRelayCommand OpcuaStartScanCommand
        {
            get => _opcuaStartScanCommand ??= new (async () =>
            {
                //設定為自動開始
                try
                {
                    Properties.Settings.Default.ConnectOnStartUp = true;
                    Properties.Settings.Default.Save();// = false;               
                }
                catch
                {

                }

                await GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance.StartScanOpcuaAsync();

            }, () => !OpcuaStartScanCommand!.IsRunning);
        }

        private AsyncRelayCommand? _opcuaStopScanCommand;
        public AsyncRelayCommand OpcuaStopScanCommand
        {
            get => _opcuaStopScanCommand ??= new(async () =>
            {
                //設定為不自動開始
                try
                {
                    Properties.Settings.Default.ConnectOnStartUp = false;
                    Properties.Settings.Default.Save();// = false;
                    await Task.WhenAny(GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance.StopScanOpcuaAsync(), Task.Delay(10000));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Debugger.Break();
                }
            }, () => !OpcuaStopScanCommand!.IsRunning);
        }





    }
}
