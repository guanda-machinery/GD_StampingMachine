using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Native;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels;
using GD_StampingMachine.ViewModels.ProductSetting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            MachanicalSpecificationVM = new MachanicalSpecificationViewModel(new MachanicalSpecificationModel()
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

            StampingFontChangedVM = new StampingFontChangedViewModel();



            if (JsonHM.ReadUseStampingFont(out var SReaduse))
            {
                StampingFontChangedVM.StampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>(
                    SReaduse.Select(use => new StampingTypeViewModel(use)));
            }
            else
            {
                StampingFontChangedVM.StampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>();

                for (int i = 1; i <= 40; i++)
                {
                    // char
                    StampingFontChangedVM.StampingTypeVMObservableCollection.Add(
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

            ParameterSettingModel ParameterSetting = new()
            {
                AxisSetting = AxisSetting,
                TimingSetting = TimingSetting,
                SeparateSetting = SeparateSetting,
                InputOutput = new(),
                EngineerSetting = EngineerSetting
            };

            ParameterSettingVM = new(ParameterSetting);

            ProductSettingVM = new();

            if (JsonHM.ReadProjectSettingJson(out List<ProjectModel> PathList))
            {
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
                        ProductSettingVM.ProductProjectVMObservableCollection.Add(new ProductProjectViewModel(PProject));
                    }
                    else
                    {
                        //需註解找不到檔案!
                        _ = MessageBoxResultShow.ShowOKAsync("", $"Can't find file {fileName}");
                        ProductSettingVM.ProductProjectVMObservableCollection.Add(new ProductProjectViewModel(new ProductProjectModel()
                        {
                            ProjectPath = EPath.ProjectPath,
                            Name = EPath.Name,
                        })
                        {
                            FileIsNotExisted = true
                        });
                    }
                });
            }

            TypeSettingSettingVM = new();


            if (JsonHM.ReadProjectDistributeListJson(out var RPDList))
            {
                RPDList.ForEach(PDistribute =>
                {
                    //PDistribute.ProductProjectNameList
                    PDistribute.ProductProjectVMObservableCollection = ProductSettingVM.ProductProjectVMObservableCollection;
                    PDistribute.SeparateBoxVMObservableCollection = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection;
                    //將製品清單拆分成兩份
                    TypeSettingSettingVM.ProjectDistributeVMObservableCollection.Add(new ProjectDistributeViewModel(PDistribute)
                    {
                        IsInDistributePage = false
                        //重新繫結
                    });

                });
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
        public MachanicalSpecificationViewModel MachanicalSpecificationVM { get; set; }
        /// <summary>
        /// 字模
        /// </summary>
        public StampingFontChangedViewModel StampingFontChangedVM { get; set; }
        /// <summary>
        /// 參數設定
        /// </summary>
        public ParameterSettingViewModel ParameterSettingVM { get; set; }
        /// <summary>
        /// 製品設定
        /// </summary>
        public ProductSettingViewModel ProductSettingVM { get; set; }
        /// <summary>
        /// 生產設定
        /// </summary>
        public TypeSettingSettingViewModel TypeSettingSettingVM { get; set; }
        /// <summary>
        /// 機台監控
        /// </summary>
        public MachineMonitorViewModel MachineMonitorVM
        {
            get;
            set;
        }
        /// <summary>
        /// 機台功能
        /// </summary>
        public MachineFunctionViewModel MachineFunctionVM
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
                }
                else
                {
                    Application.Current.Resources["PrimaryHueLightBrush"] = (SolidColorBrush)Application.Current.TryFindResource("DarkHueLightBrush");
                    Application.Current.Resources["PrimaryHueLightForegroundBrush"] = Application.Current.TryFindResource("DarkHueLightForegroundBrush");
                    Application.Current.Resources["PrimaryHueMidBrush"] = Application.Current.TryFindResource("DarkHueMidBrush");
                    Application.Current.Resources["PrimaryHueMidForegroundBrush"] = Application.Current.TryFindResource("DarkHueMidForegroundBrush");
                    Application.Current.Resources["PrimaryHueDarkBrush"] = Application.Current.TryFindResource("DarkHueDarkBrush");
                    Application.Current.Resources["PrimaryHueDarkForegroundBrush"] = Application.Current.TryFindResource("DarkHueDarkForegroundBrush");
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







        private ProjectDistributeViewModel _selectedProjectDistributeVM;
        /// <summary>
        /// 目前選定的加工專案
        /// </summary>
        public ProjectDistributeViewModel SelectedProjectDistributeVM
        {
            get => _selectedProjectDistributeVM;
            set
            {
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
            }
        }

        public ICommand ProjectDistributeVMChangeCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                if (obj is ObservableCollection<ProjectDistributeViewModel> NewProjectDistributeVMObser)
                {
                    SelectedProjectDistributeVM = NewProjectDistributeVMObser.FirstOrDefault(); ;
                }
                if (obj is ProjectDistributeViewModel NewProjectDistributeVM)
                {
                    SelectedProjectDistributeVM = NewProjectDistributeVM;
                }










            });
        }

    }
}
