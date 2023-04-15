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

        /// <summary>
        /// 按鈕命令
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        /// <summary>
        /// <see cref="ButtonCommand"/> 註冊為依賴屬性
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(FunctionToggleButton), new PropertyMetadata(CommandPropertyChanged));
        /// <summary>
        /// <see cref="ButtonCommand"/> 變更時觸發
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void CommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FunctionToggleButton menuControl = (d as FunctionToggleButton);
            menuControl.TButton.Command = (ICommand)e.NewValue;
        }

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
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(FunctionToggleButton), new PropertyMetadata(IsCheckedPropertyChanged));
        /// <summary>
        /// <see cref=""/> 變更時觸發
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void IsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FunctionToggleButton menuControl = (d as FunctionToggleButton);
            menuControl.TButton.IsChecked = (bool)e.NewValue;
        }



        /// <summary>
        /// 
        /// </summary>
        public string ButtonTitleText
        {
            get { return (string)GetValue(ButtonTitleTextProperty); }
            set { SetValue(ButtonTitleTextProperty, value); }
        }
        /// <summary>
        /// <see cref=""/> 註冊為依賴屬性
        /// </summary>
        public static readonly DependencyProperty ButtonTitleTextProperty =
            DependencyProperty.Register(nameof(ButtonTitleText), typeof(string), typeof(FunctionToggleButton), new PropertyMetadata(ButtonTitleTextPropertyChanged));
        /// <summary>
        /// <see cref=""/> 變更時觸發
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ButtonTitleTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FunctionToggleButton menuControl = (d as FunctionToggleButton);
            menuControl.ButtonTitleTextBlock.Text = (string)e.NewValue;
        }


        /// <summary>
        /// 
        /// </summary>
        public string ButtonContentText
        {
            get { return (string)GetValue(ButtonContentTextProperty); }
            set { SetValue(ButtonContentTextProperty, value); }
        }
        /// <summary>
        /// <see cref=""/> 註冊為依賴屬性
        /// </summary>
        public static readonly DependencyProperty ButtonContentTextProperty =
            DependencyProperty.Register(nameof(ButtonContentText), typeof(string), typeof(FunctionToggleButton), new PropertyMetadata(ButtonContentTextPropertyChanged));
        /// <summary>
        /// <see cref=""/> 變更時觸發
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ButtonContentTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FunctionToggleButton menuControl = (d as FunctionToggleButton);
            menuControl.ButtonContentTextBlock.Text = (string)e.NewValue;
        }

        private void TButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((e.Source as ToggleButton).IsChecked.HasValue)
                this.IsChecked = (e.Source as ToggleButton).IsChecked.Value;
            else
                this.IsChecked = false;
        }


        public Orientation StackPanelOrientation
        {
            get { return (Orientation)GetValue(StackPanelOrientationProperty); }
            set { SetValue(StackPanelOrientationProperty, value); }
        }
        /// <summary>
        /// <see cref=""/> 註冊為依賴屬性
        /// </summary>
        public static readonly DependencyProperty StackPanelOrientationProperty =
            DependencyProperty.Register(nameof(StackPanelOrientation), typeof(Orientation), typeof(FunctionToggleButton), new PropertyMetadata(StackPanelOrientationPropertyChanged));
        /// <summary>
        /// <see cref=""/> 變更時觸發
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void StackPanelOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FunctionToggleButton menuControl = (d as FunctionToggleButton);
            menuControl.MainStackPanel.Orientation = (Orientation)e.NewValue;
        }



    }




    public class FunctionToggleButtonModel
    {
        public FunctionToggleButtonModel()
        {
            Orientation = Orientation.Vertical;
            ButtonContent = "Logo";
            ButtonTitle  = "test";
        }
        public Orientation Orientation { get; set; }
        public string ButtonContent { get; set; }
        public string ButtonTitle { get; set; }
        public bool ToggleButtonIsChecked { get; set; }
        public ICommand ToggleButtonICommand { get; set; }
    }

    public class FunctionToggleButtonViewModel: ViewModelBase
    {
        public FunctionToggleButtonViewModel()
        {

        }
        public FunctionToggleButtonViewModel(FunctionToggleButtonModel _functionToggleButton)
        {
            FunctionToggleButton = _functionToggleButton;
        }
        public FunctionToggleButtonModel FunctionToggleButton = new();


        public string ButtonContent { get => FunctionToggleButton.ButtonContent; set { FunctionToggleButton.ButtonContent = value; OnPropertyChanged(); } }
        public string ButtonTitle { get => FunctionToggleButton.ButtonTitle; set { FunctionToggleButton.ButtonTitle = value; OnPropertyChanged(); } }
        public bool ToggleButtonIsChecked { get => FunctionToggleButton.ToggleButtonIsChecked; set { FunctionToggleButton.ToggleButtonIsChecked = value; OnPropertyChanged(); } }
        public ICommand ToggleButtonICommand { get => FunctionToggleButton.ToggleButtonICommand; set { FunctionToggleButton.ToggleButtonICommand = value; OnPropertyChanged(); } }
        public FunctionToggleButtonControlBaseDropTarget FunctionToggleButtonDropTarget { get; set; } = new FunctionToggleButtonControlBaseDropTarget();
    }






}
