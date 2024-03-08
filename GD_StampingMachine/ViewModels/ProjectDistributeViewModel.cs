using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Extensions;
using DevExpress.DataAccess.Json;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.Xpf;
using DevExpress.Office.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.CodeView;
using DevExpress.Xpf.Layout.Core;
using DevExpress.XtraRichEdit.Model.History;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.Method;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels.ParameterSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static DevExpress.CodeParser.CodeStyle.Formatting.Rules.Spacing;

namespace GD_StampingMachine.ViewModels
{

    public class ProjectDistributeModel
    {
        public ProjectDistributeModel()
        { 

        }


        /// <summary>
        /// 製品專案名稱
        /// </summary>
        public string? ProjectDistributeName { get; set; }
        /// <summary>
        /// 製品專案建立時間
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 製品編輯時間
        /// </summary>
        public DateTime? EditDate { get; set; }


        public List<string>? ProductProjectNameList { get; set; } =new List<string>();

        /// <summary>
        /// 製品清單
        /// </summary>
        //[JsonIgnore]
        //public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get; set; }
        /// <summary>
        /// 盒子
        /// </summary>
        //[JsonIgnore]
        //public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; }

        /// <summary>
        /// 鋼捲位置
        /// </summary>
        //public double StripSteelPosition = 0;
        /// <summary>
        /// 鋼捲長度
        /// </summary>
      //  public double StripSteelLength = 300000;


    }

    /// <summary>
    /// 加工專案
    /// </summary>
    public class ProjectDistributeViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_ProjectDistributeViewModel");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectDistribute">專案資料結構</param>
        /// <param name="productProjectVMObservableCollection">製品專案</param>
        /// <param name="separateBoxViewModelCollection">箱子</param>
        public ProjectDistributeViewModel(ProjectDistributeModel projectDistribute,
            ObservableCollection<ProductProjectViewModel> productProjectVMObservableCollection,
            ObservableCollection<SeparateBoxViewModel> separateBoxViewModelCollection)
        {
            ProjectDistribute = projectDistribute;

            StampingBoxPartsVM = new StampingBoxPartsViewModel(new StampingBoxPartModel()
            {
                ProjectDistributeName = this.ProjectDistributeName,
            })
            {
                SeparateBoxVMObservableCollection = new SeparateBoxExtViewModelObservableCollection(separateBoxViewModelCollection)
            };
            ProductProjectVMObservableCollection = productProjectVMObservableCollection;

            var newCollection = new PartsParameterViewModelObservableCollection();
            if (productProjectVMObservableCollection != null)
            {
                foreach (var productProject in productProjectVMObservableCollection)
                {
                    foreach (var productProjectPartViewModel in productProject.PartsParameterVMObservableCollection)
                    {
                        if (productProjectPartViewModel.BoxIndex.HasValue)
                            if (productProjectPartViewModel.DistributeName == this.ProjectDistributeName)
                                if (!newCollection.Contains(productProjectPartViewModel))
                                    newCollection.Add(productProjectPartViewModel);
                    }
                }
            }
            StampingBoxPartsVM.BoxPartsParameterVMCollection = newCollection;

        }





        /// <summary>
        /// 資料結構
        /// </summary>
        public readonly ProjectDistributeModel ProjectDistribute;


        internal async Task SaveProductProjectVMObservableCollectionAsync()
        {
            var saveTask = StampingMachineSingleton.Instance.ProductSettingVM.ProductProjectVMObservableCollection.Select(x => x.SaveProductProjectAsync());
            await Task.WhenAll(saveTask);
        }




        public string? ProjectDistributeName { get => ProjectDistribute.ProjectDistributeName; set { ProjectDistribute.ProjectDistributeName = value; OnPropertyChanged(); } }
        public DateTime CreatedDate { get => ProjectDistribute.CreatedDate; set { ProjectDistribute.CreatedDate = value; OnPropertyChanged(); } }
        public DateTime? EditDate { get => ProjectDistribute.EditDate; set { ProjectDistribute.EditDate = value; OnPropertyChanged(); } }
        public List<string> ProductProjectNameList { get => ProjectDistribute.ProductProjectNameList ??= new List<string>(); set { ProjectDistribute.ProductProjectNameList = value; OnPropertyChanged(); } }


