
using DevExpress.CodeParser;
using DevExpress.Xpf.Core.Internal;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GD_StampingMachine.UserControls.CustomControls
{
    /// <summary>
    /// FunctionToggleUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class FunctionToggleUserControl : UserControl
    {
        public FunctionToggleUserControl()
        {
            InitializeComponent();
        }


        //new PropertyMetadata(TitleTextPropertyChanged)
        /// <summary>
        /// <see cref=""/> 註冊為依賴屬性
        /// </summary>
        public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register(nameof(TitleText), typeof(string), typeof(FunctionToggleUserControl), new PropertyMetadata());
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(FunctionToggleUserControl), new PropertyMetadata());
        
        public static readonly DependencyProperty IsCheckedKindProperty = DependencyProperty.Register(nameof(IsCheckedKind), typeof(PackIconKind?), typeof(FunctionToggleUserControl), new PropertyMetadata(null));
        public static readonly DependencyProperty UnCheckedKindProperty = DependencyProperty.Register(nameof(UnCheckedKind), typeof(PackIconKind?), typeof(FunctionToggleUserControl), new PropertyMetadata(null));
        //public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnIsCheckedChanged));
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool?), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
        public static readonly DependencyProperty IsThreeStateProperty = DependencyProperty.Register(nameof(IsThreeState), typeof(bool), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register(nameof(TitleFontSize), typeof(double), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(14.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(FunctionToggleUserControl), new PropertyMetadata(40.0));
        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(FunctionToggleUserControl), new PropertyMetadata(40.0));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(new CornerRadius(0)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(Orientation.Vertical));
        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register(nameof(TextMargin), typeof(Thickness), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(new Thickness(0)));
        //public static readonly DependencyProperty ToggleButtonContentProperty = DependencyProperty.Register(nameof(ToggleButtonContent), typeof(object), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
        //public static readonly DependencyProperty ToggleButtonContentProperty = DependencyProperty.Register(nameof(ToggleButtonContent), typeof(object), typeof(FunctionToggleUserControl), new PropertyMetadata(null));


        public string TitleText
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }
        
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public PackIconKind? IsCheckedKind
        {
            get { return (PackIconKind?)GetValue(IsCheckedKindProperty); }
            set { SetValue(IsCheckedKindProperty, value); }
        }
        public PackIconKind? UnCheckedKind
        {
            get { return (PackIconKind?)GetValue(UnCheckedKindProperty); }
            set { SetValue(UnCheckedKindProperty, value); }
        }

        public bool IsThreeState
        {
            get { return (bool)GetValue(IsThreeStateProperty); }
            set { SetValue(IsThreeStateProperty, value); }
        }




        [Category("Appearance")]
        [TypeConverter(typeof(NullableBoolConverter))]
        [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
        public bool? IsChecked
        {
            get{ return (bool?)GetValue(IsCheckedProperty);}
            set{SetValue(IsCheckedProperty, value);}
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


        /*public object ToggleButtonContent
        {
            get
            {
                return (object)GetValue(ToggleButtonContentProperty);
            }
            set
            {
                SetValue(ToggleButtonContentProperty, value);
            }
        }*/








    }



}
