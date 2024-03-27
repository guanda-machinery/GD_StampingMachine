using DevExpress.DataAccess.Json;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Extensions;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraTreeList.Internal;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.ViewModels.MachineMonitor;
using GD_StampingMachine.ViewModels.ProductSetting;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static Opc.Ua.NodeState;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 分料盒資料結構
    /// </summary>
    public class SeparateBoxViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SeparateBoxViewModel");

        public SeparateBoxViewModel(SeparateBoxModel separateBox)
        {
            SeparateBox = separateBox;
        }
       
        [JsonIgnore]
        public virtual SeparateBoxModel SeparateBox { get; protected set; } = new();

        public SeparateBoxViewModel()
        {
            SeparateBox = new SeparateBoxModel();
        }

        public int BoxIndex
        {
            get => SeparateBox.BoxIndex;
            set
            {
                SeparateBox.BoxIndex = value;
                OnPropertyChanged();
            }
        }



        /// <summary>
        /// 箱子的容量
        /// </summary>
        public double BoxSliderValue
        {
            get => SeparateBox.BoxSliderValue;
            set
            {
                SeparateBox.BoxSliderValue = value;
                OnPropertyChanged();
                BoxSliderValueChanged?.Invoke(this, value);
            }
        }
        public static event EventHandler<double>? BoxSliderValueChanged;
        /// <summary>
        /// 盒子可用/不可用
        /// </summary>
        public bool BoxIsEnabled
        {
            get => SeparateBox.BoxIsEnabled;
            set
            {
                SeparateBox.BoxIsEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool IsUsing
        {
            get => SeparateBox.BoxIsUsing;
            set
            {
                SeparateBox.BoxIsUsing = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 分料盒與鐵片資料結構
    /// </summary>
    public class SeparateBoxExtViewModel : SeparateBoxViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SeparateBoxViewModel");


        static SeparateBoxExtViewModel()
        {
            BoxSliderValueChanged += SeparateBoxExtViewModel_BoxSliderValueChanged;
        }

        public SeparateBoxExtViewModel(SeparateBoxViewModel separateVM) : base(separateVM.SeparateBox)
        {

        }

        public SeparateBoxExtViewModel(SeparateBoxModel separateBox) : base(separateBox)
        {

        }

        public SeparateBoxExtViewModel() : base()
        {

        }

        private PartsParameterViewModelObservableCollection? _boxPartsParameterVMCollection;
        /// <summary>
        /// 盒子內的專案(未區分是否排定加工/未加工)
        /// </summary>
        public PartsParameterViewModelObservableCollection BoxPartsParameterVMCollection
        {
            get
            {
                _boxPartsParameterVMCollection??= new PartsParameterViewModelObservableCollection();
                if (_boxPartsParameterVMCollection != null)
                {
                    _boxPartsParameterVMCollection.CollectionChanged -= BoxPartsParameterVMCollection_CollectionChanged;
                    _boxPartsParameterVMCollection.CollectionChanged += BoxPartsParameterVMCollection_CollectionChanged;
                }
                return _boxPartsParameterVMCollection;
            }
            set
            {
                var devalue = _boxPartsParameterVMCollection;

                _boxPartsParameterVMCollection = value;
                if (_boxPartsParameterVMCollection != null)
                {
                    _boxPartsParameterVMCollection.CollectionChanged -= BoxPartsParameterVMCollection_CollectionChanged;
                    _boxPartsParameterVMCollection.CollectionChanged += BoxPartsParameterVMCollection_CollectionChanged;
                    Parallel.ForEach(_boxPartsParameterVMCollection, item =>
                    {
                        item.IsFinishChangedAsync -= Item_IsFinishChanged;
                        item.IsFinishChangedAsync += Item_IsFinishChanged;

                        item.IsTransportedChangedAsync -= Item_IsTransportedChanged;
                        item.IsTransportedChangedAsync += Item_IsTransportedChanged;
                    });
                }
                _ = UpdateSeparateBoxValueAsync();
                OnPropertyChanged();
                BoxPartsParameterVMCollectionChanged?.Invoke(this, new(devalue , _boxPartsParameterVMCollection));
            }
        }

        public event EventHandler<GD_CommonLibrary.ValueChangedEventArgs<PartsParameterViewModelObservableCollection>?>? BoxPartsParameterVMCollectionChanged;







        // public event EventHandler? PartsParameterStateChanged;
        /*private void Item_StateChanged(object? sender, EventArgs e)
        {
            PartsParameterStateChanged?.Invoke(this, new EventArgs());
        }*/

        public event EventHandler? PartsParameterIsFinishChanged;
        private void Item_IsFinishChanged(object? sender, bool e)
        {
            PartsParameterIsFinishChanged?.Invoke(this, new EventArgs());
            _ = UpdateSeparateBoxValueAsync();
        }

        public event EventHandler? PartsParameterIsTransportedChanged;
        private void Item_IsTransportedChanged(object? sender, bool e)
        {
            PartsParameterIsTransportedChanged?.Invoke(this, new EventArgs());
            _ = UpdateSeparateBoxValueAsync();
        }



        private void BoxPartsParameterVMCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var NParts = e.NewItems as ICollection;
            var OParts = e.OldItems as ICollection;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (NParts is ICollection newCollection)
                        {
                            foreach (var part in newCollection)
                            {
                                //if (part is INotifyPropertyChanged item)
                                if (part is PartsParameterViewModel item)
                                {
                                  //  item.PropertyChanged += Item_PropertyChanged;
                                    item.IsFinishChangedAsync -= Item_IsFinishChanged; ;
                                    item.IsTransportedChangedAsync -= Item_IsTransportedChanged; ;
                                    item.IsFinishChangedAsync += Item_IsFinishChanged; ;
                                    item.IsTransportedChangedAsync += Item_IsTransportedChanged; ;
                                }
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.OldItems is ICollection oldCollection)
                        {
                            foreach (var part in oldCollection)
                            {
                                if (part is PartsParameterViewModel item)
                                {
                                    // item.PropertyChanged += Item_PropertyChanged;
                                    item.IsFinishChangedAsync -= Item_IsFinishChanged; ;
                                    item.IsTransportedChangedAsync -= Item_IsTransportedChanged; ;
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
                                oldItem.IsFinishChangedAsync -= Item_IsFinishChanged;
                                oldItem.IsTransportedChangedAsync -= Item_IsTransportedChanged;

                                newItem.IsFinishChangedAsync += Item_IsFinishChanged; ;
                                newItem.IsTransportedChangedAsync += Item_IsTransportedChanged; ;
                                //oldItem.PropertyChanged -= Item_PropertyChanged;
                                //newItem.PropertyChanged += Item_PropertyChanged;

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
            _ = UpdateSeparateBoxValueAsync();
        }

        /// <summary>
        /// 更新箱子內的數值
        /// </summary>
        private  async Task UpdateSeparateBoxValueAsync()
        {
            await Task.Run(() =>
            {
                //已排定
                //this.BoxPieceValue = BoxPartsParameterVMCollection.Count;
                OnPropertyChanged(nameof(this.BoxPieceValue));
                //= BoxPartsParameterVMCollection.Count;
                //加工完成但尚未被移除的
                OnPropertyChanged(nameof(this.UnTransportedFinishedBoxPieceValue));
                //已經被分配加工且尚未被移除
                //  this.UnTransportedBoxPieceValue = BoxPartsParameterVMCollection.Count(x => x.WorkIndex >= 0 && !x.IsTransported);
                OnPropertyChanged(nameof(this.UnTransportedBoxPieceValue));
                //只有已完成
                // this.FinishedBoxPieceValue = 
                OnPropertyChanged(nameof(this.FinishedBoxPieceValue));
                OnPropertyChanged(nameof(this.UnFinishedBoxPieceValue));
            });
        }



        private static void SeparateBoxExtViewModel_BoxSliderValueChanged(object? sender, double e)
        {
            if(sender is SeparateBoxExtViewModel separateBoxExt)
            {
                separateBoxExt.CheckBoxIsFull();
            }
        }

        [JsonIgnore]
        public bool BoxIsFull
        {
            get
            {
                if (UnTransportedFinishedBoxPieceValue > 0)
                    return UnTransportedFinishedBoxPieceValue >= BoxSliderValue;
                else
                    return false;
            }
        }
        private void CheckBoxIsFull()
        {
            OnPropertyChanged(nameof(BoxIsFull));
        }


        //private double _boxPieceValue;
        /// <summary>
        /// 箱子內分配到加工的值
        /// </summary>
        [JsonIgnore]
        public double BoxPieceValue
        {
            get => BoxPartsParameterVMCollection.Count;
        }

       // private double _unTransportedFinishedBoxPieceValue;
        /// <summary>
        /// 箱子內已加工但尚未被移除的鐵牌
        /// </summary>
        [JsonIgnore]
        public double UnTransportedFinishedBoxPieceValue
        {
            get => BoxPartsParameterVMCollection.Count(x => x.IsFinish && !x.IsTransported);
        }

        //private double _unTransportedBoxPieceValue;
        /// <summary>
        /// 箱子內已被分配的鐵片
        /// </summary>
        [JsonIgnore]
        public double UnTransportedBoxPieceValue
        {
            get => BoxPartsParameterVMCollection.Count(x => x.WorkIndex >= 0 && !x.IsTransported);
        }



        //private double _finishedBoxPieceValue;
        /// <summary>
        /// 箱子內已加工的鐵牌
        /// </summary>
        [JsonIgnore]
        public double FinishedBoxPieceValue
        {
            get => BoxPartsParameterVMCollection.Count(x => x.IsFinish);
        }


       //private double _unFinishedBoxPieceValue;
        /// <summary>
        /// 箱子內未加工的鐵牌
        /// </summary>
        [JsonIgnore]
        public double UnFinishedBoxPieceValue
        {
            get => BoxPartsParameterVMCollection.Count(x => !x.IsFinish);
        }



    }

    /// <summary>
    /// 箱子集合體
    /// </summary>
    public class SeparateBoxExtViewModelObservableCollection : ObservableCollection<SeparateBoxExtViewModel>
    {
        public SeparateBoxExtViewModelObservableCollection():base()
        {
        }
        public SeparateBoxExtViewModelObservableCollection(IEnumerable<SeparateBoxViewModel> collection) : base()
        {

            if (collection == null)
            {
                return;
            }

            foreach (var item in collection)
            {
                var NewSeparateBoxExtViewModel = new SeparateBoxExtViewModel(item);
                NewSeparateBoxExtViewModel.BoxPartsParameterVMCollection.CollectionChanged += BoxPartsParameterVMCollection_CollectionChanged;
                NewSeparateBoxExtViewModel.BoxPartsParameterVMCollectionChanged += BoxPartsParameterVMCollectionChanged; ;

                foreach(var part in NewSeparateBoxExtViewModel.BoxPartsParameterVMCollection)
                {
                    this.AddPartsParameter(part);
                }
                base.Items.Add(NewSeparateBoxExtViewModel);
            }
        }
        public SeparateBoxExtViewModelObservableCollection(IEnumerable<SeparateBoxExtViewModel> collection) : base(collection)
        {
            foreach (var item in collection)
            {
                item.BoxPartsParameterVMCollection.CollectionChanged += BoxPartsParameterVMCollection_CollectionChanged;
                item.BoxPartsParameterVMCollectionChanged += BoxPartsParameterVMCollectionChanged;

                foreach (var part in item.BoxPartsParameterVMCollection)
                {
                    this.AddPartsParameter(part);
                    //part.PropertyChanged += NewPartsParameter_PropertyChanged; 
                }
            }
        }




        private void BoxPartsParameterVMCollectionChanged(object? sender, GD_CommonLibrary.ValueChangedEventArgs<PartsParameterViewModelObservableCollection>? e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.CollectionChanged -= BoxPartsParameterVMCollection_CollectionChanged;
                foreach (var part in e.OldValue)
                {
                    this.RemovePartsParameter(part);
                }
            }

            if (e.NewValue != null)
            {
                e.NewValue.CollectionChanged += BoxPartsParameterVMCollection_CollectionChanged;
                foreach (var part in e.NewValue)
                {
                    this.AddPartsParameter(part);
                }
            }
        }


        /// <summary>
        /// 內部的PartsParameter屬性被變更
        /// </summary>
        public event EventHandler? InternalPartsParameterStateChanged;


        public static implicit operator SeparateBoxExtViewModelObservableCollection(ObservableCollection<SeparateBoxViewModel> v)
        {
            if (v == null)
            {
                return null;
            }
            else
            {
                var op  = new SeparateBoxExtViewModelObservableCollection();
                foreach(var item in v)
                {
                    op.Add(new SeparateBoxExtViewModel(item));
                }
                return op;
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var newItems = e.NewItems as ICollection;
            var oldItems = e.OldItems as ICollection;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (newItems != null)
                    {
                        foreach (var item in newItems)
                        {
                            if (item is SeparateBoxExtViewModel separateBox)
                            {
                                separateBox.BoxPartsParameterVMCollection.CollectionChanged += BoxPartsParameterVMCollection_CollectionChanged;
                                separateBox.BoxPartsParameterVMCollectionChanged += BoxPartsParameterVMCollectionChanged;

                                foreach (var part in separateBox.BoxPartsParameterVMCollection)
                                {
                                    this.AddPartsParameter(part);
                                }
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (oldItems != null)
                        {
                            foreach (var item in oldItems)
                            {
                                if (item is SeparateBoxExtViewModel separateBox)
                                {
                                    separateBox.BoxPartsParameterVMCollection.CollectionChanged -= BoxPartsParameterVMCollection_CollectionChanged;

                                    foreach (var part in separateBox.BoxPartsParameterVMCollection)
                                    {
                                        this.RemovePartsParameter(part);
                                        //part.PropertyChanged -= NewPartsParameter_PropertyChanged;
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
                            var replacements = oldCollection.OfType<SeparateBoxExtViewModel>()
                            .Zip(newCollection.OfType<SeparateBoxExtViewModel>(), (oldItem, newItem) => (oldItem, newItem));
                            foreach (var (oldItem, newItem) in replacements)
                            {
                                oldItem.BoxPartsParameterVMCollection.CollectionChanged -= BoxPartsParameterVMCollection_CollectionChanged;
                                newItem.BoxPartsParameterVMCollection.CollectionChanged += BoxPartsParameterVMCollection_CollectionChanged;
                                
                                foreach (var Opart in oldItem.BoxPartsParameterVMCollection)
                                {
                                  //  Opart.PropertyChanged -= NewPartsParameter_PropertyChanged;
                                    this.RemovePartsParameter(Opart);
                                }

                                foreach (var Npart in newItem.BoxPartsParameterVMCollection)
                                {
                               //     Npart.PropertyChanged -= NewPartsParameter_PropertyChanged;
                                    this.AddPartsParameter(Npart);
                                }
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



            base.OnCollectionChanged(e);
        }

        private void SeparateBox_BoxPartsParameterVMCollectionChanged(object? sender, GD_CommonLibrary.ValueChangedEventArgs<PartsParameterViewModelObservableCollection>? e)
        {
            throw new NotImplementedException();
        }

        private void BoxPartsParameterVMCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var newItems = e.NewItems as ICollection;
            var oldItems = e.OldItems as ICollection;
            
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (newItems != null)
                    {
                        foreach (var item in newItems)
                        {
                            if (item is PartsParameterViewModel partsParameter)
                            {
                                this.AddPartsParameter(partsParameter);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (oldItems != null)
                    {
                        foreach (var item in oldItems)
                        {
                            if (item is PartsParameterViewModel partsParameter)
                            {
                                this.RemovePartsParameter(partsParameter);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (oldItems != null && newItems!=null)
                    {
                        var replacements = oldItems.OfType<PartsParameterViewModel>()
                        .Zip(newItems.OfType<PartsParameterViewModel>(), (oldItem, newItem) => (oldItem, newItem));
                        foreach (var (oldItem, newItem) in replacements)
                        {
                            this.ReplacePartsParameter(oldItem, newItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                default:
                    break;
            }
        }

        private void AddPartsParameter(PartsParameterViewModel partsParameter)
        {
            if (partsParameter.IsScheduled)
                ScheduledPartsParameterCollection.Add(partsParameter);
            else
                UnscheduledPartsParameterCollection.Add(partsParameter);
        }

        private void RemovePartsParameter(PartsParameterViewModel partsParameter)
        {
            // partsParameter.PropertyChanged -= NewPartsParameter_PropertyChanged;
            if (ScheduledPartsParameterCollection.Contains(partsParameter))
                ScheduledPartsParameterCollection.Remove(partsParameter);
            else if (UnscheduledPartsParameterCollection.Contains(partsParameter))
                UnscheduledPartsParameterCollection.Remove(partsParameter);
        }


        private void ReplacePartsParameter(PartsParameterViewModel oldPartsParameter, PartsParameterViewModel newPartsParameter)
        {
            var scheduledIndex = ScheduledPartsParameterCollection.IndexOf(oldPartsParameter);
            var unscheduledIndex = UnscheduledPartsParameterCollection.IndexOf(oldPartsParameter);
            /*
            if (oldPartsParameter.IsScheduled == newPartsParameter.IsScheduled)
            {
                if (ScheduledPartsParameterCollection.Contains(oldPartsParameter))
                {
                    if(scheduledIndex!=-1)
                        ScheduledPartsParameterCollection[scheduledIndex] = newPartsParameter;
                }
                else if (UnscheduledPartsParameterCollection.Contains(oldPartsParameter))
                {
                    if (unscheduledIndex != -1)
                        UnscheduledPartsParameterCollection[unscheduledIndex] = newPartsParameter;
                }
            }*/
            if (oldPartsParameter.IsScheduled == newPartsParameter.IsScheduled)
            {
                var sIndex = ScheduledPartsParameterCollection.IndexOf(oldPartsParameter);
                if (sIndex != -1)
                {
                    ScheduledPartsParameterCollection[sIndex] = newPartsParameter;
                }

                var unsIndex = UnscheduledPartsParameterCollection.IndexOf(oldPartsParameter); 
                if (sIndex != -1)
                {
                    UnscheduledPartsParameterCollection[unsIndex] = newPartsParameter;
                }
            }
            else
            {
                RemovePartsParameter(oldPartsParameter);
                AddPartsParameter(newPartsParameter);
            }
        }
        
        /*private void NewPartsParameter_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is PartsParameterViewModel Part)
            {
               Application.Current.Dispatcher.Invoke(() =>
               {
                   if (Part.IsScheduled)
                   {
                       if (UnscheduledPartsParameterCollection.Contains(Part))
                           UnscheduledPartsParameterCollection.Remove(Part);

                       if (!ScheduledPartsParameterCollection.Contains(Part))
                           ScheduledPartsParameterCollection.Add(Part);
                   }
                   else
                   {
                       if (ScheduledPartsParameterCollection.Contains(Part))
                           ScheduledPartsParameterCollection.Remove(Part);

                       if (!UnscheduledPartsParameterCollection.Contains(Part))
                           UnscheduledPartsParameterCollection.Add(Part);
                   }
               });
            }
        }*/
        
        /// <summary>
        /// 未排加工的鐵牌
        /// </summary>
        public PartsParameterViewModelObservableCollection UnscheduledPartsParameterCollection
        {
            get => _unscheduledPartsParameterCollection ??= new();
            set
            {
                _unscheduledPartsParameterCollection = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(UnscheduledPartsParameterCollection)));
            }
        }
        PartsParameterViewModelObservableCollection _unscheduledPartsParameterCollection;


        /// <summary>
        /// 已排加工的鐵牌
        /// </summary>
        public PartsParameterViewModelObservableCollection ScheduledPartsParameterCollection
        {
            get => _scheduledPartsParameterCollection ??= new();
            set
            {
                _scheduledPartsParameterCollection = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(ScheduledPartsParameterCollection)));
            }
        }
        PartsParameterViewModelObservableCollection _scheduledPartsParameterCollection;



    }







}
