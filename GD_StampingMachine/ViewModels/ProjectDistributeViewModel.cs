﻿using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Extensions;
using DevExpress.DataAccess.Json;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.Xpf;
using DevExpress.Office.Utils;
using DevExpress.Utils.DragDrop.Internal;
using DevExpress.Utils.Extensions;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.CodeView;
using DevExpress.Xpf.Layout.Core;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model.History;

using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Enum;
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


        public HashSet<string>? ProductProjectNameList { get; set; } 

        /// <summary>
        /// 製品清單
        /// </summary>
        //[JsonIgnore]
        //public ObservableCollection<ProductProjectViewModel> ProductProjectVMCollection { get; set; }
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
        /// <param name="productProjectVMCollection">製品專案</param>
        /// <param name="separateBoxViewModelCollection">箱子</param>
        /*public ProjectDistributeViewModel(ProjectDistributeModel projectDistribute,
            ObservableCollection<ProductProjectViewModel> productProjectVMObservableCollection,
            ObservableCollection<SeparateBoxViewModel> separateBoxViewModelCollection)*/
        public ProjectDistributeViewModel(ProjectDistributeModel projectDistribute , 
            SeparateBoxExtViewModelObservableCollection separateBoxExtVMCollection, ObservableCollection<ProductProjectViewModel> productProjectVMCollection)
        {
            //StampingBoxPartsVM = new StampingBoxPartsViewModel(projectDistribute.ProjectDistributeName, separateBoxExtVMCollection);
            SeparateBoxExtVMCollection = separateBoxExtVMCollection;
            ProjectDistribute = projectDistribute;
            ProductProjectVMCollection = productProjectVMCollection;
        }
       
        readonly SeparateBoxExtViewModelObservableCollection SeparateBoxExtVMCollection;



        /// <summary>
        /// 資料結構
        /// </summary>
        public readonly ProjectDistributeModel ProjectDistribute;




        internal async Task SaveProductProjectVMCollectionAsync()
        {
            var saveTask = StampingMachineSingleton.Instance.ProductSettingVM?.ProductProjectVMCollection.Select(x => x.SaveProductProjectAsync());
            if(saveTask!=null)
                await Task.WhenAll(saveTask);
        }

        public string? ProjectDistributeName { get => ProjectDistribute.ProjectDistributeName; set { ProjectDistribute.ProjectDistributeName = value; OnPropertyChanged(); } }
        public DateTime CreatedDate { get => ProjectDistribute.CreatedDate; set { ProjectDistribute.CreatedDate = value; OnPropertyChanged(); } }
        public DateTime? EditDate { get => ProjectDistribute.EditDate; set { ProjectDistribute.EditDate = value; OnPropertyChanged(); } }
        public HashSet<string> ProductProjectNameList { get => ProjectDistribute.ProductProjectNameList ??= new HashSet<string>(); set { ProjectDistribute.ProductProjectNameList = value; OnPropertyChanged(); } }


        private StampingBoxPartsViewModel? _stampingBoxPartsVM;
        /// <summary>
        /// 盒子與專案
        /// </summary>
        public StampingBoxPartsViewModel StampingBoxPartsVM
        {
            get => _stampingBoxPartsVM ??= new StampingBoxPartsViewModel(ProjectDistribute.ProjectDistributeName, SeparateBoxExtVMCollection);
            set
            {
                _stampingBoxPartsVM = value;
                OnPropertyChanged();
            }
        }



        



        private bool _IsInDistributePage = false;
        [JsonIgnore]
        public bool IsInDistributePage { get => _IsInDistributePage; set { _IsInDistributePage = value; OnPropertyChanged(); } }

        private bool _addTypeSettingProjectDraggableIsPopUp;
        public bool AddTypeSettingProjectDraggableIsPopUp
        {
            get => _addTypeSettingProjectDraggableIsPopUp;
            set
            {
                _addTypeSettingProjectDraggableIsPopUp = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _closeTypeSettingCommand;
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
                    //var a =                    this.StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x => x.BoxPartsParameterVMCollection);
                    


                    var CollectionWithThisDistributeName =
                    this.StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x=>x.BoxPartsParameterVMCollection).
                    Intersect(ProductProjectVM.PartsParameterVMObservableCollection).ToList();


                    //var CollectionWithThisDistributeName = ProjectDistributeVM.SeparateBoxVMObservableCollection.Where(x => x.DistributeName == ProjectDistributeVM.ProjectDistributeName);
                    //箱子內有專案
                    if (CollectionWithThisDistributeName?.Count > 0)
                    {
                        //有已完成的 不可關閉
                        //    if (CollectionWithThisDistributeName.ToList().Exists(x => x.MachiningStatus == MachiningStatusEnum.Finish))

                        if (CollectionWithThisDistributeName.ToList().Exists(x => x.IsFinish &&x.DistributeName == this.ProjectDistributeName))
                        {
                            await MethodWinUIMessageBox.CanNotCloseProjectAsync();
                            return;
                        }

                        //詢問是否要關閉
                        if (!await MethodWinUIMessageBox.AskCloseProjectAsync(null, ProductProjectVM.ProductProjectName))
                            return;

                        //將資料清除
                        CollectionWithThisDistributeName.ForEach(Eobj =>
                        {
                            Eobj.DistributeName = null;
                            Eobj.BoxIndex = null;

                           var separateBox = StampingBoxPartsVM.SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxPartsParameterVMCollection.Contains(Eobj));
                            separateBox?.BoxPartsParameterVMCollection.Remove(Eobj);
                        });
                        ProductProjectVM.UnBoxPartsParameterVMObservableCollection.AddRange(CollectionWithThisDistributeName);
                        
                        //await ProductProjectVM.StampingBoxPartsVM.ReLoadBoxPartsParameterVMCollectionAsync();
                        //  ProjectDistributeVM.StampingBoxPartsVM.ProductProjectVMCollection
                    }

                    this.ProductProjectNameList.Remove(ProductProjectVM.ProductProjectName);
                    this.NotReadyToTypeSettingProductProjectVMCollection.Add(ProductProjectVM);
                    this.ReadyToTypeSettingProductProjectVMCollection.Remove(ProductProjectVM);
                }
            });
        }









            



        private ProductProjectViewModel? _readyToTypeSettingProductProjectVMSelected;
        public ProductProjectViewModel? ReadyToTypeSettingProductProjectVMSelected
        {
            get => _readyToTypeSettingProductProjectVMSelected;
            set
            {
                _readyToTypeSettingProductProjectVMSelected = value;
                OnPropertyChanged();

            }
        }













        private ObservableCollection<ProductProjectViewModel>? _productProjectVMCollection;
        /// <summary>
        /// 全製品專案
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMCollection
        {
            get 
            {
                _productProjectVMCollection ??= new ObservableCollection<ProductProjectViewModel>();
                _productProjectVMCollection.CollectionChanged -= ProductProjectVMCollection_CollectionChanged;
                _productProjectVMCollection.CollectionChanged += ProductProjectVMCollection_CollectionChanged;
                foreach (var item in _productProjectVMCollection)
                {
                    item.PartsParameterVMObservableCollection.CollectionChanged -= PartsParameterVMObservableCollection_CollectionChanged;
                    item.PartsParameterVMObservableCollection.CollectionChanged += PartsParameterVMObservableCollection_CollectionChanged;
                }
                return _productProjectVMCollection;
            }
            private set
            {
                _productProjectVMCollection = value; 
                if (_productProjectVMCollection != null)
                {
                    _productProjectVMCollection.CollectionChanged -= ProductProjectVMCollection_CollectionChanged;
                    _productProjectVMCollection.CollectionChanged += ProductProjectVMCollection_CollectionChanged;
                    foreach (var item in _productProjectVMCollection)
                    {
                        item.PartsParameterVMObservableCollection.CollectionChanged -= PartsParameterVMObservableCollection_CollectionChanged;
                        item.PartsParameterVMObservableCollection.CollectionChanged += PartsParameterVMObservableCollection_CollectionChanged;
                    }
                }
                SyncProductProject(value);
                OnPropertyChanged();
            }
        }

        private void ProductProjectVMCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                default:
                    break;
            }


            if (sender is IList<ProductProjectViewModel> list)
            {
                SyncProductProject(list);

                foreach(var project in list)
                {
                    project.PartsParameterVMObservableCollection.CollectionChanged -= PartsParameterVMObservableCollection_CollectionChanged;
                    project.PartsParameterVMObservableCollection.CollectionChanged += PartsParameterVMObservableCollection_CollectionChanged;
                }

            }
        }

        private void PartsParameterVMObservableCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var allSeparateBoxPartsParameterVMCollection = StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x => x.BoxPartsParameterVMCollection).ToList();
            
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewItems is ICollection newCollection)
                        {
                            foreach (var item in newCollection)
                            {
                                if (item is PartsParameterViewModel parameter)
                                {
                                    if (parameter.BoxIndex != null && string.IsNullOrEmpty(parameter.DistributeName) && this.ProjectDistributeName == parameter.DistributeName)
                                    {
                                        var separateBox = StampingBoxPartsVM.SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxIndex == parameter.BoxIndex);
                                        if (separateBox != null && !separateBox.BoxPartsParameterVMCollection.Contains(parameter))
                                            separateBox.BoxPartsParameterVMCollection.Add(parameter);
                                    }
                                }
                            }
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
                                    if (parameter.BoxIndex != null && string.IsNullOrEmpty(parameter.DistributeName) && this.ProjectDistributeName == parameter.DistributeName)
                                    {
                                        var separateBox = StampingBoxPartsVM.SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxIndex == parameter.BoxIndex);
                                        if (separateBox != null && !separateBox.BoxPartsParameterVMCollection.Contains(parameter))
                                            separateBox.BoxPartsParameterVMCollection.Remove(parameter);
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

                            foreach (var (oldItem, newItem) in replacements)
                            {
                                if (allSeparateBoxPartsParameterVMCollection.Contains(oldItem))
                                {
                                    var separateBox = StampingBoxPartsVM.SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxPartsParameterVMCollection.Contains(oldItem));
                                   if(separateBox != null)
                                        separateBox.BoxPartsParameterVMCollection.AddOrReplace(x => x == oldItem, newItem);
                                }

                                //StampingBoxPartsVM.SelectedSeparateBoxVM
                            }
                        }
                    }
                    break;
                default:
                    break;
            }





        }

        /*
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
                                      if (!string.IsNullOrEmpty(partsParameter.DistributeName) && partsParameter.BoxIndex !=null)
                                      {
                                          var Box = StampingBoxPartsVM.SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxIndex == partsParameter.BoxIndex);
                                          if(Box!=null)
                                              Box.BoxPartsParameterVMCollection.Add(partsParameter);
                                      }
                                      else
                                      {
                                          if (!PartsParameterVMObservableCollection.Contains(partsParameter))
                                              PartsParameterVMObservableCollection.Add(partsParameter);

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

                                  var SCollection =  StampingBoxPartsVM.SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxPartsParameterVMCollection.Contains(partsParameter));
                                  if (SCollection != null)
                                      SCollection.BoxPartsParameterVMCollection.Remove(partsParameter);
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



                            //  var boxPartIndex = StampingBoxPartsVM.BoxPartsParameterVMCollection.IndexOf(oldItem);
                              var pParameter = StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x=>x.BoxPartsParameterVMCollection).FirstOrDefault(x=>x==oldItem);
                              if (pParameter != null)
                                  pParameter = newItem;
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
*/



        private void SyncProductProject(IList<ProductProjectViewModel> productProjectVMCollection)
        {
           foreach(var project in productProjectVMCollection)
            {
                project.ProductProjectIsFinishChanged -= Pproject_ProductProjectIsFinishChanged;
                project.ProductProjectIsFinishChanged += Pproject_ProductProjectIsFinishChanged;
            }


            var readyToAdd = productProjectVMCollection
                .Where(obj => obj.PartsParameterVMObservableCollection.Any(x => x.DistributeName == this.ProjectDistributeName))
                .Except(ReadyToTypeSettingProductProjectVMCollection).ToList();
            ReadyToTypeSettingProductProjectVMCollection.AddRange(readyToAdd);

            var notReadyToAdd = productProjectVMCollection
                .Except(ReadyToTypeSettingProductProjectVMCollection)
                .Except(NotReadyToTypeSettingProductProjectVMCollection).ToList();

            NotReadyToTypeSettingProductProjectVMCollection.AddRange(notReadyToAdd);

            // 找到 b 中不存在於 a 的元素
            var readyElementsToRemove = ReadyToTypeSettingProductProjectVMCollection.Except(productProjectVMCollection).ToList();
            // 刪除 b 中不存在於 a 的元素
            foreach (var element in readyElementsToRemove)
            {
                ReadyToTypeSettingProductProjectVMCollection.Remove(element);
            }

            var notReadyElementsToRemove = NotReadyToTypeSettingProductProjectVMCollection.Except(productProjectVMCollection).ToList();
            foreach (var element in notReadyElementsToRemove)
            {
                NotReadyToTypeSettingProductProjectVMCollection.Remove(element);
            }
        }

        private void Pproject_ProductProjectIsFinishChanged(object? sender, bool e)
        {
            _ = RefreshBoxPartsParameterVMRowFilterAsync();
            
        }


        [JsonIgnore]
        public static DevExpress.Mvvm.ICommand<DevExpress.Mvvm.Xpf.RowClickArgs> NotReadyRowDoubleClickCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<DevExpress.Mvvm.Xpf.RowClickArgs>((DevExpress.Mvvm.Xpf.RowClickArgs args) =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel ProjectItem)
                {
                    ProjectItem.IsMarked = !ProjectItem.IsMarked;
                }
            });
        }

        private ProductProjectViewModelObservableCollection? _notReadyToTypeSettingProductProjectVMCollection;
        public ProductProjectViewModelObservableCollection NotReadyToTypeSettingProductProjectVMCollection
        {
            get=>_notReadyToTypeSettingProductProjectVMCollection ??=new();
            private set
            {
                _notReadyToTypeSettingProductProjectVMCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ProductProjectViewModel> _readyToTypeSettingProductProjectVMCollection = new();
        /// <summary>
        /// 篩選後的專案
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<ProductProjectViewModel> ReadyToTypeSettingProductProjectVMCollection
        {
            get
            {
                _readyToTypeSettingProductProjectVMCollection ??= new();
                _readyToTypeSettingProductProjectVMCollection.CollectionChanged -= ReadyToTypeSettingProductProjectVMCollection_CollectionChanged;
               _readyToTypeSettingProductProjectVMCollection.CollectionChanged += ReadyToTypeSettingProductProjectVMCollection_CollectionChanged;
                return _readyToTypeSettingProductProjectVMCollection; 
            }
            private set
            {
                _readyToTypeSettingProductProjectVMCollection = value;
                if (_readyToTypeSettingProductProjectVMCollection!=null)
                {
                    _readyToTypeSettingProductProjectVMCollection.CollectionChanged -= ReadyToTypeSettingProductProjectVMCollection_CollectionChanged;
                    _readyToTypeSettingProductProjectVMCollection.CollectionChanged += ReadyToTypeSettingProductProjectVMCollection_CollectionChanged;
                }
                OnPropertyChanged();
            }
        }

         private ICommand?_addTypeSettingCommand;
        /// <summary>
        /// 新增排版專案
        /// </summary>
        [JsonIgnore]
        public ICommand AddTypeSettingCommand
        {
            get => _addTypeSettingCommand??=new AsyncRelayCommand<object>(async obj =>
            {
                try
                {
                    //新寫法
                    if (obj is ProductProjectViewModel ProjectDistributeVM)
                    {
                        //ProjectDistributeVM.IsMarked = true;
                        this.ProductProjectNameList.Add(ProjectDistributeVM.ProductProjectName);
                        this.ReadyToTypeSettingProductProjectVMCollection.Add(ProjectDistributeVM);
                        this.NotReadyToTypeSettingProductProjectVMCollection.Remove(ProjectDistributeVM);
                    }
                    await Task.Yield();
                }
                catch (Exception)
                {

                }
            });
        }





        private void ReadyToTypeSettingProductProjectVMCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

                var notExistedParts = AddParts.Except(StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x=>x.BoxPartsParameterVMCollection));
                var notExistedPartsGroup = notExistedParts.GroupBy(x => x.BoxIndex);
                foreach (var part in notExistedPartsGroup)
                {
                    var sBox = StampingBoxPartsVM.SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxIndex == part.Key);
                    sBox?.BoxPartsParameterVMCollection.AddRange(part.ToList());
                }

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

                var ExistedParts = AddParts.Intersect(StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x => x.BoxPartsParameterVMCollection));

                foreach (var part in ExistedParts)
                {
                    var BCollection = StampingBoxPartsVM.SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxPartsParameterVMCollection.Contains(part));
                    BCollection?.BoxPartsParameterVMCollection.Remove(part);
                }
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
                    Index = NotReadyToTypeSettingProductProjectVMCollection.FindIndex(x => x.IsMarked);
                    if (Index != -1)
                    {
                        this.ProjectDistribute.ProductProjectNameList.Add(NotReadyToTypeSettingProductProjectVMCollection[Index].ProductProjectName);
                        //  this.ProductProjectNameList.Add(NotReadyToTypeSettingProductProjectVMCollection[Index].ProductProjectName);
                        ReadyToTypeSettingProductProjectVMCollection.Add(NotReadyToTypeSettingProductProjectVMCollection[Index]);
                        NotReadyToTypeSettingProductProjectVMCollection.RemoveAt(Index);
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            });
        }





        //private PartsParameterViewModelObservableCollection? _partsParameterVMObservableCollection;
        /// <summary>
        /// GridControl ABC參數 沒放進箱子內的(等待分配的)
        /// </summary>
        /*[JsonIgnore]
        [Obsolete]
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
        }*/
        ///








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
                        var dragDropObject = e.Data.GetData(typeof(DevExpress.Xpf.Core.RecordDragDropData));
                        if (dragDropObject is DevExpress.Xpf.Core.RecordDragDropData DragDropData)
                        {
                            foreach (var _record in DragDropData.Records)
                            {
                                if (_record is PartsParameterViewModel PartsParameterVM)
                                {
                                    PartsParameterVM.DistributeName = string.Empty;
                                    PartsParameterVM.BoxIndex = null;
                                    e.Effects = System.Windows.DragDropEffects.Move;
                                }
                            }
                        }
                        //StampingBoxPartsVM.RefreshBoxPartsParameterVMRowFilter();
                        await SaveProductProjectVMCollectionAsync();
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
                        _ = SaveProductProjectVMCollectionAsync();
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


        class SeparateBoxValue
        {
            public SeparateBoxValue(SeparateBoxViewModel separateBox, int boxCurrentValue) { SeparateBox = separateBox; BoxCurrentValue = boxCurrentValue; }
            public  SeparateBoxViewModel SeparateBox { get;set; }
            public int BoxCurrentValue { get; set; }
        }

        [JsonIgnore]
        public AsyncRelayCommand ProjectGridControlInsertToBoxCommand
        {
            get => _projectGridControlInsertToBoxCommand ??= new(async () =>
            {
                await Task.Run(async () =>
                 {
                     try
                     {

                         int startIndex = StampingBoxPartsVM.SeparateBoxVMObservableCollection.FindIndex(x => x == StampingBoxPartsVM.SelectedSeparateBoxVM);
                         if (startIndex == -1)
                             startIndex = 0;
                         int stopIndex = startIndex - 1;
                         if (stopIndex == -1)
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

                         List<SeparateBoxValue> availableSeparateBoxList = new();
                         do
                         {
                             var currentElement = StampingBoxPartsVM.SeparateBoxVMObservableCollection[currentIndex];
                             if (currentElement.BoxIsEnabled && currentElement.BoxSliderValue >= 0)
                             {
                                 //設定各箱子的初始值
                                 var SBox = StampingBoxPartsVM.SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxIndex == currentElement.BoxIndex);
                                 int boxCount = 0;
                                 if (SBox != null)
                                     boxCount = SBox.BoxPartsParameterVMCollection.Count(x => !x.IsTransported && x.BoxIndex == currentElement.BoxIndex);

                                 availableSeparateBoxList.Add(new(currentElement, boxCount));
                             }
                             currentIndex = (currentIndex + 1) % StampingBoxPartsVM.SeparateBoxVMObservableCollection.Count;
                         }
                         while (currentIndex != ((stopIndex + 1) % StampingBoxPartsVM.SeparateBoxVMObservableCollection.Count));




                         List<PartsParameterViewModel> addPartsParameterViewModel = new();

                         int boxIndex = 0;
                         // foreach (var partsParameterVM in PartsParameterVMObservableCollection)
                         if (ReadyToTypeSettingProductProjectVMSelected != null)
                         {

                             var readyList = ReadyToTypeSettingProductProjectVMSelected.UnBoxPartsParameterVMObservableCollection.ToList();
                             //foreach (var partsParameterVM in readyList)
                             for (int i = 0; i < readyList.Count; i++)
                             {
                                 var partsParameterVM = readyList[i];

                                 var separateBox = availableSeparateBoxList[boxIndex].SeparateBox;

                                 partsParameterVM.DistributeName = StampingBoxPartsVM.ProjectDistributeName;
                                 partsParameterVM.BoxIndex = separateBox.BoxIndex;
                                 partsParameterVM.WorkIndex = -1;
                                 partsParameterVM.IsSended = false;
                                 partsParameterVM.IsTransported = false;

                                 addPartsParameterViewModel.Add(partsParameterVM);

                                 var SBox = StampingBoxPartsVM.SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxIndex == separateBox.BoxIndex);
                                 if (SBox != null)
                                 {
                                     //var boxCount = SBox.BoxPartsParameterVMCollection.Count(x => !x.IsTransported && x.BoxIndex == separateBox.BoxIndex);
                                     //餘數
                                     availableSeparateBoxList[boxIndex].BoxCurrentValue++;
                                     var boxCount = availableSeparateBoxList[boxIndex].BoxCurrentValue;

                                     var remainder = boxCount % (int)separateBox.BoxSliderValue;
                                     if (remainder == 0)
                                     {
                                         //箱子往後推一格
                                         boxIndex = (boxIndex + 1) % availableSeparateBoxList.Count;
                                     }
                                 }
                             } 



                             var addPartsParameterGroup = addPartsParameterViewModel.GroupBy(x => x.BoxIndex);
                             //foreach (var addPart in addPartsParameterGroup)
                             /*var options = new ParallelOptions()
                             {
                                 MaxDegreeOfParallelism = 150
                             };*/

                             //await Parallel.ForEachAsync(addPartsParameterGroup, options, async (addPart, token) =>  
                             foreach (var addPart in addPartsParameterGroup)
                             {
                                 try
                                 {
                                     if (addPart.Key != null)
                                     {
                                         var SBox = StampingBoxPartsVM.SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxIndex == addPart.Key);
                                         if (SBox != null)
                                         {
                                             var tmp = SBox.BoxPartsParameterVMCollection.ToList();
                                             tmp.AddRange(addPart.ToList());
                                             Application.Current.Dispatcher.Invoke(new Action(() =>
                                             {
                                                 SBox.BoxPartsParameterVMCollection = new(tmp);
                                             }));
                                         }
                                     }
                                 }
                                 catch (Exception ex)
                                 {
                                     _ = LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ex);

                                 }
                             }


                            /* foreach (var part in addPartsParameterViewModel)
                             {
                                 Application.Current.Dispatcher.Invoke(new Action(() =>
                                 {
                                     var remove = ReadyToTypeSettingProductProjectVMSelected.UnBoxPartsParameterVMObservableCollection.Remove(part);
                                 }));
                                 await Task.Delay(1);
                             }*/
                             var exceptCollection = ReadyToTypeSettingProductProjectVMSelected.UnBoxPartsParameterVMObservableCollection.Except(addPartsParameterViewModel);
                             ReadyToTypeSettingProductProjectVMSelected.UnBoxPartsParameterVMObservableCollection =new( exceptCollection);

                             await RefreshBoxPartsParameterVMRowFilterAsync();
                             await SaveProductProjectVMCollectionAsync();
                         }
                     }
                     catch (Exception ex)
                     {

                         Debug.WriteLine(ex);
                     }
                 });
            }, () => !ProjectGridControlInsertToBoxCommand.IsRunning);
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


                        foreach (var obj in ProductProjectVMCollection)
                        {
                            if (!ReadyToTypeSettingProductProjectVMCollection.Contains(obj))
                            {
                                if (!NotReadyToTypeSettingProductProjectVMCollection.Contains(obj))
                                {
                                    //被設為完成的不可加入
                                    if (obj.ProductProjectIsFinish)
                                    {

                                    }
                                    else
                                    {
                                        NotReadyToTypeSettingProductProjectVMCollection.Add(obj);
                                    }
                                }
                            }
                        }

                        //將被刪除的專案清除
                        var DelReadyList = ReadyToTypeSettingProductProjectVMCollection.Except(ProductProjectVMCollection).ToList();
                        var DelNotReadyList = NotReadyToTypeSettingProductProjectVMCollection.Except(ProductProjectVMCollection).ToList();

                        DelReadyList.ForEach(del =>
                        {
                            ReadyToTypeSettingProductProjectVMCollection.Remove(del);
                        });
                        DelNotReadyList.ForEach(del =>
                        {
                            NotReadyToTypeSettingProductProjectVMCollection.Remove(del);
                        });



                        int BFinishindex = 0;
                        while ((BFinishindex = NotReadyToTypeSettingProductProjectVMCollection.FindIndex(x => x.ProductProjectIsFinish)) != -1)
                        {
                            NotReadyToTypeSettingProductProjectVMCollection.RemoveAt(BFinishindex);
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
                if (StampingBoxPartsVM.SelectedSeparateBoxVM != null)
                {
                    if (ReadyToTypeSettingProductProjectVMSelected != null)
                    {
                        await Task.Run(async () =>
                        {
                            try
                            {
                                var oriMovableCollection = this.StampingBoxPartsVM.SelectedSeparateBoxVM.BoxPartsParameterVMCollection
                                ?.Where(x => (!x.IsFinish && !x.IsSended && !x.IsTransported)).ToList();
                                var movableCollection = oriMovableCollection?.Intersect(ReadyToTypeSettingProductProjectVMSelected.PartsParameterVMObservableCollection)?.ToList();

                                if (movableCollection?.Any() !=true)
                                {
                                    if(oriMovableCollection?.Any() == true)
                                    {
                                        await MessageBoxResultShow.ShowAsync(null, string.Empty, (string)Application.Current.TryFindResource("NoSelectCorrespondProductSetting"), MessageBoxButton.OK, GD_MessageBoxNotifyResult.NotifyRd);
                                    }
                                    else
                                    {
                                         //await MessageBoxResultShow.ShowAsync(null, string.Empty, (string)Application.Current.TryFindResource("NoSelectCorrespondProductSetting"), MessageBoxButton.OK, GD_MessageBoxNotifyResult.NotifyRd);
                                    }
                                    return;
                                }

                                if(movableCollection.Exists(x=>x.WorkIndex>=0))
                                {
                                    //如果有排定加工但尚未加工的
                                    var ret = await MessageBoxResultShow.ShowYesNoCancelAsync(null, string.Empty, (string)Application.Current.TryFindResource("AskCancelAlreadyScheduleProductSetting"), GD_MessageBoxNotifyResult.NotifyBl);
                                    if(ret == MessageBoxResult.Yes)
                                    {

                                    }
                                    else if (ret == MessageBoxResult.No)
                                    {
                                        movableCollection = movableCollection.Where(x => x.WorkIndex < 0).ToList();
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }


                                var options = new ParallelOptions()
                                {
                                    MaxDegreeOfParallelism = 100
                                };
                                Parallel.ForEach(movableCollection, options, moveableItem =>
                                {
                                    moveableItem.BoxIndex = null;
                                    moveableItem.WorkIndex = -1;
                                    moveableItem.DistributeName = null;
                                });

                                //依照專案名稱作分類->依序加回專案

                                var projectNameList = movableCollection.Select(x => x.ProductProjectName).Distinct();
                                foreach (var projectName in projectNameList)
                                {
                                    var projectVM = this.ProductProjectVMCollection.FirstOrDefault(x => x.ProductProjectName == projectName);
                                    var collection = movableCollection.Where(x => x.ProductProjectName == projectName);
                                    await Application.Current.Dispatcher.InvokeAsync(() =>
                                    {
                                        if (projectVM != null)
                                        {
                                            var tmp = projectVM.UnBoxPartsParameterVMObservableCollection.ToList();
                                            tmp.AddRange(collection);
                                            projectVM.UnBoxPartsParameterVMObservableCollection = new(tmp);

                                        }
                                    });

                                }

                                var origin = StampingBoxPartsVM.SelectedSeparateBoxVM.BoxPartsParameterVMCollection!.ToList();
                                StampingBoxPartsVM.SelectedSeparateBoxVM.BoxPartsParameterVMCollection = new(origin.Except(movableCollection));

                                _ = RefreshBoxPartsParameterVMRowFilterAsync();

                            }
                            catch (Exception ex)
                            {
                               _ = LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ex, true);
                                Debug.WriteLine(ex.ToString());
                            }
                            finally
                            {
                            }
                        });
                    }
                    else
                    {
                        await MessageBoxResultShow.ShowAsync(null, string.Empty, (string)Application.Current.TryFindResource("NoSelectProductSetting"), MessageBoxButton.OK, GD_MessageBoxNotifyResult.NotifyRd);
                    }
                }



            });
        }

        private bool _showIsTransported;
        public bool ShowIsTransported
        {
            get => _showIsTransported; set
            {
                _showIsTransported = value;
                OnPropertyChanged();
                _ = RefreshBoxPartsParameterVMRowFilterAsync();
            }

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
                        if (PParameter.DistributeName == this.ProjectDistributeName &&
                     PParameter.BoxIndex == StampingBoxPartsVM.SelectedSeparateBoxVM.BoxIndex
                     && (!PParameter.IsTransported || ShowIsTransported))
                            args.Visible = true;
                        else
                            args.Visible = false;
                    }
                }
            });
        }








    }
}
