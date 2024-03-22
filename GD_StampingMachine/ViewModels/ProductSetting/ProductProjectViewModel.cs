using CommunityToolkit.Mvvm.Input;
using CsvHelper.Configuration.Attributes;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Extensions;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.CodeView;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.GD_Model.ProductionSetting;
using GD_StampingMachine.Method;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels.ParameterSetting;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    /// <summary>
    /// 製品專案
    /// </summary>
    public class ProductProjectViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("NameProductProjectViewModel");


        StampingMachineJsonHelper JsonHM = new();

        static ProductProjectViewModel()
        {

        }

        public ProductProjectViewModel(ProductProjectModel productProject)
        {
            ProductProject = productProject;
            init();
        }

        public ProductProjectViewModel()
        {
            ProductProject = new();
            init();
        }

        private void init()
        {
            Singletons.StampingMachineSingleton.Instance.ParameterSettingVM.NumberSettingPageVM.NumberSettingModelCollection.CollectionChanged += SettingModelCollection_CollectionChanged;
            Singletons.StampingMachineSingleton.Instance.ParameterSettingVM.QRSettingPageVM.QRSettingModelCollection.CollectionChanged += SettingModelCollection_CollectionChanged;


            var newSavedCollection = new ObservableCollection<SettingBaseViewModel>();
            newSavedCollection.AddRange(Singletons.StampingMachineSingleton.Instance.ParameterSettingVM.NumberSettingPageVM.NumberSettingModelCollection);
            newSavedCollection.AddRange(Singletons.StampingMachineSingleton.Instance.ParameterSettingVM.QRSettingPageVM.QRSettingModelCollection);
            NumberSettingSavedCollection = newSavedCollection;
            UpdateNumberSettingSaveCollection();
        }

        private void SettingModelCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewItems is ICollection newCollection)
                        {
                            foreach (var item in newCollection)
                            {
                                if (item is SettingBaseViewModel numberSetting && !NumberSettingSavedCollection.Contains(numberSetting))
                                    NumberSettingSavedCollection.Add(numberSetting);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.OldItems is ICollection oldCollection)
                        {
                            foreach (var item in oldCollection)
                            {
                                if (item is SettingBaseViewModel numberSetting && NumberSettingSavedCollection.Contains(numberSetting))
                                    NumberSettingSavedCollection.Remove(numberSetting);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        if (e.NewItems is ICollection newCollection && e.OldItems is ICollection oldCollection)
                        {
                            var replacements = oldCollection.OfType<SettingBaseViewModel>()
                            .Zip(newCollection.OfType<SettingBaseViewModel>(), (oldItem, newItem) => (oldItem, newItem));
                            foreach (var (oldItem, newItem) in replacements)
                            {
                                var numberIndex = NumberSettingSavedCollection.IndexOf(oldItem);
                                if(numberIndex !=-1)
                                {
                                    NumberSettingSavedCollection[numberIndex] = newItem;
                                }
                                //var numberIndex = QRSettingSavedCollection.IndexOf(oldItem);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                    default:
                    break;
            }

            UpdateNumberSettingSaveCollection();
        }


        void UpdateNumberSettingSaveCollection()
        {
            try
            {

                AddNumberSettingSavedCollection = NumberSettingSavedCollection
                   .Where(x => x.SheetStampingTypeForm == this.SheetStampingTypeForm)
                   .ToObservableCollection().DeepCloneByJson();

                for (int i = 0; i < AddNumberSettingSavedCollection.Count; i++)
                {
                    var obj = _addNumberSettingSavedCollection[i];

                    obj.PlateNumber = AddNewPartsParameterVM.ParameterA;
                    obj.QrCodeContent = AddNewPartsParameterVM.ParameterC;
                    obj.QR_Special_Text = AddNewPartsParameterVM.QR_Special_Text;
                    AddNewPartsParameterVM.PropertyChanged += (s, e) =>
                    {
                        if (s is PartsParameterViewModel PParameter)
                        {
                            obj.PlateNumber = PParameter.ParameterA;
                            obj.QrCodeContent = PParameter.ParameterC;
                            obj.QR_Special_Text = PParameter.QR_Special_Text;
                        }
                    };


                }
                OnPropertyChanged(nameof(AddNumberSettingSavedCollection));


                if (EditPartsParameterVM_Cloned != null)
                {
                    _editNumberSettingSavedCollection = NumberSettingSavedCollection
                        .Where(x => x.SheetStampingTypeForm == EditPartsParameterVM_Cloned.SettingBaseVM.SheetStampingTypeForm)
                            .ToObservableCollection().DeepCloneByJson();
                }
                else
                    _editNumberSettingSavedCollection = NumberSettingSavedCollection.DeepCloneByJson();
              
                if(EditPartsParameterVM_Cloned!=null)
                {
                    EditNumberSettingSavedCollection.Add(EditPartsParameterVM_Cloned.SettingBaseVM);
                    for (int i = 0; i < EditNumberSettingSavedCollection.Count; i++)
                    {
                        var obj = EditNumberSettingSavedCollection[i];
                        obj.PlateNumber = EditPartsParameterVM_Cloned.ParameterA;
                        obj.QrCodeContent = EditPartsParameterVM_Cloned.ParameterC;
                        obj.QR_Special_Text = EditPartsParameterVM_Cloned.QR_Special_Text;

                        EditPartsParameterVM_Cloned.PropertyChanged += (s, e) =>
                        {
                            if(s is PartsParameterViewModel PParameter)
                            {
                                obj.PlateNumber = PParameter.ParameterA;
                                obj.QrCodeContent = PParameter.ParameterC;
                                obj.QR_Special_Text = PParameter.QR_Special_Text;
                            }
                        
                        };

                    }
                }

                OnPropertyChanged(nameof(EditNumberSettingSavedCollection));

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                Debugger.Break();
            }
        }



        private CancellationTokenSource _cts = new();
        protected override void Dispose(bool disposing)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            base.Dispose(disposing);
        }


        //private ProductProjectModel? _productProject;
        [JsonIgnore]
        public readonly ProductProjectModel ProductProject;// => _productProject;


        public SheetStampingTypeFormEnum SheetStampingTypeForm
        {
            get => ProductProject.SheetStampingTypeForm;
            set { ProductProject.SheetStampingTypeForm = value; OnPropertyChanged(); }
        }


        public string ProductProjectName
        {
            get => ProductProject.Name;
            set
            {
                ProductProject.Name = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 工程編號
        /// </summary>
        public string ProductProjectNumber
        {
            get => ProductProject.Number;
            set
            {
                ProductProject.Number = value; OnPropertyChanged();
            }
        }
        public string ProductProjectPath
        {
            get => ProductProject.ProjectPath;
            set
            {
                ProductProject.ProjectPath = value;
                OnPropertyChanged();
            }
        }
        public DateTime ProductProjectCreateTime
        {
            get => ProductProject.CreateTime;
            set
            {
                ProductProject.CreateTime = value;
                OnPropertyChanged();
            }
        }
        public DateTime? ProductProjectEditTime
        {
            get => ProductProject.EditTime;
            set
            {
                ProductProject.EditTime = value;
                OnPropertyChanged();
            }
        }



        private bool _isMarked = false;
        /// <summary>
        /// 新增排版專案的打勾符號
        /// </summary>
        public bool IsMarked { get => _isMarked; set { _isMarked = value; OnPropertyChanged(); IsMarkedChanged?.Invoke(this, value); } }
       
        public event EventHandler<bool>? IsMarkedChanged;



        private bool _fileIsNotExisted = false;
        public bool FileIsNotExisted { get => _fileIsNotExisted; set { _fileIsNotExisted = value; OnPropertyChanged(); } }


        /// <summary>
        /// 新增排版專案的打勾符號
        /// </summary>
        public bool ProductProjectIsFinish 
        { 
            get => ProductProject.ProductProjectIsFinish; 
            set
            { 
                ProductProject.ProductProjectIsFinish = value; 
                OnPropertyChanged();
                ProductProjectIsFinishChanged?.Invoke(this, value);
            }
        }
        
        public event EventHandler<bool>? ProductProjectIsFinishChanged;


        //private RelayCommand<object> _projectEditCommand;
        [JsonIgnore]
        public RelayCommand ProjectEditCommand
        {
            get => new(() =>
            {
                EditProjectDarggableIsPopup = true;
            });
        }

        private ICommand? _projectFinishCommand;
        /// <summary>
        /// 專案完成
        /// </summary>
        public ICommand ProjectFinishCommand
        {
            get => _projectFinishCommand??=new RelayCommand(() =>
            {
                ProductProjectIsFinish = true;
            });
        }



        /// <summary>
        /// 完成取消
        /// </summary>
        public ICommand ProjectFinishCancelCommand
        {
            get => new RelayCommand(() =>
            {
                ProductProjectIsFinish = false;
            });
        }




        private bool _isInParameterPage;
        public bool IsInParameterPage { get => _isInParameterPage; set { _isInParameterPage = value; OnPropertyChanged(); } }

        private PartsParameterViewModel? _addNewPartsParameterVM;
        public PartsParameterViewModel AddNewPartsParameterVM
        {
            get
            {
                _addNewPartsParameterVM ??= new PartsParameterViewModel(new GD_Model.ProductionSetting.PartsParameterModel()
                {

                });
                return _addNewPartsParameterVM;
            }
            set
            {
                _addNewPartsParameterVM = value;
                OnPropertyChanged();
            }
        }

        private ushort _addNewPartsCount = 1;
        public ushort AddNewPartsCount
        {
            get
            {
               if( _addNewPartsCount==0)
                    _addNewPartsCount = 1;
               return _addNewPartsCount;
             }
            set
            {
                _addNewPartsCount = value; 
                OnPropertyChanged();
            }
        }





        private ObservableCollection<SettingBaseViewModel>? _numberSettingSavedCollection;
        /// <summary>
        /// 建立零件POPUP-加工型態combobox
        /// </summary>
        public ObservableCollection<SettingBaseViewModel> NumberSettingSavedCollection
        {
            get=>_numberSettingSavedCollection ??= new ObservableCollection<SettingBaseViewModel>();
            set
            {
                _numberSettingSavedCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SettingBaseViewModel>? _addNumberSettingSavedCollection;
        [JsonIgnore]
        public ObservableCollection<SettingBaseViewModel> AddNumberSettingSavedCollection
        {
            get => _addNumberSettingSavedCollection??=new();
            set
            {
                _addNumberSettingSavedCollection = value;OnPropertyChanged();
            }
        }




        




        private PartsParameterViewModel? _partsParameterViewModelSelectItem;
        /// <summary>
        /// 參數 gridcontrol選擇
        /// </summary>

        public PartsParameterViewModel? PartsParameterViewModelSelectItem
        {
            get => _partsParameterViewModelSelectItem;
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
            });
        }




        private ICommand?_gridControlSizeChangedCommand;
        public ICommand GridControlSizeChangedCommand
        {
            get => _gridControlSizeChangedCommand ??= new RelayCommand<object>(obj =>
            {
                if (obj is System.Windows.SizeChangedEventArgs e)
                {
                    if (e.Source is DevExpress.Xpf.Grid.GridControl gridcontrol)
                    {
                        if (gridcontrol.View is DevExpress.Xpf.Grid.TableView tableview)
                        {
                            var pageSize = ((tableview.ActualHeight - tableview.HeaderPanelMinHeight - 30) / 40);
                            tableview.PageSize = (pageSize < 3 ? 3 : (int)pageSize);
                        }
                    }
                }
            });
        }

        private PartsParameterViewModel? _editPartsParameterVM_Cloned;
        public PartsParameterViewModel EditPartsParameterVM_Cloned
        {
            get => _editPartsParameterVM_Cloned ??= new PartsParameterViewModel();
            set
            {
                _editPartsParameterVM_Cloned = value;
                OnPropertyChanged();
                UpdateNumberSettingSaveCollection();
            }
        }



        private ObservableCollection<SettingBaseViewModel> _editNumberSettingSavedCollection = new();
        [JsonIgnore]
        public ObservableCollection<SettingBaseViewModel> EditNumberSettingSavedCollection
        {
            get=>_editNumberSettingSavedCollection;
        }


        private AsyncRelayCommand<object>? _productDeleteCommand;
        [JsonIgnore]
        public AsyncRelayCommand<object> ProductDeleteCommand
        {
            get => _productDeleteCommand??= new (async obj =>
            {
                if (obj is PartsParameterViewModel pParameter)
                {
                    if (await MethodWinUIMessageBox.AskDelProjectAsync(null, pParameter.ParameterA))
                     {
                         this.PartsParameterVMObservableCollection.Remove(pParameter);
                     }
                }
            });
        }





        /*public ICommand ChangePartsParameterVM_ClonedParameterCommand
        {
            get => new RelayCommand(() =>
            {
                EditNumberSettingSavedCollection.ForEach(obj =>
                {
                    if (string.IsNullOrEmpty(EditPartsParameterVM_Cloned.ParameterA))
                        obj.PlateNumber = null;
                    else
                        obj.PlateNumber = EditPartsParameterVM_Cloned.ParameterA;

                    if (!EditNumberSettingSavedCollection.Contains(EditPartsParameterVM_Cloned.SettingBaseVM))
                    {
                        var Findex = EditNumberSettingSavedCollection.FindIndex(x => x.NumberSettingMode == EditPartsParameterVM_Cloned.SettingBaseVM.NumberSettingMode);
                        if (Findex != -1)
                        {
                            EditPartsParameterVM_Cloned.SettingBaseVM.StampPlateSetting = EditNumberSettingSavedCollection[Findex].StampPlateSetting;
                        }
                    }
                });
            });
        }*/

        /// <summary>
        /// 關閉後重新克隆舊資料
        /// </summary>
        public ICommand RecoverCloneByOverwritePartsParameterCommand
        {
            get => new AsyncCommand(async () =>
            {
                await Task.Delay(50);
                //var Findex = PartsParameterVMObservableCollection.FindIndex(x => x == PartsParameterViewModelSelectItem);
                // if (Findex != -1)
               // if()
                if (PartsParameterViewModelSelectItem != null && PartsParameterVMObservableCollection.Contains(PartsParameterViewModelSelectItem))
                {
                    // EditPartsParameterVM_Cloned = new PartsParameterViewModel(PartsParameterVMObservableCollection[Findex].PartsParameter.DeepCloneByJson());
                    //EditPartsParameterVM_Cloned = PartsParameterVMObservableCollection[Findex].DeepCloneByJson();
                    EditPartsParameterVM_Cloned = PartsParameterViewModelSelectItem.DeepCloneByJson();
                }
            });
        }

        public ICommand OverwritePartsParameterByCloneCommand
        {
            get => new RelayCommand(() =>
            {
                var Findex = PartsParameterVMObservableCollection.FindIndex(x => x == PartsParameterViewModelSelectItem);
                if (Findex != -1)
                {
                    //PartsParameterVMObservableCollection[Findex] = new PartsParameterViewModel(EditPartsParameterVM_Cloned.PartsParameter.DeepCloneByJson());
                    PartsParameterVMObservableCollection[Findex] = EditPartsParameterVM_Cloned.DeepCloneByJson();
                }
            });
        }

        private AsyncRelayCommand? _importProject_SelectPathFileCommand;
        /// <summary>
        /// 匯入 選擇檔案路徑
        /// </summary>
        public AsyncRelayCommand ImportProject_SelectPathFileCommand
        {
            get => _importProject_SelectPathFileCommand ??= new (async () =>
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
        }



        private AsyncRelayCommand? _importProject_SelectPathFolderCommand;
        /// <summary>
        /// 匯入 選擇資料夾路徑
        /// </summary>
        public AsyncRelayCommand ImportProject_SelectPathFolderCommand
        {
            get => _importProject_SelectPathFolderCommand ??= new (async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                {
                    System.Windows.Forms.FolderBrowserDialog sfd = new();
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ImportFilePath = sfd.SelectedPath;
                    }
                }));
            });
        }



        private string? _importFilePath;
        /// <summary>
        /// 匯入專案路徑
        /// </summary>
        public string ImportFilePath 
        { 
            get => _importFilePath??= string.Empty; 
            set 
            { 
                _importFilePath = value; 
                OnPropertyChanged(); 
            }
        }


        public class SimpleErpClass
        {
            [Index(0)]
            public string? Plate { get; set; }
            [Index(1)]
            public int Count { get; set; }
        }


        private AsyncRelayCommand? _importProjectCommand;
        /// <summary>
        /// 匯入
        /// </summary>
        public AsyncRelayCommand ImportProjectCommand
        {
            get => _importProjectCommand ??= new (async () =>
            {
                var ManagerVM = new DevExpress.Mvvm.DXSplashScreenViewModel
                {
                    Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Image/svg/NewLogo_1-2.svg"),
                    Status = (string)System.Windows.Application.Current.TryFindResource("Connection_Init"),
                    Progress = 0,
                    IsIndeterminate = true,
                    Subtitle = null,
                    Copyright = null,
                };

                var manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.ProcessingScreenWindow(), ManagerVM);
                manager.Show(Application.Current?.MainWindow, WindowStartupLocation.CenterScreen, true, InputBlockMode.None);
                try
                {
                    ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Text_Importing") + "...";
                    List<PartsParameterViewModel> importPartsParameterVMList = new();
                    await Task.Run(async () =>
                    {
                        //跳出轉圈圈
                        List<string> importFileList = new();
                        //var importFile = ImportFilePath;
                        if (File.Exists(ImportFilePath))
                        {
                            importFileList.Add(ImportFilePath);
                        }
                        else if (Directory.Exists(ImportFilePath))
                        {
                            //取得路徑下所有檔案
                            string[] files = Directory.GetFiles(ImportFilePath);
                            var availableFiles = files.Where(fileName =>
                            Path.GetExtension(fileName).Equals(".csv", StringComparison.OrdinalIgnoreCase) ||
                            Path.GetExtension(fileName).Equals(".json", StringComparison.OrdinalIgnoreCase));

                            //如果沒有檔案->跳出提示
                            if (availableFiles.Count() == 0)
                            {
                                //await MessageBoxResultShow.ShowOKAsync(null,(string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_ImportableFileNotExisted"));
                                throw new Exception((string)Application.Current.TryFindResource("Text_ImportableFileNotExisted")); ;
                            }

                            importFileList.AddRange(availableFiles);
                        }
                        else
                        {
                            //都不是
                            return;
                        }


                        int fileCount = importFileList.Count;
                        int fileProgress = 0;
                        foreach (var importFile in importFileList)
                        {
                            ManagerVM.IsIndeterminate = false;
                            ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Text_Importing") + "..." + $"{Path.GetFileName(importFile)}" + $"({fileProgress}/{fileCount})";
                            ManagerVM.Progress = (fileProgress * 100.0) / fileCount;

                            List<ERP_IronPlateModel> ErpFile = new();
                            CsvFileManager csvManager = new();
                            if (JsonHM.ReadJsonFile(importFile, out ErpFile))
                            {
                                //陣列型
                            }
                            else if (JsonHM.ReadJsonFile(importFile, out ERP_IronPlateModel ErpOneFile))
                            {
                                ErpFile = new()
                            {
                                ErpOneFile
                            };
                                //單筆
                            }
                            else if (csvManager.ReadCSVFile(importFile, out ErpFile))
                            {

                            }
                            else if (csvManager.ReadCSVFile<SimpleErpClass>(importFile, out var CsvFileWithoutHeader, false))
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
                                continue;
                            }

                            //將資料拆分為QR/一般
                            if (ErpFile.Exists(x => !x.QrCode) && ImportProjectNumberSettingBaseVM == null)
                            {
                                await MessageBoxResultShow.ShowOKAsync(null, (string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_ImportNeedSetNumberCode"), GD_MessageBoxNotifyResult.NotifyBl);
                                return;
                            }
                            if (ErpFile.Exists(x => x.QrCode) && ImportProjectQRSettingBaseVM == null)
                            {
                                await MessageBoxResultShow.ShowOKAsync(null,(string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_ImportNeedSetQrCode"), GD_MessageBoxNotifyResult.NotifyBl);
                                return;
                            }

                            //檢查字長度
                            foreach (var _erp in ErpFile)
                            {
                                //開始轉換檔案
                                SettingBaseViewModel? SettingBaseVM;
                                if (_erp.QrCode)
                                {
                                    SettingBaseVM = ImportProjectQRSettingBaseVM?.DeepCloneByJson();
                                    // SettingBaseVM.SheetStampingTypeForm = SheetStampingTypeFormEnum.QRSheetStamping;

                                    //QRcode展開

                                    if (_erp.QrCodeContent == null || _erp.QrCodeContent.Count == 0)
                                    {
                                        //沒有Content
                                        PartsParameterModel PParameter = new PartsParameterModel()
                                        {
                                            IronPlateString = _erp.PartNumber,
                                            QrCodeContent = string.Empty,
                                            QR_Special_Text = string.Empty,
                                            StampingPlate = SettingBaseVM.StampPlateSetting
                                        };
                                        importPartsParameterVMList.Add(new PartsParameterViewModel(PParameter));
                                    }
                                    else
                                    {
                                        int maxLength = _erp.QrCodeContent.CountCompare(_erp.TrainNumber);
                                        for (int i = 0; i < maxLength; i++)
                                        {
                                            try
                                            {
                                                _erp.QrCodeContent.TryGetValue(i, out var qrcode);
                                                _erp.TrainNumber.TryGetValue(i, out var trainNum);

                                                PartsParameterModel PParameter = new PartsParameterModel()
                                                {
                                                    IronPlateString = _erp.PartNumber,
                                                    QrCodeContent = _erp.QrCodeContent[i],
                                                    QR_Special_Text = _erp.TrainNumber[i],
                                                    StampingPlate = SettingBaseVM.StampPlateSetting
                                                };
                                                importPartsParameterVMList.Add(new PartsParameterViewModel(PParameter));
                                            }
                                            catch (Exception ex)
                                            {
                                                _ = LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ex.Message, true);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    SettingBaseVM = ImportProjectNumberSettingBaseVM.DeepCloneByJson();
                                    //SettingBaseVM.SheetStampingTypeForm = SheetStampingTypeFormEnum.NormalSheetStamping;

                                    PartsParameterModel PParameter = new PartsParameterModel()
                                    {
                                        IronPlateString = _erp.PartNumber,
                                        QrCodeContent = _erp.QrCodeContent?.FirstOrDefault(),
                                        QR_Special_Text = _erp.TrainNumber?.FirstOrDefault(),
                                        StampingPlate = SettingBaseVM.StampPlateSetting
                                    };
                                    importPartsParameterVMList.Add(new PartsParameterViewModel(PParameter));
                                }






                                ProductProjectEditTime = DateTime.Now;
                            }

                            fileProgress++;
                        }
                        if (importPartsParameterVMList.Count == 0)
                        {
                            // await MessageBoxResultShow.ShowOKAsync(null,(string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_ImportFail"));
                            throw new Exception((string)Application.Current.TryFindResource("Text_ImportFail"));
                        }

                        if (importPartsParameterVMList.Count == 1)
                        {
                            ManagerVM.IsIndeterminate = true;
                        }

                    });
                    ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Text_ImportFinishing");

                    await Task.Delay(1000);
                    manager?.Close();
                    //詢問是否要加入

                    var cImportNotify = (string)Application.Current.TryFindResource("Text_ImportNotifyChecked");
                    if (cImportNotify != null)
                    {
                        string Outputstring = string.Format(cImportNotify, importPartsParameterVMList.Count);
                        var ret = await MessageBoxResultShow.ShowYesNoAsync(null,(string)Application.Current.TryFindResource("Text_notify"), Outputstring, GD_MessageBoxNotifyResult.NotifyBl);
                        if (ret != MessageBoxResult.Yes && ret != MessageBoxResult.OK)
                        {
                            throw new TaskCanceledException();
                        }
                    }



                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        PartsParameterVMObservableCollection.AddRange(importPartsParameterVMList);
                        await SaveProductProjectAsync();
                    });

                    var ImportNotify = (string)Application.Current.TryFindResource("Text_ImportNotifySuceesful");
                    if (ImportNotify != null)
                    {
                        string Outputstring = string.Format(ImportNotify, importPartsParameterVMList.Count);
                        await MessageBoxResultShow.ShowOKAsync(null,(string)Application.Current.TryFindResource("Text_notify"), Outputstring, GD_MessageBoxNotifyResult.NotifyGr);
                    }
                }
                catch (TaskCanceledException tcex)
                {
                    await LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, tcex.Message);
                }
                catch (Exception ex)
                {
                    await MessageBoxResultShow.ShowOKAsync(null,(string)Application.Current.TryFindResource("Text_notify"), ex.Message , GD_MessageBoxNotifyResult.NotifyRd);
                }
                finally
                {
                    manager?.Close();
                }
            });
        }
        /// <summary>
        /// 匯入專案-加工型態 一般鐵牌
        /// </summary>
        public NumberSettingViewModel? ImportProjectNumberSettingBaseVM { get; set; }
        /// <summary>
        /// 匯入專案-加工型態 QR鐵牌
        /// </summary>
        public QRSettingViewModel? ImportProjectQRSettingBaseVM { get; set; }

       
        private PartsParameterViewModelObservableCollection? _partsParameterVMObservableCollection;
        /// <summary>
        /// GridControl ABC參數
        /// </summary>
        public PartsParameterViewModelObservableCollection PartsParameterVMObservableCollection
        {
            get
            {
                if (_partsParameterVMObservableCollection == null)
                {
                    _partsParameterVMObservableCollection = new(ProductProject.PartsParameterObservableCollection.Select(p=>new PartsParameterViewModel(p)));
                    _partsParameterVMObservableCollection.CollectionChanged -= PartsParameterVMObservableCollection_CollectionChanged;
                    _partsParameterVMObservableCollection.CollectionChanged += PartsParameterVMObservableCollection_CollectionChanged;
                    UnBoxPartsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>(_partsParameterVMObservableCollection.Where(p => p.BoxIndex == null));
                }
                return _partsParameterVMObservableCollection;
            }
            set
            {
                _partsParameterVMObservableCollection = value;
                if (_partsParameterVMObservableCollection != null)
                {
                    _partsParameterVMObservableCollection.CollectionChanged -= PartsParameterVMObservableCollection_CollectionChanged;
                    _partsParameterVMObservableCollection.CollectionChanged += PartsParameterVMObservableCollection_CollectionChanged;
                    UnBoxPartsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>(value.Where(p => p.BoxIndex == null));
                }
                OnPropertyChanged();
            }
        }


        private ObservableCollection<PartsParameterViewModel>? _unBoxPartsParameterVMObservableCollection;
        /// <summary>
        /// 還沒放進箱子內的資料
        /// </summary>
        public ObservableCollection<PartsParameterViewModel> UnBoxPartsParameterVMObservableCollection
        {
            get => _unBoxPartsParameterVMObservableCollection ??= new(PartsParameterVMObservableCollection.Where(p => p.BoxIndex == null));
            set
            {
                _unBoxPartsParameterVMObservableCollection = value;
                OnPropertyChanged();
                
            }
        }



        private void PartsParameterVMObservableCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is IEnumerable<GD_StampingMachine.ViewModels.ProductSetting.PartsParameterViewModel> collection)
            {
                ProductProject.PartsParameterObservableCollection = collection.Select(p => p.PartsParameter).ToList();
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewItems is IList newCollection)
                        {
                            var addCollection = newCollection.Cast<PartsParameterViewModel>().Except(UnBoxPartsParameterVMObservableCollection);
                            UnBoxPartsParameterVMObservableCollection.AddRange(addCollection);

                            /*foreach (var item in newCollection)
                            {

                                if (item is PartsParameterViewModel parameter)
                                {
                                    if (!UnBoxPartsParameterVMObservableCollection.Contains(parameter))
                                    {
                                        Application.Current.Invoke(() =>
                                        {
                                            UnBoxPartsParameterVMObservableCollection.Add(parameter);
                                        });
                                    }
                                }
                            }*/
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.OldItems is ICollection newCollection)
                        {
                            foreach (var item in newCollection)
                            {
                                if (item is PartsParameterViewModel parameter)
                                {
                                    if (UnBoxPartsParameterVMObservableCollection.Contains(parameter))
                                    {
                                        Application.Current.Invoke(() =>
                                        {
                                            UnBoxPartsParameterVMObservableCollection.Remove(parameter);
                                        });
                                    }

                                }
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        if (e.NewItems is ICollection newCollection && e.OldItems is ICollection oldCollection)
                        {
                            var replacements = oldCollection.OfType<PartsParameterViewModel>()
                                .Zip(newCollection.OfType<PartsParameterViewModel>(), (oldItem, newItem) => (oldItem, newItem));

                            foreach (var item in replacements)
                            {
                                if (UnBoxPartsParameterVMObservableCollection.Contains(item.oldItem))
                                    UnBoxPartsParameterVMObservableCollection.AddOrReplace(x=>x == item.oldItem, item.newItem);
                                
                                //StampingBoxPartsVM.SelectedSeparateBoxVM
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private AsyncRelayCommand? _createPartCommand;
        /// <summary>
        /// 建立零件
        /// </summary>
        [JsonIgnore]
        public AsyncRelayCommand CreatePartCommand
        {
            get => _createPartCommand ??= new AsyncRelayCommand(async () =>
            {
                await Task.Run(() =>
                {
                    List<PartsParameterViewModel> collection = new();
                    for (int i = 0; i < AddNewPartsCount; i++)
                    {
                        var partClone = AddNewPartsParameterVM.DeepCloneByJson();
                        partClone.ProductProjectName = this.ProductProjectName;
                        collection.Add(partClone);
                        ProductProjectEditTime = DateTime.Now;
                    }

                    /*   = PartsParameterVMObservableCollection.ToList();
                    tmp.*/
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PartsParameterVMObservableCollection.AddRange(collection);
                        OnPropertyChanged(nameof(PartsParameterVMObservableCollection));
                    });

                });
                await SaveProductProjectAsync();
            },()=> !CreatePartCommand.IsRunning);
           /* set
            {
                _createPartCommand = value;
                OnPropertyChanged(nameof(CreatePartCommand));
            }*/
        }

        private ICommand?_saveProductProjectCommand;
        [JsonIgnore]
        public ICommand SaveProductProjectCommand
        {
            get => _saveProductProjectCommand ??= new AsyncRelayCommand(async () =>
            {
                if (string.IsNullOrWhiteSpace(ProductProject.ProjectPath) || string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(ProductProject.ProjectPath)))
                {
                    //跳出彈跳式視窗
                    using var dialog = new System.Windows.Forms.FolderBrowserDialog();
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                        ProductProject.ProjectPath = dialog.SelectedPath;
                }
                await SaveProductProjectAsync();
            });
            set
            {
                _saveProductProjectCommand = value;
                OnPropertyChanged();
            }
        }
        public async Task<bool> SaveProductProjectAsync()
        {
            if (ProductProject.ProjectPath != null)
            {
                if (ProductProject.CreateTime == default)
                {
                    if (ProductProject.SheetStampingTypeForm == 0)
                    {
                        //Debugger.Break();
                        return false;
                    }
                }
                try
                {
                    return await JsonHM.WriteJsonFileAsync(Path.Combine(ProductProject.ProjectPath, ProductProject.Name), ProductProject);
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                Debugger.Break();
                return false;
            }
        }


        public bool SaveProductProject()
        {
            if (ProductProject.ProjectPath != null)
            {
                if (ProductProject.CreateTime == default)
                {
                    if (ProductProject.SheetStampingTypeForm == 0)
                    {
                        Debugger.Break();
                        return false;
                    }
                }

                try
                {
                    return JsonHM.WriteJsonFile(Path.Combine(ProductProject.ProjectPath, ProductProject.Name), ProductProject);
                }
                catch
                {
                    return false;
                }
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













    }

    public class ProductProjectViewModelObservableCollection : ObservableCollection<ProductProjectViewModel>
    {
        public ProductProjectViewModelObservableCollection() : base()
        {

        }
        public ProductProjectViewModelObservableCollection(List<ProductProjectViewModel> list) : base(list)
        {
            foreach(var item in list)
                item.IsMarkedChanged += Item_IsMarkedChanged;
        }
        public ProductProjectViewModelObservableCollection(IEnumerable<ProductProjectViewModel> collection) : base(collection)
        {
            foreach (var item in collection)
                item.IsMarkedChanged += Item_IsMarkedChanged;
        }


        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    if (newItem is ProductProjectViewModel item)
                    {
                        item.IsMarkedChanged += Item_IsMarkedChanged;
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    if (oldItem is ProductProjectViewModel item)
                    {
                        item.IsMarkedChanged -= Item_IsMarkedChanged;
                    }
                }
            }
            base.OnCollectionChanged(e);
        }

        private void Item_IsMarkedChanged(object? sender, bool e)
        {
            IsMarkedList = Items is null? null: this.Items.Select(x => x.IsMarked).ToList();
        }

        private IList<bool>? _isMarkedList;
        public IList<bool>? IsMarkedList
        {
            get => _isMarkedList;
            set
            {
                _isMarkedList = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsMarkedList)));
                IsMarkedListChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<IList<bool>?>? IsMarkedListChanged;
    }



}
