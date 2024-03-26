using CommunityToolkit.Mvvm.Input;
using DevExpress.CodeParser;
using DevExpress.Mvvm.Native;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using GD_StampingMachine.Singletons;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{



    public class SeparateSettingViewModel : ParameterSettingBaseViewModel
    {
        StampingMachineJsonHelper JsonHM = new StampingMachineJsonHelper();


        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("NameSeparateSettingViewModel");
        public SeparateSettingViewModel(SeparateSettingModel? _SeparateSetting)
        {
            SeparateSetting = _SeparateSetting;
        }
        public SeparateSettingViewModel()
        {
            SeparateSetting = new();
        }




        [JsonIgnore]
        public SeparateSettingModel SeparateSetting;

        private ObservableCollection<SeparateBoxViewModel> _separateBoxVMObservableCollection;
        public ObservableCollection<SeparateBoxViewModel> SeparateBoxVMObservableCollection
        {
            get
            {
                if (_separateBoxVMObservableCollection == null)
                {
                    _separateBoxVMObservableCollection = new ObservableCollection<SeparateBoxViewModel>();
                    SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection.ForEach(obj =>
                    {
                        _separateBoxVMObservableCollection.Add(new SeparateBoxViewModel(obj));
                    });
                }
                return _separateBoxVMObservableCollection??=new();
            }
            set
            {
                _separateBoxVMObservableCollection = value;
                SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection = new List<SeparateBoxModel>();
                _separateBoxVMObservableCollection?.ForEach(obj =>
                {
                    SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection.Add(obj.SeparateBox);
                });

                foreach (var obj in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                {
                    obj.StampingBoxPartsVM.SeparateBoxVMObservableCollection = value;
                }
                OnPropertyChanged();
            }
        }


        private ObservableCollection<SeparateBoxViewModel> _separateBoxVMObservableCollectionClone;
        public ObservableCollection<SeparateBoxViewModel> SeparateBoxVMObservableCollectionClone
        {
            get => _separateBoxVMObservableCollectionClone ??= SeparateBoxVMObservableCollection.DeepCloneByJson();
            set { _separateBoxVMObservableCollectionClone = value; 
                OnPropertyChanged(); }
        }




        private SeparateBoxViewModel? _singleSetting_SeparateBoxModel;
        public SeparateBoxViewModel SingleSetting_SeparateBoxModel
        {
            get => _singleSetting_SeparateBoxModel ??= new SeparateBoxViewModel(SeparateSetting.SingleSetting_SeparateBox);
            set
            {
                _singleSetting_SeparateBoxModel = value;
                SeparateSetting.SingleSetting_SeparateBox = null;
                if (_singleSetting_SeparateBoxModel != null)
                {
                    SeparateSetting.SingleSetting_SeparateBox = _singleSetting_SeparateBoxModel.SeparateBox;
                }
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// 單一設定
        /// </summary>
        public double SingleSetting_SeparateBoxValue
        {
            get => SeparateSetting.SingleSetting_SeparateBox.BoxSliderValue;
            set
            {
                SeparateSetting.SingleSetting_SeparateBox.BoxSliderValue = value;
                OnPropertyChanged();

                if (SettingType == SettingTypeEnum.UnifiedSetting)
                {
                    SeparateBoxVMObservableCollectionClone.ForEach(x =>
                    {
                        x.BoxSliderValue = SingleSetting_SeparateBoxValue;
                    });
                }
            }
        }





        /// <summary>
        /// 個別設定
        /// </summary>
        /*public bool IsSingleSetting
        {
            get
            {
                return SettingType == SettingTypeEnum.SingleSetting;
            }
            set
            {
                SettingType = SettingTypeEnum.SingleSetting;
                OnPropertyChanged();
            }
        }*/

        /// <summary>
        /// 統一設定
        /// </summary>
        /*public bool IsUnifiedSetting
        {
            get
            {
               return SettingType == SettingTypeEnum.UnifiedSetting;
            }
            set
            {
                SettingType = SettingTypeEnum.UnifiedSetting;
                OnPropertyChanged();
            }
        }*/



        public SettingTypeEnum SettingType
        {
            get
            {
                return SeparateSetting.SettingType;
            }
            set
            {
                SeparateSetting.SettingType = value;
                OnPropertyChanged();
                // OnPropertyChanged(nameof(IsUnifiedSetting));
                //  OnPropertyChanged(nameof(IsSingleSetting));
            }
        }



        public override ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                //SingleSetting_SeparateBoxValue = 200;
                SingleSetting_SeparateBoxValue = double.PositiveInfinity;
                foreach (var separateBox in SeparateBoxVMObservableCollectionClone)
                {
                    separateBox.BoxIsEnabled = true;
                    //separateBox.BoxSliderValue = 200;
                    separateBox.BoxSliderValue = double.PositiveInfinity;
                }

                SeparateBoxVMObservableCollection = SeparateBoxVMObservableCollectionClone.DeepCloneByJson();
                //UpdateProjectDistributeSeparateBox();
            });
        }

        public override ICommand SaveSettingCommand
        {
            get => new AsyncRelayCommand(async () =>
            {
                SeparateBoxVMObservableCollection = SeparateBoxVMObservableCollectionClone.DeepCloneByJson();

                await JsonHM.WriteParameterSettingJsonSettingAsync(StampingMachineJsonHelper.ParameterSettingNameEnum.SeparateSetting, SeparateSetting, true);

                //刷新所有排版的盒子


                //UpdateProjectDistributeSeparateBox();
            });
        }
        public override ICommand LoadSettingCommand
        {
            get => new RelayCommand(() =>
            {
                if (JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.SeparateSetting, out SeparateSettingModel SSetting, true))
                {
                    SeparateSetting = SSetting;
                    OnPropertyChanged(nameof(SettingType));
                    OnPropertyChanged(nameof(SingleSetting_SeparateBoxValue));

                    SeparateBoxVMObservableCollection = new ObservableCollection<SeparateBoxViewModel>(SSetting.UnifiedSetting_SeparateBoxObservableCollection.Select(x=>new SeparateBoxViewModel(x)));
                     SeparateBoxVMObservableCollectionClone = SeparateBoxVMObservableCollection.DeepCloneByJson();
                    //OnPropertyChanged(nameof(SeparateBoxVMObservableCollectionClone));
                    //UpdateProjectDistributeSeparateBox();
                }
            });
        }





        public override ICommand DeleteSettingCommand => throw new NotImplementedException();

        public ICommand SetAllSeparateBoxIsEnabled
        {
            get => new RelayCommand<object>(Parameter =>
            {
                if (Parameter is bool ParameterBoolean)
                {
                    SeparateBoxVMObservableCollectionClone.ForEach(obj =>
                    {
                        obj.BoxIsEnabled = ParameterBoolean;
                    });
                }

            });
        }








    }
}
