using DevExpress.CodeParser;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Windows;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GD_CommonLibrary.Method
{
    public class MessageBoxResultShow
    {
        /*public static MessageBoxResult Show(string MessageTitle, string MessageString, MessageBoxButton MB_Button, MessageBoxImage MB_Image)
        {
            MessageBoxResult messageBoxReturn = MessageBoxResult.None;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Window newWindow = Application.Current.MainWindow ?? new()
                {
                    Topmost = true,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                messageBoxReturn = DevExpress.Xpf.WindowsUI.WinUIMessageBox.Show(newWindow, MessageString,
                    MessageTitle, MB_Button, MB_Image, MessageBoxResult.None, MessageBoxOptions.None, DevExpress.Xpf.Core.FloatingMode.Window);
            }));

            return messageBoxReturn;
        }
        */
        /*public static async Task<MessageBoxResult> ShowAsync(string MessageTitle, string MessageString, MessageBoxButton MB_Button, MessageBoxImage MB_Image)
{
    MessageBoxResult MessageBoxReturn = MessageBoxResult.None;
    await Application.Current.Dispatcher.InvokeAsync(new Action(() =>
    {
        MessageBoxReturn = Show(MessageTitle, MessageString, MB_Button, MB_Image);
    }));
    return MessageBoxReturn;
}*/


        public static MessageBoxResult Show(string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Image)
        {
            MessageBoxResult messageBoxReturn = MessageBoxResult.None;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageBoxWindow newWindow =  new MessageBoxWindow()
                {
                    WindowStartupLocation= WindowStartupLocation.CenterOwner,
                };

                var mainWindow = Application.Current.MainWindow ?? new Window();

                messageBoxReturn =  newWindow.FloatShow(mainWindow, MessageTitle, MessageString, MB_Button, MB_Image);
            }));
            return messageBoxReturn;
        }



        public static async Task<MessageBoxResult> ShowAsync(string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Image)
        {
            MessageBoxResult MessageBoxReturn = MessageBoxResult.None;
            await Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                MessageBoxReturn = Show(MessageTitle, MessageString, MB_Button, MB_Image);
            }));
            return MessageBoxReturn;
        }


        
        public static async Task<MessageBoxResult> ShowYesNoAsync(string MessageTitle, string MessageString, GD_MessageBoxNotifyResult boxNotify = GD_MessageBoxNotifyResult.NotifyBl)
        {
            return await ShowAsync(MessageTitle, MessageString,
                MessageBoxButton.YesNo, boxNotify);
        }

        public static async Task ShowOKAsync(string MessageTitle, string MessageString, GD_MessageBoxNotifyResult boxNotify)
        {
            await ShowAsync(MessageTitle, MessageString,
                MessageBoxButton.OK, boxNotify);
        }

        public static async Task ShowExceptionAsync(Exception ex)
        {
            await ShowAsync(
                         (string)Application.Current.TryFindResource("Text_notify"),
                         ex.Message,
                         MessageBoxButton.OK, GD_MessageBoxNotifyResult.NotifyRd);
        }
    }
}
