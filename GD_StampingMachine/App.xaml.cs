using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
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
        private void Application_Startup(object sender, StartupEventArgs e)
        {
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
                        ManagerVM.Progress = i ;
                        await Task.Delay(20);
                    }

                    ManagerVM.Title = (string)System.Windows.Application.Current.TryFindResource("Text_Starting");

                    await Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                    {
                        MachineWindow.IsEnabled = true;
                    }));
                    manager.Close();


                    await Task.Run(async () =>
                    {
                        await Task.Delay(1000);
                        await Singletons.StampMachineDataSingleton.Instance.StartScanOpcua();
                        //檢查字模
                        while (!Singletons.StampMachineDataSingleton.Instance.IsConnected)
                        {
                            await Task.Delay(100);
                        }

                        await Task.Delay(1000);
                        while(Singletons.StampMachineDataSingleton.Instance.RotatingTurntableInfoCollection.Count == 0)
                        {
                            await Task.Delay(100);
                        }
                        await Singletons.StampMachineDataSingleton.Instance.CompareFontsSettingBetweenMachineAndSoftware();






                    });
                    //等待連線 並檢查字模是否設定正確->
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debugger.Break();
                }
            });
        }
    }
}
