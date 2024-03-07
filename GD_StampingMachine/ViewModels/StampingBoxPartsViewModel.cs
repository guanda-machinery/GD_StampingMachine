﻿
using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Xpf;
using DevExpress.Utils.Extensions;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels.ParameterSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class StampingBoxPartModel
    {
        public string? ProjectDistributeName { get; set; }
        /// <summary>
        /// 盒子列表
        /// </summary>
        //public ObservableCollection<SeparateBoxViewModel>? SeparateBoxVMObservableCollection { get; set; }

       // [JsonIgnore]
        //public ObservableCollection<ProductProjectViewModel>? ProductProjectVMObservableCollection { get; set; }

       // [JsonIgnore]
        //public bool GridControl_MachiningStatusColumnVisible { get; set; } = false;
    }


    public class StampingBoxPartsViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingBoxPartsViewModel");

        public StampingBoxPartsViewModel(StampingBoxPartModel stampingBoxPart)
        {
            StampingBoxPart = stampingBoxPart;
            _ = ReLoadBoxPartsParameterVMCollectionAsync();
        }










        public async Task ReLoadBoxPartsParameterVMCollectionAsync()
        {
            var newCollection = new ObservableCollection<PartsParameterViewModel>();
            if (ProductProjectVMObservableCollection != null)
            {
                ProductProjectVMObservableCollection.ForEach(productProject =>
                {
                    productProject.PartsParameterVMCollection.ForEach((productProjectPartViewModel) =>
                    {
                        if (productProjectPartViewModel.BoxIndex.HasValue)
                            if (productProjectPartViewModel.DistributeName == StampingBoxPart.ProjectDistributeName)
                                if (!newCollection.Contains(productProjectPartViewModel))
                                    newCollection.Add(productProjectPartViewModel);
                    });
                });
            }
            BoxPartsParameterVMCollection = new();
            foreach (var pParameter in newCollection)
            {
                await Application.Current?.Dispatcher.InvokeAsync(() =>
                {
                    BoxPartsParameterVMCollection.Add(pParameter);
                });
            }
        }




        public string ProjectDistributeName { get => StampingBoxPart.ProjectDistributeName; set => StampingBoxPart.ProjectDistributeName = value; }


        public StampingBoxPartModel StampingBoxPart = new();



        private SeparateBoxViewModel? _selectedSeparateBoxVM;
        /// <summary>
        /// 選擇的盒子
        /// </summary>
        public SeparateBoxViewModel? SelectedSeparateBoxVM
        {
            get
            {
                if (_selectedSeparateBoxVM == null)
                    if (SeparateBoxVMCollection != null)
                        _selectedSeparateBoxVM = SeparateBoxVMCollection.FirstOrDefault(x => x.BoxIsEnabled);
                return _selectedSeparateBoxVM;
            }
            set
            {
                _selectedSeparateBoxVM = value;
                OnPropertyChanged();
            }
        }



        private SeparateBoxExtViewModelObservableCollection _separateBoxVMCollection;
        /// <summary>
        /// 盒子列表
        /// </summary>
        public SeparateBoxExtViewModelObservableCollection SeparateBoxVMCollection
        {
            get => _separateBoxVMCollection;
            set { _separateBoxVMCollection = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ProductProjectViewModel> _productProjectVMObservableCollection;
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get => _productProjectVMObservableCollection;
            set
            {
                _productProjectVMObservableCollection = value;
                OnPropertyChanged();
            }
        }


        //private PartsParameterViewModelObservableCollection? periousBoxPartsParameterVMCollection;
        /// <summary>
        /// 盒子內的專案
        /// </summary>
        private PartsParameterViewModelObservableCollection? _boxPartsParameterVMCollection;

        public PartsParameterViewModelObservableCollection BoxPartsParameterVMCollection
        {
            get
            {
                if (_boxPartsParameterVMCollection==null)
                    _boxPartsParameterVMCollection = new PartsParameterViewModelObservableCollection();
                return _boxPartsParameterVMCollection;
            }
            set
            {
                _boxPartsParameterVMCollection = value;
                OnPropertyChanged();
            }
        }

        private void BoxPartsParameterVMCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateSeparateBoxValue();
        }
        
        /// <summary>
        /// 更新箱子內的數值
        /// </summary>
        private void UpdateSeparateBoxValue()
        {
            try
            {
                for (int i = 0; i < SeparateBoxVMCollection.Count; i++)
                {
                    var separateBox = SeparateBoxVMCollection[i];
                    var indexCollection = BoxPartsParameterVMCollection.Where(x => x.BoxIndex == separateBox.BoxIndex).ToList();
                    //已排定
                    separateBox.BoxPieceValue = indexCollection.Count;
                    //加工完成但尚未被移除的
                    separateBox.UnTransportedFinishedBoxPieceValue = indexCollection.FindAll(x => x.IsFinish && !x.IsTransported).Count;
                    //已經被分配加工且尚未被移除
                    separateBox.UnTransportedBoxPieceValue = indexCollection.FindAll(x => x.WorkIndex >= 0 && !x.IsTransported).Count;
                    //只有已完成
                    separateBox.FinishedBoxPieceValue = indexCollection.FindAll(x => x.IsFinish).Count;
                    separateBox.UnFinishedBoxPieceValue = indexCollection.FindAll(x => !x.IsFinish).Count;
                }
            }
            catch
            {

            }
        }





        private ICommand?_gridControlSizeChangedCommand;
        [JsonIgnore]
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
                            var pageSize = ((tableview.ActualHeight - tableview.HeaderPanelMinHeight - 30) / 45);
                            tableview.PageSize = (pageSize < 3 ? 3 : (int)pageSize);
                        }
                    }
                }
            });
        }





        [JsonIgnore]
        public ICommand Box_OnDragRecordOverCommand
        {
            get => GD_Command.Box_OnDragRecordOverCommand;
        }

        [JsonIgnore]
        public ICommand Box_OnDropRecordCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                try
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
                                    if (SelectedSeparateBoxVM != null)
                                    {
                                        PartsParameterVM.DistributeName = ProjectDistributeName;// ProjectDistribute.ProjectDistributeName;
                                        PartsParameterVM.BoxIndex = SelectedSeparateBoxVM.BoxIndex;
                                        PartsParameterVM.WorkIndex = -1;
                                        e.Effects = System.Windows.DragDropEffects.Move;
                                        //RefreshBoxPartsParameterVMRowFilter();
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {

                }

            });
        }





    }
}
