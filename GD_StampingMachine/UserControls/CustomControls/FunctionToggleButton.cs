using DevExpress.DocumentView;
using DevExpress.Xpf.Core.Internal;
using DevExpress.XtraRichEdit.Layout;
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

namespace GD_StampingMachine
{
    /// <summary>
    /// 依照步驟 1a 或 1b 執行，然後執行步驟 2，以便在 XAML 檔中使用此自訂控制項。
    ///
    /// 步驟 1a) 於存在目前專案的 XAML 檔中使用此自訂控制項。
    /// 加入此 XmlNamespace 屬性至標記檔案的根項目為 
    ///要使用的: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:GD_StampingMachine.UserControls.CustomControls"
    ///
    ///
    /// 步驟 1b) 於存在其他專案的 XAML 檔中使用此自訂控制項。
    /// 加入此 XmlNamespace 屬性至標記檔案的根項目為 
    ///要使用的: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:GD_StampingMachine.UserControls.CustomControls;assembly=GD_StampingMachine.UserControls.CustomControls"
    ///
    /// 您還必須將 XAML 檔所在專案的專案參考加入
    /// 此專案並重建，以免發生編譯錯誤: 
    ///
    ///     在 [方案總管] 中以滑鼠右鍵按一下目標專案，並按一下
    ///     [加入參考]->[專案]->[瀏覽並選取此專案]
    ///
    ///
    /// 步驟 2)
    /// 開始使用 XAML 檔案中的控制項。
    ///
    ///     <MyNamespace:MainFunctionToggleButton/>
    ///
    /// </summary>
    public class MainFunctionToggleButton : ToggleButton
    {
        static MainFunctionToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(typeof(MainFunctionToggleButton)));

            ControlTitleTextProperty = DependencyProperty.Register(nameof(ControlTitleText), typeof(string), typeof(MainFunctionToggleButton), new PropertyMetadata());
            ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(MainFunctionToggleButton), new PropertyMetadata());

            PackIconKindProperty = DependencyProperty.Register(nameof(PackIconKind), typeof(PackIconKind), typeof(MainFunctionToggleButton), new PropertyMetadata(PackIconKind.None));

            ButtonContentProperty = DependencyProperty.Register(nameof(ButtonContent), typeof(UIElement), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

            IsDragableProperty = DependencyProperty.Register(nameof(IsDragable), typeof(bool), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
            IsDropableProperty = DependencyProperty.Register(nameof(IsDropable), typeof(bool), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

            TitleFontSizeProperty = DependencyProperty.Register(nameof(TitleFontSize), typeof(double), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(14.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            ImageWidthProperty = DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(MainFunctionToggleButton), new PropertyMetadata(40.0));
            ImageHeightProperty = DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(MainFunctionToggleButton), new PropertyMetadata(40.0));
            CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(new CornerRadius(0)));
            OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(Orientation.Vertical));
            TextMarginProperty = DependencyProperty.Register(nameof(TextMargin), typeof(Thickness), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(new Thickness(0)));

            ProgressProperty = DependencyProperty.Register(nameof(Progress), typeof(double), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            IsIndeterminateProperty = DependencyProperty.Register(nameof(IsIndeterminate), typeof(bool), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            ProgressVisibilityProperty = DependencyProperty.Register(nameof(ProgressVisibility), typeof(Visibility), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(Visibility.Collapsed, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

            IsCheckedBackgroundProperty = DependencyProperty.Register(nameof(IsCheckedBackground), typeof(Brush), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            MouseIsOverBackgroundProperty = DependencyProperty.Register(nameof(MouseIsOverBackground), typeof(Brush), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            IsCheckedBorderThicknessProperty = DependencyProperty.Register(nameof(IsCheckedBorderThickness), typeof(double), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

            // IsCheckedOpacityProperty = DependencyProperty.Register(nameof(IsCheckedOpacity), typeof(double), typeof(MainFunctionToggleButton), new UIPropertyMetadata(1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            IsCheckedOpacityProperty = DependencyProperty.Register(nameof(IsCheckedOpacity), typeof(double), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

            ContentPaddingProperty = DependencyProperty.Register(nameof(ContentPadding), typeof(Thickness), typeof(MainFunctionToggleButton), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        }


        public static readonly DependencyProperty ControlTitleTextProperty;
        public static readonly DependencyProperty ImageSourceProperty;
        public static readonly DependencyProperty PackIconKindProperty;
        public static readonly DependencyProperty ButtonContentProperty;

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
        public static readonly DependencyProperty IsCheckedOpacityProperty;

        public static readonly DependencyProperty ContentPaddingProperty;








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
        /// 圖片樣式
        /// </summary>
        public UIElement ButtonContent
        {
            get { return (UIElement)GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
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

        public double IsCheckedOpacity
        {
            get
            {
                return (double)GetValue(IsCheckedOpacityProperty);
            }
            set
            {
                SetValue(IsCheckedOpacityProperty, value);
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






        public Thickness ContentPadding
        {
            get
            {
                return (Thickness)GetValue(ContentPaddingProperty);
            }
            set
            {
                SetValue(ContentPaddingProperty, value);
            }
        }












    }
}
