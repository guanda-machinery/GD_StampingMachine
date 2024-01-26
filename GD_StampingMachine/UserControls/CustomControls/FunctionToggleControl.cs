using DevExpress.Xpf.Core.Internal;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GD_StampingMachine.UserControls
{
    public class FunctionToggleControl : ToggleButton , ICloneable
    {
        static FunctionToggleControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FunctionToggleControl), new FrameworkPropertyMetadata(typeof(FunctionToggleControl)));

            ControlTitleTextProperty = DependencyProperty.Register(nameof(ControlTitleText), typeof(string), typeof(FunctionToggleControl), new PropertyMetadata());
            // UnCheckedTitleTextProperty = DependencyProperty.Register(nameof(UnCheckedTitleText), typeof(string), typeof(FunctionToggleControl), new PropertyMetadata());
            ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(FunctionToggleControl), new PropertyMetadata());

            PackIconKindProperty = DependencyProperty.Register(nameof(PackIconKind), typeof(PackIconKind), typeof(FunctionToggleControl), new PropertyMetadata(PackIconKind.None));

          
            IsDragableProperty = DependencyProperty.Register(nameof(IsDragable), typeof(bool), typeof(FunctionToggleControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
            IsDropableProperty = DependencyProperty.Register(nameof(IsDropable), typeof(bool), typeof(FunctionToggleControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

            TitleFontSizeProperty = DependencyProperty.Register(nameof(TitleFontSize), typeof(double), typeof(FunctionToggleControl), new FrameworkPropertyMetadata(14.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            ImageWidthProperty = DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(FunctionToggleControl), new PropertyMetadata(40.0));
            ImageHeightProperty = DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(FunctionToggleControl), new PropertyMetadata(40.0));
            CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(FunctionToggleControl), new FrameworkPropertyMetadata(new CornerRadius(0)));
            OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(FunctionToggleControl), new FrameworkPropertyMetadata(Orientation.Vertical));
            TextMarginProperty = DependencyProperty.Register(nameof(TextMargin), typeof(Thickness), typeof(FunctionToggleControl), new FrameworkPropertyMetadata(new Thickness(0)));

            ProgressProperty = DependencyProperty.Register(nameof(Progress), typeof(double), typeof(FunctionToggleControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            IsIndeterminateProperty = DependencyProperty.Register(nameof(IsIndeterminate), typeof(bool), typeof(FunctionToggleControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            ProgressVisibilityProperty = DependencyProperty.Register(nameof(ProgressVisibility), typeof(Visibility), typeof(FunctionToggleControl), new FrameworkPropertyMetadata(Visibility.Collapsed, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

            IsCheckedBackgroundProperty = DependencyProperty.Register(nameof(IsCheckedBackground), typeof(Brush), typeof(FunctionToggleControl), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            MouseIsOverBackgroundProperty = DependencyProperty.Register(nameof(MouseIsOverBackground), typeof(Brush), typeof(FunctionToggleControl), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            IsCheckedBorderThicknessProperty = DependencyProperty.Register(nameof(IsCheckedBorderThickness), typeof(double), typeof(FunctionToggleControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

            
        }


        //new PropertyMetadata(TitleTextPropertyChanged)
        /// <summary>
        /// <see cref=""/> 註冊為依賴屬性
        /// </summary>
        public static readonly DependencyProperty ControlTitleTextProperty;
        public static readonly DependencyProperty ImageSourceProperty;
        public static readonly DependencyProperty PackIconKindProperty;


        public static readonly DependencyProperty IsDragableProperty;
        public static readonly DependencyProperty IsDropableProperty;

        public static readonly DependencyProperty TitleFontSizeProperty;
        public static readonly DependencyProperty ImageWidthProperty;
        public static readonly DependencyProperty ImageHeightProperty;
        public static readonly DependencyProperty CornerRadiusProperty;
        public static readonly DependencyProperty OrientationProperty;
        public static readonly DependencyProperty TextMarginProperty;

        public static readonly DependencyProperty ProgressProperty;
        public static readonly DependencyProperty IsIndeterminateProperty;
        public static readonly DependencyProperty ProgressVisibilityProperty;

        public static readonly DependencyProperty IsCheckedBackgroundProperty;
        public static readonly DependencyProperty MouseIsOverBackgroundProperty;
        public static readonly DependencyProperty IsCheckedBorderThicknessProperty;








        public double Progress
        {
            get
            {
                return (double)GetValue(ProgressProperty);
            }
            set
            {
                SetValue(ProgressProperty, value);
            }
        }

        public bool IsIndeterminate
        {
            get
            {
                return (bool)GetValue(IsIndeterminateProperty);
            }
            set
            {
                SetValue(IsIndeterminateProperty, value);
            }
        }

        public Visibility ProgressVisibility
        {
            get
            {
                return (Visibility)GetValue(ProgressVisibilityProperty);
            }
            set
            {
                SetValue(ProgressVisibilityProperty, value);
            }
        }

        







        public string ControlTitleText
        {
            get => (string)GetValue(ControlTitleTextProperty);
            set => SetValue(ControlTitleTextProperty, value);
        }
        /*  public string UnCheckedTitleText
          {
              get => (string)GetValue(UnCheckedTitleTextProperty);
              set => SetValue(UnCheckedTitleTextProperty, value);
          }*/

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }


        public PackIconKind PackIconKind
        {
            get { return (PackIconKind)GetValue(PackIconKindProperty); }
            set { SetValue(PackIconKindProperty, value); }
        }

        /// <summary>
        /// 可以被抓
        /// </summary>
        public bool IsDragable
        {
            get { return (bool)GetValue(IsDragableProperty); }
            set { SetValue(IsDragableProperty, value); }
        }

        /// <summary>
        /// 可以被丟
        /// </summary>
        public bool IsDropable
        {
            get { return (bool)GetValue(IsDropableProperty); }
            set { SetValue(IsDropableProperty, value); }
        }






        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }
        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }
        public Thickness TextMargin
        {
            get
            {
                return (Thickness)GetValue(TextMarginProperty);
            }
            set
            {
                SetValue(TextMarginProperty, value);
            }
        }

        public Brush IsCheckedBackground
        {
            get
            {
                return (Brush)GetValue(IsCheckedBackgroundProperty);
            }
            set
            {
                SetValue(IsCheckedBackgroundProperty, value);
            }
        }

        public Brush MouseIsOverBackground
        {
            get
            {
                return (Brush)GetValue(MouseIsOverBackgroundProperty);
            }
            set
            {
                SetValue(MouseIsOverBackgroundProperty, value);
            }
        }


        public double IsCheckedBorderThickness
        {
            get
            {
                return (double)GetValue(IsCheckedBorderThicknessProperty);
            }
            set
            {
                SetValue(IsCheckedBorderThicknessProperty, value);
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
