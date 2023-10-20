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
                {
                    await Task.Run(() =>
                    {
                        var StampingPlateProcessingSequenceViewModelList = new List<StampingPlateProcessingSequenceViewModel>();
                        //先排序 並將已經加工完成的物件去掉
                        var smcCollection = BoxPartsParameterVMObservableCollection.OrderBy(x => x.SendMachineCommandVM.WorkIndex).ToList().FindAll(
                            x => !x.SendMachineCommandVM.IsFinish && x.SendMachineCommandVM.WorkIndex >= 0 &&
                            (x.SendMachineCommandVM.WorkScheduler_QRStamping || x.SendMachineCommandVM.WorkScheduler_FontStamping || x.SendMachineCommandVM.WorkScheduler_Shearing));

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
                    });
                }
                catch (Exception ex)
                { 
                }
            }, () => !_sortWorkMachineCommand.IsRunning);
        }

        //private AsyncRelayCommand _workMachiningCommand;
        /// <summary>
        /// 加工命令
        /// </summary>


        /*
        public AsyncRelayCommand WorkMachiningCommand
        {
            get => _workMachiningCommand ??= new AsyncRelayCommand(async (CancellationToken token) =>
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        //產出加工列表->依照300-qr/160-鋼印/10-切割的位置
                        double QR_Stamping_Distance = 300;
                        double Fonts_Stamping_Distance = 160;
                        double Cut_Stamping_Distance = 10;
                        var smcCollection = BoxPartsParameterVMObservableCollection.OrderBy(x => x.SendMachineCommandVM.AbsoluteMoveDistance)
                        .ToList().FindAll(x =>
                        x.SendMachineCommandVM.AbsoluteMoveDistance > 0
                        && x.SendMachineCommandVM.WorkIndex >= 0);

                        var StampingPlateProcessingSequenceViewModelList = new List<StampingPlateProcessingSequenceViewModel>();
                        //產出加工工序
                        foreach (var smc in smcCollection)
                        {
                            try
                            {
                                //有QR加工需求
                                if (smc.SendMachineCommandVM.WorkScheduler_QRStamping)
                                {
                                    StampingPlateProcessingSequenceViewModelList.Add(new StampingPlateProcessingSequenceViewModel()
                                    {
                                        SteelBeltStampingStatus = SteelBeltStampingStatusEnum.QRCarving,
                                        SendMachineCommandVM = smc.SendMachineCommandVM,
                                        ProcessingAbsoluteDistance = smc.SendMachineCommandVM.AbsoluteMoveDistance - QR_Stamping_Distance
                                    });
                                }

                                //有字模加工需求
                                if (smc.SendMachineCommandVM.WorkScheduler_FontStamping)
                                {
                                    //將字分割成上下兩排

                                    var SpiltPlate = smc.SettingBaseVM.PlateNumber.SpiltByLength(smc.SettingBaseVM.SequenceCount);
                                    SpiltPlate.TryGetValue(0, out string plateFirstValue);
                                    SpiltPlate.TryGetValue(1, out string plateSecondValue);

                                    switch (smc.SettingBaseVM.SpecialSequence)
                                    {
                                        default:
                                        case SpecialSequenceEnum.OneRow:
                                            // plateFirstValue
                                            break;
                                        case SpecialSequenceEnum.TwoRow:
                                            // plateFirstValue
                                            // plateSecondValue
                                            break;
                                    }
                                }

                                //有切斷需求
                                if (smc.SendMachineCommandVM.WorkScheduler_Shearing)
                                {
                                    StampingPlateProcessingSequenceViewModelList.Add(new StampingPlateProcessingSequenceViewModel()
                                    {
                                        SteelBeltStampingStatus = SteelBeltStampingStatusEnum.Shearing,
                                        SendMachineCommandVM = smc.SendMachineCommandVM,
                                        ProcessingAbsoluteDistance = smc.SendMachineCommandVM.AbsoluteMoveDistance - Cut_Stamping_Distance
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }

                        while (true)
                        {
                            try
                            {
                                if (token.IsCancellationRequested)
                                {
                                    break;
                                }
                                var smcMachingCollection = BoxPartsParameterVMObservableCollection.OrderBy(x => x.SendMachineCommandVM.AbsoluteMoveDistance)
                                .ToList().FindAll(x =>
                                x.SendMachineCommandVM.AbsoluteMoveDistance > 0
                                && x.SendMachineCommandVM.WorkIndex >= 0);

                                if (smcMachingCollection.Count > 0)
                                {
                                    var mincutableBox = smcMachingCollection.MinBy(x => x.SendMachineCommandVM.AbsoluteMoveDistance);
                                    var minIndex = smcMachingCollection.FindIndex(x => x == mincutableBox);

                                    var minDistance = mincutableBox.SendMachineCommandVM.AbsoluteMoveDistance;
                                    double moveStep = 0.1;

                                    while (smcMachingCollection[minIndex].SendMachineCommandVM.AbsoluteMoveDistance > 0)
                                    {
                                        token.ThrowIfCancellationRequested();

                                        if (moveStep > smcMachingCollection[minIndex].SendMachineCommandVM.AbsoluteMoveDistance)
                                            moveStep = smcMachingCollection[minIndex].SendMachineCommandVM.AbsoluteMoveDistance;

                                        Parallel.ForEach(smcMachingCollection, smc =>
                                        {
                                            try
                                            {
                                                smc.SendMachineCommandVM.AbsoluteMoveDistance -= moveStep;
                                            }
                                            catch
                                            {

                                            }
                                        });


                                        await Task.Delay(2);
                                        StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StripSteelPosition -= moveStep;
                                        //System.Threading.Thread.Sleep(2);
                                    }

                                    for (int i = 0; i <= 100; i++)
                                    {
                                        token.ThrowIfCancellationRequested();
                                        mincutableBox.SendMachineCommandVM.WorkingProgress = i;
                                        await Task.Delay(10);
                                    }


                                    mincutableBox.SendMachineCommandVM.WorkingSteelBeltStampingStatus = SteelBeltStampingStatusEnum.Shearing;
                                    mincutableBox.FinishProgress = 100;
                                    mincutableBox.SendMachineCommandVM.AbsoluteMoveDistance = -100;
                                    mincutableBox.SendMachineCommandVM.IsFinish = true;

                                    await Task.Delay(500);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            catch (OperationCanceledException)
                            {

                            }
                            catch (Exception ex)
                            {

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                });
            }, () => !_workMachiningCommand.IsRunning);
        }
        */
        //public ICommand CancelMachiningCommand => WorkMachiningCommand.CreateCancelCommand();
        /*public ICommand AlltoZero
        {
            get => new RelayCommand(() =>
            {
                foreach (var bpp in BoxPartsParameterVMObservableCollection)
                {
                    bpp.SendMachineCommandVM.AbsoluteMoveDistance = 0;
                }
            });
        }*/



        private AsyncRelayCommand _sendMachiningCommand;
        public AsyncRelayCommand SendMachiningCommand
        {
            get => _sendMachiningCommand??= new (async (CancellationToken token) =>
            {
                await Task.Run(async () =>
                {
                    var ManagerVM = new DXSplashScreenViewModel
                    {
                        Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Image/svg/NewLogo_1-2.svg"),
                        Title = "GD_StampingMachine",
                        Status = (string)System.Windows.Application.Current.TryFindResource("Text_Loading"),
                        Progress = 0,
                        IsIndeterminate = false,
                        Subtitle = "Alpha 23.7.4",
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
                            var workableMachiningCollection = StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.ToList().FindAll(x => x.SendMachineCommandVM.WorkIndex >= 0);
                            var readyMachiningCollection = workableMachiningCollection.FindAll(x => !x.IsSended).OrderBy(x => x.SendMachineCommandVM.WorkIndex).ToList();
                            var sendedReadyMachiningCollection = workableMachiningCollection.FindAll(x => x.IsSended).OrderBy(x => x.SendMachineCommandVM.WorkIndex).ToList();


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
                            var progress = (double)sendedReadyMachiningCollection.Count / (double)workableMachiningCollection.Count;
                            ManagerVM.Progress = progress;
                            if (manager.State == SplashScreenState.Closed)
                            {
                             
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    manager.Show(null, WindowStartupLocation.CenterOwner, true, InputBlockMode.Window);
                                }));
                            }
                            //等待機台訊號 

                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();

                            var Rdatabit = await StampMachineData.GetRequestDatabit();
                            if (!Rdatabit)
                                continue;

                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();

                            //將兩行字上傳到機器
                            // var SpiltPlate = readymachining.SettingBaseVM.PlateNumber.SpiltByLength(readymachining.SettingBaseVM.SequenceCount);
                            var SpiltPlate = readymachining.IronPlateString.SpiltByLength(readymachining.SettingBaseVM.SequenceCount);
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
                            Guid myGuid = Guid.NewGuid();
                            byte[] bArr = myGuid.ToByteArray();
                            int autonum = Math.Abs(BitConverter.ToInt32(bArr, 0));

                            var boxIndex = readymachining.BoxIndex!=null ? readymachining.BoxIndex.Value : 0;

                            var _HMIIronPlateData = new IronPlateDataModel
                            {
                                bEngravingFinish = false,
                                bDataMatrixFinish = false,
                                //流水編號
                                iIronPlateID = autonum,
                                iStackingID = boxIndex, 
                                rXAxisPos1 = 10,
                                rXAxisPos2 =119 , 
                                rYAxisPos1 =25 , 
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
                                    await Task.Delay(1000);
                                    readymachining.IsSended = true;
                                    readymachining.ID = autonum;

                                }
                                await Task.Delay(100);
                            }
                            while (!sendhmi);

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        manager?.Close();
                        manager = null;
                    }
                },token);
            }, ()=>  !_sendMachiningCommand.IsRunning);
        }

        public ICommand SendMachiningCancelCommand=> SendMachiningCommand.CreateCancelCommand();

        //
        private async Task a()
        {
           var history = await StampMachineData.GetIronPlateDataCollection();
            if (history.Item1)
            {
               //history.Item2
                var projectDistributeCollection = StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection;

                foreach (var project in projectDistributeCollection)
                {
                    //project.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.FindIndex(x => x.SendMachineCommandVM.IsSended && x.ID == )
    
            }
            }

        }



        //篩選器
        [JsonIgnore]
        public DevExpress.Mvvm.ICommand<RowFilterArgs>NotArrangeWorkRowFilterCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<RowFilterArgs>(args =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.PartsParameterViewModel PartsParameter)
                {
                    if (PartsParameter.SendMachineCommandVM.WorkIndex >= 0)
                    {
                        args.Visible = false;
                    }
                    else if(PartsParameter.SendMachineCommandVM.IsFinish)
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
                    if (PartsParameter.SendMachineCommandVM.WorkIndex >= 0)
                    {
                        args.Visible = true;
                    }
                    else if (PartsParameter.SendMachineCommandVM.IsFinish)
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
                    //全選
                    if (e is IList<PartsParameterViewModel> Itemsource)
                    {
                        foreach (var partsParameter in Itemsource)
                        {
                            if (!partsParameter.SendMachineCommandVM.IsFinish)
                            {
                                if (partsParameter.SendMachineCommandVM.WorkIndex < 0)
                                    SetPartsParameterWork(partsParameter);
                            }
                        }
                    }
                    //部分
                    else if (e is IList<object> eList)
                    {
                        foreach (var item in eList)
                        {
                            if (item is PartsParameterViewModel partsParameter)
                            {
                                if (partsParameter.SendMachineCommandVM.WorkIndex < 0)
                                    SetPartsParameterWork(partsParameter);
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
            partsParameter.SendMachineCommandVM.IsFinish = false;
            var indexMax = BoxPartsParameterVMObservableCollection.Max(x => x.SendMachineCommandVM.WorkIndex);
            if (indexMax < 0)
            {
                partsParameter.SendMachineCommandVM.WorkIndex = 0;
            }
            else
            {
                partsParameter.SendMachineCommandVM.WorkIndex = indexMax + 1;
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
                    if (e is IList<PartsParameterViewModel> Itemsource)
                    {
                        foreach (var item in Itemsource)
                        {
                            CancelPartsParameterWork(item);
                        }
                    }
                    //部分
                    else if (e is IList<object> eList)
                    {
                        foreach (var item in eList)
                        {
                            if (item is PartsParameterViewModel partsParameter)
                            {
                                CancelPartsParameterWork(partsParameter);
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
            partsParameter.SendMachineCommandVM.IsFinish = false;
            partsParameter.SendMachineCommandVM.WorkIndex = -1;
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
