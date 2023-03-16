using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace GD_StampingMachine.Views
{
    public class RotateRadiaPanel : Panel
    {

        // This Panel lays its children out in a circle

        // keeping the angular distance from each child

        // equal; MeasureOverride is called before ArrangeOverride.

        double _maxChildHeight, _perimeter, _radius, _adjustFactor;

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement elem in Children)
            {
                //Give Infinite size as the avaiable size for all the children
                elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            return base.MeasureOverride(availableSize);



            _perimeter = 0;
            _maxChildHeight = 0;
            // the available size.
            foreach (UIElement uie in Children)
            {
                uie.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                _perimeter += uie.DesiredSize.Width;
                _maxChildHeight = Math.Max(_maxChildHeight, uie.DesiredSize.Height);
            }

            // If the marginal angle is not 0, 90 or 180
            // then the adjustFactor is needed.

            if (Children.Count > 2 && Children.Count != 4)

                _adjustFactor =10 ;


            // Determine the radius of the circle layout and determine

            // the RadialPanel's DesiredSize.


            _radius = _perimeter / (2 * Math.PI) + _adjustFactor;

            double _squareSize = 2 * (_radius + _maxChildHeight);

            return new Size(_squareSize, _squareSize);
        }



        // Perform arranging of children based on 

        // the final size.



        protected override Size ArrangeOverride(Size finalSize)

        {

            // Necessary variables.

            double _currentOriginX = 0,

                    _currentOriginY = 0,

                    _currentAngle = 0,

                    _centerX = 0,

                    _centerY = 0,

                    _marginalAngle = 0;



            // During measure, an adjustFactor was added to the radius

            // to account for rotated children that might fall outside

            // of the desired size.  Now, during arrange, that extra

            // space isn't needed



            _radius -= _adjustFactor;



            // Find center of the circle based on arrange size.

            // DesiredSize is not used because the Panel

            // is potentially being arranged across a larger

            // area from the default alignment values.



            _centerX = finalSize.Width / 2;

            _centerY = finalSize.Height / 2;



            // Determine the marginal angle, the angle between

            // each child on the circle.



            if (Children.Count != 0)

                _marginalAngle = 360 / Children.Count;



            foreach (UIElement uie in Children)

            {

                // Find origin from which to arrange 

                // each child of the RadialPanel (its top

                // left corner.)



                _currentOriginX = _centerX - uie.DesiredSize.Width / 2;

                _currentOriginY = _centerY - _radius - uie.DesiredSize.Height;



                // Apply a rotation on each child around the center of the

                // RadialPanel.



                uie.RenderTransform = new RotateTransform(_currentAngle);
                uie.Arrange(new Rect(new Point(_currentOriginX, _currentOriginY), new Size(uie.DesiredSize.Width, uie.DesiredSize.Height)));



                // Increment the _currentAngle by the _marginalAngle

                // to advance the next child to the appropriate position.



                _currentAngle += _marginalAngle;

            }



            // In this case, the Panel is sizing to the space

            // given, so, return the finalSize which will be used

            // to set the ActualHeight & ActualWidth and for rendering.



            return finalSize;

        }

    }


}
