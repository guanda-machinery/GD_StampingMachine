using DevExpress.Data;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using Force.DeepCloner;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    public class ProductProjectViewModel : ViewModelBase
    {
        public ProductProjectModel ProductProject { get; set; } = new ProductProjectModel();

        private ObservableCollection<PartsParameterViewModel> _pProjectPartsParameterVMObservableCollection;
        /// <summary>
        /// 參數gridcontrol用vm (展開後展開)
        /// </summary>
        public ObservableCollection<PartsParameterViewModel> PProjectPartsParameterVMObservableCollection
        {
            get
            {
                if (_pProjectPartsParameterVMObservableCollection == null)
                {
                    ProductProject.PartsParameterObservableCollection.ForEach(obj =>
                    {
                        _pProjectPartsParameterVMObservableCollection.Add(new PartsParameterViewModel(obj));
                    });
                }
                return _pProjectPartsParameterVMObservableCollection;
            }
            set
            {
                _pProjectPartsParameterVMObservableCollection = value;

                ProductProject.PartsParameterObservableCollection = new ObservableCollection<Model.ProductionSetting.PartsParameterModel>();
                _pProjectPartsParameterVMObservableCollection.ForEach(obj =>
                {
                    ProductProject.PartsParameterObservableCollection.Add(obj.PartsParameter);
                });


                OnPropertyChanged(nameof(PProjectPartsParameterVMObservableCollection));
            }
        }
        private RelayParameterizedCommand _projectEditCommand;
        public RelayParameterizedCommand ProjectEditCommand
        {
            get
            {
                if (_projectEditCommand == null)
                {
                    _projectEditCommand = new RelayParameterizedCommand(obj =>
                    {

                    });
                }
                return _projectEditCommand;
            }
            set
            {
                _projectEditCommand = value;
                OnPropertyChanged(nameof(ProjectEditCommand));
            }
        }
        private RelayParameterizedCommand _projectDeleteCommand;
        public RelayParameterizedCommand ProjectDeleteCommand
        {
            get
            {
                if (_projectDeleteCommand == null)
                {
                    _projectDeleteCommand = new RelayParameterizedCommand(obj =>
                    {
                        if (obj is GridControl ObjGridControl)
                        {
                            if (ObjGridControl.ItemsSource is ObservableCollection<ProductProjectViewModel> GridItemSource)
                            {
                                var MessageBoxReturn = WinUIMessageBox.Show(null,
                                    (string)Application.Current.TryFindResource("Text_AskDelProject") +
                                    "\r\n" +
                                    $"{this.ProductProject.Number} - {this.ProductProject.Name}" +
                                    "?"
                                    ,
                                    (string)Application.Current.TryFindResource("Text_notify"),
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Exclamation,
                                    MessageBoxResult.None,
                                    MessageBoxOptions.None,
                                    DevExpress.Xpf.Core.FloatingMode.Window);

                                if (MessageBoxReturn == MessageBoxResult.Yes)
                                    GridItemSource.Remove(this);
                            }
                        }
                    });
                }
                return _projectDeleteCommand;
            }
            set
            {
                _projectDeleteCommand = value;
                OnPropertyChanged(nameof(ProjectDeleteCommand));
            }
        }





        public PartsParameterViewModel AddNewPartsParameterVM { get; set; } = new PartsParameterViewModel();
        public NumberSettingModelBase NumberSettingModelSavedComboSelected
        {
            get => AddNewPartsParameterVM.NumberSetting;
            set
            {
                AddNewPartsParameterVM.NumberSetting = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<NumberSettingModelBase> _numberSettingModelSavedCollection;
        /// <summary>
        /// 建立零件POPUP-加工型態combobox用
        /// </summary>
        public ObservableCollection<NumberSettingModelBase> NumberSettingModelSavedCollection
        {
            get 
            {
                if (_numberSettingModelSavedCollection == null)
                {
                    _numberSettingModelSavedCollection = new ObservableCollection<NumberSettingModelBase>();
                    var CsvHM = new GD_RWCsvFile();
                    if (ProductProject.SheetStampingTypeForm == SheetStampingTypeFormEnum.NormalSheetStamping)
                    {
                        CsvHM.ReadNumberSetting(out var SavedCollection);
                        foreach (var asd in SavedCollection)
                        {
                            _numberSettingModelSavedCollection.Add(asd);
                        }
                    }
                    if (ProductProject.SheetStampingTypeForm == SheetStampingTypeFormEnum.QRSheetStamping)
                    {
                        CsvHM.ReadQRNumberSetting(out var QRSavedCollection);
                        foreach (var asd in QRSavedCollection)
                        {
                            _numberSettingModelSavedCollection.Add(asd);
                        }
                    }
                }
                return _numberSettingModelSavedCollection;
            }
            set
            {
                _numberSettingModelSavedCollection = value;
                OnPropertyChanged();
            }
        }



        private SettingViewModelBase _settingVM;
        /// <summary>
        /// 上方的排列示意圖
        /// </summary>
        public SettingViewModelBase SettingVM 
        {
            get => _settingVM;
            set
            {
                _settingVM = value;
                OnPropertyChanged();
            }
        } 


        private PartsParameterViewModel _partsParameterViewModelSelectItem;
        /// <summary>
        /// 參數gridcontrol選擇
        /// </summary>
       public PartsParameterViewModel PartsParameterViewModelSelectItem
       {
            get
            {
                if(_partsParameterViewModelSelectItem!=null)
                    SettingVM = new NumberSettingViewModel()
                    {  
                        NumberSetting = _partsParameterViewModelSelectItem.NumberSetting as NumberSettingModel
                    };
                return _partsParameterViewModelSelectItem;
            }
            set
            {
                _partsParameterViewModelSelectItem = value;
                OnPropertyChanged();
            }
       }

        private ObservableCollection<PartsParameterViewModel> _partsParameterVMObservableCollection;
        /// <summary>
        /// GridControl
        /// </summary>
        public ObservableCollection<PartsParameterViewModel> PartsParameterVMObservableCollection
        {
            get
            {
                if (_partsParameterVMObservableCollection == null)
                    _partsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();
                return _partsParameterVMObservableCollection;
            }
            set
            {
                _partsParameterVMObservableCollection = value;
                OnPropertyChanged(nameof(PartsParameterVMObservableCollection));
            }
        }



        private readonly object balanceLock = new object();

        /// <summary>
        /// 建立零件
        /// </summary>
        public ICommand CreatePartCommand
        {
            get => new RelayCommand(() =>
            {

                SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new SplashScreenWindow.ProcessingScreenWindow(), new DXSplashScreenViewModel { });

                manager.ViewModel.Status = (string)System.Windows.Application.Current.TryFindResource("Text_Loading");
                // manager.ViewModel.Status = "讀取中.";

                manager.ViewModel.Progress = 0;
                manager.ViewModel.IsIndeterminate = true;
                manager.Show(null, WindowStartupLocation.CenterScreen, true, InputBlockMode.Window);

                PartsParameterVMObservableCollection.Add(AddNewPartsParameterVM.DeepClone());

                manager.Close();

            });
        }



        private bool _addParameterDarggableIsPopup;
        public bool AddParameterDarggableIsPopup
        {
            get
            {
                return _addParameterDarggableIsPopup;
            }
            set
            {
                _addParameterDarggableIsPopup = value;
                OnPropertyChanged(nameof(AddParameterDarggableIsPopup));
            }
        }


        private bool _importParameterDarggableIsPopup;
        public bool ImportParameterDarggableIsPopup
        {
            get
            {
                return _importParameterDarggableIsPopup;
            }
            set
            {
                _importParameterDarggableIsPopup = value;
                OnPropertyChanged(nameof(ImportParameterDarggableIsPopup));
            }
        }
        private bool _exportParameterDarggableIsPopup;
        public bool ExportParameterDarggableIsPopup
        {
            get
            {
                return _exportParameterDarggableIsPopup;
            }
            set
            {
                _exportParameterDarggableIsPopup = value;
                OnPropertyChanged(nameof(ExportParameterDarggableIsPopup));
            }
        }









    }
}
