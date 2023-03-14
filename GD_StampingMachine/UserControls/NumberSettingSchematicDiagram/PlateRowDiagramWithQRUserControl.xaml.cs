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
    public partial class PlateRowDiagramWithQRUserControl : UserControl
    {
        public PlateRowDiagramWithQRUserControl()
        {
            InitializeComponent();



        }



        public int SequenceCount
        {
            get => (int)GetValue(SequenceCountProperty);
            set => SetValue(SequenceCountProperty, value);
        }

        public static readonly DependencyProperty SequenceCountProperty = DependencyProperty.Register(
            nameof(SequenceCount),
            typeof(int),
            typeof(PlateRowDiagramWithQRUserControl),
            new PropertyMetadata());

         
        public ObservableCollection<int> PlateNumberList
        {
            get => (ObservableCollection<int>)GetValue(PlateNumberListProperty);
            set => SetValue(PlateNumberListProperty, value);
        }

        public static readonly DependencyProperty PlateNumberListProperty = DependencyProperty.Register(
            nameof(PlateNumberList),
            typeof(ObservableCollection<int>),
            typeof(PlateRowDiagramWithQRUserControl),
            new PropertyMetadata());





        public SpecialSequenceEnum SpecialSequence
        {
            get => (SpecialSequenceEnum)GetValue(SpecialSequenceProperty);
            set => SetValue(SpecialSequenceProperty, value);
        }

        public static readonly DependencyProperty SpecialSequenceProperty = DependencyProperty.Register(
            nameof(SpecialSequence),
            typeof(SpecialSequenceEnum),
            typeof(PlateRowDiagramWithQRUserControl),
            new PropertyMetadata());



        public HorizontalAlignEnum PlateHorizontalAlignment
        {
            get => (HorizontalAlignEnum)GetValue(PlateHorizontalAlignmentProperty);
            set => SetValue(PlateHorizontalAlignmentProperty, value);
        }

        public static readonly DependencyProperty PlateHorizontalAlignmentProperty = DependencyProperty.Register(
            nameof(PlateHorizontalAlignment),
            typeof(HorizontalAlignEnum),
            typeof(PlateRowDiagramWithQRUserControl),
            new PropertyMetadata());


        public VerticalAlignEnum PlateVerticalAlignment
        {
            get => (VerticalAlignEnum)GetValue(PlateVerticalAlignmentProperty);
            set => SetValue(PlateVerticalAlignmentProperty, value);
        }

        public static readonly DependencyProperty PlateVerticalAlignmentProperty = DependencyProperty.Register(
            nameof(PlateVerticalAlignment),
            typeof(VerticalAlignEnum),
            typeof(PlateRowDiagramWithQRUserControl),
            new PropertyMetadata());










    }
}
