using MaterialDesignThemes.Wpf;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace GD_CommonLibrary.GD_Popup
{
    public class DraggablePopup : NonTopmostPopup
    {
        public DraggablePopup()
        {
            this.IsTopmost = false;
        }

        Point _initialMousePosition;
        bool _isDragging;

        public static readonly DependencyProperty AllowDragProperty = DependencyProperty.Register(nameof(AllowDrag), typeof(bool), typeof(DraggablePopup), new FrameworkPropertyMetadata());
        public bool AllowDrag
        {
            get { return (bool)GetValue(AllowDragProperty); }
            set { SetValue(AllowDragProperty, value); }
        }


        protected override void OnInitialized(EventArgs e)
        {
            var contents = Child as FrameworkElement;

            Debug.Assert(contents != null, "DraggablePopup either has no content if content that " +
             "does not derive from FrameworkElement. Must be fixed for dragging to work.");

            try
            {
                contents.MouseLeftButtonDown += Child_MouseLeftButtonDown;
                contents.MouseLeftButtonUp += Child_MouseLeftButtonUp;
                contents.MouseMove += Child_MouseMove;
            }
            catch (Exception)
            {
                Debugger.Break();
            }


        }
        private void Child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!AllowDrag)
                e.Handled = false;

            var element = sender as FrameworkElement;
            _initialMousePosition = e.GetPosition(null);
            element.CaptureMouse();
            _isDragging = true;
            e.Handled = true;
        }
        private void Child_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var currentPoint = e.GetPosition(null);
                HorizontalOffset += (currentPoint.X - _initialMousePosition.X);
                VerticalOffset += (currentPoint.Y - _initialMousePosition.Y);
            }
        }
        private void Child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                var element = sender as FrameworkElement;
                element.ReleaseMouseCapture();
                _isDragging = false;
                e.Handled = true;
            }
        }
    }

    public class NonTopmostPopup : PopupEx
    {
        /// <summary>
        /// Is Topmost dependency property
        /// </summary>
        public static readonly DependencyProperty IsTopmostProperty = DependencyProperty.Register(nameof(IsTopmost), typeof(bool), typeof(NonTopmostPopup), new FrameworkPropertyMetadata(false, OnIsTopmostChanged));

        private bool? _appliedTopMost;
        private bool _alreadyLoaded;
        private Window _parentWindow;

        /// <summary>
        /// Get/Set IsTopmost
        /// </summary>
        public bool IsTopmost
        {
            get { return (bool)GetValue(IsTopmostProperty); }
            set { SetValue(IsTopmostProperty, value); }
        }

        /// <summary>
        /// ctor
        /// </summary>
        public NonTopmostPopup()
        {
            this.AllowsTransparency = true;
            Loaded += OnPopupLoaded;
            Unloaded += OnPopupUnloaded;

            base.CommandBindings.Add(new CommandBinding(ClosePopupCommand, ClosePopupHandler, ClosePopupCanExecute));
            base.CommandBindings.Add(new CommandBinding(OpenPopupCommand, OpenPopupHandler, OpenPopupCanExecute));
            //這邊寫一個綁定isopen的function->也就是關閉時為打開 打開時為關閉
            base.CommandBindings.Add(new CommandBinding(PopupCommand, PopupHandler, PopupCanExecute));
        }



        static NonTopmostPopup()
        {
            ClosePopupCommand = new RoutedCommand();
            OpenPopupCommand = new RoutedCommand();
            PopupCommand = new RoutedCommand();
        }



        void OnPopupLoaded(object sender, RoutedEventArgs e)
        {
            if (_alreadyLoaded)
                return;

            _alreadyLoaded = true;

            if (Child != null)
            {
                Child.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnChildPreviewMouseLeftButtonDown), true);
            }

            _parentWindow = Window.GetWindow(this);

            if (_parentWindow == null)
                return;

            _parentWindow.Activated += OnParentWindowActivated;
            _parentWindow.Deactivated += OnParentWindowDeactivated;
        }

        private void OnPopupUnloaded(object sender, RoutedEventArgs e)
        {
            if (_parentWindow == null)
                return;
            _parentWindow.Activated -= OnParentWindowActivated;
            _parentWindow.Deactivated -= OnParentWindowDeactivated;
        }

        void OnParentWindowActivated(object sender, EventArgs e)
        {
            Debug.WriteLine("Parent Window Activated");
            SetTopmostState(true);
            if (IsTopmost == false)
            {
                SetTopmostState(IsTopmost);
            }
        }

        void OnParentWindowDeactivated(object sender, EventArgs e)
        {
            Debug.WriteLine("Parent Window Deactivated");

            //if (IsTopmost == false)
          //  {
               // SetTopmostState(IsTopmost);
           // }
        }
        void OnChildPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Child Mouse Left Button Down");

            SetTopmostState(true);

            if (!_parentWindow.IsActive && IsTopmost == false)
            {
                _parentWindow.Activate();
                Debug.WriteLine("Activating Parent from child Left Button Down");
            }
        }

        private static void OnIsTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var thisobj = (NonTopmostPopup)obj;
            thisobj.SetTopmostState(thisobj.IsTopmost);
        }

        protected override void OnOpened(EventArgs e)
        {
            SetTopmostState(IsTopmost);
            base.OnOpened(e);
        }

        private void SetTopmostState(bool isTop)
        {
            // Don’t apply state if it’s the same as incoming state
            if (_appliedTopMost.HasValue && _appliedTopMost == isTop)
            {
                return;
            }

            if (Child == null)
                return;

            var hwndSource = (PresentationSource.FromVisual(Child)) as HwndSource;

            if (hwndSource == null)
                return;
            var hwnd = hwndSource.Handle;


            if (!GetWindowRect(hwnd, out RECT rect))
                return;

            Debug.WriteLine("setting z-order " + isTop);

            if (isTop)
            {
                SetWindowPos(hwnd, HWND_TOPMOST, rect.Left, rect.Top, (int)Width, (int)Height, TOPMOST_FLAGS);
            }
            else
            {
                // Z-Order would only get refreshed/reflected if clicking the
                // the titlebar (as opposed to other parts of the external
                // window) unless I first set the popup to HWND_BOTTOM
                // then HWND_TOP before HWND_NOTOPMOST
                SetWindowPos(hwnd, HWND_BOTTOM, rect.Left, rect.Top, (int)Width, (int)Height, TOPMOST_FLAGS);
                SetWindowPos(hwnd, HWND_TOP, rect.Left, rect.Top, (int)Width, (int)Height, TOPMOST_FLAGS);
                SetWindowPos(hwnd, HWND_NOTOPMOST, rect.Left, rect.Top, (int)Width, (int)Height, TOPMOST_FLAGS);
            }

            _appliedTopMost = isTop;
        }



        public static readonly RoutedCommand OpenPopupCommand;
        public static readonly RoutedCommand ClosePopupCommand;
        public static readonly RoutedCommand PopupCommand;
        private void OpenPopupHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (!executedRoutedEventArgs.Handled)
            {
                //InternalOpen(executedRoutedEventArgs.Parameter);
                if (executedRoutedEventArgs.Parameter is bool parameterBoolean)
                {
                    InternalOpen(parameterBoolean);
                }
                else
                    InternalOpen(true);

                executedRoutedEventArgs.Handled = true;
            }
        }
        private void ClosePopupHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (!executedRoutedEventArgs.Handled)
            {
                InternalOpen(false);
                executedRoutedEventArgs.Handled = true;
            }
        }

        private void PopupHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (!executedRoutedEventArgs.Handled)
            {
                InternalOpen(!IsOpen);
                executedRoutedEventArgs.Handled = true;
            }
        }

        internal void InternalOpen(object parameter)
        {
            if (parameter != null)
                if (bool.TryParse(parameter.ToString(), out var BooleanParameter))
                    InternalOpen(BooleanParameter);
        }
        internal void InternalOpen(bool BooleanParameter)
        {
            this.IsOpen = BooleanParameter;
        }




        private void OpenPopupCanExecute(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            canExecuteRoutedEventArgs.CanExecute = !this.IsOpen;
        }

        private void ClosePopupCanExecute(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            canExecuteRoutedEventArgs.CanExecute = this.IsOpen;
        }

        private void PopupCanExecute(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            canExecuteRoutedEventArgs.CanExecute = true;
        }











        #region P/Invoke imports & definitions
#pragma warning disable 1591 //Xml-doc
#pragma warning disable 169 //Never used-warning
        // ReSharper disable InconsistentNaming
        // Imports etc. with their naming rules

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT

        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
        int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        private const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOZORDER = 0x0004;
        const UInt32 SWP_NOREDRAW = 0x0008;
        const UInt32 SWP_NOACTIVATE = 0x0010;

        const UInt32 SWP_FRAMECHANGED = 0x0020; /* The frame changed: send WM_NCCALCSIZE */
        const UInt32 SWP_SHOWWINDOW = 0x0040;
        const UInt32 SWP_HIDEWINDOW = 0x0080;
        const UInt32 SWP_NOCOPYBITS = 0x0100;
        const UInt32 SWP_NOOWNERZORDER = 0x0200; /* Don’t do owner Z ordering */
        const UInt32 SWP_NOSENDCHANGING = 0x0400; /* Don’t send WM_WINDOWPOSCHANGING */

        const UInt32 TOPMOST_FLAGS =
            SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOSIZE | SWP_NOMOVE | SWP_NOREDRAW | SWP_NOSENDCHANGING;

        // ReSharper restore InconsistentNaming
#pragma warning restore 1591
#pragma warning restore 169
        #endregion
    }
}
