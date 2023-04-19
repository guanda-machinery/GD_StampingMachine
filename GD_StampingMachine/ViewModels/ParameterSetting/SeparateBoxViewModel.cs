using GD_StampingMachine.Model;
using GD_StampingMachine.Model.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class SeparateBoxViewModel : BaseViewModelWithLog
    {
        private SeparateBoxModel SeparateBox;
        public SeparateBoxViewModel()
        {
            SeparateBox = new SeparateBoxModel();
        }
        public int BoxNumber
        {
            get => SeparateBox.BoxNumber;
            set
            {
                SeparateBox.BoxNumber = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 箱子內容物的值
        /// </summary>
        public double BoxPieceValue { get; set; }

        public double BoxSliderValue
        {
            get => SeparateBox.BoxSliderValue;
            set
            {
                SeparateBox.BoxSliderValue = value;
                OnPropertyChanged();
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

        private bool _boxSliderIsEnabled = true;
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
        }



    }
}
