using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GD_StampingMachine.UserControls
{
    /// <summary>
    /// RotateCircularKnob.xaml 的互動邏輯
    /// </summary>
    public partial class RotateCircularKnob : UserControl
    {
        public RotateCircularKnob()
        {
            InitializeComponent();
        }


        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(RotateCircularKnob), new PropertyMetadata(0d));



        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(RotateCircularKnob), new PropertyMetadata(100d));



        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(RotateCircularKnob), new PropertyMetadata(0d));



        public int Thickness
        {
            get { return (int)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(int), typeof(RotateCircularKnob), new PropertyMetadata(15));





        public Brush ProgressBrush
        {
            get { return (Brush)GetValue(ProgressBrushProperty); }
            set { SetValue(ProgressBrushProperty, value); }
        }

        public static readonly DependencyProperty ProgressBrushProperty =
            DependencyProperty.Register("ProgressBrush", typeof(Brush), typeof(RotateCircularKnob), new PropertyMetadata(Brushes.DeepSkyBlue));





        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register("TextBrush", typeof(Brush), typeof(RotateCircularKnob), new PropertyMetadata(Brushes.DeepSkyBlue));




        public int ProgressThickness
        {
            get { return (int)GetValue(ProgressThicknessProperty); }
            set { SetValue(ProgressThicknessProperty, value); }
        }

        public static readonly DependencyProperty ProgressThicknessProperty =
            DependencyProperty.Register("ProgressThickness", typeof(int), typeof(RotateCircularKnob), new PropertyMetadata(15));

    }
}
