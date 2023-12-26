using System.Windows;
using System.Windows.Controls;

namespace GD_StampingMachine.UserControls
{
    /// <summary>
    /// StampingBoxPartsUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class StampingBoxPartsUserControl : UserControl
    {
        public StampingBoxPartsUserControl()
        {
            InitializeComponent();
        }

        public bool BoxPartsParameterGridControlAllowDragDrop
        {
            set { SetValue(BoxPartsParameterGridControlAllowDragDropProperty, value); }
            get { return (bool)GetValue(BoxPartsParameterGridControlAllowDragDropProperty); }
        }
        public static readonly DependencyProperty BoxPartsParameterGridControlAllowDragDropProperty = DependencyProperty.Register(nameof(BoxPartsParameterGridControlAllowDragDrop),
                typeof(bool),
                typeof(StampingBoxPartsUserControl), new FrameworkPropertyMetadata());

        public bool BoxPartsParameterGridControlAllowDrop
        {
            set { SetValue(BoxPartsParameterGridControlAllowDropProperty, value); }
            get { return (bool)GetValue(BoxPartsParameterGridControlAllowDropProperty); }
        }
        public static readonly DependencyProperty BoxPartsParameterGridControlAllowDropProperty = DependencyProperty.Register(nameof(BoxPartsParameterGridControlAllowDrop),
                typeof(bool),
                typeof(StampingBoxPartsUserControl), new FrameworkPropertyMetadata());






        /*public bool GridControl_MachiningStatusIsVisible
        {
            get => (bool)GetValue(GridControl_MachiningStatusIsVisibleProperty);
            set => SetValue(GridControl_MachiningStatusIsVisibleProperty, value);
        }

        public static readonly DependencyProperty GridControl_MachiningStatusIsVisibleProperty = 
            DependencyProperty.Register(
            nameof(GridControl_MachiningStatusIsVisible),
            typeof(bool),
            typeof(StampingBoxPartsUserControl),
            new PropertyMetadata(GridControl_MachiningStatusIsVisibleDependencyPropertyChanged));


        private static void GridControl_MachiningStatusIsVisibleDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _UserControl = (d as GD_StampingMachine.UserControls.StampingBoxPartsUserControl);

            _UserControl.GridControl_MachiningStatusColumn.Visible = (bool)e.NewValue;

        }*/






    }
}
