using DevExpress.Charts.Native;
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
            Window parentWindow = Application.Current.MainWindow;// Window.GetWindow(this);
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

        private void FunctionToggleUserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool value && !value)
            {
                if (sender is GD_StampingMachine.UserControls.FunctionToggleUserControl FToggle && FToggle.IsChecked is true)
                {
                    FToggle.IsChecked = false;
                }
            }
        }
    }
}
