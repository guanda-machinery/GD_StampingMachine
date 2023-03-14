using DevExpress.Mvvm.Native;
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

        private string _searchText;
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                OnPropertyChanged(nameof(Search_ProductProjectVMObservableCollection));
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


        public ObservableCollection<ProductProjectViewModel> Search_ProductProjectVMObservableCollection
        {
            get
            {

                if (!string.IsNullOrEmpty(SearchText))
                {
                    var searchText_NoneUpper = SearchText.ToLower();
                    var SearchList = ProductProjectVMObservableCollection.Where
                        (x => x.ProductProject.Name.ToLower().Contains(searchText_NoneUpper) ||
                        x.ProductProject.Number.ToLower().Contains(searchText_NoneUpper) || 
                        x.ProductProject.Form.ToLower().Contains(searchText_NoneUpper));

  
                        return SearchList.ToObservableCollection();

                }
                else
                    return ProductProjectVMObservableCollection;
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
                OnPropertyChanged(nameof(ProductProjectVMObservableCollection));
                OnPropertyChanged(nameof(Search_ProductProjectVMObservableCollection));
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
