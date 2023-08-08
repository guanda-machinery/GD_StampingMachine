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

namespace GD_StampingMachine.ViewModels.ParameterSetting
{

    public class NumberSettingPageViewModel : PlateSettingPageBaseViewModel
    {
        [JsonIgnore]
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SettingViewModelNormalViewModel");

  

        private NumberSettingViewModel _numberSettingVM;
        public NumberSettingViewModel NumberSettingVM
        {
            get
            {
                if (_numberSettingVM == null)
                    _numberSettingVM = new NumberSettingViewModel();
                return _numberSettingVM;
            }
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

        private ObservableCollection<NumberSettingViewModel> _numberSettingVMModelSavedCollection;
        public ObservableCollection<NumberSettingViewModel> NumberSettingModelSavedCollection
        {
            get
            {
                if (_numberSettingVMModelSavedCollection == null)
                {
                    if (JsonHM.ReadParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.NumberSetting, out ObservableCollection<NumberSettingViewModel> SavedCollection))
                        _numberSettingVMModelSavedCollection = SavedCollection;
                    else
                        _numberSettingVMModelSavedCollection = new();
                }
                return _numberSettingVMModelSavedCollection;
            }
            set
            {
                _numberSettingVMModelSavedCollection = value;
                OnPropertyChanged(nameof(NumberSettingModelSavedCollection));
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
            get => new RelayCommand(() =>
            {
                var FIndex = NumberSettingModelSavedCollection.FindIndex(x => x.NumberSettingMode == NumberSettingVM.NumberSettingMode);
                if (FIndex != -1)
                {
                    if (Method.MethodWinUIMessageBox.AskOverwriteOrNot())
                    {
                        NumberSettingModelSavedCollection[FIndex] = NumberSettingVM.DeepCloneByJson();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    NumberSettingModelSavedCollection.Add(NumberSettingVM.DeepCloneByJson());
                }

                JsonHM.WriteParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.NumberSetting, NumberSettingModelSavedCollection, true);
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


        public override int PlateNumberListMax => 8;
    }

}