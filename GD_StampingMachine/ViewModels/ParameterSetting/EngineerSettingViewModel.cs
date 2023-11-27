using CommunityToolkit.Mvvm.Input;
using DevExpress.Xpf.Scheduling.Themes;
using DevExpress.XtraRichEdit.Printing;
using GD_CommonLibrary;
using GD_MachineConnect.Machine;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Singletons;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
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

                Console.WriteLine("Async resources disposed.");
            }
        }



        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_EngineerSettingViewModel");
        public EngineerSettingViewModel(EngineerSettingModel _EngineerSetting)
        {
            EngineerSetting = _EngineerSetting;
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



        private AsyncRelayCommand _opcuaFormBrowseServerOpenCommand;
        public AsyncRelayCommand OpcuaFormBrowseServerOpenCommand
        {
            get => _opcuaFormBrowseServerOpenCommand ??= new AsyncRelayCommand(async (CancellationToken token) =>
            {
                try
                {
                        await GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance.TestConnect();

                }
                catch (Exception ex)
                {

                }
            }, () => !_opcuaFormBrowseServerOpenCommand.IsRunning);
        }








        private AsyncRelayCommand _opcuaStartScanCommand;
        public AsyncRelayCommand OpcuaStartScanCommand
        {
            get => _opcuaStartScanCommand??=new AsyncRelayCommand(async() =>
            {
                    await GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance.StartScanOpcuaAsync();

            } ,()=> !_opcuaStartScanCommand.IsRunning);
        }

        private AsyncRelayCommand _opcuaStopScanCommand;
        public AsyncRelayCommand OpcuaStopScanCommand
        {
            get => _opcuaStopScanCommand??=new(async () =>
            {
                await Task.WhenAll( GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance.StopScanOpcuaAsync() , Task.Delay(1000)) ;
            }, () => !_opcuaStopScanCommand.IsRunning);
        }





    }
}
