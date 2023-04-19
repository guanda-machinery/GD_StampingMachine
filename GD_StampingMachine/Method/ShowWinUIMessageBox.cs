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
            var MessageBoxReturn = Show(
                (string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_SettingAskOverwrite"),
                MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (MessageBoxReturn == MessageBoxResult.Yes)
                return true;
            return false;
        }

        public static bool AskDelProject(string NumberSetting)
        {
            var MessageBoxReturn = Show(
                (string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_AskDelProject") +
                "\r\n" +
                $"{NumberSetting}" +
                "?"
                ,
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);

            return MessageBoxReturn == MessageBoxResult.Yes;
        }
        /// <summary>
        /// 詢問關閉專案
        /// </summary>
        /// <param name="NumberSetting"></param>
        /// <returns></returns>
        public static bool AskCloseProject(string ProjectName)
        {
            var MessageBoxReturn = Show(
                (string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_CloseTSProject") +
                "\r\n" +
                $"{ProjectName}" +
                "?"
                ,
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);

            return MessageBoxReturn == MessageBoxResult.Yes;
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
            Show(
                       (string)Application.Current.TryFindResource("Text_notify"),
                       _message,
                       MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void CanNotCloseProject()
        {
            Show((string)Application.Current.TryFindResource("Text_notify"), 
                (string)Application.Current.TryFindResource("Text_CantCloseTSProject")
                , MessageBoxButton.OK , MessageBoxImage.Warning);
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
