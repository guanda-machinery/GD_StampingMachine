using GD_StampingMachine.GD_Model;
using System.Text.Json.Serialization;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class SeparateBoxViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SeparateBoxViewModel");

        public SeparateBoxViewModel(SeparateBoxModel SeparateBox)
        {
            _separateBox= SeparateBox  ;
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

        private double _boxPieceValue;
        /// <summary>
        /// 箱子內容物的值
        /// </summary>
        [JsonIgnore]
        public double BoxPieceValue 
        {
            get => _boxPieceValue;
            set
            {
                _boxPieceValue = value;OnPropertyChanged();
            }
        }

        public double BoxSliderValue
        {
            get => _separateBox.BoxSliderValue;
            set
            {
                _separateBox.BoxSliderValue = value;
                OnPropertyChanged();
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
