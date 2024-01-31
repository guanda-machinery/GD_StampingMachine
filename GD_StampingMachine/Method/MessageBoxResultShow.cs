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
        public MessageBoxResultShow(string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Image, bool lockWindow = true)
        {
            var mainWindow = Application.Current.MainWindow ?? new Window();
            newWindow = new MessageBoxWindow(mainWindow, MessageTitle, MessageString, MB_Button, MB_Image, lockWindow)
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
      /*  public Task<MessageBoxResult> ShowMessageBox()
        {
            MessageBoxResult messageBoxReturn = MessageBoxResult.None;
            Application.Current.Dispatcher.Invoke(new Action(async () =>
            {
                 messageBoxReturn = newWindow.FloatShow();
            }));
            return messageBoxReturn;
        }*/


        public MessageBoxResult ShowMessageBox()
        {
            MessageBoxResult messageBoxReturn = MessageBoxResult.None;
            messageBoxReturn = newWindow.FloatShow();
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



        public static Task<MessageBoxResult> ShowAsync(string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Image, bool lockWindow = true)
        {
            TaskCompletionSource<MessageBoxResult> tcs = new();
            MessageBoxResult MessageBoxReturn = MessageBoxResult.None;
           
            _ =Task.Run(async () =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        try
                        {
                            MessageBoxReturn = new MessageBoxResultShow(MessageTitle, MessageString, MB_Button, MB_Image, lockWindow).ShowMessageBox();
                            tcs.SetResult(MessageBoxReturn);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }
    


        
        public static Task<MessageBoxResult> ShowYesNoAsync(string MessageTitle, string MessageString, GD_MessageBoxNotifyResult boxNotify = GD_MessageBoxNotifyResult.NotifyBl , bool lockWindow = true)
        {
            return ShowAsync(MessageTitle, MessageString,
                MessageBoxButton.YesNo, boxNotify, lockWindow);
        }

        public static Task<MessageBoxResult> ShowOKAsync(string MessageTitle, string MessageString, GD_MessageBoxNotifyResult boxNotify , bool lockWindow = true)
        {
           return ShowAsync(MessageTitle, MessageString,
                MessageBoxButton.OK, boxNotify , lockWindow);
        }

        public static Task<MessageBoxResult> ShowOKCancelAsync(string MessageTitle, string MessageString, GD_MessageBoxNotifyResult boxNotify, bool lockWindow = true)
        {
            return ShowAsync(MessageTitle, MessageString,
                MessageBoxButton.OKCancel, boxNotify, lockWindow);
        }


        public static async Task ShowExceptionAsync(Exception ex , bool lockWindow = true)
        {
            await ShowAsync(
                         (string)Application.Current.TryFindResource("Text_notify"),
                         ex.Message,
                         MessageBoxButton.OK, GD_MessageBoxNotifyResult.NotifyRd, lockWindow);
        }
    }
}
