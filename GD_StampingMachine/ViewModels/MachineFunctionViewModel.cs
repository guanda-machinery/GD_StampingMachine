using DevExpress.Data.Extensions;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class MachineFunctionViewModel : BaseViewModelWithLog
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("btnDescription_MachineFunction");




        public ParameterSettingViewModel ParameterSettingVM { get; set; } = new();



        public StampingFontChangedViewModel StampingFontChangedVM { get; set; } = new();



        /// <summary>
        /// QR資訊
        /// </summary>
        /*public ObservableCollection<StampingPlateSettingModel> QRSettingModelCollection { get; set; }

        private StampingPlateSettingModel _selectedQRSettingModelCollection;
        public StampingPlateSettingModel SelectedQRSettingModelCollection
        {
            get => _selectedQRSettingModelCollection;
            set
            {
                _selectedQRSettingModelCollection = value;
                OnPropertyChanged();
            }
        }*/


        /*private StampingTypeViewModel _stampingTypeModel_readyStamping;
        /// <summary>
        /// 轉盤資訊
        /// </summary>
        public StampingTypeViewModel StampingTypeModel_ReadyStamping
        {
            get => _stampingTypeModel_readyStamping;
            set { _stampingTypeModel_readyStamping = value; OnPropertyChanged(); }
        }*/

        /*private ObservableCollection<StampingTypeViewModel> _stampingTypeVMObservableCollection;
        public ObservableCollection<StampingTypeViewModel> StampingTypeVMObservableCollection
        {

            get => _stampingTypeVMObservableCollection;
            set { _stampingTypeVMObservableCollection = value; OnPropertyChanged(); }
        }
        */












     /*   private bool _manualOperatingMode = false;

       public bool ManualOperatingMode
        {
            get => _manualOperatingMode;
            set 
            {
                Feeding_Component_Button_IsChecked = true;
                Stamping_Component_Button_IsChecked = true;
                ShearCut_Component_Button_IsChecked = true;
                Separator_Component_Button_IsChecked = true;
                _manualOperatingMode = value;
                OnPropertyChanged();
            }
        }*/


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
                    SeparateBox_Rotate(-1);
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
                    SeparateBox_Rotate(1);
                }
                catch (Exception ex)
                {

                }

            });
        }

        private void SeparateBox_Rotate(int step)
        {
            var MinIndex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.ToList().FindIndex(x => x.BoxIsEnabled);
            var Maxindex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.ToList().FindLastIndex(x => x.BoxIsEnabled);


            var IsUsingindex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.FindIndex(x => x.IsUsing);
            if (IsUsingindex == -1)
            {
                //ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection[IsUsingindex].IsUsing = false;
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
                //IsUsingindex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.Count-1;
                IsUsingindex = Maxindex;
            }

            if (IsUsingindex > Maxindex)
            {
                //IsUsingindex = 0;
                IsUsingindex = MinIndex;
                //最小的可用index
            }

            if(IsUsingindex!=-1)
                ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection[IsUsingindex].IsUsing = true;


        }











    }
}
