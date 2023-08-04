using GD_CommonLibrary;
using GD_StampingMachine.GD_Model;
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

        /// <summary>
        /// QR資訊
        /// </summary>
        public ObservableCollection<StampingPlateSettingModel> QRSettingModelCollection { get; set; }


        private StampingPlateSettingModel _selectedQRSettingModelCollection;
        public StampingPlateSettingModel SelectedQRSettingModelCollection
        {
            get => _selectedQRSettingModelCollection;
            set
            {
                _selectedQRSettingModelCollection = value;
            }
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

        public bool _cylinder_1_Up_IsEnabled = true;
        public bool Cylinder_1_Up_IsEnabled
        {
            get => _cylinder_1_Up_IsEnabled;
            set { _cylinder_1_Up_IsEnabled = value; OnPropertyChanged(); }
        }
        public bool _cylinder_1_Middle_IsEnabled = true;
        public bool Cylinder_1_Middle_IsEnabled
        {
            get => _cylinder_1_Middle_IsEnabled;
            set { _cylinder_1_Middle_IsEnabled = value; OnPropertyChanged(); }
        }

        public bool _cylinder_1_Down_IsEnabled = true;
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










    }
}
