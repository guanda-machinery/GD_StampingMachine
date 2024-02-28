using DevExpress.CodeParser;
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
    public class PlateDiagramCustomControl : UserControl
    {
        public PlateDiagramCustomControl()
        {

        }

        static PlateDiagramCustomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(typeof(PlateDiagramCustomControl)));
            PlateDiagramIndexProperty = DependencyProperty.Register(nameof(PlateDiagramIndex), typeof(string), typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            ProjectNameProperty = DependencyProperty.Register(nameof(ProjectName), typeof(string), typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


            OutLineBorderThicknessProperty = DependencyProperty.Register(nameof(OutLineBorderThickness), typeof(Thickness), typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));      
            OutLineBackgroundProperty = DependencyProperty.Register(nameof(OutLineBackground), typeof(Brush), typeof(PlateDiagramCustomControl), new PropertyMetadata(Brushes.Transparent));
            OutLineBorderBrushProperty = DependencyProperty.Register(nameof(OutLineBorderBrush), typeof(Brush), typeof(PlateDiagramCustomControl), new PropertyMetadata(Brushes.Transparent));
        }

        public static readonly DependencyProperty PlateDiagramIndexProperty;
        public static readonly DependencyProperty ProjectNameProperty;
        public static readonly DependencyProperty OutLineBorderThicknessProperty;
        public static readonly DependencyProperty OutLineBackgroundProperty;
        public static readonly DependencyProperty OutLineBorderBrushProperty;

        public Thickness OutLineBorderThickness
        {
            get => (Thickness)GetValue(OutLineBorderThicknessProperty);
            set => SetValue(OutLineBorderThicknessProperty, value);
        }
        public Brush OutLineBackground
        {
            get => (Brush)GetValue(OutLineBackgroundProperty);
            set => SetValue(OutLineBackgroundProperty, value);
        }
        public Brush OutLineBorderBrush
        {
            get => (Brush)GetValue(OutLineBorderBrushProperty);
            set => SetValue(OutLineBorderBrushProperty, value);
        }

        public string PlateDiagramIndex
        {
            get => (string)GetValue(PlateDiagramIndexProperty);
            set => SetValue(PlateDiagramIndexProperty, value);
        }
        public string ProjectName
        {
            get => (string)GetValue(ProjectNameProperty);
            set => SetValue(ProjectNameProperty, value);
        }

       /* public string Content
        {
            get => (string)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }*/





    }
}
