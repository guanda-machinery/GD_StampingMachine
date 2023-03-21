using DevExpress.Mvvm.Native;
using DevExpress.Utils.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        private string NumberSettingFilePath
        {
            get => Path.Combine(Directory.GetCurrentDirectory(), "NumberSetting", "Normal.csv");
        }


        private NumberSettingModel _numberSetting;
        public NumberSettingModel NumberSetting
        {
            get
            {
                if(_numberSetting == null)
                    _numberSetting = new NumberSettingModel();
                return _numberSetting;
            }
            set
            {
                if(value == new NumberSettingModel())
                {
                    SequenceCountComboBoxSelectValue = null;
                    SpecialSequenceComboBoxSelectValue = null;
                    VerticalAlignEnumComboBoxSelectValue = null;
                    HorizontalAlignEnumComboBoxSelectValue = null;
                } 
                else if(value!= null)
                {
                    SequenceCountComboBoxSelectValue = NumberSetting.SequenceCount;
                    SpecialSequenceComboBoxSelectValue = NumberSetting.SpecialSequence;
                    VerticalAlignEnumComboBoxSelectValue = NumberSetting.VerticalAlign;
                    HorizontalAlignEnumComboBoxSelectValue = NumberSetting.HorizontalAlign;
                }
                _numberSetting = value;
                OnPropertyChanged(nameof(NumberSetting));
            }
        }

        public NumberSettingModel NumberSettingModelSavedCollectionSelected { get; set; }
        public List<NumberSettingModel> NumberSettingModelSavedCollection
        {
            get
            {
                var CsvHM = new CsvHelperMethod();
                CsvHM.ReadCSVFileIEnumerable(NumberSettingFilePath , out List<NumberSettingModel> NumberSettingList );
                return NumberSettingList;
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


        public virtual int PlateNumberListMax { get; set; } =8;
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
                    if(SequenceCountComboBoxSelectValue.Value <= PlateNumberListMax)               
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
                if (_horizontalAlignEnumComboBoxSelectValue.HasValue)
                    NumberSetting.HorizontalAlign = _horizontalAlignEnumComboBoxSelectValue.Value;
                OnPropertyChanged(nameof(HorizontalAlignEnumComboBoxSelectValue));
            }
        }
        public VerticalAlignEnum? VerticalAlignEnumComboBoxSelectValue
        {
            get => _verticalAlignEnumComboBoxSelectValue;
            set 
            {
                _verticalAlignEnumComboBoxSelectValue = value;
                if (_verticalAlignEnumComboBoxSelectValue.HasValue)
                    NumberSetting.VerticalAlign = _verticalAlignEnumComboBoxSelectValue.Value;
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





        public virtual ICommand LoadModeCommand
        {
            get => new RelayCommand(() =>
            {
                var CsvHM = new CsvHelperMethod();
                CsvHM.ReadCSVFile(NumberSettingFilePath, out NumberSettingModel LoadNumberSetting2);
                NumberSetting = LoadNumberSetting2;
            });
        }
        public virtual ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                NumberSetting = new NumberSettingModel();
            });
        }




        public virtual ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {
                var CsvHM = new CsvHelperMethod();

                CsvHM.WriteCSVFile(NumberSettingFilePath , NumberSettingModelSavedCollection);
                //CsvHM.WriteCSVFile<NumberSettingModel>(NumberSettingFilePath, NumberSetting);

                OnPropertyChanged(nameof(NumberSettingModelSavedCollection));
            });
        }


    }

}
