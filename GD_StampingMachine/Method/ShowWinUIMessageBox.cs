using DevExpress.Xpf.WindowsUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GD_StampingMachine.Method
{
    public static class MethodWinUIMessageBox
    {

        public static bool AskOverwriteOrNot()
        {
            var MessageBoxReturn = MethodWinUIMessageBox.Show(
                (string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_SettingAskOverwrite"),
                MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (MessageBoxReturn == MessageBoxResult.Yes)
                return true;
            return false;
        }

        public static void SaveSuccessful(bool IsSuccessful)
        {
            string _message;
            if (IsSuccessful)
            {
                _message = (string)Application.Current.TryFindResource("Text_SaveSuccessful");
            }
            else
            {
                _message = (string)Application.Current.TryFindResource("Text_SaveFail");
            }
            MethodWinUIMessageBox.Show(
                       (string)Application.Current.TryFindResource("Text_notify"),
                       _message,
                       MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public static MessageBoxResult Show(string MessageTitle , string MessageString, MessageBoxButton MB_Button , MessageBoxImage MB_Image)
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

    }
}
