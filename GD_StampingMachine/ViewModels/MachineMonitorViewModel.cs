using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.ExpressionEditor;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.GD_Model.ProductionSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class MachineMonitorModel
    {
        public ProjectDistributeViewModel ProjectDistributeVM { get; set; }
    }

    public class MachineMonitorViewModel : GD_CommonLibrary.BaseViewModel
    {

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("btnDescription_MachineMonitoring");
        public MachineMonitorViewModel(MachineMonitorModel _machineMonitor)
        {
            MachineMonitor = _machineMonitor;
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
                ProjectID = "測試專案",
                ParamA = "testA",
                ParamB = "testB",
                ParamC = "testC",
                MachiningStatus = MachiningStatusEnum.Run
            }));

            if (ProjectDistributeVMObservableCollection != null)
                ProjectDistributeVMSelected = ProjectDistributeVMObservableCollection.FirstOrDefault();
            GridControlRefresh();
        }


        private MachineMonitorModel _machineMonitor;
        public MachineMonitorModel MachineMonitor { get => _machineMonitor ??= new MachineMonitorModel(); set => _machineMonitor = value; }
        public ProjectDistributeViewModel ProjectDistributeVMSelected
        {
            get => MachineMonitor.ProjectDistributeVM;
            set
            {
                MachineMonitor.ProjectDistributeVM = value;
                OnPropertyChanged();
                GridControlRefresh();
            }
        }


        public ICommand ProjectDistributeVMChangeCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                if (obj is ObservableCollection<ProjectDistributeViewModel> NewProjectDistributeVMObser)
                {
                    ProjectDistributeVMSelected = NewProjectDistributeVMObser.FirstOrDefault(); ;
                }
                if (obj is ProjectDistributeViewModel NewProjectDistributeVM)
                {
                    ProjectDistributeVMSelected = NewProjectDistributeVM;
                }
            });
        }


        public TypeSettingSettingViewModel TypeSettingSettingVM { get => Singletons.StampingMachineSingleton.Instance.TypeSettingSettingVM; }
        public ObservableCollection<ProjectDistributeViewModel> ProjectDistributeVMObservableCollection
        {
            get => TypeSettingSettingVM.ProjectDistributeVMObservableCollection;
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
        public ObservableCollection<PartsParameterViewModel> MachiningPartsVMObservableCollection { get => _machiningPartsVMObservableCollection; set { _machiningPartsVMObservableCollection = value; OnPropertyChanged(); } }// = new();

        [JsonIgnore]
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


        [JsonIgnore]
        public ICommand GridControlRefreshCommand
        {
            get => new RelayCommand(() =>
            {
                GridControlRefresh();
            });
        }

        public void GridControlRefresh()
        {
            try
            {
                if (ProjectDistributeVMSelected != null)
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

                  /*  if (StampingBoxPartsVM.SelectedSeparateBoxVM != null)
                        StampingBoxPartsVM.BoxPartsParameterVMObservableCollectionRefresh();*/
                }
            }
            catch (Exception ex)
            {

            }
        }

        public ICommand Box_OnDragRecordOverCommand
        {
            get => Commands.GD_Command.Box_OnDragRecordOverCommand;
        }

        public ICommand Box_OnDropRecordCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                if (obj is DevExpress.Xpf.Core.DropRecordEventArgs e)
                {
                    if (e.Data.GetData(typeof(DevExpress.Xpf.Core.RecordDragDropData)) is DevExpress.Xpf.Core.RecordDragDropData DragDropData)
                    {
                        foreach (var _record in DragDropData.Records)
                        {
                            if (_record is PartsParameterViewModel PartsParameterVM)
                            {
                                //看目前選擇哪一個盒子
                                if (StampingBoxPartsVM.SelectedSeparateBoxVM != null)
                                {
                                    if (ProjectDistributeVMSelected != null)
                                    {
                                        PartsParameterVM.DistributeName = StampingBoxPartsVM.ProjectDistributeName;// ProjectDistribute.ProjectDistributeName;
                                        PartsParameterVM.BoxIndex = StampingBoxPartsVM.SelectedSeparateBoxVM.BoxIndex;
                                        e.Effects = System.Windows.DragDropEffects.Move;
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }


















    }
}
