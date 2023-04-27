using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.CommonDialogs;
using DevExpress.Utils.CommonDialogs.Internal;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
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

namespace GD_StampingMachine.ViewModels
{
    //[GenerateViewModel]
    public partial class StampingMainViewModel : BaseViewModelWithLog
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingMainViewModel");

        public StampingMainViewModel()
        {
            Task.Run(() =>
             {
                 while (true)
                 {
                     DateTimeNow = DateTime.Now;
                     Thread.Sleep(100);
                 }
             });


            //SynchronizationContext _SynchronizationContext = SynchronizationContext.Current;

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
                        DateTimeNow = DateTime.Now;
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
                //LogDataObservableCollection = this.LogDataObservableCollection
            };


            if (JsonHM.ReadStampingFontChanged(out var SReadSFC))
            {
                StampingFontChangedVM = SReadSFC;
            }
            else
            {
                StampingFontChangedVM = new StampingFontChangedViewModel
                {
                    StampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>()
            {
                    new StampingTypeViewModel(){ StampingTypeNumber =1 , StampingTypeString = "1" , StampingTypeUseCount=25 } ,
                    new StampingTypeViewModel(){ StampingTypeNumber =2 , StampingTypeString = "2" , StampingTypeUseCount=180},
                    new StampingTypeViewModel(){ StampingTypeNumber =3, StampingTypeString = "3" , StampingTypeUseCount=25},
                    new StampingTypeViewModel(){ StampingTypeNumber =4, StampingTypeString = "4" , StampingTypeUseCount=25},
                    new StampingTypeViewModel(){ StampingTypeNumber =5, StampingTypeString = "5" , StampingTypeUseCount=25},
                    new StampingTypeViewModel(){ StampingTypeNumber =6, StampingTypeString = "6" , StampingTypeUseCount=25},
                    new StampingTypeViewModel(){ StampingTypeNumber =7, StampingTypeString = "7" , StampingTypeUseCount=25},
                    new StampingTypeViewModel(){ StampingTypeNumber =8, StampingTypeString = "8" , StampingTypeUseCount=25},

                    new StampingTypeViewModel(){ StampingTypeNumber =9, StampingTypeString = "9" , StampingTypeUseCount=25},

                    new StampingTypeViewModel(){ StampingTypeNumber =10, StampingTypeString = "0" , StampingTypeUseCount=25},

                    new StampingTypeViewModel(){ StampingTypeNumber =11, StampingTypeString = "A" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =12, StampingTypeString = "B" , StampingTypeUseCount=8677},

                    new StampingTypeViewModel(){ StampingTypeNumber =13, StampingTypeString = "C" , StampingTypeUseCount=7025},

                    new StampingTypeViewModel(){ StampingTypeNumber =14, StampingTypeString = "D" , StampingTypeUseCount=3015},

                    new StampingTypeViewModel(){ StampingTypeNumber =15, StampingTypeString = "E" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =16, StampingTypeString = "F" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =17, StampingTypeString = "G" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =18, StampingTypeString = "H" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =19, StampingTypeString = "I" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =20, StampingTypeString = "J" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =21, StampingTypeString = "K" , StampingTypeUseCount=5071 },

                    new StampingTypeViewModel(){ StampingTypeNumber =22, StampingTypeString = "L" , StampingTypeUseCount=1562},

                    new StampingTypeViewModel(){ StampingTypeNumber =23, StampingTypeString = "M" , StampingTypeUseCount=71},

                    new StampingTypeViewModel(){ StampingTypeNumber =24, StampingTypeString = "N" , StampingTypeUseCount=9071},

                    new StampingTypeViewModel(){ StampingTypeNumber =25, StampingTypeString = "O" , StampingTypeUseCount=5071},

                    new StampingTypeViewModel(){ StampingTypeNumber =26, StampingTypeString = "P" , StampingTypeUseCount=5071},

                    new StampingTypeViewModel(){ StampingTypeNumber =27, StampingTypeString = "Q" , StampingTypeUseCount=5071},

                    new StampingTypeViewModel(){ StampingTypeNumber =28, StampingTypeString = "R" , StampingTypeUseCount=5071},

                    new StampingTypeViewModel(){ StampingTypeNumber =29, StampingTypeString = "S" , StampingTypeUseCount=5071},

                    new StampingTypeViewModel(){ StampingTypeNumber =30, StampingTypeString = "T" , StampingTypeUseCount=5071},

                    new StampingTypeViewModel(){ StampingTypeNumber =31, StampingTypeString = "U" , StampingTypeUseCount=50},

                    new StampingTypeViewModel(){ StampingTypeNumber =32, StampingTypeString = "V" , StampingTypeUseCount=110},

                    new StampingTypeViewModel(){ StampingTypeNumber =33, StampingTypeString = "W" , StampingTypeUseCount=550},

                    new StampingTypeViewModel(){ StampingTypeNumber =34, StampingTypeString = "X" , StampingTypeUseCount=24},

                    new StampingTypeViewModel(){ StampingTypeNumber =35, StampingTypeString = "Y" , StampingTypeUseCount=5},
                    new StampingTypeViewModel(){ StampingTypeNumber =36, StampingTypeString = "Z" , StampingTypeUseCount=5},
                    new StampingTypeViewModel(){ StampingTypeNumber =37, StampingTypeString = "a" , StampingTypeUseCount=450},
                    new StampingTypeViewModel(){ StampingTypeNumber =38, StampingTypeString = "b" , StampingTypeUseCount=677},
                    new StampingTypeViewModel(){ StampingTypeNumber =39, StampingTypeString = "g" , StampingTypeUseCount=150},
                    new StampingTypeViewModel(){ StampingTypeNumber =40, StampingTypeString = "-" , StampingTypeUseCount=2550}
             },
                    UnusedStampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>()
            {
                    new StampingTypeViewModel(){ StampingTypeNumber =0 , StampingTypeString = "ㄅ" , StampingTypeUseCount=0} ,
                    new StampingTypeViewModel(){ StampingTypeNumber =0 , StampingTypeString = "ㄆ" , StampingTypeUseCount=0},
                    new StampingTypeViewModel(){ StampingTypeNumber =0, StampingTypeString = "ㄇ" , StampingTypeUseCount=0}
            },
                    //LogDataObservableCollection = this.LogDataObservableCollection
                }; 
            }

