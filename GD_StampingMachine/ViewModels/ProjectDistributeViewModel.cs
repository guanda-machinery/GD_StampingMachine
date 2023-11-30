using DevExpress.CodeParser;
using DevExpress.Xpf.Grid;
using GD_StampingMachine;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.Method;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.GD_Model.ProductionSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using GD_CommonLibrary;
using Newtonsoft.Json;
using DevExpress.Mvvm.Native;
using DevExpress.Data.Extensions;
using CommunityToolkit.Mvvm.Input;

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


        public ObservableCollection<string> ProductProjectNameList{get;set;} = new ObservableCollection<string>();

        /// <summary>
        /// 製品清單
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get; set; }
        /// <summary>
        /// 盒子
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; }




        /// <summary>
        /// 鋼捲位置
        /// </summary>
        public double StripSteelPosition = 0;
        /// <summary>
        /// 鋼捲長度
        /// </summary>
        public double StripSteelLength = 300000;


    }

    /// <summary>
    /// 加工專案
    /// </summary>
    public class ProjectDistributeViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_ProjectDistributeViewModel");

        public ProjectDistributeViewModel(ProjectDistributeModel _projectDistribute)
        {
            if (_projectDistribute == null)
                _projectDistribute = new ProjectDistributeModel();
            ProjectDistribute = _projectDistribute;

            StampingBoxPartsVM = new StampingBoxPartsViewModel(new StampingBoxPartModel()
            {
                ProjectDistributeName = this.ProjectDistributeName,
                ProductProjectVMObservableCollection = this.ProductProjectVMObservableCollection,
                SeparateBoxVMObservableCollection = this.SeparateBoxVMObservableCollection,
                GridControl_MachiningStatusColumnVisible = true,
            });

            if (_projectDistribute.ProductProjectNameList != null)
            {
                foreach (var PName in _projectDistribute.ProductProjectNameList)
                {
                    var Index = ProductProjectVMObservableCollection.FindIndex(x => x.ProductProjectName == PName);
                    if (Index != -1)
                    {
                        if (!ReadyToTypeSettingProductProjectVMObservableCollection.Contains(ProductProjectVMObservableCollection[Index]))
                            ReadyToTypeSettingProductProjectVMObservableCollection.Add(ProductProjectVMObservableCollection[Index]);
                    }
                }
            }
            foreach (var obj in ProductProjectVMObservableCollection)
            {
                var Index = obj.PartsParameterVMObservableCollection.FindIndex(x => x.DistributeName == this.ProjectDistributeName);
                if (Index != -1)
                {
                    if(!ReadyToTypeSettingProductProjectVMObservableCollection.Contains(obj))
                        ReadyToTypeSettingProductProjectVMObservableCollection.Add(obj);
                }
            }
            foreach (var obj in ProductProjectVMObservableCollection)
            {
                if (!ReadyToTypeSettingProductProjectVMObservableCollection.Contains(obj))
                    if (!NotReadyToTypeSettingProductProjectVMObservableCollection.Contains(obj))
                        NotReadyToTypeSettingProductProjectVMObservableCollection.Add(obj);
            }
        }
        /// <summary>
        /// 資料結構
        /// </summary>
        public ProjectDistributeModel ProjectDistribute { get; private set; } = new ProjectDistributeModel();


        internal async Task SaveProductProjectVMObservableCollectionAsync()
        {
            var saveTask = ProductProjectVMObservableCollection.Select(x => x.SaveProductProjectAsync());
            await Task.WhenAll(saveTask);
        }




        public string ProjectDistributeName { get => ProjectDistribute.ProjectDistributeName; set { ProjectDistribute.ProjectDistributeName = value; OnPropertyChanged(); } }
        public DateTime CreatedDate { get => ProjectDistribute.CreatedDate; set { ProjectDistribute.CreatedDate = value; OnPropertyChanged(); } }
        public DateTime? EditDate { get => ProjectDistribute.EditDate; set { ProjectDistribute.EditDate = value; OnPropertyChanged(); } }


 
        private StampingBoxPartsViewModel _stampingBoxPartsVM;
        /// <summary>
        /// 盒子與專案
        /// </summary>
        public StampingBoxPartsViewModel StampingBoxPartsVM 
        {
            get=> _stampingBoxPartsVM; 
            set 
            { 
                _stampingBoxPartsVM = value;
                OnPropertyChanged(); 
            } 
        }




        /// <summary>
        /// 鋼捲位置
        /// </summary>
        public double StripSteelPosition
        {
            get => ProjectDistribute.StripSteelPosition; set { ProjectDistribute. StripSteelPosition = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 鋼捲長度
        /// </summary>
        public double StripSteelLength
        {
            get => ProjectDistribute.StripSteelLength; set { ProjectDistribute.StripSteelLength = value; OnPropertyChanged(); }
        }







        private bool _IsInDistributePage = false;
        [JsonIgnore]
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



        private ProductProjectViewModel _notReadyToTypeSettingProductProjectVMCurrentItem;
        public ProductProjectViewModel NotReadyToTypeSettingProductProjectVMCurrentItem
        {
            get => _notReadyToTypeSettingProductProjectVMCurrentItem;
            set
            {
                _notReadyToTypeSettingProductProjectVMCurrentItem = value;
                OnPropertyChanged();
                //PartsParameterVMObservableCollectionRefresh();
            }
        }

        private ProductProjectViewModel _readyToTypeSettingProductProjectVMCurrentItem;
        public ProductProjectViewModel ReadyToTypeSettingProductProjectVMCurrentItem
        {
            get => _readyToTypeSettingProductProjectVMCurrentItem;
            set
            {
                _readyToTypeSettingProductProjectVMCurrentItem = value;
                OnPropertyChanged(nameof(ReadyToTypeSettingProductProjectVMCurrentItem));
                //PartsParameterVMObservableCollectionRefresh();
            }
        }

        
        // private ObservableCollection<ProductProjectViewModel> _productProjectVMObservableCollection;
        private ProductProjectViewModel _readyToTypeSettingProductProjectVMSelected;
        public ProductProjectViewModel ReadyToTypeSettingProductProjectVMSelected
        {
            get => _readyToTypeSettingProductProjectVMSelected;
            set
            {
                _readyToTypeSettingProductProjectVMSelected = value;
                OnPropertyChanged();
                PartsParameterVMObservableCollectionRefresh();
            }
        }

        [JsonIgnore]
        public ICommand ReloadTypeSettingSettingsCommand
        {
            get => new RelayCommand(() =>
            {
                PartsParameterVMObservableCollectionRefresh();
            });
        }

        public void PartsParameterVMObservableCollectionRefresh()
        {
            PartsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();
            if (ReadyToTypeSettingProductProjectVMSelected != null)
            {
                var NotInBoxList = ReadyToTypeSettingProductProjectVMSelected.PartsParameterVMObservableCollection.ToList().FindAll(x => x.BoxIndex == null && x.DistributeName == null);

                foreach (var obj in NotInBoxList)
                {
                    PartsParameterVMObservableCollection.Add(obj);
                }
            }

            foreach (var obj in ProductProjectVMObservableCollection)
            {
                if (!ReadyToTypeSettingProductProjectVMObservableCollection.Contains(obj))
                {
                    if (!NotReadyToTypeSettingProductProjectVMObservableCollection.Contains(obj))
                    {
                        //被設為完成的不可加入
                        if (obj.ProductProjectIsFinish)
                        {

                        }
                        else
                        {
                            NotReadyToTypeSettingProductProjectVMObservableCollection.Add(obj);
                        }
                    }
                }
            }

            //將被刪除的專案清除
            var DelReadyList = ReadyToTypeSettingProductProjectVMObservableCollection.Except(ProductProjectVMObservableCollection).ToList();
            var DelNotReadyList = NotReadyToTypeSettingProductProjectVMObservableCollection.Except(ProductProjectVMObservableCollection).ToList();

            DelReadyList.ForEach(del =>
            {
                ReadyToTypeSettingProductProjectVMObservableCollection.Remove(del);
            });
            DelNotReadyList.ForEach(del =>
            {
                NotReadyToTypeSettingProductProjectVMObservableCollection.Remove(del);
            });



            int BFinishindex = 0;
            while ((BFinishindex = NotReadyToTypeSettingProductProjectVMObservableCollection.FindIndex(x => x.ProductProjectIsFinish))!=-1)
            {
                NotReadyToTypeSettingProductProjectVMObservableCollection.RemoveAt(BFinishindex);
            }


        }




        /// <summary>
        /// 全製品專案
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get
            {
                return ProjectDistribute.ProductProjectVMObservableCollection ??= new ObservableCollection<ProductProjectViewModel>();
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

        [JsonIgnore]
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
        [JsonIgnore]
        public ObservableCollection<ProductProjectViewModel> ReadyToTypeSettingProductProjectVMObservableCollection
        {
            get => _readyToTypeSettingProductProjectVMObservableCollection;
            set
            {
                _readyToTypeSettingProductProjectVMObservableCollection = value;
                OnPropertyChanged();
            }
        }



        [JsonIgnore]
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
                        this.ProjectDistribute.ProductProjectNameList.Add(NotReadyToTypeSettingProductProjectVMObservableCollection[Index].ProductProjectName);
                      //  this.ProductProjectNameList.Add(NotReadyToTypeSettingProductProjectVMObservableCollection[Index].ProductProjectName);
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
        [JsonIgnore]
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
        /// 從箱子拿出來  放回去專案
        /// </summary>
        [JsonIgnore]
        public ICommand NoneBox_OnDropRecordCommand
        {
            get
            {
                return new AsyncRelayCommand<object>(async obj =>
                {
                    if (obj is DevExpress.Xpf.Core.DropRecordEventArgs e)
                    {
                        var DragDropData = e.Data.GetData(typeof(DevExpress.Xpf.Core.RecordDragDropData)) as DevExpress.Xpf.Core.RecordDragDropData;
                        foreach (var _record in DragDropData.Records)
                        {
                            if (_record is PartsParameterViewModel PartsParameterVM)
                            {
                                PartsParameterVM.DistributeName = null;
                                PartsParameterVM.BoxIndex = null;
                                e.Effects = System.Windows.DragDropEffects.Move;                             
                                
                             await SaveProductProjectVMObservableCollectionAsync();
                            }
                        }

                    }
                });
            }
        }
        /// <summary>
        /// 檢驗 不可把不同名稱的專案丟回去
        /// </summary>
        [JsonIgnore]
        public ICommand NoneBox_OnDragRecordOverCommand
        {
            get => new RelayCommand<DevExpress.Xpf.Core.DragRecordOverEventArgs>(obj =>
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
                                    if (PartsParameterVM.ProjectID != ReadyToTypeSettingProductProjectVMSelected.ProductProjectName)
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


        private ICommand _box_OnDropRecordCommand;
        /// <summary>
        /// 丟入盒子內
        /// </summary> 
        [JsonIgnore]
        public ICommand Box_OnDropRecordCommand
        {
            get => _box_OnDropRecordCommand??=new AsyncRelayCommand<object>(async obj =>
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
                                    PartsParameterVM.DistributeName = StampingBoxPartsVM.ProjectDistributeName;// ProjectDistribute.ProjectDistributeName;
                                    PartsParameterVM.BoxIndex = StampingBoxPartsVM.SelectedSeparateBoxVM.BoxIndex;
                                    e.Effects = System.Windows.DragDropEffects.Move;

                                   await SaveProductProjectVMObservableCollectionAsync();
                                }
                            }
                        }
                    }
                }
            });
        }

        private ICommand _box_OnDragRecordOverCommand;
        [JsonIgnore]
        public ICommand Box_OnDragRecordOverCommand
        {
            get => _box_OnDragRecordOverCommand??= GD_Command.Box_OnDragRecordOverCommand;
        }

        public ICommand PartsParameterVMCollection_Unassigned_RowFilterCommand
        {
            get => GD_Command.PartsParameterVMCollection_Unassigned_RowFilterCommand;
        }



        private ICommand _projectGridControlRowDoubleClickCommand;
           
         [JsonIgnore]
        public ICommand ProjectGridControlRowDoubleClickCommand
        {
            get => _projectGridControlRowDoubleClickCommand??= new DevExpress.Mvvm.DelegateCommand<DevExpress.Mvvm.Xpf.RowClickArgs>(async (DevExpress.Mvvm.Xpf.RowClickArgs args) =>
            {
                try
                {
                    if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.PartsParameterViewModel partsParameterVM)
                    {
                        partsParameterVM.DistributeName = StampingBoxPartsVM.ProjectDistributeName;
                        partsParameterVM.BoxIndex = StampingBoxPartsVM.SelectedSeparateBoxVM.BoxIndex;
                        StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.Add(partsParameterVM);
                        await SaveProductProjectVMObservableCollectionAsync();

                        OnPropertyChanged(nameof(PartsParameterVMCollection_Unassigned_RowFilterCommand));
                    }
                }
                catch
                {

                }

               /* if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel ProjectItem)
                {
                }*/
            });
        }



    }
}
