using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class SettingViewModelBase: ViewModelBase
    {
        private int? _sequenceCountComboBoxSelectValue = null;
        /// <summary>
        /// 單排數量
        /// </summary>
        public int? SequenceCountComboBoxSelectValue
        {
            get
            {
                return _sequenceCountComboBoxSelectValue;
            }
            set
            {
                _sequenceCountComboBoxSelectValue = value;
                OnPropertyChanged(nameof(SequenceCountComboBoxSelectValue));
                OnPropertyChanged(nameof(PlateNumberList));
            }
        }
        public virtual int PlateNumberListMax { get; set; } = 8;
        /// <summary>
        /// 這是鐵牌上要打的位置
        /// </summary>
        public ObservableCollection<int> PlateNumberList
        {
            get
            {
                int ColumnCount = PlateNumberListMax;
                if (SequenceCountComboBoxSelectValue.HasValue)
                {
                    if (SequenceCountComboBoxSelectValue.Value <= PlateNumberListMax)
                        ColumnCount = SequenceCountComboBoxSelectValue.Value;
                }

                int RowCount = 2;
                _ = SpecialSequenceComboBoxSelectValue switch
                {
                    SpecialSequenceEnum.OneRow => RowCount = 1,
                    SpecialSequenceEnum.TwoRow => RowCount = 2,
                    _ => RowCount = 2,
                };

                var NumberList = new ObservableCollection<int>();

                var ListCount = ColumnCount * RowCount;
                for (int i = 0; i < ListCount; i++)
                {
                    NumberList.Add(i + 1);
                }
                return NumberList;
            }
        }
        private HorizontalAlignEnum? _horizontalAlignEnumComboBoxSelectValue;
        private VerticalAlignEnum? _verticalAlignEnumComboBoxSelectValue;
        public HorizontalAlignEnum? HorizontalAlignEnumComboBoxSelectValue
        {
            get => _horizontalAlignEnumComboBoxSelectValue;
            set
            {
                _horizontalAlignEnumComboBoxSelectValue = value;
                OnPropertyChanged(nameof(HorizontalAlignEnumComboBoxSelectValue));
            }
        }
        public VerticalAlignEnum? VerticalAlignEnumComboBoxSelectValue
        {
            get => _verticalAlignEnumComboBoxSelectValue;
            set
            {
                _verticalAlignEnumComboBoxSelectValue = value;
                OnPropertyChanged(nameof(VerticalAlignEnumComboBoxSelectValue));
            }
        }
        public Array HorizontalAlignmentCollection
        {
            get
            {
                return System.Enum.GetValues(typeof(HorizontalAlignEnum));
            }
        }
        public Array VerticalAlignmentCollection
        {
            get
            {
                return System.Enum.GetValues(typeof(VerticalAlignEnum));
            }
        }

        private SpecialSequenceEnum? _specialSequenceComboBoxSelectValue = null;
        /// <summary>
        /// 特殊排序
        /// </summary>
        public SpecialSequenceEnum? SpecialSequenceComboBoxSelectValue
        {
            get
            {
                return _specialSequenceComboBoxSelectValue;
            }
            set
            {
                _specialSequenceComboBoxSelectValue = value;
                OnPropertyChanged(nameof(SpecialSequenceComboBoxSelectValue));
                OnPropertyChanged(nameof(PlateNumberList));
            }
        }

        public ObservableCollection<int> SequenceCountCollection
        {
            get
            {
                var CountCollection = new ObservableCollection<int>();
                for (int i = 1; i <= PlateNumberListMax; i++)
                {
                    CountCollection.Add(i);
                }
                return CountCollection;
            }
        }

        public ObservableCollection<SpecialSequenceEnum> SpecialSequenceCollection
        {
            get
            {
                var EnumList = new ObservableCollection<SpecialSequenceEnum>();
                foreach (SpecialSequenceEnum EachEnum in System.Enum.GetValues(typeof(SpecialSequenceEnum)))
                {
                    EnumList.Add(EachEnum);
                }

                return EnumList;
            }
        }


    }
}
