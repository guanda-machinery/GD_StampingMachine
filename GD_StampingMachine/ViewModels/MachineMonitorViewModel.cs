using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Extensions;
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
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static GD_StampingMachine.Singletons.StampMachineDataSingleton.StampingOpcUANode;

namespace GD_StampingMachine.ViewModels
{
    /*public class MachineMonitorModel
    {
        public ProjectDistributeViewModel ProjectDistributeVM { get; set; }
    }*/

    public class MachineMonitorViewModel : GD_CommonLibrary.BaseViewModel
    {

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("btnDescription_MachineMonitoring");
        public MachineMonitorViewModel()
        {
            StampingMachineSingleton.Instance.SelectedProjectDistributeVMChanged += Instance_SelectedProjectDistributeVMChanged;

        }

        private void Instance_SelectedProjectDistributeVMChanged(object sender, GD_CommonLibrary.ValueChangedEventArgs<ProjectDistributeViewModel> e)
        {
            SelectedProjectDistributeVM = e.NewValue;
        }

        readonly StampMachineDataSingleton StampMachineData = GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance;


        /// <summary>
        /// 鋼帶捲集合
        /// </summary>
        //private ObservableCollection<StampingSteelBeltViewModel> _stampingSteelBeltVMObservableCollection = new();
        //public ObservableCollection<StampingSteelBeltViewModel> StampingSteelBeltVMObservableCollection { get => _stampingSteelBeltVMObservableCollection; set { _stampingSteelBeltVMObservableCollection = value; OnPropertyChanged(); } }


        public double StampWidth { get; set; } = 50;


        public ObservableCollection<PartsParameterViewModel> BoxPartsParameterVMObservableCollection => StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection;

        private ProjectDistributeViewModel _selectedProjectDistributeVM;
        public ProjectDistributeViewModel SelectedProjectDistributeVM 
        {
            get => _selectedProjectDistributeVM; set { _selectedProjectDistributeVM = value;OnPropertyChanged(); }
        } 






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



        /*public bool ShowIsSended
        {
            get => _showIsSended;
            set
            {
                _showIsSended = value; OnPropertyChanged(); OnPropertyChanged(nameof(ArrangeWorkRowFilterCommand));
            }
        }*/



        /*public bool ShowDataMatrixIsFinish
        {
            get => _showDataMatrixIsFinish;
            set
            {
                _showDataMatrixIsFinish = value; OnPropertyChanged(); OnPropertyChanged(nameof(ArrangeWorkRowFilterCommand));
            }
        }*/
      /*  public bool ShowEngravingIsFinish
        {
            get => _showEngravingIsFinish;
            set
            {
                _showEngravingIsFinish = value; OnPropertyChanged(); OnPropertyChanged(nameof(ArrangeWorkRowFilterCommand));
            }
        }*/
        /*public bool ShowShearingIsFinish
        {
            get => _showShearingIsFinish;
            set
            {
                _showShearingIsFinish = value; OnPropertyChanged(); OnPropertyChanged(nameof(ArrangeWorkRowFilterCommand));
            }
        }*/
        /*public bool ShowIsFinish
        {
            get => _showIsFinish;
            set
            {
                _showIsFinish = value; OnPropertyChanged(); OnPropertyChanged(nameof(ArrangeWorkRowFilterCommand));
            }
        }*/



