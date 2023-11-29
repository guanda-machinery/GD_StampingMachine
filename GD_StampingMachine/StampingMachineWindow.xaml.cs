using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI;
using GD_CommonLibrary.Method;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var MessageBoxReturn =  MessageBoxResultShow.Show(
                (string)Application.Current.TryFindResource("Text_notify"), 
                (string)Application.Current.TryFindResource("Text_AskCloseProgram") ,
                  MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (MessageBoxReturn == MessageBoxResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                return;
            }

        }


        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
