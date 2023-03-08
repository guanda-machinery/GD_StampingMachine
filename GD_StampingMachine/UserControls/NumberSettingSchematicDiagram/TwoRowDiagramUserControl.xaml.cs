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








    }
}
