using DevExpress.Mvvm.Native;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    public class ProductSettingViewModel : ViewModelBase
    {
        private bool _addProjectDarggableIsPopup;
        public bool AddProjectDarggableIsPopup
        {
            get
            {
                return _addProjectDarggableIsPopup;
            }
            set
            {
                _addProjectDarggableIsPopup = value;
                OnPropertyChanged(nameof(AddProjectDarggableIsPopup));
            }
        }

        public string ProjectPathText
        {
            get
            {
                return CreatedProjectModel.ProjectPath;
            }
            set
            {
                CreatedProjectModel.ProjectPath = value;
                OnPropertyChanged(nameof(ProjectPathText));
            }
        }


        private ObservableCollection<ProductProjectViewModel> _productProjectVMObservableCollection;
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get
            {
                if (_productProjectVMObservableCollection == null)
                    _productProjectVMObservableCollection = new ObservableCollection<ProductProjectViewModel>();
                return _productProjectVMObservableCollection;
            }
            set
            {
                _productProjectVMObservableCollection = value;
                OnPropertyChanged(nameof(ProductProjectVMObservableCollection));
            }
        }





        public ICommand SetProjectFolder
        {
            get => new RelayCommand(() =>
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    DialogResult result = dialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        ProjectPathText = dialog.SelectedPath;
                    }
                }
            });
         }

        public ICommand CreateProjectCommand
        {
            get=> new RelayCommand(() =>
            {
                //若不clone會導致資料互相繫結
                ProductProjectVMObservableCollection.Add(new ProductProjectViewModel()
                {
                    ProductProject = (ProductProjectModel)CreatedProjectModel.Clone()
                });
                OnPropertyChanged(nameof(ProductProjectVMObservableCollection));
              //  OnPropertyChanged(nameof(Search_ProductProjectVMObservableCollection));
                /*
                ProductProjectVMObservableCollection.Add(new ProductProjectViewModel()
                {
                    ProductProject = new ProductProjectModel()
                    { 
                        Name="創典科技總公司基地(new)",
                        Number = "newAS001",
                        Form ="newQR",
                        CreateTime=DateTime.Now
                    }

                });
                */
            });
        }

        //   public ProductProjectModel CreatedProjectModel { get; set;} = new ProductProjectModel();
        public ProductProjectModel CreatedProjectModel { get; set;} = new ProductProjectModel()
           {
               Name = "NewProject",
               Number = "newAS001",
               Form = "newQR",
               CreateTime = DateTime.Now
           };

























    }
}
