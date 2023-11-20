using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using GD_StampingMachine.Singletons;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GD_StampingMachine
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

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
            StampingMachineWindow MachineWindow = new StampingMachineWindow(); ;
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(100);
                    //  Thread.Sleep(100);
                    SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.StartSplashScreen(), ManagerVM);
                    await Application.Current.Dispatcher.InvokeAsync(new Action(async () =>
                    {
                        MachineWindow.Show();
                        MachineWindow.IsEnabled = false;

                        MachineWindow.Topmost = true;
                        await Task.Delay(100);
                        MachineWindow.Topmost = false;

                        manager.Show(Application.Current.MainWindow, WindowStartupLocation.CenterOwner, true, InputBlockMode.Window);
                    }));
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

                    await Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                    {
                        MachineWindow.IsEnabled = true;
                    }));
                    manager.Close();

                    await Task.Delay(1000);
                    await Singletons.StampMachineDataSingleton.Instance.StartScanOpcua();

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debugger.Break();
                }
            });
        }

        private async void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                //紀錄異常
                LogDataSingleton.Instance.AddLogData(nameof(App), exception.Message, true);
                
                //切斷機台連線
                await StampMachineDataSingleton.Instance.DisposeAsync();

                //顯示彈窗

                MessageBox.Show($"An unhandled exception occurred: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }
}
