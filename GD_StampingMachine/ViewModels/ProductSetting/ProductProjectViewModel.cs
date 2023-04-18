using DevExpress.Data;
using DevExpress.DataAccess.Json;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using GD_StampingMachine.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    public class ProductProjectViewModel : ViewModelBase
    {
        public ProductProjectViewModel(ProductProjectModel _productProject)
        {
            ProductProject = _productProject;
            ProductProject.PartsParameterObservableCollection.ForEach(obj =>
            {
                PartsParameterVMObservableCollection.Add(new PartsParameterViewModel(obj));
            });
            RefreshNumberSettingSavedCollection();

            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {

                        Thread.Sleep(1000);
                        ProductProjectFinishProcessing = 0;
                        if (PartsParameterVMObservableCollection.Count > 0)
                        {
                            double AverageProgress = 0;
                            PartsParameterVMObservableCollection.ForEach(p =>
                            {
                                AverageProgress += p.FinishProgress / PartsParameterVMObservableCollection.Count;
                            });

                            ProductProjectFinishProcessing = AverageProgress;
                        }
                        //OnPropertyChanged(nameof(PartsParameterVMObservableCollection));
                    }
                }
                catch(Exception ex)
                {

                }
            });
        }

        private ProductProjectModel ProductProject = new ProductProjectModel();



        public SheetStampingTypeFormEnum ProductProjectSheetStampingTypeForm
        {
            get => ProductProject.SheetStampingTypeForm;
            set
            {
                ProductProject.SheetStampingTypeForm = value;
                OnPropertyChanged(nameof(ProductProjectSheetStampingTypeForm));
            }
        }
        public string ProductProjectName
        {
            get => ProductProject.Name;
            set
            {
                ProductProject.Name = value;
                OnPropertyChanged(nameof(ProductProjectName));
            }
        }
        public string ProductProjectNumber
        {
            get => ProductProject.Number;
            set
            {
                ProductProject.Number = value; OnPropertyChanged(nameof(ProductProjectNumber));
            }
        }
        public string ProductProjectPath
        {
            get => ProductProject.ProjectPath;
            set
            {
                ProductProject.ProjectPath = value;
                OnPropertyChanged(nameof(ProductProjectPath));
            }
        }
        public DateTime ProductProjectCreateTime
        {
            get => ProductProject.CreateTime;
            set
            {
                ProductProject.CreateTime = value;
                OnPropertyChanged(nameof(ProductProjectCreateTime));
            }
        }
        public DateTime? ProductProjectEditTime
        {
            get => ProductProject.EditTime;
            set
            {
                ProductProject.EditTime = value;
                OnPropertyChanged(nameof(ProductProjectEditTime));
            }
        }




        /// <summary>
        /// 進度條 會以專案內的資料為準
        /// </summary>
        public double ProductProjectFinishProcessing
        {
            get
            {
                return ProductProject.FinishProgress;
            }
            set
            {
                ProductProject.FinishProgress = value;
                OnPropertyChanged(nameof(ProductProjectFinishProcessing));
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
                                if (ObjGridControl.CurrentItem is ProductProjectViewModel CurrentItem)
                                {
                                    var MessageBoxReturn = WinUIMessageBox.Show(null,
                                        (string)Application.Current.TryFindResource("Text_AskDelProject") +
                                        "\r\n" +
                                        $"{CurrentItem.ProductProject.Number} - {CurrentItem.ProductProject.Name}" +
                                        "?"
                                        ,
                                        (string)Application.Current.TryFindResource("Text_notify"),
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Exclamation,
                                        MessageBoxResult.None,
                                        MessageBoxOptions.None,
                                        DevExpress.Xpf.Core.FloatingMode.Window);

                                    if (MessageBoxReturn == MessageBoxResult.Yes)
                                        GridItemSource.Remove(CurrentItem);
                                }
                            }
                        }
                    });
                }
                return _projectDeleteCommand;
            }
            /*set
            {
                _projectDeleteCommand = value;
                OnPropertyChanged(nameof(ProjectDeleteCommand));
            }*/
        }










        private bool _isInParameterPage;
        public bool IsInParameterPage { get => _isInParameterPage; set { _isInParameterPage = value; OnPropertyChanged(); } }

        private PartsParameterViewModel _addNewPartsParameterVM;
        public PartsParameterViewModel AddNewPartsParameterVM
        {
            get
            {
                if(_addNewPartsParameterVM == null)
                    _addNewPartsParameterVM = new PartsParameterViewModel(new Model.ProductionSetting.PartsParameterModel() { ProjectName = ProductProjectName });
                return _addNewPartsParameterVM;
            }
            set
            {
                _addNewPartsParameterVM = value; 
                OnPropertyChanged();
            }
        } 
        public SettingViewModelBase SelectedSettingVMBase
        {
            get => AddNewPartsParameterVM.SettingVMBase;
            set 
            {
                AddNewPartsParameterVM.SettingVMBase = value;
                OnPropertyChanged();
            }
        }
 


        private ObservableCollection<SettingViewModelBase> _numberSettingSavedCollection;
        /// <summary>
        /// 建立零件POPUP-加工型態combobox
        /// </summary>
        public ObservableCollection<SettingViewModelBase> NumberSettingSavedCollection
        {
            get 
            {
                if (_numberSettingSavedCollection == null)
                    _numberSettingSavedCollection = new ObservableCollection<SettingViewModelBase>();
                return _numberSettingSavedCollection;
             }
            set
            {
                _numberSettingSavedCollection = value;
                OnPropertyChanged();
            }
        }


        public ICommand RefreshSavedCollectionCommand
        {
            get => new RelayCommand(() =>
            {
                RefreshNumberSettingSavedCollection();
            });
        }

        public void RefreshNumberSettingSavedCollection()
        {
            var newSavedCollection = new ObservableCollection<SettingViewModelBase>();
            var CsvHM = new GD_CsvHelperMethod();
            if (ProductProject.SheetStampingTypeForm == SheetStampingTypeFormEnum.NormalSheetStamping)
            {
                CsvHM.ReadNumberSetting(out var SavedCollection);
                foreach (var asd in SavedCollection)
                {
                    newSavedCollection.Add(new NumberSettingViewModel(asd));
                }
            }
            if (ProductProject.SheetStampingTypeForm == SheetStampingTypeFormEnum.QRSheetStamping)
            {
                CsvHM.ReadQRNumberSetting(out var QRSavedCollection);
                foreach (var asd in QRSavedCollection)
                {
                    newSavedCollection.Add(new QRSettingViewModel(asd));
                }
            }

            // if (!NumberSettingSavedCollection.Equals(newSavedCollection))
            //NumberSettingSavedCollection = newSavedCollection;
            // if (!NumberSettingSavedCollection.ToList().SequenceEqual(newSavedCollection.ToList()))
            // {
            SelectedSettingVMBase = null;
            NumberSettingSavedCollection = newSavedCollection;
            //  }

        }

        
        private SettingViewModelBase _settingVM;
        /// <summary>
        /// 上方的排列示意圖(純顯示)
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
                if (_partsParameterViewModelSelectItem != null)
                {
                    SettingVM = _partsParameterViewModelSelectItem.SettingVMBase.DeepCloneByJson();
                }
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
        /// GridControl ABC參數
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




        /// <summary>
        /// 建立零件
        /// </summary>
        public ICommand CreatePartCommand
        {
            get => new RelayCommand(() =>
            {
                PartsParameterVMObservableCollection.Add(AddNewPartsParameterVM.DeepCloneByJson());
                //儲存 ProductProject
                ProductProjectEditTime = DateTime.Now;
                SaveProductProject();
            });
        }
        public ICommand SaveProductProjectCommand 
        {
            get => new RelayCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(ProductProject.ProjectPath) || string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(ProductProject.ProjectPath)))
                {
                    //跳出彈跳式視窗
                    using var dialog = new System.Windows.Forms.FolderBrowserDialog();
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                        ProductProject.ProjectPath = dialog.SelectedPath;
                }

                SaveProductProject();
            });
        }
        public bool SaveProductProject()
        {

            if (ProductProject.ProjectPath != null)
            {
                if (!Path.HasExtension(ProductProject.ProjectPath))
                {
                    ProductProject.ProjectPath = Path.Combine(ProductProject.ProjectPath, ProductProject.Name , ".csv");
                    ProductProject.EditTime= DateTime.Now;
                }

                return new GD_CsvHelperMethod().WriteProductProject(ProductProject.ProjectPath, ProductProject);


            }

            return false;
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

        public ICommand MoveItemBetweenGridControlCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                if(obj == null)
                {
                    throw new Exception();
                }

                if(obj is object[] objectArray)
                {
                    if (objectArray.Count() == 2)
                    {
                        DevExpress.Xpf.Grid.GridControl GridControlSource = objectArray[0] as DevExpress.Xpf.Grid.GridControl;
                        DevExpress.Xpf.Grid.GridControl GridControlTarget = objectArray[1] as DevExpress.Xpf.Grid.GridControl ;
                        if(GridControlSource != null && GridControlTarget != null)
                        {
                            if(GridControlSource.CurrentItem is ProductProjectViewModel _currentItem &&
                            GridControlSource.ItemsSource is ObservableCollection<GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel> SourceItemS &&
                            GridControlTarget.ItemsSource is ObservableCollection<GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel> TargetItemS)
                            {
                                TargetItemS.Add(_currentItem);
                                SourceItemS.Remove(_currentItem);
                            }
                        }
                    }
                }
            });
        }





    }
}
