using GD_StampingMachine.GD_Enum;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GD_StampingMachine.Windows
{


    /// <summary>
    /// MessageBoxWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MessageBoxWindow : Window
    {

        static MessageBoxWindow ()
        {
            BoxButtonProperty = DependencyProperty.Register(nameof(BoxButton), typeof(MessageBoxButton), typeof(MessageBoxWindow), new FrameworkPropertyMetadata(MessageBoxButton.OK, OnBoxButtonChanged));
        }
        public MessageBoxWindow(Window owner, string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Icon, bool lockWindow = true)
        {
            InitializeComponent();
            BoxButton= MB_Button;

            if (lockWindow && owner != null)
            {
                var ownerOriginTopmost = owner.Topmost;
                owner.Topmost = true;
                
                overlay = new Window
                {
                    Width = owner.ActualWidth,
                    Height = owner.ActualHeight,
                    Background = Brushes.Gray,
                    WindowState = owner.WindowState,
                    Left = owner.Left,
                    Top = owner.Top,
                    WindowStyle = WindowStyle.None,
                    AllowsTransparency = true,
                    Opacity = 0.5,
                    Owner = owner,
                    ShowInTaskbar = false
                };
                overlay?.Show();
                owner.Topmost = ownerOriginTopmost;
            }
            Notify_Bl_Image.Visibility = Visibility.Collapsed; ;
            Notify_Rd_Image.Visibility = Visibility.Collapsed; ;
            Notify_Gr_Image.Visibility = Visibility.Collapsed; ;
            Notify_Ye_Image.Visibility = Visibility.Collapsed; ;
            switch (MB_Icon)
            {
                case GD_MessageBoxNotifyResult.NotifyBl:
                    Notify_Bl_Image.Visibility = Visibility.Visible;
                    break;
                case GD_MessageBoxNotifyResult.NotifyGr:
                    Notify_Gr_Image.Visibility = Visibility.Visible;
                    break;
                case GD_MessageBoxNotifyResult.NotifyRd:

                    Notify_Rd_Image.Visibility = Visibility.Visible;
                    break;
                case GD_MessageBoxNotifyResult.NotifyYe:

                    Notify_Ye_Image.Visibility = Visibility.Visible;
                    break;


                default:
                    break;
            }

            OkButtonGrid.Visibility = MB_Button.HasFlag(MessageBoxButton.OK) ? Visibility.Visible : Visibility.Collapsed;
            OkCancelButtonGrid.Visibility = MB_Button.HasFlag(MessageBoxButton.OKCancel) ? Visibility.Visible : Visibility.Collapsed;
            YesNoButtonGrid.Visibility = MB_Button.HasFlag(MessageBoxButton.YesNo) ? Visibility.Visible : Visibility.Collapsed;
            YesNoCancelButtonGrid.Visibility = MB_Button.HasFlag(MessageBoxButton.YesNoCancel) ? Visibility.Visible : Visibility.Collapsed;

            this.Owner = overlay ?? new Window();
            this.Title = MessageTitle ?? string.Empty;
            this.ContentTextBlock.Text = MessageString ?? string.Empty;
        }

        static readonly DependencyProperty BoxButtonProperty;


        public MessageBoxButton BoxButton
        {
            get => (MessageBoxButton)GetValue(BoxButtonProperty);
            set => SetValue(BoxButtonProperty, value);
        }

        private static void OnBoxButtonChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var thisobj = (MessageBoxWindow)obj;
            if( e.NewValue is MessageBoxButton boxButton)
            {
                thisobj.OkButtonGrid.Visibility = boxButton.HasFlag(MessageBoxButton.OK) ? Visibility.Visible : Visibility.Collapsed;
                thisobj.OkCancelButtonGrid.Visibility = boxButton.HasFlag(MessageBoxButton.OKCancel) ? Visibility.Visible : Visibility.Collapsed;
                thisobj.YesNoButtonGrid.Visibility = boxButton.HasFlag(MessageBoxButton.YesNo) ? Visibility.Visible : Visibility.Collapsed;
                thisobj.YesNoCancelButtonGrid.Visibility = boxButton.HasFlag(MessageBoxButton.YesNoCancel) ? Visibility.Visible : Visibility.Collapsed;
            }
        }



        public MessageBoxResult Result { get; private set; }


        readonly Window overlay = null;

        public MessageBoxResult FloatShow()
        {
            this.ShowDialog();
            this.CloseMessageBox();
            return Result;
        }

        private void CloseMessageBox()
        {
            this.Close();
            overlay?.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                this.DragMove();
            }
        }





        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.None;

            this.CloseMessageBox();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;

            this.CloseMessageBox();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;

            this.CloseMessageBox();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            this.CloseMessageBox();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            this.CloseMessageBox();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Result = MessageBoxResult.None;
                this.CloseMessageBox();
            }
            else if (e.Key == Key.Y)
            {
                if (BoxButton is MessageBoxButton.YesNoCancel || BoxButton is MessageBoxButton.YesNo)
                {
                    Result = MessageBoxResult.Yes;
                    this.Close();
                }
            }
            else if (e.Key == Key.N)
            {
                if (BoxButton is MessageBoxButton.YesNoCancel || BoxButton is MessageBoxButton.YesNo)
                {
                    Result = MessageBoxResult.No;
                    this.Close();
                }
            }
            else if (e.Key == Key.C)
            {
                if (BoxButton is MessageBoxButton.OKCancel || BoxButton is MessageBoxButton.YesNoCancel)
                {
                    Result = MessageBoxResult.Cancel;
                    this.Close();
                }
            }


        }
    }
}
