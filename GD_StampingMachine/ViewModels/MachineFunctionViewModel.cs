using DevExpress.Data.Extensions;
using DevExpress.Utils.StructuredStorage.Internal;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
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
        public double StampingProductWidth
        {
            get => _stampingProductWidth;
            set { _stampingProductWidth = value; OnPropertyChanged(); }
        }


        /*private double _steelBeltWidth = 100;

        public double SteelBeltWidth
        {
            get => _steelBeltWidth;
            set { _steelBeltWidth = value; OnPropertyChanged(); }
        }*/





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
            set => _sendMachineCommandVMObservableCollection = value;
        }

        public ICommand SendMachineCommand
        {
            get => new RelayParameterizedCommand(para =>
            {
                if (para != null)
                {
                    if (para is GD_StampingMachine.ViewModels.ParameterSetting.SettingBaseViewModel SettingBaseVM)
                    {
                        SendMachineCommandVMObservableCollection.Add(new SendMachineCommandViewModel()
                        {
                            SteelBeltStampingStatus = SteelBeltStampingStatusEnum.None,
                            RelativeMoveDistance = double.PositiveInfinity,
                            AbsoluteMoveDistance = double.PositiveInfinity,
                            SettingBaseVM = SettingBaseVM,
                            //尺寸
                            StampWidth = 35

                        }); ;
                    }
                }
            });
        }
        public ICommand SoftSendMachineCommand
        {
            get => new RelayCommand(() =>
            {

                //SendMachineCommandVMObservableCollection.


                //先依照功能算出個別的鋼片距離的Relative distance 後 
                //按照大小重新排序 每做一次加工就排序一次?
                //預先算出所有加工進行?


            });
        }
        //需輸入三種數字 代表三種機器距離切割線的位置 並以切割線為基準算出所有鋼片的絕對座標
        //用一按鍵重整數值

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


        #endregion







    }
}
