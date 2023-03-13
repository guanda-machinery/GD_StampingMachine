using DevExpress.Utils.Extensions;
using GD_StampingMachine.Enum;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;




namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class NumberSettingViewModel : ViewModelBase
    {
        /// <summary>
        /// 預覽圖VM
        /// </summary>
        //public NumberSettingSchematicDiagramViewModel NumberSettingSchematicDiagramVM = new NumberSettingSchematicDiagramViewModel();


        private NumberSettingModel _NumberSettingModel = new NumberSettingModel();

        public NumberSettingModel NumberSetting
        {
            get
            {
                return _NumberSettingModel;
            }
            set
            {
                _NumberSettingModel = value;
                OnPropertyChanged(nameof(NumberSetting));
            }

        }

        public ObservableCollection<NumberSettingModeEnum> NumberSettingModeCollection
        {
            get
            {
                var EnumList = new ObservableCollection<NumberSettingModeEnum>();
                foreach (NumberSettingModeEnum EachEnum in System.Enum.GetValues(typeof(NumberSettingModeEnum)))
                {
                    EnumList.Add(EachEnum);
                }
                return EnumList;
            }
        }

        public ObservableCollection<int> SequenceCountCollection
        {
            get
            {
                var CountCollection = new ObservableCollection<int>();
                for (int i = 1; i <= 8; i++)
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


        private int? _sequenceCountComboBoxSelectValue = null;
        /// <summary>
        /// 單排數量
        /// </summary>
        public int? SequenceCountComboBoxSelectValue
        {
            get
            {
                if (_sequenceCountComboBoxSelectValue.HasValue)
                {
                    //NumberSettingSchematicDiagramVM.SequenceCount = _sequenceCountComboBoxSelectValue.Value;
                    NumberSetting.SequenceCount = _sequenceCountComboBoxSelectValue.Value;
                }
                return _sequenceCountComboBoxSelectValue;
            }
            set
            {
                _sequenceCountComboBoxSelectValue = value;
                OnPropertyChanged(nameof(SequenceCountComboBoxSelectValue));
                OnPropertyChanged(nameof(PlateNumberList));
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
                if (_specialSequenceComboBoxSelectValue.HasValue)
                {
                    NumberSetting.SpecialSequence = _specialSequenceComboBoxSelectValue.Value;
                }
                return _specialSequenceComboBoxSelectValue;

            }
            set
            {
                _specialSequenceComboBoxSelectValue = value;
                OnPropertyChanged(nameof(SpecialSequenceComboBoxSelectValue));
                OnPropertyChanged(nameof(PlateNumberList));
            }
        }

        /// <summary>
        /// 這是鐵牌上要打的位置
        /// </summary>
        public ObservableCollection<int> PlateNumberList
        {
            get
            {
                int ColumnCount = 8;
                if (SequenceCountComboBoxSelectValue.HasValue)
                {
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


        public HorizontalAlignment _horizontalAlignmentComboBoxSelectValue= HorizontalAlignment.Left;
        public VerticalAlignment _verticalAlignmentComboBoxSelectValue = VerticalAlignment.Top;
        public HorizontalAlignment HorizontalAlignmentComboBoxSelectValue
        {
            get => _horizontalAlignmentComboBoxSelectValue;
            set
            {
                _horizontalAlignmentComboBoxSelectValue = value; OnPropertyChanged(nameof(HorizontalAlignmentComboBoxSelectValue));
            }
        }
        public VerticalAlignment VerticalAlignmentComboBoxSelectValue
        {
            get => _verticalAlignmentComboBoxSelectValue;
            set { _verticalAlignmentComboBoxSelectValue = value;OnPropertyChanged(nameof(VerticalAlignmentComboBoxSelectValue)); }
        }


        public ObservableCollection<HorizontalAlignment> HorizontalAlignmentCollection
        {
            get
            {
                var EnumList = new ObservableCollection<HorizontalAlignment>();
                foreach (HorizontalAlignment EachEnum in System.Enum.GetValues(typeof(HorizontalAlignment)))
                {
                    EnumList.Add(EachEnum);
                }
                EnumList.Remove(HorizontalAlignment.Stretch);

                return EnumList;
            }
        }



        public ObservableCollection<VerticalAlignment> VerticalAlignmentCollection
        {
            get
            {
                var EnumList = new ObservableCollection<VerticalAlignment>();
                foreach (VerticalAlignment EachEnum in System.Enum.GetValues(typeof(VerticalAlignment)))
                {
                    EnumList.Add(EachEnum);
                }
                EnumList.Remove(VerticalAlignment.Stretch);

                return EnumList;
            }
        }




        public ICommand LoadModeCommand
        {
            get => new RelayCommand(() =>
            {

            });
        }
        public ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                NumberSetting = new NumberSettingModel();
            });
        }

        public ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {

            });
        }
    }










}
