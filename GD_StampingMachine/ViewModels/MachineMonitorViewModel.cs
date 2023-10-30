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


       private AsyncRelayCommand _sortWorkMachineCommand;
        /// <summary>
        /// 將工作按照順序預排
        /// </summary>
        public AsyncRelayCommand SortWorkMachineCommand
        {
            get => _sortWorkMachineCommand ??= new AsyncRelayCommand(async (CancellationToken token) =>
            {
                try
                {/*
                    await Task.Run(() =>
                    {
                        var StampingPlateProcessingSequenceViewModelList = new List<StampingPlateProcessingSequenceViewModel>();
                        //先排序 並將已經加工完成的物件去掉
                        var smcCollection = BoxPartsParameterVMObservableCollection.OrderBy(x => x.WorkIndex).ToList().FindAll(
                            x => !x.IsFinish && x.WorkIndex >= 0 &&
                            (x.WorkScheduler_QRStamping || x.WorkScheduler_FontStamping || x.WorkScheduler_Shearing));

                        //將加工物進行排序

                        if (smcCollection.Count > 0)
                        {
                            smcCollection[0].SendMachineCommandVM.AbsoluteMoveDistance = 50;

                            for (int i = 1; i < smcCollection.Count; i++)
                            {
                                smcCollection[i].SendMachineCommandVM.AbsoluteMoveDistance = smcCollection[0].SendMachineCommandVM.AbsoluteMoveDistance + StampWidth * i;
                                smcCollection[i].SendMachineCommandVM.RelativeMoveDistance = smcCollection[0].SendMachineCommandVM.RelativeMoveDistance + StampWidth * i;

                            }

                            StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StripSteelPosition = 0;
                            StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StripSteelLength = smcCollection.MaxOrDefault(x => x.SendMachineCommandVM.RelativeMoveDistance) + StampWidth + 100;


                        }
                    });*/
                }
                catch (Exception ex)
                { 
                }
            }, () => !_sortWorkMachineCommand.IsRunning);
        }




        private AsyncRelayCommand _sendMachiningCommand;
        public AsyncRelayCommand SendMachiningCommand
        {
            get => _sendMachiningCommand??= new (async (CancellationToken token) =>
            {
                await Task.Run(async() =>
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
                    try
                    {
                        //開始依序傳送資料
                        while (true)
                        {
                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();

                            //StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection
                            //取得歷史資料
                            //var history = await StampMachineData.GetIronPlateDataCollection();
                            var workableMachiningCollection = StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.ToList().FindAll(x => x.WorkIndex >= 0);
                            var readyMachiningCollection = workableMachiningCollection.FindAll(x => !x.IsSended).OrderBy(x => x.WorkIndex).ToList();
                            var sendedReadyMachiningCollection = workableMachiningCollection.FindAll(x => x.IsSended).OrderBy(x => x.WorkIndex).ToList();

                            //取得機台上第0筆(準備被推出去的那一筆)
                            //var GetIronPlate = await StampMachineData.GetIronPlateDataCollection();
                            /*int firstID = 0;
                            if (!GetIronPlate.Item1)
                            {
                                continue;
                            }
                            else
                            {
                                var firstPlate = GetIronPlate.Item2.First();
                                if (firstPlate != null)
                                    firstID = firstPlate.iIronPlateID;
                            }*/



                            if (readyMachiningCollection.Count == 0)
                            {
                                await MessageBoxResultShow.ShowOK(
                                    (string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("NoneMachiningData"));
                                break;
                            }
                            var readymachining = readyMachiningCollection.First();
                            if (readymachining == null)
                            {//沒有可加工的資料

                                break; 
                            }
                            var progress = ((double)sendedReadyMachiningCollection.Count*100) / (double)workableMachiningCollection.Count;
                            ManagerVM.Progress = progress;


                            if (manager.State == SplashScreenState.Closed)
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    manager.Show(Application.Current.MainWindow, WindowStartupLocation.CenterScreen, true, InputBlockMode.None);
                                }));
                            }
                            //等待機台訊號 

                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();

                            ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_WaitRequsetSignal");
                            ManagerVM.Subtitle = $"[{readymachining.WorkIndex}] [{readymachining.IronPlateString}]";
                            var Rdatabit = await StampMachineData.GetRequestDatabit();
                            if (!Rdatabit)
                            {
                                await Task.Delay(1000);
                                continue;
                            }

                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();

                            //將兩行字上傳到機器
                            //readymachining.
                            string ironPlateString = "";
                            readymachining.SettingBaseVM.PlateNumberList.ForEach(pNumber =>
                            {
                                if (string.IsNullOrWhiteSpace(pNumber.FontString))
                                {
                                    ironPlateString += " ";
                                }
                                else
                                    ironPlateString += pNumber.FontString;
                            });
                           // var SpiltPlate = readymachining.IronPlateString.SpiltByLength(readymachining.SettingBaseVM.SequenceCount);
                            var SpiltPlate = ironPlateString.SpiltByLength(readymachining.SettingBaseVM.SequenceCount);
                            //靠左 靠右等功能
                            SpiltPlate.TryGetValue(0, out string plateFirstValue);
                            SpiltPlate.TryGetValue(1, out string plateSecondValue);


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
                            foreach(var productProject in StampingMachineSingleton.Instance.ProductSettingVM.ProductProjectVMObservableCollection)
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
                            } while (allBoxPartsParameterViewModel.Exists(x => x.ID == autonum));

                            var boxIndex = readymachining.BoxIndex!=null ? readymachining.BoxIndex.Value : 0;
                            var _HMIIronPlateData = new IronPlateDataModel
                            {
                                bEngravingFinish = false,
                                bDataMatrixFinish = false,
                                //流水編號
                                iIronPlateID = autonum,
                                iStackingID = boxIndex, 
                                rXAxisPos1 = 10,
                                rXAxisPos2 =25 , 
                                rYAxisPos1 =119 , 
                                rYAxisPos2= 119,
                                sDataMatrixName1 = readymachining.QrCodeContent,
                                sDataMatrixName2 = readymachining.QR_Special_Text,
                                sIronPlateName1 = plateFirstValue,
                                sIronPlateName2 = plateSecondValue
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
                                        setRequestDatabitSuccesfful = await StampMachineData.SetRequestDatabit(false);
                                        await Task.Delay(100);
                                    }
                                    while (!setRequestDatabitSuccesfful);
                                    ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_WritingMachiningDataSucessful");

                                    await Task.Delay(1000);
                                    readymachining.IsSended = true;
                                    readymachining.ID = autonum;
                                    //下完命令後 找出第一格的id 並將其設定為完成
                                   

                                }
                                await Task.Delay(100);
                            }
                            while (!sendhmi);

                        }
                    }
                    catch (OperationCanceledException)
                    {

                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_MachiningProcessEnd");
                        await Task.Delay(1000);
                        manager?.Close();
                    }
                }, token);
            }, ()=>  !_sendMachiningCommand.IsRunning);
        }
        private AsyncRelayCommand _completeMachiningDataCommand;
        /// <summary>
        /// 將剩下的工作做完->一直傳送空字串直到沒有任何料為止
        /// </summary>
        public AsyncRelayCommand CompleteMachiningDataCommand
        {
            get => _clearMachiningDataCommand ??= new(async (CancellationToken token) =>
            {

            });
        }

        public ICommand SendMachiningCancelCommand => new RelayCommand(() =>
        {
            SendMachiningCommand.Cancel();
            CompleteMachiningDataCommand.Cancel();
        });






        private AsyncRelayCommand _clearMachiningDataCommand;
        public AsyncRelayCommand ClearMachiningDataCommand
        {
            get => _clearMachiningDataCommand ??= new(async (CancellationToken token) =>
            {
                await Task.Run(async () =>
                {
                    _clearMachiningDataCommand.Cancel();
                    try
                    {
                        var result = MessageBoxResultShow.ShowYesNo((string)Application.Current.TryFindResource("Text_notify"),
                        (string)Application.Current.TryFindResource("Text_AskClearAllMachiningData")) ;
                        //跳出彈窗

                        if(await result != MessageBoxResult.Yes)
                        {
                            return;
                        }

                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();

                        bool isGetIronPlate;
                        List<IronPlateDataModel> ironPlateDataCollection = new List<IronPlateDataModel>(); ;
                        do
                        {
                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();

                            var getIronPlate = await StampMachineData.GetIronPlateDataCollection();
                            isGetIronPlate = getIronPlate.Item1;
                            ironPlateDataCollection = getIronPlate.Item2;
                        } while (!isGetIronPlate);

                       var NewEmptyIronPlateDataCollection = Enumerable.Repeat(new IronPlateDataModel() , ironPlateDataCollection.Count).ToList();

                        bool isSetIronPlate = false;
                        do
                        {
                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();
                            isSetIronPlate = await StampMachineData.SetIronPlateDataCollection(NewEmptyIronPlateDataCollection);
                        } while (!isSetIronPlate);

                    }
                    catch (Exception ex)
                    {
                        //跳出
                    }
                }, token);

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

        /// <summary>
        /// 排定加工
        /// </summary>
        public AsyncRelayCommand<object> ArrangeWorkCommand
        {
            get => new AsyncRelayCommand<object>(async (e, token) =>
            {
                await Task.Run(() =>
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
                });
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




        /// <summary>
        /// 解除加工
        /// </summary>
        public AsyncRelayCommand<object> CancelArrangeWorkCommand
        {
            get => new AsyncRelayCommand<object>(async (e, token) =>
            {                //全選
                await Task.Run(() =>
                {
                    if(e is IEnumerable Itemsources)
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
                });
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
