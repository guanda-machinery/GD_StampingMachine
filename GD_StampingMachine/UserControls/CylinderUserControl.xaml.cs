using GD_CommonLibrary;
using GD_StampingMachine.UserControls.CustomControls;
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
    /// CylinderControlUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class CylinderUserControl : UserControl
    {
        public CylinderUserControl()
        {
            InitializeComponent();
        }


        public ICommand CylinderUpCommand
        {
            get { return (ICommand)GetValue(CylinderUpCommandProperty); }
            set { SetValue(CylinderUpCommandProperty, value); }
        }
        public static readonly DependencyProperty CylinderUpCommandProperty =
        DependencyProperty.Register(nameof(CylinderUpCommand), typeof(ICommand), typeof(CylinderUserControl), new PropertyMetadata(UpCommandChanged));
    
        private static void UpCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CylinderUserControl uControl = (CylinderUserControl)d;
           // uControl.ViewModel.CylinderUpCommand = e.NewValue as ICommand;
        }

    }




}
