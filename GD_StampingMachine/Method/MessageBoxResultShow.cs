﻿using DevExpress.CodeParser;
using GD_StampingMachine.Windows;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GD_CommonLibrary.Method
{
    public class MessageBoxResultShow
    {


        public static MessageBoxResult Show(string MessageTitle, string MessageString, MessageBoxButton MB_Button, MessageBoxImage MB_Image)
        {
            MessageBoxResult messageBoxReturn = MessageBoxResult.None;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Window newWindow = Application.Current.MainWindow ?? new()
                {
                    Topmost = true,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                


                messageBoxReturn = DevExpress.Xpf.WindowsUI.WinUIMessageBox.Show(newWindow, MessageString,
                    MessageTitle, MB_Button, MB_Image, MessageBoxResult.None, MessageBoxOptions.None, DevExpress.Xpf.Core.FloatingMode.Window);
            }));

            return messageBoxReturn;
        }

        public static MessageBoxResult FloatShow(string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxFloatResult MB_Image)
        {
            MessageBoxResult messageBoxReturn = MessageBoxResult.None;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageBoxWindow newWindow =  new MessageBoxWindow()
                {
                    WindowStartupLocation= WindowStartupLocation.CenterOwner,
                };

                var mainWindow = Application.Current.MainWindow ?? new Window();

                messageBoxReturn =  newWindow.FloatShow(mainWindow, MessageTitle, MessageString, MB_Button, MB_Image);
            }));
            return messageBoxReturn;
        }



        public static async Task<MessageBoxResult> FloatShowAsync(string MessageTitle, string MessageString, MessageBoxButton MB_Button, GD_MessageBoxFloatResult MB_Image)
        {
            MessageBoxResult MessageBoxReturn = MessageBoxResult.None;
            await Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                MessageBoxReturn = FloatShow(MessageTitle, MessageString, MB_Button, MB_Image);
            }));
            return MessageBoxReturn;
        }








        public static async Task<MessageBoxResult> ShowAsync(string MessageTitle, string MessageString, MessageBoxButton MB_Button, MessageBoxImage MB_Image)
        {
            MessageBoxResult MessageBoxReturn = MessageBoxResult.None;
            await Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                MessageBoxReturn = Show(MessageTitle, MessageString, MB_Button, MB_Image);
            }));
            return MessageBoxReturn;
        }


        public static async Task<MessageBoxResult> ShowYesNoAsync(string MessageTitle, string MessageString, MessageBoxImage BoxImage = MessageBoxImage.Information)
        {
            return await ShowAsync(MessageTitle, MessageString,
                MessageBoxButton.YesNo, BoxImage);
        }

        public static async Task ShowOKAsync(string MessageTitle, string MessageString, MessageBoxImage BoxImage = MessageBoxImage.Information)
        {
            await ShowAsync(MessageTitle, MessageString,
                MessageBoxButton.OK, BoxImage);
        }

        public static async Task ShowExceptionAsync(Exception ex)
        {
            await ShowAsync(
                         (string)Application.Current.TryFindResource("Text_notify"),
                         ex.Message,
                         MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
