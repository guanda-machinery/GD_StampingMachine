using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf.Native;
using DevExpress.Utils.Extensions;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
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
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class NumberSettingViewModel : SettingViewModelBase
    {
        public NumberSettingViewModel(NormalSettingModel _NumberSetting) : base(_NumberSetting)
        {        
            NumberSetting = _NumberSetting;
        }

       private NormalSettingModel _numberSetting;
        public override NormalSettingModel NumberSetting
        {
            get
            {
                if (_numberSetting == null)
                    _numberSetting = new QRSettingModel();
                if (_numberSetting != null)
                {
                    if (SequenceCountComboBoxSelectValue.HasValue)
                        _numberSetting.SequenceCount = SequenceCountComboBoxSelectValue.Value;
                    if (SpecialSequenceComboBoxSelectValue.HasValue)
                        _numberSetting.SpecialSequence = SpecialSequenceComboBoxSelectValue.Value;
                    if (VerticalAlignEnumComboBoxSelectValue.HasValue)
                        _numberSetting.VerticalAlign = VerticalAlignEnumComboBoxSelectValue.Value;
                    if (HorizontalAlignEnumComboBoxSelectValue.HasValue)
                        _numberSetting.HorizontalAlign = HorizontalAlignEnumComboBoxSelectValue.Value;

                    _numberSetting.IronPlateMargin = new Normal_IronPlateMarginStruct
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
                if (_numberSetting == new QRSettingModel())
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
                }
                OnPropertyChanged();
            }
        }

        public override string NumberSettingMode
        {
            get => NumberSetting.NumberSettingMode;
            set { NumberSetting.NumberSettingMode = value; OnPropertyChanged(); }
        }



        private NormalSettingModel _numberSettingModelCollectionSelected;
        public NormalSettingModel NumberSettingModelCollectionSelected
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

        private ObservableCollection<NormalSettingModel> _numberSettingModelSavedCollection;


        public ObservableCollection<NormalSettingModel> NumberSettingModelSavedCollection
        {
            get
            {
                if (_numberSettingModelSavedCollection == null)
                {
                    if (JsonHM.ReadParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.NumberSetting, out ObservableCollection<NormalSettingModel> SavedCollection))
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

        public override int PlateNumberListMax => 8;

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
                NumberSetting = new NormalSettingModel();
            });
        }
        public override ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {
                var FIndex = NumberSettingModelSavedCollection.FindIndex(x => x.NumberSettingMode == NumberSetting.NumberSettingMode);
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
    }

}
