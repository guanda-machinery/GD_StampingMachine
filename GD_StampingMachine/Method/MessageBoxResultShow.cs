
using GD_StampingMachine;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Windows;
using Microsoft.VisualStudio.Threading;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.VisualStudio.Shell;

namespace GD_CommonLibrary.Method
{
    /// <summary>
    /// 彈出式視窗
    /// </summary>
    public class MessageBoxResultShow
    {

        public MessageBoxResultShow(Window? parent, string? messageTitle, string? messageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Image, bool lockWindow = true)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                newWindow = new MessageBoxWindow(parent, 
                    messageTitle, messageString, MB_Button, MB_Image, lockWindow)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };
            });
        }

        MessageBoxWindow? newWindow;

        /// <summary>
        /// 可被外部關閉的彈出式視窗
        /// </summary>
        /// <param name="MessageTitle"></param>
        /// <param name="MessageString"></param>
        /// <param name="MB_Button"></param>
        /// <param name="MB_Image"></param>
        /// <returns></returns>
      /*  public async Task<MessageBoxResult> ShowMessageBox()
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
            MessageBoxResult messageBoxReturn = newWindow!.FloatShow();
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
                try
                {
                    newWindow?.Close();
                }
                catch
                {

                }
            }));
        }


        public static MessageBoxResult Show(Window? Parent, string? MessageTitle, string? MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Image, bool lockWindow = true)
        {
            return new MessageBoxResultShow(Parent, MessageTitle, MessageString, MB_Button, MB_Image, lockWindow).ShowMessageBox();
        }
        public static async Task<MessageBoxResult> ShowAsync(Window? Parent, string? MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Image, bool lockWindow = true)
        {
            TaskCompletionSource<MessageBoxResult> tcs = new();
            MessageBoxResult MessageBoxReturn = MessageBoxResult.None;
           
            _ =Task.Run(async () =>
            {
                try
                {
                    await Application.Current.Dispatcher.InvokeAsync(new Action( () =>
                    {
                        try
                        {
                            MessageBoxReturn = Show(Parent, MessageTitle, MessageString, MB_Button, MB_Image, lockWindow);
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
            return await tcs.Task;
        }
    


        
        public static Task<MessageBoxResult> ShowYesNoAsync(Window? Parent, string? MessageTitle, string MessageString, GD_MessageBoxNotifyResult boxNotify = GD_MessageBoxNotifyResult.NotifyBl , bool lockWindow = true)
        {
            return ShowAsync(Parent, MessageTitle, MessageString,
                MessageBoxButton.YesNo, boxNotify, lockWindow);
        }

        public static Task<MessageBoxResult> ShowYesNoCancelAsync(Window? Parent, string? MessageTitle, string MessageString, GD_MessageBoxNotifyResult boxNotify = GD_MessageBoxNotifyResult.NotifyBl, bool lockWindow = true)
        {
            return ShowAsync(Parent, MessageTitle, MessageString,
                MessageBoxButton.YesNoCancel, boxNotify, lockWindow);
        }


        public static Task<MessageBoxResult> ShowOKAsync(Window? Parent, string MessageTitle, string MessageString, GD_MessageBoxNotifyResult boxNotify , bool lockWindow = true)
        {
           return ShowAsync(Parent, MessageTitle, MessageString,
                MessageBoxButton.OK, boxNotify , lockWindow);
        }

        public static Task<MessageBoxResult> ShowOKCancelAsync(Window? Parent, string MessageTitle, string MessageString, GD_MessageBoxNotifyResult boxNotify, bool lockWindow = true)
        {
            return ShowAsync(Parent, MessageTitle,MessageString,
                MessageBoxButton.OKCancel, boxNotify, lockWindow);
        }


        public static async Task ShowExceptionAsync(Window? Parent, Exception ex , bool lockWindow = true)
        {
            await ShowAsync(Parent,
                         (string)Application.Current.TryFindResource("Text_notify"),
                         ex.Message,
                         MessageBoxButton.OK, GD_MessageBoxNotifyResult.NotifyRd, lockWindow);
        }
    }
}
