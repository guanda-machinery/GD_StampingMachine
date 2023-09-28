using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class MachineFunctionTestViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => throw new NotImplementedException();

        #region 機台移動命令

        private double _steelBeltLength = 450;

        public double SteelBeltLength
        {
            get => _steelBeltLength;
            set { _steelBeltLength = value; OnPropertyChanged(); }
        }

        private double _stampingProductWidth = 110;
        /// <summary>
        /// 素材寬度
        /// </summary>
        public double StampingProductWidth
        {
            get => _stampingProductWidth;
            set { _stampingProductWidth = value; OnPropertyChanged(); }
        }



        private double _stampingProductMargin = 5;
        /// <summary>
        /// 鐵片間隔
        /// </summary>
        public double StampingProductMargin
        {
            get => _stampingProductMargin;
            set { _stampingProductMargin = value; OnPropertyChanged(); }
        }

        private double _stampingFontHeight = 15;
        /// <summary>
        /// 字模高度 雙排字會移動0/StampingFontHeight ，單排字則看上中下決定0/ 1/2 StampingFontHeight
        /// </summary>
        public double StampingFontHeight
        {
            get => _stampingFontHeight;
            set { _stampingFontHeight = value; OnPropertyChanged(); }
        }



        private double _qr_Stamping_Distance = 50;
        public double QR_Stamping_Distance
        {
            get => _qr_Stamping_Distance;
            set { _qr_Stamping_Distance = value; OnPropertyChanged(); }
        }

        private double _fonts_Stamping_Distance = 50;
        public double Fonts_Stamping_Distance
        {
            get => _fonts_Stamping_Distance;
            set { _fonts_Stamping_Distance = value; OnPropertyChanged(); }
        }


        private double _magnificationRatio = 1;
        public double MagnificationRatio
        {
            get
            {
                if (_magnificationRatio <= 0)
                    _magnificationRatio = 0.01;
                if (_magnificationRatio > 2)
                    _magnificationRatio = 2;

                _magnificationRatio = Math.Round(_magnificationRatio, 3);
                return _magnificationRatio;
            }
            set { _magnificationRatio = value; OnPropertyChanged(); }
        }







        private ObservableCollection<SendMachineCommandViewModel> _sendMachineCommandVMObservableCollection;

        public ObservableCollection<SendMachineCommandViewModel> SendMachineCommandVMObservableCollection
        {
            get => _sendMachineCommandVMObservableCollection ??= new ObservableCollection<SendMachineCommandViewModel>();
            set { _sendMachineCommandVMObservableCollection = value; OnPropertyChanged(); }
        }






        /// <summary>
        /// 新建加工
        /// </summary>
        public ICommand SendMachineCommand
        {
            get => new RelayCommand<object>(para =>
            {
                bool _scheduler_FontStamping = false;
                bool _scheduler_QRStamping = false;
                bool _scheduler_Shearing = true;

                GD_StampingMachine.ViewModels.ParameterSetting.SettingBaseViewModel SettingBaseVM = new ParameterSetting.NumberSettingViewModel()
                {
                    SequenceCount = 0
                };


                if (para != null)
                {
                    if (para is GD_StampingMachine.ViewModels.ParameterSetting.SettingBaseViewModel ParaSettingBaseVM)
                    {
                        _scheduler_FontStamping = true;


                        SettingBaseVM = ParaSettingBaseVM;
                        if (SettingBaseVM is ParameterSetting.QRSettingViewModel)
                        {
                            _scheduler_QRStamping = true;
                        }
                        else
                        {
                            _scheduler_QRStamping = false;
                        }
                    }
                }






                //先找第一個在哪 若不存在則給預設值
                if (SendMachineCommandVMObservableCollection.Count == 0)
                {
                    SendMachineCommandVMObservableCollection.Add(new SendMachineCommandViewModel()
                    {
                        SteelBeltStampingStatus = SteelBeltStampingStatusEnum.None,
                        WorkNumber = 0,
                        AbsoluteMoveDistance = QR_Stamping_Distance + Fonts_Stamping_Distance,
                        SettingBaseVM = SettingBaseVM,
                        StampWidth = 35,

                        WorkScheduler_FontStamping = _scheduler_FontStamping,
                        WorkScheduler_QRStamping = _scheduler_QRStamping,
                        WorkScheduler_Shearing = _scheduler_Shearing,
                        IsFinish = false

                    }); ;
                }
                else
                {
                    //找出最後一個
                    var AbsoluteMaxSMC = SendMachineCommandVMObservableCollection.MaxBy(x => x.AbsoluteMoveDistance);//.LastOrDefault();
                    var WorkNumberMax = SendMachineCommandVMObservableCollection.Max(x => x.WorkNumber);//.LastOrDefault();
                    var Distance = AbsoluteMaxSMC.StampWidth + StampingProductMargin;
                    if (AbsoluteMaxSMC.AbsoluteMoveDistance >= 0)
                    {
                        Distance += AbsoluteMaxSMC.AbsoluteMoveDistance;
                    }

                    SendMachineCommandVMObservableCollection.Add(new SendMachineCommandViewModel()
                    {
                        SteelBeltStampingStatus = SteelBeltStampingStatusEnum.None,
                        WorkNumber = WorkNumberMax + 1,
                        AbsoluteMoveDistance = Distance,
                        SettingBaseVM = SettingBaseVM,
                        StampWidth = AbsoluteMaxSMC.StampWidth,

                        WorkScheduler_FontStamping = _scheduler_FontStamping,
                        WorkScheduler_QRStamping = _scheduler_QRStamping,
                        WorkScheduler_Shearing = _scheduler_Shearing,
                        IsFinish = false
                    }); ; ;
                }



            });
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand DeleteSendMachineCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                if (obj is GridControl ProjectGridControl)
                {
                    if (ProjectGridControl.ItemsSource is IList<GD_StampingMachine.ViewModels.SendMachineCommandViewModel> IEnumerableData)
                    {
                        if (ProjectGridControl.SelectedItems != null)
                        {
                            for (int i = ProjectGridControl.SelectedItems.Count - 1; i >= 0; i--)
                            {
                                var rIndex = IEnumerableData.FindIndex(x => x == ProjectGridControl.SelectedItems[i]);
                                if (rIndex != -1)
                                    IEnumerableData.RemoveAt(rIndex);
                            }

                        }
                    }

                }
            });
        }


        private ObservableCollection<StampingPlateProcessingSequenceViewModel> _stampingPlateProcessingSequenceVMObservableCollection;

        /// <summary>
        /// 沖壓加工陣列
        /// </summary>
        public ObservableCollection<StampingPlateProcessingSequenceViewModel> StampingPlateProcessingSequenceVMObservableCollection
        {
            get => _stampingPlateProcessingSequenceVMObservableCollection ??= new ObservableCollection<StampingPlateProcessingSequenceViewModel>();
            set { _stampingPlateProcessingSequenceVMObservableCollection = value; OnPropertyChanged(); }
        }












        //需輸入三種數字 代表三種機器距離切割線的位置 並以切割線為基準算出所有鋼片的絕對座標
        //用一按鍵重整數值

        public ICommand X_axisMoveSendMachineCommand
        {
            get => new RelayCommand<object>(para =>
            {
                double MoveStep = 1;
                if (para is double doublepara)
                {
                    MoveStep = doublepara;
                }
                AbsoluteMoveDistance(MoveStep);


            });
        }



        private bool _beltIsMoving;
        public bool BeltIsMoving
        {
            get => _beltIsMoving;
            set
            {
                _beltIsMoving = value; OnPropertyChanged();
            }
        }

        /// <summary>
        /// 移動所有物件
        /// </summary>
        public ICommand BeltMoveCommand
        {
            get => new RelayCommand(() =>
            {
                BeltIsMoving = true;

                foreach (var SMC in SendMachineCommandVMObservableCollection)
                {
                    //SMC.AbsoluteMoveDistance 
                }
                BeltIsMoving = false;
            });
        }

        private bool _isWorking = false;
        public bool IsWorking { get => _isWorking; set { _isWorking = value; OnPropertyChanged(); } }



        public ICommand SoftSendMachineCommand
        {
            get => new RelayCommand(() =>
            {

                SendMachineCommandVMObservableCollection = new ObservableCollection<SendMachineCommandViewModel>
                (SendMachineCommandVMObservableCollection.OrderBy(x => x.AbsoluteMoveDistance));
            });
        }


        /// <summary>
        /// 產生加工序列
        /// </summary>
        public ICommand GenerateProcessingSequenceCommand
        {
            get => new RelayCommand(() =>
            {
                //先排序 並將已經加工完成的物件丟掉
                var smcCollection = SendMachineCommandVMObservableCollection.ToList().FindAll(x => !x.IsFinish && (x.WorkScheduler_QRStamping || x.WorkScheduler_FontStamping || x.WorkScheduler_Shearing));
                //計算所有工序的座標 並於稍後排列

                //陣列內的物件 : 加工類型/加工物件/需移動的距離
                var StampingPlateProcessingSequenceViewModelList = new List<StampingPlateProcessingSequenceViewModel>();

                foreach (var smc in smcCollection)
                {
                    //有QR加工需求
                    if (smc.WorkScheduler_QRStamping)
                    {
                        StampingPlateProcessingSequenceViewModelList.Add(new StampingPlateProcessingSequenceViewModel()
                        {
                            SteelBeltStampingStatus = SteelBeltStampingStatusEnum.QRCarving,
                            SendMachineCommandVM = smc,
                            ProcessingAbsoluteDistance = smc.AbsoluteMoveDistance - (QR_Stamping_Distance + Fonts_Stamping_Distance)
                        });
                    }

                    //有字模加工需求
                    //※須注意字模有分靠上 靠下 置中
                    //且當字模有兩排時需產出兩條加工程式
                    if (smc.WorkScheduler_FontStamping)
                    {
                        switch (smc.SettingBaseVM.SpecialSequence)
                        {
                            default:
                            case SpecialSequenceEnum.OneRow:
                                switch (smc.SettingBaseVM.VerticalAlign)
                                {
                                    case VerticalAlignEnum.Top:
                                        StampingPlateProcessingSequenceViewModelList.Add(new StampingPlateProcessingSequenceViewModel()
                                        {
                                            SteelBeltStampingStatus = SteelBeltStampingStatusEnum.Stamping,
                                            SendMachineCommandVM = smc,
                                            ProcessingAbsoluteDistance = smc.AbsoluteMoveDistance - Fonts_Stamping_Distance
                                        });
                                        break;

                                    case VerticalAlignEnum.Center:
                                        StampingPlateProcessingSequenceViewModelList.Add(new StampingPlateProcessingSequenceViewModel()
                                        {
                                            SteelBeltStampingStatus = SteelBeltStampingStatusEnum.Stamping,
                                            SendMachineCommandVM = smc,
                                            ProcessingAbsoluteDistance = smc.AbsoluteMoveDistance - (Fonts_Stamping_Distance - StampingFontHeight / 2)
                                        });
                                        break;

                                    case VerticalAlignEnum.Bottom:
                                        StampingPlateProcessingSequenceViewModelList.Add(new StampingPlateProcessingSequenceViewModel()
                                        {
                                            SteelBeltStampingStatus = SteelBeltStampingStatusEnum.Stamping,
                                            SendMachineCommandVM = smc,
                                            ProcessingAbsoluteDistance = smc.AbsoluteMoveDistance - (Fonts_Stamping_Distance - StampingFontHeight)
                                        });
                                        break;
                                    default:
                                        break;
                                }

                                break;

                            case SpecialSequenceEnum.TwoRow:

                                StampingPlateProcessingSequenceViewModelList.Add(new StampingPlateProcessingSequenceViewModel()
                                {
                                    SteelBeltStampingStatus = SteelBeltStampingStatusEnum.Stamping,
                                    SendMachineCommandVM = smc,
                                    ProcessingAbsoluteDistance = smc.AbsoluteMoveDistance - (Fonts_Stamping_Distance)
                                });

                                StampingPlateProcessingSequenceViewModelList.Add(new StampingPlateProcessingSequenceViewModel()
                                {
                                    SteelBeltStampingStatus = SteelBeltStampingStatusEnum.Stamping,
                                    SendMachineCommandVM = smc,
                                    ProcessingAbsoluteDistance = smc.AbsoluteMoveDistance - (Fonts_Stamping_Distance - StampingFontHeight)
                                });
                                break;
                        }
                    }

                    //有切斷需求
                    if (smc.WorkScheduler_Shearing)
                    {
                        StampingPlateProcessingSequenceViewModelList.Add(new StampingPlateProcessingSequenceViewModel()
                        {
                            SteelBeltStampingStatus = SteelBeltStampingStatusEnum.Shearing,
                            SendMachineCommandVM = smc,
                            ProcessingAbsoluteDistance = smc.AbsoluteMoveDistance
                        });
                    }

                }

                //重新排序 依照距離順序由小到大
                StampingPlateProcessingSequenceViewModelList = StampingPlateProcessingSequenceViewModelList.OrderBy(x => x.ProcessingAbsoluteDistance).ToList();
                //計算相對距離

                if (StampingPlateProcessingSequenceViewModelList.Count > 0)
                    StampingPlateProcessingSequenceViewModelList[0].ProcessingRelativeDistance = StampingPlateProcessingSequenceViewModelList[0].ProcessingAbsoluteDistance;
                for (int i = 1; i < StampingPlateProcessingSequenceViewModelList.Count; i++)
                {
                    StampingPlateProcessingSequenceViewModelList[i].ProcessingRelativeDistance = StampingPlateProcessingSequenceViewModelList[i].ProcessingAbsoluteDistance - StampingPlateProcessingSequenceViewModelList[i - 1].ProcessingAbsoluteDistance;
                }


                StampingPlateProcessingSequenceVMObservableCollection = StampingPlateProcessingSequenceViewModelList.ToObservableCollection();
            });
        }


        /// <summary>
        /// 暫停用的觸發
        /// </summary>
        // private ManualResetEvent autoWorkCommandResetEvent = new ManualResetEvent(true);

        ///中斷用處發
        //private CancellationTokenSource AutoWorkcts;
        private AsyncRelayCommand _autoWorkCommand;
        public AsyncRelayCommand AutoWorkCommand
        {
            get => _autoWorkCommand??=new AsyncRelayCommand(async() =>
            {
                try
                {
                    //執行大量運算

                    IsWorking = true;

                    var FindPredicate = new Predicate<StampingPlateProcessingSequenceViewModel>(
                        x => x.ProcessingAbsoluteDistance >= 0
                    && !x.SendMachineCommandVM.IsFinish
                    && !x.ProcessingIsFinish);

                    //如果存在
                    if (StampingPlateProcessingSequenceVMObservableCollection.FindIndex(x => x.ProcessingAbsoluteDistance < 0
                 && !x.SendMachineCommandVM.IsFinish
                 && !x.ProcessingIsFinish) != -1)
                    {
                        //有無法加工到的工序 需先倒轉鋼帶或忽略無法加工的工序
                        if (await GD_CommonLibrary.Method.MessageBoxResultShow.ShowYesNo("", $"\r\n;") != System.Windows.MessageBoxResult.Yes)
                            return;
                    }

                    //  while (StampingPlateProcessingSequenceVMObservableCollection.FindIndex(x => x.ProcessingAbsoluteDistance >= 0 && !x.IsFinish) != -1)
                    while (StampingPlateProcessingSequenceVMObservableCollection.FindIndex(FindPredicate) != -1)
                    {

                        //把最上面ProcessingAbsoluteDistance大於0的鐵片移動到指定位置
                        var FIndex = StampingPlateProcessingSequenceVMObservableCollection.FindIndex(FindPredicate);

                        var moveDistance = StampingPlateProcessingSequenceVMObservableCollection[FIndex].ProcessingAbsoluteDistance;
                        double moveStep = -0.5;
                        while (StampingPlateProcessingSequenceVMObservableCollection[FIndex].ProcessingAbsoluteDistance >= Math.Abs(moveStep))
                        {

                            //StampingPlateProcessingSequenceVMObservableCollection[FIndex].ProcessingAbsoluteDistance-= moveStep;
                            //須重構成function 與移動指令共用
                            AbsoluteMoveDistance(moveStep);
                            await Task.Delay(10);
                        }
                        //最後一階
                        var LastMove = StampingPlateProcessingSequenceVMObservableCollection[FIndex].ProcessingAbsoluteDistance;
                        AbsoluteMoveDistance(-LastMove);
                        StampingPlateProcessingSequenceVMObservableCollection[FIndex].ProcessingAbsoluteDistance = 0;
                        //每個工作到位置後 等待三秒
                        if (false)
                        {
                            //單動
                            await Task.Delay(3000);
                            StampingPlateProcessingSequenceVMObservableCollection[FIndex].ProcessingIsFinish = true;
                            StampingPlateProcessingSequenceVMObservableCollection[FIndex].SendMachineCommandVM.SteelBeltStampingStatus = StampingPlateProcessingSequenceVMObservableCollection[FIndex].SteelBeltStampingStatus;
                        }
                        else
                        {
                            //所有在指定位置上的都可以加工
                            var ReadyWorkList = StampingPlateProcessingSequenceVMObservableCollection.ToList().FindAll(x => x.ProcessingAbsoluteDistance == 0 && !x.ProcessingIsFinish);

                            List<Task> AllworkTask = new();

                            foreach (var work in ReadyWorkList)
                            {
                                if (work.SendMachineCommandVM.IsFinish)
                                    continue;

                                Task WrokTask = Task.Run(async () =>
                                {
                                    try
                                    {
                                        //把該鋼片設為正在加工
                                        //在這邊加入進度條
                                        work.SendMachineCommandVM.IsWorking = true;
                                        work.SendMachineCommandVM.WorkingSteelBeltStampingStatus = work.SteelBeltStampingStatus;
                                        work.SendMachineCommandVM.WorkingProgress = 0;

                                        //各種工作所花時間不同
                                        //autoWorkCommandResetEvent.WaitOne();

                                        while (work.SendMachineCommandVM.WorkingProgress < 100d)
                                        {
                                            switch (work.SteelBeltStampingStatus)
                                            {
                                                case SteelBeltStampingStatusEnum.QRCarving:
                                                    work.SendMachineCommandVM.WorkingProgress += 100d / 150d;
                                                    break;
                                                case SteelBeltStampingStatusEnum.Stamping:
                                                    work.SendMachineCommandVM.WorkingProgress += 100d / 175d;
                                                    break;
                                                case SteelBeltStampingStatusEnum.Shearing:
                                                    work.SendMachineCommandVM.WorkingProgress += 100d / 100d;
                                                    break;

                                                default:
                                                case SteelBeltStampingStatusEnum.None:
                                                    work.SendMachineCommandVM.WorkingProgress = 100;

                                                    break;
                                            }
                                            await Task.Delay(5);
                                        }

                                        await Task.Delay(500);

                                        if (work == StampingPlateProcessingSequenceVMObservableCollection.Last(x => x.SendMachineCommandVM == work.SendMachineCommandVM))
                                        {
                                            work.SendMachineCommandVM.IsFinish = true;
                                        }



                                        work.ProcessingIsFinish = true;
                                        work.SendMachineCommandVM.WorkingProgress = 100d;
                                        work.SendMachineCommandVM.IsWorking = false;
                                        work.SendMachineCommandVM.SteelBeltStampingStatus = work.SteelBeltStampingStatus;
                                        work.SendMachineCommandVM.WorkingProgress = 0;
                                        //await Task.Delay(3000);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debugger.Break();
                                    }
                                });
                                AllworkTask.Add(WrokTask);
                                await Task.Delay(500);



                            }

                            await Task.WhenAll(AllworkTask);
                        }
                        //同步加工 如果有其他同位置的零件可以一起加工
                    }


                    await Task.Delay(1000);

                    IsWorking = false;
                }
                catch (Exception ex)
                {

                }
            }, () => !_autoWorkCommand.IsRunning);
        }

        public ICommand Recover
        {
            get => new RelayCommand(() =>
            {

                var returnOriginValue = SendMachineCommandVMObservableCollection.Min(x => x.AbsoluteMoveDistance);
                returnOriginValue = returnOriginValue - QR_Stamping_Distance - Fonts_Stamping_Distance;

                foreach (var item in SendMachineCommandVMObservableCollection)
                {
                    item.AbsoluteMoveDistance -= returnOriginValue;
                    item.SteelBeltStampingStatus = SteelBeltStampingStatusEnum.None;
                    item.IsFinish = false;
                }

                foreach (var item in StampingPlateProcessingSequenceVMObservableCollection)
                {
                    item.ProcessingAbsoluteDistance -= returnOriginValue;
                    item.ProcessingIsFinish = false;// true;
                    item.SendMachineCommandVM.WorkingProgress = 0;
                }



            });
        }

        /*public ICommand PauseAutoWorkCommand
        {
            get => new RelayCommand(() =>
            {
                //autoWorkCommandResetEvent.WaitOne();
                autoWorkCommandResetEvent.Set();
                autoWorkCommandResetEvent.Reset();

            });
        }*/
        /* public ICommand StopAutoWorkCommand
         {
             get => new RelayCommand(() =>
             {
                 if (AutoWorkcts != null && !AutoWorkcts.IsCancellationRequested)
                 {
                     AutoWorkcts.Cancel();
                     IsWorking = false;
                 }

             });
         }*/


        public void AbsoluteMoveDistance(double MoveStep)
        {
            foreach (var smc in SendMachineCommandVMObservableCollection)
            {
                smc.AbsoluteMoveDistance += MoveStep;
            }

            foreach (var spp in StampingPlateProcessingSequenceVMObservableCollection)
            {
                spp.ProcessingAbsoluteDistance += MoveStep;
            }
        }














        #endregion

    }

    public class StampingPlateProcessingSequenceViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => throw new NotImplementedException();


        private SteelBeltStampingStatusEnum _steelBeltStampingStatus = SteelBeltStampingStatusEnum.None;

        /// <summary>
        /// 命令
        /// </summary>
        public SteelBeltStampingStatusEnum SteelBeltStampingStatus
        {
            get => _steelBeltStampingStatus;
            set { _steelBeltStampingStatus = value; OnPropertyChanged(); }
        }

        private GD_StampingMachine.ViewModels.SendMachineCommandViewModel _sendMachineCommandVM;
        public GD_StampingMachine.ViewModels.SendMachineCommandViewModel SendMachineCommandVM
        {
            get => _sendMachineCommandVM;
            set
            {
                _sendMachineCommandVM = value;
                OnPropertyChanged();
            }
        }

        private double _processingAbsoluteDistance = 0;
        public double ProcessingAbsoluteDistance
        {
            get => _processingAbsoluteDistance;
            set
            {
                _processingAbsoluteDistance = value;
                OnPropertyChanged();
            }
        }


        private double? _processingRelativeDistance;
        public double? ProcessingRelativeDistance
        {
            get => _processingRelativeDistance;
            set
            {
                _processingRelativeDistance = value; OnPropertyChanged();
            }

        }


        private bool _processingIsFinish = false;
        /// <summary>
        /// 標記為完成
        /// </summary>
        public bool ProcessingIsFinish { get => _processingIsFinish; set { _processingIsFinish = value; OnPropertyChanged(); } }

    }

}
