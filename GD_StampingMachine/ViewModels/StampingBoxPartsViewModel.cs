
using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Xpf;
using DevExpress.Utils.Extensions;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels.ParameterSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class StampingBoxPartsViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingBoxPartsViewModel");
        public StampingBoxPartsViewModel(SeparateBoxExtViewModelObservableCollection separateBoxExtViewModelCollection)
        {
            ProjectDistributeName = string.Empty;
            SeparateBoxVMObservableCollection = separateBoxExtViewModelCollection;
            UpdateSeparateBoxValue();
        }

        public StampingBoxPartsViewModel(string? projectDistributeName, SeparateBoxExtViewModelObservableCollection separateBoxExtViewModelCollection)
        {
            ProjectDistributeName = projectDistributeName;
            SeparateBoxVMObservableCollection = separateBoxExtViewModelCollection;
            UpdateSeparateBoxValue();
        }

        public StampingBoxPartsViewModel(string? projectDistributeName, SeparateBoxExtViewModelObservableCollection separateBoxExtViewModelCollection , PartsParameterViewModelObservableCollection boxPartsParameterVMCollection)
        {
            ProjectDistributeName = projectDistributeName;
            SeparateBoxVMObservableCollection = separateBoxExtViewModelCollection;
            BoxPartsParameterVMCollection = boxPartsParameterVMCollection;
            UpdateSeparateBoxValue();
        }







        private string? _projectDistributeName;
        public string? ProjectDistributeName { get => _projectDistributeName; set { _projectDistributeName = value; OnPropertyChanged(); } }


        //public StampingBoxPartModel StampingBoxPart = new();



        private SeparateBoxViewModel? _selectedSeparateBoxVM;
        /// <summary>
        /// 選擇的盒子
        /// </summary>
        public SeparateBoxViewModel? SelectedSeparateBoxVM
        {
            get
            {
                if (_selectedSeparateBoxVM == null)
                    if (SeparateBoxVMObservableCollection != null)
                    { 
                        _selectedSeparateBoxVM = SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxIsEnabled);

                    }
                return _selectedSeparateBoxVM;
            }
            set
            {
                _selectedSeparateBoxVM = value;
                OnPropertyChanged();
            }
        }



        private SeparateBoxExtViewModelObservableCollection _separateBoxVMObservableCollection;
        /// <summary>
        /// 盒子列表
        /// </summary>
        public SeparateBoxExtViewModelObservableCollection SeparateBoxVMObservableCollection
        {
            get => _separateBoxVMObservableCollection;
            set { _separateBoxVMObservableCollection = value; OnPropertyChanged(); }
        }

        //public ObservableCollection<ProductProjectViewModel> _productProjectVMObservableCollection;
        /// <summary>
        /// 
        /// </summary>
        /*public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get => _productProjectVMObservableCollection;
            set
            {
                _productProjectVMObservableCollection = value;
                OnPropertyChanged();
            }
        }*/



        private PartsParameterViewModelObservableCollection? _boxPartsParameterVMCollection;
        //private PartsParameterViewModelObservableCollection? periousBoxPartsParameterVMCollection;
        /// <summary>
        /// 盒子內的專案
        /// </summary>
        public PartsParameterViewModelObservableCollection BoxPartsParameterVMCollection
        {
            get
            {
                _boxPartsParameterVMCollection??= new PartsParameterViewModelObservableCollection();

                _boxPartsParameterVMCollection.CollectionChanged -= BoxPartsParameterVMCollection_CollectionChanged;
                _boxPartsParameterVMCollection.CollectionChanged += BoxPartsParameterVMCollection_CollectionChanged;

                foreach (var part in _boxPartsParameterVMCollection)
                {
                    part.StateChanged -= Item_StateChanged;
                    part.IsFinishChanged -= ParameterChanged; ;
                    part.WorkIndexChanged -= ParameterChanged;
                    part.IsTransportedChanged -= ParameterChanged;

                    part.StateChanged += Item_StateChanged;
                    part.IsFinishChanged += ParameterChanged; ;
                    part.WorkIndexChanged += ParameterChanged;
                    part.IsTransportedChanged += ParameterChanged;
                }

                return _boxPartsParameterVMCollection;
            }
            set
            {
                _boxPartsParameterVMCollection = value;

                if (_boxPartsParameterVMCollection != null)
                {
                    _boxPartsParameterVMCollection.CollectionChanged -= BoxPartsParameterVMCollection_CollectionChanged;
                    _boxPartsParameterVMCollection.CollectionChanged += BoxPartsParameterVMCollection_CollectionChanged;
                    foreach (var part in _boxPartsParameterVMCollection)
                    {
                        part.StateChanged -= Item_StateChanged;
                        part.IsFinishChanged -= ParameterChanged; ;
                        part.WorkIndexChanged -= ParameterChanged;
                        part.IsTransportedChanged -= ParameterChanged;

                        part.StateChanged += Item_StateChanged;
                        part.IsFinishChanged += ParameterChanged; ;
                        part.WorkIndexChanged += ParameterChanged;
                        part.IsTransportedChanged += ParameterChanged;
                    }
                }


                UpdateSeparateBoxValue();
                OnPropertyChanged();
            }
        }



        private void BoxPartsParameterVMCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateSeparateBoxValue();
            if(e.NewItems is ICollection NParts)
            {
                foreach (var part in NParts)
                {
                    if (part is PartsParameterViewModel item)
                    {
                        item.StateChanged += Item_StateChanged;
                        item.IsFinishChanged += ParameterChanged; ;
                        item.WorkIndexChanged += ParameterChanged;
                        item.IsTransportedChanged += ParameterChanged;
                    }
                }
            }

            if (e.OldItems is ICollection OParts)
            {
                foreach (var part in OParts)
                {
                    if (part is PartsParameterViewModel item)
                    {
                        item.StateChanged -= Item_StateChanged;
                        item.IsFinishChanged -= ParameterChanged; ;
                        item.WorkIndexChanged -= ParameterChanged;
                        item.IsTransportedChanged -= ParameterChanged;
                    }
                }
            }

        }

        private void Item_StateChanged(object? sender, EventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 內部參數變動通知
        /// </summary>
        public event EventHandler? StateChanged;




        private void ParameterChanged<T>(object? sender, T e)
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
                if (SeparateBoxVMObservableCollection != null)
                {
                    for (int i = 0; i < SeparateBoxVMObservableCollection.Count; i++)
                    {
                        var separateBox = SeparateBoxVMObservableCollection[i];
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
