
using DevExpress.Diagram.Core.Shapes;
using DevExpress.Mvvm;
using DevExpress.Xpf.WindowsUI;
using GD_CommonLibrary.Method;
using GD_StampingMachine.Method;
using GD_StampingMachine.Properties;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GD_StampingMachine
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override  void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

            var mutex = new System.Threading.Mutex(true, System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName, out bool ret);
            if (!ret)
            {
                Debugger.Break();
                 MessageBoxResultShow.Show((string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_ProgramisAlreadyOpen"), MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }


            //啟用掃描
            var ManagerVM = new DXSplashScreenViewModel
            {
                Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Image/svg/NewLogo_1-2.svg"),
                Title = "GD_StampingMachine",
                Status = (string)System.Windows.Application.Current.TryFindResource("Text_Loading"),
                Progress = 0,
                IsIndeterminate = false,
                Subtitle = "Alpha 23.7.4",
                Copyright = "Copyright © 2023 GUANDA",
            };
            StampingMachineWindow MachineWindow = new StampingMachineWindow();

            MachineWindow.Show();

            DevExpress.Xpf.Core.SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.StartSplashScreen(), ManagerVM);
            _ = Task.Run(async () =>
            {
                try
                {
                    await Application.Current?.Dispatcher.InvokeAsync(async () =>
                    {
                        await Task.Delay(100);
                        MachineWindow.IsEnabled = false;


                        MachineWindow.Topmost = true;
                        MachineWindow.Topmost = false;
                        manager.Show(Current.MainWindow, WindowStartupLocation.CenterOwner, true, DevExpress.Xpf.Core.InputBlockMode.Window);
                    });



                        if (Settings.Default.ConnectOnStartUp)
                        await Singletons.StampMachineDataSingleton.Instance.StartScanOpcuaAsync();
                    await Task.Yield();
                    await Task.Delay(1000);

                    //manager.Show(null, WindowStartupLocation.CenterScreen, true, InputBlockMode.Window);
                    ManagerVM.IsIndeterminate = true;

                    await Task.Delay(100);
                    ManagerVM.IsIndeterminate = false;
                    for (int i = 0; i <= 100; i++)
                    {
                        ManagerVM.Progress = i;
                        await Task.Delay(20);
                    }

                    ManagerVM.Title = (string)System.Windows.Application.Current.TryFindResource("Text_Starting");
 
                    await Application.Current?.Dispatcher.InvokeAsync( () =>
                    {
                        MachineWindow.Visibility = Visibility.Visible;
                            MachineWindow.IsEnabled = true;
                    });
                    manager.Close();
                }
                catch (Exception ex)
                {
                    _ = MessageBoxResultShow.ShowOKAsync("App", ex.Message);
                    System.Diagnostics.Debugger.Break();
                }
            });
        }

        private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
               var ExTask = Task.Run(async () =>
                {
                    //紀錄異常
                    await LogDataSingleton.Instance.AddLogDataAsync(nameof(App), exception.Message, true);
                    //切斷機台連線
                    await StampMachineDataSingleton.Instance.DisposeAsync();
                    await MessageBoxResultShow.ShowOKAsync(nameof(App), exception.Message);
                });
                //ExTask.Wait();

                //顯示彈窗
                MessageBox.Show($"An unhandled exception occurred: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ExTask.Wait();
            }
        }



        private void Application_Exit(object sender, ExitEventArgs e)
        {
            var JsonHM = new StampingMachineJsonHelper();

            // 開始一個 Task 來執行非同步操作
            var stoptask = Task.Run(async () => 
            { 
                await StampMachineDataSingleton.Instance.StopScanOpcuaAsync(); 
            } );


            //存檔
            var Model_IEnumerable = StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection.Select(x => x.ProjectDistribute).ToList();
            JsonHM.WriteProjectDistributeListJson(Model_IEnumerable);
            StampingMachineSingleton.Instance.ProductSettingVM.ProductProjectVMObservableCollection.Select(x => x.SaveProductProject());

            stoptask.Wait();
        }
    }
}
