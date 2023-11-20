using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI;
using GD_StampingMachine.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GD_StampingMachine
{
    /// <summary>
    /// StampingMachineWindow.xaml 的互動邏輯
    /// </summary>
    public partial class StampingMachineWindow : Window
    {
        public StampingMachineWindow()
        {
            InitializeComponent();
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var MessageBoxReturn = WinUIMessageBox.Show(new Window(),
    (string)Application.Current.TryFindResource("Text_AskCloseProgram"),
    (string)Application.Current.TryFindResource("Text_notify"),
    MessageBoxButton.YesNo,
    MessageBoxImage.Exclamation,
    MessageBoxResult.None,
    MessageBoxOptions.None,
    FloatingMode.Window);

            if (MessageBoxReturn == MessageBoxResult.Yes)
                e.Cancel = false;
            else
                e.Cancel = true;

            await StampMachineDataSingleton.Instance.DisposeAsync();
            //呼叫各儲存命令
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);

        }
    }
}
