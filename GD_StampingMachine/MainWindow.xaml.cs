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



            SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new SplashScreenWindow.ProcessingScreenWindow(), new DXSplashScreenViewModel { });

            manager.ViewModel.Status = (string)System.Windows.Application.Current.TryFindResource("Text_Loading");
           // manager.ViewModel.Status = "讀取中.";

            manager.ViewModel.Progress = 0;
            manager.ViewModel.IsIndeterminate = false;
            manager.Show(null, WindowStartupLocation.CenterScreen, true, InputBlockMode.Window);

            Task.Run(() =>
            {
               Thread.Sleep(1000);
                for (int i = 0; i <= 100; i++)
                {
                    manager.ViewModel.Progress = i;
                    Thread.Sleep(10);
                }
                //manager.ViewModel.Status = "正在啟動...";
                manager.ViewModel.Status = (string)System.Windows.Application.Current.TryFindResource("Text_Starting");

                Dispatcher.BeginInvoke(new Action(delegate
                {
                    var MachineWindow = new StampingMachineWindow();
                    MachineWindow.Show();
                }));
                manager.Close();
            });


            LoadLanguage();

            //靜態調用. 當資源字典變更成其他語系後, Title並不會隨著變化
            //lblTitle.Content = FindResource(“lblTitle”).ToString();
            //動態調用, Title會隨著字典而變化.
            // lblTitle.SetResourceReference(Label.ContentProperty, “lblTitle”);

            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-TW");

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
