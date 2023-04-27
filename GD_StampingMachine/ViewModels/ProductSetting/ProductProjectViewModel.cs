﻿using DevExpress.Data;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using GD_StampingMachine.ViewModels.ParameterSetting;
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

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    public class ProductProjectViewModel : BaseViewModelWithLog
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_ProductProjectViewModel");
        public ProductProjectViewModel(ProductProjectModel ProductProject)
        {
            _productProject = ProductProject;
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
                catch (Exception ex)
                {

                }
            });
        }

        private readonly ProductProjectModel _productProject = new();



        public SheetStampingTypeFormEnum ProductProjectSheetStampingTypeForm
        {
            get => _productProject.SheetStampingTypeForm;
            set
            {
                _productProject.SheetStampingTypeForm = value;
                OnPropertyChanged(nameof(ProductProjectSheetStampingTypeForm));
            }
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



        private RelayParameterizedCommand _projectEditCommand;
        public RelayParameterizedCommand ProjectEditCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                EditProjectDarggableIsPopup= true;


            });
        }
        // private RelayParameterizedCommand _projectDeleteCommand;
        public RelayParameterizedCommand ProjectDeleteCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                if (obj is GridControl ObjGridControl)
                {
                    if (ObjGridControl.ItemsSource is ObservableCollection<ProductProjectViewModel> GridItemSource)
                    {
                        if (ObjGridControl.CurrentItem is ProductProjectViewModel CurrentItem)
                        {
                            if (MethodWinUIMessageBox.AskDelProject($"{CurrentItem._productProject.Number} - {CurrentItem._productProject.Name}"))
                                GridItemSource.Remove(CurrentItem);
                        }
                    }
                }
            });
        }








        private bool _isInParameterPage;
        public bool IsInParameterPage { get => _isInParameterPage; set { _isInParameterPage = value; OnPropertyChanged(); } }

        private PartsParameterViewModel _addNewPartsParameterVM;
        public PartsParameterViewModel AddNewPartsParameterVM
        {
            get
            {
                if(_addNewPartsParameterVM == null)
                    _addNewPartsParameterVM = new PartsParameterViewModel(new Model.ProductionSetting.PartsParameterModel() { ProjectID = ProductProjectName });
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
            if (_productProject != null)
            {
                if (_productProject.SheetStampingTypeForm == SheetStampingTypeFormEnum.NormalSheetStamping)
                {
                    JsonHM.ReadNumberSetting(out var SavedCollection);
                    if (SavedCollection != null)
                        foreach (var asd in SavedCollection)
                            newSavedCollection.Add(new NumberSettingViewModel(asd));

                }
                if (_productProject.SheetStampingTypeForm == SheetStampingTypeFormEnum.QRSheetStamping)
                {
                    JsonHM.ReadQRNumberSetting(out var QRSavedCollection);
                    if (QRSavedCollection != null)
                        foreach (var asd in QRSavedCollection)
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
                    _partsParameterVMObservableCollection = new();
                _productProject.PartsParameterObservableCollection =  _partsParameterVMObservableCollection.Select(x => x.PartsParameter).ToObservableCollection();
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
        }
        public bool SaveProductProject()
        {
            if (_productProject.ProjectPath != null)
            {
                JsonHM.WriteProductProject(_productProject);
            }
            else
            {
                Debugger.Break();
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



        /// <summary>
        /// 新增排版專案
        /// </summary>
        public ICommand AddTypeSettingCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                if (obj == null)
                {
                    throw new Exception();
                }

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
        public ICommand CloseTypeSettingCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                if (obj == null)
                {
                    throw new Exception();
                }

                if (obj is object[] objectArray)
                {
                    if (objectArray.Count() == 3)
                    {
                        DevExpress.Xpf.Grid.GridControl GridControlSource = objectArray[0] as DevExpress.Xpf.Grid.GridControl;
                        DevExpress.Xpf.Grid.GridControl GridControlTarget = objectArray[1] as DevExpress.Xpf.Grid.GridControl;
                        string ProjectDistributeName =( objectArray[2] as TextBlock).Text;
                        if (GridControlSource != null && GridControlTarget != null)
                        {
                            if (GridControlSource.CurrentItem is ProductProjectViewModel _currentItem &&
                            GridControlSource.ItemsSource is ObservableCollection<GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel> SourceItemS &&
                            GridControlTarget.ItemsSource is ObservableCollection<GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel> TargetItemS)
                            {
                              //  PartsParameterVMObservableCollectionRefresh
                                var CollectionWithThisDistributeName = _currentItem.PartsParameterVMObservableCollection.Where(x => x.DistributeName == ProjectDistributeName);

                                //箱子內有專案
                                if (CollectionWithThisDistributeName.Count() > 0)
                                {
                                    //有已完成的 不可關閉
                                    if (CollectionWithThisDistributeName.ToList().Exists(x => x.MachiningStatus == MachiningStatusEnum.Finish))
                                    {
                                        MethodWinUIMessageBox.CanNotCloseProject();
                                        return;
                                    }

                                    //詢問是否要關閉
                                    if (!MethodWinUIMessageBox.AskCloseProject(_currentItem.ProductProjectName))
                                        return;

                                    //將資料清除
                                    CollectionWithThisDistributeName.ForEach(Eobj =>
                                    {
                                        Eobj.DistributeName = null;
                                        Eobj.BoxIndex = null;
                                    });
                                }
                                TargetItemS.Add(_currentItem);
                                SourceItemS.Remove(_currentItem);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            });
        }

        public ICommand BoxPartsParameterVMObservableCollectionRefreshCommand { get; set; }





    }
}
