using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GD_StampingMachine.UserControls
{
    public class ImageBrightDarkCustomControl : Control
    {


        static ImageBrightDarkCustomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageBrightDarkCustomControl), new FrameworkPropertyMetadata(typeof(ImageBrightDarkCustomControl)));

            BrightImageSourceProperty = DependencyProperty.Register(nameof(BrightImageSource), typeof(ImageSource), typeof(ImageBrightDarkCustomControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnBrightImageSourceChanged, null), null);
            DarkImageSourceProperty = DependencyProperty.Register(nameof(DarkImageSource), typeof(ImageSource), typeof(ImageBrightDarkCustomControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnDarkImageSourceChanged, null), null);
        }


        public static readonly DependencyProperty BrightImageSourceProperty;
        public static readonly DependencyProperty DarkImageSourceProperty;

        public ImageSource BrightImageSource
        {
            get => (ImageSource)GetValue(BrightImageSourceProperty);
            set => SetValue(BrightImageSourceProperty, value);
        }
        public ImageSource DarkImageSource
        {
            get => (ImageSource)GetValue(DarkImageSourceProperty);
            set => SetValue(DarkImageSourceProperty, value);
        }

        private static void OnBrightImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageBrightDarkCustomControl image = (ImageBrightDarkCustomControl)d;
            image.BrightImageSource = (ImageSource)e.NewValue;
        }
        private static void OnDarkImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageBrightDarkCustomControl image = (ImageBrightDarkCustomControl)d;
            image.DarkImageSource = (ImageSource)e.NewValue;
        }


    }
}
