using DevExpress.Dialogs.Core.ViewModel;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GD_StampingMachine.UserControls
{
    /// <summary>
    /// TriggerSliderUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class TriggerSliderUserControl : UserControl
    {
        public TriggerSliderUserControl()
        {
            InitializeComponent();
        }

        private void RepeatButton_Minus_Click(object sender, RoutedEventArgs e)
        {
            MainSlider.Value--;
        }
        private void RepeatButton_Plus_Click(object sender, RoutedEventArgs e)
        {
            MainSlider.Value++;
        }

        public double MainSilderValue
        {
            get => (double)GetValue(MainSilderValueProperty);
            set => SetValue(MainSilderValueProperty, value);
        }

        public static readonly DependencyProperty MainSilderValueProperty = DependencyProperty.Register(
            nameof(MainSilderValue),
            typeof(double),
            typeof(TriggerSliderUserControl),
            new PropertyMetadata());















    }
}