            ParameterSettingVM = new()
            {
                //LogDataObservableCollection = this.LogDataObservableCollection
            };

            ProductSettingVM = new()
            {
                ProductProjectVMObservableCollection = new ObservableCollection<ProductProjectViewModel>()
                {
                    new(new()
                    {
                        Name = "創典科技總公司基地",
                        ProjectPath="C:/",
                        Number = "AS001",
                        SheetStampingTypeForm = GD_Enum.SheetStampingTypeFormEnum.QRSheetStamping,
                        CreateTime = new DateTime(2022, 10, 27, 14, 02, 00),
                        EditTime = DateTime.Now,
                        FinishProgress = 10,
                    }),
                    new(new()
                    {                        ProjectPath="C:/",
                        Name = "創典科技總公司基地-1",
                        Number = "AS002",
                        SheetStampingTypeForm = GD_Enum.SheetStampingTypeFormEnum.QRSheetStamping,
                        CreateTime = new DateTime(2022, 10, 27, 14, 02, 00),
                        FinishProgress = 26
                    }),
                    new(new()
                    {                        ProjectPath="C:/",
                        Name = "創典科技總公司基地-3",
                        Number = "AS003",
                        SheetStampingTypeForm = GD_Enum.SheetStampingTypeFormEnum.QRSheetStamping,
                        CreateTime = new DateTime(2022, 10, 27, 14, 02, 00),
                        FinishProgress = 51
                    }),
                    new(new() {
                                                ProjectPath="C:/",
                        Name = "創典科技總公司基地-4",
                        Number = "AS003",
                        SheetStampingTypeForm = GD_Enum.SheetStampingTypeFormEnum.QRSheetStamping,
                        CreateTime = new DateTime(2022, 10, 27, 14, 02, 00),
                        FinishProgress = 76
                    })
                },
            };

            TypeSettingSettingVM = new(new TypeSettingSettingModel()
            {
                ProductProjectVMObservableCollection = ProductSettingVM.ProductProjectVMObservableCollection,
                SeparateBoxVMObservableCollection = ParameterSettingVM.SeparateSettingVM.UnifiedSetting_SeparateBoxModel,
            });

            MachiningSettingVM = new MachiningSettingViewModel(new MachiningSettingModel()
            {
                ProjectDistributeVMObservableCollection = TypeSettingSettingVM.ProjectDistributeVMObservableCollection,
            })
            {
                /*StampingBoxPartsVM = new StampingBoxPartsViewModel(new StampingBoxPartModel()
                {
                    ProjectDistributeVMObservableCollection = TypeSettingSettingVM.ProjectDistributeVMObservableCollection,
                    ProductProjectVMObservableCollection = ProductSettingVM.ProductProjectVMObservableCollection,
                    SeparateBoxVMObservableCollection = ParameterSettingVM.SeparateSettingVM.UnifiedSetting_SeparateBoxModel,
                    GridControl_MachiningStatusColumnVisible= true,
                })*/
            };
        }

        private DateTime _dateTimeNow = new DateTime();
        public DateTime DateTimeNow 
        { 
            get 
            {
                return _dateTimeNow;
            }
            set
            {
                _dateTimeNow = value;
                OnPropertyChanged(nameof(DateTimeNow));
            }
        }



        public RelayCommand OpenProjectFileCommand
        {
            get
            {
                return new RelayCommand(()=> 
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




        #region VM
        public MachanicalSpecificationViewModel MachanicalSpecificationVM { get; set; } 
        public StampingFontChangedViewModel StampingFontChangedVM { get; set; } 
        public ParameterSettingViewModel ParameterSettingVM { get; set; } 
        public ProductSettingViewModel ProductSettingVM { get; set; }
        public TypeSettingSettingViewModel TypeSettingSettingVM { get; set; }

        /// <summary>
        /// 加工設定
        /// </summary>
        public MachiningSettingViewModel MachiningSettingVM { get; set; }

        #endregion
        public ICommand ReloadTypeSettingSettingsCommand
        {
            get => new RelayCommand(() =>
            {
                if(TypeSettingSettingVM.ProjectDistributeVM!=null)
                    TypeSettingSettingVM.ProjectDistributeVM.PartsParameterVMObservableCollectionRefresh();
            });
        }
        public ICommand ReloadMachiningSettingsCommand
        {
            get
            {
                // return MachiningSettingVM.StampingBoxPartsVM.BoxPartsParameterVMObservableCollectionRefreshCommand;
                 return MachiningSettingVM.GridControlRefreshCommand;
            }
        }
    }




}
