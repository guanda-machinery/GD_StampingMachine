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
                    await Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                    {
                        manager.Show(null, WindowStartupLocation.CenterOwner, true, InputBlockMode.Window);
                    }));

                    //manager.Show(null, WindowStartupLocation.CenterScreen, true, InputBlockMode.Window);
                    ManagerVM.IsIndeterminate = true;


                    var SplashScreenIsShown = Task.Run(async () =>
                    {
                        while (manager.State != SplashScreenState.Shown)
                        {
                            await Task.Delay(100);
                        }
                    });


                    //如果五秒內都沒有出現彈窗 則不再等待
                    await Task.WhenAny(SplashScreenIsShown, Task.Delay(5000));

                    await Task.Delay(1000);
                    for (int i = 0; i <= 1000; i++)
                    {
                        ManagerVM.IsIndeterminate = false;
                        ManagerVM.Progress = i / 10;
                        await Task.Delay(2);
                    }

                    ManagerVM.Title = (string)System.Windows.Application.Current.TryFindResource("Text_Starting");


                    var ThreadOperTask = Task.Run(async () =>
                    {
                        await Dispatcher.InvokeAsync(new Action(async delegate
                        {
                            try
                            {
                                MachineWindow.Show();
                                MachineWindow.Topmost = true;
                                await Task.Delay(100);
                                MachineWindow.Topmost = false;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                Environment.Exit(0);
                            }
                        }));
                    });

                    //當等待最少三秒後才關閉視窗
                    await Task.Delay(3000);
                    await ThreadOperTask;
                    //await Task.WhenAll(ThreadOperTask, Task.Delay(3000));
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
