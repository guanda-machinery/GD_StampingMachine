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
        protected static MessageBoxResult Show(string MessageTitle, string MessageString, MessageBoxButton MB_Button, MessageBoxImage MB_Image)
        {
            MessageBoxResult MessageBoxReturn = MessageBoxResult.None;
           
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MessageBoxReturn = WinUIMessageBox.Show(new Window(), MessageString,
               MessageTitle,
               MB_Button,
               MB_Image,
               MessageBoxResult.None,
               MessageBoxOptions.None,
               DevExpress.Xpf.Core.FloatingMode.Window);
            })).Wait();
            return MessageBoxReturn;
        }

        public static MessageBoxResult ShowYesNo(string MessageTitle, string MessageString, MessageBoxImage BoxImage = MessageBoxImage.Information)
        {
            return Show(MessageTitle, MessageString,
                MessageBoxButton.YesNo, BoxImage);
        }

        public static void ShowOK(string MessageTitle, string MessageString , MessageBoxImage BoxImage = MessageBoxImage.Information)
        {
             Show(MessageTitle, MessageString,
                MessageBoxButton.OK, BoxImage);
        }

        public static void ShowException(Exception ex)
        {
            Show(
                         (string)Application.Current.TryFindResource("Text_notify"),
                         ex.Message,
                         MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
