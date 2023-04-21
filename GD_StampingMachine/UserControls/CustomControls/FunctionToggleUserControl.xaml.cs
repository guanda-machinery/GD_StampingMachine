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

        public UIElement ButtonContent
        {
            get { return (UIElement)GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }
        /// <summary>
        /// <see cref=""/> 註冊為依賴屬性
        /// </summary>
        public static readonly DependencyProperty ButtonContentProperty =
            DependencyProperty.Register(nameof(ButtonContent), typeof(UIElement), typeof(FunctionToggleUserControl), new PropertyMetadata(ButtonContentPropertyChanged));
        /// <summary>
        /// <see cref=""/>變更時觸發
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ButtonContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FunctionToggleUserControl).TButton.Content = (UIElement)e.NewValue;
        }



        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }



        /// <summary>
        /// <see cref=""/> 註冊為依賴屬性
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(FunctionToggleUserControl), new PropertyMetadata(CommandPropertyChanged));
        /// <summary>
        /// <see cref=""/>變更時觸發
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void CommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FunctionToggleUserControl).TButton.Command = (ICommand)e.NewValue;
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

        public static readonly DependencyProperty CommandParameterProperty = 
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(FunctionToggleUserControl), new PropertyMetadata(CommandParameterPropertyChanged));
        private static void CommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FunctionToggleUserControl).TButton.CommandParameter = e.NewValue;
        }


        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        /// <summary>
        /// <see cref=""/> 註冊為依賴屬性
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(FunctionToggleUserControl), new PropertyMetadata(IsCheckedPropertyChanged));
        /// <summary>
        /// <see cref=""/>變更時觸發
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void IsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FunctionToggleUserControl).TButton.IsChecked = (bool)e.NewValue;
        }


        public string TitleText
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }
        /// <summary>
        /// <see cref=""/> 註冊為依賴屬性
        /// </summary>
        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register(nameof(TitleText), typeof(string), typeof(FunctionToggleUserControl), new PropertyMetadata(TitleTextPropertyChanged));
        /// <summary>
        /// <see cref=""/>變更時觸發
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void TitleTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FunctionToggleUserControl).TitleTextBlock.Text = (string)e.NewValue;
        }










    }
}
