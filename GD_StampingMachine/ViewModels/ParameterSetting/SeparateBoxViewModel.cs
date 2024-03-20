using DevExpress.DataAccess.Json;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Extensions;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraTreeList.Internal;
using GD_StampingMachine.GD_Model;
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
using System.Threading.Tasks;
using System.Windows;

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
                _boxPartsParameterVMCollection ??= new PartsParameterViewModelObservableCollection();
                SubscriberBoxPartsParameterChange(_boxPartsParameterVMCollection);
                return _boxPartsParameterVMCollection;
            }
            set
            {
                _boxPartsParameterVMCollection = value;
                SubscriberBoxPartsParameterChange(_boxPartsParameterVMCollection);
                UpdateSeparateBoxValue();
                OnPropertyChanged();
            }
        }

 

        private void SubscriberBoxPartsParameterChange(PartsParameterViewModelObservableCollection collection)
        {
            if (collection != null)
            {
                collection.CollectionChanged -= BoxPartsParameterVMCollection_CollectionChanged;
                collection.CollectionChanged += BoxPartsParameterVMCollection_CollectionChanged;
                foreach (var item in collection)
                {
                    item.PropertyChanged -= Item_PropertyChanged;

                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
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
                                if (part is INotifyPropertyChanged item)
                                {
                                    item.PropertyChanged += Item_PropertyChanged;
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
                                    item.PropertyChanged += Item_PropertyChanged;
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
                                oldItem.PropertyChanged -= Item_PropertyChanged;
                                newItem.PropertyChanged += Item_PropertyChanged;

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

            UpdateSeparateBoxValue();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
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
                //已排定
                this.BoxPieceValue = BoxPartsParameterVMCollection.Count;
                //加工完成但尚未被移除的
                this.UnTransportedFinishedBoxPieceValue = BoxPartsParameterVMCollection.Count(x => x.IsFinish && !x.IsTransported);
                //已經被分配加工且尚未被移除
                this.UnTransportedBoxPieceValue = BoxPartsParameterVMCollection.Count(x => x.WorkIndex >= 0 && !x.IsTransported);
                //只有已完成
                this.FinishedBoxPieceValue = BoxPartsParameterVMCollection.Count(x => x.IsFinish);
                this.UnFinishedBoxPieceValue = BoxPartsParameterVMCollection.Count(x => !x.IsFinish);
            }
            catch
            {

            }
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


        private double _boxPieceValue;
        /// <summary>
        /// 箱子內分配到加工的值
        /// </summary>
        [JsonIgnore]
        public double BoxPieceValue
        {
            get => _boxPieceValue;
            set
            {
                _boxPieceValue = value;
                OnPropertyChanged();
            }
        }

        private double _unTransportedFinishedBoxPieceValue;
        /// <summary>
        /// 箱子內已加工但尚未被移除的鐵牌
        /// </summary>
        [JsonIgnore]
        public double UnTransportedFinishedBoxPieceValue
        {
            get => _unTransportedFinishedBoxPieceValue;
            set
            {
                _unTransportedFinishedBoxPieceValue = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BoxIsFull));
            }
        }

        private double _unTransportedBoxPieceValue;
        /// <summary>
        /// 箱子內已被分配的鐵片
        /// </summary>
        [JsonIgnore]
        public double UnTransportedBoxPieceValue
        {
            get => _unTransportedBoxPieceValue;
            set
            {
                _unTransportedBoxPieceValue = value;
                OnPropertyChanged();
            }
        }



        private double _finishedBoxPieceValue;
        /// <summary>
        /// 箱子內已加工的鐵牌
        /// </summary>
        [JsonIgnore]
        public double FinishedBoxPieceValue
        {
            get => _finishedBoxPieceValue;
            set
            {
                _finishedBoxPieceValue = value;
                OnPropertyChanged();
            }
        }


        private double _unFinishedBoxPieceValue;
        /// <summary>
        /// 箱子內未加工的鐵牌
        /// </summary>
        [JsonIgnore]
        public double UnFinishedBoxPieceValue
        {
            get => _unFinishedBoxPieceValue;
            set
            {
                _unFinishedBoxPieceValue = value;
                OnPropertyChanged();
            }
        }



    }

    /// <summary>
    /// 箱子集合體
    /// </summary>
    public class SeparateBoxExtViewModelObservableCollection : ObservableCollection<SeparateBoxExtViewModel>
    {
        public SeparateBoxExtViewModelObservableCollection():base()
        {
            m_scheduledPartsParameterCollection = new();
            m_scheduledPartsParameterCollectionReadOnly = new ReadOnlyObservableCollection<PartsParameterViewModel>(m_scheduledPartsParameterCollection);
            
            m_unscheduledPartsParameterCollection = new();
            m_unscheduledPartsParameterCollectionReadOnly = new ReadOnlyObservableCollection<PartsParameterViewModel>(m_unscheduledPartsParameterCollection);
        }
        public SeparateBoxExtViewModelObservableCollection(IEnumerable<SeparateBoxViewModel> collection) : base()
        {
            m_scheduledPartsParameterCollection = new();
            m_scheduledPartsParameterCollectionReadOnly = new ReadOnlyObservableCollection<PartsParameterViewModel>(m_scheduledPartsParameterCollection);

            m_unscheduledPartsParameterCollection = new();
            m_unscheduledPartsParameterCollectionReadOnly = new ReadOnlyObservableCollection<PartsParameterViewModel>(m_unscheduledPartsParameterCollection);



            if (collection == null)
            {
                return;
            }

            foreach (var item in collection)
            {
                var NewSeparateBoxExtViewModel = new SeparateBoxExtViewModel(item);
                NewSeparateBoxExtViewModel.BoxPartsParameterVMCollection.CollectionChanged += BoxPartsParameterVMCollection_CollectionChanged;

                foreach(var part in NewSeparateBoxExtViewModel.BoxPartsParameterVMCollection)
                {
                    part.PropertyChanged += NewPartsParameter_PropertyChanged;
                }
                base.Items.Add(NewSeparateBoxExtViewModel);
            }
        }

       /* private void Part_StateChanged(object? sender, EventArgs e)
        {
            InternalPartsParameterStateChanged?.Invoke(this, sender, e);
        }*/

        /// <summary>
        /// 內部的PartsParameter屬性被變更
        /// </summary>
        public event EventHandler? InternalPartsParameterStateChanged;



        public SeparateBoxExtViewModelObservableCollection(IEnumerable<SeparateBoxExtViewModel> collection) : base(collection)
        {
            m_scheduledPartsParameterCollection = new();
            m_scheduledPartsParameterCollectionReadOnly = new ReadOnlyObservableCollection<PartsParameterViewModel>(m_scheduledPartsParameterCollection);

            m_unscheduledPartsParameterCollection = new();
            m_unscheduledPartsParameterCollectionReadOnly = new ReadOnlyObservableCollection<PartsParameterViewModel>(m_unscheduledPartsParameterCollection);



            foreach (var item in collection)
            {
                item.BoxPartsParameterVMCollection.CollectionChanged += BoxPartsParameterVMCollection_CollectionChanged;

                foreach (var part in item.BoxPartsParameterVMCollection)
                {
                    part.PropertyChanged += NewPartsParameter_PropertyChanged; 
                }
            }
        }

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
                                foreach (var part in separateBox.BoxPartsParameterVMCollection)
                                {
                                    this.AddPartsParameter(part);
                                    part.PropertyChanged += NewPartsParameter_PropertyChanged;
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
                                        part.PropertyChanged -= NewPartsParameter_PropertyChanged;
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
                                    Opart.PropertyChanged -= NewPartsParameter_PropertyChanged;
                                    this.RemovePartsParameter(Opart);
                                }

                                foreach (var Npart in newItem.BoxPartsParameterVMCollection)
                                {
                                    Npart.PropertyChanged -= NewPartsParameter_PropertyChanged;
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
            partsParameter.PropertyChanged += NewPartsParameter_PropertyChanged;

            if (partsParameter.IsScheduled)
                m_scheduledPartsParameterCollection.Add(partsParameter);
            else
                m_unscheduledPartsParameterCollection.Add(partsParameter);

        }
        private void RemovePartsParameter(PartsParameterViewModel partsParameter)
        {
            partsParameter.PropertyChanged -= NewPartsParameter_PropertyChanged;

            if (partsParameter.IsScheduled)
                m_scheduledPartsParameterCollection.Remove(partsParameter);
            else
                m_unscheduledPartsParameterCollection.Remove(partsParameter);
        }


        private void ReplacePartsParameter(PartsParameterViewModel oldPartsParameter, PartsParameterViewModel newPartsParameter)
        {
            var scheduledIndex = ScheduledPartsParameterCollection.IndexOf(oldPartsParameter);
            var unscheduledIndex = UnscheduledPartsParameterCollection.IndexOf(oldPartsParameter);

            newPartsParameter.PropertyChanged += NewPartsParameter_PropertyChanged;

            oldPartsParameter.PropertyChanged -= NewPartsParameter_PropertyChanged;

            if (oldPartsParameter.IsScheduled == newPartsParameter.IsScheduled)
            {
                if (oldPartsParameter.IsScheduled)
                {
                    if(scheduledIndex!=-1)
                        m_scheduledPartsParameterCollection[scheduledIndex] = newPartsParameter;
                }
                else
                {
                    if (unscheduledIndex != -1)
                        m_unscheduledPartsParameterCollection[unscheduledIndex] = newPartsParameter;
                }
            }
            else
            {
                RemovePartsParameter(oldPartsParameter);
                AddPartsParameter(newPartsParameter);
            }
        }

        private void NewPartsParameter_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(sender is PartsParameterViewModel Part)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Part.IsScheduled)
                    {
                        if (UnscheduledPartsParameterCollection.Contains(Part))
                            m_unscheduledPartsParameterCollection.Remove(Part);

                        if (!ScheduledPartsParameterCollection.Contains(Part))
                            m_scheduledPartsParameterCollection.Add(Part);
                    }
                    else
                    {
                        if (ScheduledPartsParameterCollection.Contains(Part))
                            m_scheduledPartsParameterCollection.Remove(Part);

                        if (!UnscheduledPartsParameterCollection.Contains(Part))
                            m_unscheduledPartsParameterCollection.Add(Part);
                    }
                });
            }


        }

        /// <summary>
        /// 未排加工的鐵牌
        /// </summary>
        public ReadOnlyObservableCollection<PartsParameterViewModel> UnscheduledPartsParameterCollection
        {
            get => m_unscheduledPartsParameterCollectionReadOnly;
        }
        readonly PartsParameterViewModelObservableCollection m_unscheduledPartsParameterCollection;
        readonly ReadOnlyObservableCollection<PartsParameterViewModel> m_unscheduledPartsParameterCollectionReadOnly;


        /// <summary>
        /// 已排加工的鐵牌
        /// </summary>
        public ReadOnlyObservableCollection<PartsParameterViewModel> ScheduledPartsParameterCollection
        {
            get => m_scheduledPartsParameterCollectionReadOnly;
        }
        readonly PartsParameterViewModelObservableCollection m_scheduledPartsParameterCollection;
        readonly ReadOnlyObservableCollection<PartsParameterViewModel> m_scheduledPartsParameterCollectionReadOnly;



    }







}
