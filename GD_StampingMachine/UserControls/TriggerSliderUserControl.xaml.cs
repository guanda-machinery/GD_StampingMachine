using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            MainSlider.Value -= TickFrequency;
        }
        private void RepeatButton_Plus_Click(object sender, RoutedEventArgs e)
        {
            MainSlider.Value += TickFrequency;
        }

        public double MainSliderValue
        {
            get => (double)GetValue(MainSliderValueProperty);
            set => SetValue(MainSliderValueProperty, value);
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


        public SolidColorBrush RepeatButtonForeground
        {
            get => (SolidColorBrush)GetValue(RepeatButtonForegroundProperty);
            set => SetValue(RepeatButtonForegroundProperty, value);
        }

        public ICommand MainSliderValueChanged
        {
            get => (ICommand)GetValue(MainSliderValueChangedProperty);
            set => SetValue(MainSliderValueChangedProperty, value);
        }




        public static readonly DependencyProperty MainSliderValueProperty = DependencyProperty.Register(
         nameof(MainSliderValue),
         typeof(double),
         typeof(TriggerSliderUserControl),
         new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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

        public static readonly DependencyProperty RepeatButtonForegroundProperty = DependencyProperty.Register(
 nameof(RepeatButtonForeground),
 typeof(SolidColorBrush),
 typeof(TriggerSliderUserControl),
 new PropertyMetadata(Brushes.White));


        public static readonly DependencyProperty MainSliderValueChangedProperty = DependencyProperty.Register(
 nameof(MainSliderValueChanged),
 typeof(ICommand),
 typeof(TriggerSliderUserControl),
 new PropertyMetadata());


    }
}
