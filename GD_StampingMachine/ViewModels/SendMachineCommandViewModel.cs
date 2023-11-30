using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Singletons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    /// <summary>
    /// 傳送給機台使用的資料
    /// </summary>
    public class SendMachineCommandViewModel :BaseViewModel
    {
        public override string ViewModelName => "SendMachineCommandViewModel";

        public SendMachineCommandViewModel()
        {

        }

        public SendMachineCommandViewModel(SendMachineCommandModel sendMachineCommand)
        {
            SendMachineCommand = sendMachineCommand;
            if (AbsoluteMoveDistance != 0)
                AbsoluteMoveDistanceAnimation = AbsoluteMoveDistance;
        }

        [JsonIgnore]
        public readonly SendMachineCommandModel SendMachineCommand = new();


        /// <summary>
        /// 已完成加工
        /// </summary>
        public SteelBeltStampingStatusEnum SteelBeltStampingStatus
        {
            get=> SendMachineCommand.SteelBeltStampingStatus; 
            set 
            {
                SendMachineCommand.SteelBeltStampingStatus = value;
                OnPropertyChanged(); 
            } 
        }



        private CancellationTokenSource cts = new CancellationTokenSource();
        /// <summary>
        /// 工作需要移動的絕對距離(目前位置離加工位置多遠)
        /// </summary>
        public double AbsoluteMoveDistance
        {
            get => SendMachineCommand.AbsoluteMoveDistance;
            set
            {
                SendMachineCommand.AbsoluteMoveDistance = value;
                OnPropertyChanged();

                cts?.Cancel();
                cts = new CancellationTokenSource();
                _ = Task.Run(async () =>
                {
                    try
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
                                AbsoluteMoveDistanceAnimation = AbsoluteMoveDistanceAnimation + PercentDiff;
                                await Task.Delay(1);
                                //cts.Token.ThrowIfCancellationRequested();
                                if (cts.Token.IsCancellationRequested)
                                    break;
                            }
                        }

                    }
                    catch (OperationCanceledException)
                    {

                    }
                    catch (Exception ex)
                    {
                        await LogDataSingleton.Instance.AddLogDataAsync(ViewModelName, ex.Message);
                    }
                    finally
                    {
                        AbsoluteMoveDistanceAnimation = value;
                    }
                });
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
            get => _absoluteMoveDistanceAnimation ;
            private set{_absoluteMoveDistanceAnimation = value; OnPropertyChanged();}
        }







        public int WorkNumber { get => SendMachineCommand.WorkNumber; set { SendMachineCommand.WorkNumber = value; OnPropertyChanged(); } }

        private GD_StampingMachine.ViewModels.ParameterSetting.SettingBaseViewModel _settingBaseVM;
        /// <summary>
        /// 鐵牌
        /// </summary>
        public GD_StampingMachine.ViewModels.ParameterSetting.SettingBaseViewModel SettingBaseVM
        {
            get=>_settingBaseVM;             
            set
            {
                _settingBaseVM = value;
                if (value != null)
                    SendMachineCommand.StampPlateSetting = value.StampPlateSetting;
                OnPropertyChanged();
            }
        }

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

        /// <summary>
        /// 已傳送 有此標記者會被略過
        /// </summary>
        public bool IsSended
        {
            get => SendMachineCommand.IsSended; set { SendMachineCommand.IsSended = value; OnPropertyChanged(); }
        }

        //private bool _isWorking = false;
        /// <summary>
        /// 標記為正在加工
        /// </summary>
        public bool IsWorking { get => SendMachineCommand.IsWorking; set { SendMachineCommand.IsWorking = value; OnPropertyChanged(); } }



        //private int _workIndex = false;
        /// <summary>
        /// 加工順序標記
        /// </summary>
        public int WorkIndex { get => SendMachineCommand.WorkIndex; set { SendMachineCommand.WorkIndex = value; OnPropertyChanged(); } }




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
        /// 標記為完成
        /// </summary>
        public double WorkingProgress { get => _workingProgress; set { _workingProgress = value; OnPropertyChanged(); } }





    }


}
