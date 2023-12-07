using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI;
using GD_CommonLibrary.Method;
using GD_StampingMachine.Singletons;
using System;
using System.Collections.Generic;
using System.Data;
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

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }



        //Point _initialMousePosition;
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
              /*  if (WindowState == WindowState.Maximized)
                {
                    var currentPoint = e.GetPosition(null);
                    if(currentPoint != _initialMousePosition)
                    {
                        WindowState =  WindowState.Normal;

                    }
                }*/

                this.DragMove();
            }

            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Maximized;
            }

            //_initialMousePosition = e.GetPosition(null);
        }

       // private DateTime lastMouseMoveTime = DateTime.MinValue;
        //private TimeSpan minMouseMoveInterval = TimeSpan.FromMilliseconds(100); // 設定最小處理間隔

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
          /*  DateTime now = DateTime.Now;
            if ((now - lastMouseMoveTime) >= minMouseMoveInterval)
            {
                _initialMousePosition = e.GetPosition(null);
                lastMouseMoveTime = now;
            }*/
        }
    }
}
