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



namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 繼承
    /// </summary>
    public class QRSettingViewModel : SettingViewModelBase
    {
        public QRSettingViewModel(QRSettingModel _NumberSetting) : base(_NumberSetting)
        {
            NumberSetting = _NumberSetting;
        }

       // public new QRSettingModel NumberSetting { get; set; }

        private QRSettingModel _qrSetting;
        public new QRSettingModel NumberSetting
        {
            get 
            {
                if (_qrSetting == null)
                    _qrSetting = new QRSettingModel();
                else
                {
                    if (SequenceCountComboBoxSelectValue.HasValue)
                        _qrSetting.SequenceCount = SequenceCountComboBoxSelectValue.Value;
                    if (SpecialSequenceComboBoxSelectValue.HasValue)
                        _qrSetting.SpecialSequence = SpecialSequenceComboBoxSelectValue.Value;
                    if (VerticalAlignEnumComboBoxSelectValue.HasValue)
                        _qrSetting.VerticalAlign = VerticalAlignEnumComboBoxSelectValue.Value;
                    if (HorizontalAlignEnumComboBoxSelectValue.HasValue)
                        _qrSetting.HorizontalAlign = HorizontalAlignEnumComboBoxSelectValue.Value;

                    _qrSetting.IronPlateMargin = new QR_IronPlateMarginStruct
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
                return _qrSetting;
            } 
            set 
            {
                _qrSetting = value;
                if (_qrSetting == new QRSettingModel ())
                {
                    SequenceCountComboBoxSelectValue = null;
                    SpecialSequenceComboBoxSelectValue = null;
                    VerticalAlignEnumComboBoxSelectValue = null;
                    HorizontalAlignEnumComboBoxSelectValue = null;
                }
                else if (_qrSetting != null)
                {
                    SequenceCountComboBoxSelectValue = _qrSetting.SequenceCount;
                    SpecialSequenceComboBoxSelectValue = _qrSetting.SpecialSequence;
                    VerticalAlignEnumComboBoxSelectValue = _qrSetting.VerticalAlign;
                    HorizontalAlignEnumComboBoxSelectValue = _qrSetting.HorizontalAlign;

                    this.Margin_A = _qrSetting.IronPlateMargin.A_Margin;
                    this.Margin_B = _qrSetting.IronPlateMargin.B_Margin;
                    this.Margin_C = _qrSetting.IronPlateMargin.C_Margin;
                    this.Margin_D = _qrSetting.IronPlateMargin.D_Margin;
                    this.Margin_E = _qrSetting.IronPlateMargin.E_Margin;
                    this.Margin_F = _qrSetting.IronPlateMargin.F_Margin;
                    this.Margin_G = _qrSetting.IronPlateMargin.G_Margin;
                    this.Margin_H = _qrSetting.IronPlateMargin.H_Margin;
                    this.Margin_I = _qrSetting.IronPlateMargin.I_Margin;


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
        private QRSettingModel _qrSettingModelCollectionSelected;
        public QRSettingModel QRSettingModelCollectionSelected
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

        private ObservableCollection<QRSettingModel> _qrSettingModelModelCollection;
        public ObservableCollection<QRSettingModel> QRSettingModelCollection
        {
            get
            {
                if (_qrSettingModelModelCollection == null)
                {

                    if (JsonHM.ReadParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.QRSetting, out ObservableCollection<QRSettingModel> SavedCollection,false))
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
                NumberSetting = new QRSettingModel();
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
    }










}