        private AsyncRelayCommand _sendMachiningCommand;
        public AsyncRelayCommand SendMachiningCommand
        {
            get => _sendMachiningCommand ??= new(async (CancellationToken token) =>
            {
                /*var ManagerVM = new DXSplashScreenViewModel
                {
                    Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Image/svg/NewLogo_1-2.svg"),
                    Title = "GD_StampingMachine",
                    Status = (string)System.Windows.Application.Current.TryFindResource("Connection_MachiningProcessStart"),
                    Progress = 0,
                    IsIndeterminate = true,
                    Subtitle = "",
                    Copyright = "Copyright © 2023 GUANDA",
                };
                SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.ProcessingScreenWindow(), ManagerVM);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    manager.Show(Application.Current.MainWindow, WindowStartupLocation.CenterScreen, true, InputBlockMode.None);
                });
                */
                //CancellationTokenSource cts = new();

                try
                {
                    //當剪切壓下時 將第一根設定為完成

                    //當第一片的id變化時 將上一片設定為完成
                    // _ = MonitorFirstIronPlateAsync(() => StampMachineData.FirstIronPlateID, cts);
                    // _ = MonitorLastIronPlateAsync(() => StampMachineData.LastIronPlateID, cts);

                    StampMachineData.FirstIronPlateIDChanged += StampMachineData_FirstIronPlateIDChanged;
                    StampMachineData.LastIronPlateIDChanged += StampMachineData_LastIronPlateIDChanged;

                    StampMachineData.Cylinder_HydraulicEngraving_IsStopDownChanged += StampMachineData_Cylinder_HydraulicEngraving_IsStopDownChanged;
                    StampMachineData.Cylinder_HydraulicCutting_IsCutPointChanged += StampMachineData_Cylinder_HydraulicCutting_IsCutPointChanged; 
                    //開始計算剪切數量
                    //鋼印下壓
                    //_ = MonitorStampingFontAsync(() => StampMachineData.Cylinder_HydraulicEngraving_IsStopDown, cts);

                    //開始依序傳送資料
                    await Task.Run(async () =>
                     {
                         try
                         {
                             while (true)
                             {
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
                                             ConnectManager.Show(Application.Current.MainWindow, WindowStartupLocation.CenterScreen, true, InputBlockMode.None);
                                         });
                                         //等待連線

                                        // if (!Debugger.IsAttached)
                                          //   await WaitForCondition.WaitIsTrueAsync(() => StampMachineData.IsConnected, token);

                                     }
                                     catch
                                     {
                                         ConnectManager.Close();
                                         break;
                                     }

                                     ConnectManager.Close();
                                 }
                                 //燈號
                                 //StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection
                                 //取得歷史資料
                                 //var history = await StampMachineData.GetIronPlateDataCollection();
                                
                                 var workableMachiningCollection = SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.ToList().FindAll(x => x.WorkIndex >= 0);
                                 //var workableMachiningCollection = StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.ToList().FindAll(x => x.WorkIndex >= 0);
                                 var a = workableMachiningCollection.Count(x => x.IsFinish);

                                 //準備加工(未上傳)
                                 var readyMachiningCollection = workableMachiningCollection.FindAll(x => !x.IsSended).OrderBy(x => x.WorkIndex).ToList();
                                 //已上傳
                                 var sendedReadyMachiningCollection = workableMachiningCollection.FindAll(x => x.IsSended).OrderBy(x => x.WorkIndex).ToList();

                                 if (readyMachiningCollection.Count == 0)
                                 {
                                     //manager?.Close();
                                     /*  await MessageBoxResultShow.ShowOKAsync(
                                           (string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("NoneMachiningData"));*/
                                     var result = await MessageBoxResultShow.ShowAsync(string.Empty, (string)Application.Current.TryFindResource("NoneMachiningData"), MessageBoxButton.OK, GD_MessageBoxNotifyResult.NotifyRd);
                                     _ = Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("NoneMachiningData"));
                                     break;
                                 }
                                 var readymachining = readyMachiningCollection.First();
                                 if (readymachining == null)
                                 {//沒有可加工的資料
                                     break;
                                 }
                                 var progress = ((double)sendedReadyMachiningCollection.Count * 100) / (double)workableMachiningCollection.Count;
                                 SendMachiningProgress = progress;


                                 if (token.IsCancellationRequested)
                                     token.ThrowIfCancellationRequested();
                                 _ = Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("Connection_WaitRequsetSignal"));



                                 string plateFirstValue = "";
                                 string plateSecondValue = "";
                                 readymachining.SettingBaseVM.PlateNumberList1.ForEach(p =>
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

                                 readymachining.SettingBaseVM.PlateNumberList2.ForEach(p =>
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
                                 List<PartsParameterViewModel> allBoxPartsParameterViewModel = new();
                                 //所有排版專案
                                 foreach (var productProject in StampingMachineSingleton.Instance.ProductSettingVM.ProductProjectVMObservableCollection)
                                 {
                                     allBoxPartsParameterViewModel.AddRange(productProject.PartsParameterVMObservableCollection);
                                 }

                                 int autonum = 0;
                                 do
                                 {
                                     Guid myGuid = Guid.NewGuid();
                                     byte[] bArr = myGuid.ToByteArray();
                                     autonum = Math.Abs(BitConverter.ToInt32(bArr, 0));
                                     //檢查是否有存在該編號
                                 }
                                 while (allBoxPartsParameterViewModel.Exists(x => x.ID == autonum));

                                 var boxIndex = readymachining.BoxIndex != null ? readymachining.BoxIndex.Value : 0;

                                 //readymachining.SettingBaseVM.IronPlateMarginVM.A_Margin
                                 var _HMIIronPlateData = new IronPlateDataModel
                                 {
                                     bEngravingFinish = false,
                                     bDataMatrixFinish = false,
                                     //流水編號
                                     iIronPlateID = autonum,
                                     iStackingID = boxIndex,
                                     rXAxisPos1 = readymachining.SettingBaseVM.StampingMarginPosVM.rXAxisPos1, //     10
                                     rXAxisPos2 = readymachining.SettingBaseVM.StampingMarginPosVM.rXAxisPos2,//25
                                     rYAxisPos1 = readymachining.SettingBaseVM.StampingMarginPosVM.rYAxisPos1 + MachineConst.StampingMachineYPosition, //119 = 14+105 //
                                     rYAxisPos2 = readymachining.SettingBaseVM.StampingMarginPosVM.rYAxisPos1 + MachineConst.StampingMachineYPosition, //*14+105 // 
                                     sDataMatrixName1 = string.IsNullOrWhiteSpace(readymachining.ParameterC) ? string.Empty : readymachining.ParameterC,
                                     sDataMatrixName2 = string.IsNullOrWhiteSpace(readymachining.QR_Special_Text) ? string.Empty : readymachining.QR_Special_Text,
                                     sIronPlateName1 = string.IsNullOrWhiteSpace(plateFirstValue) ? string.Empty : plateFirstValue,
                                     sIronPlateName2 = string.IsNullOrWhiteSpace(plateSecondValue) ? string.Empty : plateSecondValue
                                 };

                                 try
                                 {
                                     _ = Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("Connection_WaitRequsetSignal"));
                                     await WaitForCondition.WaitIsTrueAsync(() => StampMachineData.Rdatabit, 5000, token);
                                 }
                                 catch
                                 {
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
                                                 await Task.Delay(2000);
                                                 await Task.Yield();
                                             }
                                         }
                                         while (true);
                                         await Task.Delay(50);
                                         /*ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_WritingMachiningDataSucessful");
                                         Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ManagerVM.Status);*/
                                         await Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("Connection_WritingMachiningDataSucessful"));

                                         await Task.Delay(100);
                                         readymachining.ID = autonum;
                                     }
                                     await Task.Yield();

                                 }
                                 while (!sendhmi);

