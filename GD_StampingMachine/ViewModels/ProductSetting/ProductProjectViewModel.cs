using DevExpress.Data;
using DevExpress.Mvvm;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    public class ProductProjectViewModel : ViewModelBase
    {
        public ProductProjectModel ProductProject { get; set; } = new ProductProjectModel();

        private RelayParameterizedCommand _projectEditCommand;
        public RelayParameterizedCommand ProjectEditCommand
        {
            get
            {
                if (_projectEditCommand == null)
                {
                    _projectEditCommand = new RelayParameterizedCommand(obj =>
                    {

                    });
                }
                return _projectEditCommand;
            }
            set
            {
                _projectEditCommand = value;
                OnPropertyChanged(nameof(ProjectEditCommand));
            }
        }

        private RelayParameterizedCommand _projectDeleteCommand;
        public RelayParameterizedCommand ProjectDeleteCommand
        {
            get
            {
                if (_projectDeleteCommand == null)
                {
                    _projectDeleteCommand = new RelayParameterizedCommand(obj =>
                    {
                       if (obj is GridControl ObjGridControl)
                        {
                            if (ObjGridControl.ItemsSource is ObservableCollection<ProductProjectViewModel> GridItemSource)
                            {
                                var MessageBoxReturn = WinUIMessageBox.Show(null,
                                    (string)Application.Current.TryFindResource("Text_AskDelProject") +
                                    "\r\n" +
                                    $"{this.ProductProject.Number} - {this.ProductProject.Name}"+
                                    "?"
                                    ,
                                    (string)Application.Current.TryFindResource("Text_notify"),
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Exclamation,
                                    MessageBoxResult.None,
                                    MessageBoxOptions.None,
                                    DevExpress.Xpf.Core.FloatingMode.Window);

                                if (MessageBoxReturn == MessageBoxResult.Yes)
                                    GridItemSource.Remove(this);
                            }
                        }
                    });
                }
                return _projectDeleteCommand;
            }
            set
            {
                _projectDeleteCommand = value;
                OnPropertyChanged(nameof(ProjectDeleteCommand));
            }
        }

    }
}
