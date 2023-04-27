using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.ExpressionEditor;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model.ProductionSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class MachiningSettingModel
    {
        public ProjectDistributeViewModel ProjectDistributeVMObservableCollectionSelected { get; set; }
        public ObservableCollection<ProjectDistributeViewModel> ProjectDistributeVMObservableCollection { get; set; }
    }

    public class MachiningSettingViewModel : BaseViewModelWithLog
    {

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_Description_PunchedFontSettingViewModel");

        public MachiningSettingViewModel(MachiningSettingModel _machiningSetting)
        {
            MachiningSetting = _machiningSetting;
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
                BoxIndex = null,
                ProjectID="測試專案",
                ParamA = "testA",
                ParamB = "testB",
                ParamC = "testC",
                MachiningStatus = MachiningStatusEnum.Run
            }));

            if (ProjectDistributeVMObservableCollection  != null)
                ProjectDistributeVMSelected = ProjectDistributeVMObservableCollection.FirstOrDefault();
            GridControlRefresh();
        }

        public MachiningSettingModel MachiningSetting =new();
        public ProjectDistributeViewModel ProjectDistributeVMSelected 
        {
            get=>MachiningSetting.ProjectDistributeVMObservableCollectionSelected;
            set 
            {
                MachiningSetting.ProjectDistributeVMObservableCollectionSelected = value;
                OnPropertyChanged();
            }
        }

     


        public ObservableCollection<ProjectDistributeViewModel> ProjectDistributeVMObservableCollection
        {
            get => MachiningSetting.ProjectDistributeVMObservableCollection;
            private set { MachiningSetting.ProjectDistributeVMObservableCollection = value; OnPropertyChanged(); }
        }


        private StampingBoxPartsViewModel _stampingBoxPartsVM;
        /// <summary>
        /// 盒子VM
        /// </summary>
        public StampingBoxPartsViewModel StampingBoxPartsVM
        {
            get => _stampingBoxPartsVM;
            private set
            {
                _stampingBoxPartsVM = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 鋼帶捲集合
        /// </summary>
        private ObservableCollection<StampingSteelBeltViewModel> _stampingSteelBeltVMObservableCollection = new();
        public ObservableCollection<StampingSteelBeltViewModel> StampingSteelBeltVMObservableCollection { get => _stampingSteelBeltVMObservableCollection; set { _stampingSteelBeltVMObservableCollection = value; OnPropertyChanged(); } }

        private ObservableCollection<PartsParameterViewModel> _machiningPartsVMObservableCollection = new();
        public ObservableCollection<PartsParameterViewModel> MachiningPartsVMObservableCollection { get=> _machiningPartsVMObservableCollection; set { _machiningPartsVMObservableCollection =value;OnPropertyChanged(); } }// = new();


        public ICommand ComboBoxEdit_EditValueChanged
        {
            get => new RelayParameterizedCommand(obj =>
            {
                if (obj is DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
                {
                    GridControlRefresh();
                }
            });
        }



        public ICommand GridControlRefreshCommand
        {
            get => new RelayCommand(() =>
            {
                GridControlRefresh();
            });
        }
      

        public void GridControlRefresh()
        {
            StampingBoxPartsVM = new StampingBoxPartsViewModel(new StampingBoxPartModel()
            {
                ProjectDistributeName = ProjectDistributeVMSelected.ProjectDistributeName,
                ProjectDistributeVMObservableCollection = this.ProjectDistributeVMObservableCollection,
                ProductProjectVMObservableCollection = ProjectDistributeVMSelected.ProductProjectVMObservableCollection,
                SeparateBoxVMObservableCollection = ProjectDistributeVMSelected.SeparateBoxVMObservableCollection,
                GridControl_MachiningStatusColumnVisible = true,
            });

            MachiningPartsVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();

            StampingBoxPartsVM.ProductProjectVMObservableCollection.ForEach(x =>
            {
                x.PartsParameterVMObservableCollection.ForEach(y =>
                {
                    if (y.DistributeName == StampingBoxPartsVM.ProjectDistributeName && y.MachiningStatus == MachiningStatusEnum.Run)
                        MachiningPartsVMObservableCollection.Add(y);
                });
            });

            if (StampingBoxPartsVM.SelectedSeparateBoxVM != null)
                StampingBoxPartsVM.BoxPartsParameterVMObservableCollectionRefresh();
        }





    }



}
