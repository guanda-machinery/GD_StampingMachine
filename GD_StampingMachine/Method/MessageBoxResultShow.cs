using DevExpress.CodeParser;
using GD_StampingMachine;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Windows;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GD_CommonLibrary.Method
{
    /// <summary>
    /// 彈出式視窗
    /// </summary>
    public class MessageBoxResultShow
    {
        public MessageBoxResultShow(string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Image)
        {
            var mainWindow = Application.Current.MainWindow ?? new Window();
            newWindow = new MessageBoxWindow(mainWindow, MessageTitle, MessageString, MB_Button, MB_Image)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
        }
        readonly MessageBoxWindow newWindow;
        /// <summary>
        /// 可被外部關閉的彈出式視窗
        /// </summary>
        /// <param name="MessageTitle"></param>
        /// <param name="MessageString"></param>
        /// <param name="MB_Button"></param>
        /// <param name="MB_Image"></param>
        /// <returns></returns>
        public MessageBoxResult ShowMessageBox()
        {
            MessageBoxResult messageBoxReturn = MessageBoxResult.None;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                messageBoxReturn = newWindow.FloatShow();
            }));
            return messageBoxReturn;
        }


        public async Task<MessageBoxResult> ShowMessageBoxAsync()
        {
            MessageBoxResult messageBoxReturn = MessageBoxResult.None;
            await Task.Run(() =>
            {
                messageBoxReturn = ShowMessageBox();
            });
            return messageBoxReturn;
        }




        /// <summary>
        /// 關閉彈出式視窗
        /// </summary>
        /// <param name="MessageTitle"></param>
        /// <param name="MessageString"></param>
        /// <param name="MB_Button"></param>
        /// <param name="MB_Image"></param>
        /// <returns></returns>
        public void CloseMessageBox()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                newWindow?.Close();
            }));
        }
        public async Task CloseMessageBoxAsync()
        {
            await Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                newWindow?.Close();
            }));


        }



        public static MessageBoxResult Show(string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Image)
        {
            return new MessageBoxResultShow(MessageTitle, MessageString, MB_Button, MB_Image).ShowMessageBox();
        }

        public static async Task<MessageBoxResult> ShowAsync(string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Image)
        {
            MessageBoxResult MessageBoxReturn= MessageBoxResult.None;
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