                                 //等待最後一支變成id
                                 //   await WaitForCondition.WaitAsync(() => StampMachineData.LastIronPlateID.oldValue, readymachining.ID, token);
                                 //ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_MachiningIsProcessing");
                                 // await Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ManagerVM.Status);
                                 await Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("Connection_MachiningIsProcessing"));

                                 readymachining.IsSended = true;
                                 await Task.Yield();
                             }
                         }
                         catch
                         {

                         }
                     }, token);
                }
                catch (OperationCanceledException oex)
                {

                    await LogDataSingleton.Instance.AddLogDataAsync(ViewModelName, oex.Message);
                }
                catch (Exception ex)
                {
                    await LogDataSingleton.Instance.AddLogDataAsync(ViewModelName, ex.Message);
                }
                finally
                {
                    StampMachineData.FirstIronPlateIDChanged -= StampMachineData_FirstIronPlateIDChanged;
                    StampMachineData.LastIronPlateIDChanged -= StampMachineData_LastIronPlateIDChanged;
                    StampMachineData.Cylinder_HydraulicEngraving_IsStopDownChanged -= StampMachineData_Cylinder_HydraulicEngraving_IsStopDownChanged;
                    StampMachineData.Cylinder_HydraulicCutting_IsCutPointChanged -= StampMachineData_Cylinder_HydraulicCutting_IsCutPointChanged;

                    //cts.Cancel();
                    //取消第一格的訂閱
                    await Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("Connection_MachiningProcessEnd"));

                    //ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_MachiningProcessEnd");
                    //await Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ManagerVM.Status);
                    await Task.Delay(1000);
                    //manager?.Close();
                    SendMachiningProgress = 0;

                }
            }, () => !_sendMachiningCommand.IsRunning);
        }



        private void StampMachineData_Cylinder_HydraulicCutting_IsCutPointChanged(object sender, GD_CommonLibrary.ValueChangedEventArgs<bool> e)
        {

            try
            {
                if (e.NewValue)
                {
                    //取得id
                    var eRotateStation = StampMachineData.EngravingRotateStation;
                    _ = Task.Run(async () =>
                    {
                        //找出正在被敲的那片id
                        //StampMachineData.Cylinder_GuideRod_Fixed_IsUp
                        var FirstIronPlateTuple = await Singletons.StampMachineDataSingleton.Instance.GetFirstIronPlateIDAsync();
                        if (FirstIronPlateTuple.Item1)
                        {
                            foreach (var projectDistributeVM in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                            {
                                var FirstIronID = FirstIronPlateTuple.Item2;
                                var finishPartParameter = projectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.FirstOrDefault(x => x.ID == FirstIronID);
                                if (finishPartParameter != null)
                                {
                                    finishPartParameter.ShearingIsFinish = true;
                                    //  finishPartParameter.IsFinish = true;
                                    try              
                                    {
                                        await projectDistributeVM.SaveProductProjectVMObservableCollectionAsync();
                                    }
                                    catch
                                    {

                                    }
                                    break;
                                }
                            }
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


        private AsyncRelayCommand _completeMachiningDataCommand;
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

                CancellationTokenSource cts = new();

                try
                {
                    //_ = MonitorFirstIronPlateAsync(() => StampMachineData.FirstIronPlateID, cts);
                    // _ = MonitorLastIronPlateAsync(() => StampMachineData.LastIronPlateID, cts);

                    StampMachineData.FirstIronPlateIDChanged += StampMachineData_FirstIronPlateIDChanged;
                    StampMachineData.LastIronPlateIDChanged += StampMachineData_LastIronPlateIDChanged;
                    StampMachineData.Cylinder_HydraulicEngraving_IsStopDownChanged += StampMachineData_Cylinder_HydraulicEngraving_IsStopDownChanged;
                    StampMachineData.Cylinder_HydraulicCutting_IsCutPointChanged += StampMachineData_Cylinder_HydraulicCutting_IsCutPointChanged;

                    // _ = MonitorStampingFontAsync(() => StampMachineData.Cylinder_HydraulicEngraving_IsStopDown, cts);

                    int? isNeedWorkListLengthInit = null;
                    while (true)
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

                        var (result , ironPlateDataCollection) = await StampMachineData.GetIronPlateDataCollectionAsync();
                        if (!result)
                        {
                            manager?.Close();
                            await MessageBoxResultShow.ShowOKAsync(
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
                            await MessageBoxResultShow.ShowOKAsync(
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
                            // var send = StampMachineData.AsyncSendMachiningData(readymachining.SettingBaseVM, token, int.MaxValue);

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
                                    await Task.Delay(100);
                                }
                                while (!setRequestDatabitSuccesfful);
                                ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_WritingMachiningDataSucessful");

                                await Task.Delay(1000);
                            }
                            await Task.Delay(100);
                        }
                        while (!sendhmi);
                    }
                }
                catch (OperationCanceledException)
                {

                }
                finally
                {

                    StampMachineData.FirstIronPlateIDChanged -= StampMachineData_FirstIronPlateIDChanged;
                    StampMachineData.LastIronPlateIDChanged -= StampMachineData_LastIronPlateIDChanged;
                    StampMachineData.Cylinder_HydraulicEngraving_IsStopDownChanged -= StampMachineData_Cylinder_HydraulicEngraving_IsStopDownChanged; 
                    StampMachineData.Cylinder_HydraulicCutting_IsCutPointChanged -= StampMachineData_Cylinder_HydraulicCutting_IsCutPointChanged;


                    cts.Cancel();
                    ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_MachiningProcessEnd");
                    await Task.Delay(1000);
                    manager?.Close();
                    CompleteMachiningProgress = 0;
                }
            });
        }

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

                    /* var getIdAsync = await StampMachineData.GetFirstIronPlateIDAsync();
                     if (getIdAsync.Item1)
                     {*/
                    _ = Task.Run(async () =>
                    {
                        foreach (var projectDistributeVM in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                        {
                            var finishPartParameter = projectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.FirstOrDefault(x => x.ID == ironPlateID().oldValue);
                            if (finishPartParameter != null)
                            {
                                finishPartParameter.FinishProgress = 100;
                                finishPartParameter.ShearingIsFinish = true;
                                finishPartParameter.IsFinish = true;
                                try
                                {
                                    await projectDistributeVM.SaveProductProjectVMObservableCollectionAsync();
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
        }
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
                            var finishPartParameter = projectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.FirstOrDefault(x => x.ID == ironPlateID().oldValue);
                            if (finishPartParameter != null)
                            {
                                finishPartParameter.FinishProgress = 33;
                                finishPartParameter.DataMatrixIsFinish = true;
                                try
                                {
                                    await projectDistributeVM.SaveProductProjectVMObservableCollectionAsync();
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
        }
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
                                var finishPartParameter = projectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.FirstOrDefault(x => x.ID == EngravingID);
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
                                        await projectDistributeVM.SaveProductProjectVMObservableCollectionAsync();
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



        private void StampMachineData_Cylinder_HydraulicEngraving_IsStopDownChanged(object sender, GD_CommonLibrary.ValueChangedEventArgs<bool> e)
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
                        //紀錄敲下去那一下是哪一顆字 並記錄次數
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
                                var finishPartParameter = projectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.FirstOrDefault(x => x.ID == EngravingID);
                                if (finishPartParameter != null)
                                {
                                    if (finishPartParameter.FinishProgress < 66)
                                    {
                                        var numberLength = finishPartParameter.SettingBaseVM.PlateNumber.Replace(" ", string.Empty).Length;
                                        //敲一次就跳一次完成度 不會超過66
                                        finishPartParameter.FinishProgress += ((float)33.3 / numberLength);
                                    }

                                   // finishPartParameter.ShearingIsFinish = true;
                                  //  finishPartParameter.IsFinish = true;
                                    try
                                    {
                                        await projectDistributeVM.SaveProductProjectVMObservableCollectionAsync();
                                    }
                                    catch
                                    {

                                    }
                                    break;
                                }
                            }
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
        private void StampMachineData_FirstIronPlateIDChanged(object sender, GD_CommonLibrary.ValueChangedEventArgs<int> e)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    foreach (var projectDistributeVM in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                    {
                        var finishPartParameter = projectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.FirstOrDefault(x => x.ID == e.OldValue);
                        if (finishPartParameter != null)
                        {
                            finishPartParameter.FinishProgress = 100;
                            finishPartParameter.ShearingIsFinish = true;
                            finishPartParameter.IsFinish = true;
                            try
                            {
                                await projectDistributeVM.SaveProductProjectVMObservableCollectionAsync();
                            }
                            catch
                            {

                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Debugger.Break();
                }
            });
        }

        /// <summary>
        /// 敲QR的id變化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StampMachineData_LastIronPlateIDChanged(object sender, GD_CommonLibrary.ValueChangedEventArgs<int> e)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    foreach (var projectDistributeVM in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                    {
                        var finishPartParameter = projectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.FirstOrDefault(x => x.ID == e.OldValue);
                        if (finishPartParameter != null)
                        {
                            finishPartParameter.FinishProgress = 33;
                            finishPartParameter.DataMatrixIsFinish = true;
                            try
                            {
                                await projectDistributeVM.SaveProductProjectVMObservableCollectionAsync();
                            }
                            catch
                            {

                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Debugger.Break();
                }
            });
        }


        public AsyncRelayCommand _sendMachiningCancelCommand;
        public AsyncRelayCommand SendMachiningCancelCommand
        {
            get => _sendMachiningCancelCommand ??= new (async () =>
            {
                SendMachiningCommand.Cancel();
                CompleteMachiningDataCommand.Cancel();
                await WaitForCondition.WaitAsyncIsFalse(() => SendMachiningCommand.IsRunning);
                await WaitForCondition.WaitAsyncIsFalse(() => CompleteMachiningDataCommand.IsRunning);
            }, () => !SendMachiningCancelCommand.IsRunning);
        }

        private AsyncRelayCommand _clearMachiningDataCommand;
        /// <summary>
        /// 清除所有資料
        /// </summary>
        public AsyncRelayCommand ClearMachiningDataCommand
        {
            get => _clearMachiningDataCommand ??= new(async (CancellationToken token) =>
            {
                try
                {
                    var result = MessageBoxResultShow.ShowYesNoAsync((string)Application.Current.TryFindResource("Text_notify"),
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
                        await MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
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




        //篩選器
        [JsonIgnore]
        public DevExpress.Mvvm.ICommand<RowFilterArgs> NotArrangeWorkRowFilterCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<RowFilterArgs>(args =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.PartsParameterViewModel PartsParameter)
                {
                    if (PartsParameter.WorkIndex >= 0)
                    {
                        args.Visible = false;
                    }
                    else if (PartsParameter.IsFinish)
                    {
                        args.Visible = false;
                    }
                    else
                    {
                        args.Visible = true;
                    }
                }
            });
        }


        //篩選器
        [JsonIgnore]
        public DevExpress.Mvvm.ICommand<RowFilterArgs> ArrangeWorkRowFilterCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<RowFilterArgs>(args =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.PartsParameterViewModel PartsParameter)
                {
                    /*  if (PartsParameter.WorkIndex >= 0 && ShowIsScheduled)
                      {
                          args.Visible = true;
                      }*/

                    if (PartsParameter.IsTransported)
                    {
                        args.Visible = false;
                    }
                   /* if (!PartsParameter.IsSended)
                    {
                        args.Visible = true;
                    }*/
                    /*else if(PartsParameter.IsSended && !ShowIsSended)
                    {
                        args.Visible = false;
                    }
                    else if (PartsParameter.DataMatrixIsFinish && !ShowDataMatrixIsFinish)
                    {
                        args.Visible = false;
                    }
                    else if (PartsParameter.EngravingIsFinish && !ShowEngravingIsFinish)
                    {
                        args.Visible = false;
                    }
                    else if (PartsParameter.ShearingIsFinish && !ShowShearingIsFinish)
                    {
                        args.Visible = false;
                    }
                    else if (PartsParameter.IsFinish && !ShowIsFinish)
                    {
                        args.Visible = false;
                    }*/
                    else if (PartsParameter.WorkIndex >= 0)
                    {
                        args.Visible = true;
                    }
                    else
                    {
                        args.Visible = false;
                    }

                }
            });
        }






        private AsyncRelayCommand<object> _arrangeWorkCommand;
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
                    else if(e is IEnumerable enumerableSources)
                    {
                       foreach(var enumSource in enumerableSources)
                        {
                            if(enumSource is PartsParameterViewModel partsParameterVM)
                            {
                                partsParameterVMCollection.Add(partsParameterVM);
                            }
                        }
                    }

                    if (partsParameterVMCollection != null)
                    {
                        if (partsParameterVMCollection.Count > 0)
                        {
                            var workableData = new List<PartsParameterViewModel>();

                            var BoxCapacityDict = SelectedProjectDistributeVM.StampingBoxPartsVM
                            .SeparateBoxVMObservableCollection
                            .Where(box => box.BoxIsEnabled)
                            .ToDictionary(box => box.BoxIndex, box => box.BoxSliderValue - box.UnTransportedBoxPieceValue);

                            var wData = partsParameterVMCollection.FindAll(x => x.WorkIndex < 0 && !x.IsFinish && !x.IsSended);
                            foreach (var partsParameter in wData)
                            {
                                var unTransportedCount = BoxPartsParameterVMObservableCollection
                                    .Count(x => x.BoxIndex == partsParameter.BoxIndex && x.WorkIndex >= 0 && !x.IsTransported);
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
                                foreach (var partsParameter in workableData)
                                {
                                    var indexMax = BoxPartsParameterVMObservableCollection.Max(x => x.WorkIndex);
                                    partsParameter.IsFinish = false;
                                    partsParameter.IsTransported = false;
                                    partsParameter.WorkIndex = indexMax < 0 ? 0 : indexMax + 1;
                                }


                                OnPropertyChanged(nameof(NotArrangeWorkRowFilterCommand));
                                OnPropertyChanged(nameof(ArrangeWorkRowFilterCommand));
                            }
                            else
                            {
                                //沒有可加工的東西
                                //沒有空位
                                //Debugger.Break();

                                await MessageBoxResultShow.ShowOKAsync("", (string)Application.Current.TryFindResource("Text_MachiningProcessIsOccupyConfirm"), GD_MessageBoxNotifyResult.NotifyYe);
                            }
                        }
                        else
                        {
                            await MessageBoxResultShow.ShowOKAsync("", (string)Application.Current.TryFindResource("Text_UnselectAnyPlateConfirm"), GD_MessageBoxNotifyResult.NotifyBl);
                            //Debugger.Break();
                        }
                    }
                    


                });

                //OnPropertyChanged(nameof(CustomColumnSortByWorkIndex));
                RefreshBoxPartsParameterVMRowFilter();
                await SelectedProjectDistributeVM.SaveProductProjectVMObservableCollectionAsync();

            }, e=> !ArrangeWorkCommand.IsRunning);
        }





        private AsyncRelayCommand<object> _cancelArrangeWorkCommand;
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
                                 OnPropertyChanged(nameof(NotArrangeWorkRowFilterCommand));
                                 OnPropertyChanged(nameof(ArrangeWorkRowFilterCommand));
                             }
                         }

                         if (selectedData && !dataIsChanged)
                         {
                             //沒有資料被移動 -   資料已進入機台，不可解除
                             await MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("MachiningDataIsAlreadySended"), 
                                 GD_MessageBoxNotifyResult.NotifyRd);
                         }
                     }
                     OnPropertyChanged(nameof(NotArrangeWorkRowFilterCommand));
                     OnPropertyChanged(nameof(ArrangeWorkRowFilterCommand));
                     RefreshBoxPartsParameterVMRowFilter();
                 });
            });
        }


        private ICommand _separateBoxVMObservableCollectionelectionChangedCommand;
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
            OnPropertyChanged(nameof(BoxPartsParameterVMRowFilterCommand));
        }


        [JsonIgnore]
        public ICommand BoxPartsParameterVMRowFilterCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<RowFilterArgs>(args =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.PartsParameterViewModel PParameter)
                {
                    if (SelectedProjectDistributeVM.StampingBoxPartsVM.SelectedSeparateBoxVM != null)
                    {
                        if (PParameter.DistributeName == SelectedProjectDistributeVM.ProjectDistributeName && 
                        PParameter.BoxIndex == SelectedProjectDistributeVM.StampingBoxPartsVM.SelectedSeparateBoxVM.BoxIndex &&
                        PParameter.WorkIndex>=0&& !PParameter.IsTransported)
                            args.Visible = true;
                        else
                            args.Visible = false;
                    }
                }
            });
        }

        private ICommand _clearFinishItemCommand;
        public ICommand ClearFinishItemCommand
        {
            get => _clearFinishItemCommand ??= new AsyncRelayCommand(async () =>
            {
                var selectedBoxIndex = SelectedProjectDistributeVM.StampingBoxPartsVM.SelectedSeparateBoxVM.BoxIndex;
                string Outputstring = "";
                var clearBoxConfirmNotify = (string)Application.Current.TryFindResource("Text_ClearBoxConfirm");
                if (clearBoxConfirmNotify != null)
                {
                     Outputstring = string.Format(clearBoxConfirmNotify, selectedBoxIndex);
                }
                var result =  await MessageBoxResultShow.ShowOKCancelAsync(null , Outputstring, GD_MessageBoxNotifyResult.NotifyBl);
                if (result is MessageBoxResult.OK || result is MessageBoxResult.Yes)
                {
                    var unTransportedCollection = this.BoxPartsParameterVMObservableCollection.ToList().FindAll(x => x.BoxIndex == selectedBoxIndex && x.IsFinish && !x.IsTransported);
                    foreach (var unTransPorted in unTransportedCollection)
                    {
                        unTransPorted.IsTransported = true;
                    }
                    RefreshBoxPartsParameterVMRowFilter();
                }
                OnPropertyChanged(nameof(ArrangeWorkRowFilterCommand));
                OnPropertyChanged(nameof(NotArrangeWorkRowFilterCommand));

            });
        }





    }
}
