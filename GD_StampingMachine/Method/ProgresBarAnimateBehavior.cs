using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;


namespace GD_StampingMachine
{
    public class ProgresBarAnimateBehavior : Behavior<ProgressBar>
    {

        private bool _IsAnimating = false;
        private double oldValue = double.NaN;
        private double newValue = double.NaN;

        protected override void OnAttached() => AssociatedObject.ValueChanged += ProgressBar_ValueChanged;

        protected override void OnDetaching() => AssociatedObject.ValueChanged -= ProgressBar_ValueChanged;

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            if (this._IsAnimating)
            {
                this.newValue = e.NewValue;
                return;
            }
            this._IsAnimating = true;
            oldValue = e.OldValue;
            DoubleAnimation animation = new DoubleAnimation(e.OldValue, e.NewValue, new Duration(TimeSpan.FromSeconds(1)), FillBehavior.Stop);
            animation.Completed += Db_Completed;
            AssociatedObject.BeginAnimation(ProgressBar.ValueProperty, animation);
            e.Handled = true;
        }

        private void Db_Completed(object sender, EventArgs e)
        {
            if (Double.IsNaN(newValue)) this._IsAnimating = false;
            else
            {
                DoubleAnimation animation = new DoubleAnimation(this.oldValue, this.newValue, new Duration(TimeSpan.FromSeconds(1)), FillBehavior.Stop);
                this.oldValue = this.newValue;
                this.newValue = Double.NaN;
                animation.Completed += Db_Completed;
                AssociatedObject.BeginAnimation(ProgressBar.ValueProperty, animation);
            }
        }

    }
}
