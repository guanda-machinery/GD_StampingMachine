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
        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }
        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        public double TickFrequency
        {
            get => (double)GetValue(TickFrequencyProperty);
            set => SetValue(TickFrequencyProperty, value);
        }


        public static readonly DependencyProperty MainSilderValueProperty = DependencyProperty.Register(
         nameof(MainSilderValue),
         typeof(double),
         typeof(TriggerSliderUserControl),
         new PropertyMetadata(0.0));

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
         nameof(Minimum),
         typeof(double),
         typeof(TriggerSliderUserControl),
         new PropertyMetadata(0.0));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
         nameof(Maximum),
         typeof(double),
         typeof(TriggerSliderUserControl),
         new PropertyMetadata(100.0));

        public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register(
         nameof(TickFrequency),
         typeof(double),
         typeof(TriggerSliderUserControl),
         new PropertyMetadata(1.0));


    }
}
