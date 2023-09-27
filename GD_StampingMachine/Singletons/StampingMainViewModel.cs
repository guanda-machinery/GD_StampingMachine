using DevExpress.Mvvm.Native;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GD_CommonLibrary;
using Newtonsoft.Json;
using GD_CommonLibrary.Extensions;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Mvvm;
using System.Windows.Controls;
using System.Windows.Media;
using GD_CommonLibrary.Method;
using CommunityToolkit.Mvvm.Input;

namespace GD_StampingMachine.ViewModels
{


    public partial class StampingMainViewModel : GD_CommonLibrary.BaseViewModel
    {
        /// <summary>
        /// 解構
        /// </summary>

        StampingMachineJsonHelper JsonHM = new();

        ~StampingMainViewModel()
        {
            if (ParameterSettingVM.AxisSettingVM.AxisSetting != null)
                JsonHM.WriteParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.AxisSetting, ParameterSettingVM.AxisSettingVM.AxisSetting);

            if (ParameterSettingVM.TimingSettingVM.TimingSetting != null)
                JsonHM.WriteParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.TimingSetting, ParameterSettingVM.TimingSettingVM.TimingSetting);

            if (ParameterSettingVM.EngineerSettingVM.EngineerSetting != null)
                JsonHM.WriteParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.EngineerSetting, ParameterSettingVM.EngineerSettingVM.EngineerSetting);

            if (ParameterSettingVM.SeparateSettingVM.SeparateSetting != null)
                JsonHM.WriteParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.SeparateSetting, ParameterSettingVM.SeparateSettingVM.SeparateSetting);
        }

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingMainViewModel");

        public StampingMainViewModel()
        {
            //測試模式
            if (Debugger.IsAttached)
            {
                Task.Run(async () =>
                {
                    for (int ErrorCount = 0; true; ErrorCount++)
                    {
                        Singletons.LogDataSingleton.Instance.AddLogData("Debug", $"TestMessage-{ErrorCount}", ErrorCount % 5 == 0);
                       // Thread.Sleep(1000);
                        await Task.Delay(1000);
                    }
                });
            }

            StampingMachineJsonHelper JsonHM = new();

            Task.Run(async() =>
            {
                while (true)
                {
                    DateTimeNow = DateTime.Now;
                    await Task.Delay(100);
                }
            });

            Task.Run(async() =>
            {

                await Task.Delay(5000);
                //Thread.Sleep(5000);
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

                        Singletons.LogDataSingleton.Instance.AddLogData(this.ViewModelName , "SaveProjectDistributeListFile");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Debugger.Break();
                    }

                    try
                    {
                        if (JsonHM.WriteMachineSettingJson(StampingMachineJsonHelper.MachineSettingNameEnum.UseStampingFont, StampingFontChangedVM.StampingTypeVMObservableCollection))
                        {

                        }
                        if (JsonHM.WriteMachineSettingJson(StampingMachineJsonHelper.MachineSettingNameEnum.UnUseStampingFont, StampingFontChangedVM.UnusedStampingTypeVMObservableCollection))
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Debugger.Break();
                    }

                    await Task.Delay(5000);
                }
            });




        }

        private DateTime _dateTimeNow = new DateTime();
        [JsonIgnore]
        public DateTime DateTimeNow
        {
            get => _dateTimeNow; set { _dateTimeNow =value; OnPropertyChanged(); } 
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









        #region VM
        private Singletons.StampingMachineSingleton stampingMain = Singletons.StampingMachineSingleton.Instance;
        /// <summary>
        /// 關於本機
        /// </summary>
        public MachanicalSpecificationViewModel MachanicalSpecificationVM { get => stampingMain.MachanicalSpecificationVM; set => stampingMain.MachanicalSpecificationVM = value; }
        /// <summary>
        /// 字模設定
        /// </summary>
        public StampingFontChangedViewModel StampingFontChangedVM { get => stampingMain.StampingFontChangedVM; set => stampingMain.StampingFontChangedVM = value; }
        /// <summary>
        /// 參數設定
        /// </summary>
        public ParameterSettingViewModel ParameterSettingVM
        {
            get => stampingMain.ParameterSettingVM; 
            set => stampingMain.ParameterSettingVM = value; }
        /// <summary>
        /// 製品設定
        /// </summary>
        public ProductSettingViewModel ProductSettingVM { get => stampingMain.ProductSettingVM; set => stampingMain.ProductSettingVM = value; }
        /// <summary>
        /// 排版設定
        /// </summary>
        public TypeSettingSettingViewModel TypeSettingSettingVM { get => stampingMain.TypeSettingSettingVM; set => stampingMain.TypeSettingSettingVM = value; }

        /// <summary>
        /// 加工監控
        /// </summary>
        public MachineMonitorViewModel MachineMonitorVM { get => stampingMain.MachineMonitorVM; set => stampingMain.MachineMonitorVM = value; }

        /// <summary>
        /// 機台功能
        /// </summary>
        public MachineFunctionViewModel MachineFunctionVM { get => stampingMain.MachineFunctionVM; set => stampingMain.MachineFunctionVM = value; }

        private MachineFunctionTestViewModel _machineFunctionTestVM;
        public MachineFunctionTestViewModel MachineFunctionTestVM { get => _machineFunctionTestVM ??= new MachineFunctionTestViewModel(); set => _machineFunctionTestVM = value; }
        

        /// <summary>
        /// 機台警報
        /// </summary>
        public DXObservableCollection<OperatingLogViewModel> LogDataObservableCollection
        {
            get
            {
                return Singletons.LogDataSingleton.Instance.DataObservableCollection;
            }
        }



        #endregion


        [JsonIgnore]
        public ICommand DownloadAndUpdatedCommand
        {
            get => new RelayCommand<object>(Parameter =>
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

        



    }




}
