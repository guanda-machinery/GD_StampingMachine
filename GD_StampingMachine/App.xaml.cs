using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using GD_CommonLibrary.Method;
using GD_StampingMachine.Properties;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels;
using System;
using System.Diagnostics;
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


        protected override void OnStartup(StartupEventArgs e)
        {
            

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            base.OnStartup(e);
            var mutex = new System.Threading.Mutex(true, System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName, out bool ret);
            if (!ret)
            {
                MessageBoxResultShow.Show((string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_ProgramisAlreadyOpen"), MessageBoxButton.OK, GD_Enum.GD_MessageBoxNotifyResult.NotifyYe);
                Environment.Exit(0);
            }

            //啟用掃描
            var ManagerVM = new DXSplashScreenViewModel
            {
                Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Resources/svg/stmLight.svg"),
                Title = "GD_StampingMachine",
                Status = (string)System.Windows.Application.Current.TryFindResource("Text_Loading"),
                Progress = 0,
                IsIndeterminate = false,
                Subtitle = "Alpha 24.1.29",
                Copyright = "Copyright © 2024 GUANDA",
            };


            StampingMachineWindow MachineWindow = new()
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            MachineWindow.DataContext ??= new StampingMachineWindowViewModel()
            {
                Opacity = 0,
                IsEnabled = false,
            };
            var stampingMachineWindowVM = (StampingMachineWindowViewModel)MachineWindow.DataContext;
            MachineWindow.Show();

            MachineWindow.WindowState = GD_StampingMachine.Properties.Settings.Default.WindowState;

            DevExpress.Xpf.Core.SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.StartSplashScreen(), ManagerVM);
           // manager.Show(Current.MainWindow, WindowStartupLocation.CenterOwner, true, DevExpress.Xpf.Core.InputBlockMode.Window); 
            manager.Show(null, WindowStartupLocation.CenterOwner, true, DevExpress.Xpf.Core.InputBlockMode.Window);

            _ = Task.Run(async () =>
            {
                try
                {
                    stampingMachineWindowVM.Opacity = 0.01;
                    ManagerVM.IsIndeterminate = true;
                   /* var monitorTask = Task.Run(async () =>
                    {
                        stampingMachineWindowVM.StampingMainVM.TBtn_MachineMonitorIsChecked = true;
                        await Task.Delay(1000);
                        stampingMachineWindowVM.StampingMainVM.TBtn_MachineMonitorIsChecked = false;
                    });
                    await Task.Delay(2000);*/

                    ManagerVM.IsIndeterminate = false;

                    for (int i = 0; i <= 100; i++)
                    {
                        ManagerVM.Progress = i / 1.0;
                        stampingMachineWindowVM.Opacity = i / 100.0;

                        if(i>20)
                        {
                            //await monitorTask; ;
                        }

                        await Task.Delay(1);
                    }
                    await Task.Delay(1000);

                    ManagerVM.Title = (string)System.Windows.Application.Current.TryFindResource("Text_Starting");

                    await Task.Delay(1000);
                    stampingMachineWindowVM.IsEnabled = true;
                    manager.Close();

                    try
                    {
                        if (Settings.Default.ConnectOnStartUp)
                            await Singletons.StampMachineDataSingleton.Instance.StartScanOpcuaAsync();
                    }
                    catch
                    {

                    }


                }
                catch (Exception ex)
                {
                    _ = MessageBoxResultShow.ShowExceptionAsync(ex);
                    System.Diagnostics.Debugger.Break();
                }
            });


            /*
            var resourceDictionary = CulturesHelper.LoadResourceDictionary(new CultureInfo("zh-tw"));

            Dictionary<string, string> dictionary = resourceDictionary
                .Cast<DictionaryEntry>()
                .ToDictionary(entry => entry.Key.ToString(), entry => entry.Value.ToString());

            var list = dictionary.Keys.ToList();
            list.Sort();

            List<Tuple<int, string, string>> dict = new();
            int i = 1;
            foreach(var key in list)
            {
                dict.Add(new(i, key.ToString(), dictionary[key].ToString()));
                    i++;
            }

            new CsvFileManager().WriteCSVFileIEnumerable(@"C:\Users\USER\Desktop\cult.csv",dict);
            */
        }








        protected override void OnExit(ExitEventArgs e)
        {
            SemaphoreSlim semaphore = new(0);

            StampMachineDataSingleton.Instance.Disconnect();
            var DisconnectTask = Task.Run(async () =>
            {
                await StampMachineDataSingleton.Instance.StopScanOpcuaAsync();
                semaphore.Release();
            });

            semaphore.Wait(10000);
            base.OnExit(e);
        }



        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            SemaphoreSlim semaphore = new(0);
            if (e.ExceptionObject is Exception exception)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        //紀錄異常
                        await LogDataSingleton.Instance.AddLogDataAsync(exception.Source, exception.Message, true);
                        //切斷機台連線
                        await StampMachineDataSingleton.Instance.DisposeAsync();
                        await MessageBoxResultShow.ShowOKAsync(exception.Source, exception.Message, GD_Enum.GD_MessageBoxNotifyResult.NotifyRd);
                    }
                    catch
                    {

                    }
                    finally
                    {
                        semaphore.Release();
                    }

                });
                semaphore.Wait();
                //顯示彈窗
                MessageBox.Show($"An unhandled exception occurred: {exception.Message} , Source:{exception.Source}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }




    }
}
