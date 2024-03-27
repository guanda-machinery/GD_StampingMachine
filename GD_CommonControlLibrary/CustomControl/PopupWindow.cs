using DevExpress.Pdf.Native.BouncyCastle.Security;
using GD_CommonControlLibrary.GD_Popup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_CommonControlLibrary.GD_Popup
{
    public class PopupWindow : System.Windows.Window
    {
        static PopupWindow()
        {
            IsOpenedProperty = DependencyProperty.Register(nameof(IsOpened), typeof(bool), typeof(PopupWindow), new PropertyMetadata(false, IsOpenedPropertyChanged));
        }

        public static readonly DependencyProperty IsOpenedProperty;

        public PopupWindow()
        {
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;

        }

        public bool IsOpened
        {
            get => (bool)GetValue(IsOpenedProperty);
            set => SetValue(IsOpenedProperty, value);
        }



        private static void IsOpenedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PopupWindow thisObj && e.NewValue is bool isopen)
            {
                if(isopen && !thisObj.IsActive)
                    thisObj.Show();

                if (!isopen && thisObj.IsActive)
                    thisObj.Close();
            }


        }



        private bool isMouseDown;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            isMouseDown = e.MouseDevice.LeftButton == MouseButtonState.Pressed;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            isMouseDown = false;
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            isMouseDown = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            //  if (AllowDrag && isMouseDown && e.LeftButton == MouseButtonState.Pressed)
            if (isMouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                isMouseDown = false;
                DragMove();
            }
        }


    }
}
