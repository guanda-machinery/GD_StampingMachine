using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Extensions;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Interfaces;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using Newtonsoft.Json;
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
    public class QRSettingPageViewModel : PlateSettingPageBaseViewModel//, IStampingPlateVM
    {

        StampingMachineJsonHelper JsonHM = new StampingMachineJsonHelper();
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SettingViewModelQRViewModel");




        private QRSettingViewModel _qrSetting;
        public QRSettingViewModel QRSettingVM
        {
            get
            {
                if (_qrSetting == null)
                    _qrSetting = new QRSettingViewModel();
                return _qrSetting;
            }
            set { _qrSetting = value; OnPropertyChanged(); }
        }


        public override SheetStampingTypeFormEnum SheetStampingTypeForm => SheetStampingTypeFormEnum.qrcode;
        /// <summary>
        /// 選擇
        /// </summary>
        private QRSettingViewModel _qrSettingModelCollectionSelected;
        public QRSettingViewModel QRSettingModelCollectionSelected
        {
            get => _qrSettingModelCollectionSelected;
            set
            {
                _qrSettingModelCollectionSelected = value;
                if (_qrSettingModelCollectionSelected != null)
                    QRSettingVM = _qrSettingModelCollectionSelected.DeepCloneByJson();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<QRSettingViewModel> _qrSettingModelModelCollection;
        public ObservableCollection<QRSettingViewModel> QRSettingModelCollection
        {
            get => _qrSettingModelModelCollection??=new();
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

        public override ICommand LoadSettingCommand
        {
            get => new RelayCommand(() =>
            {
                if (QRSettingModelCollectionSelected != null)
                    QRSettingVM = QRSettingModelCollectionSelected.DeepCloneByJson();
            });
        }
        public override ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                QRSettingVM = new QRSettingViewModel();
            });
        }
        public override ICommand SaveSettingCommand
        {
            get => new AsyncRelayCommand(async () =>
            {
                await Task.Run(async () =>
                {
                    var FIndex = QRSettingModelCollection.ToList().FindIndex(x => x.NumberSettingMode == QRSettingVM.NumberSettingMode);
                    if (FIndex != -1)
                    {
                        if (await Method.MethodWinUIMessageBox.AskOverwriteOrNot())
                        {
                            QRSettingModelCollection[FIndex] = QRSettingVM.DeepCloneByJson();
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        QRSettingModelCollection.Add(QRSettingVM.DeepCloneByJson());
                    }

                    this.JsonHM.WriteParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.QRSetting, QRSettingModelCollection, true);
                });
            });
        }
        public override ICommand DeleteSettingCommand
        {
            get => new RelayCommand(() =>
            {
                if (QRSettingModelCollectionSelected != null)
                {
                    QRSettingModelCollection.Remove(QRSettingModelCollectionSelected);
                    JsonHM.WriteParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.QRSetting, QRSettingModelCollection);
                }
            });
        }




        private QRSettingViewModel _numberSettingVMModelCollectionSelected;
        [JsonIgnore]
        public QRSettingViewModel NumberSettingModelCollectionSelected
        {
            get => _numberSettingVMModelCollectionSelected;
            set
            {
                _numberSettingVMModelCollectionSelected = value;
                if (_numberSettingVMModelCollectionSelected != null)
                    QRSettingVM = _numberSettingVMModelCollectionSelected.DeepCloneByJson();
                OnPropertyChanged();
            }
        }


    }
}
