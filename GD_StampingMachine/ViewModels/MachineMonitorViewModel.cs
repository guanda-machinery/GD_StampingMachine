using CommunityToolkit.Mvvm.Input;
using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.Xpf;
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
            StampingSteelBeltVMObservableCollection = new();

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
            }
        }


        /// <summary>
        /// 鋼帶捲集合
        /// </summary>
        private ObservableCollection<StampingSteelBeltViewModel> _stampingSteelBeltVMObservableCollection = new();
        public ObservableCollection<StampingSteelBeltViewModel> StampingSteelBeltVMObservableCollection { get => _stampingSteelBeltVMObservableCollection; set { _stampingSteelBeltVMObservableCollection = value; OnPropertyChanged(); } }


        public double StampWidth { get; set; } = 50;


        private ObservableCollection<PartsParameterViewModel> _boxPartsParameterVMObservableCollection
        {
            get
            {
                if (StampingMachineSingleton.Instance.SelectedProjectDistributeVM != null)
                    return StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection;
                return new ObservableCollection<PartsParameterViewModel>();
            }
        }
        public ICommand SoftSendMachineCommand
        {
            get => new RelayCommand(() =>
            {

                if (_boxPartsParameterVMObservableCollection != null)
                {
                    if (_boxPartsParameterVMObservableCollection.Count != 0)
                    {
                        _boxPartsParameterVMObservableCollection[0].SendMachineCommandVM.AbsoluteMoveDistance = 0;

                        for (int i = 1; i < _boxPartsParameterVMObservableCollection.Count; i++)
                        {
                            _boxPartsParameterVMObservableCollection[i].SendMachineCommandVM.AbsoluteMoveDistance = _boxPartsParameterVMObservableCollection[0].SendMachineCommandVM.AbsoluteMoveDistance + StampWidth * i;
                        }

                        for (int i = 1; i < _boxPartsParameterVMObservableCollection.Count; i++)
                        {
                            _boxPartsParameterVMObservableCollection[i].SendMachineCommandVM.RelativeMoveDistance = _boxPartsParameterVMObservableCollection[0].SendMachineCommandVM.RelativeMoveDistance + StampWidth * i;
                        }
                        StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.StripSteelLength = _boxPartsParameterVMObservableCollection.MaxOrDefault(x => x.SendMachineCommandVM.RelativeMoveDistance) + StampWidth + 100;
                    }

                    StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.StripSteelPosition = 0;
                }
            });
        }

        public ICommand SetStatusSendMachineCommand
        {
            get => new RelayCommand(() =>
            {
                //ObservableCollection<PartsParameterViewModel> _boxPartsParameterVMObservableCollection = StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection;
                if (_boxPartsParameterVMObservableCollection != null)
                {
                    for (int i = 0; i < _boxPartsParameterVMObservableCollection.Count; i++)
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

                        _boxPartsParameterVMObservableCollection[i].SendMachineCommandVM.WorkingSteelBeltStampingStatus = steelBeltStampingStatus;
                        _boxPartsParameterVMObservableCollection[i].SendMachineCommandVM.SteelBeltStampingStatus = steelBeltStampingStatus;
                    }
                }



            });
        }


        private AsyncRelayCommand _sendWorkMachineCommand;
        public AsyncRelayCommand SendWorkMachineCommand
        {
            get => _sendWorkMachineCommand ??= new AsyncRelayCommand(async (CancellationToken token) =>
            {
                try
                {
                    //產出加工列表->依照300-qr/160-鋼印/10-切割的位置
                    double QR_Stamping_Distance = 300;
                    double Fonts_Stamping_Distance = 160;
                    double Cut_Stamping_Distance = 10;

                    var StampingPlateProcessingSequenceViewModelList = new List<StampingPlateProcessingSequenceViewModel>();
                    //先排序 並將已經加工完成的物件去掉
                    var smcCollection = _boxPartsParameterVMObservableCollection.ToList().FindAll(
                        x => !x.SendMachineCommandVM.IsFinish && x.SendMachineCommandVM.WorkIndex >= 0 &&
                        (x.SendMachineCommandVM.WorkScheduler_QRStamping || x.SendMachineCommandVM.WorkScheduler_FontStamping || x.SendMachineCommandVM.WorkScheduler_Shearing));

                    smcCollection.OrderBy(x => x.SendMachineCommandVM.WorkIndex);

                    //產出加工工序
                    foreach(var smc in smcCollection)
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
                        catch(Exception ex)
                        { 
                        }
                    }

                    await Task.Delay(1);

                    await Task.Run(async () =>
                    {
                        //   _boxPartsParameterVMObservableCollection.FindIndex(x=>x.SendMachineCommandVM.WorkScheduler_QRStamping is qr)
                        return;
                        while (true)
                        {
                            try
                            { 
                            
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    });
                }
                catch (Exception ex)
                { 
                }
            }, () => !_sendWorkMachineCommand.IsRunning);
        }
                





        private AsyncRelayCommand _moveSendMachineCommand;
        public AsyncRelayCommand MoveSendMachineCommand
        {
            get => _moveSendMachineCommand ??= new AsyncRelayCommand(async (CancellationToken token) =>
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cutableBoxPartsCollection = _boxPartsParameterVMObservableCollection.ToList().FindAll(x => x.SendMachineCommandVM.AbsoluteMoveDistance > 0);
                            if (token.IsCancellationRequested)
                            {
                                break;
                            }
                            else if (cutableBoxPartsCollection.Count > 0)
                            {
                                var mincutableBox = cutableBoxPartsCollection.MinBy(x => x.SendMachineCommandVM.AbsoluteMoveDistance);
                                var minIndex = _boxPartsParameterVMObservableCollection.FindIndex(x => x == mincutableBox);
                                var minDistance = mincutableBox.SendMachineCommandVM.AbsoluteMoveDistance;
                                double moveStep = 0.1;

                                while (_boxPartsParameterVMObservableCollection[minIndex].SendMachineCommandVM.AbsoluteMoveDistance > 0)
                                {
                                    token.ThrowIfCancellationRequested();

                                    if (moveStep > _boxPartsParameterVMObservableCollection[minIndex].SendMachineCommandVM.AbsoluteMoveDistance)
                                        moveStep = _boxPartsParameterVMObservableCollection[minIndex].SendMachineCommandVM.AbsoluteMoveDistance;
                                    // foreach (var smc in _boxPartsParameterVMObservableCollection)
                                    Parallel.ForEach(_boxPartsParameterVMObservableCollection, smc =>
                                    {
                                        smc.SendMachineCommandVM.AbsoluteMoveDistance -= moveStep;
                                    });


                                    await Task.Delay(2);
                                    StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.StripSteelPosition -= moveStep;
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

                                await Task.Delay(500);
                                //System.Threading.Thread.Sleep(500);
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
                catch (OperationCanceledException)
                {

                }
            }, () => !_moveSendMachineCommand.IsRunning);
        }

        public ICommand StopMoveSendMachineCommand
        {
            get => MoveSendMachineCommand.CreateCancelCommand();
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
            var indexMax = _boxPartsParameterVMObservableCollection.Max(x => x.SendMachineCommandVM.WorkIndex);
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
