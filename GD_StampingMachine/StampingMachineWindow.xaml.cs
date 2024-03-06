using CommunityToolkit.Mvvm.Input;
using GD_CommonLibrary.Method;
using GD_StampingMachine.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine
{
    /// <summary>
    /// StampingMachineWindow.xaml 的互動邏輯
    /// </summary>
    public partial class StampingMachineWindow : Window
    {
        public StampingMachineWindow()
        {
            this.DataContext = new StampingMachineWindowViewModel()
            {
                //Visibility = Visibility.Collapsed,
                Opacity = 0,
                IsEnabled = false,
            };
            InitializeComponent();


        }



        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }



        //Point _initialMousePosition;
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                this.DragMove();
            }

            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Maximized;
            }

            //_initialMousePosition = e.GetPosition(null);

        }





        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                // 在最大化狀態下，調整視窗大小以避免覆蓋開始工具列
                // 這裡可以根據開始工具列的高度進行調整

                System.Windows.Forms.Screen currentScreen = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(this).Handle);
                // 取得該螢幕的高度
                // double screenHeight = currentScreen.Bounds.Height;

                double screenHeight = currentScreen.WorkingArea.Height;
                //MaxHeight = screenHeight;
                //MaxHeight = SystemParameters.PrimaryScreenHeight;
                this.MainGrid.Margin = new Thickness(6);
            }
            else
            {
                MaxHeight = double.PositiveInfinity;
                this.MainGrid.Margin = new Thickness(0);
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current?.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var MessageBoxReturn = MessageBoxResultShow.Show(
                    (string)Application.Current.TryFindResource("Text_notify"),
                    (string)Application.Current.TryFindResource("Text_AskCloseProgram"), MessageBoxButton.YesNo, GD_Enum.GD_MessageBoxNotifyResult.NotifyYe);

            if (MessageBoxReturn == MessageBoxResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }

        }





        private ICommand?_closingCommand;
        public ICommand ClosingCommand
        {
            get => _closingCommand ??= new AsyncRelayCommand<System.ComponentModel.CancelEventArgs>(async e =>
            {
                await Task.CompletedTask;

            });
        }





        /*
private bool isResizing = false;
private Point resizeStartPoint;
private void Border_MouseDown(object sender, MouseButtonEventArgs e)
{
if(e.LeftButton == MouseButtonState.Pressed)
{
isResizing = true;
resizeStartPoint = e.GetPosition(this);

}

}
private void Border_MouseUp(object sender, MouseButtonEventArgs e)
{
if (e.LeftButton == MouseButtonState.Released)
{
isResizing = false;
}
}
private void Border_MouseMove(object sender, MouseEventArgs e)
{
var mousePos = e.GetPosition(this);
if(sender is  FrameworkElement element)
{
const double range = 20;
var width = element.ActualWidth;
var height = element.ActualHeight;

if (mousePos.X < range && mousePos.Y < range)
{
  element.Cursor = Cursors.SizeNWSE;
  //在左上方
}
else if (mousePos.X > width - range && mousePos.Y < range)
{
  element.Cursor = Cursors.SizeNESW;
  //在右上方
}
else if (mousePos.X < range && mousePos.Y > height - range)
{
  element.Cursor = Cursors.SizeNESW;
  //在左下方
}
else if (mousePos.X > width - range && mousePos.Y > height - range) 
{
  element.Cursor = Cursors.SizeNWSE;
  //在右下方
}
else if (mousePos.X < range)
{

  if (isResizing)
  {
      this.Width += mousePos.X - resizeStartPoint.X;
      this.Left -= mousePos.X - resizeStartPoint.X;
  }
  // element.Width = mousePos.Y - resizeStartPoint.Y;

  // 滑鼠在畫面的左方
  element.Cursor = Cursors.SizeWE;
}
else if (mousePos.X > width - range)
{
  element.Cursor = Cursors.SizeWE;
  // 滑鼠在畫面的右方
}
else if (mousePos.Y < range)
{
  element.Cursor = Cursors.SizeNS;
  // 滑鼠在畫面的上方
}
else if (mousePos.Y > height - range)
{
  element.Cursor = Cursors.SizeNS;
  // 滑鼠在畫面的下方
}



//上方
//下方
//SizeNS

//左方
//右方
//SizeWE


//左上
//右下
//SizeNWSE

//右上
//左下
//SizeNESW


//element.Cursor = Cursors.Hand;

}
}*/


        // private DateTime lastMouseMoveTime = DateTime.MinValue;
        //private TimeSpan minMouseMoveInterval = TimeSpan.FromMilliseconds(100); // 設定最小處理間隔


    }
}
