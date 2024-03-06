using GD_StampingMachine.GD_Model;
using GD_StampingMachine.ViewModels.ProductSetting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class SeparateBoxViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SeparateBoxViewModel");

        public SeparateBoxViewModel(SeparateBoxModel separateBox)
        {
            SeparateBox = separateBox;
        }

        [JsonIgnore]
        public readonly SeparateBoxModel SeparateBox;

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
                OnPropertyChanged(nameof(BoxIsFull));
            }
        }

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
        public class SeparateBoxViewModelObservableCollection : ObservableCollection<SeparateBoxViewModel>
    {
        //protected new List<PartsParameterViewModel> Items => (List<PartsParameterViewModel>)base.Items;

        public SeparateBoxViewModelObservableCollection()
        {
            //this.CollectionChanged += PartsParameterViewModelObservableCollection_CollectionChanged; ;
        }
        /*
        public SeparateBoxViewModelObservableCollection(List<SeparateBoxViewModel> list) : base(list)
        {
            //this.CollectionChanged += PartsParameterViewModelObservableCollection_CollectionChanged; ;
            foreach (var item in list)
            {
                item.FinishProgressChanged += item_FinishProgressChanged;
                item.IsFinishChanged += item_IsFinishChanged;
                item.DistributeNameChanged += Item_DistributeNameChanged;
            }
        }
        public SeparateBoxViewModelObservableCollection(IEnumerable<SeparateBoxViewModel> collection)
        {
            //this.CollectionChanged += PartsParameterViewModelObservableCollection_CollectionChanged; ;

            IList<PartsParameterViewModel> items = Items;
            if (collection == null || items == null)
            {
                return;
            }
            foreach (var item in collection)
            {
                item.FinishProgressChanged += item_FinishProgressChanged;
                item.IsFinishChanged += item_IsFinishChanged;
                item.DistributeNameChanged += Item_DistributeNameChanged;
                items.Add(item);
            }
        }
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    if (newItem is PartsParameterViewModel item)
                    {
                        item.FinishProgressChanged += item_FinishProgressChanged;
                        item.IsFinishChanged += item_IsFinishChanged;
                        item.DistributeNameChanged += Item_DistributeNameChanged;
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (var oldItems in e.OldItems)
                {
                    if (oldItems is PartsParameterViewModel item)
                    {
                        item.FinishProgressChanged -= item_FinishProgressChanged;
                        item.IsFinishChanged -= item_IsFinishChanged;
                        item.DistributeNameChanged -= Item_DistributeNameChanged;
                    }
                }
            }

            CalcFinishProgress();
            CalcUnFinishedCount();
            CalcNotAssignedProductProjectCount();

            base.OnCollectionChanged(e);
        }

        protected override void InsertItem(int index, PartsParameterViewModel item)
        {
            item.FinishProgressChanged += item_FinishProgressChanged;
            item.IsFinishChanged += item_IsFinishChanged;
            item.DistributeNameChanged += Item_DistributeNameChanged;
            CalcFinishProgress();
            CalcUnFinishedCount();
            CalcNotAssignedProductProjectCount();
            base.InsertItem(index, item);
        }

        private void item_FinishProgressChanged(object? sender, float e)
        {
            CalcFinishProgress();
        }
        private void item_IsFinishChanged(object? sender, bool e)
        {
            CalcUnFinishedCount();
        }

        private void Item_DistributeNameChanged(object? sender, string e)
        {
            CalcNotAssignedProductProjectCount();
            //throw new NotImplementedException();
        }

        private void CalcFinishProgress()
        {
            this.FinishProgress = this.Any() ? this.Average(p => p.FinishProgress) : 0;
        }
        private void CalcUnFinishedCount()
        {
            UnFinishedCount = this.Any() ? this.Count(p => !p.IsFinish) : 0;
        }

        private void CalcNotAssignedProductProjectCount()
        {
            NotAssignedProductProjectCount = this.Any() ? this.Count(p => string.IsNullOrEmpty(p.DistributeName)) : 0;
        }





        public event EventHandler<float>? FinishProgressChanged;
        public event EventHandler<int>? UnFinishedCountChanged;
        public event EventHandler<int>? NotAssignedProductProjectCountChanged;

        private float _finishProgress;
        /// <summary>
        /// 進度條(平均值)
        /// </summary>
        public float FinishProgress
        {
            get => _finishProgress;
            private set
            {
                _finishProgress = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(FinishProgress)));
                FinishProgressChanged?.Invoke(this, value);
            }
        }

        private int _unFinishedCount;
        /// <summary>
        /// 未完成的總和
        /// </summary>
        public int UnFinishedCount
        {
            get => _unFinishedCount;
            private set
            {
                _unFinishedCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(UnFinishedCount)));
                UnFinishedCountChanged?.Invoke(this, value);
            }
        }


        private int _notAssignedProductProjectCount;
        /// <summary>
        /// 未排版的資料
        /// </summary>
        public int NotAssignedProductProjectCount
        {
            get => _notAssignedProductProjectCount;
            private set
            {
                _notAssignedProductProjectCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(NotAssignedProductProjectCount)));
                NotAssignedProductProjectCountChanged?.Invoke(this, value);
            }
        }*/
    }


}
