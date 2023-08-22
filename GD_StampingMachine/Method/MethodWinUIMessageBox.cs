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
            var MessageBoxReturn = ShowYesNo(
                (string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_SettingAskOverwrite"));
            return MessageBoxReturn == MessageBoxResult.Yes;
        }

        public static bool AskDelProject(string NumberSetting)
        {
            var MessageBoxReturn = ShowYesNo(
                (string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_AskDelProject") +
                "\r\n" +
                $"{NumberSetting}" +
                "?");
            return MessageBoxReturn == MessageBoxResult.Yes;
        }
        /// <summary>
        /// 詢問關閉專案
        /// </summary>
        /// <param name="NumberSetting"></param>
        /// <returns></returns>
        public static bool AskCloseProject(string ProjectName)
        {
            var MessageBoxReturn = ShowYesNo(
                (string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_TSProjectExistedInBox") + 
                "\r\n"+
                (string)Application.Current.TryFindResource("Text_CloseTSProject") +
                "\r\n" +
                $"{ProjectName}" +
                "?");

            return MessageBoxReturn == MessageBoxResult.Yes;
        }

        /// <summary>
        /// 儲存成功
        /// </summary>
        /// <param name="IsSuccessful"></param>
        public static void SaveSuccessful(string Path  ,bool IsSuccessful)
        {
            string _message = string.Empty;
            if (!string.IsNullOrEmpty(Path))
            {
                _message += Path + "\r\n";
            }

            if (IsSuccessful)
            {
                _message += (string)Application.Current.TryFindResource("Text_SaveSuccessful");
            }
            else
            {
                _message +=  (string)Application.Current.TryFindResource("Text_SaveFail");
            }
            ShowOK(
                       (string)Application.Current.TryFindResource("Text_notify"),
                       _message);
        }
        /// <summary>
        /// 讀取成功
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="IsSuccessful"></param>
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
            ShowOK(
                       (string)Application.Current.TryFindResource("Text_notify"),
                       _message);
        }



        public static void CanNotCloseProject()
        {
            ShowOK((string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_CantCloseTSProject"));
        }

        public static void CanNotDeleteProject()
        {
            ShowOK((string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_CantDelTSProject"));
        }



        public static void ProjectIsExisted_CantOpenProject()
        {
            ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_ProjectIsExistedCantOpenProject")
                , MessageBoxImage.Warning);
        }


        /// <summary>
        /// 無法建立專案
        /// </summary>
        public static void CanNotCreateProjectFileNameIsEmpty()
        {
            ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_ProjectNameCantBeEmpty") 
                , MessageBoxImage.Warning);
        }

        /// <summary>
        /// 無法建立專案
        /// </summary>
        public static void CanNotCreateProject(string ProjectName =null)
        {
            string AddMessage = "";
            if (ProjectName != null)
                AddMessage = "\r\n" + ProjectName;

            ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_ProjectIsExistedCantCreateProject")+ AddMessage
                , MessageBoxImage.Warning);
        }



        public static void ProjectIsLoaded()
        {
            ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_ProjectIsLoaded")
                , MessageBoxImage.Warning);
        }





    }
}
