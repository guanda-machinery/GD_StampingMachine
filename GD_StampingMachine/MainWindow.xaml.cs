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
                Subtitle = "Alpha 23.4.24",
                Copyright = "Copyright © 2022 GUANDA",
            };

            LoadLanguage();
            //靜態調用. 當資源字典變更成其他語系後, Title並不會隨著變化
            //lblTitle.Content = FindResource(“lblTitle”).ToString();
            //動態調用, Title會隨著字典而變化.
            // lblTitle.SetResourceReference(Label.ContentProperty, “lblTitle”);
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-TW");
    
            Task.Run(() =>
            {


                Thread.Sleep(100);
                SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.StartSplashScreen(), ManagerVM);
                manager.Show(null, WindowStartupLocation.CenterScreen, true, InputBlockMode.Window);
                ManagerVM.IsIndeterminate = true;

                StampingMachineWindow MachineWindow;
                var ThreadOper = Dispatcher.BeginInvoke(new Action(delegate
                {
                    Thread.Sleep(1000);
                    MachineWindow = new StampingMachineWindow();
                    MachineWindow.Show();
                }));

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
                manager.Close();
            });



         }


        private void LoadLanguage()
        {
            CultureInfo currentCultureInfo = CultureInfo.CurrentCulture;
            ResourceDictionary langRd = null;
            try
            {
                langRd = Application.LoadComponent(
                new Uri(@"Language\" + "StringResource." + currentCultureInfo.Name + ".xaml ", UriKind.Relative)) as ResourceDictionary;
            }
            catch
            {
            }

            if (langRd != null)
            {
                if (this.Resources.MergedDictionaries.Count > 0)
                {
                    this.Resources.MergedDictionaries.Clear();
                }
                this.Resources.MergedDictionaries.Add(langRd);
            }
        }



    
    }
}
