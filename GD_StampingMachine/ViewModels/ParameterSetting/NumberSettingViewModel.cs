using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf.Native;
using DevExpress.Utils.Extensions;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Interfaces;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using GD_StampingMachine.Model.ParameterSetting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class NumberSettingViewModel : ParameterSettingBaseViewModel, IStampingPlateVM
    {
        [JsonIgnore]
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SettingViewModelNormalViewModel");

        public NumberSettingViewModel(StampingPlateSettingModel _NumberSetting) 
        {        
            NumberSetting = _NumberSetting;

            NumberSettingModelCollectionSelected = NumberSettingModelSavedCollection.FirstOrDefault();

        }
        private StampingPlateSettingModel _numberSetting;
        public virtual StampingPlateSettingModel NumberSetting
        {
            get
            {
                if (_numberSetting == null)
                    _numberSetting = new StampingPlateSettingModel();
                if (_numberSetting != null)
                {
                    _numberSetting.NumberSettingMode = NumberSettingMode;
                    if (SequenceCountComboBoxSelectValue.HasValue)
                        _numberSetting.SequenceCount = SequenceCountComboBoxSelectValue.Value;
                    if (SpecialSequenceComboBoxSelectValue.HasValue)
                        _numberSetting.SpecialSequence = SpecialSequenceComboBoxSelectValue.Value;
                    if (VerticalAlignEnumComboBoxSelectValue.HasValue)
                        _numberSetting.VerticalAlign = VerticalAlignEnumComboBoxSelectValue.Value;
                    if (HorizontalAlignEnumComboBoxSelectValue.HasValue)
                        _numberSetting.HorizontalAlign = HorizontalAlignEnumComboBoxSelectValue.Value;

                    _numberSetting.IronPlateMargin = new PlateMarginStruct
                    {
                        A_Margin = this.Margin_A,
                        B_Margin = this.Margin_B,
                        C_Margin = this.Margin_C,
                        D_Margin = this.Margin_D,
                        E_Margin = this.Margin_E,
                    };
                }
                return _numberSetting;
            }
            set
            {
                _numberSetting = value;
                if (_numberSetting == new StampingPlateSettingModel())
                {
                    SequenceCountComboBoxSelectValue = null;
                    SpecialSequenceComboBoxSelectValue = null;
                    VerticalAlignEnumComboBoxSelectValue = null;
                    HorizontalAlignEnumComboBoxSelectValue = null;
                }
                else if (_numberSetting != null)
                {
                    NumberSettingMode = _numberSetting.NumberSettingMode;
                    SequenceCountComboBoxSelectValue = _numberSetting.SequenceCount;
                    SpecialSequenceComboBoxSelectValue = _numberSetting.SpecialSequence;
                    VerticalAlignEnumComboBoxSelectValue = _numberSetting.VerticalAlign;
                    HorizontalAlignEnumComboBoxSelectValue = _numberSetting.HorizontalAlign;

                    this.Margin_A = _numberSetting.IronPlateMargin.A_Margin;
                    this.Margin_B = _numberSetting.IronPlateMargin.B_Margin;
                    this.Margin_C = _numberSetting.IronPlateMargin.C_Margin;
                    this.Margin_D = _numberSetting.IronPlateMargin.D_Margin;
                    this.Margin_E = _numberSetting.IronPlateMargin.E_Margin;
                }
                OnPropertyChanged();
            }
        }

        private string _numberSettingMode;
        public virtual string NumberSettingMode
        {
            get => _numberSettingMode;
            set { _numberSettingMode = value; OnPropertyChanged(); }
        }


        private StampingPlateSettingModel _numberSettingModelCollectionSelected;
        [JsonIgnore]
        public StampingPlateSettingModel NumberSettingModelCollectionSelected
        {
            get => _numberSettingModelCollectionSelected;
            set
            {
                _numberSettingModelCollectionSelected = value;
                if (_numberSettingModelCollectionSelected != null)
                    NumberSetting = _numberSettingModelCollectionSelected.DeepCloneByJson();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<StampingPlateSettingModel> _numberSettingModelSavedCollection;


        public ObservableCollection<StampingPlateSettingModel> NumberSettingModelSavedCollection
        {
            get
            {
                if (_numberSettingModelSavedCollection == null)
                {
                    if (JsonHM.ReadParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.NumberSetting, out ObservableCollection<StampingPlateSettingModel> SavedCollection))
                        _numberSettingModelSavedCollection = SavedCollection;
                    else
                        _numberSettingModelSavedCollection = new();
                }
                return _numberSettingModelSavedCollection;
            }
            set
            {
                _numberSettingModelSavedCollection = value;
                OnPropertyChanged(nameof(NumberSettingModelSavedCollection));
            }
        }



        public override ICommand LoadSettingCommand 
        {
            get => new RelayCommand(() =>
            {
                if(NumberSettingModelCollectionSelected != null)
                    NumberSetting = NumberSettingModelCollectionSelected.DeepCloneByJson();
            });
        }
        public override ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                NumberSetting = new StampingPlateSettingModel();
            });
        }
        public override ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {
                var FIndex = NumberSettingModelSavedCollection.FindIndex(x => x.NumberSettingMode == NumberSettingMode);
                if (FIndex != -1)
                {
                    if (Method.MethodWinUIMessageBox.AskOverwriteOrNot())
                    {
                        NumberSettingModelSavedCollection[FIndex] = NumberSetting.DeepCloneByJson();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    NumberSettingModelSavedCollection.Add(NumberSetting.DeepCloneByJson());
                }

                JsonHM.WriteParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.NumberSetting, NumberSettingModelSavedCollection ,true);
            });
        }
        public override ICommand DeleteSettingCommand
        {
            get => new RelayCommand(() =>
            {
                if (NumberSettingModelCollectionSelected != null)
                {
                    NumberSettingModelSavedCollection.Remove(NumberSettingModelCollectionSelected);
                    JsonHM.WriteParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.NumberSetting, NumberSettingModelSavedCollection);
                }
            });
         }





        private int _sequenceCount = 0;
        private SpecialSequenceEnum _specialSequence;
        private HorizontalAlignEnum _horizontalAlign;
        private VerticalAlignEnum _verticalAlign;
        private PlateMarginStruct _ironPlateMargin = new PlateMarginStruct();

        public int SequenceCount { get => _sequenceCount; set { _sequenceCount = value; OnPropertyChanged(); } }
        public SpecialSequenceEnum SpecialSequence { get => _specialSequence; set { _specialSequence = value; OnPropertyChanged(); } }
        public HorizontalAlignEnum HorizontalAlign { get => _horizontalAlign; set { _horizontalAlign = value; OnPropertyChanged(); } }
        public VerticalAlignEnum VerticalAlign { get => _verticalAlign; set { _verticalAlign = value; OnPropertyChanged(); } }
        //public virtual PlateMarginStruct IronPlateMargin { get => NumberSetting.IronPlateMargin; set { NumberSetting.IronPlateMargin = value;  OnPropertyChanged(); } }

        private double _margin_A;
        private double _margin_B;
        private double _margin_C;
        private double _margin_D;
        private double _margin_E;


        public double Margin_A { get => _margin_A; set { _margin_A = value; OnPropertyChanged(); } }
        public double Margin_B { get => _margin_B; set { _margin_B = value; OnPropertyChanged(); } }
        public double Margin_C { get => _margin_C; set { _margin_C = value; OnPropertyChanged(); } }
        public double Margin_D { get => _margin_D; set { _margin_D = value; OnPropertyChanged(); } }
        public double Margin_E { get => _margin_E; set { _margin_E = value; OnPropertyChanged(); } }



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
        public virtual int PlateNumberListMax { get; } = 8;
        /// <summary>
        /// 這是鐵牌上要打的位置
        /// </summary>

        [JsonIgnore]
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

        [JsonIgnore]
        public HorizontalAlignEnum? HorizontalAlignEnumComboBoxSelectValue
        {
            get => _horizontalAlignEnumComboBoxSelectValue;
            set
            {
                _horizontalAlignEnumComboBoxSelectValue = value;
                OnPropertyChanged(nameof(HorizontalAlignEnumComboBoxSelectValue));
            }
        }
        [JsonIgnore]
        public VerticalAlignEnum? VerticalAlignEnumComboBoxSelectValue
        {
            get => _verticalAlignEnumComboBoxSelectValue;
            set
            {
                _verticalAlignEnumComboBoxSelectValue = value;
                OnPropertyChanged(nameof(VerticalAlignEnumComboBoxSelectValue));
            }
        }
        [JsonIgnore]
        public Array HorizontalAlignmentCollection
        {
            get
            {
                return System.Enum.GetValues(typeof(HorizontalAlignEnum));
            }
        }
        [JsonIgnore]
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

        [JsonIgnore]
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

        [JsonIgnore]
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

        [JsonIgnore]
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