        private StampingBoxPartsViewModel _stampingBoxPartsVM;
        /// <summary>
        /// 盒子與專案
        /// </summary>
        public StampingBoxPartsViewModel StampingBoxPartsVM
        {
            get => _stampingBoxPartsVM;
            set
            {
                _stampingBoxPartsVM = value;
                OnPropertyChanged();
            }
        }







        /// <summary>
        /// 鋼捲位置
        /// </summary>
        /*public double StripSteelPosition
        {
            get => ProjectDistribute.StripSteelPosition; set { ProjectDistribute.StripSteelPosition = value; OnPropertyChanged(); }
        }*/

        /// <summary>
        /// 鋼捲長度
        /// </summary>
       /* public double StripSteelLength
        {
            get => ProjectDistribute.StripSteelLength; set { ProjectDistribute.StripSteelLength = value; OnPropertyChanged(); }
        }*/







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

        





        private DevExpress.Mvvm.ICommand<RowFilterArgs>? _notReadyToTypeSettingProductProjectRowFilterCommand;
        public DevExpress.Mvvm.ICommand<RowFilterArgs> NotReadyToTypeSettingProductProjectRowFilterCommand
        {
            get => _notReadyToTypeSettingProductProjectRowFilterCommand ??= new DevExpress.Mvvm.DelegateCommand<RowFilterArgs>(args =>
            {
                if (args.Item is ProductProjectViewModel ProductProject)
                {
                    if(ProductProject.ProductProjectIsFinish)
                        args.Visible = false;
                    else
                        args.Visible = true;
                }
                else
                {
                    args.Visible = true;
                }
            });
        }
            


        private ProductProjectViewModel? _notReadyToTypeSettingProductProjectVMCurrentItem;
        public ProductProjectViewModel? NotReadyToTypeSettingProductProjectVMCurrentItem
        {
            get => _notReadyToTypeSettingProductProjectVMCurrentItem;
            set
            {
                _notReadyToTypeSettingProductProjectVMCurrentItem = value;
                OnPropertyChanged();
                //PartsParameterVMCollectionRefresh();
            }
        }
        private ProductProjectViewModel? _readyToTypeSettingProductProjectVMCurrentItem;
        public ProductProjectViewModel? ReadyToTypeSettingProductProjectVMCurrentItem
        {
            get => _readyToTypeSettingProductProjectVMCurrentItem;
            set
            {
                _readyToTypeSettingProductProjectVMCurrentItem = value;
                OnPropertyChanged(nameof(ReadyToTypeSettingProductProjectVMCurrentItem));
            }
        }


        // private ObservableCollection<ProductProjectViewModel> _productProjectVMObservableCollection;
        private ProductProjectViewModel? _readyToTypeSettingProductProjectVMSelected;
        public ProductProjectViewModel? ReadyToTypeSettingProductProjectVMSelected
        {
            get => _readyToTypeSettingProductProjectVMSelected;
            set
            {
                _readyToTypeSettingProductProjectVMSelected = value;
                OnPropertyChanged();
                if (value == null)
                    PartsParameterVMObservableCollection = null;
                else
                    PartsParameterVMObservableCollection = new PartsParameterViewModelObservableCollection(value.PartsParameterVMObservableCollection
                    .Where(x => x.BoxIndex == null && string.IsNullOrEmpty(x.DistributeName) && !x.IsFinish));
            }
        }













