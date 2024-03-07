using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Extensions;
using DevExpress.XtraGauges.Core.Model;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Singletons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static GD_StampingMachine.Singletons.StampMachineDataSingleton.StampingOpcUANode;

namespace GD_StampingMachine.ViewModels
{
    public class MachineFunctionViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("btnDescription_MachineFunction");
        public ParameterSettingViewModel ParameterSettingVM { get => Singletons.StampingMachineSingleton.Instance.ParameterSettingVM; }
        public StampingFontChangedViewModel StampingFontChangedVM { get => Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM; }

        /// <summary>
        /// 已選擇的加工專案
        /// </summary>
        public ProjectDistributeViewModel ProjectDistributeVMSelected
        {
            get => StampingMachineSingleton.Instance.SelectedProjectDistributeVM;
            set
            {
                StampingMachineSingleton.Instance.SelectedProjectDistributeVM = value;
                OnPropertyChanged();
            }
        }

        public MachineFunctionViewModel()
        {
            _ = Task.Run(async () => { await Task.Delay(10000); SleepSettingStart(); });


        }

        //private int SeparateBoxIndexNow =-1;


        /// <summary>
        /// 從機台端蒐集到的資料
        /// </summary>
        [JsonIgnore]
        public StampMachineDataSingleton StampMachineData { get; set; } = Singletons.StampMachineDataSingleton.Instance;


        private ICommand?_gridControlSizeChangedCommand;
        public ICommand GridControlSizeChangedCommand
        {
            get => _gridControlSizeChangedCommand ??= new RelayCommand<object>(obj =>
            {

                if (obj is System.Windows.SizeChangedEventArgs e)
                {
                    if (e.Source is DevExpress.Xpf.Grid.GridControl gridcontrol)
                    {
                        if (gridcontrol.View is DevExpress.Xpf.Grid.TableView tableview)
                        {
                            var pageSize = ((tableview.ActualHeight - tableview.HeaderPanelMinHeight - 30) / 40);
                            tableview.PageSize = (pageSize < 3 ? 3 : (int)pageSize);
                        }
                    }
                }
            });
        }


        private ICommand?_gridControlLoadedCommand;
        public ICommand GridControlLoadedCommand
        {
            get => _gridControlLoadedCommand ??= new AsyncRelayCommand<object>(async obj =>
            {
                await Task.CompletedTask;

                if (obj is System.Windows.RoutedEventArgs e)
                {
                    if (e.Source is DevExpress.Xpf.Grid.GridControl gridcontrol)
                    {

                    }
                }
            });
        }












        private bool _feeding_Component_Button_IsChecked = false;
        /// <summary>
        /// 選擇進料組
        /// </summary>
        public bool Feeding_Component_Button_IsChecked
        {
            get => _feeding_Component_Button_IsChecked;
            set { _feeding_Component_Button_IsChecked = value; OnPropertyChanged(); }

        }


        private bool _qrCode_Component_Button_IsChecked = false;
        public bool QRCode_Component_Button_IsChecked
        {
            get => _qrCode_Component_Button_IsChecked;
            set { _qrCode_Component_Button_IsChecked = value; OnPropertyChanged(); }
        }



        private bool _stamping_Component_Button_IsChecked = false;
        /// <summary>
        /// 選擇進料組
        /// </summary>
        public bool Stamping_Component_Button_IsChecked
        {
            get => _stamping_Component_Button_IsChecked;
            set { _stamping_Component_Button_IsChecked = value; OnPropertyChanged(); }

        }
        private bool _shearCut_Component_Button_IsChecked = false;
        /// <summary>
        /// 選擇進料組
        /// </summary>
        public bool ShearCut_Component_Button_IsChecked
        {
            get => _shearCut_Component_Button_IsChecked;
            set { _shearCut_Component_Button_IsChecked = value; OnPropertyChanged(); }

        }
        private bool _Separator_Component_Button_IsChecked = false;
        /// <summary>
        /// 選擇進料組
        /// </summary>
        public bool Separator_Component_Button_IsChecked
        {
            get => _Separator_Component_Button_IsChecked;
            set { _Separator_Component_Button_IsChecked = value; OnPropertyChanged(); }

        }

        /*private bool _isManualMode = false;
        public bool IsManualMode
        {
            get => _isManualMode; set { _isManualMode = value; OnPropertyChanged(); }
        }*/





