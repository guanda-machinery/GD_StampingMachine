
using DevExpress.Office.Forms;
using DevExpress.Utils.StructuredStorage.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Interfaces;
using GD_StampingMachine.Method;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.GD_Model.ProductionSetting;
using GD_StampingMachine.Properties;
using GD_StampingMachine.ViewModels.ParameterSetting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using GD_StampingMachine.Model;
using GD_CommonLibrary.Extensions;
using Microsoft.Xaml.Behaviors;
using DevExpress.CodeParser;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    /// <summary>
    /// ABC參數加工型
    /// </summary>
    public partial class PartsParameterViewModel : GD_CommonLibrary.BaseViewModel
    {
        [JsonIgnore]
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_PartsParameterViewModel");

        public PartsParameterViewModel()
        { 

        }
       public PartsParameterViewModel(PartsParameterModel PParameter)
       {
            PartsParameter = PParameter;
        }



        private PartsParameterModel _partsParameter;
        public PartsParameterModel PartsParameter 
        {
            get => _partsParameter ??= new PartsParameterModel();
            private set => _partsParameter = value;
        }

        /// <summary>
        /// 加工進程
        /// </summary>
        public float FinishProgress
        {
            get => PartsParameter.Processing;
            set
            {
                PartsParameter.Processing = value;
                OnPropertyChanged(nameof(FinishProgress));
            }
        }

        /// <summary>
        /// 加工專案名
        /// </summary>
        public string DistributeName
        {
            get => PartsParameter.DistributeName;
            set
            {
                PartsParameter.DistributeName = value; OnPropertyChanged();
            }
        }
        /// <summary>
        /// 專案名
        /// </summary>
        public string ProjectID
        {
            get => PartsParameter.ProjectID;
            set
            {
                PartsParameter.ProjectID = value; OnPropertyChanged();
            }
        }


        public string ParameterA
        {
            get => PartsParameter.ParamA;
            set
            {
                PartsParameter.ParamA = value;
                OnPropertyChanged(nameof(ParameterA));
                OnPropertyChanged(nameof(SettingBaseVM));
            }
        }
        public string ParameterB
        {
            get => PartsParameter.ParamB;
            set
            {
                PartsParameter.ParamB = value;
                OnPropertyChanged(nameof(ParameterB));
            }
        }
        public string ParameterC
        {
            get => PartsParameter.ParamC;
            set
            {
                PartsParameter.ParamC = value;
                OnPropertyChanged(nameof(ParameterC));
            }
        }

        public MachiningStatusEnum MachiningStatus
        {
            get => PartsParameter.MachiningStatus;
            set
            {
                PartsParameter.MachiningStatus = value;
                OnPropertyChanged(nameof(MachiningStatus));
            }
        }


        /// <summary>
        /// (加工)盒子編號
        /// </summary>
        public int? BoxIndex
        {
            get => PartsParameter.BoxIndex;
            set
            {
                PartsParameter.BoxIndex = value;
                OnPropertyChanged();
            }
        }

        private SettingBaseViewModel _settingBaseVM;//= new NumberSettingViewModel();
        /// <summary>
        /// 金屬牌樣式
        /// </summary>
        public SettingBaseViewModel SettingBaseVM
        {
            get
            {
                 if (_settingBaseVM == null)
                 {
                     if (PartsParameter.StampingPlate.SheetStampingTypeForm == SheetStampingTypeFormEnum.qrcode)
                         _settingBaseVM = new QRSettingViewModel(PartsParameter.StampingPlate);
                     else
                         _settingBaseVM = new NumberSettingViewModel(PartsParameter.StampingPlate);
                 }
                _settingBaseVM.PlateNumber = ParameterA;
                 return _settingBaseVM;
            }
            set
            {
                 _settingBaseVM = value;
                 if(value != null)
                     PartsParameter.StampingPlate = value.StampPlateSetting;
                OnPropertyChanged();
            }
        }
        
                                           

        public bool _editPartDarggableIsPopup;
        /// <summary>
        /// 編輯視窗
        /// </summary>
        public bool EditPartDarggableIsPopup
        {
            get => _editPartDarggableIsPopup;
            set  { _editPartDarggableIsPopup = value;OnPropertyChanged();
            }
        }


        [JsonIgnore]
        public RelayCommand ProjectEditCommand
        {
            get => new(() =>
            {
                EditPartDarggableIsPopup = true;
            });
        }

        [JsonIgnore]
        public RelayParameterizedCommand ProjectDeleteCommand
        {
            get => new (async obj =>
            {
                if (obj is GridControl ObjGridControl)
                {
                    if (ObjGridControl.ItemsSource is ObservableCollection<PartsParameterViewModel> GridItemSource)
                    {
                        if (SettingBaseVM != null)
                        {
                            if (MethodWinUIMessageBox.AskDelProject(this.SettingBaseVM.NumberSettingMode))
                            {
                                GridItemSource.Remove(this);
                            }
                        }
                        else
                            GridItemSource.Remove(this);

                    }
                }
            });
        }
    }


    public partial class PartsParameterViewModel
    {

        [JsonIgnore]
        public readonly SendMachineCommandModel SendMachineCommand = new();

        /// <summary>
        /// 已完成加工
        /// </summary>
        public SteelBeltStampingStatusEnum SteelBeltStampingStatus
        {
            get => SendMachineCommand.SteelBeltStampingStatus;
            set
            {
                SendMachineCommand.SteelBeltStampingStatus = value;
                OnPropertyChanged();
            }
        }




        private CancellationTokenSource cts = new CancellationTokenSource();
        private Task MoveTask;

        /// <summary>
        /// 工作需要移動的絕對距離(目前位置離加工位置多遠)
        /// </summary>
        public double AbsoluteMoveDistance
        {
            get => SendMachineCommand.AbsoluteMoveDistance;
            set
            {
                SendMachineCommand.AbsoluteMoveDistance = value;
                Task.Run(async () =>
                {
                    if (cts != null)
                        cts.Cancel();

                    if (MoveTask != null)
                        await MoveTask;

                    cts = new CancellationTokenSource();
                    MoveTask = Task.Run(async () =>
                    {
                        if (AbsoluteMoveDistanceAnimation.HasValue)
                        {
                            var absAnimationValue = AbsoluteMoveDistanceAnimation.Value;
                            var Diff = value - absAnimationValue;
                            var AbsDiff = Math.Abs(Diff);
                            double MoveDiff = 10;
                            if (AbsDiff > 10)
                            {
                                MoveDiff = 25;
                            }
                            else if (AbsDiff > 10)
                            {
                                MoveDiff = 10;
                            }
                            else if (AbsDiff >= 5)
                            {
                                MoveDiff = 5;
                            }
                            else if (AbsDiff >= 0)
                            {
                                MoveDiff = 1;
                            }

                            var PercentDiff = Diff / MoveDiff;
                            for (double i = 0; i < Math.Abs(Diff); i += Math.Abs(PercentDiff))
                            {
                                if (!AbsoluteMoveDistanceAnimation.HasValue)
                                    AbsoluteMoveDistanceAnimation = 0;
                                AbsoluteMoveDistanceAnimation += PercentDiff;
                                await Task.Delay(0);
                                if (cts.IsCancellationRequested)
                                {
                                    break;
                                }
                            }
                        }
                        AbsoluteMoveDistanceAnimation = value;
                    }, cts.Token);
                });


                OnPropertyChanged();
            }
        }








        public double RelativeMoveDistance
        {
            get => SendMachineCommand.RelativeMoveDistance;
            set
            {
                SendMachineCommand.RelativeMoveDistance = value;
                OnPropertyChanged();
            }
        }

        private double? _absoluteMoveDistanceAnimation;
        public double? AbsoluteMoveDistanceAnimation
        {
            get => _absoluteMoveDistanceAnimation;
            private set { _absoluteMoveDistanceAnimation = value; OnPropertyChanged(); }
        }

        public int WorkNumber { get => SendMachineCommand.WorkNumber; set { SendMachineCommand.WorkNumber = value; OnPropertyChanged(); } }

        public double StampWidth
        {
            get => SendMachineCommand.StampWidth;
            set
            {
                SendMachineCommand.StampWidth = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// QR加工 true會進行加工
        /// </summary>
        public bool WorkScheduler_QRStamping
        {
            get => SendMachineCommand.WorkScheduler_QRStamping; set { SendMachineCommand.WorkScheduler_QRStamping = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 鋼印加工 true會進行加工
        /// </summary>
        public bool WorkScheduler_FontStamping
        {

            get => SendMachineCommand.WorkScheduler_FontStamping; set { SendMachineCommand.WorkScheduler_FontStamping = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 剪斷 true會進行加工
        /// </summary>
        public bool WorkScheduler_Shearing
        {
            get => SendMachineCommand.WorkScheduler_Shearing; set { SendMachineCommand.WorkScheduler_Shearing = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 已完成 有此標記者會被略過
        /// </summary>
        public bool IsFinish
        {
            get => SendMachineCommand.IsFinish; set { SendMachineCommand.IsFinish = value; OnPropertyChanged(); }
        }


        private bool _isWorking = false;
        /// <summary>
        /// 標記為正在加工
        /// </summary>
        public bool IsWorking { get => _isWorking; set { _isWorking = value; OnPropertyChanged(); } }




        private SteelBeltStampingStatusEnum _workingSteelBeltStampingStatus;
        /// <summary>
        /// 正在加工的種類(渲染用)
        /// </summary>
        public SteelBeltStampingStatusEnum WorkingSteelBeltStampingStatus
        {
            get => _workingSteelBeltStampingStatus;
            set
            {
                _workingSteelBeltStampingStatus = value;
                OnPropertyChanged();
            }
        }

        private double _workingProgress = 0;
        /// <summary>
        /// 正在加工的圖標
        /// </summary>
        public double WorkingProgress { get => _workingProgress; set { _workingProgress = value; OnPropertyChanged(); } }





    }







}
