using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using DevExpress.Office.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.StructuredStorage.Internal;
using DevExpress.Xpf.Grid;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.XtraScheduler.Native;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class MachineFunctionViewModel : BaseViewModelWithLog
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("btnDescription_MachineFunction");



        public ParameterSettingViewModel ParameterSettingVM { get => StampingMainModel.Instance.ParameterSettingVM; }
        public StampingFontChangedViewModel StampingFontChangedVM { get => StampingMainModel.Instance.StampingFontChangedVM; }

        public MachineFunctionViewModel()
        {

            var DegreeRate = 0;
            if (ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.Count != 0)
            {
                DegreeRate = 360 / ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.Count;
                var Uindex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.FindIndex(x => x.IsUsing);
                if (Uindex != -1)
                    SeparateBox_RotateAngle = -DegreeRate * Uindex;
            }


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















        private bool _cylinder_1_Up_ReedSwitchIsTriggered = false;
        /// <summary>
        /// 氣壓缸1上方磁簧
        /// </summary>
        public bool Cylinder_1_Up_ReedSwitchIsTriggered
        {
            get => _cylinder_1_Up_ReedSwitchIsTriggered;
            set { _cylinder_1_Up_ReedSwitchIsTriggered = value; OnPropertyChanged(); }
        }
        private bool _cylinder_1_Middle_ReedSwitchIsTriggered = false;
        /// <summary>
        /// 氣壓缸1中間磁簧
        /// </summary>
        public bool Cylinder_1_Middle_ReedSwitchIsTriggered
        {
            get => _cylinder_1_Middle_ReedSwitchIsTriggered;
            set { _cylinder_1_Middle_ReedSwitchIsTriggered = value; OnPropertyChanged(); }
        }
        private bool _cylinder_1_Down_ReedSwitchIsTriggered = false;
        /// <summary>
        /// 氣壓缸1下方磁簧
        /// </summary>
        public bool Cylinder_1_Down_ReedSwitchIsTriggered
        {
            get => _cylinder_1_Down_ReedSwitchIsTriggered;
            set { _cylinder_1_Down_ReedSwitchIsTriggered = value; OnPropertyChanged(); }
        }

        private bool _cylinder_1_isUp;
        /// <summary>
        /// 氣壓缸1上io觸發
        /// </summary>
        public bool Cylinder_1_isUp
        {
            get => _cylinder_1_isUp;
            set { _cylinder_1_isUp = value; OnPropertyChanged(); }
        }

        private bool _cylinder_1_isMiddle;
        /// <summary>
        /// 氣壓缸1上io觸發
        /// </summary>
        public bool Cylinder_1_isMiddle
        {
            get => _cylinder_1_isMiddle;
            set { _cylinder_1_isMiddle = value; OnPropertyChanged(); }
        }
        private bool _cylinder_1_isDown;
        /// <summary>
        /// 氣壓缸1上io觸發
        /// </summary>
        public bool Cylinder_1_isDown
        {
            get => _cylinder_1_isDown;
            set { _cylinder_1_isDown = value; OnPropertyChanged(); }
        }

        private bool _cylinder_1_Up_IsEnabled = true;
        public bool Cylinder_1_Up_IsEnabled
        {
            get => _cylinder_1_Up_IsEnabled;
            set { _cylinder_1_Up_IsEnabled = value; OnPropertyChanged(); }
        }
        private bool _cylinder_1_Middle_IsEnabled = true;
        public bool Cylinder_1_Middle_IsEnabled
        {
            get => _cylinder_1_Middle_IsEnabled;
            set { _cylinder_1_Middle_IsEnabled = value; OnPropertyChanged(); }
        }

        private bool _cylinder_1_Down_IsEnabled = true;
        public bool Cylinder_1_Down_IsEnabled
        {
            get => _cylinder_1_Down_IsEnabled;
            set { _cylinder_1_Down_IsEnabled = value; OnPropertyChanged(); }
        }




        
        



        /// <summary>
        /// 氣壓缸1上升
        /// </summary>
        public ICommand Cylinder_1_Up_Command
        {
            get => new RelayCommand(() =>
            {
                Cylinder_1_Up_IsEnabled = false;

                Cylinder_1_Up_ReedSwitchIsTriggered = false;
                Cylinder_1_Middle_ReedSwitchIsTriggered = false;
                Cylinder_1_Down_ReedSwitchIsTriggered = false;
                Task.Run(() =>
                {
                    Task.Delay(500).Wait();
                    Cylinder_1_Up_IsEnabled = true;
                    Cylinder_1_Up_ReedSwitchIsTriggered = true;
                });
            });
        }
        public ICommand Cylinder_1_Mid_Command
        {
            get => new RelayCommand(() =>
            {
                Cylinder_1_Middle_IsEnabled = false;

                Cylinder_1_Up_ReedSwitchIsTriggered = false;
                Cylinder_1_Middle_ReedSwitchIsTriggered = false;
                Cylinder_1_Down_ReedSwitchIsTriggered = false;
                Task.Run(() =>
                {
                    Task.Delay(500).Wait();
                    Cylinder_1_Middle_IsEnabled = true;
                    Cylinder_1_Middle_ReedSwitchIsTriggered = true;
                });
            });
        }
        public ICommand Cylinder_1_Down_Command
        {
            get => new RelayCommand(() =>
            {
                Cylinder_1_Down_IsEnabled = false;

                Cylinder_1_Up_ReedSwitchIsTriggered = false;
                Cylinder_1_Middle_ReedSwitchIsTriggered = false;
                Cylinder_1_Down_ReedSwitchIsTriggered = false;
                Task.Run(() =>
                {
                    Task.Delay(500).Wait();
                    Cylinder_1_Down_IsEnabled = true;
                    Cylinder_1_Down_ReedSwitchIsTriggered = true;
                });

            });
        }




        public ICommand SeparateBox_ClockwiseRotateCommand
        {
            get => new RelayCommand(() =>
            {
                try
                {
                    SeparateBox_Rotate(1);
                }
                catch (Exception ex)
                {

                }

            });
        }

        public ICommand SeparateBox_CounterClockwiseRotateCommand
        {
            get => new RelayCommand(() =>
            {
                try
                {
                    SeparateBox_Rotate(-1);
                }
                catch (Exception ex)
                {

                }

            });
        }



        object SeparateBox_Rotatelock = new();

        private bool _isRotating = false;
        public bool IsRotating
        {
            get => _isRotating; set { _isRotating = value; OnPropertyChanged(); } 
        }


        private void SeparateBox_Rotate(int step)
        {
            var MinIndex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.ToList().FindIndex(x => x.BoxIsEnabled);
            var Maxindex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.ToList().FindLastIndex(x => x.BoxIsEnabled);


            var IsUsingindex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.FindIndex(x => x.IsUsing);
            if (IsUsingindex == -1)
            {
                if(step>0)
                {
                    IsUsingindex = MinIndex;
                }
                else  if (step <= 0)
                {
                    IsUsingindex = Maxindex;
                }
                ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection[IsUsingindex].IsUsing = true;
                return;
            }
            else
                ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection[IsUsingindex].IsUsing = false;

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

            if (IsUsingindex != -1)
            {
                ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection[IsUsingindex].IsUsing = true;
                //取得

                IsRotating = false;
               
                Task.Run(async () =>
                {
                    IsRotating = true;

                    //角度比例
                    var DegreeRate = 360 / ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.Count;
                    //目標

                    //先取得目前的位置
                    var tempRotate = SeparateBox_RotateAngle;
                    //檢查正反轉
                    var endRotatePoint = 360 - DegreeRate * IsUsingindex;
                    while (true)
                    {
                        if (step > 0)
                        {
                            SeparateBox_RotateAngle -= 1;
                        }
                        else
                        {
                            SeparateBox_RotateAngle +=1;
                        }

                        if (Math.Abs(SeparateBox_RotateAngle - endRotatePoint) < 2 || Math.Abs(SeparateBox_RotateAngle - endRotatePoint) > 360)
                            break;

                        if (!IsRotating)
                            break;

                        await Task.Delay(1);
                    }

                    if (IsRotating)
                    {
                        SeparateBox_RotateAngle = endRotatePoint;
                        IsRotating = false;
                    }
                });
            }
        }

        private double _separateBox_RotateAngle = 0;
        public double SeparateBox_RotateAngle
        { 
            get => _separateBox_RotateAngle; 
            set 
            { _separateBox_RotateAngle = value; OnPropertyChanged(); 
            } 
        }
















        #region 機台移動命令

        private double _steelBeltLength = 450;

        public double SteelBeltLength
        {
            get => _steelBeltLength;
            set { _steelBeltLength = value; OnPropertyChanged(); }
        }

        private double _stampingProductWidth = 100;
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

        private double _stampingFontHeight= 10;
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
            get => new RelayParameterizedCommand(para =>
            {
                bool _scheduler_FontStamping = false;
                bool _scheduler_QRStamping = false;
                bool _scheduler_Shearing = true;

                GD_StampingMachine.ViewModels.ParameterSetting.SettingBaseViewModel SettingBaseVM = new NumberSettingViewModel()
                {
                    SequenceCount = 0
                };


                if (para != null)
                {
                    if (para is GD_StampingMachine.ViewModels.ParameterSetting.SettingBaseViewModel ParaSettingBaseVM)
                    {
                        _scheduler_FontStamping = true;


                        SettingBaseVM = ParaSettingBaseVM;
                        if (SettingBaseVM is QRSettingViewModel)
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
                    if(AbsoluteMaxSMC.AbsoluteMoveDistance>=0)
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
            get => new RelayParameterizedCommand(obj =>
            {
                if(obj is GridControl ProjectGridControl)
                {
                   if (ProjectGridControl.ItemsSource is IList<GD_StampingMachine.ViewModels.SendMachineCommandViewModel> IEnumerableData)
                    {
                        if (ProjectGridControl.SelectedItems != null)
                        {
                            for (int i = ProjectGridControl.SelectedItems.Count - 1; i >= 0; i--)
                            {
                                var rIndex = IEnumerableData.FindIndex(x => x == ProjectGridControl.SelectedItems[i]);
                                if(rIndex !=-1)
                                    IEnumerableData.RemoveAt(rIndex);
                            }

                        }               
                    }

                }
            });
        }

        public ICommand SoftSendMachineCommand
        {
            get => new RelayCommand(() =>
            {

                SendMachineCommandVMObservableCollection = new ObservableCollection<SendMachineCommandViewModel>
                (SendMachineCommandVMObservableCollection.OrderBy(x=>x.AbsoluteMoveDistance));


                //先依照功能算出個別的鋼片距離的Relative distance 後 
                //按照大小重新排序 每做一次加工就排序一次?
                //預先算出所有加工進行?


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
              var  StampingPlateProcessingSequenceViewModelList = new List<StampingPlateProcessingSequenceViewModel>();

                foreach (var smc in smcCollection)
                {
                    //有QR加工需求
                    if(smc.WorkScheduler_QRStamping)
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
                                            ProcessingAbsoluteDistance = smc.AbsoluteMoveDistance -  Fonts_Stamping_Distance
                                        });
                                        break;

                                    case VerticalAlignEnum.Center:
                                        StampingPlateProcessingSequenceViewModelList.Add(new StampingPlateProcessingSequenceViewModel()
                                        {
                                            SteelBeltStampingStatus = SteelBeltStampingStatusEnum.Stamping,
                                            SendMachineCommandVM = smc,
                                            ProcessingAbsoluteDistance = smc.AbsoluteMoveDistance - (Fonts_Stamping_Distance - StampingFontHeight/2)
                                        });
                                        break;

                                    case VerticalAlignEnum.Bottom:
                                        StampingPlateProcessingSequenceViewModelList.Add(new StampingPlateProcessingSequenceViewModel()
                                        {
                                            SteelBeltStampingStatus = SteelBeltStampingStatusEnum.Stamping,
                                            SendMachineCommandVM = smc,
                                            ProcessingAbsoluteDistance = smc.AbsoluteMoveDistance - ( Fonts_Stamping_Distance - StampingFontHeight)
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
                                    ProcessingAbsoluteDistance = smc.AbsoluteMoveDistance -( Fonts_Stamping_Distance)
                                });

                                StampingPlateProcessingSequenceViewModelList.Add(new StampingPlateProcessingSequenceViewModel()
                                {
                                    SteelBeltStampingStatus = SteelBeltStampingStatusEnum.Stamping,
                                    SendMachineCommandVM = smc,
                                    ProcessingAbsoluteDistance = smc.AbsoluteMoveDistance - ( Fonts_Stamping_Distance - StampingFontHeight)
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

                if(StampingPlateProcessingSequenceViewModelList.Count>0)
                    StampingPlateProcessingSequenceViewModelList[0].ProcessingRelativeDistance = StampingPlateProcessingSequenceViewModelList[0].ProcessingAbsoluteDistance;
                for ( int i= 1;i< StampingPlateProcessingSequenceViewModelList.Count;i++ )
                {
                    StampingPlateProcessingSequenceViewModelList[i].ProcessingRelativeDistance = StampingPlateProcessingSequenceViewModelList[i].ProcessingAbsoluteDistance - StampingPlateProcessingSequenceViewModelList[i - 1].ProcessingAbsoluteDistance;
                }


                StampingPlateProcessingSequenceVMObservableCollection = StampingPlateProcessingSequenceViewModelList.ToObservableCollection();
            });
        }


        private ObservableCollection<StampingPlateProcessingSequenceViewModel> _stampingPlateProcessingSequenceVMObservableCollection;

        /// <summary>
        /// 沖壓加工陣列
        /// </summary>
        public ObservableCollection<StampingPlateProcessingSequenceViewModel> StampingPlateProcessingSequenceVMObservableCollection
        {
            get => _stampingPlateProcessingSequenceVMObservableCollection ??= new ObservableCollection<StampingPlateProcessingSequenceViewModel>();
            set { _stampingPlateProcessingSequenceVMObservableCollection = value;OnPropertyChanged(); }
        }












        //需輸入三種數字 代表三種機器距離切割線的位置 並以切割線為基準算出所有鋼片的絕對座標
        //用一按鍵重整數值

        public ICommand X_axisMoveSendMachineCommand
        {
            get => new RelayParameterizedCommand(para =>
            {
                double MoveStep = 1;
                if(para is double doublepara)
                {
                    MoveStep = doublepara;
                }

                foreach(var smc in SendMachineCommandVMObservableCollection)
                {
                    smc.AbsoluteMoveDistance += MoveStep;
                    
                }

                foreach (var spp in StampingPlateProcessingSequenceVMObservableCollection)
                {
                    spp.ProcessingAbsoluteDistance += MoveStep;
                }

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

                foreach(var SMC in SendMachineCommandVMObservableCollection)
                {
                    //SMC.AbsoluteMoveDistance 
                }







                BeltIsMoving = false;
            });
        }

        private bool _isWorking = false;
        public bool IsWorking { get => _isWorking; set { _isWorking = value; OnPropertyChanged(); } }


      
        public ICommand AutoWorkCommand
        {
            get => new RelayCommand(() =>
            {
                Task.Run(async() =>
                {
                    IsWorking = true;

                    var FindPredicate = new Predicate<StampingPlateProcessingSequenceViewModel>(x => x.ProcessingAbsoluteDistance >= 0 && !x.IsFinish);

                    //  while (StampingPlateProcessingSequenceVMObservableCollection.FindIndex(x => x.ProcessingAbsoluteDistance >= 0 && !x.IsFinish) != -1)
                    while (StampingPlateProcessingSequenceVMObservableCollection.FindIndex(FindPredicate) != -1)
                    {
                        //把最上面ProcessingAbsoluteDistance大於0的鐵片移動到指定位置
                        var FIndex = StampingPlateProcessingSequenceVMObservableCollection.FindIndex(FindPredicate);

                        var moveDistance = StampingPlateProcessingSequenceVMObservableCollection[FIndex].ProcessingAbsoluteDistance;
                        double moveStep = 0.5;

                        while (StampingPlateProcessingSequenceVMObservableCollection[FIndex].ProcessingAbsoluteDistance >= moveStep)
                        {
                            //StampingPlateProcessingSequenceVMObservableCollection[FIndex].ProcessingAbsoluteDistance-= moveStep;
                            //須重構成function 與移動指令共用
                            foreach (var item in StampingPlateProcessingSequenceVMObservableCollection)
                            {
                                item.ProcessingAbsoluteDistance -= moveStep;

                            }
                            foreach (var item in SendMachineCommandVMObservableCollection)
                            {
                                item.AbsoluteMoveDistance -= moveStep;
                            }

                            await Task.Delay(10);
                        }

                        //最後一階
                        var LastMove = StampingPlateProcessingSequenceVMObservableCollection[FIndex].ProcessingAbsoluteDistance;
                        foreach (var item in StampingPlateProcessingSequenceVMObservableCollection)
                        {
                            item.ProcessingAbsoluteDistance -= LastMove;
                        }
                        foreach (var item in SendMachineCommandVMObservableCollection)
                        {
                            item.AbsoluteMoveDistance -= LastMove;
                        }
                        StampingPlateProcessingSequenceVMObservableCollection[FIndex].ProcessingAbsoluteDistance = 0;

                        //每個工作到位置後 等待三秒

                        if (false)
                        {
                            //單動
                            await Task.Delay(3000);
                            StampingPlateProcessingSequenceVMObservableCollection[FIndex].IsFinish = true;
                            StampingPlateProcessingSequenceVMObservableCollection[FIndex].SendMachineCommandVM.SteelBeltStampingStatus = StampingPlateProcessingSequenceVMObservableCollection[FIndex].SteelBeltStampingStatus;
                        }
                        else
                        {
                            //所有在指定位置上的都可以加工
                            var ReadyWorkList = StampingPlateProcessingSequenceVMObservableCollection.ToList().FindAll(x => x.ProcessingAbsoluteDistance == 0 && !x.IsFinish);

                            List<Task> AllworkTask = new List<Task>();

                            foreach(var work in ReadyWorkList)
                            {
                                Task WrokTask = Task.Run(async() =>
                                {
                                    //把該鋼片設為正在加工
                                    //在這邊加入進度條
                                    work.SendMachineCommandVM.IsWorking = true;
                                    //各種工作所花時間不同
                                    switch (work.SteelBeltStampingStatus)
                                    {
                                        case SteelBeltStampingStatusEnum.QRCarving:
                                            //send command 

                                            await Task.Delay(1000);
                                            //wait complete
                                            break;
                                        case SteelBeltStampingStatusEnum.Stamping:

                                            await Task.Delay(3000);

                                            break;
                                        case SteelBeltStampingStatusEnum.Shearing:

                                            await Task.Delay(100);
                                            break;
                                        default:
                                        case SteelBeltStampingStatusEnum.None:
                                            await Task.Delay(10);
                                            break;
                                    }

                                    work.IsFinish = true;
                                    work.SendMachineCommandVM.IsWorking = false;
                                    work.SendMachineCommandVM.SteelBeltStampingStatus = work.SteelBeltStampingStatus;
                                    // await Task.Delay(3000);
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

                });

            });
        }

        public ICommand Recover
        {
            get => new RelayCommand(() =>
            {
            
                var returnOriginValue = SendMachineCommandVMObservableCollection.Min(x => x.AbsoluteMoveDistance);
                returnOriginValue = returnOriginValue - QR_Stamping_Distance- Fonts_Stamping_Distance ;

                foreach (var item in StampingPlateProcessingSequenceVMObservableCollection)
                {
                    item.ProcessingAbsoluteDistance -= returnOriginValue;
                    item.IsFinish = false;// true;
                    //item.SteelBeltStampingStatus
                }
                foreach (var item in SendMachineCommandVMObservableCollection)
                {
                    item.AbsoluteMoveDistance -= returnOriginValue;
                    item.SteelBeltStampingStatus = SteelBeltStampingStatusEnum.None;
                }



            });
        }




            #endregion










        }

    public class StampingPlateProcessingSequenceViewModel: BaseViewModel
    {
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


        private bool _isFinish = false;
        /// <summary>
        /// 標記為完成
        /// </summary>
        public bool IsFinish { get => _isFinish; set { _isFinish = value; OnPropertyChanged(); } }





    }

}
