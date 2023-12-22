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


        public override SheetStampingTypeFormEnum SheetStampingTypeForm => SheetStampingTypeFormEnum.NormalSheetStamping;


        public override int PlateNumberListMax => 8;


        private NumberSettingViewModel _numberSettingVM;
        public NumberSettingViewModel NumberSettingVM
        {
            get=> _numberSettingVM ??= new NumberSettingViewModel()
            {
                SequenceCount = 8
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
                if (value != null)
                {
                    NumberSettingVM = new NumberSettingViewModel(value.StampPlateSetting.DeepCloneByJson());
                    //NumberSettingVM = _numberSettingVMModelCollectionSelected.DeepCloneByJson();
                }
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

        private ICommand _loadSettingCommand;
        public override ICommand LoadSettingCommand
        {
            get => _loadSettingCommand??= new RelayCommand(() =>
            {
                if (NumberSettingModelCollectionSelected != null)
                    NumberSettingVM = NumberSettingModelCollectionSelected.DeepCloneByJson();
            });
        }

        private ICommand _recoverSettingCommand;
        public override ICommand RecoverSettingCommand
        {
            get => _recoverSettingCommand??=new RelayCommand(() =>
            {
                NumberSettingVM = new NumberSettingViewModel();
            });
        }

        public ICommand _saveSettingCommand;
        public override ICommand SaveSettingCommand
        {
            get => _saveSettingCommand??= new AsyncRelayCommand(async () =>
            {
                var FIndex = NumberSettingModelCollection.FindIndex(x => x.NumberSettingMode == NumberSettingVM.NumberSettingMode);
                if (FIndex != -1)
                {
                    if (await Method.MethodWinUIMessageBox.AskOverwriteOrNotAsync() == MessageBoxResult.Yes)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                         {
                             //NumberSettingModelCollection[FIndex] = new NumberSettingViewModel(NumberSettingVM.StampPlateSetting.DeepCloneByJson());
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
                        // NumberSettingModelCollection.Add(new NumberSettingViewModel(NumberSettingVM.StampPlateSetting.DeepCloneByJson()));
                        NumberSettingModelCollection.Add(NumberSettingVM.DeepCloneByJson());
                    });
                }

               await JsonHM.WriteParameterSettingJsonSettingAsync(StampingMachineJsonHelper.ParameterSettingNameEnum.NumberSetting, NumberSettingModelCollection.Select(x => x.StampPlateSetting), true);
            });
        }

        private ICommand _deleteSettingCommand;
        public override ICommand DeleteSettingCommand
        {
            get => _deleteSettingCommand??= new AsyncRelayCommand(async () =>
            {
                if (NumberSettingModelCollectionSelected != null)
                {
                    NumberSettingModelCollection.Remove(NumberSettingModelCollectionSelected);
                   await JsonHM.WriteParameterSettingJsonSettingAsync(StampingMachineJsonHelper.ParameterSettingNameEnum.NumberSetting, NumberSettingModelCollection.Select(x => x.StampPlateSetting));
                }
            });
        }

    }

}