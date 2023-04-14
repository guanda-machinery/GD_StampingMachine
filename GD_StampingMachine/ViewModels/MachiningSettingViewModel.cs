using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model.ProductionSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class MachiningSettingViewModel:ViewModelBase
    {
        public MachiningSettingViewModel()
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

            MachiningPartsVMObservableCollection = new();
            MachiningPartsVMObservableCollection.Add(new PartsParameterViewModel(new PartsParameterModel()
            {
                BoxNumber = null,
                ProjectName="測試專案",
                Parametert_A = "testA",
                Parametert_B = "testB",
                Parametert_C = "testC",
                MachiningStatus = MachiningStatusEnum.Run
            }));

        }

        /*private void MachiningPartsVMObservableCollectionRefresh()
        {
            if (StampingBoxPartsVM != null)
            {
                var SIndex = StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.FindIndex(x => x.BoxNumber != 0);

                if (SIndex != -1)
                    MachiningPartsVMObservableCollection.Add(StampingBoxPartsVM.BoxPartsParameterVMObservableCollection[SIndex]);
            }
        }*/


        private StampingBoxPartsViewModel _stampingBoxPartsVM;
        /// <summary>
        /// 選擇盒子VM
        /// </summary>
        public StampingBoxPartsViewModel StampingBoxPartsVM
        {
            get => _stampingBoxPartsVM;
            set
            {
                _stampingBoxPartsVM = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StampingSteelBeltViewModel> StampingSteelBeltVMObservableCollection { get; set; }

        public ObservableCollection<PartsParameterViewModel> MachiningPartsVMObservableCollection { get; set; } = new();
    }



}