        private AsyncRelayCommand? _separateBox_ClockwiseRotateCommand;
        public AsyncRelayCommand SeparateBox_ClockwiseRotateCommand
        {
            get => _separateBox_ClockwiseRotateCommand ??= new(async () =>
            {

                try
                {
                    var Index = StampMachineData.SeparateBoxIndex;
                    // var Index = SeparateBoxIndexNow;
                    var LocationIndex = Index + 1;
                    if (LocationIndex >= ParameterSettingVM.SeparateSettingVM.SeparateBoxViewModelCollection.Count)
                        LocationIndex = 0;
                    if (LocationIndex < 0)
                        LocationIndex = ParameterSettingVM.SeparateSettingVM.SeparateBoxViewModelCollection.Count - 1;


                    if (await StampMachineData.SetSeparateBoxNumberAsync(LocationIndex))
                    {

                    }

                }
                catch (Exception ex)
                {

                    await LogDataSingleton.Instance.AddLogDataAsync(ViewModelName, ex.Message);
                }
                finally
                {

                }

            }, () => !SeparateBox_ClockwiseRotateCommand.IsRunning);
        }

        private AsyncRelayCommand? _separateBox_CounterClockwiseRotateCommand;
        public AsyncRelayCommand SeparateBox_CounterClockwiseRotateCommand
        {
            get => _separateBox_CounterClockwiseRotateCommand ??= new (async () =>
            {
                try
                {
                    var Index = StampMachineData.SeparateBoxIndex;
                    // var Index = SeparateBoxIndexNow;
                    var LocationIndex = Index - 1;
                    if (LocationIndex >= ParameterSettingVM.SeparateSettingVM.SeparateBoxViewModelCollection.Count)
                        LocationIndex = 0;
                    if (LocationIndex < 0)
                        LocationIndex = ParameterSettingVM.SeparateSettingVM.SeparateBoxViewModelCollection.Count - 1;
                    //SeparateBox_Rotate(LocationIndex, 1);

                    if (await StampMachineData.SetSeparateBoxNumberAsync(LocationIndex))
                    {

                    }

                }
                catch (Exception ex)
                {

                    await LogDataSingleton.Instance.AddLogDataAsync(ViewModelName, ex.Message);
                }
                finally
                {

                }
            }, () => !SeparateBox_CounterClockwiseRotateCommand.IsRunning);
        }






        /// <summary>
        /// 可計算轉盤上 下一個要轉的目標位置
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>

        private int NextSeparateBox_Rotate(int step)
        {
            var MinIndex = ParameterSettingVM.SeparateSettingVM.SeparateBoxViewModelCollection.ToList().FindIndex(x => x.BoxIsEnabled);
            var Maxindex = ParameterSettingVM.SeparateSettingVM.SeparateBoxViewModelCollection.ToList().FindLastIndex(x => x.BoxIsEnabled);
            var IsUsingindex = ParameterSettingVM.SeparateSettingVM.SeparateBoxViewModelCollection.FindIndex(x => x.IsUsing);
            if (IsUsingindex == -1)
            {
                if (step > 0)
                {
                    IsUsingindex = MinIndex;
                }
                else if (step <= 0)
                {
                    IsUsingindex = Maxindex;
                }
                ParameterSettingVM.SeparateSettingVM.SeparateBoxViewModelCollection[IsUsingindex].IsUsing = true;
                return IsUsingindex;
            }
            else
                ParameterSettingVM.SeparateSettingVM.SeparateBoxViewModelCollection[IsUsingindex].IsUsing = false;

            IsUsingindex += step;


            if (IsUsingindex < MinIndex)
            {
                IsUsingindex = Maxindex;
            }

            if (IsUsingindex > Maxindex)
            {
                //IsUsingindex = 0;
                IsUsingindex = MinIndex;
                //最小的可用index
            }
            return IsUsingindex;
        }

        private bool _sleepModeIsActivated;
        /// <summary>
        /// 啟用睡眠功能
        /// </summary>
        public bool SleepModeIsActivated
        {
            get => _sleepModeIsActivated; set
            {
                _sleepModeIsActivated = value; OnPropertyChanged();
            }
        }



        private AsyncRelayCommand? _sleepSettingStartCommand;
        public AsyncRelayCommand SleepSettingStartCommand
        {
            get => _sleepSettingStartCommand ??= new AsyncRelayCommand(async () =>
            {
                await SleepSettingStopAsync();
                SleepSettingStart();
            }, () => !SleepSettingStartCommand.IsRunning);
        }

        private AsyncRelayCommand? _sleepSettingStopCommand;
        public AsyncRelayCommand SleepSettingStopCommand
        {
            get => _sleepSettingStopCommand ??= new AsyncRelayCommand(async () =>
            {
                await SleepSettingStopAsync();
            }, () => !_sleepSettingStopCommand.IsRunning);
        }






