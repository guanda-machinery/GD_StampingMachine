using DevExpress.Xpf.Core;
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
using DevExpress.Xpf.WindowsUI;

namespace GD_StampingMachine
{
    /// <summary>
    /// StampingMachineWindow.xaml 的互動邏輯
    /// </summary>
    public partial class StampingMachineWindow : ThemedWindow
    {
        public StampingMachineWindow()
        {
            InitializeComponent();
        }

        private void ThemedWindow_Closed(object sender, EventArgs e)
        {
                Environment.Exit(0);
        }

        private void ThemedWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var MessageBoxReturn = WinUIMessageBox.Show(null,
                "是否要結束程式?",
                "通知",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,
                MessageBoxResult.None,
                MessageBoxOptions.None,
                FloatingMode.Window);
            if (MessageBoxReturn == MessageBoxResult.Yes)
                e.Cancel = false;
            else
                e.Cancel = true;
            //呼叫各儲存命令
        }
    }
}
