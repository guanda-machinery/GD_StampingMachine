using DevExpress.CodeParser.Diagnostics;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GD_StampingMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Hide();



            //SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new SplashScreenWindows.ProcessingScreenWindow(), new DXSplashScreenViewModel { }); 

            var ManagerVM = new DXSplashScreenViewModel
            {
                Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Image/svg/NewLogo_1-2.svg"),
                Status = (string)System.Windows.Application.Current.TryFindResource("Text_Loading"),
                Progress = 0,
                IsIndeterminate = false,
                Subtitle = "Alpha 23.7.4",
                Copyright = "Copyright © 2023 GUANDA",
            };


            Task.Run(() =>
            {
                Thread.Sleep(100);
                SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.StartSplashScreen(), ManagerVM);
                manager.Show(null, WindowStartupLocation.CenterScreen, true, InputBlockMode.Window);
                ManagerVM.IsIndeterminate = true;
               
                var StartD = DateTime.Now;
                while (manager.State == SplashScreenState.Shown)
                {
                    Thread.Sleep(100);
                    if (Math.Abs((DateTime.Now - StartD).TotalSeconds) > 5)
                    {
                        //如果五秒內都沒有出現彈窗 則不再等待
                        break;
                    }
                }
                if (manager.State == SplashScreenState.Showing)
                {
                    Thread.Sleep(1000);
                }

                StartD = DateTime.Now;


                Thread.Sleep(10);



                StampingMachineWindow MachineWindow;
                var ThreadOper = Dispatcher.BeginInvoke(new Action(delegate
                {
                    try
                    {
                        MachineWindow = new StampingMachineWindow();
                        MachineWindow.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show (ex.Message);
                        Environment.Exit(0);
                    }

                }));

                Thread.Sleep(1000);
                for (int i = 0; i <= 1000; i++)
                {
                    ManagerVM.IsIndeterminate = false;
                    ManagerVM.Progress = i/10;
                    Thread.Sleep(2);

                    //if (ThreadOper.Result != null)
                    //    break;
                }
                ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Text_Starting");
              

                ThreadOper.Wait();

                //當等待最少三秒後才關閉視窗
                while (Math.Abs((DateTime.Now - StartD).TotalSeconds) < 5)
                {
                    Task.Delay(100);
                }

                manager.Close();
            });



         }



    
    }
}
