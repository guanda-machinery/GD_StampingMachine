﻿using GD_StampingMachine.GD_Model;
using System.Text.Json.Serialization;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class SeparateBoxViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SeparateBoxViewModel");

        public SeparateBoxViewModel(SeparateBoxModel SeparateBox)
        {
            _separateBox = SeparateBox;
        }

        [JsonIgnore]
        public readonly SeparateBoxModel _separateBox;

        public SeparateBoxViewModel()
        {
            _separateBox = new SeparateBoxModel();
        }
        public int BoxIndex
        {
            get => _separateBox.BoxIndex;
            set
            {
                _separateBox.BoxIndex = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool BoxIsFull => UntransportedFinishedBoxPieceValue >= BoxSliderValue;


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

        private double _untransportedFinishedBoxPieceValue;
        /// <summary>
        /// 箱子內已加工但尚未被移除的鐵牌
        /// </summary>
        [JsonIgnore]
        public double UntransportedFinishedBoxPieceValue
        {
            get => _untransportedFinishedBoxPieceValue;
            set
            {
                _untransportedFinishedBoxPieceValue = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BoxIsFull));
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
            get => _separateBox.BoxSliderValue;
            set
            {
                _separateBox.BoxSliderValue = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BoxIsFull));
            }
        }

        /// <summary>
        /// 盒子可用/不可用
        /// </summary>
        public bool BoxIsEnabled
        {
            get => _separateBox.BoxIsEnabled;
            set
            {
                _separateBox.BoxIsEnabled = value;
                OnPropertyChanged();
            }
        }

        /*private bool _boxSliderIsEnabled = true;
        /// <summary>
        /// SliderBar啟用/關閉
        /// </summary>
        public bool BoxSliderIsEnabled
        {
            get => _boxSliderIsEnabled;
            set
            {
                _boxSliderIsEnabled = value;
                OnPropertyChanged();
            }
        }*/


        public bool IsUsing
        {
            get => _separateBox.BoxIsUsing;
            set
            {
                _separateBox.BoxIsUsing = value;
                OnPropertyChanged();
            }
        }


    }





}
