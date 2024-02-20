using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GD_StampingMachine.UserControls
{
    /// <summary>
    /// 
    /// </summary>
    public class GradientEllipse : Control
    {
        static GradientEllipse()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GradientEllipse), new FrameworkPropertyMetadata(typeof(GradientEllipse)));

            FillProperty = DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(GradientEllipse), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        }

        public static readonly DependencyProperty FillProperty;


        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }



    }
}