        List<(DateTime RestTime, DateTime OpenTime)> SleepRangeList = new();
        Task? SleepSettingTask;
        CancellationTokenSource? SleepSettingCts;
        public void SleepSettingStart()
        {
            if (SleepSettingTask == null)
            {
                SleepSettingCts = new CancellationTokenSource();
                SleepSettingTask = Task.Run(async () =>
                {
                    TaskCompletionSource<bool> tcs = new();
                    SleepModeIsActivated = true;
                    try
                    {
                        while (true)
                        {
                            //若停留在計時頁面
                            if (ParameterSettingVM.TbtnTimimgSettingIsChecked)
                                continue;


                            if (SleepSettingCts.Token.IsCancellationRequested)
                                SleepSettingCts.Token.ThrowIfCancellationRequested();
                            try
                            {
                                //休眠
                                //找出現在是否在自動狀態
                                //   DateTime.Now
                                var EnableTimingControl = ParameterSettingVM.TimingSettingVM.TimingControlVMCollection.Where(x => x.IsEnable).ToList();

                                //若目前時間落在之間
                                if (!SleepRangeList.Exists(x => x.RestTime < DateTime.Now && DateTime.Now < x.OpenTime))
                                {
                                    var restTimeRange = EnableTimingControl.Where(x => x.DateTimeIsBetween(DateTime.Now)).ToList();
                                    if (restTimeRange.Count != 0)
                                    {
                                        //找出啟動和休息的最大區間
                                        List<DateTime> restList = new List<DateTime>();
                                        List<DateTime> openList = new List<DateTime>();
                                        foreach (var timeRange in restTimeRange)
                                        {
                                            var todayDate = DateTime.Now.Date;
                                            DateTime rtime, oTime;
                                            if (timeRange.OpenTime >= timeRange.RestTime)
                                            {
                                                rtime = todayDate.AddTicks(timeRange.RestTime.Ticks);
                                                oTime = todayDate.AddTicks(timeRange.OpenTime.Ticks);
                                            }
                                            else
                                            {
                                                rtime = todayDate.AddTicks(timeRange.RestTime.Ticks).AddDays(-1);
                                                oTime = todayDate.AddTicks(timeRange.OpenTime.Ticks);
                                            }

                                            restList.Add(rtime);
                                            openList.Add(oTime);
                                        }
                                        DateTime restMin = restList.Min();
                                        DateTime openMax = openList.Max();
                                        //在休息時間內不會再問是否要啟動
                                        //sleeptime
                                        SleepRangeList.Add(new(restMin, openMax));

                                        //跳出等待
                                        string Outputting = "";
                                        var sleepString=  (string)System.Windows.Application.Current.TryFindResource("Text_MachiningSleeping");
                                        if (Outputting != null)
                                        {
                                            Outputting = string.Format(sleepString, openMax.ToString("HH:mm:ss"));
                                        }

                                        var result = new MessageBoxResultShow("", Outputting, MessageBoxButton.OK, GD_MessageBoxNotifyResult.NotifyBl);


                                        CancellationTokenSource cts = new CancellationTokenSource();
                                        //彈出式視窗關閉
                                        var showTask = Task.Run(() => result.ShowMessageBox());
                                        //等待時間到
                                        var waitTask = Task.Run(async () =>
                                        {
                                            try
                                            {
                                                while (true)
                                                {
                                                    if (SleepSettingCts.Token.IsCancellationRequested)
                                                    {
                                                        cts.Cancel();
                                                    }

                                                    if (cts.Token.IsCancellationRequested)
                                                    {
                                                        cts.Token.ThrowIfCancellationRequested();
                                                    }

                                                    if (DateTime.Now > openMax)
                                                    {
                                                        break;
                                                    }
                                                    await Task.Delay(1000);
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }, cts.Token);
                                        //機台停止

                                        var originMode = StampMachineData.OperationMode;
                                        var machineSleepTask = Task.Run(async () =>
                                        {
                                            var ret = false;
                                            try
                                            {
                                                if (StampMachineData.IsConnected)
                                                {
                                                    if ((StampMachineData.OperationMode == OperationModeEnum.FullAutomatic ||
                                                    StampMachineData.OperationMode == OperationModeEnum.HalfAutomatic))
                                                    {
                                                        await StampMachineData.SetOperationModeAsync(OperationModeEnum.HalfAutomatic);
                                                        //等待機台完成工作
                                                        await Task.Delay(500);
                                                        await WaitForCondition.WaitAsync(() => StampMachineData.StoppedLamp, true, cts.Token);
                                                        await Task.Delay(100);
                                                        await StampMachineData.SetOperationModeAsync(OperationModeEnum.Setup);
                                                        await Task.Delay(500);
                                                        await StampMachineData.SetHydraulicPumpMotorAsync(false);
                                                        await WaitForCondition.WaitAsync(() => StampMachineData.HydraulicPumpIsActive, false, cts.Token);
                                                        //關閉油壓

                                                        ret = true;
                                                    }
                                                }
                                                else
                                                {
                                                    var (result, value) = await StampMachineData.GetHydraulicPumpMotorAsync();
                                                    if(result && value)
                                                    {
                                                        await StampMachineData.SetOperationModeAsync(OperationModeEnum.Setup);
                                                        await Task.Delay(100);
                                                        await StampMachineData.SetHydraulicPumpMotorAsync(false);
                                                    }
                                                }

                                            }
                                            catch (OperationCanceledException)
                                            {
                                                Debugger.Break();
                                            }
                                            catch (Exception)
                                            {

                                            }
                                            return ret;
                                        });

                                        //監視機台停止後是否有其他行為
                                        var MonitorMachineAwakeTask = Task.Run(async () =>
                                        {
                                            try
                                            {
                                                await Task.WhenAny(machineSleepTask).ConfigureAwait(false);
                                                var isNotSetupAutoTask = WaitForCondition.WaitChangeAsync(() => StampMachineData.OperationMode, cts.Token);
                                                var isStartTask = WaitForCondition.WaitChangeAsync(() => StampMachineData.RunningLamp, cts.Token);
                                                var isStopTask = WaitForCondition.WaitChangeAsync(() => StampMachineData.StoppedLamp, cts.Token);
                                                var isAlarmTask = WaitForCondition.WaitChangeAsync(() => StampMachineData.AlarmLamp, cts.Token);
                                                var isESPTask = WaitForCondition.WaitChangeAsync(() => StampMachineData.DI_EmergencyStop1, cts.Token);
                                                await Task.WhenAny(isNotSetupAutoTask, isStartTask, isStopTask, isAlarmTask, isESPTask);
                                            }
                                            catch
                                            {

                                            }
                                        });


                                        if (waitTask == await Task.WhenAny(waitTask, showTask, MonitorMachineAwakeTask))
                                        {
                                            //正常結束休眠 可被喚醒
                                            try
                                            {
                                                //機台有進行休眠流程且沒有被打斷
                                                var IsSleeped = await machineSleepTask;
                                                if (IsSleeped)
                                                {
                                                    if (await StampMachineData.SetHydraulicPumpMotorAsync(true))
                                                    {
                                                        //馬達啟動超時
                                                        var MotorOutTimeCts = new CancellationTokenSource(15000);
                                                        try
                                                        {
                                                            await WaitForCondition.WaitAsync(() => StampMachineData.HydraulicPumpIsActive, true, cts.Token, MotorOutTimeCts.Token);
                                                            if (await StampMachineData.SetOperationModeAsync(originMode))
                                                            {
                                                                await WaitForCondition.WaitAsync(() => StampMachineData.OperationMode, originMode, cts.Token);
                                                                if (originMode == OperationModeEnum.FullAutomatic)
                                                                    await StampMachineData.CycleStartAsync();
                                                            }
                                                        }
                                                        catch (OperationCanceledException cex)
                                                        {
                                                            if(cex.CancellationToken == MotorOutTimeCts.Token)
                                                            {
                                                                //馬達啟動超時
                                                               _ =  MessageBoxResultShow.ShowOKAsync("", (string)System.Windows.Application.Current.TryFindResource("Text_HydraulicPumpMotorActivatedFailure") , GD_MessageBoxNotifyResult.NotifyRd , false);
                                                            }



                                                        }
                                                        catch(Exception ex)
                                                        {
                                                        Debug.WriteLine(ex.ToString());
                                                        }

                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }

                                       
                                        //機台上做出特定行為導致被中斷
                                        //關閉視窗
                                        await result.CloseMessageBoxAsync();
                                        //被中斷
                                        cts?.Cancel();
                                    }
                                }
                            }
                            catch(OperationCanceledException)
                            {

                            }
                            catch(Exception ex)
                            {
                                Debug.WriteLine(ex.ToString());

                            }
                            await Task.Delay(100);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        tcs.SetResult(false);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                    SleepModeIsActivated = false;

                    return tcs.Task;
                }, SleepSettingCts.Token);
            }
        }

        public async Task SleepSettingStopAsync()
        {
            if (SleepSettingTask != null)
            {
                try
                {
                    SleepSettingCts?.Cancel();
                    // if(SleepSettingTask.Status == TaskStatus.Running)
                    await SleepSettingTask;
                }
                catch (OperationCanceledException e)
                {
                    Debug.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
                }
                catch
                {

                }
                finally
                {
                    SleepSettingTask.Dispose();
                    SleepSettingCts?.Dispose();
                    SleepSettingTask = null;
                    SleepSettingCts = null;
                }
            }
        }







    }
}
