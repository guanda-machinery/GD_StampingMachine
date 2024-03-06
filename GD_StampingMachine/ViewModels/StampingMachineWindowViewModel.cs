using CommunityToolkit.Mvvm.Input;
using GD_CommonLibrary.Method;
using System;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class StampingMachineWindowViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingMachineWindowViewModel");





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


        private ICommand?_maximizeWindowCommand;
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

        private ICommand?_minimizeWindowCommand;
        public ICommand MinimizeWindowCommand
        {
            get => _minimizeWindowCommand ??= new RelayCommand(() =>
            {
                WindowState = WindowState.Minimized;
            });
        }

        private ICommand?_closeWindowCommand;
        public ICommand CloseWindowCommand
        {
            get => _closeWindowCommand ??= new RelayCommand<System.Windows.RoutedEventArgs>(e =>
            {
                Window parentWindow = Window.GetWindow((DependencyObject)e.Source);
                parentWindow?.Close();
            });
        }

        private StampingMainViewModel? _stampingMainVM;
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
