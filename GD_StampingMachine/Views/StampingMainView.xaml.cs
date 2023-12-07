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



        private bool AllowDrag = true;
        Point _initialMousePosition;
        bool _isDragging;

        private void ColorZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!AllowDrag)
                e.Handled = false;

            var element = sender as FrameworkElement;
            _initialMousePosition = e.GetPosition(null);
            element.CaptureMouse();
            _isDragging = true;
            e.Handled = true;
        }
        private void ColorZone_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var currentPoint = e.GetPosition(null);

                Window parentWindow = Window.GetWindow(this);
                if (parentWindow != null)
                {
                    parentWindow.Left += (currentPoint.X - _initialMousePosition.X);
                    parentWindow.Top += (currentPoint.Y - _initialMousePosition.Y);
                }
            }
        }
        private void ColorZone_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                var element = sender as FrameworkElement;
                element.ReleaseMouseCapture();
                _isDragging = false;
                e.Handled = true;
            }
        }




    }
}
