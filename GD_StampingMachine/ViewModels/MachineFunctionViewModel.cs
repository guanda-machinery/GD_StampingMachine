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
using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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



        public ParameterSettingViewModel ParameterSettingVM { get => Singletons.StampingMachineSingleton.Instance.ParameterSettingVM; }
        public StampingFontChangedViewModel StampingFontChangedVM { get => Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM; }

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
                if (step > 0)
                {
                    IsUsingindex = MinIndex;
                }
                else if (step <= 0)
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
                            SeparateBox_RotateAngle += 1;
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
            {
                _separateBox_RotateAngle = value; OnPropertyChanged();
            }
        }


    }
}
