using DevExpress.CodeParser;
using GD_StampingMachine.GD_Enum;
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
using static GD_StampingMachine.Singletons.StampMachineDataSingleton.StampingOpcUANode;

namespace GD_StampingMachine.UserControls
{
    public class PlateDiagramCustomControl : UserControl
    {

        static PlateDiagramCustomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(typeof(PlateDiagramCustomControl)));
            PlateDiagramIndexProperty = DependencyProperty.Register(nameof(PlateDiagramIndex), typeof(string), typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            ProjectNameProperty = DependencyProperty.Register(nameof(ProjectName), typeof(string), typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

            StampingProcessProperty = DependencyProperty.Register(nameof(StampingProcess), typeof(StampingProcessEnum), typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(StampingProcessEnum.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            IsBrightModeProperty = DependencyProperty.Register(nameof(IsBrightMode), typeof(bool), typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            //OutLineBorderThicknessProperty = DependencyProperty.Register(nameof(OutLineBorderThickness), typeof(Thickness), typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));      
            OutLineBackgroundProperty = DependencyProperty.Register(nameof(OutLineBackground), typeof(Brush), typeof(PlateDiagramCustomControl), new PropertyMetadata(Brushes.Transparent));
            OutLineBorderBrushProperty = DependencyProperty.Register(nameof(OutLineBorderBrush), typeof(Brush), typeof(PlateDiagramCustomControl), new PropertyMetadata(Brushes.Transparent));
        }

        public static readonly DependencyProperty PlateDiagramIndexProperty;
        public static readonly DependencyProperty ProjectNameProperty;

        public static readonly DependencyProperty StampingProcessProperty;
        public static readonly DependencyProperty IsBrightModeProperty;
        //public static readonly DependencyProperty OutLineBorderThicknessProperty;
        public static readonly DependencyProperty OutLineBackgroundProperty;
        public static readonly DependencyProperty OutLineBorderBrushProperty;





        /* public Thickness OutLineBorderThickness
         {
             get => (Thickness)GetValue(OutLineBorderThicknessProperty);
             set => SetValue(OutLineBorderThicknessProperty, value);
         }
        */
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

        public StampingProcessEnum StampingProcess
        {
            get => (StampingProcessEnum)GetValue(StampingProcessProperty);
            set => SetValue(StampingProcessProperty, value);
        }

        public bool IsBrightMode
        {
            get => (bool)GetValue(IsBrightModeProperty);
            set => SetValue(IsBrightModeProperty, value);
        }


        /* public string Content
         {
             get => (string)GetValue(ContentProperty);
             set => SetValue(ContentProperty, value);
         }*/





    }


}
