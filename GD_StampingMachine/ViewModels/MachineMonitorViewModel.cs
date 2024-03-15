using CommunityToolkit.Mvvm.Input;
using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.DataAccess.Json;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels.MachineMonitor;
using GD_StampingMachine.ViewModels.ProductSetting;
using GD_StampingMachine.Windows;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static GD_StampingMachine.Singletons.StampMachineDataSingleton.StampingOpcUANode;

namespace GD_StampingMachine.ViewModels
{
    /*public class MachineMonitorModel
    {
        public ProjectDistributeViewModel ProjectDistributeVM { get; set; }
    }*/

    public class MachineMonitorViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("btnDescription_MachineMonitoring");

        public MachineMonitorViewModel()
        {

            StampingMachineSingleton.Instance.SelectedProjectDistributeVMChanged += (s, e) =>
            {
                SelectedProjectDistributeVM = e.NewValue;
            };
        }

        StampMachineDataSingleton StampMachineData => GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance;
        ProductSettingViewModel? ProductSettingVM => GD_StampingMachine.Singletons.StampingMachineSingleton.Instance.ProductSettingVM;

        private ProjectDistributeViewModel? _selectedProjectDistributeVM;
        public ProjectDistributeViewModel? SelectedProjectDistributeVM
        {
            get => _selectedProjectDistributeVM;  private set { _selectedProjectDistributeVM = value; OnPropertyChanged(); }
        }







        /// <summary>
        /// 鋼帶捲集合
        /// </summary>
        //private ObservableCollection<StampingSteelBeltViewModel> _stampingSteelBeltVMObservableCollection = new();
        //public ObservableCollection<StampingSteelBeltViewModel> StampingSteelBeltVMObservableCollection { get => _stampingSteelBeltVMObservableCollection; set { _stampingSteelBeltVMObservableCollection = value; OnPropertyChanged(); } }







        private double _sendMachiningProgress;
        public double SendMachiningProgress
        {
            get => _sendMachiningProgress;
            set
            {
                _sendMachiningProgress = value; OnPropertyChanged();
            }
        }






        /*private bool _showIsSended = true;
        private bool _showDataMatrixIsFinish = true;
        private bool _showEngravingIsFinish = true;
        private bool _showShearingIsFinish = true;
        private bool _showIsFinish = true;*/

