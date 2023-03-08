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
    /// SettingButtonUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class SettingButtonUserControl : UserControl
    {
        public SettingButtonUserControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MainSilderValueProperty = DependencyProperty.Register(
            nameof(MainSilderValue),
            typeof(double),
            typeof(SettingButtonUserControl),
            new PropertyMetadata());

        public ICommand MainSilderValue
        {
            get => (ICommand)GetValue(MainSilderValueProperty);
            set => SetValue(MainSilderValueProperty, value);
        }


    }
}
