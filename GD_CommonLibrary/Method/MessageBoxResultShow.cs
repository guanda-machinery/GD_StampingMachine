using DevExpress.Xpf.WindowsUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace GD_CommonLibrary.Method
{
    public class MessageBoxResultShow
    {
        public static async Task<MessageBoxResult> Show(string MessageTitle, string MessageString, MessageBoxButton MB_Button, MessageBoxImage MB_Image)
        {
            MessageBoxResult MessageBoxReturn = MessageBoxResult.None;
            var NewWindow = new Window();
            NewWindow.Topmost = true;

            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MessageBoxReturn = WinUIMessageBox.Show(NewWindow, MessageString,
               MessageTitle,
               MB_Button,
               MB_Image,
               MessageBoxResult.None,
               MessageBoxOptions.None,
               DevExpress.Xpf.Core.FloatingMode.Window);
            }));
            return MessageBoxReturn;
        }



        public static async Task<MessageBoxResult> ShowYesNo(string MessageTitle, string MessageString, MessageBoxImage BoxImage = MessageBoxImage.Information)
        {
            return await Show(MessageTitle, MessageString,
                MessageBoxButton.YesNo, BoxImage);
        }

        public static async Task ShowOK(string MessageTitle, string MessageString , MessageBoxImage BoxImage = MessageBoxImage.Information)
        {
            await Show(MessageTitle, MessageString,
                MessageBoxButton.OK, BoxImage);
        }

        public static async Task ShowException(Exception ex)
        {
            await Show(
                         (string)Application.Current.TryFindResource("Text_notify"),
                         ex.Message,
                         MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
