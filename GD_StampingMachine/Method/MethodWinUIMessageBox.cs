using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Windows;
using System.Threading.Tasks;
using System.Windows;

namespace GD_StampingMachine.Method
{
    public class MethodWinUIMessageBox : MessageBoxResultShow
    {
        protected MethodWinUIMessageBox(string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxNotifyResult MB_Image, bool lockWindow = true) : base(MessageTitle, MessageString, MB_Button, MB_Image, lockWindow)
        {

        }

        public static async Task<MessageBoxResult> AskOverwriteOrNotAsync()
        {
            var MessageBoxReturn = ShowYesNoAsync(
                (string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_SettingAskOverwrite"));
            return await MessageBoxReturn;
        }

        public static async Task<MessageBoxResult> AskDelProjectAsync(string NumberSetting)
        {
            var MessageBoxReturn = ShowYesNoAsync(
                (string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_AskDelProject") +
                "\r\n" +
                $"{NumberSetting}" +
                "?", GD_MessageBoxNotifyResult.NotifyBl);
            return await MessageBoxReturn;
        }
        /// <summary>
        /// 詢問關閉專案
        /// </summary>
        /// <param name="NumberSetting"></param>
        /// <returns></returns>
        public static async Task<MessageBoxResult> AskCloseProjectAsync(string ProjectName)
        {
            var MessageBoxReturn = ShowYesNoAsync(
                (string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_TSProjectExistedInBox") +
                "\r\n" +
                (string)Application.Current.TryFindResource("Text_CloseTSProject") +
                "\r\n" +
                $"{ProjectName}" +
                "?", GD_MessageBoxNotifyResult.NotifyBl);

            return await MessageBoxReturn;
        }

        /// <summary>
        /// 儲存成功
        /// </summary>
        /// <param name="IsSuccessful"></param>
        public static async Task SaveSuccessfulAsync(string Path, bool IsSuccessful)
        {
            string _message = string.Empty;
            if (!string.IsNullOrEmpty(Path))
            {
                _message += Path + "\r\n";
            }

            if (IsSuccessful)
            {
                _message += (string)Application.Current.TryFindResource("Text_SaveSuccessful");
                await ShowOKAsync(
                           (string)Application.Current.TryFindResource("Text_notify"),
                           _message, GD_MessageBoxNotifyResult.NotifyGr);
            }
            else
            {
                _message += (string)Application.Current.TryFindResource("Text_SaveFail");

                await ShowOKAsync(
                           (string)Application.Current.TryFindResource("Text_notify"),
                           _message, GD_MessageBoxNotifyResult.NotifyRd);
            }
        }
        /// <summary>
        /// 讀取成功
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="IsSuccessful"></param>
        public static async Task LoadSuccessfulAsync(string Path, bool IsSuccessful)
        {
            GD_MessageBoxNotifyResult notify;

            string _message = Path;
            if (IsSuccessful)
            {
                _message += "\r\n" + (string)Application.Current.TryFindResource("Text_LoadSuccessful");
                notify = GD_MessageBoxNotifyResult.NotifyGr;
            }
            else
            {
                _message += "\r\n" + (string)Application.Current.TryFindResource("Text_LoadFail");
                notify = GD_MessageBoxNotifyResult.NotifyRd;
            }

            await ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
           _message, notify);

        }



        public static async Task CanNotCloseProjectAsync()
        {
            await ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_CantCloseTSProject") , GD_MessageBoxNotifyResult.NotifyYe);
        }

        public static async Task CanNotDeleteProjectAsync()
        {
            await ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_CantDelTSProject"), GD_MessageBoxNotifyResult.NotifyYe);
        }



        public static async Task ProjectIsExisted_CantOpenProjectAsync()
        {
            await ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_ProjectIsExistedCantOpenProject")
                , GD_MessageBoxNotifyResult.NotifyYe);
        }


        /// <summary>
        /// 無法建立專案
        /// </summary>
        public static async Task CanNotCreateProjectFileNameIsEmptyAsync()
        {
            await ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_ProjectNameCantBeEmpty")
                , GD_MessageBoxNotifyResult.NotifyRd);
        }

        /// <summary>
        /// 無法建立專案
        /// </summary>
        public async static Task CanNotCreateProjectAsync(string ProjectName = null)
        {
            string AddMessage = "";
            if (ProjectName != null)
                AddMessage = "\r\n" + ProjectName;

            await ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_ProjectIsExistedCantCreateProject") + AddMessage
                , GD_MessageBoxNotifyResult.NotifyRd);
        }



        public async static Task ProjectIsLoadedAsync()
        {
            await ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                (string)Application.Current.TryFindResource("Text_ProjectIsLoaded")
                , GD_MessageBoxNotifyResult.NotifyRd);
        }





    }
}
