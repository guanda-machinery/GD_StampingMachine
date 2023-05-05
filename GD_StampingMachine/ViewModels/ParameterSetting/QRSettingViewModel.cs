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
      

        public QRSettingViewModel(StampingPlateSettingModel _QRSetting) : base(_QRSetting)
        {
            NumberSetting = _QRSetting;
            QRSettingModelCollectionSelected = QRSettingModelCollection.FirstOrDefault();
        }


        private StampingPlateSettingModel _qrSetting;
        public override StampingPlateSettingModel NumberSetting
        {
            get 
            {
                
                if (_qrSetting == null)
                    _qrSetting = new StampingPlateSettingModel();
                else
                {
                    _qrSetting.NumberSettingMode = NumberSettingMode;
                    if (SequenceCountComboBoxSelectValue.HasValue)
                        _qrSetting.SequenceCount = SequenceCountComboBoxSelectValue.Value;
                    if (SpecialSequenceComboBoxSelectValue.HasValue)
                        _qrSetting.SpecialSequence = SpecialSequenceComboBoxSelectValue.Value;
                    if (VerticalAlignEnumComboBoxSelectValue.HasValue)
                        _qrSetting.VerticalAlign = VerticalAlignEnumComboBoxSelectValue.Value;
                    if (HorizontalAlignEnumComboBoxSelectValue.HasValue)
                        _qrSetting.HorizontalAlign = HorizontalAlignEnumComboBoxSelectValue.Value;

                    _qrSetting.IronPlateMargin = new PlateMarginStruct
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
                if (_qrSetting == new StampingPlateSettingModel ())
                {
                    SequenceCountComboBoxSelectValue = null;
                    SpecialSequenceComboBoxSelectValue = null;
                    VerticalAlignEnumComboBoxSelectValue = null;
                    HorizontalAlignEnumComboBoxSelectValue = null;
                }
                else if (_qrSetting != null)
                {
                    NumberSettingMode = _qrSetting.NumberSettingMode;
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
        private string _numberSettingMode;
        public override string NumberSettingMode
        {
            get => _numberSettingMode;
            set { _numberSettingMode = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 選擇
        /// </summary>
        private StampingPlateSettingModel _qrSettingModelCollectionSelected;
        public StampingPlateSettingModel QRSettingModelCollectionSelected
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

        private ObservableCollection<StampingPlateSettingModel> _qrSettingModelModelCollection;
        public ObservableCollection<StampingPlateSettingModel> QRSettingModelCollection
        {
            get
            {
                if (_qrSettingModelModelCollection == null)
                {
                    if (JsonHM.ReadParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.QRSetting, out ObservableCollection<StampingPlateSettingModel> SavedCollection,false))
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
                NumberSetting = new StampingPlateSettingModel();
            });
        }

        public override ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {
                var FIndex = QRSettingModelCollection.FindIndex(x => x.NumberSettingMode == NumberSettingMode);
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
