using DevExpress.CodeParser;
using DevExpress.Data.Browsing;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GD_StampingMachine.UserControls.CustomControls
{
    /// <summary>
    /// FunctionToggleButton.xaml 的互動邏輯
    /// </summary>
    public partial class FunctionToggleButton : UserControl
    {
        public FunctionToggleButton()
        {
            InitializeComponent();
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(FunctionToggleButton), new PropertyMetadata());


        public string ButtonContentText
        {
            get { return (string)GetValue(ButtonContentTextProperty); }
            set { SetValue(ButtonContentTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonContentTextProperty =
            DependencyProperty.Register(nameof(ButtonContentText), typeof(string), typeof(FunctionToggleButton), new PropertyMetadata("Logo"));

        /// <summary>
        /// 按鈕勾選
        /// </summary>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        /// <summary>
        /// <see cref=""/> 註冊為依賴屬性
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(FunctionToggleButton), new PropertyMetadata());


        public string ButtonTitleText
        {
            get { return (string)GetValue(ButtonTitleTextProperty); }
            set { SetValue(ButtonTitleTextProperty, value); }
        }
        public static readonly DependencyProperty ButtonTitleTextProperty =
    DependencyProperty.Register(nameof(ButtonTitleText), typeof(string), typeof(FunctionToggleButton), new PropertyMetadata());

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        public static readonly DependencyProperty OrientationProperty =
    DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(FunctionToggleButton), new PropertyMetadata(Orientation.Vertical));



        public Visibility ProgressBarCollapsed
        {
            get { return (Visibility)GetValue(ProgressBarCollapsedProperty); }
            set { SetValue(ProgressBarCollapsedProperty, value); }
        }
        public static readonly DependencyProperty ProgressBarCollapsedProperty =
    DependencyProperty.Register(nameof(ProgressBarCollapsed), typeof(Visibility), typeof(FunctionToggleButton), new PropertyMetadata());


        public bool ProgressIsIndeterminate
        {
            get { return (bool)GetValue(ProgressIsIndeterminateProperty); }
            set { SetValue(ProgressIsIndeterminateProperty, value); }
        }
        public static readonly DependencyProperty ProgressIsIndeterminateProperty =
    DependencyProperty.Register(nameof(ProgressIsIndeterminate), typeof(bool), typeof(FunctionToggleButton), new PropertyMetadata(true));

        public double ProgressBarValue
        {
            get { return (double)GetValue(ProgressBarValueProperty); }
            set { SetValue(ProgressBarValueProperty, value); }
        }
        public static readonly DependencyProperty ProgressBarValueProperty =
    DependencyProperty.Register(nameof(ProgressBarValue), typeof(double), typeof(FunctionToggleButton), new PropertyMetadata(50.0));

        public double ProgressBarMaximum
        {
            get { return (double)GetValue(ProgressBarMaximumProperty); }
            set { SetValue(ProgressBarMaximumProperty, value); }
        }
        public static readonly DependencyProperty ProgressBarMaximumProperty =
    DependencyProperty.Register(nameof(ProgressBarMaximum), typeof(double), typeof(FunctionToggleButton), new PropertyMetadata(100.0));

        public double ProgressBarMinimum
        {
            get { return (double)GetValue(ProgressBarMinimumProperty); }
            set { SetValue(ProgressBarMinimumProperty, value); }
        }
        public static readonly DependencyProperty ProgressBarMinimumProperty =
    DependencyProperty.Register(nameof(ProgressBarMinimum), typeof(double), typeof(FunctionToggleButton), new PropertyMetadata(0.0));
















        private void Border_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TButton.IsChecked = !TButton.IsChecked;
        }
    }

}
