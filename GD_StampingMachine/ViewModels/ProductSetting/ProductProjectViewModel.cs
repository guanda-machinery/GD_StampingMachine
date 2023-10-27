using DevExpress.Data;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Office.Utils;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Interfaces;
using GD_StampingMachine.Method;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.ViewModels.ParameterSetting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GD_StampingMachine.Model;
using Newtonsoft.Json.Linq;
using DevExpress.Mvvm.Xpf;
using CommunityToolkit.Mvvm.Input;
using GD_CommonLibrary.Method;
using CsvHelper.Configuration.Attributes;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    /// <summary>
    /// 製品專案
    /// </summary>
    public class ProductProjectViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_ProductProjectViewModel");


        StampingMachineJsonHelper JsonHM = new();

        public ProductProjectViewModel(ProductProjectModel ProductProject)
        {
            _productProject = ProductProject;
            foreach (var obj in _productProject.PartsParameterObservableCollection)
            {
                PartsParameterVMObservableCollection.Add(new PartsParameterViewModel(obj));
            }

            RefreshNumberSettingSavedCollection();

            Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        await Task.Delay(1000);
                        double AverageProgress = 0;
                       foreach(var p in PartsParameterVMObservableCollection)
                        {
                            AverageProgress += p.FinishProgress / PartsParameterVMObservableCollection.Count;
                        }
                        ProductProjectFinishProcessing = AverageProgress;
                    }
                }
                catch (Exception ex)
                {

                }
            });
        }


        public ProductProjectViewModel()
        {
            _productProject = new();
            foreach (var obj in _productProject.PartsParameterObservableCollection)
            {
                PartsParameterVMObservableCollection.Add(new PartsParameterViewModel(obj));
            }
            RefreshNumberSettingSavedCollection();
        }


        public readonly ProductProjectModel _productProject;


        public SheetStampingTypeFormEnum SheetStampingTypeForm
        {
            get => _productProject.SheetStampingTypeForm;
            set { _productProject.SheetStampingTypeForm = value; OnPropertyChanged(); }
        }


        public string ProductProjectName
        {
            get => _productProject.Name;
            set
            {
                _productProject.Name = value;
                OnPropertyChanged(nameof(ProductProjectName));
            }
        }
        /// <summary>
        /// 工程編號
        /// </summary>
        public string ProductProjectNumber
        {
            get => _productProject.Number;
            set
            {
                _productProject.Number = value; OnPropertyChanged(nameof(ProductProjectNumber));
            }
        }
        public string ProductProjectPath
        {
            get => _productProject.ProjectPath;
            set
            {
                _productProject.ProjectPath = value;
                OnPropertyChanged(nameof(ProductProjectPath));
            }
        }
        public DateTime ProductProjectCreateTime
        {
            get => _productProject.CreateTime;
            set
            {
                _productProject.CreateTime = value;
                OnPropertyChanged(nameof(ProductProjectCreateTime));
            }
        }
        public DateTime? ProductProjectEditTime
        {
            get => _productProject.EditTime;
            set
            {
                _productProject.EditTime = value;
                OnPropertyChanged(nameof(ProductProjectEditTime));
            }
        }



        private bool _isMarked = false;
        /// <summary>
        /// 新增排版專案的打勾符號
        /// </summary>
        public bool IsMarked { get => _isMarked; set { _isMarked = value; OnPropertyChanged(); } }

        /// <summary>
        /// 新增排版專案的打勾符號
        /// </summary>
        public bool IsFinish { get => _productProject.IsFinish; set { _productProject.IsFinish = value; OnPropertyChanged(); } }


        /// <summary>
        /// 進度條 會以專案內的資料為準
        /// </summary>
        public double ProductProjectFinishProcessing
        {
            get
            {
                return _productProject.FinishProgress;
            }
            set
            {
                _productProject.FinishProgress = value;
                OnPropertyChanged(nameof(ProductProjectFinishProcessing));
            }
        }



        //private RelayCommand<object> _projectEditCommand;
        [JsonIgnore]
        public RelayCommand ProjectEditCommand
        {
            get => new RelayCommand(() =>
            {
                EditProjectDarggableIsPopup = true;
            });
        }
        // private RelayCommand<object> _projectDeleteCommand;
        public AsyncRelayCommand<GridControl> ProjectDeleteCommand
        {
            get => new(async ObjGridControl =>
            {
                if (ObjGridControl is not null)
                {
                    if (ObjGridControl.ItemsSource is ObservableCollection<ProductProjectViewModel> GridItemSource)
                    {
                        if (ObjGridControl.CurrentItem is ProductProjectViewModel CurrentItem)
                        {
                            if (CurrentItem.PartsParameterVMObservableCollection.FindIndex(x => !string.IsNullOrEmpty(x.DistributeName)) != -1)
                                await MethodWinUIMessageBox.CanNotCloseProject();
                            else
                            {
                                if (await MethodWinUIMessageBox.AskDelProject($"{CurrentItem._productProject.Number} - {CurrentItem._productProject.Name}"))
                                    GridItemSource.Remove(CurrentItem);
                            }

                        }
                    }
                }
            });
        }
        /// <summary>
        /// 專案完成
        /// </summary>
        public ICommand ProjectFinishCommand
        {
            get => new RelayCommand(() =>
            {
                IsFinish = true;
            });
        }
        /// <summary>
        /// 完成取消
        /// </summary>
        public ICommand ProjectFinishCancelCommand
        {
            get => new RelayCommand(() =>
            {
                IsFinish = false;
            });
        }




        private bool _isInParameterPage;
        public bool IsInParameterPage { get => _isInParameterPage; set { _isInParameterPage = value; OnPropertyChanged(); } }

        private PartsParameterViewModel _addNewPartsParameterVM;
        public PartsParameterViewModel AddNewPartsParameterVM
        {
            get
            {
                _addNewPartsParameterVM ??= new PartsParameterViewModel(new GD_Model.ProductionSetting.PartsParameterModel()
                {
                    ProjectID = ProductProjectName,
                });
                _addNewPartsParameterVM.SettingBaseVM.SheetStampingTypeForm = this.SheetStampingTypeForm;
                return _addNewPartsParameterVM;
            }
            set
            {
                _addNewPartsParameterVM = value;
                OnPropertyChanged();
            }
        }








        private ObservableCollection<SettingBaseViewModel> _numberSettingSavedCollection;
        /// <summary>
        /// 建立零件POPUP-加工型態combobox
        /// </summary>
        public ObservableCollection<SettingBaseViewModel> NumberSettingSavedCollection
        {
            get
            {
                _numberSettingSavedCollection ??= new ObservableCollection<SettingBaseViewModel>();
                return _numberSettingSavedCollection;
            }
            set
            {
                _numberSettingSavedCollection = value;
                OnPropertyChanged();
            }
        }


        [JsonIgnore]
        public ICommand AddNewPartsParameterVM_ClonedParameterCommand
        {
            get => new RelayCommand(() =>
            {
                AddNumberSettingSavedCollection.ForEach(obj =>
                {
                    if (string.IsNullOrEmpty(AddNewPartsParameterVM.IronPlateString))
                        obj.PlateNumber = null;
                    else
                        obj.PlateNumber = AddNewPartsParameterVM.IronPlateString;

                    if (string.IsNullOrEmpty(AddNewPartsParameterVM.QR_Special_Text))
                        obj.QR_Special_Text = null;
                    else
                        obj.QR_Special_Text = AddNewPartsParameterVM.QR_Special_Text;
                });
            });
        }



        private ObservableCollection<SettingBaseViewModel> _addNumberSettingSavedCollection;
        [JsonIgnore]
        public ObservableCollection<SettingBaseViewModel> AddNumberSettingSavedCollection
        {
            get
            {
                return _addNumberSettingSavedCollection ??= new ObservableCollection<SettingBaseViewModel>() ;
            }
            set
            {
                _addNumberSettingSavedCollection = value;
                OnPropertyChanged();
            }
        }



        private ObservableCollection<SettingBaseViewModel> _editNumberSettingSavedCollection;
        [JsonIgnore]
        public ObservableCollection<SettingBaseViewModel> EditNumberSettingSavedCollection
        {
            get 
            {
                return _editNumberSettingSavedCollection ??= new ObservableCollection<SettingBaseViewModel>(); ;
              }
            set 
            {
                _editNumberSettingSavedCollection = value;
                OnPropertyChanged();
            }
         }
        





        private ICommand _refreshSavedCollectionCommand;
        [JsonIgnore]
        public ICommand RefreshSavedCollectionCommand
        {
            get =>_refreshSavedCollectionCommand ??= new RelayCommand(() =>
            {
                RefreshNumberSettingSavedCollection();
            });
            set
            {
                _refreshSavedCollectionCommand = value;
            }
        }

        public void RefreshNumberSettingSavedCollection()
        {
            var newSavedCollection = new DXObservableCollection<SettingBaseViewModel>();

            newSavedCollection.AddRange(Singletons.StampingMachineSingleton.Instance.ParameterSettingVM.NumberSettingPageVM.NumberSettingModelCollection);
            newSavedCollection.AddRange(Singletons.StampingMachineSingleton.Instance.ParameterSettingVM.QRSettingPageVM.QRSettingModelCollection);

            NumberSettingSavedCollection = newSavedCollection;


            AddNumberSettingSavedCollection = NumberSettingSavedCollection.DeepCloneByJson();

              //  if (this.SheetStampingTypeForm != SheetStampingTypeFormEnum.None)
                    AddNumberSettingSavedCollection = AddNumberSettingSavedCollection.ToList()
                        .FindAll(x => x.SheetStampingTypeForm == this.SheetStampingTypeForm)
                        .ToObservableCollection();

                AddNumberSettingSavedCollection.ForEach(obj =>
                {
                    var _partNumber = AddNewPartsParameterVM.ParameterA;
                    if (string.IsNullOrEmpty(_partNumber))
                        obj.PlateNumber = string.Empty;
                    else
                        obj.PlateNumber = _partNumber;
                });
            

            EditNumberSettingSavedCollection = NumberSettingSavedCollection.DeepCloneByJson();
            if (EditPartsParameterVM_Cloned != null)
            {
                //if (EditPartsParameterVM_Cloned.SettingBaseVM.SheetStampingTypeForm != SheetStampingTypeFormEnum.None)
                    EditNumberSettingSavedCollection = EditNumberSettingSavedCollection.ToList()
                        .FindAll(x => x.SheetStampingTypeForm == EditPartsParameterVM_Cloned.SettingBaseVM.SheetStampingTypeForm)
                        .ToObservableCollection();

                EditNumberSettingSavedCollection.ForEach(obj =>
                {
                    var _partNumber = EditPartsParameterVM_Cloned.ParameterA;
                    if (string.IsNullOrEmpty(_partNumber))
                        obj.PlateNumber = string.Empty;
                    else
                        obj.PlateNumber = _partNumber;
                });
            }
        }


        private PartsParameterViewModel _partsParameterViewModelSelectItem;
        /// <summary>
        /// 參數gridcontrol選擇
        /// </summary>
        
        public PartsParameterViewModel PartsParameterViewModelSelectItem
        {
            get=> _partsParameterViewModelSelectItem;
            set
            {
                _partsParameterViewModelSelectItem = value;
                OnPropertyChanged();
            }
        }

        public ICommand PartsParameterViewModelSelectedItemChangedCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                EditPartsParameterVM_Cloned = PartsParameterViewModelSelectItem.DeepCloneByJson();
                RefreshNumberSettingSavedCollection();
            });
        }



        private PartsParameterViewModel _editPartsParameterVM_Cloned;
        public PartsParameterViewModel EditPartsParameterVM_Cloned
        {
            get => _editPartsParameterVM_Cloned ??= new PartsParameterViewModel();
            set
            {
                _editPartsParameterVM_Cloned = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChangePartsParameterVM_ClonedParameterCommand
        {
            get => new RelayCommand(() =>
            {

                EditNumberSettingSavedCollection.ForEach(obj =>
                {
                    if (string.IsNullOrEmpty(EditPartsParameterVM_Cloned.IronPlateString))
                        obj.PlateNumber = null;
                    else
                        obj.PlateNumber = EditPartsParameterVM_Cloned.IronPlateString;

                    if (!EditNumberSettingSavedCollection.Contains(EditPartsParameterVM_Cloned.SettingBaseVM))
                    {
                        //EditNumberSettingSavedCollection.Add(EditPartsParameterVM_Cloned.SettingBaseVM);
                       var Findex = EditNumberSettingSavedCollection.FindIndex(x => x.NumberSettingMode == EditPartsParameterVM_Cloned.SettingBaseVM.NumberSettingMode);
                        if (Findex != -1)
                        {
                            EditPartsParameterVM_Cloned.SettingBaseVM = EditNumberSettingSavedCollection[Findex];
                        }
                    }


                });


            });
        }





        public ICommand OverwritePartsParameterByCloneCommand
        {
            get => new RelayCommand(() =>
            {
                var Findex = PartsParameterVMObservableCollection.FindIndex(x => x == PartsParameterViewModelSelectItem);
                if (Findex != -1)
                {
                    PartsParameterVMObservableCollection[Findex] = EditPartsParameterVM_Cloned.DeepCloneByJson();
                }
            });
        }

        /// <summary>
        /// 匯入 選擇檔案路徑
        /// </summary>
        public AsyncRelayCommand ImportProject_SelectPathFileCommand
        {
            get => new AsyncRelayCommand(async () =>
            {
                await Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                    {
                        System.Windows.Forms.OpenFileDialog sfd = new()
                        {
                            Filter = "SerializationFile (*.csv;*.json)|*.csv;*.json|All files (*.*)|*.*"
                        };

                        if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            ImportFilePath = sfd.FileName;
                        }
                    }));
                });
            });
        }

        private string importFilePath;
        /// <summary>
        /// 匯入專案路徑
        /// </summary>
        public string ImportFilePath { get=> importFilePath; set { importFilePath = value; OnPropertyChanged(); } }


        public class SimpleErpClass
        {
            [Index(0)]
            public string Plate { get; set; }
            [Index(1)]
            public int Count { get; set; }
        }


        /// <summary>
        /// 匯入
        /// </summary>
        public RelayCommand ImportProjectCommand
        {
            get => new RelayCommand(async () =>
            {
                await Task.Run(async () =>
                {
                    List<ERP_IronPlateModel> ErpFile = new();
                    CsvFileManager csvManager = new CsvFileManager();
                    if (JsonHM.ReadJsonFile(ImportFilePath, out ErpFile))
                    {

                    }
                    else if (csvManager.ReadCSVFile(ImportFilePath, out ErpFile, true))
                    {

                    }
                    else if (csvManager.ReadCSVFile<SimpleErpClass>(ImportFilePath, out var CsvFileWithoutHeader, false))
                    {
                        ErpFile = new();
                        //僅有檔頭
                        foreach (var item in CsvFileWithoutHeader)
                        {
                            ERP_IronPlateModel erpIronPlate = new()
                            { 
                                PartNumber = item.Plate, 
                                QrCode = false
                            };
                            var erpIronPlateList = Enumerable.Repeat(erpIronPlate, item.Count);
                            ErpFile.AddRange(erpIronPlateList);
                        }
                    }
                    else
                    {
                        await MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_ImportFail"));
                        return;
                        //跳出錯誤
                    }

                    //將資料拆分為QR/一般
                    if (ErpFile.Exists(x => !x.QrCode) && ImportProjectNumberSettingBaseVM == null)
                    {
                        await MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_ImportNeedSetNumberCode"));
                        return;
                    }
                    if (ErpFile.Exists(x => x.QrCode) && ImportProjectQRSettingBaseVM == null)
                    {
                        await MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_ImportNeedSetQrCode"));
                        return;
                    }

                    //檢查字長度
                    List<PartsParameterViewModel> importPartsParameterVMList = new ();
                    foreach (var _erp in ErpFile)
                    {
                        //開始轉換檔案
                        SettingBaseViewModel SettingBaseVM;
                        if (_erp.QrCode)
                        {
                            SettingBaseVM = ImportProjectQRSettingBaseVM.DeepCloneByJson();
                            SettingBaseVM.SheetStampingTypeForm = SheetStampingTypeFormEnum.qrcode;
                        }
                        else
                        {
                            SettingBaseVM = ImportProjectNumberSettingBaseVM.DeepCloneByJson();
                            SettingBaseVM.SheetStampingTypeForm = SheetStampingTypeFormEnum.normal;
                        }

                        importPartsParameterVMList.Add(new PartsParameterViewModel()
                        {
                            
                            IronPlateString = _erp.PartNumber,
                            QrCodeContent = _erp.QrCodeContent?.FirstOrDefault(),
                            QR_Special_Text = _erp.TrainNumber?.FirstOrDefault(),
                            SettingBaseVM = SettingBaseVM
                        });

                        ProductProjectEditTime = DateTime.Now;
                    }

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        PartsParameterVMObservableCollection.AddRange(importPartsParameterVMList);
                    });
                    //儲存 ProductProject
                    SaveProductProject();
                });
            });
        }
        /// <summary>
        /// 匯入專案-加工型態 一般鐵牌
        /// </summary>
        public NumberSettingViewModel ImportProjectNumberSettingBaseVM { get; set; }
        /// <summary>
        /// 匯入專案-加工型態 QR鐵牌
        /// </summary>
        public QRSettingViewModel ImportProjectQRSettingBaseVM { get; set; }







        private DXObservableCollection<PartsParameterViewModel> _partsParameterVMObservableCollection;
        /// <summary>
        /// GridControl ABC參數
        /// </summary>
        public DXObservableCollection<PartsParameterViewModel> PartsParameterVMObservableCollection
        {
            get
            {
                _partsParameterVMObservableCollection ??= new();
                _productProject.PartsParameterObservableCollection =  _partsParameterVMObservableCollection.Select(x => x.PartsParameter).ToObservableCollection();
                return _partsParameterVMObservableCollection;
            }
            set
            {
                _partsParameterVMObservableCollection = value;
                OnPropertyChanged(nameof(PartsParameterVMObservableCollection));
            }
        }








        private ICommand _createPartCommand;
        /// <summary>
        /// 建立零件
        /// </summary>
        [JsonIgnore]
        public ICommand CreatePartCommand
        {
            get =>_createPartCommand ??= new RelayCommand(() =>
            {
                PartsParameterVMObservableCollection.Add(AddNewPartsParameterVM.DeepCloneByJson());
                //儲存 ProductProject
                ProductProjectEditTime = DateTime.Now;
                SaveProductProject();
            });
            set
            { 
                _createPartCommand = value;
                OnPropertyChanged(nameof(CreatePartCommand)); 
            }
        }

        private ICommand _saveProductProjectCommand;
        [JsonIgnore]
        public ICommand SaveProductProjectCommand
        {
            get => _saveProductProjectCommand ??= new RelayCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(_productProject.ProjectPath) || string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(_productProject.ProjectPath)))
                {
                    //跳出彈跳式視窗
                    using var dialog = new System.Windows.Forms.FolderBrowserDialog();
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                        _productProject.ProjectPath = dialog.SelectedPath;
                }
                SaveProductProject();
            });
            set
            {
                _saveProductProjectCommand = value;
                OnPropertyChanged();
            }
        }
        public bool SaveProductProject()
        {
            if (_productProject.ProjectPath != null)
            {
                return JsonHM.WriteJsonFile(Path.Combine( _productProject.ProjectPath, _productProject.Name), _productProject);
            }
            else
            {
                Debugger.Break();
                return false;
            }
        }

        private bool _addParameterDarggableIsPopup;
        /// <summary>
        /// 加入新零件
        /// </summary>
        public bool AddParameterDarggableIsPopup
        {
            get
            {
                return _addParameterDarggableIsPopup;
            }
            set
            {
                _addParameterDarggableIsPopup = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }


        private bool _editProjectDarggableIsPopup;
        public bool EditProjectDarggableIsPopup
        {
            get
            {
                return _editProjectDarggableIsPopup;
            }
            set
            {
                _editProjectDarggableIsPopup = value;
                OnPropertyChanged();
            }
        }



       // private ICommand _addTypeSettingCommand;
        /// <summary>
        /// 新增排版專案
        /// </summary>
        [JsonIgnore]
        public ICommand AddTypeSettingCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                if (obj == null)
                {
                    throw new Exception();
                }

                //新寫法
                if (obj is GD_StampingMachine.ViewModels.ProjectDistributeViewModel ProjectDistributeVM)
                {
                    if (ProjectDistributeVM.NotReadyToTypeSettingProductProjectVMCurrentItem != null)
                    {
                        ProjectDistributeVM. NotReadyToTypeSettingProductProjectVMCurrentItem.IsMarked = true;
                        ProjectDistributeVM.ProjectDistribute.ProductProjectNameList.Add(ProjectDistributeVM.NotReadyToTypeSettingProductProjectVMCurrentItem.ProductProjectName);
                        ProjectDistributeVM.ReadyToTypeSettingProductProjectVMObservableCollection.Add(ProjectDistributeVM.NotReadyToTypeSettingProductProjectVMCurrentItem);
                        ProjectDistributeVM.NotReadyToTypeSettingProductProjectVMObservableCollection.Remove(ProjectDistributeVM.NotReadyToTypeSettingProductProjectVMCurrentItem);
                    }
                }

                //舊寫法
                if (obj is object[] objectArray)
                {
                    if (objectArray.Count() == 2)
                    {
                        DevExpress.Xpf.Grid.GridControl GridControlSource = objectArray[0] as DevExpress.Xpf.Grid.GridControl;
                        DevExpress.Xpf.Grid.GridControl GridControlTarget = objectArray[1] as DevExpress.Xpf.Grid.GridControl;
                        if (GridControlSource != null && GridControlTarget != null)
                        {
                            if (GridControlSource.CurrentItem is ProductProjectViewModel _currentItem &&
                            GridControlSource.ItemsSource is ObservableCollection<GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel> SourceItemS &&
                            GridControlTarget.ItemsSource is ObservableCollection<GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel> TargetItemS)
                            {
                                _currentItem.IsMarked= true;
                                TargetItemS.Add(_currentItem);
                                SourceItemS.Remove(_currentItem);
                            }
                        }
                    }
                }
            });
        }



        /// <summary>
        /// 關閉排版專案
        /// </summary>
        [JsonIgnore]
        public ICommand CloseTypeSettingCommand
        {
            get => new AsyncRelayCommand<object>(async obj =>
            {
                    if (obj == null)
                    {
                        throw new Exception();
                    }

                    //新寫法
                    if (obj is GD_StampingMachine.ViewModels.ProjectDistributeViewModel ProjectDistributeVM)
                    {
                        if (ProjectDistributeVM.ReadyToTypeSettingProductProjectVMCurrentItem != null)
                        {
                            var CollectionWithThisDistributeName = ProjectDistributeVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.ToList().FindAll(x => x.DistributeName == ProjectDistributeVM.ProjectDistributeName);

                            //var CollectionWithThisDistributeName = ProjectDistributeVM.SeparateBoxVMObservableCollection.ToList().FindAll(x => x.DistributeName == ProjectDistributeVM.ProjectDistributeName);
                            //箱子內有專案
                            if (CollectionWithThisDistributeName.Count > 0)
                            {
                                //有已完成的 不可關閉
                                if (CollectionWithThisDistributeName.ToList().Exists(x => x.MachiningStatus == MachiningStatusEnum.Finish))
                                {
                                    await MethodWinUIMessageBox.CanNotCloseProject();
                                    return;
                                }

                                //詢問是否要關閉
                                if (!(await MethodWinUIMessageBox.AskCloseProject(ProjectDistributeVM.ReadyToTypeSettingProductProjectVMCurrentItem.ProductProjectName)))
                                    return;

                                //將資料清除
                                CollectionWithThisDistributeName.ForEach(Eobj =>
                                {
                                    Eobj.DistributeName = null;
                                    Eobj.BoxIndex = null;
                                });
                                ProjectDistributeVM.StampingBoxPartsVM.ReLoadBoxPartsParameterVMObservableCollection();
                                //  ProjectDistributeVM.StampingBoxPartsVM.ProductProjectVMObservableCollection
                            }

                            ProjectDistributeVM.ProjectDistribute.ProductProjectNameList.Remove(ProjectDistributeVM.ReadyToTypeSettingProductProjectVMCurrentItem.ProductProjectName);
                            ProjectDistributeVM.NotReadyToTypeSettingProductProjectVMObservableCollection.Add(ProjectDistributeVM.ReadyToTypeSettingProductProjectVMCurrentItem);
                            ProjectDistributeVM.ReadyToTypeSettingProductProjectVMObservableCollection.Remove(ProjectDistributeVM.ReadyToTypeSettingProductProjectVMCurrentItem);
                        }
                    }

            });
        }

        //[JsonIgnore]
       // public ICommand BoxPartsParameterVMObservableCollectionRefreshCommand { get; set; }










    }
}
