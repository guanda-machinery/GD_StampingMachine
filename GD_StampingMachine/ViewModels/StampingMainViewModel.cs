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
using DevExpress.Xpf.Editors;
using Newtonsoft.Json;

namespace GD_StampingMachine.ViewModels
{
    public class StampingMainModel
    {
        public MachanicalSpecificationViewModel MachanicalSpecificationVM { get; set; } 
        public StampingFontChangedViewModel StampingFontChangedVM { get; set; }
        public ParameterSettingViewModel ParameterSettingVM { get; set; }
        public ProductSettingViewModel ProductSettingVM { get; set; }
        public TypeSettingSettingViewModel TypeSettingSettingVM { get; set; }
        public MachiningSettingViewModel MachiningSettingVM { get; set; }
    }

    public partial class StampingMainViewModel : BaseViewModelWithLog
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingMainViewModel");

        private StampingMainModel StampingMain = new();

        public StampingMainViewModel()
        {
            StampingMain = new StampingMainModel();

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

            /*if (JsonHM.ReadStampingAllData(out var JsonStampingMain))
            {
                StampingMain = JsonStampingMain;
                TypeSettingSettingVM.TypeSettingSetting.ProductProjectVMObservableCollection = ProductSettingVM.ProductProjectVMObservableCollection;
                TypeSettingSettingVM.TypeSettingSetting.SeparateBoxVMObservableCollection = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection;
                MachiningSettingVM.MachiningSetting.ProjectDistributeVMObservableCollection = TypeSettingSettingVM.ProjectDistributeVMObservableCollection;
            }
            else*/
            {
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
                        StampingFontChangedVM.StampingTypeVMObservableCollection.Add(new StampingTypeViewModel(new StampingTypeModel() { StampingTypeNumber = i, StampingTypeString = "1", StampingTypeUseCount = 0 }));
                    }
                    StampingFontChangedVM.UnusedStampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>()
                {
                    new StampingTypeViewModel(new StampingTypeModel(){ StampingTypeNumber =0 , StampingTypeString = "ㄅ" , StampingTypeUseCount=0}) ,
                    new StampingTypeViewModel(new StampingTypeModel(){ StampingTypeNumber =0 , StampingTypeString = "ㄆ" , StampingTypeUseCount=0}),
                    new StampingTypeViewModel(new StampingTypeModel(){ StampingTypeNumber =0, StampingTypeString = "ㄇ" , StampingTypeUseCount=0})
                };

                    //LogDataObservableCollection = this.LogDataObservableCollection
                }
                ParameterSettingVM = new()
                {
                    //LogDataObservableCollection = this.LogDataObservableCollection
                };
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
                            ProductSettingVM.ProductProjectVMObservableCollection.Add(new ProductProjectViewModel(PProject));
                        }
                        else
                        {
                            //需註解找不到檔案!
                            ProductSettingVM.ProductProjectVMObservableCollection.Add(new ProductProjectViewModel(new ProductProjectModel()));
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
                        PDistribute.ProductProjectVMObservableCollection = ProductSettingVM.ProductProjectVMObservableCollection;
                        PDistribute.SeparateBoxVMObservableCollection = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection;
                    TypeSettingSettingVM.ProjectDistributeVMObservableCollection.Add(new ProjectDistributeViewModel(PDistribute)
                    { 
                        IsInDistributePage = false
                        //重新繫結
                    });
                });
                }


                MachiningSettingVM = new MachiningSettingViewModel(new MachiningSettingModel()
                {
                    ProjectDistributeVMObservableCollection = TypeSettingSettingVM.ProjectDistributeVMObservableCollection,
                });
            }



            Task.Run(() =>
            {
                while (true)
                {
                    DateTimeNow = DateTime.Now;
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
                    }
                    catch(Exception ex)
                    {
                        Debugger.Break();
                    }
                    Thread.Sleep(3000);
                }
            });


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
        public MachanicalSpecificationViewModel MachanicalSpecificationVM { get => StampingMain.MachanicalSpecificationVM; set => StampingMain.MachanicalSpecificationVM = value; }
        public StampingFontChangedViewModel StampingFontChangedVM { get => StampingMain.StampingFontChangedVM; set => StampingMain.StampingFontChangedVM = value; }
        public ParameterSettingViewModel ParameterSettingVM { get => StampingMain.ParameterSettingVM; set => StampingMain.ParameterSettingVM = value; }
        public ProductSettingViewModel ProductSettingVM { get => StampingMain.ProductSettingVM; set => StampingMain.ProductSettingVM = value; }
        public TypeSettingSettingViewModel TypeSettingSettingVM { get => StampingMain.TypeSettingSettingVM; set => StampingMain.TypeSettingSettingVM = value; }

        /// <summary>
        /// 加工設定
        /// </summary>
        public MachiningSettingViewModel MachiningSettingVM { get => StampingMain.MachiningSettingVM; set => StampingMain.MachiningSettingVM = value; }

        #endregion
        [JsonIgnore]
        public ICommand ReloadTypeSettingSettingsCommand
        {
            get => new RelayCommand(() =>
            {
                if(TypeSettingSettingVM.ProjectDistributeVM!=null)
                    TypeSettingSettingVM.ProjectDistributeVM.PartsParameterVMObservableCollectionRefresh();
            });
        }
        [JsonIgnore]
        public ICommand ReloadMachiningSettingsCommand
        {
            get
            {
                 return MachiningSettingVM.GridControlRefreshCommand;
            }
        }
    }




}
