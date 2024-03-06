using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.Xpf;
using DevExpress.Office.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.Xpf.CodeView;
using DevExpress.XtraRichEdit.Model.History;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels.ParameterSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public double StripSteelPosition = 0;
        /// <summary>
        /// 鋼捲長度
        /// </summary>
        public double StripSteelLength = 300000;


    }

    /// <summary>
    /// 加工專案
    /// </summary>
    public class ProjectDistributeViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_ProjectDistributeViewModel");

        public ProjectDistributeViewModel(ProjectDistributeModel projectDistribute, ObservableCollection<ProductProjectViewModel> productProjectVMObservableCollection, ObservableCollection<SeparateBoxViewModel> separateBoxVMObservableCollection)
        {
            ProjectDistribute = projectDistribute;
            ProductProjectVMObservableCollection = productProjectVMObservableCollection;
            SeparateBoxVMObservableCollection = separateBoxVMObservableCollection;
        }





        /// <summary>
        /// 資料結構
        /// </summary>
        public readonly ProjectDistributeModel ProjectDistribute;


        internal async Task SaveProductProjectVMObservableCollectionAsync()
        {
            var saveTask = ProductProjectVMObservableCollection.Select(x => x.SaveProductProjectAsync());
            await Task.WhenAll(saveTask);
        }




        public string? ProjectDistributeName { get => ProjectDistribute.ProjectDistributeName; set { ProjectDistribute.ProjectDistributeName = value; OnPropertyChanged(); } }
        public DateTime CreatedDate { get => ProjectDistribute.CreatedDate; set { ProjectDistribute.CreatedDate = value; OnPropertyChanged(); } }
        public DateTime? EditDate { get => ProjectDistribute.EditDate; set { ProjectDistribute.EditDate = value; OnPropertyChanged(); } }
        public List<string> ProductProjectNameList { get => ProjectDistribute.ProductProjectNameList ??= new List<string>(); set { ProjectDistribute.ProductProjectNameList = value; OnPropertyChanged(); } }


        private StampingBoxPartsViewModel? _stampingBoxPartsVM;
        /// <summary>
        /// 盒子與專案
        /// </summary>
        public StampingBoxPartsViewModel StampingBoxPartsVM
        {
            get => _stampingBoxPartsVM ??= new StampingBoxPartsViewModel(new StampingBoxPartModel()
            {
                ProjectDistributeName = this.ProjectDistributeName,
                ProductProjectVMObservableCollection = this.ProductProjectVMObservableCollection,
                SeparateBoxVMObservableCollection = SeparateBoxVMObservableCollection.DeepCloneByJson(),
            });
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
            get => ProjectDistribute.StripSteelPosition; set { ProjectDistribute.StripSteelPosition = value; OnPropertyChanged(); }
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
                //PartsParameterVMCollectionRefresh();
            }
        }





        private ICommand? _readyToTypeSettingProductProjectVMSelectedChangedCommand;
        public ICommand ReadyToTypeSettingProductProjectVMSelectedChangedCommand
        {
            get => _readyToTypeSettingProductProjectVMSelectedChangedCommand??= new RelayCommand<object>(obj =>
            {

                if(obj is DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
                {
                    if(e.NewItem is GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel selectProductProject)
                    {
                        PartsParameterVMCollection = new ObservableCollection<PartsParameterViewModel>(selectProductProject.PartsParameterVMCollection.Where(x => x.BoxIndex == null && string.IsNullOrEmpty(x.DistributeName) && !x.IsFinish));
                    }
                    else
                    {
                        PartsParameterVMCollection = null;
                    }
                }

            });
        }

        private ObservableCollection<ProductProjectViewModel>? _productProjectVMObservableCollection;
        /// <summary>
        /// 全製品專案
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get => _productProjectVMObservableCollection ??= new ObservableCollection<ProductProjectViewModel>();
            set
            {
                _productProjectVMObservableCollection = value;
                InitProductProject(value);
                OnPropertyChanged();
            }
        }


        private void InitProductProject(ObservableCollection<ProductProjectViewModel> productProjectVMObservableCollection)
        {
            if (ProductProjectNameList != null)
            {
                foreach (var PName in ProductProjectNameList)
                {
                    var Index = ProductProjectVMObservableCollection.FindIndex(x => x.ProductProjectName == PName);
                    if (Index != -1)
                    {
                        if (!ReadyToTypeSettingProductProjectVMObservableCollection.Contains(ProductProjectVMObservableCollection[Index]))
                            ReadyToTypeSettingProductProjectVMObservableCollection.Add(ProductProjectVMObservableCollection[Index]);
                    }
                }
            }

            var readyToAdd = productProjectVMObservableCollection
                .Where(obj => obj.PartsParameterVMCollection.Any(x => x.DistributeName == this.ProjectDistributeName))
                .Where(obj => !ReadyToTypeSettingProductProjectVMObservableCollection.Contains(obj));

            var notReadyToAdd = productProjectVMObservableCollection
                .Where(obj => !ReadyToTypeSettingProductProjectVMObservableCollection.Contains(obj) && !NotReadyToTypeSettingProductProjectVMObservableCollection.Contains(obj));

            ReadyToTypeSettingProductProjectVMObservableCollection.AddRange(readyToAdd);
            NotReadyToTypeSettingProductProjectVMObservableCollection.AddRange(notReadyToAdd);

            ReadyToTypeSettingProductProjectVMCurrentItem = ReadyToTypeSettingProductProjectVMObservableCollection.FirstOrDefault();
        }

        private ObservableCollection<ParameterSetting.SeparateBoxViewModel>? _separateBoxVMObservableCollection;
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection
        {
            get=> _separateBoxVMObservableCollection??= new ObservableCollection<ParameterSetting.SeparateBoxViewModel>();
            set
            {
                _separateBoxVMObservableCollection = value;
                OnPropertyChanged();
            }
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






        private ObservableCollection<PartsParameterViewModel>? _previousPartsParameterVMCollection;
        private ObservableCollection<PartsParameterViewModel>? _partsParameterVMCollection;
        /// <summary>
        /// GridControl ABC參數 沒放進箱子內的
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<PartsParameterViewModel> PartsParameterVMCollection
        {
            get
            {
                if (_partsParameterVMCollection ==null)
                    _partsParameterVMCollection = new ObservableCollection<PartsParameterViewModel>();
                /*
                _partsParameterVMCollection.CollectionChanged += _partsParameterVMCollection_CollectionChanged;

                foreach (var item in _partsParameterVMCollection)
                {
                    item.DistributeNameChanged += Item_DistributeNameChanged;
                }*/
                return _partsParameterVMCollection;
            }
            set
            {
                _partsParameterVMCollection = value;
                OnPropertyChanged();
            }
        }

        /*private void _partsParameterVMCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is PartsParameterViewModel part)
                    {
                        part.DistributeNameChanged += Item_DistributeNameChanged;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach(var item in e.OldItems)
                {
                    if (item is PartsParameterViewModel part)
                    {
                        part.DistributeNameChanged -= Item_DistributeNameChanged ;
                    }
                }
            }
        }*/

        /*private void Item_DistributeNameChanged(object? sender, string e)
        {
            if (string.IsNullOrEmpty(e))
            {
                if (sender is PartsParameterViewModel part && PartsParameterVMCollection.Contains(part))
                {
                   // PartsParameterVMCollection.Remove(part);
                }
            }
            else
            {

            }
        }*/




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
                                PartsParameterVM.DistributeName = null;
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
                            .Any(PartsParameterVM =>
                            {
                                if (ReadyToTypeSettingProductProjectVMSelected == null)
                                {
                                    return false;
                                }
                                if (!ReadyToTypeSettingProductProjectVMSelected.PartsParameterVMCollection.Contains(PartsParameterVM))
                                {
                                    //不在陣列內但名稱一樣的也能放
                                    if (ReadyToTypeSettingProductProjectVMSelected.ProductProjectName != PartsParameterVM.ProductProjectName)
                                    {
                                        return false;
                                    }
                                }
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
                        await SaveProductProjectVMObservableCollectionAsync();
                        RefreshBoxPartsParameterVMRowFilter();
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
                        foreach (var partsParameterVM in PartsParameterVMCollection)
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
                        foreach (var partsParameterVM in PartsParameterVMCollection)
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
                               var remove = PartsParameterVMCollection.Remove(part);
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






        [JsonIgnore]
        public ICommand ReloadTypeSettingSettingsCommand
        {
            get => new AsyncRelayCommand(async() =>
            {
                await Task.Run(async () =>
                {
                    await   ReloadPartsParameterVMCollectionAsync();
                });
            });
        }

        public async Task ReloadPartsParameterVMCollectionAsync()
        {
            await Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    if (ReadyToTypeSettingProductProjectVMSelected == null)
                        PartsParameterVMCollection = new ObservableCollection<PartsParameterViewModel>();
                    else
                        PartsParameterVMCollection = new ObservableCollection<PartsParameterViewModel>(ReadyToTypeSettingProductProjectVMSelected.PartsParameterVMCollection.Where(x => x.BoxIndex == null && string.IsNullOrEmpty(x.DistributeName) && !x.IsFinish));


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
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    Debugger.Break();
                }
            });
        }


        private ICommand?_separateBoxVMObservableCollectionelectionChangedCommand;
        [JsonIgnore]
        public ICommand SeparateBoxVMObservableCollectionelectionChangedCommand
        {
            get => _separateBoxVMObservableCollectionelectionChangedCommand ??= new RelayCommand<object>(obj =>
            {
                RefreshBoxPartsParameterVMRowFilter();
            });
        }

        /// <summary>
        /// 重新整理篩選器
        /// </summary>
        public void RefreshBoxPartsParameterVMRowFilter()
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
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
                        var selectedBoxIndex = this.StampingBoxPartsVM.SelectedSeparateBoxVM.BoxIndex;
                        var movableCollection = this.StampingBoxPartsVM.BoxPartsParameterVMCollection.Where(x =>
                        x.BoxIndex == selectedBoxIndex &&
                        (x.IsFinish || x.IsSended || !x.IsTransported));

                        foreach (var moveableItem in movableCollection)
                        {
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
                                await project?.ReloadPartsParameterVMCollectionAsync();
                            }
                        }
                        //   }

                        RefreshBoxPartsParameterVMRowFilter();
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
