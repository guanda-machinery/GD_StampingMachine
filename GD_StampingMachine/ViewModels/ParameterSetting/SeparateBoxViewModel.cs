using GD_StampingMachine.GD_Model;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        public virtual SeparateBoxModel SeparateBox { get; } =new();

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

    public class SeparateBoxExtViewModel : SeparateBoxViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SeparateBoxViewModel");

        static SeparateBoxExtViewModel()
        {
            BoxSliderValueChanged += SeparateBoxExtViewModel_BoxSliderValueChanged;

        }

        private static void SeparateBoxExtViewModel_BoxSliderValueChanged(object? sender, double e)
        {
            if(sender is SeparateBoxExtViewModel separateBoxExt)
            {
                separateBoxExt.CheckBoxIsFull();
            }
        }



        public SeparateBoxExtViewModel(SeparateBoxModel separateBox) : base(separateBox)
        {
        }



        public SeparateBoxExtViewModel() : base()
        {

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







}
