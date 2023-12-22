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

namespace GD_StampingMachine.UserControls.NumberSettingSchematicDiagram
{
    /// <summary>
    /// QRPlateRowDiagramControl.xaml 的互動邏輯
    /// </summary>
    public partial class QRPlateRowDiagramControl : UserControl
    {
        public QRPlateRowDiagramControl()
        {
            InitializeComponent();

            this.Resources.Add(nameof(RedMeasurementLineVisibility), Visibility.Visible);
            this.Resources.Add(nameof(PlateRowBackground), (SolidColorBrush)new BrushConverter().ConvertFrom("#d6d5ce"));
        }


        public Visibility RedMeasurementLineVisibility
        {
            get => (Visibility)GetValue(RedMeasurementLineVisibilityProperty);
            set => SetValue(RedMeasurementLineVisibilityProperty, value);
        }

        /// <summary>
        /// 金屬板背景色
        /// </summary>
        public Brush PlateRowBackground
        {
            get => (Brush)GetValue(PlateRowBackgroundProperty);
            set => SetValue(PlateRowBackgroundProperty, value);
        }

        /// <summary>
        /// 沖壓字模背景色
        /// </summary>
        public Brush FontStampBackground
        {
            get => (Brush)GetValue(FontStampBackgroundProperty);
            set => SetValue(FontStampBackgroundProperty, value);
        }




        public static readonly DependencyProperty RedMeasurementLineVisibilityProperty = DependencyProperty.Register(
            nameof(RedMeasurementLineVisibility),
            typeof(Visibility),
            typeof(UserControl),
            new PropertyMetadata(Visibility.Visible));



        public static readonly DependencyProperty PlateRowBackgroundProperty = DependencyProperty.Register(
            nameof(PlateRowBackground),
            typeof(Brush),
            typeof(UserControl),
            new PropertyMetadata((SolidColorBrush)new BrushConverter().ConvertFrom("#d6d5ce")));



        public static readonly DependencyProperty FontStampBackgroundProperty = DependencyProperty.Register(
            nameof(FontStampBackground),
            typeof(Brush),
            typeof(UserControl),
            new PropertyMetadata((SolidColorBrush)new BrushConverter().ConvertFrom("#55534d")));





    }
}
