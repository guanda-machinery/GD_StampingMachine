using GD_StampingMachine.Enum;
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
    /// NumberSettingSchematicDiagramUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class TwoRowDiagramUserControl : UserControl
    {
        public TwoRowDiagramUserControl()
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
            typeof(TwoRowDiagramUserControl),
            new PropertyMetadata());

        public SpecialSequenceEnum SpecialSequence
        {
            get => (SpecialSequenceEnum)GetValue(SpecialSequenceProperty);
            set => SetValue(SpecialSequenceProperty, value);
        }

        public static readonly DependencyProperty SpecialSequenceProperty = DependencyProperty.Register(
            nameof(SpecialSequence),
            typeof(SpecialSequenceEnum),
            typeof(TwoRowDiagramUserControl),
            new PropertyMetadata());



        public HorizontalAlignment PlateHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(PlateHorizontalAlignmentProperty);
            set => SetValue(PlateHorizontalAlignmentProperty, value);
        }

        public static readonly DependencyProperty PlateHorizontalAlignmentProperty = DependencyProperty.Register(
            nameof(PlateHorizontalAlignment),
            typeof(HorizontalAlignment),
            typeof(TwoRowDiagramUserControl),
            new PropertyMetadata());


        public VerticalAlignment PlateVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(PlateVerticalAlignmentProperty);
            set => SetValue(PlateVerticalAlignmentProperty, value);
        }

        public static readonly DependencyProperty PlateVerticalAlignmentProperty = DependencyProperty.Register(
            nameof(PlateVerticalAlignment),
            typeof(VerticalAlignment),
            typeof(TwoRowDiagramUserControl),
            new PropertyMetadata());








    }
}
