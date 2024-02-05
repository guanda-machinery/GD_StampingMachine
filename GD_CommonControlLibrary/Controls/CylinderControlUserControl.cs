using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GD_CommonControlLibrary
{

    public class CylinderControlUserControl:Control
    {

        public static readonly DependencyProperty CylinderUpCommandProperty =
        DependencyProperty.Register(nameof(CylinderUpCommand), typeof(ICommand), typeof(CylinderControlUserControl), new PropertyMetadata());

        public static readonly DependencyProperty CylinderUpCommandProperty =
        DependencyProperty.Register(nameof(CylinderUpCommand), typeof(ICommand), typeof(CylinderControlUserControl), new PropertyMetadata());

        public static readonly DependencyProperty CylinderUpCommandProperty =
        DependencyProperty.Register(nameof(CylinderDownCommand), typeof(ICommand), typeof(CylinderControlUserControl), new PropertyMetadata());

        public ICommand CylinderUpCommand
        {
            get { return (ICommand)GetValue(CylinderUpCommandProperty); }
            set { SetValue(CylinderUpCommandProperty, value); }
        }

        public ICommand CylinderUpCommand
        {
            get { return (ICommand)GetValue(CylinderUpCommandProperty); }
            set { SetValue(CylinderUpCommandProperty, value); }
        }

        public ICommand CylinderUpCommand
        {
            get { return (ICommand)GetValue(CylinderUpCommandProperty); }
            set { SetValue(CylinderUpCommandProperty, value); }
        }

        public enum CylinderMode
        {

        }



    }


}
