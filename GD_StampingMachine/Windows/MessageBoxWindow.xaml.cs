using DevExpress.Xpf.Core;
using GD_StampingMachine.GD_Enum;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

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
            MessageBoxNotifyResultProperty = DependencyProperty.Register(nameof(MessageBoxNotifyResult), typeof(GD_MessageBoxNotifyResult), typeof(MessageBoxWindow), new FrameworkPropertyMetadata(GD_MessageBoxNotifyResult.NotifyBl, OnMessageBoxNotifyResultChanged));



        }

    


        public MessageBoxWindow(Window parent, string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Icon, bool lockWindow = true)
        {
            InitializeComponent();

            BoxButton = MB_Button;
            OkButtonGrid.Visibility = BoxButton is MessageBoxButton.OK ? Visibility.Visible : Visibility.Collapsed;
            OkCancelButtonGrid.Visibility = BoxButton is MessageBoxButton.OKCancel ? Visibility.Visible : Visibility.Collapsed;
            YesNoButtonGrid.Visibility = BoxButton is MessageBoxButton.YesNo ? Visibility.Visible : Visibility.Collapsed;
            YesNoCancelButtonGrid.Visibility = BoxButton is MessageBoxButton.YesNoCancel ? Visibility.Visible : Visibility.Collapsed;

            MessageBoxNotifyResult = MB_Icon;
            Notify_Bl_Image.Visibility = MessageBoxNotifyResult is GD_MessageBoxNotifyResult.NotifyBl ? Visibility.Visible : Visibility.Collapsed;
            Notify_Gr_Image.Visibility = MessageBoxNotifyResult is GD_MessageBoxNotifyResult.NotifyGr ? Visibility.Visible : Visibility.Collapsed;
            Notify_Rd_Image.Visibility = MessageBoxNotifyResult is GD_MessageBoxNotifyResult.NotifyRd ? Visibility.Visible : Visibility.Collapsed;
            Notify_Ye_Image.Visibility = MessageBoxNotifyResult is GD_MessageBoxNotifyResult.NotifyYe ? Visibility.Visible : Visibility.Collapsed;

            // Parent = parent;

            parent ??= Application.Current.MainWindow ?? new Window();




           if (lockWindow)
            {
                if (parent.Visibility is not Visibility.Visible || parent.Opacity < 0.3)
                {

                }
                else
                {


                    Overlay = new Window
                    {
                       // Placement= PlacementMode.Relative,
                       // PlacementTarget= Owner,
                        //IsTopmost = false,
                        //AllowTopMost = false,
                        //WindowStartupLocation = WindowStartupLocation.Manual ,
                        WindowStyle= parent.WindowStyle,
                        WindowState = parent.WindowState,
                        //StaysOpen = true,
                        //IsOpen = true,
                        AllowsTransparency = parent.AllowsTransparency,
                        Top = parent.Top,
                        Left = parent.Left,
                        //PlacementRectangle = new Rect(owner.Left, owner.Top, owner.ActualWidth, owner.ActualHeight),
                         Width = parent.ActualWidth,
                        Height = parent.ActualHeight,

                        Owner = parent,
                        Opacity = 0.5,
                        //Background = Brushes.Gray,
                        ShowInTaskbar =false,
                        Content= new Border()
                        {
                            Background = Brushes.Gray,
                            Opacity= 0.5,
                        }
                    };

                    Overlay.Closed += delegate (object sender, EventArgs e)
                    {
                        Overlay.Owner?.Focus();
                    };


                }


            }

            /*OkButtonGrid.Visibility = MB_Button is MessageBoxButton.OK ? Visibility.Visible : Visibility.Collapsed;
            OkCancelButtonGrid.Visibility = MB_Button is MessageBoxButton.OKCancel ? Visibility.Visible : Visibility.Collapsed;
            YesNoButtonGrid.Visibility = MB_Button is MessageBoxButton.YesNo ? Visibility.Visible : Visibility.Collapsed;
            YesNoCancelButtonGrid.Visibility = MB_Button is MessageBoxButton.YesNoCancel ? Visibility.Visible : Visibility.Collapsed;*/

            // var ZindexMax = Application.Current.Windows.Cast<Window>().Max(window => Panel.GetZIndex(window));
            //   this.SetValue(Panel.ZIndexProperty, ZindexMax - 1);
            //this.Topmost = true;
           // this.Owner = owner;
            this.Title = MessageTitle ?? string.Empty;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.ContentTextBlock.Text = MessageString ?? string.Empty;
        }



        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CloseMessageBox();
        }

        static readonly DependencyProperty BoxButtonProperty;
        static readonly DependencyProperty MessageBoxNotifyResultProperty; 
        


        public MessageBoxButton BoxButton
        {
            get => (MessageBoxButton)GetValue(BoxButtonProperty);
            set => SetValue(BoxButtonProperty, value);
        }

        public GD_MessageBoxNotifyResult MessageBoxNotifyResult
        {
            get => (GD_MessageBoxNotifyResult)GetValue(MessageBoxNotifyResultProperty);
            set => SetValue(MessageBoxNotifyResultProperty, value);
        }


        private static void OnBoxButtonChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var thisobj = (MessageBoxWindow)obj;
            if( e.NewValue is MessageBoxButton boxButton)
            {
                thisobj.OkButtonGrid.Visibility = boxButton is MessageBoxButton.OK ? Visibility.Visible : Visibility.Collapsed;
                thisobj.OkCancelButtonGrid.Visibility = boxButton is MessageBoxButton.OKCancel ? Visibility.Visible : Visibility.Collapsed;
                thisobj.YesNoButtonGrid.Visibility = boxButton is MessageBoxButton.YesNo ? Visibility.Visible : Visibility.Collapsed;
                thisobj.YesNoCancelButtonGrid.Visibility = boxButton is MessageBoxButton.YesNoCancel ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private static void OnMessageBoxNotifyResultChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var thisobj = (MessageBoxWindow)obj;
            if (e.NewValue is GD_MessageBoxNotifyResult Notify)
            {

                thisobj.Notify_Bl_Image.Visibility = Notify is GD_MessageBoxNotifyResult.NotifyBl ? Visibility.Visible : Visibility.Collapsed;
                thisobj.Notify_Gr_Image.Visibility = Notify is GD_MessageBoxNotifyResult.NotifyGr ? Visibility.Visible : Visibility.Collapsed;
                thisobj.Notify_Rd_Image.Visibility = Notify is GD_MessageBoxNotifyResult.NotifyRd ? Visibility.Visible : Visibility.Collapsed;
                thisobj.Notify_Ye_Image.Visibility = Notify is GD_MessageBoxNotifyResult.NotifyYe ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        



        private Window CalcOwner()
        {
            Window frameworkElement = null;
            if (Application.Current != null && Application.Current.Dispatcher.CheckAccess())
            {
                foreach (Window window3 in Application.Current.Windows)
                {
                    if (window3.IsActive && window3.Background != Brushes.Transparent)
                    {
                        frameworkElement = window3;
                        break;
                    }
                }

                if (frameworkElement == null && Application.Current.Windows.Count > 0)
                {
                    Window window2 = Application.Current.Windows[0];
                    if (window2.Background != Brushes.Transparent)
                    {
                        frameworkElement = window2;
                    }
                }
            }

            return frameworkElement;
        }



        public MessageBoxResult Result { get; private set; }

        //readonly Window Parent = null;
        readonly Window Overlay = null;

        public MessageBoxResult FloatShow()
        {
            //   this.Focus();
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (Overlay != null)
                {
                    Overlay.Show();
                    this.Owner = Overlay;
                }
                else
                {
                    this.Topmost = true;
                }
                this.ShowDialog();
                this.CloseMessageBox();
            }));

           return Result;
        }

        private void CloseMessageBox()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
                Overlay?.Close();
          /*      if (overlay != null)
                    overlay.IsOpen = false;*/
            }));
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
            CloseMessageBox();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            CloseMessageBox();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            CloseMessageBox();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            CloseMessageBox();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK; 
            CloseMessageBox();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Result = MessageBoxResult.None;
                CloseMessageBox();
            }
            else if (e.Key == Key.Y)
            {
                if (BoxButton is MessageBoxButton.YesNoCancel || BoxButton is MessageBoxButton.YesNo)
                {
                    Result = MessageBoxResult.Yes;
                    CloseMessageBox();
                }
            }
            else if (e.Key == Key.N)
            {
                if (BoxButton is MessageBoxButton.YesNoCancel || BoxButton is MessageBoxButton.YesNo)
                {
                    Result = MessageBoxResult.No;
                    CloseMessageBox();
                }
            }
            else if (e.Key == Key.C)
            {
                if (BoxButton is MessageBoxButton.OKCancel || BoxButton is MessageBoxButton.YesNoCancel)
                {
                    Result = MessageBoxResult.Cancel;
                    CloseMessageBox();
                }
            }


        }
    }
}
