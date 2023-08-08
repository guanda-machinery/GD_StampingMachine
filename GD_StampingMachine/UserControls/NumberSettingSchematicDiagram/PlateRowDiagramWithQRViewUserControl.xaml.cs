using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace GD_StampingMachine.UserControls.NumberSettingSchematicDiagram
{
    /// <summary>
    /// NumberSettingSchematicDiagramUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class PlateRowDiagramWithQRViewUserControl : UserControl
    {
        public PlateRowDiagramWithQRViewUserControl()
        {
            InitializeComponent();



        }


        public Visibility RedMeasurementLineVisibility
        {
            get => (Visibility)GetValue(RedMeasurementLineVisibilityProperty);
            set => SetValue(RedMeasurementLineVisibilityProperty, value);
        }

        public static readonly DependencyProperty RedMeasurementLineVisibilityProperty = DependencyProperty.Register(
            nameof(RedMeasurementLineVisibility),
            typeof(Visibility),
            typeof(PlateRowDiagramWithQRViewUserControl),
            new PropertyMetadata(Visibility.Visible, RedMeasurementLineVisibilityPropertyChanged));

        private static void RedMeasurementLineVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var UserControl = (d as PlateRowDiagramWithQRViewUserControl);
            UserControl.Resources["RedMeasurementLineVisibility"] = e.NewValue;
            //UserControl.Resources.Remove("RedMeasurementLineVisibilityProperty");
            //UserControl.Resources.Add("RedMeasurementLineVisibilityProperty",(Visibility)e.NewValue);
        }



    }
}
