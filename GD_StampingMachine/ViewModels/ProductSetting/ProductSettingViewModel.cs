using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    public class ProductSettingViewModel : ViewModelBase
    {




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

        public ICommand CreateProjectCommand
        {
            get=> new RelayCommand(() =>
            {
                //若不clone會導致資料互相繫結
                ProductProjectVMObservableCollection.Add(new ProductProjectViewModel()
                {
                    ProductProject = (ProductProjectModel)CreatedProjectModel.Clone()
                });

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
               Name = "創典科技總公司基地(new)",
               Number = "newAS001",
               Form = "newQR",
               CreateTime = DateTime.Now
           };





    }
}
