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
        public static MessageBoxResult Show(string MessageTitle, string MessageString, MessageBoxButton MB_Button, MessageBoxImage MB_Image)
        {
            var MessageBoxReturn = WinUIMessageBox.Show(null,
                MessageString,
                MessageTitle,
                MB_Button,
               MB_Image,
                MessageBoxResult.None,
                MessageBoxOptions.None,
                DevExpress.Xpf.Core.FloatingMode.Window);
            return MessageBoxReturn;
        }
        public static void ShowException(Exception ex)
        {
            Show(
                       (string)Application.Current.TryFindResource("Text_notify"),
                       ex.Message,
                       MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
