using CommunityToolkit.Mvvm.Input;
using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.Xpf;
using DevExpress.Pdf.Native;
using DevExpress.Xpf.Editors.ExpressionEditor;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.XtraEditors.Filtering;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
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


        /// <summary>
        /// 鋼帶捲集合
        /// </summary>
        //private ObservableCollection<StampingSteelBeltViewModel> _stampingSteelBeltVMObservableCollection = new();
        //public ObservableCollection<StampingSteelBeltViewModel> StampingSteelBeltVMObservableCollection { get => _stampingSteelBeltVMObservableCollection; set { _stampingSteelBeltVMObservableCollection = value; OnPropertyChanged(); } }


        public double StampWidth { get; set; } = 50;


        private ObservableCollection<PartsParameterViewModel> BoxPartsParameterVMObservableCollection=> StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection;
     

        public ICommand SetStatusSendMachineCommand
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
        }


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

                }
                catch (Exception ex)
                { 
                }
            }, () => !_sortWorkMachineCommand.IsRunning);
        }

        private AsyncRelayCommand _workMachiningCommand;
        /// <summary>
        /// 加工命令
        /// </summary>



        public AsyncRelayCommand WorkMachiningCommand
        {
            get => _workMachiningCommand ??= new AsyncRelayCommand(async (CancellationToken token) =>
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
            }, () => !_workMachiningCommand.IsRunning);
        }



        public ICommand CancelMachiningCommand => WorkMachiningCommand.CreateCancelCommand();
        public ICommand AlltoZero
        {
            get => new RelayCommand(() =>
            {
                foreach (var bpp in BoxPartsParameterVMObservableCollection)
                {
                    bpp.SendMachineCommandVM.AbsoluteMoveDistance = 0;
                }
            });
        }

        public AsyncRelayCommand SendMachiningCommand
        {
            get => new AsyncRelayCommand(async (CancellationToken token) =>
            {
                //開始依序傳送資料
                while (true)
                {
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    var readyMachiningCollection = BoxPartsParameterVMObservableCollection.OrderBy(x => x.SendMachineCommandVM.WorkIndex)
                    .ToList().FindAll(x =>
                     !x.SendMachineCommandVM.IsFinish
                    && x.SendMachineCommandVM.WorkIndex >= 0);
                    if (readyMachiningCollection.Count == 0)
                        break;

                    var readymachining =  readyMachiningCollection.First();
                    if (readymachining == null)
                        break;

                    //將兩行字上傳到機器
                    if(await  GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance.AsyncSendMachiningData(readymachining.SettingBaseVM , int.MaxValue))
                    {
                        //成功上傳 等待他加工完成
                        //等待數秒後當作加工完成
                        readymachining.SendMachineCommandVM.WorkingProgress = 0;

                        await Task.Delay(5000);
                        readymachining.SendMachineCommandVM.IsFinish = true;
                    }

                }




            }, ()=>!SendMachiningCommand.IsRunning);
        }



        public AsyncRelayCommand SendMachiningCommand2
        {
            get => new AsyncRelayCommand(async (CancellationToken token) =>
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                var StampMachineData = GD_StampingMachine.Singletons.StampMachineDataSingleton.Instance;
                var a = await StampMachineData.GetHMIIronPlateData();
                var b = await StampMachineData.GetIronPlateDataCollection();

                List<IronPlateDataModel> ironPlateDataList = new();
                BoxPartsParameterVMObservableCollection.ForEach(Bpp =>
                {
                    int boxIndex = 1;
                    if (Bpp.BoxIndex != null)
                        boxIndex = Bpp.BoxIndex.Value;

                    var SpiltPlate = Bpp.SettingBaseVM.PlateNumber.SpiltByLength(Bpp.SettingBaseVM.SequenceCount);
                    SpiltPlate.TryGetValue(0, out string plateFirstValue);
                    SpiltPlate.TryGetValue(1, out string plateSecondValue);

                    ironPlateDataList.Add(new IronPlateDataModel()
                    {
                        bEngravingFinish = false,
                        bDataMatrixFinish = false,
                        iIronPlateID=1,
                        iStackingID = boxIndex,
                        rXAxisPos1 = 10 ,
                        rYAxisPos1 = 119, 
                        rXAxisPos2 = 25,
                        rYAxisPos2 = 119,
                        sIronPlateName1 = plateFirstValue,
                        sIronPlateName2 = plateSecondValue
                    });

                });

                var setBool  = await StampMachineData.SetIronPlateDataCollection2(ironPlateDataList);
                //var c = await StampMachineData.SetIronPlateDataCollection();

            }, () => !SendMachiningCommand.IsRunning);
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
                //全選
                if (e is IList<PartsParameterViewModel> Itemsource)
                {
                    foreach(var partsParameter in Itemsource)
                    {
                        if(!partsParameter.SendMachineCommandVM.IsFinish)
                        {
                            if (partsParameter.SendMachineCommandVM.WorkIndex < 0)
                                SetPartsParameterWork(partsParameter);
                        }
                    }
                }               
                //部分
               else if (e is IList<object> eList)
               {
                    foreach(var item in eList)
                    {
                        if(item is PartsParameterViewModel partsParameter)
                        {
                            if(partsParameter.SendMachineCommandVM.WorkIndex < 0)
                                SetPartsParameterWork(partsParameter);
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
