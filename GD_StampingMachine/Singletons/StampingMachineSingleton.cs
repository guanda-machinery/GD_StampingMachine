using CommunityToolkit.Mvvm.Input;
using DevExpress.CodeParser;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Extensions;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels;
using GD_StampingMachine.ViewModels.ParameterSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GD_StampingMachine.Singletons
{

    public class StampingMachineSingleton : BaseSingleton<StampingMachineSingleton>
    {
        protected override void Init()
        {
            StampingMachineJsonHelper JsonHM = new StampingMachineJsonHelper();
            MechanicalSpecificationVM = new MachanicalSpecificationViewModel(new MachanicalSpecificationModel()
            {
                AllowMachiningSize = new AllowMachiningSizeModel()
                {
                    WebHeightLowerLimited = 75,
                    WebHeightUpperLimited = 500,
                    FlangeWidthLowerLimited = 150,
                    FlangeWidthUpperLimited = 1050,
                    MachiningMinLength = 2400,
                    MachiningMaxLength = 99999
                },
                MachiningProperty = new MachiningPropertyModel()
                {
                    HorizontalDrillCount = 1,
                    VerticalDrillCount = 2,
                    Each_HorizontalDrill_SpindleCount = 1,
                    Each_VerticalDrill_SpindleCount = 1,
                    AuxiliaryAxisEffectiveTravelMax = 300,
                    MaxDrillDiameter = 40,
                    MaxDrillThickness = 80,
                    SpindleMaxPower = 15,
                    SpindleToolHolder = SpindleToolHolderEnum.BT40,
                    SpindleRotationalFrequencyMin = 180,
                    SpindleRotationalFrequencyMax = 400,
                    SpindleFeedSpeedMin = 40,
                    SpindleFeedSpeedMax = 1000,
                    SpindleMoveSpeed = 24
                },
                MachineSize = new MachineSizeModel()
                {
                    Length = 5450,
                    Width = 2000,
                    Height = 2000,
                    Weight = 14.5
                },
            })
            {

            };

            List<StampingTypeViewModel> StampingCollection = new();
            if (JsonHM.ReadUseStampingFont(out var SReaduse))
            {
                StampingCollection = SReaduse.Select(use => new StampingTypeViewModel(use)).ToList();
            }
            else
            {
                for (int i = 1; i <= 40; i++)
                {
                    // char
                    StampingCollection.Add(
                        new StampingTypeViewModel(
                            new StampingTypeModel()
                            {
                                StampingTypeNumber = i,
                                StampingTypeString = (64 + i).ToChar().ToString(),
                                StampingTypeUseCount = 0
                            })
                        );
                }
            }

            StampingFontChangedVM = new StampingFontChangedViewModel(StampingCollection);
            
            {
                if (JsonHM.ReadUnUseStampingFont(out List<StampingTypeModel> SReadunuse))
                {
                    StampingFontChangedVM.UnusedStampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>(
                        SReadunuse.Select(unuse => new StampingTypeViewModel(unuse)));
                }
                else
                {
                    StampingFontChangedVM.UnusedStampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>()
                {
                    new StampingTypeViewModel(new StampingTypeModel(){ StampingTypeNumber =0 , StampingTypeString = "ㄅ" , StampingTypeUseCount=0}) ,
                    new StampingTypeViewModel(new StampingTypeModel(){ StampingTypeNumber =0 , StampingTypeString = "ㄆ" , StampingTypeUseCount=0}),
                    new StampingTypeViewModel(new StampingTypeModel(){ StampingTypeNumber =0, StampingTypeString = "ㄇ" , StampingTypeUseCount=0})
                };
                }
                AxisSettingModel AxisSetting = new();
                if (JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.AxisSetting, out AxisSettingModel JsonAxisSetting))
                {
                    AxisSetting = JsonAxisSetting;
                }

                TimingSettingModel TimingSetting = new();
                if (JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.TimingSetting, out TimingSettingModel JsonTimingSetting))
                {
                    TimingSetting = JsonTimingSetting;
                }

                EngineerSettingModel EngineerSetting = new();
                if (JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.EngineerSetting, out EngineerSettingModel JsonEngineerSetting))
                {
                    EngineerSetting = JsonEngineerSetting;
                }

                SeparateSettingModel SeparateSetting = new();
                if (JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.SeparateSetting, out SeparateSettingModel JsonSeparateSetting))
                {
                    SeparateSetting = JsonSeparateSetting;
                }
                else
                {
                    List<SeparateBoxModel> boxCollection = new();
                    for (int i = 0; i < 10; i++)
                    {
                        boxCollection.Add(new SeparateBoxModel()
                        {
                            BoxIndex = i + 1,
                            BoxIsEnabled = true,
                            BoxIsUsing = false,
                            BoxSliderValue = 0
                        });
                    }

                    SeparateSetting = new()
                    {
                        UnifiedSetting_SeparateBoxObservableCollection = boxCollection
                    };
                }
                ParameterSettingModel ParameterSetting = new()
                {
                    AxisSetting = AxisSetting,
                    TimingSetting = TimingSetting,
                    SeparateSetting = SeparateSetting,
                    InputOutput = new(),
                    EngineerSetting = EngineerSetting
                };

                ParameterSettingVM = new(ParameterSetting);
            }

            ProductSettingVM = new();

            if (JsonHM.ReadProjectSettingJson(out List<ProjectModel> PathList))
            {
                List<string> FileNotExistedList = new List<string>();
                PathList.ForEach(EPath =>
                {
                    string fileName = null;
                    if (!string.IsNullOrEmpty(EPath.ProjectPath) && !string.IsNullOrEmpty(EPath.Name))
                    {
                        fileName = System.IO.Path.Combine(EPath.ProjectPath, EPath.Name);
                    }

                    //加工專案為到處放的形式 沒有固定位置
                    if (!string.IsNullOrEmpty(fileName) &&
                    JsonHM.ReadJsonFile(fileName, out ProductProjectModel PProject))
                    {
                        ProductSettingVM.ProductProjectVMCollection.Add(new ProductProjectViewModel(PProject));
                    }
                    else
                    {
                        //需註解找不到檔案!
                        FileNotExistedList.Add(fileName);
                        ProductSettingVM.ProductProjectVMCollection.Add(new ProductProjectViewModel(new ProductProjectModel()
                        {
                            ProjectPath = EPath.ProjectPath,
                            Name = EPath.Name,
                        })
                        {
                            FileIsNotExisted = true
                        });
                    }
                });

                //這邊開始顯示
                if(FileNotExistedList.FirstOrDefault(x=> !string.IsNullOrWhiteSpace(x)) !=null )
                    _ = MessageBoxResultShow.ShowOKAsync(null,"", $"{(string)Application.Current.TryFindResource("Text_FileNotExisted")} {FileNotExistedList.FirstOrDefault(x=> !string.IsNullOrWhiteSpace(x))}", GD_MessageBoxNotifyResult.NotifyRd);
            }

            TypeSettingSettingVM = new();

            if (JsonHM.ReadProjectDistributeListJson(out var RPDList))
            {
                //RPDList.ForEach(PDistribute =>
                foreach(var PDistribute in RPDList)
                {
                    try
                    {
                        var separateBoxExtVMCollection = new SeparateBoxExtViewModelObservableCollection(ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection);
                        //將製品清單拆分成兩份
                        TypeSettingSettingVM.ProjectDistributeVMObservableCollection.Add(new ProjectDistributeViewModel(PDistribute, separateBoxExtVMCollection, ProductSettingVM.ProductProjectVMCollection)); 
                    }
                    catch
                    {
                        Debugger.Break();
                    }
                }
            }


            

            MachineMonitorVM = new();

            MachineFunctionVM = new();



            try
            {
                var projectDistributeVM = TypeSettingSettingVM.ProjectDistributeVMObservableCollection.FirstOrDefault(x => x.ProjectDistributeName == Properties.Settings.Default.SelectedProjectDistributeName);
                if (projectDistributeVM != null)
                    SelectedProjectDistributeVM = projectDistributeVM;
            }
            catch
            {

            }


            IsBrightMode = Properties.Settings.Default.IsBrightMode;
            try
            {
                IsBrightMode = Properties.Settings.Default.IsBrightMode;
            }
            catch
            {

            }



        }
    


        /// <summary>
        /// 機台規格
        /// </summary>
        public MachanicalSpecificationViewModel? MechanicalSpecificationVM { get; set; }
        /// <summary>
        /// 字模
        /// </summary>
        public StampingFontChangedViewModel? StampingFontChangedVM { get; set; }
        /// <summary>
        /// 參數設定
        /// </summary>
        public ParameterSettingViewModel? ParameterSettingVM { get; set; }
        /// <summary>
        /// 製品設定
        /// </summary>
        public ProductSettingViewModel? ProductSettingVM { get; set; }
        /// <summary>
        /// 排版設定(生產設定)
        /// </summary>
        public TypeSettingSettingViewModel? TypeSettingSettingVM { get; set; }
        /// <summary>
        /// 機台監控
        /// </summary>
        public MachineMonitorViewModel? MachineMonitorVM
        {
            get;
            set;
        }
        /// <summary>
        /// 機台功能
        /// </summary>
        public MachineFunctionViewModel? MachineFunctionVM
        {
            get;
            set;
        }


        private bool _isBrightMode;
        public bool IsBrightMode
        {
            get => _isBrightMode;//;Properties.Settings.Default.IsBrightMode;
            set
            {
                _isBrightMode = value;
                if (value)
                {
                    Application.Current.Resources["PrimaryHueLightBrush"] = Application.Current.TryFindResource("BrightHueLightBrush");
                    Application.Current.Resources["PrimaryHueLightForegroundBrush"] = Application.Current.TryFindResource("BrightHueLightForegroundBrush");
                    Application.Current.Resources["PrimaryHueMidBrush"] = Application.Current.TryFindResource("BrightHueMidBrush");
                    Application.Current.Resources["PrimaryHueMidForegroundBrush"] = Application.Current.TryFindResource("BrightHueMidForegroundBrush");
                    Application.Current.Resources["PrimaryHueDarkBrush"] = Application.Current.TryFindResource("BrightHueDarkBrush");
                    Application.Current.Resources["PrimaryHueDarkForegroundBrush"] = Application.Current.TryFindResource("BrightHueDarkForegroundBrush");
                    Application.Current.Resources["PrimaryButtonBrush"] = Application.Current.TryFindResource("BrightPrimaryButtonBrush");
              
                    Application.Current.Resources["PrimaryGridControlBackground"] = Application.Current.TryFindResource("BrightPrimaryGridControlBackground");
                    Application.Current.Resources["SecondGridControlBackground"] = Application.Current.TryFindResource("BrightSecondGridControlBackground");

                }
                else
                {
                    Application.Current.Resources["PrimaryHueLightBrush"] = (SolidColorBrush)Application.Current.TryFindResource("DarkHueLightBrush");
                    Application.Current.Resources["PrimaryHueLightForegroundBrush"] = Application.Current.TryFindResource("DarkHueLightForegroundBrush");
                    Application.Current.Resources["PrimaryHueMidBrush"] = Application.Current.TryFindResource("DarkHueMidBrush");
                    Application.Current.Resources["PrimaryHueMidForegroundBrush"] = Application.Current.TryFindResource("DarkHueMidForegroundBrush");
                    Application.Current.Resources["PrimaryHueDarkBrush"] = Application.Current.TryFindResource("DarkHueDarkBrush");
                    Application.Current.Resources["PrimaryHueDarkForegroundBrush"] = Application.Current.TryFindResource("DarkHueDarkForegroundBrush");
                    Application.Current.Resources["PrimaryButtonBrush"] = Application.Current.TryFindResource("DarkPrimaryButtonBrush");
              
                    Application.Current.Resources["PrimaryGridControlBackground"] = Application.Current.TryFindResource("DarkPrimaryGridControlBackground");
                    Application.Current.Resources["SecondGridControlBackground"] = Application.Current.TryFindResource("DarkSecondGridControlBackground");

                }

                try
                {
                    Properties.Settings.Default.IsBrightMode = value;
                    Properties.Settings.Default.Save();
                }
                catch
                {

                }

                OnPropertyChanged();
            }
        }


        public bool FalseValue
        {
            get => false;
            set
            { 
            }
        }






        private ProjectDistributeViewModel? _selectedProjectDistributeVM;
        /// <summary>
        /// 目前選定的加工專案
        /// </summary>
        public ProjectDistributeViewModel? SelectedProjectDistributeVM
        {
            get => _selectedProjectDistributeVM;
            set
            {
                var oldvalue = _selectedProjectDistributeVM;
                _selectedProjectDistributeVM = value;
                try
                {
                    if (value != null)
                        Properties.Settings.Default.SelectedProjectDistributeName = value.ProjectDistributeName;
                    else
                        Properties.Settings.Default.SelectedProjectDistributeName = string.Empty;
                    Properties.Settings.Default.Save();
                }
                catch
                {

                }
                OnPropertyChanged();
                OnIsWorkChanged(new GD_CommonLibrary.ValueChangedEventArgs<ProjectDistributeViewModel>(oldvalue, value));

            }
        }

        public event EventHandler<GD_CommonLibrary.ValueChangedEventArgs<ProjectDistributeViewModel>>? SelectedProjectDistributeVMChanged;

        protected virtual void OnIsWorkChanged(GD_CommonLibrary.ValueChangedEventArgs<ProjectDistributeViewModel> e)
        {
            SelectedProjectDistributeVMChanged?.Invoke(this, e);
        }




        public ICommand ProjectDistributeVMChangeCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                if (obj is ObservableCollection<ProjectDistributeViewModel> newProjectDistributeVMCollection)
                {
                    SelectedProjectDistributeVM = newProjectDistributeVMCollection.FirstOrDefault(); ;
                }
                if (obj is ProjectDistributeViewModel NewProjectDistributeVM)
                {
                    SelectedProjectDistributeVM = NewProjectDistributeVM;
                }
            });
        }

    }
}
