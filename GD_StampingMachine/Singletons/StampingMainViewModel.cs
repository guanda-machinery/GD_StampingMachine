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
using GD_StampingMachine.Singletons;


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
            Dispose(disposing: false);
        }
        private bool disposedValue;
   
        protected override void Dispose(bool disposing)
        {

            if (!disposedValue)
            {
                if (disposing)
                {
                   _ = SaveStampingMachineJson();
                    // TODO: 處置受控狀態 (受控物件)
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }


        public async Task SaveStampingMachineJson()
        {
            if (ParameterSettingVM.AxisSettingVM.AxisSetting != null)
                await JsonHM.WriteParameterSettingJsonSettingAsync(StampingMachineJsonHelper.ParameterSettingNameEnum.AxisSetting, ParameterSettingVM.AxisSettingVM.AxisSetting);

            if (ParameterSettingVM.TimingSettingVM.TimingSetting != null)
                await JsonHM.WriteParameterSettingJsonSettingAsync(StampingMachineJsonHelper.ParameterSettingNameEnum.TimingSetting, ParameterSettingVM.TimingSettingVM.TimingSetting);

            if (ParameterSettingVM.EngineerSettingVM.EngineerSetting != null)
                await JsonHM.WriteParameterSettingJsonSettingAsync(StampingMachineJsonHelper.ParameterSettingNameEnum.EngineerSetting, ParameterSettingVM.EngineerSettingVM.EngineerSetting);

            if (ParameterSettingVM.SeparateSettingVM.SeparateSetting != null)
                await JsonHM.WriteParameterSettingJsonSettingAsync(StampingMachineJsonHelper.ParameterSettingNameEnum.SeparateSetting, ParameterSettingVM.SeparateSettingVM.SeparateSetting);

            try
            {
             

                List<StampingTypeModel> UseStampingFont = StampingFontChangedVM.StampingTypeVMObservableCollection
                    .Select(x => x.StampingType).ToList();

                if (await JsonHM.WriteUseStampingFontAsync(UseStampingFont))
                {

                }

                List<StampingTypeModel> UnuseStampingFont = StampingFontChangedVM.UnusedStampingTypeVMObservableCollection
                    .Select(x => x.StampingType).ToList();

                if (await JsonHM.WriteUnUseStampingFontAsync(UnuseStampingFont))
                {

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Debugger.Break();
            }
        }


        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingMainViewModel");

        public StampingMainViewModel()
        {
            //測試模式
            if (Debugger.IsAttached)
            {
                _ = Task.Run(async () =>
                {
                    for (int ErrorCount = 0; true; ErrorCount++)
                    {
                        _ = Singletons.LogDataSingleton.Instance.AddLogDataAsync("Debug", $"TestMessage-{ErrorCount}", ErrorCount % 5 == 0);
                        // Thread.Sleep(1000);
                        await Task.Delay(1000);
                    }
                });
            }

            StampingMachineJsonHelper JsonHM = new();

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    DateTimeNow = DateTime.Now;
                    await Task.Delay(1000);
                }
            });

            _ = Task.Run(async () =>
            {
                await Task.Delay(10000);
                //Thread.Sleep(5000);
                while (true)
                {
                    await SaveStampingMachineJson();
                    await Task.Delay(60000);
                }
            });




        }

        private DateTime _dateTimeNow = new DateTime();
        [JsonIgnore]
        public DateTime DateTimeNow
        {
            get => _dateTimeNow; set { _dateTimeNow = value; OnPropertyChanged(); }
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
            set => stampingMain.ParameterSettingVM = value;
        }
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
       /* public DXObservableCollection<OperatingLogViewModel> LogDataObservableCollection
        {
            get
            {
                return Singletons.LogDataSingleton.Instance.DataObservableCollection;
            }
        }*/

        #endregion



        private AsyncRelayCommand _downloadAndUpdatedCommand;
        [JsonIgnore]
        public AsyncRelayCommand DownloadAndUpdatedCommand
        {
            get => _downloadAndUpdatedCommand ??= new(async () =>
            {
                var ManagerVM = new DXSplashScreenViewModel
                {
                    Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Image/svg/NewLogo_1-2.svg"),
                    Status = (string)System.Windows.Application.Current.TryFindResource("Text_Loading"),
                    Progress = 0,
                    IsIndeterminate = false,
                    Subtitle = Properties.Settings.Default.Version,
                    Copyright = Properties.Settings.Default.Copyright,
                };
                SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.ProcessingScreenWindow(), ManagerVM);
                manager.Show(Application.Current.MainWindow, WindowStartupLocation.CenterScreen, true, InputBlockMode.None);
                ManagerVM.IsIndeterminate = false;
                //等待結束 
                for (double i = 10000; i < 0; i--)
                {
                    ManagerVM.Progress = i / 100;
                    await Task.Delay(100);
                }

                manager.Close();

            }, () => !DownloadAndUpdatedCommand.IsRunning);
        }


        public RelayCommand JumpToEngineeringModeCommand
        {
            get=> new(()=>
            {
                StampingMachineSingleton.Instance.ParameterSettingVM.TbtnSeEngineeringModeIsChecked = true;
            });
        }





    }




}
