using DevExpress.CodeParser;
using DevExpress.XtraScheduler;
using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace GD_StampingMachine.Windows
{


    /// <summary>
    /// MessageBoxWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MessageBoxWindow : Window
    {
        public MessageBoxWindow()
        {
            InitializeComponent();
        }


        public MessageBoxResult Result { get; set; } = MessageBoxResult.None;





        public MessageBoxResult FloatShow(Window owner, string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Icon , bool lockWindow = true)
        {
            Window overlay = null;
            if (lockWindow && owner != null)
            {
                overlay = new Window
                {
                    Width = owner.ActualWidth,
                    Height = owner.ActualHeight,
                    Background = Brushes.Gray,

                    Left = owner.Left,
                    Top = owner.Top,
                    WindowStyle = WindowStyle.None,
                    AllowsTransparency = true,
                    Opacity = 0.5,
                    Owner = owner,
                    ShowInTaskbar = false
                };
            }
            overlay?.Show();

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


            OkButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
            YesButton.Visibility = Visibility.Collapsed;
            NoButton.Visibility = Visibility.Collapsed;
            switch (MB_Button)
            {
                case MessageBoxButton.OK:
                    OkButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                    OkButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    CancelButton.Visibility = Visibility.Visible;
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }

            this.Owner = overlay;
            this.Title = MessageTitle ?? string.Empty;
            this.ContentTextBlock.Text = MessageString ?? string.Empty;

            this.ShowDialog();

            overlay?.Close();
            return Result;
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
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            this.Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            this.Close();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Result = MessageBoxResult.None;
                this.Close();
            }

            /*if (e.Key == Key.Y)
            {
                Result = MessageBoxResult.Yes;
                this.Close();
            }*/




        }
    }
}
