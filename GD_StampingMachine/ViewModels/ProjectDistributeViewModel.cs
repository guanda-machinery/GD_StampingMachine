using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Utils.Extensions;
using DevExpress.Xpf.Grid;
using GD_StampingMachine;
using GD_StampingMachine.Extensions;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using GD_StampingMachine.Model.ProductionSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{

    public class ProjectDistributeModel
    {
        /// <summary>
        /// 製品專案名稱
        /// </summary>
        public string ProjectDistributeName { get; set; }
        /// <summary>
        /// 製品專案建立時間
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 製品編輯時間
        /// </summary>
        public DateTime? EditDate { get; set; }
        /// <summary>
        /// 製品清單
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get; set; }
        /// <summary>
        /// 盒子
        /// </summary>
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; }
    }


    public class ProjectDistributeViewModel : BaseViewModelWithLog
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_ProjectDistributeViewModel");

        public ProjectDistributeViewModel(ProjectDistributeModel _projectDistribute)
        {
            ProjectDistribute = _projectDistribute;
            StampingBoxPartsVM = new StampingBoxPartsViewModel(new StampingBoxPartModel
            {
                ProjectDistributeName = ProjectDistribute.ProjectDistributeName,
                SeparateBoxVMObservableCollection = _projectDistribute.SeparateBoxVMObservableCollection,
                ProductProjectVMObservableCollection = _projectDistribute.ProductProjectVMObservableCollection,
                Box_OnDropRecordCommand = this.Box_OnDropRecordCommand,
                Box_OnDragRecordOverCommand = this.Box_OnDragRecordOverCommand,
                GridControl_MachiningStatusColumnVisible = false
            });

            NotReadyToTypeSettingProductProjectVMObservableCollection = new ObservableCollection<ProductProjectViewModel>();
            ProductProjectVMObservableCollection.ForEach(obj =>
            {
                if (ReadyToTypeSettingProductProjectVMObservableCollection.FindIndex(x => x == obj) == -1)
                    NotReadyToTypeSettingProductProjectVMObservableCollection.Add(obj);
            });

        }

        public ProjectDistributeModel ProjectDistribute { get; } = new ProjectDistributeModel();

        public string ProjectDistributeName { get => ProjectDistribute.ProjectDistributeName; set { ProjectDistribute.ProjectDistributeName = value; OnPropertyChanged(); } }
        public DateTime CreatedDate { get => ProjectDistribute.CreatedDate; set { ProjectDistribute.CreatedDate = value; OnPropertyChanged(); } }
        public DateTime? EditDate { get => ProjectDistribute.EditDate; set { ProjectDistribute.EditDate = value; OnPropertyChanged(); } }


        public StampingBoxPartsViewModel StampingBoxPartsVM { get; set; }

        public ICommand RefreshStampingBoxCommand
        {
            get => StampingBoxPartsVM.BoxPartsParameterVMObservableCollectionRefreshCommand;
        }



        private bool _IsInDistributePage = false;
        public bool IsInDistributePage { get => _IsInDistributePage; set { _IsInDistributePage = value; OnPropertyChanged(); } }

        private bool _addTypeSettingProjectDarggableIsPopUp;
        public bool AddTypeSettingProjectDarggableIsPopUp
        {
            get => _addTypeSettingProjectDarggableIsPopUp;
            set
            {
                _addTypeSettingProjectDarggableIsPopUp = value;
                OnPropertyChanged();
            }
        }




        //  public string ProjectDistributeName { get => ProjectDistribute.ProjectDistributeName; }



        // private ObservableCollection<ProductProjectViewModel> _productProjectVMObservableCollection;
        private ProductProjectViewModel _readyToTypeSettingProductProjectVMSelected;
        public ProductProjectViewModel ReadyToTypeSettingProductProjectVMSelected
        {
            get => _readyToTypeSettingProductProjectVMSelected;
            set
            {
                _readyToTypeSettingProductProjectVMSelected = value;
                OnPropertyChanged(nameof(ReadyToTypeSettingProductProjectVMSelected));
                PartsParameterVMObservableCollectionRefresh();
            }
        }

        public void PartsParameterVMObservableCollectionRefresh()
        {
            if (ReadyToTypeSettingProductProjectVMSelected != null)
            {
                PartsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();
                ReadyToTypeSettingProductProjectVMSelected.PartsParameterVMObservableCollection.Where(x => x.BoxNumber == null && x.DistributeName == null).ForEach(obj =>
                {
                    PartsParameterVMObservableCollection.Add(obj);
                });
            }

            ProductProjectVMObservableCollection.ForEach(obj =>
            {
                if (!ReadyToTypeSettingProductProjectVMObservableCollection.Contains(obj))
                    if (!NotReadyToTypeSettingProductProjectVMObservableCollection.Contains(obj))
                        NotReadyToTypeSettingProductProjectVMObservableCollection.Add(obj);
            });
        }




        /// <summary>
        /// 全製品專案
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get
            {
                if (ProjectDistribute.ProductProjectVMObservableCollection == null)
                    ProjectDistribute.ProductProjectVMObservableCollection = new ObservableCollection<ProductProjectViewModel>();
                return ProjectDistribute.ProductProjectVMObservableCollection;
            }
            set
            {
                ProjectDistribute.ProductProjectVMObservableCollection = value;
                OnPropertyChanged();
            }
        }


        private ProductProjectViewModel _notReadyToTypeSettingProductProjectVMSelected;
        public ProductProjectViewModel NotReadyToTypeSettingProductProjectVMSelected
        {
            get=> _notReadyToTypeSettingProductProjectVMSelected; 
            set 
            { 
                _notReadyToTypeSettingProductProjectVMSelected = value;
                OnPropertyChanged();
            } 
        }


        public DevExpress.Mvvm.ICommand<DevExpress.Mvvm.Xpf.RowClickArgs> NotReadyRowDoubleClickCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<DevExpress.Mvvm.Xpf.RowClickArgs>((DevExpress.Mvvm.Xpf.RowClickArgs args) =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel ProjectItem)
                {
                    ProjectItem.IsMarked = !ProjectItem.IsMarked;
                }
            });
        }


        private ObservableCollection<ProductProjectViewModel> _notreadyToTypeSettingProductProjectVMObservableCollection =new();
        public ObservableCollection<ProductProjectViewModel> NotReadyToTypeSettingProductProjectVMObservableCollection
        {
            get
            {
                return _notreadyToTypeSettingProductProjectVMObservableCollection;
            }
            set
            {
                _notreadyToTypeSettingProductProjectVMObservableCollection = value; 
                OnPropertyChanged();
            }
        }


       private ObservableCollection<ProductProjectViewModel> _readyToTypeSettingProductProjectVMObservableCollection = new();
        /// <summary>
        /// 篩選後的專案
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ReadyToTypeSettingProductProjectVMObservableCollection
        {
            get => _readyToTypeSettingProductProjectVMObservableCollection;
            set
            {
                _readyToTypeSettingProductProjectVMObservableCollection = value;
                OnPropertyChanged();
            }
        }


        public ICommand MoveNotReadyToReadyCommand
        {
            get => new RelayCommand(() =>
            {
                var Index = -1;
                do
                {
                    Index = NotReadyToTypeSettingProductProjectVMObservableCollection.FindIndex(x=>x.IsMarked);
                    if(Index !=-1)
                    {
                        ReadyToTypeSettingProductProjectVMObservableCollection.Add(NotReadyToTypeSettingProductProjectVMObservableCollection[Index]);
                        NotReadyToTypeSettingProductProjectVMObservableCollection.RemoveAt(Index);
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            });
        }






        private ObservableCollection<PartsParameterViewModel> _partsParameterVMObservableCollection;
        /// <summary>
        /// GridControl ABC參數 沒放進箱子內的
        /// </summary>
        public ObservableCollection<PartsParameterViewModel> PartsParameterVMObservableCollection
        {
            get
            {
                _partsParameterVMObservableCollection ??= new ObservableCollection<PartsParameterViewModel>();
                return _partsParameterVMObservableCollection;
            }
            set
            {
                _partsParameterVMObservableCollection = value;
                OnPropertyChanged(nameof(PartsParameterVMObservableCollection));
            }
        }




        /// <summary>
        /// 盒子列表
        /// </summary>
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection
        {
            get => ProjectDistribute.SeparateBoxVMObservableCollection;
            set
            {
                ProjectDistribute.SeparateBoxVMObservableCollection = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 選擇盒子觸發的行為
        /// </summary>
        /*public ICommand SeparateBoxVMObservableCollectionelectionChangedCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                if (obj is System.Windows.Controls.SelectionChangedEventArgs e)
                {
                    if (e.AddedItems.Count > 0)
                    {
                        if (e.AddedItems[0] is GD_StampingMachine.ViewModels.ParameterSetting.SeparateBoxViewModel NewSeparateBoxVM)
                        {
                            BoxPartsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();
                            ProductProjectVMObservableCollection.ForEach(productProject =>
                            {
                                productProject.PartsParameterVMObservableCollection.ForEach((productProjectPartViewModel) =>
                                {
                                    if (productProjectPartViewModel.BoxNumber.HasValue)
                                        if (NewSeparateBoxVM.BoxNumber == productProjectPartViewModel.BoxNumber.Value && productProjectPartViewModel.DistributeName == this.ProjectDistributeName)
                                            if (!BoxPartsParameterVMObservableCollection.Contains(productProjectPartViewModel))
                                                BoxPartsParameterVMObservableCollection.Add(productProjectPartViewModel);
                                });
                            });
                        }
                    }
                    
                }
             });
        }*/
        








        /// <summary>
        /// 從箱子拿出來  
        /// </summary>
        public ICommand NoneBox_OnDropRecordCommand
        {
            get
            {
                return new RelayParameterizedCommand(obj =>
                {
                    if (obj is DevExpress.Xpf.Core.DropRecordEventArgs e)
                    {
                        var DragDropData = e.Data.GetData(typeof(DevExpress.Xpf.Core.RecordDragDropData)) as DevExpress.Xpf.Core.RecordDragDropData;
                        foreach (var _record in DragDropData.Records)
                        {
                            if (_record is PartsParameterViewModel PartsParameterVM)
                            {
                                PartsParameterVM.DistributeName = null;
                                PartsParameterVM.BoxNumber = null;
                                e.Effects = System.Windows.DragDropEffects.Move;
                            }
                        }

                    }
                });
            }
        }

        public ICommand NoneBox_OnDragRecordOverCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                if (obj is DevExpress.Xpf.Core.DragRecordOverEventArgs e)
                {
                    e.Effects = System.Windows.DragDropEffects.None;
                    if (e.Data.GetData(typeof(DevExpress.Xpf.Core.RecordDragDropData)) is DevExpress.Xpf.Core.RecordDragDropData DragDropData)
                    {
                        foreach (var _record in DragDropData.Records)
                        {
                            if (_record is PartsParameterViewModel PartsParameterVM)
                            {
                                if (ReadyToTypeSettingProductProjectVMSelected != null)
                                {
                                    if (PartsParameterVM.ProjectName != ReadyToTypeSettingProductProjectVMSelected.ProductProjectName)
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        e.Effects = System.Windows.DragDropEffects.Move;
                    }
                }
            });
        }


        /// <summary>
        /// 丟入盒子內
        /// </summary>
        public ICommand Box_OnDropRecordCommand
        {
            get
            {
                return new RelayParameterizedCommand(obj =>
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
                                        PartsParameterVM.DistributeName = ProjectDistributeName;// ProjectDistribute.ProjectDistributeName;
                                        PartsParameterVM.BoxNumber = StampingBoxPartsVM.SelectedSeparateBoxVM.BoxNumber;
                                        e.Effects = System.Windows.DragDropEffects.Move;
                                    }
                                }
                            }
                        }
                    }

                });
            }
        }

        public ICommand Box_OnDragRecordOverCommand
        {
            get
            {
                return new RelayParameterizedCommand(obj =>
                {
                    if (obj is DevExpress.Xpf.Core.DragRecordOverEventArgs e)
                    {
                        e.Effects = System.Windows.DragDropEffects.None;
                        if (e.Data.GetData(typeof(DevExpress.Xpf.Core.RecordDragDropData)) is DevExpress.Xpf.Core.RecordDragDropData DragDropData)
                        { 
                            //if (StampingBoxPartsVM.SelectedSeparateBoxVM != null)
                            e.Effects = System.Windows.DragDropEffects.Move;
                        }
                     

                    }
                });
            }
        }



    }
}
