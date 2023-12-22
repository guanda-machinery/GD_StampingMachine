using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GD_CommonLibrary
{
    public class CircularRangeSlider : RangeBase
    {
        FrameworkElement SliderContainer;
        //FrameworkElement StartArea;
        //FrameworkElement EndArea; 
        FrameworkElement StartCanvas, ArcCanvas, EndCanvas;
        public static readonly DependencyProperty RadiusProperty;
        public static readonly DependencyProperty StartProperty;   
        public static readonly DependencyProperty EndProperty;
        public static readonly DependencyProperty TickProperty;
        public static readonly DependencyProperty ArcStartProperty;
        public static readonly DependencyProperty ArcEndProperty;
        public static readonly DependencyProperty ArcAngleStartProperty;
        public static readonly DependencyProperty ArcAngleEndProperty;

        

        public static readonly DependencyProperty IsLargeArcProperty;
        public static readonly DependencyProperty FontCalibrationProperty;
        public static readonly DependencyProperty LargeCalibrationProperty;
        public static readonly DependencyProperty SmallCalibrationProperty;

        

        public static readonly RoutedEvent StartChangedEvent;
        public static readonly RoutedEvent EndChangedEvent;



        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<double> StartChanged
        {
            add
            {
                AddHandler(StartChangedEvent, value);
            }
            remove
            {
                RemoveHandler(StartChangedEvent, value);
            }
        }

        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<double> EndChanged
        {
            add
            {
                AddHandler(EndChangedEvent, value);
            }
            remove
            {
                RemoveHandler(EndChangedEvent, value);
            }
        }

        private static void OnStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularRangeSlider RangeSlider = (CircularRangeSlider)d;
        //    (UIElementAutomationPeer.FromElement(RangeSlider) as RangeBaseAutomationPeer)?.RaiseValuePropertyChangedEvent((double)e.OldValue, (double)e.NewValue);
            RangeSlider.OnStartChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnStartChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);
            routedPropertyChangedEventArgs.RoutedEvent = StartChangedEvent;
            RaiseEvent(routedPropertyChangedEventArgs);
        }

        private static void OnEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularRangeSlider RangeSlider = (CircularRangeSlider)d;
            //    (UIElementAutomationPeer.FromElement(RangeSlider) as RangeBaseAutomationPeer)?.RaiseValuePropertyChangedEvent((double)e.OldValue, (double)e.NewValue);
            RangeSlider.OnEndChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnEndChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);
            routedPropertyChangedEventArgs.RoutedEvent = EndChangedEvent;
            RaiseEvent(routedPropertyChangedEventArgs);
        }

        internal static object ConstrainToRange(DependencyObject d, object value)
        {
            CircularRangeSlider rangeSlider = (CircularRangeSlider)d;
            double minimum = rangeSlider.Minimum;
            double num = (double)value;
            if (num < minimum)
            {
                return minimum;
            }

            double maximum = rangeSlider.Maximum;
            if (num > maximum)
            {
                return maximum;
            }

            //篩選值 依照tick
          /*  if (rangeSlider.Tick>0)
            {
                var numFloor = Math.Floor(num / rangeSlider.Tick) * rangeSlider.Tick;
                return numFloor
            }*/


            return value;
        }
   
        private static bool IsValidDoubleValue(object value)
        {
            if (value != null)
            {
                double num = (double)value;
                if (num != double.NaN)
                {
                    return !double.IsInfinity(num);
                }
            }
            return false;
        }




       // const double Radius = 150;

        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set=> SetValue(RadiusProperty, value); 
        }

        public double Start
        {
            get => (double)GetValue(StartProperty);
            set=> SetValue(StartProperty, value); 
        }
        public double End
        {
            get => (double)GetValue(EndProperty);
            set=> SetValue(EndProperty, value);         
        }

        public double Tick
        {
            get => (double)GetValue(TickProperty);
            set =>SetValue(TickProperty, value); 
        }


        public Point ArcStart
        {
            get => (Point)GetValue(ArcStartProperty);
            set => SetValue(ArcStartProperty, value);
        }
        public Point ArcEnd
        {
            get => (Point)GetValue(ArcEndProperty);
            set => SetValue(ArcEndProperty, value);
        }

        public double ArcAngleStart
        {
            get => (double)GetValue(ArcAngleStartProperty);
            set => SetValue(ArcAngleStartProperty, value);
        }
        public double ArcAngleEnd
        {
            get => (double)GetValue(ArcAngleEndProperty);
            set => SetValue(ArcAngleEndProperty, value);
        }
        




        public bool IsLargeArc
        {
            get => (bool)GetValue(IsLargeArcProperty);
            set => SetValue(IsLargeArcProperty, value);
        }

        /*public ObservableCollection<double> LargeCalibrationFonts
        {
            get => (ushort)GetValue(LargeCalibrationFontsProperty);
            set => SetValue(LargeCalibrationFontsProperty, value);
        }*/


        

        public uint LargeCalibration
        {
            get => (uint)GetValue(LargeCalibrationProperty);
            set => SetValue(LargeCalibrationProperty, value);
        }
        public uint SmallCalibration
        {
            get => (uint)GetValue(SmallCalibrationProperty);
            set => SetValue(SmallCalibrationProperty, value);
        }


        static CircularRangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularRangeSlider), new FrameworkPropertyMetadata(typeof(CircularRangeSlider)));
            RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(CircularRangeSlider),
                new FrameworkPropertyMetadata(150.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            StartProperty = DependencyProperty.Register("Start", typeof(double), typeof(CircularRangeSlider),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnStartChanged, ConstrainToRange), IsValidDoubleValue);
            EndProperty = DependencyProperty.Register("End", typeof(double), typeof(CircularRangeSlider),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnEndChanged, ConstrainToRange), IsValidDoubleValue);
            TickProperty = DependencyProperty.Register("Tick", typeof(double), typeof(CircularRangeSlider),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            ArcStartProperty = DependencyProperty.Register("ArcStart", typeof(Point), typeof(CircularRangeSlider),
                new FrameworkPropertyMetadata(new Point(0,-130), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.None));
            ArcEndProperty = DependencyProperty.Register("ArcEnd", typeof(Point), typeof(CircularRangeSlider),
                new FrameworkPropertyMetadata(new Point(0,-130), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.None));

            ArcAngleStartProperty = DependencyProperty.Register("ArcAngleStart", typeof(double), typeof(CircularRangeSlider),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.None));
            ArcAngleEndProperty = DependencyProperty.Register("ArcAngleEnd", typeof(double), typeof(CircularRangeSlider),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.None));

            IsLargeArcProperty = DependencyProperty.Register("IsLargeArc", typeof(bool), typeof(CircularRangeSlider),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.None));



            FontCalibrationProperty = DependencyProperty.Register("FontCalibration", typeof(uint), typeof(CircularRangeSlider),
                new FrameworkPropertyMetadata(0u, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.None));
            LargeCalibrationProperty = DependencyProperty.Register("LargeCalibration", typeof(uint), typeof(CircularRangeSlider),
                new FrameworkPropertyMetadata(0u, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.None));
            SmallCalibrationProperty = DependencyProperty.Register("SmallCalibration", typeof(uint), typeof(CircularRangeSlider),
                new FrameworkPropertyMetadata(0u, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.None));



            StartChangedEvent = EventManager.RegisterRoutedEvent("StartChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(CircularRangeSlider));
            EndChangedEvent = EventManager.RegisterRoutedEvent("EndChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(CircularRangeSlider));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SliderContainer = GetTemplateChild("PART_SliderContainer") as FrameworkElement;
            if (SliderContainer != null)
            {
                SliderContainer.MouseLeave += SliderContainer_MouseLeave; ;
                SliderContainer.MouseUp += SliderContainer_MouseUp;
                //SliderContainer.MouseDown += SliderContainer_MouseDown;
                SliderContainer.MouseMove += SliderContainer_MouseMove;
            }

           // StartArea = GetTemplateChild("PART_StartArea") as FrameworkElement;
           // EndArea = GetTemplateChild("PART_EndArea") as FrameworkElement;

           // StartThumb = GetTemplateChild("PART_StartThumb") as Thumb;
           // EndThumb = GetTemplateChild("PART_EndThumb") as Thumb;


            StartCanvas = GetTemplateChild("PART_StartCanvas") as FrameworkElement;
            EndCanvas = GetTemplateChild("PART_EndCanvas") as FrameworkElement;
            ArcCanvas = GetTemplateChild("PART_Arc") as FrameworkElement;
       
            if (StartCanvas != null)
            {
                StartCanvas.MouseDown += StartCanvas_MouseDown;
            }

            if (EndCanvas != null)
            {
                EndCanvas.MouseDown += EndCanvas_MouseDown;
            }

            if (ArcCanvas != null)
            {
                ArcCanvas.MouseDown += ArcCanvas_MouseDown;
            }
            EndChanged += CircularRangeSlider_RangeChanged;
            StartChanged += CircularRangeSlider_RangeChanged;


        }

        private void SliderContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            _StartCanvasisPressed = false;
            _EndCanvasisPressed = false;
            _ArcCanvasisPressed = false;
            _LastMousePosition = null;
        }


        //private double? _start = null;
        //private double? _end = null;

        private void CircularRangeSlider_RangeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double range = Maximum - Minimum;

            var startArc = (2 * Math.PI * Start / range) - Math.PI / 2;
           ArcAngleStart = startArc*180/Math.PI +90.0;
            var p1x = Math.Round(Radius * Math.Cos(startArc), 2);
            var p1y = Math.Round(Radius * Math.Sin(startArc), 2);

            var endArc = (2 * Math.PI * End / range) - Math.PI / 2;
            ArcAngleEnd = endArc * 180 / Math.PI +90.0;
            var p2x = Math.Round(Radius * Math.Cos(endArc), 2);
            var p2y = Math.Round(Radius * Math.Sin(endArc), 2);

            ArcStart = new Point(p1x, p1y);
            ArcEnd = new Point(p2x, p2y);

            if (Math.Abs(startArc - endArc) > Math.PI)
            {
                IsLargeArc = !(startArc > endArc);
            }
            else
            {
                IsLargeArc = (startArc > endArc);
            }

        }

        private void EndCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _EndCanvasisPressed = true;
        }

        private void StartCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _StartCanvasisPressed = true;
        }
        private void ArcCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
           // _start = null;
            //_end = null;
           // _LastMousePosition = e.GetPosition((IInputElement)sender);
            _ArcCanvasisPressed = true;
        }



        Point? _LastMousePosition = null;

        private bool _StartCanvasisPressed = false;
        private bool _EndCanvasisPressed = false;
        private bool _ArcCanvasisPressed = false;
        /* private void SliderContainer_MouseDown(object sender, MouseButtonEventArgs e)
         {
             throw new NotImplementedException();
         }*/
        private void SliderContainer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _StartCanvasisPressed = false;
            _EndCanvasisPressed = false; 
            _ArcCanvasisPressed = false;
            _LastMousePosition = null;
           // _start = null;
          //  _end = null;
        }
        private void SliderContainer_MouseMove(object sender, MouseEventArgs e)
        {

            if (_StartCanvasisPressed)
            {
                //Find the parent canvas.
                /*if (_templateCanvas == null)
                {
                    _templateCanvas = FindParent<Canvas>(e.Source as Ellipse);
                    if (_templateCanvas == null) return;
                }*/
                //Canculate the current rotation angle and set the value.
                Point newPos = e.GetPosition((IInputElement)sender);
                double angle = GetAngleR(newPos, Radius);
                // knob.Value = (knob.Maximum - knob.Minimum) * angle / (2 * Math.PI);
                double start = (Maximum - Minimum) * angle / (2 * Math.PI);
                if (Math.Abs(Start - start) > Tick)
                {
                    if (Math.Abs(start - End) > 0)
                    {
                        if (Tick > 0)
                        {
                            start = Math.Floor(start / Tick) * Tick;
                        }


                        Start = start;
                    }
                    else
                    {

                    }
                }
            }

            if (_EndCanvasisPressed)
            {
                //Find the parent canvas.
                /*if (_templateCanvas == null)
                {
                    _templateCanvas = FindParent<Canvas>(e.Source as Ellipse);
                    if (_templateCanvas == null) return;
                }*/
                //Canculate the current rotation angle and set the value.
                Point newPos = e.GetPosition((IInputElement)sender);
                double angle = GetAngleR(newPos, Radius);
                // knob.Value = (knob.Maximum - knob.Minimum) * angle / (2 * Math.PI);
                var end = (Maximum - Minimum) * angle / (2 * Math.PI);

                if (Math.Abs(End - end) > Tick)
                {
                    if (Math.Abs(Start - end) > 0)
                    {
                        if(Tick>0)
                            end = Math.Floor(end / Tick) * Tick;
                        End = end;
                    }
                    else
                    {

                    }
                }
            }

            if (_ArcCanvasisPressed)
            {
                if(_LastMousePosition.HasValue)
                {
                    Point newPos = e.GetPosition((IInputElement)sender);
                    double newAngle = GetAngleR(newPos, Radius);
                    double oldAngle = GetAngleR(_LastMousePosition.Value, Radius);
                    var angle = newAngle - oldAngle;
                    var rTick = (Maximum - Minimum) * angle / (2 * Math.PI);
                    //if(!_start.HasValue)
                      var  _start = this.Start; 



                    if (_start + rTick > this.Maximum)
                    {
                        _start += rTick - this.Maximum;
                    }
                    else if (_start + rTick < this.Minimum)
                    {
                        _start += rTick + this.Maximum;
                    }
                    else
                        _start += rTick;
                    Start = _start;
                    /*
                    if (Math.Abs(Start - _start.Value) > Tick)
                    {
                        if (Math.Abs(End - _start.Value) > 0)
                        {
                            if (Tick > 0)
                                _start = Math.Floor(_start.Value / Tick) * Tick;
                            Start = _start.Value;

                        }
                        else
                        {

                        }
                    }*/

                    //if (!_end.HasValue)
                       var _end = this.End;

                    if (_end + rTick > this.Maximum)
                    {
                        _end += rTick - this.Maximum;
                    }
                    else if (_end + rTick < this.Minimum)
                    {
                        _end += rTick + this.Maximum;
                    }
                    else
                        _end += rTick;

                    End = _end;

                    /*
                    if (Math.Abs(End - _end.Value) > Tick)
                    {
                        if (Math.Abs(Start - _end.Value) > 0)
                        {
                            if (Tick > 0)
                                _end = Math.Floor(_end.Value / Tick) * Tick;
                            End = _end.Value;

                        }
                        else
                        {

                        }
                    }*/


                }
                _LastMousePosition = e.GetPosition((IInputElement)sender);
            }


        }













        private double GetAngleR(Point pos, double Radius)
        {
            //Calculate out the distance(r) between the center and the position
            Point center = new Point(Radius, Radius);
            double xDiff = center.X - pos.X;
            double yDiff = center.Y - pos.Y;
            double r = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);

            //Calculate the angle
            double angle = Math.Acos((center.Y - pos.Y) / r);
            Console.WriteLine("r:{0},y:{1},angle:{2}.", r, pos.Y, angle);
            if (pos.X < Radius)
                angle = 2 * Math.PI - angle;
            if (Double.IsNaN(angle))
                return 0.0;
            else
                return angle;
        }



    }


    public class ValueAngleConverter : BaseMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter,
                      System.Globalization.CultureInfo culture)
        {

            double current = 0;
            try
            {
                double value = (double)values[0];
                double minimum = (double)values[1];
                double maximum = (double)values[2];
                current = (value / (maximum - minimum)) * 360;
                if (current == 360)
                    current = 359.999;
            }
            catch
            {

            }
            return current;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter,
              System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class IntToCollectionConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(int.TryParse(value.ToString(), out int result))
            {
                ObservableCollection<double> list = new ObservableCollection<double>();
               
                for(int i=0; i< result;i++ )
                {
                    list.Add(i);
                }
                return list;
            }

            return new Collection<object>();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }





}
