using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.Xpf;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ProductSetting
{

    public class ProductSettingViewModel : GD_CommonControlLibrary.BaseViewModel
    {

        StampingMachineJsonHelper JsonHM = new();

        public override string ViewModelName => (string)Application.Current.TryFindResource("Name_ProductSettingViewModel");

        private bool _addProjectDarggableIsPopup;
        public bool AddProjectDarggableIsPopup
        {
            get
            {
                return _addProjectDarggableIsPopup;
            }
            set
            {
                _addProjectDarggableIsPopup = value;
                OnPropertyChanged();
            }
        }

        private bool _openProjectFileDarggableIsPopup;
        public bool OpenProjectFileDarggableIsPopup
        {
            get
            {
                return _openProjectFileDarggableIsPopup;
            }
            set
            {
                _openProjectFileDarggableIsPopup = value;
                OnPropertyChanged();
            }
        }

        public string ProjectPathText
        {
            get => CreatedProjectVM.ProductProjectPath;
            set
            {
                if (value != null)
                {
                    value = value.Replace(@"/", @"\");
                    while (value.Contains(@"\\"))
                    {
                        value = value.Replace(@"\\", @"\");
                    }

                    while (value.Contains(@".."))
                    {
                        value = value.Replace(@".", @".");
                    }
                }


                CreatedProjectVM.ProductProjectPath = value;
                OnPropertyChanged(nameof(ProjectPathText));
            }
        }





        private ObservableCollection<ProductProjectViewModel> _productProjectVMObservableCollection;
        /// <summary>
        /// 排版專案
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get
            {
                return _productProjectVMObservableCollection ??= new ObservableCollection<ProductProjectViewModel>();
            }
            set
            {
                _productProjectVMObservableCollection = value;
                OnPropertyChanged(nameof(ProductProjectVMObservableCollection));
            }
        }

        [JsonIgnore]
        public ICommand SetProjectFolder
        {
            get => new RelayCommand<object>(obj =>
            {
                var sfd = new System.Windows.Forms.SaveFileDialog()
                {
                    Filter = "Json files (*.json)|*.json",
                    FileName = CreatedProjectVM.ProductProjectName
                };

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    CreatedProjectVM.ProductProjectName = Path.GetFileNameWithoutExtension(sfd.FileName);
                    CreatedProjectVM.ProductProjectPath = Path.GetDirectoryName(sfd.FileName);
                }
            });
        }
        [JsonIgnore]
        public AsyncRelayCommand CreateProjectCommand
        {
            get => new (async () =>
            {
                await Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, (string)Application.Current.TryFindResource("btnAddProject"));
                var ExistedIndex = ProductProjectVMObservableCollection.FindIndex(x => x.ProductProjectName == CreatedProjectVM.ProductProjectName);
                //檔案已存在 詢問是否要覆蓋
                if (ExistedIndex != -1)
                {
                    if (!await MethodWinUIMessageBox.AskOverwriteOrNotAsync())
                        return;
                }

                if (await CreatedProjectVM.SaveProductProjectAsync())
                {
                    //若不clone會導致資料互相繫結
                    if (ExistedIndex != -1)
                    {
                        ProductProjectVMObservableCollection[ExistedIndex] = CreatedProjectVM.DeepCloneByJson();
                    }
                    else
                    {
                        ProductProjectVMObservableCollection.Add(CreatedProjectVM.DeepCloneByJson());
                    }
                    await SaveProductListSettingAsync();
                }
            }, () => !CreateProjectCommand.IsRunning);
        }

        /// <summary>
        /// 新建專案
        /// </summary>
        [JsonIgnore]
        public ProductProjectViewModel CreatedProjectVM { get; } = new ProductProjectViewModel(new ProductProjectModel()
        {
            Name = "NewProject",
            Number = "newAS001",
            CreateTime = DateTime.Now
        });

        [JsonIgnore]
        public Array SheetStampingTypeEnumCollection
        {
            get
            {
                return new SheetStampingTypeFormEnum[]
                {
                    SheetStampingTypeFormEnum.NormalSheetStamping, SheetStampingTypeFormEnum.QRSheetStamping
                };
                //return System.Enum.GetValues(typeof(SheetStampingTypeFormEnum));
            }
        }


        
        private DevExpress.Mvvm.DelegateCommand<DevExpress.Mvvm.Xpf.RowClickArgs>? _rowDoubleClickCommand;
        [JsonIgnore]
        public ICommand RowDoubleClickCommand
        {
            get => _rowDoubleClickCommand ??= new DevExpress.Mvvm.DelegateCommand<DevExpress.Mvvm.Xpf.RowClickArgs>((DevExpress.Mvvm.Xpf.RowClickArgs args) =>
            {
                _ = Task.Run(() =>
                {
                    if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel ProjectItem)
                    {
                        if (SelectProductProjectVM != ProjectItem)
                            SelectProductProjectVM = ProjectItem;
                        SelectProductProjectVM.IsInParameterPage = true;
                    }
                });
            },args =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel ProjectItem)
                {
                    return !ProjectItem.FileIsNotExisted;
                }
                return false;
            });
        }

        private ICommand? _loadProductSettingCommand;
        [JsonIgnore]
        public ICommand LoadProductSettingCommand
        {
            get => _loadProductSettingCommand ??= new AsyncRelayCommand(async () =>
            {
                try
                {
                    if (JsonHM.ManualReadProductProject(out ProductProjectModel ReadProductProject))
                    {
                        var ExisedIndex = ProductProjectVMObservableCollection.FindIndex(x => x.ProductProjectName == ReadProductProject.Name);
                        if (ExisedIndex != -1)
                        {
                            if (ProductProjectVMObservableCollection[ExisedIndex].ProductProjectPath == ReadProductProject.ProjectPath)
                                await MethodWinUIMessageBox.ProjectIsLoadedAsync();
                            else
                                await MethodWinUIMessageBox.ProjectIsExisted_CantOpenProjectAsync();
                            return;
                        }

                       // if (ReadProductProject.CreateTime != default)
                        {
                            //已完成的專案

                            if (!IsShowAllProject)
                            {
                                if (ReadProductProject.ProductProjectIsFinish)
                                    IsShowHiddenProject = true;
                                else
                                    IsShowHiddenProject = false;

                                //OnPropertyChanged(nameof(ProductProjectRowFilterCommand));
                            }

                            ProductProjectVMObservableCollection.Add(new ProductProjectViewModel(ReadProductProject));
                            await SaveProductListSettingAsync();
                        }





                    }
                }
                catch (Exception ex)
                {
                    await MessageBoxResultShow.ShowOKAsync(ViewModelName, ex.Message , GD_MessageBoxNotifyResult.NotifyRd);
                    Debugger.Break();
                }
            });
        }



        private ICommand? _reLoadProductSettingCommand;
        public ICommand ReLoadProductSettingCommand
        {
            get => _reLoadProductSettingCommand ??= new AsyncRelayCommand<object>(async (obj) =>
            {
                if (obj is ProductProjectViewModel productProjectVM)
                {
                   
                    if (JsonHM.ManualReadProductProject(productProjectVM.ProductProjectName,out ProductProjectModel ReadProductProject))
                    {
                        //如果檔案已存在 跳過
                        var existedProject = ProductProjectVMObservableCollection.FirstOrDefault(x => x.ProductProjectName == ReadProductProject.Name);
                        if (existedProject != null)
                        {
                            //+ existedProject.ProductProjectName
                            await MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                                      (string)Application.Current.TryFindResource("Text_ProjectIsExistedCantOpenProject"), GD_MessageBoxNotifyResult.NotifyRd);
                            return;
                        }


                        var index = ProductProjectVMObservableCollection.FindIndex(x => x == productProjectVM);
                        if (index != -1)
                        {


                            ProductProjectVMObservableCollection[index] = new ProductProjectViewModel(ReadProductProject);
                            await this.SaveProductListSettingAsync();

                            await MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                                          (string)Application.Current.TryFindResource("Text_LoadSuccessful") , GD_MessageBoxNotifyResult.NotifyGr);
                        }
                    }
                }

            });
        }




        private AsyncRelayCommand? _saveProductSettingCommand;
        [JsonIgnore]
        public AsyncRelayCommand SaveProductSettingCommand
        {
            get => _saveProductSettingCommand ??= new (async () =>
            {
                var saveList = ProductProjectVMObservableCollection.Select(obj => obj.SaveProductProjectAsync());
                await Task.WhenAll(saveList);

                var Result = await SaveProductListSettingAsync();
                await MethodWinUIMessageBox.SaveSuccessfulAsync(null, Result);
            });
        }



        private ICommand?_saveProductListSettingCommand;
        [JsonIgnore]
        public ICommand SaveProductListSettingCommand
        {
            get => _saveProductListSettingCommand ??= new AsyncRelayCommand(async () =>
            {
                await SaveProductListSettingAsync();
            });
        }

        private async Task<bool> SaveProductListSettingAsync()
        {
            //儲存一份路徑清單
            List<ProjectModel> PathList = new List<ProjectModel>();
            //將所有檔案儲存
            ProductProjectVMObservableCollection.ForEach(obj =>
            {
                PathList.Add(new ProjectModel
                {
                    Name = obj.ProductProjectName,
                    Number = obj.ProductProjectNumber,
                    ProjectPath = obj.ProductProjectPath
                });
            });

            return await JsonHM.WriteProjectSettingJsonAsync(PathList);
        }





        private ProductProjectViewModel? _selectProductProjectVM;
        /// <summary>
        /// 新增零件的vm
        /// </summary>

        public ProductProjectViewModel SelectProductProjectVM
        {
            get => _selectProductProjectVM ??= new ProductProjectViewModel(new ProductProjectModel());
            set
            {
                _selectProductProjectVM = value;
                OnPropertyChanged();

            }
        }



        private bool _isShowHiddenProject;
        private bool _isShowAllProject;

        public bool IsShowHiddenProject { get => _isShowHiddenProject; set { _isShowHiddenProject = value; OnPropertyChanged(); } }
        public bool IsShowAllProject { get => _isShowAllProject; set { _isShowAllProject = value; OnPropertyChanged(); } }


        public ICommand _updateFiltrationLogicCommand;
        public ICommand UpdateFiltrationLogicCommand
        {
            get => _updateFiltrationLogicCommand ??= new RelayCommand(() =>
            {
                UpdateFiltrationLogic();
            });
        }

        void UpdateFiltrationLogic()
        {
            OnPropertyChanged(nameof(ProductProjectRowFilterCommand));
        }



        // private DevExpress.Mvvm.DelegateCommand<RowFilterArgs> _productProjectRowFilterCommand;
        //篩選器
        [JsonIgnore]
        public ICommand ProductProjectRowFilterCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<RowFilterArgs>(args =>
            {
                if (args?.Item is GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel PProject)
                {
                    if (IsShowAllProject)
                    {
                        args.Visible = true;
                    }
                    else
                    {
                        if (IsShowHiddenProject)
                            args.Visible = PProject.ProductProjectIsFinish;
                        else
                            args.Visible = !PProject.ProductProjectIsFinish;
                    }
                }
            });
        }








        /*

        [JsonIgnore]
        public ICommand DragDropCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                if (obj is DragEventArgs e)
                {
                    e.Effects = System.Windows.DragDropEffects.Copy;
                }
            });
        }




        [JsonIgnore]
        public ICommand PreviewDragOverCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                if (obj is DragEventArgs e)
                {
                    e.Effects = System.Windows.DragDropEffects.Copy;
                }
            });
        }



        [JsonIgnore]
        public ICommand PreviewDragEnterCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                if (obj is DragEventArgs e)
                {
                    e.Effects = System.Windows.DragDropEffects.Copy;
                }
            });
        }

        */





    }
}
