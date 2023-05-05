using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GD_CommonLibrary;
using GD_StampingMachine.Interfaces;
using GD_StampingMachine.Model.ParameterSetting;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 繼承
    /// </summary>
    public class QRSettingViewModel : NumberSettingViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SettingViewModelQRViewModel");
      

        public QRSettingViewModel(QRStampingPlateSettingModel _QRSetting) : base(_QRSetting)
        {
            NumberSetting = _QRSetting;
        }


        // public new QRSettingModel NumberSetting { get; set; }

        private QRStampingPlateSettingModel _numberSetting;
        public new QRStampingPlateSettingModel NumberSetting
        {
            get 
            {
                if (_numberSetting == null)
                    _numberSetting = new QRStampingPlateSettingModel();
                else
                {
                    if (SequenceCountComboBoxSelectValue.HasValue)
                        _numberSetting.SequenceCount = SequenceCountComboBoxSelectValue.Value;
                    if (SpecialSequenceComboBoxSelectValue.HasValue)
                        _numberSetting.SpecialSequence = SpecialSequenceComboBoxSelectValue.Value;
                    if (VerticalAlignEnumComboBoxSelectValue.HasValue)
                        _numberSetting.VerticalAlign = VerticalAlignEnumComboBoxSelectValue.Value;
                    if (HorizontalAlignEnumComboBoxSelectValue.HasValue)
                        _numberSetting.HorizontalAlign = HorizontalAlignEnumComboBoxSelectValue.Value;

                    _numberSetting.IronPlateMargin = new QRIronPlateMarginStruct
                    {
                        A_Margin = this.Margin_A,
                        B_Margin = this.Margin_B,
                        C_Margin = this.Margin_C,
                        D_Margin = this.Margin_D,
                        E_Margin = this.Margin_E,
                        F_Margin = this.Margin_F,
                        G_Margin = this.Margin_G,
                        H_Margin = this.Margin_H,
                        I_Margin = this.Margin_I,
                    };
                }
                return _numberSetting;
            } 
            set 
            {
                _numberSetting = value;
                if (_numberSetting == new QRStampingPlateSettingModel ())
                {
                    SequenceCountComboBoxSelectValue = null;
                    SpecialSequenceComboBoxSelectValue = null;
                    VerticalAlignEnumComboBoxSelectValue = null;
                    HorizontalAlignEnumComboBoxSelectValue = null;
                }
                else if (_numberSetting != null)
                {
                    SequenceCountComboBoxSelectValue = _numberSetting.SequenceCount;
                    SpecialSequenceComboBoxSelectValue = _numberSetting.SpecialSequence;
                    VerticalAlignEnumComboBoxSelectValue = _numberSetting.VerticalAlign;
                    HorizontalAlignEnumComboBoxSelectValue = _numberSetting.HorizontalAlign;

                    this.Margin_A = _numberSetting.IronPlateMargin.A_Margin;
                    this.Margin_B = _numberSetting.IronPlateMargin.B_Margin;
                    this.Margin_C = _numberSetting.IronPlateMargin.C_Margin;
                    this.Margin_D = _numberSetting.IronPlateMargin.D_Margin;
                    this.Margin_E = _numberSetting.IronPlateMargin.E_Margin;
                    this.Margin_F = _numberSetting.IronPlateMargin.F_Margin;
                    this.Margin_G = _numberSetting.IronPlateMargin.G_Margin;
                    this.Margin_H = _numberSetting.IronPlateMargin.H_Margin;
                    this.Margin_I = _numberSetting.IronPlateMargin.I_Margin;
                }
                OnPropertyChanged();
            }

        }

        public override string NumberSettingMode
        {
            get => NumberSetting.NumberSettingMode;
            set { NumberSetting.NumberSettingMode = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 選擇
        /// </summary>
        private QRStampingPlateSettingModel _qrSettingModelCollectionSelected;
        public QRStampingPlateSettingModel QRSettingModelCollectionSelected
        {
            get => _qrSettingModelCollectionSelected;
            set
            {
                _qrSettingModelCollectionSelected = value;
                if (_qrSettingModelCollectionSelected != null)
                    NumberSetting = _qrSettingModelCollectionSelected.DeepCloneByJson();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<QRStampingPlateSettingModel> _qrSettingModelModelCollection;
        public ObservableCollection<QRStampingPlateSettingModel> QRSettingModelCollection
        {
            get
            {
                if (_qrSettingModelModelCollection == null)
                {
                    if (JsonHM.ReadParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.QRSetting, out ObservableCollection<QRStampingPlateSettingModel> SavedCollection,false))
                        _qrSettingModelModelCollection = SavedCollection;                
                    else
                        _qrSettingModelModelCollection = new();
                }
                return _qrSettingModelModelCollection;
            }
            set
            {
                _qrSettingModelModelCollection = value;
                OnPropertyChanged(nameof(QRSettingModelCollection));
            }
        }




        public ObservableCollection<int> CharactersCountCollection
        {
            get
            {
                var EnumList = new ObservableCollection<int>
                {
                    1,
                    2,
                    3,
                    4,
                    5
                };


                return EnumList;
            }
        }
        public ObservableCollection<CharactersFormEnum> CharactersFormEnumCollection
        {
            get
            {
                var EnumList = new ObservableCollection<CharactersFormEnum>();
                foreach (CharactersFormEnum EachEnum in System.Enum.GetValues(typeof(CharactersFormEnum)))
                {
                    EnumList.Add(EachEnum);
                }

                return EnumList;
            }
        }
        public ObservableCollection<string> ModelSizeCollection
        {
            get
            {
                var EnumList = new ObservableCollection<string>
                {
                    "11x11",
                    "11x22",
                    "22x22"
                };

                return EnumList;
            }
        }

        public override int PlateNumberListMax => 6;



        private double _margin_F;

        private double _margin_G;

        private double _margin_H;

        private double _margin_I;

        public double Margin_F { get => _margin_F; set { _margin_F = value; OnPropertyChanged(); } }
        public double Margin_G { get => _margin_G; set { _margin_G = value; OnPropertyChanged(); } }
        public double Margin_H { get => _margin_H; set { _margin_H = value; OnPropertyChanged(); } }
        public double Margin_I { get => _margin_I; set { _margin_I = value; OnPropertyChanged(); } }



        public override ICommand LoadSettingCommand 
        {
            get => new RelayCommand(() =>
            {
                if (QRSettingModelCollectionSelected != null)
                    NumberSetting = QRSettingModelCollectionSelected.DeepCloneByJson();
            });
        }
        public override ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                NumberSetting = new QRStampingPlateSettingModel();
            });
        }

        public override ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {
                var FIndex = QRSettingModelCollection.FindIndex(x => x.NumberSettingMode == NumberSetting.NumberSettingMode);
                if (FIndex != -1)
                {
                    if (Method.MethodWinUIMessageBox.AskOverwriteOrNot())
                    {
                        QRSettingModelCollection[FIndex] = NumberSetting.DeepCloneByJson();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    QRSettingModelCollection.Add(NumberSetting.DeepCloneByJson());
                }

                this.JsonHM.WriteParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.QRSetting, QRSettingModelCollection, true);
            });
        }

        public override ICommand DeleteSettingCommand
        {
            get => new RelayCommand(() =>
            {
                if (QRSettingModelCollectionSelected != null)
                {
                    QRSettingModelCollection.Remove(QRSettingModelCollectionSelected);
                    JsonHM.WriteParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.QRSetting, QRSettingModelCollection);
                }
            });
        }
       // public PlateMarginStruct IronPlateMargin { get => NumberSetting.IronPlateMargin; set { NumberSetting.IronPlateMargin = value; OnPropertyChanged(); } }
    }










}
