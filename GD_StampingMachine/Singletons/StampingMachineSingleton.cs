using DevExpress.Mvvm.Native;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels.ProductSetting;
using GD_StampingMachine.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DevExpress.Data.Extensions;
using CommunityToolkit.Mvvm.Input;

namespace GD_StampingMachine.Singletons
{

    public class StampingMachineSingleton : BaseSingleton<StampingMachineSingleton> , INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        protected void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }




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


       
            if (JsonHM.ReadMachineSettingJson(StampingMachineJsonHelper.MachineSettingNameEnum.UseStampingFont, out ObservableCollection<StampingTypeViewModel> SReaduse))
            {
                StampingFontChangedVM.StampingTypeVMObservableCollection = SReaduse;
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


             if (JsonHM.ReadMachineSettingJson(StampingMachineJsonHelper.MachineSettingNameEnum.UnUseStampingFont, out ObservableCollection<StampingTypeViewModel> SReadunuse))
            {
                StampingFontChangedVM.UnusedStampingTypeVMObservableCollection = SReadunuse;
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
                    //加工專案為到處放的形式 沒有固定位置
                    if (JsonHM.ReadJsonFile(System.IO.Path.Combine(EPath.ProjectPath, EPath.Name), out ProductProjectModel PProject))
                    {
                        ProductSettingVM.ProductProjectVMObservableCollection.Add(new ProductProjectViewModel(PProject));
                    }
                    else
                    {
                        //需註解找不到檔案!
                        MessageBoxResultShow.ShowOK("", $"Can't find file {System.IO.Path.Combine(EPath.ProjectPath, EPath.Name)}");
                        ProductSettingVM.ProductProjectVMObservableCollection.Add(new ProductProjectViewModel(new ProductProjectModel()
                        {
                            ProjectPath = EPath.ProjectPath,
                            Name = EPath.Name,
                            FileIsNotExisted = true
                        })); ;
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


            IsBrightMode = Properties.Settings.Default.IsBrightMode;

            var index_projectDistributeVM = TypeSettingSettingVM.ProjectDistributeVMObservableCollection.FindIndex(x => x.ProjectDistributeName == Properties.Settings.Default.SelectedProjectDistributeName);
            if (index_projectDistributeVM != -1)
                SelectedProjectDistributeVM = TypeSettingSettingVM.ProjectDistributeVMObservableCollection[index_projectDistributeVM];
        }

        public MachanicalSpecificationViewModel MachanicalSpecificationVM { get; set; }
        public StampingFontChangedViewModel StampingFontChangedVM { get; set; }
        public ParameterSettingViewModel ParameterSettingVM { get; set; }
        public ProductSettingViewModel ProductSettingVM { get; set; }
        public TypeSettingSettingViewModel TypeSettingSettingVM { get; set; }
        public MachineMonitorViewModel MachineMonitorVM
        {
            get;
            set;
        }
        public MachineFunctionViewModel MachineFunctionVM
        {
            get;
            set;
        }
        public DXObservableCollection<OperatingLogViewModel> LogDataObservableCollection
        {
            get
            {
                return Singletons.LogDataSingleton.Instance.DataObservableCollection;
            }
        }


        public bool IsBrightMode
        {
            get => Properties.Settings.Default.IsBrightMode;
            set
            {
                Properties.Settings.Default.IsBrightMode = value;
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
                Properties.Settings.Default.Save();
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
                if(value!= null)
                    Properties.Settings.Default.SelectedProjectDistributeName = value.ProjectDistributeName;
                else
                    Properties.Settings.Default.SelectedProjectDistributeName = string.Empty;
                Properties.Settings.Default.Save();
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
