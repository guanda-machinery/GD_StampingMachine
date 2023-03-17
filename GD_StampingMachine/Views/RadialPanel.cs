using DevExpress.CodeParser;
using DevExpress.Mvvm.Native;
using GD_StampingMachine.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GD_StampingMachine.Views
{
    public class RadialPanel : Panel , INotifyPropertyChanged 
    {

        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
            nameof(StartAngle),
            typeof(double),
            typeof(RadialPanel),
        new PropertyMetadata(0.0, Angle_PropertyChanged));

        /// <summary>
        /// 起始原點
        /// </summary>
        public double StartAngle
        {
            get => (double)GetValue(StartAngleProperty);
            set=>SetValue(StartAngleProperty, value);
        }

        public static readonly DependencyProperty RoundAngleProperty = DependencyProperty.Register(
            nameof(RoundAngle),
            typeof(double),
            typeof(RadialPanel), new PropertyMetadata(360.0 , Angle_PropertyChanged));

        /// <summary>
        /// 旋轉範圍
        /// </summary>
        public double RoundAngle
        {
            get=> (double)GetValue(RoundAngleProperty);
            set => SetValue(RoundAngleProperty, value);
        }


        public static readonly DependencyProperty ItemRotateAngleProperty = DependencyProperty.Register(
            nameof(ItemRotateAngle),
            typeof(double),
            typeof(RadialPanel),
            new PropertyMetadata(0.0, Angle_PropertyChanged));

        /// <summary>
        /// 內部物件旋轉
        /// </summary>
        public double ItemRotateAngle
        {
            get => (double)GetValue(ItemRotateAngleProperty);
            set => SetValue(ItemRotateAngleProperty, value);
        }


        public static readonly DependencyProperty RotateAngleProperty = DependencyProperty.Register(
            nameof(RotateAngle),
            typeof(double),
            typeof(RadialPanel),
            new PropertyMetadata(0.0, Angle_PropertyChanged));

        /// <summary>
        /// 內部物件旋轉
        /// </summary>
        public double RotateAngle
        {
            get => (double)GetValue(RotateAngleProperty);
            set => SetValue(RotateAngleProperty, value);
        }



        private static void Angle_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {          
            //刷新是否有更好做法?
            var TempWidth = (d as RadialPanel).Width;
            (d as RadialPanel).Width = 0;
            (d as RadialPanel).Width = TempWidth;
        }




        // Measure each children and give as much room as they want 
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement elem in Children)
            {
                //Give Infinite size as the avaiable size for all the children
                elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            return base.MeasureOverride(availableSize);
        }


        //Arrange all children based on the geometric equations for the circle.

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;

            double _currentAngle = ItemRotateAngle + StartAngle - RotateAngle;

            //double _marginalAngle = 360 / Children.Count;
            double _marginalAngle = RoundAngle / Children.Count;
            double _radian =( StartAngle+ RotateAngle) * (Math.PI / 180);
            //Degrees converted to Radian by multiplying with PI/180
            double _incrementalAngularSpaceRadian = _marginalAngle * (Math.PI / 180);

            //An approximate radii based on the avialable size , obviusly a better approach is needed here.
            double radiusX = finalSize.Width / 2.4;
            double radiusY = finalSize.Height / 2.4;
            foreach (UIElement elem in Children)
            {
                //Calculate the point on the circle for the element
                Point childPoint = new Point(Math.Cos(_radian) * radiusX, Math.Sin(_radian) * radiusY);
                //Offsetting the point to the Avalable rectangular area which is FinalSize.
                Point actualChildPoint = new Point(finalSize.Width / 2 + childPoint.X - elem.DesiredSize.Width / 2, finalSize.Height / 2 + childPoint.Y - elem.DesiredSize.Height / 2);
                //Call Arrange method on the child element by giving the calculated point as the placementPoint.
                elem.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, elem.DesiredSize.Width, elem.DesiredSize.Height));
                //Calculate the new _angle for the next element
                _radian += _incrementalAngularSpaceRadian;


                //內容物旋轉
                if (ItemRotateAngle != 0)
                {
                    elem.RenderTransformOrigin = new Point(0.5, 0.5);
                    elem.RenderTransform = new RotateTransform(-_currentAngle);
                    _currentAngle -= _marginalAngle;
                }
                else
                {
                    elem.RenderTransform = new RotateTransform(0);
                }
            }



            return finalSize;

        }

        //彈幕感
        /*        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;

            double _currentAngle = ItemRotateAngle + StartAngle + RotateAngle;

            //double _marginalAngle = 360 / Children.Count;
            double _marginalAngle = RoundAngle / Children.Count;
            double _radian =( StartAngle+ RotateAngle) * (Math.PI / 180);
            //Degrees converted to Radian by multiplying with PI/180
            double _incrementalAngularSpaceRadian = _marginalAngle * (Math.PI / 180);

            //An approximate radii based on the avialable size , obviusly a better approach is needed here.
            double radiusX = finalSize.Width / 2.4;
            double radiusY = finalSize.Height / 2.4;
            foreach (UIElement elem in Children)
            {
                //Calculate the point on the circle for the element
                Point childPoint = new Point(Math.Cos(_radian) * radiusX, Math.Sin(_radian) * radiusY);
                //Offsetting the point to the Avalable rectangular area which is FinalSize.
                Point actualChildPoint = new Point(finalSize.Width / 2 + childPoint.X - elem.DesiredSize.Width / 2, finalSize.Height / 2 + childPoint.Y - elem.DesiredSize.Height / 2);
                //Call Arrange method on the child element by giving the calculated point as the placementPoint.
                elem.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, elem.DesiredSize.Width, elem.DesiredSize.Height));
                //Calculate the new _angle for the next element
                _radian += _incrementalAngularSpaceRadian;


                //內容物旋轉
                if (ItemRotateAngle != 0)
                {
                    elem.RenderTransformOrigin = new Point(0.5, 0.5);
                    elem.RenderTransform = new RotateTransform(-_currentAngle);
                    _currentAngle -= _marginalAngle;
                }
                else
                {
                    elem.RenderTransform = new RotateTransform(0);
                }
            }



            return finalSize;

        }*/



        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// 屬性變更事件
        /// </summary>
        /// <param name="propertyName">屬性名稱</param>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



    }
}
