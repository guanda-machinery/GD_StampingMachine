using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Interfaces;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;




namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 繼承
    /// </summary>
    public class QRSettingViewModel : SettingViewModelBase
    {


        private QRSettingModel _qrSetting = new QRSettingModel();
        public QRSettingModel QRSetting
        {
            get 
            {
                if (_qrSetting == null)
                    _qrSetting = new QRSettingModel();

                if (_qrSetting != null)
                {
                    if (SequenceCountComboBoxSelectValue.HasValue)
                        _qrSetting.SequenceCount = SequenceCountComboBoxSelectValue.Value;
                    if (SpecialSequenceComboBoxSelectValue.HasValue)
                        _qrSetting.SpecialSequence = SpecialSequenceComboBoxSelectValue.Value;
                    if (VerticalAlignEnumComboBoxSelectValue.HasValue)
                        _qrSetting.VerticalAlign = VerticalAlignEnumComboBoxSelectValue.Value;
                    if (HorizontalAlignEnumComboBoxSelectValue.HasValue)
                        _qrSetting.HorizontalAlign = HorizontalAlignEnumComboBoxSelectValue.Value;
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
                }
                OnPropertyChanged();
            }

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
                    QRSetting = _qrSettingModelCollectionSelected.Clone() as QRSettingModel;
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
                    CsvHM.ReadQRNumberSetting(out var SavedCollection);
                    _qrSettingModelModelCollection = SavedCollection.ToObservableCollection();
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


        public override ICommand LoadModeCommand
        {
            get => new RelayCommand(() =>
            {
                if (QRSettingModelCollectionSelected != null)
                    QRSetting = QRSettingModelCollectionSelected.Clone() as QRSettingModel;
            });
        }
        public override ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                QRSetting = new QRSettingModel();
            });
        }

        public override ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {
                var FIndex = QRSettingModelCollection.FindIndex(x => x.NumberSettingMode == QRSetting.NumberSettingMode);
                if (FIndex != -1)
                {
                    if (Method.MethodWinUIMessageBox.AskOverwriteOrNot())
                    {
                        QRSettingModelCollection[FIndex] = QRSetting.Clone() as QRSettingModel;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    QRSettingModelCollection.Add(QRSetting.Clone() as QRSettingModel);
                }

                if (this.CsvHM.WriteQRNumberSetting(QRSettingModelCollection.ToList()))
                    Method.MethodWinUIMessageBox.SaveSuccessful(true);
                else
                    Method.MethodWinUIMessageBox.SaveSuccessful(false);
            });
        }

        public override ICommand DeleteSettingCommand
        {
            get => new RelayCommand(() =>
            {
                if (QRSettingModelCollectionSelected != null)
                {
                    QRSettingModelCollection.Remove(QRSettingModelCollectionSelected);
                    CsvHM.WriteQRNumberSetting(QRSettingModelCollection.ToList());
                }
            });
        }
    }










}
