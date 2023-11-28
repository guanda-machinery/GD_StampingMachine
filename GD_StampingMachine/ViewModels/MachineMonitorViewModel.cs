using CommunityToolkit.Mvvm.Input;
using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.Xpf;
using DevExpress.Office.Crypto;
using DevExpress.Pdf.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.ExpressionEditor;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.XtraEditors.Filtering;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.GD_Model.ProductionSetting;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels.ProductSetting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static DevExpress.Utils.Drawing.Helpers.NativeMethods;

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
            /*StampingSteelBeltVMObservableCollection = new();

            //由右到左排列 
            for (int i = 0; i < 3; i++)
            {
                StampingSteelBeltVMObservableCollection.Add(
                    new StampingSteelBeltViewModel(
                        new StampingSteelBeltModel()
                        {
                            BeltString = "Guanda",
                            BeltNumberString = "001",
                            MachiningStatus = SteelBeltStampingStatusEnum.Shearing
                        }));
            }
            for (int i = 0; i < 5; i++)
            {
                StampingSteelBeltVMObservableCollection.Add(
                    new StampingSteelBeltViewModel(
                        new StampingSteelBeltModel()
                        {
                            BeltString = "Guanda",
                            BeltNumberString = "001",
                            MachiningStatus = SteelBeltStampingStatusEnum.Stamping
                        }));
            }
            for (int i = 0; i < 5; i++)
            {
                StampingSteelBeltVMObservableCollection.Add(
                    new StampingSteelBeltViewModel(
                        new StampingSteelBeltModel()
                        {
                            BeltString = null,
                            BeltNumberString = null,
                            MachiningStatus = SteelBeltStampingStatusEnum.QRCarving
                        }));
            }
            for (int i = 0; i < 2; i++)
            {
                StampingSteelBeltVMObservableCollection.Add(
                    new StampingSteelBeltViewModel(
                        new StampingSteelBeltModel()
                        {
                            BeltString = null,
                            BeltNumberString = null,
                            MachiningStatus = SteelBeltStampingStatusEnum.None
                        }));
            }*/

        }

        readonly StampMachineDataSingleton StampMachineData = GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance;


        /// <summary>
        /// 鋼帶捲集合
        /// </summary>
        //private ObservableCollection<StampingSteelBeltViewModel> _stampingSteelBeltVMObservableCollection = new();
        //public ObservableCollection<StampingSteelBeltViewModel> StampingSteelBeltVMObservableCollection { get => _stampingSteelBeltVMObservableCollection; set { _stampingSteelBeltVMObservableCollection = value; OnPropertyChanged(); } }


        public double StampWidth { get; set; } = 50;


        private ObservableCollection<PartsParameterViewModel> BoxPartsParameterVMObservableCollection=> StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection;


        /*public ICommand SetStatusSendMachineCommand
        {
            get => new RelayCommand(() =>
            {
               var smc = BoxPartsParameterVMObservableCollection.OrderBy(x => x.SendMachineCommandVM.WorkIndex).
                ToList().FindAll(x=>!x.SendMachineCommandVM.IsFinish && x.SendMachineCommandVM.WorkIndex>=0);
                if (smc != null)
                {
                    for (int i = 0; i < smc.Count; i++)
                    {
                        var steelBeltStampingStatus = SteelBeltStampingStatusEnum.None;
                        if (i < 3)
                        {
                            steelBeltStampingStatus = SteelBeltStampingStatusEnum.Shearing;
                        }
                        else if (i < 9)
                        {
                            steelBeltStampingStatus = SteelBeltStampingStatusEnum.Stamping;
                        }
                        else if (i < 14)
                        {
                            steelBeltStampingStatus = SteelBeltStampingStatusEnum.QRCarving;
                        }
                        else
                        {
                            steelBeltStampingStatus = SteelBeltStampingStatusEnum.None;
                        }
                        smc[i].SendMachineCommandVM.WorkingSteelBeltStampingStatus = steelBeltStampingStatus;
                        smc[i].SendMachineCommandVM.SteelBeltStampingStatus = steelBeltStampingStatus;
                    }
                }



            });
        }*/




        private AsyncRelayCommand _sendMachiningCommand;
        public AsyncRelayCommand SendMachiningCommand
        {
            get => _sendMachiningCommand ??= new(async (CancellationToken token) =>
            {
                var ManagerVM = new DXSplashScreenViewModel
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

                CancellationTokenSource cts = new CancellationTokenSource();

                try
                {
            
                    //當剪切壓下時 將第一根設定為完成
                    _ = MonitorFirstIronPlateAsync(()=>StampMachineData.Cylinder_HydraulicCutting_IsCutPoint,cts);
                    _ = MonitorStampingFontAsync(() => StampMachineData.Cylinder_HydraulicEngraving_IsStopDown, cts);
                    //開始依序傳送資料
                    while (true)
                    {
                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();
                        
                        //先等待連線
                        await WaitForCondition.WaitIsTrueAsync(() => StampMachineData.IsConnected, token);
                        //燈號

                        //StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection
                        //取得歷史資料
                        //var history = await StampMachineData.GetIronPlateDataCollection();
                        var workableMachiningCollection = StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.ToList().FindAll(x => x.WorkIndex >= 0);
                  
                        //準備加工(未上傳)
                        var readyMachiningCollection = workableMachiningCollection.FindAll(x => !x.IsSended).OrderBy(x => x.WorkIndex).ToList();
                        //已上傳
                        var sendedReadyMachiningCollection = workableMachiningCollection.FindAll(x => x.IsSended).OrderBy(x => x.WorkIndex).ToList();

                        if (readyMachiningCollection.Count == 0)
                        {
                            manager?.Close();
                            await MessageBoxResultShow.ShowOKAsync(
                                (string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("NoneMachiningData"));
                            break;
                        }
                        var readymachining = readyMachiningCollection.First();
                        if (readymachining == null)
                        {//沒有可加工的資料
                            break;
                        }
                        var progress = ((double)sendedReadyMachiningCollection.Count * 100) / (double)workableMachiningCollection.Count;
                        ManagerVM.Progress = progress;

                        //等待機台訊號 

                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();

                        ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_WaitRequsetSignal");
                        ManagerVM.Subtitle = $"[{readymachining.WorkIndex}] [{readymachining.IronPlateString}]";

                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();

                        //將兩行字上傳到機器
                        //readymachining.
                        string ironPlateString = "";

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
                            rXAxisPos1 = 10,
                            rXAxisPos2 = 25,
                            rYAxisPos1 = 119,
                            rYAxisPos2 = 119,
                            sDataMatrixName1 = string.IsNullOrEmpty(readymachining.QrCodeContent) ? string.Empty :readymachining.QrCodeContent,
                            sDataMatrixName2 = string.IsNullOrEmpty(readymachining.QrCodeContent) ? string.Empty : readymachining.QR_Special_Text,
                            sIronPlateName1 = string.IsNullOrEmpty(plateFirstValue)? string.Empty :plateFirstValue,
                            sIronPlateName2 = string.IsNullOrEmpty(plateSecondValue)? string.Empty : plateSecondValue
                        };


                        try
                        {
                            await WaitForCondition.WaitIsTrueAsync(() => StampMachineData.Rdatabit, 1000, token);
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
                            // var send = StampMachineData.AsyncSendMachiningData(readymachining.SettingBaseVM, token, int.MaxValue);

                            ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_WritingMachiningData");
                            sendhmi = await StampMachineData.SetHMIIronPlateData(_HMIIronPlateData);
                            //hmi設定完之後還需要進行設定變更!
                            if (sendhmi)
                            {
                                do
                                {
                                    if (token.IsCancellationRequested)
                                        token.ThrowIfCancellationRequested();
                                    try
                                    {
                                        if (await StampMachineData.SetRequestDatabit(false))
                                        {
                                            await WaitForCondition.WaitAsyncIsFalse(() => StampMachineData.Rdatabit, token);
                                            break;
                                        }
                                    }
                                    catch
                                    {
                                        await Task.Delay(100);
                                    }
                                }
                                while (true);

                                ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_WritingMachiningDataSucessful");

                                await Task.Delay(1000);
                                readymachining.IsSended = true;
                                readymachining.ID = autonum;
                            }
                            await Task.Delay(100);
                        }
                        while (!sendhmi);

                    }
                }
                catch (OperationCanceledException oex)
                {

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    cts.Cancel();
                    //取消第一格的訂閱
                    ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_MachiningProcessEnd");
                    await Task.Delay(1000);
                    manager?.Close();
                }
            }, () => !_sendMachiningCommand.IsRunning);
        }









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
             
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    manager.Show(Application.Current.MainWindow, WindowStartupLocation.CenterScreen, true, InputBlockMode.None);
                });
                CancellationTokenSource cts = new();
                try
                {
                    _ = MonitorFirstIronPlateAsync(() => StampMachineData.Cylinder_HydraulicCutting_IsCutPoint, cts);
                    _ = MonitorStampingFontAsync(() => StampMachineData.Cylinder_HydraulicEngraving_IsStopDown, cts);

                    int? isNeedWorkListLengthInit = null;
                    while (true)
                    {
                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();

                        await WaitForCondition.WaitIsTrueAsync(() => StampMachineData.IsConnected, token);


                        var ironPlateDataRet = await StampMachineData.GetIronPlateDataCollection();
                        if(!ironPlateDataRet.Item1)
                        {
                            manager?.Close();
                            await MessageBoxResultShow.ShowOKAsync(
                                (string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("MachineOffline"));
                            break;
                        }

                        //檢查清單內若有待加工(id !=0 或 四種加工資料任一欄有值)
                        var isNeedWorkList = ironPlateDataRet.Item2.FindAll(x => x.iIronPlateID != 0 ||
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
                                ManagerVM.Progress = ((double)(isNeedWorkListLengthInit.Value - isNeedWorkList.Count)*100) / isNeedWorkListLengthInit.Value;
                            }
                        }
                        
                       



                        if (isNeedWorkList.Count ==0)
                        {
                            manager?.Close();
                            await MessageBoxResultShow.ShowOKAsync(
                                (string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("NoneMachiningData"));
                            break;
                        }

                        ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_WaitRequsetSignal");
                        ManagerVM.Subtitle = null ;

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
                            sendhmi = await StampMachineData.SetHMIIronPlateData(_HMIIronPlateData);
                            //hmi設定完之後還需要進行設定變更!
                            if (sendhmi)
                            {
                                bool setRequestDatabitSuccesfful = false;
                                do
                                {
                                    if(await StampMachineData.SetRequestDatabit(false))
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
                catch(OperationCanceledException)
                {

                }
                finally
                {
                    cts.Cancel();
                    ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_MachiningProcessEnd");
                    await Task.Delay(1000);
                    manager?.Close();
                }
            });
        }

        private async Task MonitorFirstIronPlateAsync(Func<bool> IsStopDown ,  CancellationTokenSource cts)
        {
            //等待值
            try
            {
                var token = cts.Token;
                int previousID = 0;
                while (!token.IsCancellationRequested)
                {
                    await WaitForCondition.WaitIsTrueAsync(IsStopDown, token);
                    //取得id
                    var getIdAsync = await StampMachineData.GetFirstIronPlateID();
                    if (getIdAsync.Item1)
                    {
                        if (previousID != getIdAsync.Item2)
                        {
                            foreach (var rojectDistributeVM in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                            {
                                var finishPartParameter = rojectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.FirstOrDefault(x => x.ID == previousID);
                                if (finishPartParameter != null)
                                {
                                    finishPartParameter.ShearingIsFinish = true;
                                    finishPartParameter.IsFinish = true;

                                    try
                                    {
                                        await rojectDistributeVM?.SaveProductProjectVMObservableCollectionAsync();
                                    }
                                    catch
                                    {

                                    }

                                    break;
                                }
                            }
                            previousID = getIdAsync.Item2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task MonitorStampingFontAsync(Func<bool> IsStopDown, CancellationTokenSource cts)
        {
            //等待值
            try
            {
                var token = cts.Token;
            //    int previousStation = 0;
                while (!token.IsCancellationRequested)
                {
                    await WaitForCondition.WaitIsTrueAsync(IsStopDown, token);
                    //取得id
                    var eRotateStation = StampMachineData.EngravingRotateStation;
                    if (Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection.TryGetValue(eRotateStation, out var stamptype))
                    {
                        stamptype.StampingTypeUseCount++;
                        await Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.SaveStampingTypeVMObservableCollectionAsync();
                    }
                }
            }
            catch (Exception ex)
            {

            }



        }





        public AsyncRelayCommand _sendMachiningCancelCommand;
        public AsyncRelayCommand SendMachiningCancelCommand
        {
            get => _sendMachiningCancelCommand ??= new AsyncRelayCommand(async () =>
            {
                SendMachiningCommand.Cancel();
                CompleteMachiningDataCommand.Cancel();


                await WaitForCondition.WaitAsyncIsFalse(()=>SendMachiningCommand.IsRunning);
                await WaitForCondition.WaitAsyncIsFalse(()=>CompleteMachiningDataCommand.IsRunning);



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
                    List<IronPlateDataModel> ironPlateDataCollection = new List<IronPlateDataModel>(); ;
                    if (StampMachineData.IsConnected)
                    {
                        do
                        {
                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();

                            var getIronPlate = await StampMachineData.GetIronPlateDataCollection();
                            isGetIronPlate = getIronPlate.Item1;
                            ironPlateDataCollection = getIronPlate.Item2;
                        } while (!isGetIronPlate);

                        var NewEmptyIronPlateDataCollection = Enumerable.Repeat(new IronPlateDataModel(), ironPlateDataCollection.Count).ToList();

                        bool isSetIronPlate = false;
                        do
                        {
                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();
                            isSetIronPlate = await StampMachineData.SetIronPlateDataCollection(NewEmptyIronPlateDataCollection);
                        } while (!isSetIronPlate);




                    }
                    else
                    {
                        await MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                            (string)Application.Current.TryFindResource("MachineOffline"));
                    }
                }
                catch (Exception ex)
                {
                    //跳出
                }
            }, () => !_clearMachiningDataCommand.IsRunning);
        }




        //篩選器
        [JsonIgnore]
        public DevExpress.Mvvm.ICommand<RowFilterArgs>NotArrangeWorkRowFilterCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<RowFilterArgs>(args =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.PartsParameterViewModel PartsParameter)
                {
                    if (PartsParameter.WorkIndex >= 0)
                    {
                        args.Visible = false;
                    }
                    else if(PartsParameter.IsFinish)
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
                    if (PartsParameter.WorkIndex >= 0)
                    {
                        args.Visible = true;
                    }
                    else if (PartsParameter.IsFinish)
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
            get => _arrangeWorkCommand??= new AsyncRelayCommand<object>(async (e, token) =>
            {
                if (e is IEnumerable Itemsources)
                {
                    foreach (var item in Itemsources)
                    {

                        if (item is PartsParameterViewModel partsParameter)
                        {
                            if (partsParameter.WorkIndex < 0)
                                SetPartsParameterWork(partsParameter);
                            partsParameter.MachiningStatus = MachiningStatusEnum.Ready;
                        }
                    }
                }
                OnPropertyChanged(nameof(NotArrangeWorkRowFilterCommand));
                OnPropertyChanged(nameof(ArrangeWorkRowFilterCommand));
                //OnPropertyChanged(nameof(CustomColumnSortByWorkIndex));

            }, e => !ArrangeWorkCommand.IsRunning);
        }

        private void SetPartsParameterWork(PartsParameterViewModel partsParameter)
        {
            //找清單中最大的index
            partsParameter.IsFinish = false;
            var indexMax = BoxPartsParameterVMObservableCollection.Max(x => x.WorkIndex);
            if (indexMax < 0)
            {
                partsParameter.WorkIndex = 0;
            }
            else
            {
                partsParameter.WorkIndex = indexMax + 1;
            }
        }




        private AsyncRelayCommand<object> _cancelArrangeWorkCommand;
        /// <summary>
        /// 解除加工
        /// </summary>
        public AsyncRelayCommand<object> CancelArrangeWorkCommand
        {
            get => _cancelArrangeWorkCommand ??= new AsyncRelayCommand<object>(async (e, token) =>
            {              
                //全選
                if (e is IEnumerable Itemsources)
                {
                    foreach (var item in Itemsources)
                    {
                        if (item is PartsParameterViewModel partsParameter)
                        {
                            if (!partsParameter.IsSended)
                            {
                                CancelPartsParameterWork(partsParameter);
                                partsParameter.MachiningStatus = MachiningStatusEnum.None;
                            }
                        }
                    }
                }
                OnPropertyChanged(nameof(NotArrangeWorkRowFilterCommand));
                OnPropertyChanged(nameof(ArrangeWorkRowFilterCommand));
            }, e => !CancelArrangeWorkCommand.IsRunning);
        }
        private void CancelPartsParameterWork(PartsParameterViewModel partsParameter)
        {
            partsParameter.IsFinish = false;
            partsParameter.WorkIndex = -1;
        }





        //分組排列
        // CustomColumnSortCommand="{Binding CustomColumnSortByWorkIndex}"
        /*public DevExpress.Mvvm.ICommand<RowSortArgs> CustomColumnSortByWorkIndex
        {
            get => new DevExpress.Mvvm.DelegateCommand<RowSortArgs>(args =>
            {
                if (args.FieldName == "Day")
                {
                    // int dayIndex1 = GetDayIndex(args.FirstValue);
                    // int dayIndex2 = GetDayIndex(args.SecondValue);
                    //  args.Result = dayIndex1.CompareTo(dayIndex2);
                }
            });
        }*/





    }
}
