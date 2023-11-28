
using DevExpress.CodeParser;
using DevExpress.Utils.MVVM;
using DevExpress.Xpf.Core.Internal;
using GD_CommonLibrary.GD_Popup;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
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

namespace GD_StampingMachine.UserControls
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
        public static readonly DependencyProperty ControlTitleTextProperty = DependencyProperty.Register(nameof(ControlTitleText), typeof(string), typeof(FunctionToggleUserControl), new PropertyMetadata());
        //public static readonly DependencyProperty UnCheckedTitleTextProperty = DependencyProperty.Register(nameof(UnCheckedTitleText), typeof(string), typeof(FunctionToggleUserControl), new PropertyMetadata());
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(FunctionToggleUserControl), new PropertyMetadata());
        
        public static readonly DependencyProperty PackIconKindProperty = DependencyProperty.Register(nameof(PackIconKind), typeof(PackIconKind), typeof(FunctionToggleUserControl), new PropertyMetadata(PackIconKind.None));

        public static readonly DependencyProperty ButtonContentProperty =  DependencyProperty.Register(nameof(ButtonContent), typeof(UIElement), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
        //public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnIsCheckedChanged));
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool?), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
        //public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(false, OnIsCheckedChanged));

        public static readonly DependencyProperty IsThreeStateProperty = DependencyProperty.Register(nameof(IsThreeState), typeof(bool), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        public static readonly DependencyProperty IsDragableProperty = DependencyProperty.Register(nameof(IsDragable), typeof(bool), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
        public static readonly DependencyProperty IsDropableProperty = DependencyProperty.Register(nameof(IsDropable), typeof(bool), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register(nameof(TitleFontSize), typeof(double), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(14.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(FunctionToggleUserControl), new PropertyMetadata(40.0));
        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(FunctionToggleUserControl), new PropertyMetadata(40.0));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(new CornerRadius(0)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(Orientation.Vertical));
        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register(nameof(TextMargin), typeof(Thickness), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(new Thickness(0)));
       
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(nameof(Progress), typeof(double), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(nameof(IsIndeterminate), typeof(bool), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ProgressVisibilityProperty = DependencyProperty.Register(nameof(ProgressVisibility), typeof(Visibility), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(Visibility.Collapsed, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty IsCheckedBackgroundProperty = DependencyProperty.Register(nameof(IsCheckedBackground), typeof(Brush), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty MouseIsOverBackgroundProperty = DependencyProperty.Register(nameof(MouseIsOverBackground), typeof(Brush), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsCheckedBorderThicknessProperty = DependencyProperty.Register(nameof(IsCheckedBorderThickness), typeof(double), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(FunctionToggleUserControl), new FrameworkPropertyMetadata((object)null));
      
        private static void OnIsCheckedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var thisobj = (FunctionToggleUserControl)obj;

            thisobj.Toggle.IsChecked = e.NewValue as bool?;
        }




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
            get { return (bool) GetValue(IsIndeterminateProperty);
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
            get=>(string)GetValue(ControlTitleTextProperty); 
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
        /// 圖片樣式
        /// </summary>
        public UIElement ButtonContent
        {
            get { return (UIElement)GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }

        /// <summary>
        /// <see cref=""/>變更時觸發
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
       /* private static void ButtonContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FunctionToggleUserControl).Toggle.Content = (UIElement)e.NewValue;
        }*/


        public bool IsThreeState
        {
            get { return (bool)GetValue(IsThreeStateProperty); }
            set { SetValue(IsThreeStateProperty, value); }
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



        [Category("Appearance")]
        [TypeConverter(typeof(NullableBoolConverter))]
        [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
        public bool? IsChecked
        {
            get
            {
                object value = GetValue(IsCheckedProperty);
                if (value == null)
                {
                    return null;
                }
                return (bool)value;
            }
            set=>SetValue(IsCheckedProperty, value.HasValue? BooleanBoxes.Box(value.Value) : null);
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



        [Bindable(true)]
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }



        [Bindable(true)]
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        [Bindable(true)]
        [Category("Action")]
        public IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)GetValue(CommandTargetProperty);
            }
            set
            {
                SetValue(CommandTargetProperty, value);
            }
        }










    }



}
