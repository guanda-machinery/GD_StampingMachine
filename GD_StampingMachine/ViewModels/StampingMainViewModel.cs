using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.CommonDialogs;
using DevExpress.Utils.CommonDialogs.Internal;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.ViewModels.ProductSetting;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using GD_CommonLibrary;
using DevExpress.Xpf.Editors;
using Newtonsoft.Json;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.SplashScreenWindows;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Mvvm;
using System.Windows.Controls;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System.Windows.Media;

namespace GD_StampingMachine.ViewModels
{
    public class StampingMainModel
    {
        public MachanicalSpecificationViewModel MachanicalSpecificationVM { get; set; } 
        public StampingFontChangedViewModel StampingFontChangedVM { get; set; }
        public ParameterSettingViewModel ParameterSettingVM { get; set; }
        public ProductSettingViewModel ProductSettingVM { get; set; }
        public TypeSettingSettingViewModel TypeSettingSettingVM { get; set; }
        public MachineMonitorViewModel MachineMonitorVM { get; set; }
        public MachineFunctionViewModel MachineFunctionVM { get; set; }
    }

    public partial class StampingMainViewModel : BaseViewModelWithLog
    {
        /// <summary>
        /// 解構
        /// </summary>

        ~StampingMainViewModel()
        {
            if (ParameterSettingVM.AxisSettingVM.AxisSetting != null)
                JsonHM.WriteParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.AxisSetting, ParameterSettingVM.AxisSettingVM.AxisSetting);

            if (ParameterSettingVM.TimingSettingVM.TimingSetting != null)
                JsonHM.WriteParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.TimingSetting, ParameterSettingVM.TimingSettingVM.TimingSetting);