        bool _showIsTransported = false;
        public bool ShowIsTransported
        {
            get => _showIsTransported;
            set
            {
                _showIsTransported = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(BoxPartsParameterVMRowFilterCommand));
            }
        }

        bool _testedIsChecked = false;
        public bool TestedIsChecked
        {
            get => _testedIsChecked;
            set
            {
                _testedIsChecked = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TestedIsChecked));
            }
        }
        


        private AsyncRelayCommand? _sendMachiningCommand;
        public AsyncRelayCommand SendMachiningCommand
        {
            get => _sendMachiningCommand ??= new(async (CancellationToken token) =>
            {
                try
                {
                    PreviousFirstIronPlateID = StampMachineData.PlateBaseObservableCollection.FirstOrDefault()?.ID;
                    PreviousMiddleIronPlateID = StampMachineData.PlateBaseObservableCollection.LastOrDefault(x => x.EngravingIsFinish)?.ID;
                    PreviousLasttIronPlateID = StampMachineData.PlateBaseObservableCollection.LastOrDefault()?.ID;

                    //開始依序傳送資料
                    await Task.Run(async () =>
                     {
                         try
                         {
                             StampMachineData.PlateBaseObservableCollection.CollectionChanged += PlateBaseObservableCollection_CollectionChanged;
                             StampMachineData.Cylinder_HydraulicEngraving_IsStopDownChanged += StampMachineData_Cylinder_HydraulicEngraving_IsStopDownChanged;
                             StampMachineData.Cylinder_HydraulicCutting_IsCutPointChanged += StampMachineData_Cylinder_HydraulicCutting_IsCutPointChanged;
                             while (true)
                             {
                                 await Task.Delay(50,token);
                                 if (token.IsCancellationRequested)
                                     token.ThrowIfCancellationRequested();
                                 if (!StampMachineData.IsConnected)
                                 {
                                     var ConnectManagerVM = new DXSplashScreenViewModel
                                     {
                                         Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Image/svg/NewLogo_1-2.svg"),
                                         Title = "GD_StampingMachine",
                                         Status = (string)System.Windows.Application.Current.TryFindResource("WaitConnection"),
                                         Progress = 0,
                                         IsIndeterminate = true,
                                         Subtitle = "",
                                         Copyright = "Copyright © 2023 GUANDA",
                                     };
                                     SplashScreenManager ConnectManager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.ProcessingScreenWindow(), ConnectManagerVM);
                                     try
                                     {
                                         _ = Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("WaitConnection"));
                                         await Application.Current.Dispatcher.InvokeAsync(() =>
                                         {
                                             ConnectManager.Show(Application.Current?.MainWindow, WindowStartupLocation.CenterScreen, true, InputBlockMode.None);
                                         });
                                         //等待連線
                                         await Task.Delay(10,token);
                                         CancellationTokenSource cts = new(60000);

                                         await WaitForCondition.WaitAsync(() => StampMachineData.IsConnected,true, token, cts.Token);
                                     }
                                     catch(OperationCanceledException ocex)
                                     {
                                         //外部解除
                                         if(ocex.CancellationToken == token)
                                         {
                                             throw;
                                         }
                                     }
                                     catch(Exception ex)
                                     {
                                         break;
                                     }
                                     finally
                                     {
                                         ConnectManager.Close();
                                     }
                                 }
                                 //燈號
                                 //StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMCollection
                                 //取得歷史資料
                                 //var history = await StampMachineData.GetIronPlateDataCollection();

                                 var workableMachiningCollection = SelectedProjectDistributeVM?.StampingBoxPartsVM.SeparateBoxVMObservableCollection.ScheduledPartsParameterCollection.Where(x => x.WorkIndex >= 0).ToList();
                                 if (workableMachiningCollection != null)
                                 {
                                     //準備加工(未上傳)
                                     var readyMachiningCollection = workableMachiningCollection.Where(x => !x.IsSended && !x.IsFinish).OrderBy(x => x.WorkIndex);
                                     //已上傳
                                     var sendedReadyMachiningCollection = workableMachiningCollection.Where(x => x.IsSended).OrderBy(x => x.WorkIndex);

                                     var readyMachining = readyMachiningCollection?.First();
                                     if (readyMachining == null || readyMachiningCollection == null)
                                     {
                                         var result = await MessageBoxResultShow.ShowAsync(null, string.Empty, (string)Application.Current.TryFindResource("NoneMachiningData"), MessageBoxButton.OK, GD_MessageBoxNotifyResult.NotifyRd);
                                         _ = Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("NoneMachiningData"));
                                         break;
                                     }
                                     else
                                     {
                                         //檢查：鐵牌上的字是否存在於輪盤上
                                         foreach (var machining in readyMachiningCollection)
                                         {
                                             var missingChars = machining.ParameterA.Where(x => !StampMachineData.RotatingTurntableInfoCollection.Any(y => y.StampingTypeString.Contains(x)));

                                         }


                                         double progress = 0;
                                         if (sendedReadyMachiningCollection != null && workableMachiningCollection != null)
                                             progress = ((double)sendedReadyMachiningCollection.Count() * 100) / (double)workableMachiningCollection.Count();

                                         SendMachiningProgress = progress;


                                         string plateFirstValue = "";
                                         string plateSecondValue = "";
                                         readyMachining.SettingBaseVM.PlateNumberList1.ForEach(p =>
                                         {
                                             if (string.IsNullOrWhiteSpace(p.FontString))
                                             {
                                                 plateFirstValue += " ";
                                             }
                                             else
                                             {
                                                 plateFirstValue += p.FontString;
                                             }
                                         });

                                         readyMachining.SettingBaseVM.PlateNumberList2.ForEach(p =>
                                         {
                                             if (string.IsNullOrWhiteSpace(p.FontString))
                                             {
                                                 plateSecondValue += " ";
                                             }
                                             else
                                             {
                                                 plateSecondValue += p.FontString;
                                             }
                                         });

                                         plateFirstValue = plateFirstValue.TrimEnd();
                                         plateSecondValue = plateSecondValue.TrimEnd();

                                         plateFirstValue ??= string.Empty;
                                         plateSecondValue ??= string.Empty;

                                         /*
                                          * rXAxisPos1 = 10,
                                          * rYAxisPos1 = 119,
                                          * rXAxisPos2 = 25,
                                          * rYAxisPos2 = 119,
                                          * */
                                         //產生一個獨有的id
                                         //生成一個

                                         //List<PartsParameterViewModel> allBoxPartsParameterViewModel = new();
                                         //所有排版專案
                                         /*foreach (var productProject in ProductSettingVM.ProductProjectVMCollection)
                                         {
                                             allBoxPartsParameterViewModel.AddRange(productProject.PartsParameterVMObservableCollection);
                                         }*/
                                         var allBoxPartsParameterViewModel = ProductSettingVM.ProductProjectVMCollection
                                         .SelectMany(productProject => productProject.PartsParameterVMObservableCollection);

                                         HashSet<int> existingIds = new(allBoxPartsParameterViewModel.Select(x => x.ID));
                                         int hashGuid = 0;
                                         do
                                         {
                                             hashGuid = Math.Abs(Guid.NewGuid().GetHashCode());
                                             //檢查是否有存在該編號
                                         } while (existingIds.Contains(hashGuid));


                                         var boxIndex = readyMachining.BoxIndex != null ? readyMachining.BoxIndex.Value : 0;

                                         //readyMachining.SettingBaseVM.IronPlateMarginVM.A_Margin
                                         var _HMIIronPlateData = new IronPlateDataModel
                                         {
                                             bEngravingFinish = false,
                                             bDataMatrixFinish = false,
                                             //流水編號
                                             iIronPlateID = hashGuid,
                                             iStackingID = boxIndex,
                                             rXAxisPos1 = readyMachining.SettingBaseVM.StampingMarginPosVM.rXAxisPos1, //     10
                                             rXAxisPos2 = readyMachining.SettingBaseVM.StampingMarginPosVM.rXAxisPos2,//25
                                             rYAxisPos1 = readyMachining.SettingBaseVM.StampingMarginPosVM.rYAxisPos1 + MachineConst.StampingMachineYPosition, //119 = 14+105 //
                                             rYAxisPos2 = readyMachining.SettingBaseVM.StampingMarginPosVM.rYAxisPos1 + MachineConst.StampingMachineYPosition, //*14+105 // 
                                             sDataMatrixName1 = string.IsNullOrWhiteSpace(readyMachining.ParameterC) ? string.Empty : readyMachining.ParameterC,
                                             sDataMatrixName2 = string.IsNullOrWhiteSpace(readyMachining.QR_Special_Text) ? string.Empty : readyMachining.QR_Special_Text,
                                             sIronPlateName1 = string.IsNullOrWhiteSpace(plateFirstValue) ? string.Empty : plateFirstValue,
                                             sIronPlateName2 = string.IsNullOrWhiteSpace(plateSecondValue) ? string.Empty : plateSecondValue
                                         };


                                         try
                                         {
                                             _ = Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("Connection_WaitRequsetSignal"));

                                             var Rdatabit = false;
                                             var requestDatabit = StampMachineData.GetRequestDatabitAsync();
                                             if ((await requestDatabit).Item1)
                                                 Rdatabit = (await requestDatabit).Item2;

                                             if (!Rdatabit)
                                                 await WaitForCondition.WaitIsTrueAsync(() => StampMachineData.Rdatabit, 2000, token);
                                         }
                                         catch
                                         {
                                             await Task.Delay(1000, token);
                                             continue;
                                         }

                                         var sendhmi = false;
                                         do
                                         {
                                             if (token.IsCancellationRequested)
                                                 token.ThrowIfCancellationRequested();
                                             _ = Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("Connection_WritingMachiningData"));
                                             sendhmi = await StampMachineData.SetHMIIronPlateDataAsync(_HMIIronPlateData);
                                             //hmi設定完之後還需要進行設定變更!
                                             if (sendhmi)
                                             {
                                                 do
                                                 {
                                                     if (token.IsCancellationRequested)
                                                         token.ThrowIfCancellationRequested();
                                                     try
                                                     {
                                                         if (await StampMachineData.SetRequestDatabitAsync(false))
                                                         {
                                                             await WaitForCondition.WaitAsyncIsFalse(() => StampMachineData.Rdatabit, token);
                                                             break;
                                                         }
                                                     }
                                                     catch
                                                     {
                                                         await Task.Delay(500, token);
                                                         await Task.Yield();
                                                     }
                                                 }
                                                 while (true);

                                                 await Task.Delay(1000, token);
                                                 /*ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_WritingMachiningDataSuccessful");
                                                 Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ManagerVM.Status);*/
                                                 readyMachining.ID = hashGuid;
                                                 _ = Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("Connection_WritingMachiningDataSuccessful"));
                                             }
                                         }
                                         while (!sendhmi);

                                         //readyMachining.IsSended = true;

                                         //等待最後一支變成id
                                         //   await WaitForCondition.WaitAsync(() => StampMachineData.LastIronPlateID.oldValue, readyMachining.ID, token);
                                         //ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_MachiningIsProcessing");
                                         // await Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ManagerVM.Status);
                                         await Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("Connection_MachiningIsProcessing"));

                                         readyMachining.IsSended = true;
                                         await SelectedProjectDistributeVM?.SaveProductProjectVMCollectionAsync();

                                         await Task.Delay(2000, token);
                                         await Task.Yield();
                                     }
                                 }
                             }
                         }
                         catch(Exception ex)
                         {
                             _ = LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ex.Message);
                         }
                         finally
                         {
                             StampMachineData.PlateBaseObservableCollection.CollectionChanged -= PlateBaseObservableCollection_CollectionChanged;
                             StampMachineData.Cylinder_HydraulicEngraving_IsStopDownChanged -= StampMachineData_Cylinder_HydraulicEngraving_IsStopDownChanged;
                             StampMachineData.Cylinder_HydraulicCutting_IsCutPointChanged -= StampMachineData_Cylinder_HydraulicCutting_IsCutPointChanged;

                         }
                     }, token);
                }
                catch (OperationCanceledException ocex)
                {
                    _ = LogDataSingleton.Instance.AddLogDataAsync(ViewModelName, ocex.Message);
                }
                catch (Exception ex)
                {
                    _ = LogDataSingleton.Instance.AddLogDataAsync(ViewModelName, ex.Message);
                    MessageBox.Show(ex.Message);
                }
                finally
                {

                    //cts.Cancel();
                    //取消第一格的訂閱
                    _ = Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("Connection_MachiningProcessEnd"));

                    //ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_MachiningProcessEnd");
                    //await Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ManagerVM.Status);
                    //await Task.Delay(1000);
                    //manager?.Close();
                    SendMachiningProgress = 0;

                }
            }, () => !SendMachiningCommand.IsRunning && !CompleteMachiningDataCommand.IsRunning);
        }


        private int? PreviousFirstIronPlateID ;
        private int? PreviousMiddleIronPlateID;
        private int? PreviousLasttIronPlateID;



        private void PlateBaseObservableCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (sender is ICollection<PlateMonitorViewModel> PlateCollection)
                {
                    var PartCollection = ProductSettingVM.ProductProjectVMCollection.SelectMany(x => x.PartsParameterVMObservableCollection);

                    if (PlateCollection.FirstOrDefault()?.ID != PreviousFirstIronPlateID)
                    {
                        if (PreviousFirstIronPlateID != null)
                        {
                            var FirstPart = PartCollection.FirstOrDefault(x => x.ID == PreviousFirstIronPlateID); 
                            if (FirstPart != null)
                            {
                                FirstPart.FinishProgress = 100;
                                FirstPart.IsFinish = true;
                            }
                        }
                    }

                    if (PlateCollection.LastOrDefault(x=> x.EngravingIsFinish)?.ID != PreviousMiddleIronPlateID)
                    {
                        if (PreviousMiddleIronPlateID != null)
                        {
                            var MiddlePart = PartCollection.LastOrDefault(x => x.ID == PreviousMiddleIronPlateID);
                            if (MiddlePart != null)
                            {
                                if(MiddlePart.FinishProgress<66)
                                    MiddlePart.FinishProgress = 66;
                                //MiddlePart.EngravingIsFinish = true;
                            }
                        }
                    }

                    if (PlateCollection.LastOrDefault()?.ID != PreviousLasttIronPlateID)
                    {
                        if (PreviousLasttIronPlateID != null)
                        {
                            var LastPart = PartCollection.LastOrDefault(x => x.ID == PreviousLasttIronPlateID);
                            if (LastPart != null)
                            {

                                LastPart.FinishProgress = 33;
                                LastPart.DataMatrixIsFinish = true;
                            }
                        }
                    }



                    PreviousFirstIronPlateID = PlateCollection.FirstOrDefault()?.ID;
                    PreviousMiddleIronPlateID = PlateCollection.LastOrDefault(x=>x.EngravingIsFinish)?.ID;
                    PreviousLasttIronPlateID = PlateCollection.LastOrDefault()?.ID;
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void StampMachineData_Cylinder_HydraulicCutting_IsCutPointChanged(object? sender, GD_CommonLibrary.ValueChangedEventArgs<bool> e)
        {
            try
            {
                if (e.NewValue)
                {
                    //取得id
                    var eRotateStation = StampMachineData.EngravingRotateStation;
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            // 鐵片
                            var firstIronID = StampMachineData.PlateBaseObservableCollection.FirstOrDefault()?.ID;

                            // 查找具有相同ID 
                            var project = StampingMachineSingleton.Instance.TypeSettingSettingVM?.ProjectDistributeVMObservableCollection
                                .SelectMany(x => x.ProductProjectVMCollection)
                                .FirstOrDefault(x => x.PartsParameterVMObservableCollection.Any(y => y.ID == firstIronID));

                            if (project != null)
                            {
                                var finishPartParameter = project.PartsParameterVMObservableCollection.FirstOrDefault(x => x.ID == firstIronID);
                                if (finishPartParameter != null)
                                {
                                    finishPartParameter.FinishProgress = 99;
                                    finishPartParameter.ShearingIsFinish = true;
                                    await project.SaveProductProjectAsync();
                                }
                            }
                        }
                        catch
                        {

                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _ = LogDataSingleton.Instance.AddLogDataAsync(ViewModelName, ex.Message);
            }


        }

        private double _completeMachiningProgress;
        public double CompleteMachiningProgress { get => _completeMachiningProgress; set { _completeMachiningProgress = value; OnPropertyChanged(); } }


        private AsyncRelayCommand? _completeMachiningDataCommand;
        /// <summary>
        /// 將剩下的工作做完->一直傳送空字串直到沒有任何料為止
        /// </summary>
        public AsyncRelayCommand CompleteMachiningDataCommand
        {
            get => _completeMachiningDataCommand ??= new(async (CancellationToken token) =>
            {
                var ManagerVM = new DXSplashScreenViewModel
                {
                    Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Image/svg/NewLogo_1-2.svg"),
                    Title = "GD_StampingMachine",
                    Status = (string)System.Windows.Application.Current.TryFindResource("Connection_MachiningProcessStart"),
                    Progress = 0,
                    IsIndeterminate = false,
                    Subtitle = "",
                    Copyright = "Copyright © 2023 GUANDA",
                };
                SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.ProcessingScreenWindow(), ManagerVM);

                manager.Show(Application.Current.MainWindow, WindowStartupLocation.CenterScreen, true, InputBlockMode.None);
                try
                {
                    PreviousFirstIronPlateID = StampMachineData.PlateBaseObservableCollection.FirstOrDefault()?.ID;
                    PreviousMiddleIronPlateID = StampMachineData.PlateBaseObservableCollection.LastOrDefault(x => x.EngravingIsFinish)?.ID;
                    PreviousLasttIronPlateID = StampMachineData.PlateBaseObservableCollection.LastOrDefault()?.ID;

                    StampMachineData.PlateBaseObservableCollection.CollectionChanged += PlateBaseObservableCollection_CollectionChanged;

                    StampMachineData.Cylinder_HydraulicEngraving_IsStopDownChanged += StampMachineData_Cylinder_HydraulicEngraving_IsStopDownChanged;
                    StampMachineData.Cylinder_HydraulicCutting_IsCutPointChanged += StampMachineData_Cylinder_HydraulicCutting_IsCutPointChanged;

                    int? isNeedWorkListLengthInit = null;
                    while (true)
                    {
                        try
                        {
                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();

                            if (!StampMachineData.IsConnected)
                            {
                                _ = Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("WaitConnection"));
                                ManagerVM.Status = (string)Application.Current.TryFindResource("WaitConnection");

                                await WaitForCondition.WaitIsTrueAsync(() => StampMachineData.IsConnected, token);
                                ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_MachiningProcessStart");
                            }

                            var (result, ironPlateDataCollection) = await StampMachineData.GetIronPlateDataCollectionAsync();
                            if (!result)
                            {
                                manager?.Close();
                                await MessageBoxResultShow.ShowOKAsync(null,
                                    (string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("MachineOffline"), GD_MessageBoxNotifyResult.NotifyBl);
                                break;
                            }

                            //檢查清單內若有待加工(id !=0 或 四種加工資料任一欄有值)
                            var isNeedWorkList = ironPlateDataCollection.FindAll(x => x.iIronPlateID != 0 ||
                            !string.IsNullOrEmpty(x.sDataMatrixName1) ||
                            !string.IsNullOrEmpty(x.sDataMatrixName2) ||
                            !string.IsNullOrEmpty(x.sIronPlateName1) ||
                            !string.IsNullOrEmpty(x.sIronPlateName2));
                            ManagerVM.Subtitle = (string)Application.Current.TryFindResource("RemainingMachiningData") + " = " + isNeedWorkList.Count;

                            //初始化
                            if (!isNeedWorkListLengthInit.HasValue)
                                isNeedWorkListLengthInit = isNeedWorkList.Count;

                            if (isNeedWorkListLengthInit.HasValue)
                            {
                                if (isNeedWorkListLengthInit == 0)
                                {
                                    ManagerVM.Progress = 100;
                                }
                                else
                                {
                                    ManagerVM.Progress = ((double)(isNeedWorkListLengthInit.Value - isNeedWorkList.Count) * 100) / isNeedWorkListLengthInit.Value;
                                    CompleteMachiningProgress = ManagerVM.Progress;
                                }
                            }

                            if (isNeedWorkList.Count == 0)
                            {
                                manager?.Close();
                                await MessageBoxResultShow.ShowOKAsync(null,
                                    (string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("NoneMachiningData"), GD_MessageBoxNotifyResult.NotifyBl);
                                break;
                            }

                            ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_WaitRequsetSignal");
                            ManagerVM.Subtitle = null;

                            //等待加工訊號
                            try
                            {
                                await WaitForCondition.WaitAsync(() => StampMachineData.Rdatabit, true, 5000);
                            }
                            catch
                            {
                                continue;
                            }

                            var _HMIIronPlateData = new IronPlateDataModel
                            {
                                bEngravingFinish = false,
                                bDataMatrixFinish = false,
                                //流水編號
                                iIronPlateID = 0,
                                iStackingID = 0,
                                rXAxisPos1 = 10,
                                rXAxisPos2 = 25,
                                rYAxisPos1 = 119,
                                rYAxisPos2 = 119,
                                sDataMatrixName1 = string.Empty,
                                sDataMatrixName2 = string.Empty,
                                sIronPlateName1 = string.Empty,
                                sIronPlateName2 = string.Empty
                            };
                            var sendhmi = false;
                            do
                            {
                                if (token.IsCancellationRequested)
                                    token.ThrowIfCancellationRequested();
                                // var send = StampMachineData.AsyncSendMachiningData(readyMachining.SettingBaseVM, token, int.MaxValue);

                                ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_WritingMachiningData");
                                sendhmi = await StampMachineData.SetHMIIronPlateDataAsync(_HMIIronPlateData);
                                //hmi設定完之後還需要進行設定變更!
                                if (sendhmi)
                                {
                                    bool setRequestDatabitSuccesfful = false;
                                    do
                                    {
                                        if (await StampMachineData.SetRequestDatabitAsync(false))
                                        {
                                            await WaitForCondition.WaitAsyncIsFalse(() => StampMachineData.Rdatabit, token);
                                        }
                                        await Task.Delay(100, token);
                                    }
                                    while (!setRequestDatabitSuccesfful);
                                    ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_WritingMachiningDataSuccessful");

                                    await Task.Delay(100, token);
                                }
                                await Task.Delay(100, token);
                            }
                            while (!sendhmi);
                        }
                        catch (OperationCanceledException ocex)
                        {
                            throw;
                        }
                        catch(Exception ex)
                        {
                            _ = LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ex, true);
                        }
                    }
                }
                catch (OperationCanceledException ocex)
                {
                    _ = LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ocex, false);
                }
                catch (Exception ex)
                {
                    _ = LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ex , true);
                }
                finally
                {
                    StampMachineData.PlateBaseObservableCollection.CollectionChanged -= PlateBaseObservableCollection_CollectionChanged;

                    StampMachineData.Cylinder_HydraulicEngraving_IsStopDownChanged -= StampMachineData_Cylinder_HydraulicEngraving_IsStopDownChanged;
                    StampMachineData.Cylinder_HydraulicCutting_IsCutPointChanged -= StampMachineData_Cylinder_HydraulicCutting_IsCutPointChanged;

                    //cts.Cancel();
                    ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_MachiningProcessEnd");
                    manager?.Close();
                    CompleteMachiningProgress = 0;
                }
            }, () => !CompleteMachiningDataCommand.IsRunning && !SendMachiningCommand.IsRunning);
        }

        /*
        [Obsolete]
        private async Task MonitorFirstIronPlateAsync(Func<(int oldValue, int NewValue)> ironPlateID, CancellationTokenSource cts)
        {
            //等待值
            try
            {
                var token = cts.Token;
                //int previousID = 0;
                while (!token.IsCancellationRequested)
                {
                    //取得上一筆
                    var lastValue = ironPlateID();
                    await WaitForCondition.WaitAsync(ironPlateID, lastValue, false, token);
                    //取得id
                    //改為訂閱第一片id

                    //var getIdAsync = await StampMachineData.GetFirstIronPlateIDAsync();
                   //  if (getIdAsync.Item1)
                    // {
                    _ = Task.Run(async () =>
                    {
                        foreach (var projectDistributeVM in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                        {
                            var finishPartParameter = projectDistributeVM.StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x=>x.BoxPartsParameterVMCollection).FirstOrDefault(x => x.ID == ironPlateID().oldValue);

                            if (finishPartParameter != null)
                            {
                                finishPartParameter.FinishProgress = 100;d
                                finishPartParameter.ShearingIsFinish = true;
                                finishPartParameter.IsFinish = true;
                                try
                                {
                                    await projectDistributeVM.SaveProductProjectVMCollectionAsync();
                                }
                                catch
                                {

                                }
                                break;
                            }
                        }
                    });
                    // StampMachineData.GetIronPlateDataCollectionAsync

                    //切割一片後記錄
                    //StampingMachineJsonHelper JsonHM = new();
                    //await JsonHM.WriteJsonFileAsync(Path.Combine(ProductProject.ProjectPath, ProductProject.Name), ProductProject);

                    //  }
                    //等待彈起
                }
            }
            catch (Exception ex)
            {

                await LogDataSingleton.Instance.AddLogDataAsync(ViewModelName, ex.Message);
            }
        }*/
        /*
        [Obsolete]
        private async Task MonitorLastIronPlateAsync(Func<(int oldValue, int newValue)> ironPlateID, CancellationTokenSource cts)
        {
            try
            {
                var token = cts.Token;
                while (!token.IsCancellationRequested)
                {
                    //取得上一筆
                    var lastValue = ironPlateID();
                    await WaitForCondition.WaitAsync(ironPlateID, lastValue, false, token);

                    _ = Task.Run(async () =>
                    {
                        foreach (var projectDistributeVM in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                        {
                            var finishPartParameter = projectDistributeVM.StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x => x.BoxPartsParameterVMCollection).FirstOrDefault(x => x.ID == ironPlateID().oldValue);
                            if (finishPartParameter != null)
                            {
                                finishPartParameter.FinishProgress = 33;
                                finishPartParameter.DataMatrixIsFinish = true;
                                try
                                {
                                    await projectDistributeVM.SaveProductProjectVMCollectionAsync();
                                }
                                catch
                                {

                                }
                                break;
                            }
                        }
                    });
                }

            }
            catch
            {
            }
        }*/
        /*
        [Obsolete]
        private async Task MonitorStampingFontAsync(Func<bool> IsStopDown, CancellationTokenSource cts)
        {
            //等待值
            try
            {
                var token = cts.Token;
                //    int previousStation = 0;
                while (!token.IsCancellationRequested)
                {
                    await WaitForCondition.WaitAsync(IsStopDown, true, token);
                    //取得id
                    var eRotateStation = StampMachineData.EngravingRotateStation;
                    _ = Task.Run(async () =>
                    {

                        if (Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection.TryGetValue(eRotateStation, out var stamptype))
                        {
                            stamptype.StampingTypeUseCount++;
                            await Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.SaveStampingTypeVMObservableCollectionAsync();
                        }

                        //找出正在被敲的那片id
                        //StampMachineData.Cylinder_GuideRod_Fixed_IsUp
                        var engravingTuple = await Singletons.StampMachineDataSingleton.Instance.GetEngravingIronPlateIDAsync();
                        if (engravingTuple.Item1)
                        {
                            foreach (var projectDistributeVM in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                            {
                                var EngravingID = engravingTuple.Item2;
                                var finishPartParameter = projectDistributeVM.StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x => x.BoxPartsParameterVMCollection).FirstOrDefault(x => x.ID == EngravingID);
                                if (finishPartParameter != null)
                                {
                                    if (finishPartParameter.FinishProgress < 66)
                                    {
                                        var numberLength = finishPartParameter.SettingBaseVM.PlateNumber.Replace(" ", string.Empty).Length;
                                        //敲一次就跳一次完成度 不會超過66
                                        finishPartParameter.FinishProgress += ((float)33.3 / numberLength);
                                    }

                                   // finishPartParameter.ShearingIsFinish = true;
                                   // finishPartParameter.IsFinish = true;
                                    try
                                    {
                                        await projectDistributeVM.SaveProductProjectVMCollectionAsync();
                                    }
                                    catch
                                    {

                                    }
                                    break;
                                }
                            }
                        }

                    });
                    await WaitForCondition.WaitAsync(IsStopDown, false, token);
                }
            }
            catch (Exception ex)
            {

                await LogDataSingleton.Instance.AddLogDataAsync(ViewModelName, ex.Message);
            }



        }
        */


        private void StampMachineData_Cylinder_HydraulicEngraving_IsStopDownChanged(object? sender, GD_CommonLibrary.ValueChangedEventArgs<bool> e)
        {
            //等待值
            try
            {
                if (e.NewValue)
                {
                    //取得id
                    var eRotateStation = StampMachineData.EngravingRotateStation;
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            //紀錄敲下去那一下是哪一顆字 並記錄次數
                            if(Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM!=null
                            && Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection.TryGetValue(eRotateStation, out var stamptype))
                            {
                                stamptype.StampingTypeUseCount++;
                                await Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.SaveStampingTypeVMObservableCollectionAsync();
                            }
                            // 鐵片
                            var engravingID = StampMachineData.PlateBaseObservableCollection.FirstOrDefault()?.ID;
                            
                            //找出正在被敲的那片id
                            var project = StampingMachineSingleton.Instance.TypeSettingSettingVM?.ProjectDistributeVMObservableCollection
                                .SelectMany(x => x.ProductProjectVMCollection)
                                .FirstOrDefault(x => x.PartsParameterVMObservableCollection.Any(y => y.ID == engravingID));

                            if (project != null)
                            {
                                var engravingPartParameter = project.PartsParameterVMObservableCollection.FirstOrDefault(x => x.ID == engravingID);
                                if (engravingPartParameter != null)
                                {
                                    if (engravingPartParameter.FinishProgress < 33)
                                        engravingPartParameter.FinishProgress = 33;

                                    if (engravingPartParameter.FinishProgress < 66)
                                    {
                                        var numberLength = engravingPartParameter.SettingBaseVM.PlateNumber.Replace(" ", string.Empty).Length;
                                        //敲一次就跳一次完成度 不會超過66
                                        engravingPartParameter.FinishProgress += ((float)33.3 / numberLength);
                                        await project.SaveProductProjectAsync();
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }

                    });


                }
            }
            catch (Exception ex)
            {
                _ = LogDataSingleton.Instance.AddLogDataAsync(ViewModelName, ex.Message);
            }
        }

        /// <summary>
        /// 剪切區的id變化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*private void StampMachineData_FirstIronPlateIDChanged(object? sender, GD_CommonLibrary.ValueChangedEventArgs<int> e)
        {

            try
            {
                foreach (var projectDistributeVM in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                {
                    var finishPartParameter = projectDistributeVM.StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x=>x.BoxPartsParameterVMCollection).FirstOrDefault(x => x.ID == e.OldValue);
                    if (finishPartParameter != null)
                    {
                        finishPartParameter.FinishProgress = 100;
                        finishPartParameter.ShearingIsFinish = true;
                        finishPartParameter.IsFinish = true;
                        _ = projectDistributeVM.SaveProductProjectVMCollectionAsync();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Debugger.Break();
            }
        }
        */
        /// <summary>
        /// 敲QR的id變化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*private void StampMachineData_LastIronPlateIDChanged(object? sender, GD_CommonLibrary.ValueChangedEventArgs<int> e)
        {
            try
            {
                foreach (var projectDistributeVM in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                {
                    var finishPartParameter = projectDistributeVM.StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x => x.BoxPartsParameterVMCollection).FirstOrDefault(x => x.ID == e.OldValue);
                    if (finishPartParameter != null)
                    {
                        finishPartParameter.FinishProgress = 33;
                        finishPartParameter.DataMatrixIsFinish = true;

                        _ = projectDistributeVM.SaveProductProjectVMCollectionAsync();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Debugger.Break();
            }
        }*/


        private AsyncRelayCommand? _sendMachiningCancelCommand;
        public AsyncRelayCommand SendMachiningCancelCommand
        {
            get => _sendMachiningCancelCommand ??= new (async () =>
            {
                try
                {
                    SendMachiningCommand.Cancel();
                    CompleteMachiningDataCommand.Cancel();
                    await WaitForCondition.WaitAsyncIsFalse(() => SendMachiningCommand.IsRunning);
                    await WaitForCondition.WaitAsyncIsFalse(() => CompleteMachiningDataCommand.IsRunning);
                }
                catch
                {

                }
            }, () => !SendMachiningCancelCommand.IsRunning);
        }

        private AsyncRelayCommand? _clearMachiningDataCommand;
        /// <summary>
        /// 清除所有資料
        /// </summary>
        public AsyncRelayCommand ClearMachiningDataCommand
        {
            get => _clearMachiningDataCommand ??= new(async (CancellationToken token) =>
            {
                try
                {
                    var result = MessageBoxResultShow.ShowYesNoAsync(null, (string)Application.Current.TryFindResource("Text_notify"),
                    (string)Application.Current.TryFindResource("Text_AskClearAllMachiningData"));
                    //跳出彈窗

                    if (await result != MessageBoxResult.Yes)
                    {
                        return;
                    }

                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    bool isGetIronPlate;
                    List<IronPlateDataModel> ironPlateDataCollection = new();
                    if (StampMachineData.IsConnected)
                    {
                        do
                        {
                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();

                            var getIronPlate = await StampMachineData.GetIronPlateDataCollectionAsync();
                            isGetIronPlate = getIronPlate.result;
                            ironPlateDataCollection = getIronPlate.ironPlateCollection;
                        } while (!isGetIronPlate);

                        var NewEmptyIronPlateDataCollection = Enumerable.Repeat(new IronPlateDataModel()
                        {
                            sDataMatrixName1 = string.Empty,
                            sDataMatrixName2 = string.Empty,
                            sIronPlateName1 = string.Empty,
                            sIronPlateName2 = string.Empty
                        }, ironPlateDataCollection.Count).ToList();

                        bool isSetIronPlate = false;
                        do
                        {
                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();
                            isSetIronPlate = await StampMachineData.SetIronPlateDataCollectionAsync(NewEmptyIronPlateDataCollection);
                        } while (!isSetIronPlate);




                    }
                    else
                    {
                        await MessageBoxResultShow.ShowOKAsync(null,(string)Application.Current.TryFindResource("Text_notify"),
                            (string)Application.Current.TryFindResource("MachineOffline") , GD_MessageBoxNotifyResult.NotifyRd);
                    }
                }
                catch (Exception ex)
                {
                    await LogDataSingleton.Instance.AddLogDataAsync(ViewModelName, ex.Message);
                    //跳出
                }
            }, () => !_clearMachiningDataCommand.IsRunning);
        }


        private AsyncRelayCommand<object>? _arrangeWorkCommand;
        /// <summary>
        /// 排定加工
        /// </summary>
        public AsyncRelayCommand<object> ArrangeWorkCommand
        {
            get => _arrangeWorkCommand ??= new AsyncRelayCommand<object>(async e =>
            {
                await Task.Run(async () =>
                {
                var partsParameterVMCollection = new List<PartsParameterViewModel>();

                if (e is IList<PartsParameterViewModel> Itemsources)
                {
                    partsParameterVMCollection = Itemsources.ToList();
                }
                else if (e is IEnumerable enumerableSources)
                {
                    foreach (var enumSource in enumerableSources)
                    {
                        if (enumSource is PartsParameterViewModel partsParameterVM)
                        {
                            partsParameterVMCollection.Add(partsParameterVM);
                        }
                    }
                }

                    await Task.Run(async() =>
                    {
                        try
                        {
                            if (SelectedProjectDistributeVM != null)
                            {
                                if (partsParameterVMCollection != null)
                                {
                                    if (partsParameterVMCollection.Count > 0)
                                    {
                                        var workableData = new List<PartsParameterViewModel>();
                                        var BoxCapacityDict = SelectedProjectDistributeVM.StampingBoxPartsVM
                                        .SeparateBoxVMObservableCollection
                                        .Where(box => box.BoxIsEnabled)
                                        .ToDictionary(box => box.BoxIndex,
                                        box => box.BoxSliderValue <= 0 ? double.MaxValue : box.BoxSliderValue - box.UnTransportedBoxPieceValue);

                                        var wData = partsParameterVMCollection.Where(x => x.WorkIndex < 0 && !x.IsFinish && !x.IsSended);
                                        foreach (var partsParameter in wData)
                                        {
                                            SelectedProjectDistributeVM.StampingBoxPartsVM.SeparateBoxVMObservableCollection.
                                            UnscheduledPartsParameterCollection.Count(x => x.BoxIndex == partsParameter.BoxIndex);
                                            /*var unTransportedCount = SelectedProjectDistributeVM.PartsParameterVMObservableCollection
                                                .Count(x => x.BoxIndex == partsParameter.BoxIndex && x.WorkIndex >= 0 && !x.IsTransported);*/
                                            if (partsParameter.BoxIndex is int boxIndex)
                                            {
                                                if (BoxCapacityDict.TryGetValue(boxIndex, out var Bvalue))
                                                {
                                                    if (Bvalue > 0)
                                                    {
                                                        workableData.Add(partsParameter);
                                                        BoxCapacityDict[boxIndex]--;
                                                    }
                                                }
                                            }
                                        }
                                        var group = workableData.GroupBy(x => x.BoxIndex);

                                        if (workableData.Count > 0)
                                        {
                                            var indexMax = SelectedProjectDistributeVM.StampingBoxPartsVM.
                                            SeparateBoxVMObservableCollection.SelectMany(x => x.BoxPartsParameterVMCollection).Count()==0
                                            ? -1 :
                                            SelectedProjectDistributeVM.StampingBoxPartsVM.SeparateBoxVMObservableCollection.
                                            SelectMany(x=>x.BoxPartsParameterVMCollection).Max(x => x.WorkIndex);


                                            foreach (var partsParameter in workableData)
                                            //for (int i=0;i< workableData.Count;i++)
                                            {
                                                indexMax++;
                                                //var partsParameter = workableData[i];
                                                partsParameter.IsFinish = false;
                                                partsParameter.IsTransported = false;
                                                partsParameter.WorkIndex = indexMax;
                                            }
                                        }
                                        else if (partsParameterVMCollection
                                        .Select(x => x.BoxIndex)
                                        .Any(boxIndex => SelectedProjectDistributeVM.StampingBoxPartsVM.SeparateBoxVMObservableCollection
                                        .Any(box => box.BoxIndex == boxIndex && !box.BoxIsEnabled)))
                                        {
                                            //目標箱子被關閉
                                            await MessageBoxResultShow.ShowOKAsync(null, "", (string)Application.Current.TryFindResource("Text_BoxIsNotEnableFailAddToMachiningProcess"), GD_MessageBoxNotifyResult.NotifyYe , false);
                                        }
                                        else
                                        {
                                            //沒有可加工的東西
                                            //沒有空位

                                            await MessageBoxResultShow.ShowOKAsync(null, "", (string)Application.Current.TryFindResource("Text_MachiningProcessIsOccupyConfirm"), GD_MessageBoxNotifyResult.NotifyYe, false);
                                        }
                                    }
                                    else
                                    {
                                        await MessageBoxResultShow.ShowOKAsync(null, "", (string)Application.Current.TryFindResource("Text_UnselectAnyPlateConfirm"), GD_MessageBoxNotifyResult.NotifyBl);
                                        //Debugger.Break();
                                    }
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine(ex);
                            Debugger.Break();
                        }
                    });
                });

                //OnPropertyChanged(nameof(CustomColumnSortByWorkIndex));
                RefreshBoxPartsParameterVMRowFilter();
                if(SelectedProjectDistributeVM!=null)
                    await SelectedProjectDistributeVM.SaveProductProjectVMCollectionAsync();




            }, e=> !ArrangeWorkCommand.IsRunning);
        }





        private AsyncRelayCommand<object>? _cancelArrangeWorkCommand;
        /// <summary>
        /// 解除加工
        /// </summary>
        public AsyncRelayCommand<object> CancelArrangeWorkCommand
        {
            get => _cancelArrangeWorkCommand ??= new AsyncRelayCommand<object>(async e =>
            {
                await Task.Run(async () =>
                 {
                     var partsParameterVMCollection = new List<PartsParameterViewModel>();

                     if (e is IList<PartsParameterViewModel> Itemsources)
                     {
                         partsParameterVMCollection = Itemsources.ToList();
                     }
                     else if (e is IEnumerable enumerableSources)
                     {
                         foreach (var enumSource in enumerableSources)
                         {
                             if (enumSource is PartsParameterViewModel partsParameterVM)
                             {
                                 partsParameterVMCollection.Add(partsParameterVM);
                             }
                         }
                     }

                     if (partsParameterVMCollection != null)
                     {
                         var cancelableData = partsParameterVMCollection.FindAll(x => x.WorkIndex >= 0 && !x.IsFinish && !x.IsSended);

                         bool dataIsChanged = false;
                         bool selectedData = false;
                         foreach (var item in cancelableData)
                         {
                             selectedData = true;
                             if (item is PartsParameterViewModel partsParameterVM)
                             {
                                 dataIsChanged = true;
                                 partsParameterVM.WorkIndex = -1;
                                 RefreshBoxPartsParameterVMRowFilter();
                             }
                         }

                         if (selectedData && !dataIsChanged)
                         {
                             //沒有資料被移動 -   資料已進入機台，不可解除
                             await MessageBoxResultShow.ShowOKAsync(null,(string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("MachiningDataIsAlreadySended"), 
                                 GD_MessageBoxNotifyResult.NotifyRd);
                         }
                     }
                     RefreshBoxPartsParameterVMRowFilter();

                     await SelectedProjectDistributeVM?.SaveProductProjectVMCollectionAsync();
                 });
            });
        }


        private ICommand?_separateBoxVMObservableCollectionelectionChangedCommand;
        [JsonIgnore]
        public ICommand SeparateBoxVMObservableCollectionelectionChangedCommand
        {
            get => _separateBoxVMObservableCollectionelectionChangedCommand ??= new RelayCommand<object>(obj =>
            {
                RefreshBoxPartsParameterVMRowFilter();
            });
        }

        public void RefreshBoxPartsParameterVMRowFilter()
        {
            Application.Current.Dispatcher.Invoke(() => 
            {
                OnPropertyChanged(nameof(BoxPartsParameterVMRowFilterCommand));
            }
            ); ;
        }

        //private DevExpress.Mvvm.DelegateCommand<RowFilterArgs> _boxPartsParameterVMRowFilterCommand;
        [JsonIgnore]
        public DevExpress.Mvvm.DelegateCommand<RowFilterArgs> BoxPartsParameterVMRowFilterCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<RowFilterArgs>(args =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.PartsParameterViewModel PParameter)
                {
                    try
                    {
                        if (SelectedProjectDistributeVM?.StampingBoxPartsVM?.SelectedSeparateBoxVM != null)
                        {
                            if (PParameter.DistributeName == SelectedProjectDistributeVM.ProjectDistributeName &&
                            PParameter.BoxIndex == SelectedProjectDistributeVM.StampingBoxPartsVM.SelectedSeparateBoxVM.BoxIndex &&
                            PParameter.WorkIndex >= 0 &&(!PParameter.IsTransported || ShowIsTransported))
                            {
                                args.Visible = true  ;
                            }
                            else
                            {
                                args.Visible = false;
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            });
        }

        private ICommand?_clearFinishItemCommand;
        public ICommand ClearFinishItemCommand
        {
            get => _clearFinishItemCommand ??= new AsyncRelayCommand(async () =>
            {
                try
                {
                    
                    if (SelectedProjectDistributeVM!=null && SelectedProjectDistributeVM?.StampingBoxPartsVM.SelectedSeparateBoxVM != null)
                    {
                        var selectedBoxIndex = SelectedProjectDistributeVM.StampingBoxPartsVM.SelectedSeparateBoxVM.BoxIndex;
                        string Outputstring = "";
                        var clearBoxConfirmNotify = (string)Application.Current.TryFindResource("Text_ClearBoxConfirm");
                        if (clearBoxConfirmNotify != null)
                        {
                            Outputstring = string.Format(clearBoxConfirmNotify, selectedBoxIndex);
                        }
                        var result = await MessageBoxResultShow.ShowYesNoAsync(null, null, Outputstring, GD_MessageBoxNotifyResult.NotifyBl);
                        if (result is MessageBoxResult.Yes)
                        {
                            ShowIsTransported = false;

                            var unTransportedCollection = this.SelectedProjectDistributeVM?.StampingBoxPartsVM.SeparateBoxVMObservableCollection.ScheduledPartsParameterCollection.Where(x => x.BoxIndex == selectedBoxIndex && x.IsFinish && !x.IsTransported);
                            if (unTransportedCollection != null)
                                foreach (var unTransPorted in unTransportedCollection)
                                {
                                    unTransPorted.IsTransported = true;
                                }


                            RefreshBoxPartsParameterVMRowFilter();
                            _ = SelectedProjectDistributeVM.SaveProductProjectVMCollectionAsync();
                        }
                    }
                    else
                    {

                    }
                }
                catch
                {

                }
                RefreshBoxPartsParameterVMRowFilter();

            });
        }





    }
}
