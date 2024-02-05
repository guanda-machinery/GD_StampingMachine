using CommunityToolkit.Mvvm.Input;
using GD_CommonLibrary.Method;
using System;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class StampingMachineWindowViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingMachineWindowViewModel");






        private ICommand _closingCommand;
        public ICommand ClosingCommand
        {
            get => _closingCommand ??= new AsyncRelayCommand<System.ComponentModel.CancelEventArgs>(async e =>
            {
                var MessageBoxReturn =await MessageBoxResultShow.ShowAsync(
                    (string)Application.Current.TryFindResource("Text_notify"),
                    (string)Application.Current.TryFindResource("Text_AskCloseProgram"), MessageBoxButton.YesNo , GD_Enum.GD_MessageBoxNotifyResult.NotifyYe);
            
                if (MessageBoxReturn == MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            });
        }

        private ICommand _closedCommand;
        public ICommand ClosedCommand
        {
            get => _closedCommand ??= new RelayCommand<EventArgs>(e =>
            {
                Application.Current.Shutdown();
            });
        }

        private WindowState _windowState = WindowState.Normal;
        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                if (_windowState != WindowState.Minimized)
                {
                    try
                    {
                        Properties.Settings.Default.WindowState = WindowState;
                        Properties.Settings.Default.Save();
                    }
                    catch
                    {

                    }
                }
                OnPropertyChanged();
            }
        }


        private bool _topmost = false;
        public bool Topmost
        {
            get => _topmost; set { _topmost = value; OnPropertyChanged(); }
        }
        private bool _isEnabled = false;
        public bool IsEnabled
        {
            get => _isEnabled; set { _isEnabled = value; OnPropertyChanged(); }
        }

        private Visibility _visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get => _visibility; set { _visibility = value; OnPropertyChanged(); }
        }



        private double _opacity = 1;
        public double Opacity
        {
            get => _opacity; set { _opacity = value; OnPropertyChanged(); }
        }




        private ICommand _maximizeWindowCommand;
        public ICommand MaximizeWindowCommand
        {
            get => _maximizeWindowCommand ??= new RelayCommand(() =>
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else if (WindowState == WindowState.Normal)
                    WindowState = WindowState.Maximized;
            });
        }

        private ICommand _minimizeWindowCommand;
        public ICommand MinimizeWindowCommand
        {
            get => _minimizeWindowCommand ??= new RelayCommand(() =>
            {
                WindowState = WindowState.Minimized;
            });
        }

        private ICommand _closeWindowCommand;
        public ICommand CloseWindowCommand
        {
            get => _closeWindowCommand ??= new RelayCommand<System.Windows.RoutedEventArgs>(e =>
            {
                Window parentWindow = Window.GetWindow((DependencyObject)e.Source);
                parentWindow?.Close();
            });
        }

        private StampingMainViewModel _stampingMainVM;
       public StampingMainViewModel StampingMainVM
        {
            get => _stampingMainVM ??= new StampingMainViewModel();
            set
            {
                _stampingMainVM = value; OnPropertyChanged();
            }
        }

    }
}