            if (ParameterSettingVM.EngineerSettingVM.EngineerSetting != null)
                JsonHM.WriteParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.EngineerSetting, ParameterSettingVM.EngineerSettingVM.EngineerSetting);

            if (ParameterSettingVM.SeparateSettingVM.SeparateSetting != null)
                JsonHM.WriteParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.SeparateSetting, ParameterSettingVM.SeparateSettingVM.SeparateSetting);
        }




        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingMainViewModel");

        private StampingMainModel StampingMain = new();

        public StampingMainViewModel()
        {
            StampingMain = new StampingMainModel();
            //測試模式
            if (Debugger.IsAttached)
            {
                Task.Run(() =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        AddLogData("Debug", $"TestMessage-{i}");
                    }

                    for (int ErrorCount = 0; true; ErrorCount++)
                    {
                        AddLogData("Debug", $"TestMessage-{ErrorCount}", ErrorCount % 5 == 0);
                        Thread.Sleep(1000);

                    }
                });
            }




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

            if (JsonHM.ReadMachineSettingJson(GD_JsonHelperMethod.MachineSettingNameEnum.StampingFont, out StampingFontChangedViewModel SReadSFC))
            {
                StampingFontChangedVM = SReadSFC;
            }
            else
            {
                StampingFontChangedVM = new StampingFontChangedViewModel
                {
                    StampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>()
                };
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
                StampingFontChangedVM.UnusedStampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>()
                {
                    new StampingTypeViewModel(new StampingTypeModel(){ StampingTypeNumber =0 , StampingTypeString = "ㄅ" , StampingTypeUseCount=0}) ,
                    new StampingTypeViewModel(new StampingTypeModel(){ StampingTypeNumber =0 , StampingTypeString = "ㄆ" , StampingTypeUseCount=0}),
                    new StampingTypeViewModel(new StampingTypeModel(){ StampingTypeNumber =0, StampingTypeString = "ㄇ" , StampingTypeUseCount=0})
                };
            }


            AxisSettingModel AxisSetting = new();
            if (JsonHM.ReadParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.AxisSetting, out AxisSettingModel JsonAxisSetting))
            {
                AxisSetting = JsonAxisSetting;
            }

            TimingSettingModel TimingSetting = new();
            if (JsonHM.ReadParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.TimingSetting, out TimingSettingModel JsonTimingSetting))
            {
                TimingSetting = JsonTimingSetting;
            }

            EngineerSettingModel EngineerSetting = new();
            if (JsonHM.ReadParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.EngineerSetting, out EngineerSettingModel JsonEngineerSetting))
            {
                EngineerSetting = JsonEngineerSetting;
            }

            SeparateSettingModel SeparateSetting = new();
            if (JsonHM.ReadParameterSettingJsonSetting(GD_JsonHelperMethod.ParameterSettingNameEnum.SeparateSetting, out SeparateSettingModel JsonSeparateSetting))
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

            ProductSettingVM = new()
            {
                ProductProjectVMObservableCollection = new ObservableCollection<ProductProjectViewModel>()
            };
            if (JsonHM.ReadProjectSettingJson(out List<ProjectModel> PathList))
            {
                PathList.ForEach(EPath =>
                {
                    //加工專案為到處放的形式 沒有固定位置
                    if (JsonHM.ReadJsonFile(Path.Combine(EPath.ProjectPath, EPath.Name), out ProductProjectModel PProject))
                    {
                        //var a = PProject.PartsParameterObservableCollection;
                        ProductSettingVM.ProductProjectVMObservableCollection.Add(new ProductProjectViewModel(PProject));
                    }
                    else
                    {
                        //需註解找不到檔案!
                        ProductSettingVM.ProductProjectVMObservableCollection.Add(new ProductProjectViewModel(new ProductProjectModel()
                        {
                            ProjectPath = EPath.ProjectPath,
                            Name = EPath.Name,
                            FileIsNotExisted = true
                        })); ;
                    }
                });
            }

            TypeSettingSettingVM = new(new TypeSettingSettingModel()
            {
                ProductProjectVMObservableCollection = ProductSettingVM.ProductProjectVMObservableCollection,
                SeparateBoxVMObservableCollection = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection,

            });
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


            MachineMonitorVM = new MachineMonitorViewModel(new MachineMonitorModel()
            {
                ProjectDistributeVMObservableCollection = TypeSettingSettingVM.ProjectDistributeVMObservableCollection,
            });

            TypeSettingSettingVM.ChangeProjectDistributeCommand = MachineMonitorVM.ProjectDistributeVMChangeCommand;


            MachineFunctionVM = new MachineFunctionViewModel()
            {
                ParameterSettingVM = ParameterSettingVM,
                StampingFontChangedVM = StampingFontChangedVM,
                   //QRSettingModelCollection = ParameterSettingVM.QRSettingVM.QRSettingModelCollection,
                //StampingTypeVMObservableCollection = StampingFontChangedVM.StampingTypeVMObservableCollection,
                  // StampingTypeModel_ReadyStamping = StampingFontChangedVM.StampingTypeModel_ReadyStamping
            };





            Task.Run(() =>
            {
                while (true)
                {
                    OnPropertyChanged(nameof(DateTimeNow));
                    Thread.Sleep(100);
                }
            });

            Task.Run(() =>
            {
                Thread.Sleep(5000);
                while (true)
                {
                    //定期存檔
                    /*if (StampingFontChangedVM != null)
                    {
                        if (JsonHM.WriteMachineSettingJson(GD_JsonHelperMethod.MachineSettingNameEnum.StampingFont, StampingFontChangedVM))
                        {

                        }
                    }*/
                    try
                    {
                        var Model_IEnumerable = TypeSettingSettingVM.ProjectDistributeVMObservableCollection.Select(x => x.ProjectDistribute).ToList();
                        //定期存檔

                        JsonHM.WriteProjectDistributeListJson(Model_IEnumerable);

                        AddLogData("SaveProjectDistributeListFile");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Debugger.Break();
                    }

                    try
                    {
                        if (JsonHM.WriteMachineSettingJson(GD_JsonHelperMethod.MachineSettingNameEnum.StampingFont, StampingFontChangedVM))
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Debugger.Break();
                    }

                    Thread.Sleep(3000);
                }
            });


        }

        //private DateTime _dateTimeNow = new DateTime();
        [JsonIgnore]
        public DateTime DateTimeNow
        {
            get => DateTime.Now;
        }
     
        [JsonIgnore]
        public RelayCommand OpenProjectFileCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var fileContent = string.Empty;
                    var filePath = string.Empty;

                    using (var openFileDialog = new System.Windows.Forms.OpenFileDialog())
                    {
                        openFileDialog.InitialDirectory = "c:\\";
                        openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                        openFileDialog.FilterIndex = 2;
                        openFileDialog.RestoreDirectory = true;

                        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            //Get the path of specified file
                            filePath = openFileDialog.FileName;

                            //Read the contents of the file into a stream
                            var fileStream = openFileDialog.OpenFile();

                            using (StreamReader reader = new StreamReader(fileStream))
                            {
                                fileContent = reader.ReadToEnd();
                            }
                        }
                    }
                });
            }
        }


        private bool _isBrightMode = false;
        public bool IsBrightMode 
        {get => _isBrightMode;
            set
            {
                _isBrightMode = value;
                if (_isBrightMode)
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



                OnPropertyChanged();
            }
        } 






        #region VM
        /// <summary>
        /// 關於本機
        /// </summary>
        public MachanicalSpecificationViewModel MachanicalSpecificationVM { get => StampingMain.MachanicalSpecificationVM; set => StampingMain.MachanicalSpecificationVM = value; }
        /// <summary>
        /// 字模設定
        /// </summary>
        public StampingFontChangedViewModel StampingFontChangedVM { get => StampingMain.StampingFontChangedVM; set => StampingMain.StampingFontChangedVM = value; }
        /// <summary>
        /// 參數設定
        /// </summary>
        public ParameterSettingViewModel ParameterSettingVM { get => StampingMain.ParameterSettingVM; set => StampingMain.ParameterSettingVM = value; }
        /// <summary>
        /// 製品設定
        /// </summary>
        public ProductSettingViewModel ProductSettingVM { get => StampingMain.ProductSettingVM; set => StampingMain.ProductSettingVM = value; }
        /// <summary>
        /// 排版設定
        /// </summary>
        public TypeSettingSettingViewModel TypeSettingSettingVM { get => StampingMain.TypeSettingSettingVM; set => StampingMain.TypeSettingSettingVM = value; }

        /// <summary>
        /// 加工監控
        /// </summary>
        public MachineMonitorViewModel MachineMonitorVM { get => StampingMain.MachineMonitorVM; set => StampingMain.MachineMonitorVM = value; }

        /// <summary>
        /// 機台功能
        /// </summary>
        public MachineFunctionViewModel MachineFunctionVM { get => StampingMain.MachineFunctionVM; set => StampingMain.MachineFunctionVM = value; }



        #endregion


        [JsonIgnore]
        public ICommand DownloadAndUpdatedCommand
        {
            get => new RelayParameterizedCommand(Parameter =>
            {
                if (Parameter is Button)
                {
                    (Parameter as Button).IsEnabled = false;
                }


                var ManagerVM = new DXSplashScreenViewModel
                {
                    Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Image/svg/NewLogo_1-2.svg"),
                    Status = (string)System.Windows.Application.Current.TryFindResource("Text_Loading"),
                    Progress = 0,
                    IsIndeterminate = false,
                    Subtitle = "Alpha 23.4.24",
                    Copyright = "Copyright © 2022 GUANDA",
                };

                SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.ProcessingScreenWindow(), ManagerVM);
                manager.Show(null, WindowStartupLocation.CenterScreen, true, InputBlockMode.Window);
                ManagerVM.IsIndeterminate = false;

                //等待結束 
                Task.Run(() =>
                {
                    try
                    {
                        for (double i = 10000; i < 0; i--)
                        {
                            ManagerVM.Progress = i / 10000;
                        }
                        Thread.Sleep(100);




                        System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            manager.Close();
                            if (Parameter is Button)
                            {
                                (Parameter as Button).IsEnabled = true;
                            }
                        });
                    }
                    catch (Exception ex)
                    {

                    }
                });




            });
        }


        private bool IsAlert
        {
            get;
            set;
        }
        



    }




}
