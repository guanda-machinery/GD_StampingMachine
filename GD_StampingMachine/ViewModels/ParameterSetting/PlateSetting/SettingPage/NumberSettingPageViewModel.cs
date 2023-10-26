using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf.Native;
using DevExpress.Utils.Extensions;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Interfaces;
using GD_StampingMachine.Method;
using GD_StampingMachine.GD_Model;
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
using GD_StampingMachine.Model;
using Newtonsoft.Json.Linq;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Data;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{

    public class NumberSettingPageViewModel : PlateSettingPageBaseViewModel
    {

        StampingMachineJsonHelper JsonHM = new StampingMachineJsonHelper();

        [JsonIgnore]
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SettingViewModelNormalViewModel");


        public override SheetStampingTypeFormEnum SheetStampingTypeForm => SheetStampingTypeFormEnum.normal;





        private NumberSettingViewModel _numberSettingVM;
        public NumberSettingViewModel NumberSettingVM
        {
            get=> _numberSettingVM ??= new NumberSettingViewModel()
            {
                SequenceCount = 6
            };
            set
            {
                _numberSettingVM = value;
                OnPropertyChanged();
            }
        }


        private NumberSettingViewModel _numberSettingVMModelCollectionSelected;
        [JsonIgnore]
        public NumberSettingViewModel NumberSettingModelCollectionSelected
        {
            get => _numberSettingVMModelCollectionSelected;
            set
            {
                _numberSettingVMModelCollectionSelected = value;
                if (_numberSettingVMModelCollectionSelected != null)
                    NumberSettingVM = _numberSettingVMModelCollectionSelected.DeepCloneByJson();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<NumberSettingViewModel> _numberSettingModelCollection;
        public ObservableCollection<NumberSettingViewModel> NumberSettingModelCollection
        {
            get => _numberSettingModelCollection ??= new();
            set
            {
                _numberSettingModelCollection = value;
                OnPropertyChanged();
            }
        }

        public override ICommand LoadSettingCommand
        {
            get => new RelayCommand(() =>
            {
                if (NumberSettingModelCollectionSelected != null)
                    NumberSettingVM = NumberSettingModelCollectionSelected.DeepCloneByJson();
            });
        }

        public override ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                NumberSettingVM = new NumberSettingViewModel();
            });
        }

        public override ICommand SaveSettingCommand
        {
            get => new AsyncRelayCommand(async () =>
            {
                await Task.Run(async () =>
                {

                    var FIndex = NumberSettingModelCollection.FindIndex(x => x.NumberSettingMode == NumberSettingVM.NumberSettingMode);
                    if (FIndex != -1)
                    {
                        if (await Method.MethodWinUIMessageBox.AskOverwriteOrNot())
                        {
                           Application.Current.Dispatcher.Invoke(() =>
                            {
                                NumberSettingModelCollection[FIndex] = NumberSettingVM.DeepCloneByJson();
                            });
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            NumberSettingModelCollection.Add(NumberSettingVM.DeepCloneByJson());
                        });
                    }

                    JsonHM.WriteParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.NumberSetting, NumberSettingModelCollection, true);
                });
            });
        }
        public override ICommand DeleteSettingCommand
        {
            get => new RelayCommand(() =>
            {
                if (NumberSettingModelCollectionSelected != null)
                {
                    NumberSettingModelCollection.Remove(NumberSettingModelCollectionSelected);
                    JsonHM.WriteParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.NumberSetting, NumberSettingModelCollection);
                }
            });
        }

    }

}