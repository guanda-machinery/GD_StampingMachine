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


        /// <summary>
        /// 修改命令
        /// </summary>
   /*     public double MainSilderValue
        {
            get { return (double)GetValue(MainSilderValueProperty); }
            set { SetValue(MainSilderValueProperty, value); }
        }*/
        /// <summary>
        /// <see cref="MainSilderValueProperty"/> 註冊相依屬性
        /// </summary>
       // public static readonly DependencyProperty MainSilderValueProperty =
         //   DependencyProperty.Register(nameof(MainSilderValue), typeof(double), typeof(TriggerSliderUserControl), new PropertyMetadata(MainSilderValuePropertyChange));
        /// <summary>
        /// <see cref="MainSilderValuePropertyChange"/> 變更時觸發
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
       /* private static void MainSilderValuePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TriggerSliderUserControl)d).MainSlider.Value = (double)e.NewValue;
        }*/


        public static readonly DependencyProperty MainSilderValueProperty = DependencyProperty.Register(
            nameof(MainSilderValue),
            typeof(double),
            typeof(TriggerSliderUserControl),
            new PropertyMetadata());
        public double MainSilderValue
        {
            get => (double)GetValue(MainSilderValueProperty);
            set => SetValue(MainSilderValueProperty, value);
        }
















    }
}
