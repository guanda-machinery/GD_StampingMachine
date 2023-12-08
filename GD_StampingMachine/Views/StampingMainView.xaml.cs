using DevExpress.Utils;
using GD_StampingMachine.ViewModels;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GD_StampingMachine.Views
{
    /// <summary>
    /// Interaction logic for StampingMainView.xaml
    /// </summary>
    public partial class StampingMainView : UserControl
    {
        public StampingMainView()
        {
            InitializeComponent();
        }


        private void ColorZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
                {
                    parentWindow.DragMove();
                }
                if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
                {
                    if (parentWindow.WindowState == WindowState.Maximized)
                        parentWindow.WindowState = WindowState.Normal;
                    else
                        parentWindow.WindowState = WindowState.Maximized;
                }
            }


        }




    }
}
