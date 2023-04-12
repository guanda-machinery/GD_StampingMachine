using DevExpress.Mvvm.DataAnnotations;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace GD_StampingMachine.ViewModels
{
    public class TypeSettingSettingModel
    {    
        /// <summary>
        /// 製品清單
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get; set; }
        /// <summary>
        /// 盒子列表
        /// </summary>
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; }
    }

    public class TypeSettingSettingViewModel : ViewModelBase
    {
        public TypeSettingSettingViewModel(TypeSettingSettingModel _typeSettingSetting )
        {
              var ProjectDistributeModelList = new List<ProjectDistributeModel>() 
              {
                  new ProjectDistributeModel()
                  {
                      ProjectDistributeName="Test1",
                      ProductProjectVMObservableCollection = _typeSettingSetting.ProductProjectVMObservableCollection,
                      SeparateBoxVMObservableCollection = _typeSettingSetting.SeparateBoxVMObservableCollection
                  },                  
                  new ProjectDistributeModel()
                  {
                      ProjectDistributeName="Test2",
                      ProductProjectVMObservableCollection = _typeSettingSetting.ProductProjectVMObservableCollection,
                      SeparateBoxVMObservableCollection = _typeSettingSetting.SeparateBoxVMObservableCollection
                  },
              };

            ProjectDistributeModelList.ForEach(projectDistribute =>
            {
                ProjectDistributeVMObservableCollection.Add(new ProjectDistributeViewModel(projectDistribute));
            });
            ProjectDistributeVM = ProjectDistributeVMObservableCollection.First();
        }

        public ObservableCollection<ProjectDistributeViewModel> ProjectDistributeVMObservableCollection { get; set; }= new();


        public DevExpress.Mvvm.ICommand<DevExpress.Mvvm.Xpf.RowClickArgs> RowDoubleClickCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<DevExpress.Mvvm.Xpf.RowClickArgs>((DevExpress.Mvvm.Xpf.RowClickArgs args) =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProjectDistributeViewModel ProjectItem)
                {
                    if (ProjectDistributeVM != ProjectItem)
                        ProjectDistributeVM = ProjectItem;
                    ProjectDistributeVM.IsInDistributePage = true;
                }
            });
        }

        private ProjectDistributeViewModel _projectDistributeVM;
        public ProjectDistributeViewModel ProjectDistributeVM { get=> _projectDistributeVM; set { _projectDistributeVM = value; OnPropertyChanged(); } }


    }
}
