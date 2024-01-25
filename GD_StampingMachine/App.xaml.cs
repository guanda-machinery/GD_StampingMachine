using DevExpress.Mvvm;
using GD_CommonLibrary.Method;
using GD_StampingMachine.Properties;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels;
using Microsoft.VisualStudio.Threading;
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
                Debugger.Break();
                MessageBoxResultShow.Show((string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_ProgramisAlreadyOpen"), MessageBoxButton.OK,  GD_Enum.GD_MessageBoxNotifyResult.NotifyYe);
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
            StampingMachineWindow MachineWindow = new StampingMachineWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            StampingMachineWindowViewModel stampingMachineWindowVM = new StampingMachineWindowViewModel
            {
                Opacity = 0,
                IsEnabled = false,
            };
            MachineWindow.DataContext = stampingMachineWindowVM;
            MachineWindow.Show();
            DevExpress.Xpf.Core.SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.StartSplashScreen(), ManagerVM);

            manager.Show(Current.MainWindow, WindowStartupLocation.CenterOwner, true, DevExpress.Xpf.Core.InputBlockMode.Window);

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Yield();
                    await Task.Delay(1000);

                    //manager.Show(null, WindowStartupLocation.CenterScreen, true, InputBlockMode.Window);
                    ManagerVM.IsIndeterminate = true;
                    await Task.Delay(100);

                    for (int i = 0; i <= 100; i++)
                    {
                        ManagerVM.Progress = i / 1.0;
                        stampingMachineWindowVM.Opacity = i / 100.0;
                        await Task.Delay(1);
                    }
                    await Task.Delay(1000);

                    ManagerVM.Title = (string)System.Windows.Application.Current.TryFindResource("Text_Starting");
                    //stampingMachineWindowVM.Visibility = Visibility.Visible;
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
            SemaphoreSlim semaphore = new SemaphoreSlim(0);
            var DisconnectTask = Task.Run(async () =>
            {
                await StampMachineDataSingleton.Instance.StopScanOpcuaAsync();
                semaphore.Release();
            });

            semaphore.Wait(5000);
            base.OnExit(e);
        }



        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(0);
            if (e.ExceptionObject is Exception exception)
            {
                _ = Task.Run(async () =>
                {
                    //紀錄異常
                    await LogDataSingleton.Instance.AddLogDataAsync(exception.Source, exception.Message, true);
                    //切斷機台連線
                    await StampMachineDataSingleton.Instance.DisposeAsync();
                    await MessageBoxResultShow.ShowOKAsync(exception.Source, exception.Message , GD_Enum.GD_MessageBoxNotifyResult.NotifyRd);
                    semaphore.Release();
                });
                //顯示彈窗
                MessageBox.Show($"An unhandled exception occurred: {exception.Message} , Source:{exception.Source}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                semaphore.Wait();
            }
        }




    }
}