        private ObservableCollection<ProductProjectViewModel>? _productProjectVMObservableCollection;
        /// <summary>
        /// 全製品專案
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get 
            { 
                _productProjectVMObservableCollection ??= new ObservableCollection<ProductProjectViewModel>();
                _productProjectVMObservableCollection.CollectionChanged -= _productProjectVMObservableCollection_CollectionChanged;
                _productProjectVMObservableCollection.CollectionChanged += _productProjectVMObservableCollection_CollectionChanged;
                return _productProjectVMObservableCollection;
            }
            set
            {

                _productProjectVMObservableCollection = value; 
                if (_productProjectVMObservableCollection != null)
                {
                    _productProjectVMObservableCollection.CollectionChanged -= _productProjectVMObservableCollection_CollectionChanged;
                    _productProjectVMObservableCollection.CollectionChanged += _productProjectVMObservableCollection_CollectionChanged;
                    foreach (var item in _productProjectVMObservableCollection)
                    {
                        item.PartsParameterVMObservableCollection.CollectionChanged -= selectProductProjectPartsParameterVMObservableCollection_CollectionChanged;
                        item.PartsParameterVMObservableCollection.CollectionChanged += selectProductProjectPartsParameterVMObservableCollection_CollectionChanged;
                    }
                }
                SyncProductProject(value);
                OnPropertyChanged(nameof(ProductProjectVMObservableCollection));
            }
        }

        private void _productProjectVMObservableCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender is IList<ProductProjectViewModel> list)
            {
                SyncProductProject(list);
            }
            /* if (e.NewItems is ICollection newCollection)
             {
                 List< ProductProjectViewModel > products = new List< ProductProjectViewModel >();
                 foreach(var item in newCollection)
                 {
                     if(item is ProductProjectViewModel productProjectVM)
                     {
                         products.Add(productProjectVM);
                     }
                 }
                 UpdateProductProject(products);
             }

             if (e.OldItems is ICollection oldCollection)
             {
                 foreach (var item in oldCollection)
                 {
                     if (item is ProductProjectViewModel productProjectVM)
                     {

                     }
                 }
             }*/
        }
        private void selectProductProjectPartsParameterVMObservableCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            if (e.NewItems is ICollection newCollection)
                            {
                                foreach (var item in newCollection)
                                {
                                    if (item is PartsParameterViewModel partsParameter)
                                    {
                                        if (ReadyToTypeSettingProductProjectVMSelected?.ProductProjectName == partsParameter.ProductProjectName)
                                        {
                                            if (string.IsNullOrEmpty(partsParameter.DistributeName) && !partsParameter.IsFinish)
                                            {
                                                if (!PartsParameterVMObservableCollection.Contains(partsParameter))
                                                    PartsParameterVMObservableCollection.Add(partsParameter);
                                            }
                                            else
                                            {
                                                if (!StampingBoxPartsVM.BoxPartsParameterVMCollection.Contains(partsParameter))
                                                    StampingBoxPartsVM.BoxPartsParameterVMCollection.Add(partsParameter);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        {
                            if (e.OldItems is ICollection oldCollection)
                            {
                                foreach (var item in oldCollection.OfType<PartsParameterViewModel>())
                                {
                                    if (item is PartsParameterViewModel partsParameter)
                                    {
                                        if (PartsParameterVMObservableCollection.Contains(partsParameter))
                                            PartsParameterVMObservableCollection.Remove(partsParameter);
                                        if (StampingBoxPartsVM.BoxPartsParameterVMCollection.Contains(partsParameter))
                                            StampingBoxPartsVM.BoxPartsParameterVMCollection.Remove(partsParameter);
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

                                foreach (var (oldItem, newItem) in replacements)
                                {
                                    var partIndex = PartsParameterVMObservableCollection.IndexOf(oldItem);
                                    if (partIndex != -1)
                                        PartsParameterVMObservableCollection[partIndex] = newItem;

                                    var boxPartIndex = StampingBoxPartsVM.BoxPartsParameterVMCollection.IndexOf(oldItem);
                                    if (boxPartIndex != -1)
                                        StampingBoxPartsVM.BoxPartsParameterVMCollection[boxPartIndex] = newItem;
                                }
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;

                    case NotifyCollectionChangedAction.Reset:
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }




        private void SyncProductProject(IList<ProductProjectViewModel> productProjectVMObservableCollection)
        {
            var readyToAdd = productProjectVMObservableCollection
                .Where(obj => obj.PartsParameterVMObservableCollection.Any(x => x.DistributeName == this.ProjectDistributeName))
                .Except(ReadyToTypeSettingProductProjectVMObservableCollection);

            var notReadyToAdd = productProjectVMObservableCollection
                .Except(ReadyToTypeSettingProductProjectVMObservableCollection)
                .Except(NotReadyToTypeSettingProductProjectVMObservableCollection);

            ReadyToTypeSettingProductProjectVMObservableCollection.AddRange(readyToAdd);
            NotReadyToTypeSettingProductProjectVMObservableCollection.AddRange(notReadyToAdd);


            // 找到 b 中不存在於 a 的元素
            var readyElementsToRemove = ReadyToTypeSettingProductProjectVMObservableCollection.Except(productProjectVMObservableCollection).ToList();
            // 刪除 b 中不存在於 a 的元素
            foreach (var element in readyElementsToRemove)
            {
                ReadyToTypeSettingProductProjectVMObservableCollection.Remove(element);
            }

            var notReadyElementsToRemove = NotReadyToTypeSettingProductProjectVMObservableCollection.Except(productProjectVMObservableCollection).ToList();
            foreach (var element in notReadyElementsToRemove)
            {
                NotReadyToTypeSettingProductProjectVMObservableCollection.Remove(element);
            }
            ReadyToTypeSettingProductProjectVMCurrentItem ??= ReadyToTypeSettingProductProjectVMObservableCollection.FirstOrDefault();

        }
        

        private ProductProjectViewModel? _notReadyToTypeSettingProductProjectVMSelected;
        public ProductProjectViewModel? NotReadyToTypeSettingProductProjectVMSelected
        {
            get => _notReadyToTypeSettingProductProjectVMSelected;
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


        private ObservableCollection<ProductProjectViewModel> _notReadyToTypeSettingProductProjectVMObservableCollection = new();

        public ObservableCollection<ProductProjectViewModel> NotReadyToTypeSettingProductProjectVMObservableCollection
        {
            get
            {
                return _notReadyToTypeSettingProductProjectVMObservableCollection;
            }
            set
            {
                _notReadyToTypeSettingProductProjectVMObservableCollection = value;
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
            get
            {
                _readyToTypeSettingProductProjectVMObservableCollection ??= new();
                _readyToTypeSettingProductProjectVMObservableCollection.CollectionChanged -= ReadyToTypeSettingProductProjectVMObservableCollection_CollectionChanged;
               _readyToTypeSettingProductProjectVMObservableCollection.CollectionChanged += ReadyToTypeSettingProductProjectVMObservableCollection_CollectionChanged;
                return _readyToTypeSettingProductProjectVMObservableCollection; 
            }
            set
            {
                _readyToTypeSettingProductProjectVMObservableCollection = value;
                if (_readyToTypeSettingProductProjectVMObservableCollection!=null)
                {
                    _readyToTypeSettingProductProjectVMObservableCollection.CollectionChanged -= ReadyToTypeSettingProductProjectVMObservableCollection_CollectionChanged;
                    _readyToTypeSettingProductProjectVMObservableCollection.CollectionChanged += ReadyToTypeSettingProductProjectVMObservableCollection_CollectionChanged;
                }
                OnPropertyChanged();
            }
        }

        private void ReadyToTypeSettingProductProjectVMObservableCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //尋找盒子內是否有相同的物件 若無則新增
            if (e.NewItems is ICollection newCollection)
            {
                List<PartsParameterViewModel> AddParts = new();
                foreach (var item in newCollection)
                {
                    if (item is ProductProjectViewModel productProject)
                    {
                        AddParts.AddRange(productProject.PartsParameterVMObservableCollection.Where(x => x.DistributeName == StampingBoxPartsVM.ProjectDistributeName).ToList());
                    }
                }
                var notExistedParts = AddParts.Except(StampingBoxPartsVM.BoxPartsParameterVMCollection);
                StampingBoxPartsVM.BoxPartsParameterVMCollection.AddRange(notExistedParts);
            }

            //尋找盒子內是否有相同的物件 若有則刪除
            if (e.OldItems is ICollection oldCollection)
            {
                List<PartsParameterViewModel> AddParts = new();
                foreach (var item in oldCollection)
                {
                    if (item is ProductProjectViewModel productProject)
                    {
                        AddParts.AddRange(productProject.PartsParameterVMObservableCollection.Where(x => x.DistributeName == StampingBoxPartsVM.ProjectDistributeName).ToList());
                    }
                }

                var ExistedParts = AddParts.Intersect(StampingBoxPartsVM.BoxPartsParameterVMCollection);

                foreach(var part in ExistedParts)
                {
                   if(StampingBoxPartsVM.BoxPartsParameterVMCollection.Contains(part))
                        StampingBoxPartsVM.BoxPartsParameterVMCollection.Remove(part);
                }

              
            }

        }

        private ICommand _closeTypeSettingCommand;
        /// <summary>
        /// 關閉排版專案
        /// </summary>
        [JsonIgnore]
        public ICommand CloseTypeSettingCommand
        {
            get => _closeTypeSettingCommand ??= new AsyncRelayCommand<object>(async obj =>
            {
                if (obj == null)
                {
                    throw new Exception();
                }
                //新寫法
                if (obj is GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel ProductProjectVM)
                {
                    
                    var CollectionWithThisDistributeName =
                    this.StampingBoxPartsVM.BoxPartsParameterVMCollection.Intersect(ProductProjectVM.PartsParameterVMObservableCollection).ToList();
                    //var CollectionWithThisDistributeName = ProjectDistributeVM.SeparateBoxVMObservableCollection.Where(x => x.DistributeName == ProjectDistributeVM.ProjectDistributeName);
                    //箱子內有專案
                    if (CollectionWithThisDistributeName?.Count() > 0)
                    {
                        //有已完成的 不可關閉
                        //    if (CollectionWithThisDistributeName.ToList().Exists(x => x.MachiningStatus == MachiningStatusEnum.Finish))

                        if (CollectionWithThisDistributeName.ToList().Exists(x => x.IsFinish))
                        {
                            await MethodWinUIMessageBox.CanNotCloseProjectAsync();
                            return;
                        }

                        //詢問是否要關閉
                        if (!await MethodWinUIMessageBox.AskCloseProjectAsync(this.ReadyToTypeSettingProductProjectVMCurrentItem.ProductProjectName))
                            return;

                        //將資料清除
                        CollectionWithThisDistributeName.ForEach(Eobj =>
                        {
                            Eobj.DistributeName = null;
                            Eobj.BoxIndex = null;
                        });
                        //await   ProjectDistributeVM.StampingBoxPartsVM.ReLoadBoxPartsParameterVMCollectionAsync();
                        //  ProjectDistributeVM.StampingBoxPartsVM.ProductProjectVMObservableCollection
                    }

                    this.ProductProjectNameList.Remove(this.ReadyToTypeSettingProductProjectVMCurrentItem.ProductProjectName);
                    this.NotReadyToTypeSettingProductProjectVMObservableCollection.Add(this.ReadyToTypeSettingProductProjectVMCurrentItem);
                    this.ReadyToTypeSettingProductProjectVMObservableCollection.Remove(this.ReadyToTypeSettingProductProjectVMCurrentItem);
                }
            });
        }



        [JsonIgnore]
        public ICommand MoveNotReadyToReadyCommand
        {
            get => new RelayCommand(() =>
            {
                var Index = -1;
                do
                {
                    Index = NotReadyToTypeSettingProductProjectVMObservableCollection.FindIndex(x => x.IsMarked);
                    if (Index != -1)
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






       // private ObservableCollection<PartsParameterViewModel>? _previousPartsParameterVMCollection;
        private PartsParameterViewModelObservableCollection? _partsParameterVMObservableCollection;
        /// <summary>
        /// GridControl ABC參數 沒放進箱子內的
        /// </summary>
        [JsonIgnore]
        public PartsParameterViewModelObservableCollection PartsParameterVMObservableCollection
        {
            get
            {
                if (_partsParameterVMObservableCollection ==null)
                    _partsParameterVMObservableCollection = new PartsParameterViewModelObservableCollection();
                return _partsParameterVMObservableCollection;
            }
            set
            {
                _partsParameterVMObservableCollection = value;
                OnPropertyChanged();
            }
        }




        /// <summary>
        /// 盒子列表
        /// </summary>
        /* public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection
         {
             get => ProjectDistribute.SeparateBoxVMObservableCollection;
             set
             {
                 ProjectDistribute.SeparateBoxVMObservableCollection = value;
                 OnPropertyChanged();
             }
         }*/


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
                                PartsParameterVM.DistributeName = string.Empty;
                                PartsParameterVM.BoxIndex = null;
                                e.Effects = System.Windows.DragDropEffects.Move;
                            }
                        }

                        //StampingBoxPartsVM.RefreshBoxPartsParameterVMRowFilter();
                        await SaveProductProjectVMObservableCollectionAsync();
                        //OnPropertyChanged(nameof(PartsParameterVMCollection_Unassigned_RowFilterCommand));
                    }
                });
            }
        }


        private ICommand?_noneBox_OnDragRecordOverCommand;
        /// <summary>
        /// 檢驗 不可把不同名稱的專案丟回去
        /// </summary>
        [JsonIgnore]
        public ICommand NoneBox_OnDragRecordOverCommand
        {
            get => _noneBox_OnDragRecordOverCommand ??= new RelayCommand<object>(obj =>
            {
                if (obj is DevExpress.Xpf.Core.DragRecordOverEventArgs e)
                {
                    e.Effects = System.Windows.DragDropEffects.None;
                    if (e.Data.GetData(typeof(DevExpress.Xpf.Core.RecordDragDropData)) is DevExpress.Xpf.Core.RecordDragDropData DragDropData)
                    {
                        var canMove = DragDropData.Records
                            .OfType<PartsParameterViewModel>()
                            .All(PartsParameterVM =>
                            {
                                if (ReadyToTypeSettingProductProjectVMSelected == null)
                                {
                                    return false;
                                }
                                if (!ReadyToTypeSettingProductProjectVMSelected.PartsParameterVMObservableCollection.Contains(PartsParameterVM))
                                {
                                    //不在陣列內但名稱一樣的也能放
                                    if (ReadyToTypeSettingProductProjectVMSelected.ProductProjectName != PartsParameterVM.ProductProjectName)
                                    {
                                        return false;
                                    }
                                }
                                if (ReadyToTypeSettingProductProjectVMSelected.ProductProjectName != PartsParameterVM.ProductProjectName) 
                                    return false;

                                return !(PartsParameterVM.IsFinish || PartsParameterVM.IsSended || PartsParameterVM.ID > 0);
                            });

                        e.Effects = canMove ? System.Windows.DragDropEffects.Move : System.Windows.DragDropEffects.None;
                    }
                }
            });
        }

        private ICommand?_box_OnDropRecordCommand;
        /// <summary>
        /// 丟入盒子內
        /// </summary> 
        [JsonIgnore]
        public ICommand Box_OnDropRecordCommand
        {
            get => _box_OnDropRecordCommand ??= new AsyncRelayCommand<object>(async obj =>
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
                                }
                            }
                        }
                        e.Effects = System.Windows.DragDropEffects.Move;
                        await RefreshBoxPartsParameterVMRowFilterAsync();
                        _ = SaveProductProjectVMObservableCollectionAsync();
                    }
                   
                }
            });
        }

        private ICommand?_box_OnDragRecordOverCommand;
        [JsonIgnore]
        public ICommand Box_OnDragRecordOverCommand
        {
            get => _box_OnDragRecordOverCommand ??= GD_Command.Box_OnDragRecordOverCommand;
        }

        /*public ICommand PartsParameterVMCollection_Unassigned_RowFilterCommand
        {
            get => GD_Command.PartsParameterVMCollection_Unassigned_RowFilterCommand;
        }*/


        private AsyncRelayCommand? _projectGridControlInsertToBoxCommand;

        [JsonIgnore]
        public AsyncRelayCommand ProjectGridControlInsertToBoxCommand
        {
            get => _projectGridControlInsertToBoxCommand ??= new (async () =>
            {
               await Task.Run(async () =>
                {
                    try
                    {

                        int startIndex = StampingBoxPartsVM.SeparateBoxVMObservableCollection.FindIndex(x => x == StampingBoxPartsVM.SelectedSeparateBoxVM);
                        if (startIndex == -1)
                            startIndex = 0;
                        int stopIndex = startIndex - 1;
                        if(stopIndex == -1)
                        {
                            if (StampingBoxPartsVM.SeparateBoxVMObservableCollection.Count != 0)
                                stopIndex = StampingBoxPartsVM.SeparateBoxVMObservableCollection.Count - 1;
                            else
                                stopIndex = 0;
                        }

                        int currentIndex = startIndex;
                        //依照選擇的盒子往後放
                        //被停用的盒子不放

                        //可用的盒子
                        List<ParameterSetting.SeparateBoxViewModel> availableSeparateBoxCollection = new();
                        do
                        {
                            var currentElement = StampingBoxPartsVM.SeparateBoxVMObservableCollection[currentIndex];
                            if (currentElement.BoxIsEnabled && currentElement.BoxSliderValue >0)
                            {
                                availableSeparateBoxCollection.Add(currentElement);
                            }
                            // Debug.WriteLine(currentElement);
                            currentIndex = (currentIndex + 1) % StampingBoxPartsVM.SeparateBoxVMObservableCollection.Count;
                        }
                        while (currentIndex != ((stopIndex + 1) % StampingBoxPartsVM.SeparateBoxVMObservableCollection.Count));

                        List<PartsParameterViewModel> addPartsParameterViewModel = new();

                        int boxIndex = 0;
                        foreach (var partsParameterVM in PartsParameterVMObservableCollection)
                        {
                            var sepratebox = availableSeparateBoxCollection[boxIndex];

                            partsParameterVM.DistributeName = StampingBoxPartsVM.ProjectDistributeName;
                            partsParameterVM.BoxIndex = sepratebox.BoxIndex;
                            partsParameterVM.WorkIndex = -1;
                            partsParameterVM.IsSended = false;
                            partsParameterVM.IsTransported = false;
             


                            addPartsParameterViewModel.Add(partsParameterVM);
                            Application.Current?.Dispatcher.Invoke(new Action(() =>
                            {
                                StampingBoxPartsVM.BoxPartsParameterVMCollection.Add(partsParameterVM);
                            }));

                            var boxCount = StampingBoxPartsVM.BoxPartsParameterVMCollection.Count(x => !x.IsTransported && x.BoxIndex == sepratebox.BoxIndex);
                            //餘數
                            var remainder = boxCount % (int)sepratebox.BoxSliderValue;
                            if (remainder ==0)
                            {
                                //箱子往後推一格
                                boxIndex = (boxIndex + 1) % availableSeparateBoxCollection.Count;
                            }

                        }



                        /*
                        foreach (var partsParameterVM in PartsParameterVMObservableCollection)
                        {
                            if (partsParameterVM.BoxIndex != null)
                                continue;

                            //照順序放入 找出條件後將東西丟進去
                            foreach (var sepratebox in availableSeparateBoxCollection)
                            {
                                //確認盒子內有多少
                               var boxCount = StampingBoxPartsVM.BoxPartsParameterVMCollection.Count(x => !x.IsTransported  && x.BoxIndex == sepratebox.BoxIndex);

                                if(boxCount<= sepratebox.BoxSliderValue)
                                {
                                    partsParameterVM.DistributeName = StampingBoxPartsVM.ProjectDistributeName;
                                    partsParameterVM.BoxIndex = sepratebox.BoxIndex;

                                    addPartsParameterViewModel.Add(partsParameterVM);

                                    Application.Current?.Dispatcher.Invoke(new Action(() =>
                                    {
                                        StampingBoxPartsVM.BoxPartsParameterVMCollection.Add(partsParameterVM);
                                    }));

                                    break;
                                }
                            }
                        }*/
                        //將多餘的部分移除
                    
                        Application.Current?.Dispatcher.Invoke(new Action(() =>
                        {
                            foreach(var part in addPartsParameterViewModel)
                            {
                               var remove = PartsParameterVMObservableCollection.Remove(part);
                            }
                        }));
                        //ReloadPartsParameterVMCollection();

                        await SaveProductProjectVMObservableCollectionAsync();

                       // OnPropertyChanged(nameof(PartsParameterVMCollection_Unassigned_RowFilterCommand));
                    }
                    catch (Exception ex)
                    {

                        Debug.WriteLine(ex);
                    }
                });
            },()=> !ProjectGridControlInsertToBoxCommand.IsRunning);
        }






       /* [JsonIgnore]
        public ICommand ReloadTypeSettingSettingsCommand
        {
            get => new AsyncRelayCommand(async() =>
            {
                await Task.Run(async () =>
                {
                 //   await   ReloadPartsParameterVMCollectionAsync();
                });
            });
        }*/
        /*
        public async Task ReloadPartsParameterVMCollectionAsync()
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        if (ReadyToTypeSettingProductProjectVMSelected == null)
                            PartsParameterVMObservableCollection = new PartsParameterViewModelObservableCollection();
                        else
                            PartsParameterVMObservableCollection = new PartsParameterViewModelObservableCollection(ReadyToTypeSettingProductProjectVMSelected.PartsParameterVMObservableCollection.Where(x => x.BoxIndex == null && string.IsNullOrEmpty(x.DistributeName) && !x.IsFinish));


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
                        while ((BFinishindex = NotReadyToTypeSettingProductProjectVMObservableCollection.FindIndex(x => x.ProductProjectIsFinish)) != -1)
                        {
                            NotReadyToTypeSettingProductProjectVMObservableCollection.RemoveAt(BFinishindex);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                        Debugger.Break();
                    }
                });
            }
            catch
            {

            }
        }
        */

        private ICommand?_separateBoxVMObservableCollectionelectionChangedCommand;
        [JsonIgnore]
        public ICommand SeparateBoxVMObservableCollectionelectionChangedCommand
        {
            get => _separateBoxVMObservableCollectionelectionChangedCommand ??= new AsyncRelayCommand<object>(async obj =>
            {
              await  RefreshBoxPartsParameterVMRowFilterAsync();
            });
        }

        /// <summary>
        /// 重新整理篩選器
        /// </summary>
        public async Task RefreshBoxPartsParameterVMRowFilterAsync()
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                OnPropertyChanged(nameof(BoxPartsParameterVMRowFilterCommand));
            });
        }



        private ICommand?_allBoxItemReturnToProjectCommand;
        public ICommand AllBoxItemReturnToProjectCommand
        {
            get => _allBoxItemReturnToProjectCommand ??= new AsyncRelayCommand(async () =>
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        var selectedBoxIndex = this.StampingBoxPartsVM.SelectedSeparateBoxVM?.BoxIndex;
                        var movableCollection = this.StampingBoxPartsVM.BoxPartsParameterVMCollection.Where(x =>
                        x.BoxIndex == selectedBoxIndex && 
                        (x.IsFinish || x.IsSended || !x.IsTransported)).ToList();

                        //foreach (var moveableItem in movableCollection)
                        for (int i = 0; i < movableCollection.Count; i++)
                        {
                            var moveableItem = movableCollection[i];
                            moveableItem.BoxIndex = null;
                            moveableItem.WorkIndex = -1;
                            moveableItem.DistributeName = null;
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                             {
                                 this.StampingBoxPartsVM.BoxPartsParameterVMCollection.Remove(moveableItem);
                             });
                        }

                        //需要重新刷新的專案
                        //   var needRefreshProductProjectCollection = movableCollection.Select(x=>x.ProductProjectName).Distinct();
                        // foreach(var needRefreshProductProject in needRefreshProductProjectCollection)
                        //{
                        var projectList = StampingMachineSingleton.Instance.TypeSettingSettingVM?.ProjectDistributeVMObservableCollection;
                        if (projectList != null)
                        {
                            foreach (var project in projectList)
                            {
                               // _ = project?.ReloadPartsParameterVMCollectionAsync();
                            }
                        }
                        //   }

                        await RefreshBoxPartsParameterVMRowFilterAsync();
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                });
            });
        }







        [JsonIgnore]
        public ICommand BoxPartsParameterVMRowFilterCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<RowFilterArgs>(args =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.PartsParameterViewModel PParameter)
                {
                    if (StampingBoxPartsVM.SelectedSeparateBoxVM != null)
                    {
                        if (PParameter.DistributeName == this.ProjectDistributeName && PParameter.BoxIndex == StampingBoxPartsVM.SelectedSeparateBoxVM.BoxIndex && !PParameter.IsTransported)
                            args.Visible = true;
                        else
                            args.Visible = false;
                    }
                }
            });
        }








    }
}
