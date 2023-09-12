using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.ExpressionEditor;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.GD_Model.ProductionSetting;
using GD_StampingMachine.Singletons;
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
    /*public class MachineMonitorModel
    {
        public ProjectDistributeViewModel ProjectDistributeVM { get; set; }
    }*/

    public class MachineMonitorViewModel : GD_CommonLibrary.BaseViewModel
    {

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("btnDescription_MachineMonitoring");
        public MachineMonitorViewModel()
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
        }


        /// <summary>
        /// 鋼帶捲集合
        /// </summary>
        private ObservableCollection<StampingSteelBeltViewModel> _stampingSteelBeltVMObservableCollection = new();
        public ObservableCollection<StampingSteelBeltViewModel> StampingSteelBeltVMObservableCollection { get => _stampingSteelBeltVMObservableCollection; set { _stampingSteelBeltVMObservableCollection = value; OnPropertyChanged(); } }


        public double StampWidth { get; set; } = 50;


        
        public ICommand SoftSendMachineCommand
        {
            get => new RelayCommand(() =>
            {
                ObservableCollection<PartsParameterViewModel> _boxPartsParameterVMObservableCollection = StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection;
                if (_boxPartsParameterVMObservableCollection != null)
                    for (int i = 1; i < _boxPartsParameterVMObservableCollection.Count; i++)
                    {
                        //boxPartsParameterVMObservableCollection[i].AbsoluteMoveDistance = boxPartsParameterVMObservableCollection[i - 1].AbsoluteMoveDistance + boxPartsParameterVMObservableCollection[i - 1].StampWidth;
                        //boxPartsParameterVMObservableCollection[i].AbsoluteMoveDistance = boxPartsParameterVMObservableCollection[i - 1].AbsoluteMoveDistance + StampWidth;
                        _boxPartsParameterVMObservableCollection[i].AbsoluteMoveDistance = _boxPartsParameterVMObservableCollection[0].AbsoluteMoveDistance + StampWidth * i;
                    }
            });
        }

        public ICommand SetStatusSendMachineCommand
        {
            get => new RelayCommand(() =>
            {
                ObservableCollection<PartsParameterViewModel> _boxPartsParameterVMObservableCollection = StampingMachineSingleton.Instance.SelectedProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection;
                if (_boxPartsParameterVMObservableCollection != null)
                    for (int i = 0; i < _boxPartsParameterVMObservableCollection.Count; i++)
                    {
                        //boxPartsParameterVMObservableCollection[i].AbsoluteMoveDistance = boxPartsParameterVMObservableCollection[i - 1].AbsoluteMoveDistance + boxPartsParameterVMObservableCollection[i - 1].StampWidth;
                        //boxPartsParameterVMObservableCollection[i].AbsoluteMoveDistance = boxPartsParameterVMObservableCollection[i - 1].AbsoluteMoveDistance + StampWidth;
                        var steelBeltStampingStatus = SteelBeltStampingStatusEnum.None;
                        if (i<3)
                        {
                            steelBeltStampingStatus = SteelBeltStampingStatusEnum.Shearing;
                        }
                        else if (i<6)
                        {
                            steelBeltStampingStatus = SteelBeltStampingStatusEnum.Stamping;
                        }
                        else if(i<10)
                        {
                            steelBeltStampingStatus = SteelBeltStampingStatusEnum.QRCarving;
                        }
                        else
                        {
                            steelBeltStampingStatus = SteelBeltStampingStatusEnum.None;
                        }
                        _boxPartsParameterVMObservableCollection[i].SteelBeltStampingStatus = steelBeltStampingStatus;
                    }


            });
        }






            }
}
