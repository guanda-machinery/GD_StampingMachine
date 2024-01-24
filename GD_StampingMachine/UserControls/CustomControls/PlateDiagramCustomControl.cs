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
        static PlateDiagramCustomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(typeof(PlateDiagramCustomControl)));

            PlateDiagramIndexProperty = DependencyProperty.Register(nameof(PlateDiagramIndex), typeof(string), typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            ProjectNameProperty = DependencyProperty.Register(nameof(ProjectName), typeof(string), typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
           // ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(PlateDiagramCustomControl), new FrameworkPropertyMetadata((object)null, (PropertyChangedCallback)OnContentChanged));
        }

        public static readonly DependencyProperty PlateDiagramIndexProperty;
        public static readonly DependencyProperty ProjectNameProperty;
        //public static readonly DependencyProperty ContentProperty;



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
