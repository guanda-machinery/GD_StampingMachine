using DevExpress.Xpf.WindowsUI;
using GD_CommonLibrary.Method;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GD_StampingMachine.Method
{
    public class MethodWinUIMessageBox : MessageBoxResultShow
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

        /// <summary>
        /// 儲存成功
        /// </summary>
        /// <param name="IsSuccessful"></param>
        public static void SaveSuccessful(string Path  ,bool IsSuccessful)
        {
            string _message = Path;
            if (IsSuccessful)
            {
                _message +=  "\r\n" +(string)Application.Current.TryFindResource("Text_SaveSuccessful");
            }
            else
            {
                _message +=  "\r\n" + (string)Application.Current.TryFindResource("Text_SaveFail");
            }
            Show(
                       (string)Application.Current.TryFindResource("Text_notify"),
                       _message,
                       MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void LoadSuccessful(string Path , bool IsSuccessful)
        {
            string _message = Path;
            if (IsSuccessful)
            {
                _message += "\r\n" + (string)Application.Current.TryFindResource("Text_LoadSuccessful");
            }
            else
            {
                _message += "\r\n" + (string)Application.Current.TryFindResource("Text_LoadFail");
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

        public static void ProjectIsExisted_CantOpenProject()
        {
            Show((string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_ProjectIsExistedCantOpenProject")
                , MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public static void ProjectIsLoaded()
        {
            Show((string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_ProjectIsLoaded")
                , MessageBoxButton.OK, MessageBoxImage.Warning);
        }



    }
}
