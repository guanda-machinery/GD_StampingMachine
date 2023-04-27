using DevExpress.CodeParser;
using DevExpress.Mvvm.DataAnnotations;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GD_CommonLibrary;

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

    public class TypeSettingSettingViewModel : BaseViewModelWithLog
    {

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_TypeSettingSettingViewModel");

        public TypeSettingSettingViewModel(TypeSettingSettingModel _typeSettingSetting)
        {
            TypeSettingSetting = _typeSettingSetting;
            var ProjectDistributeModelList = new List<ProjectDistributeModel>()
              {
                  new ProjectDistributeModel()
                  {
                      ProjectDistributeName="排版專案一",
                      CreatedDate = DateTime.Now,

                      ProductProjectVMObservableCollection = TypeSettingSetting.ProductProjectVMObservableCollection,
                      SeparateBoxVMObservableCollection = TypeSettingSetting.SeparateBoxVMObservableCollection
                  },
                  new ProjectDistributeModel()
                  {
                      ProjectDistributeName="排版專案二",
                      CreatedDate = DateTime.Now,
                      ProductProjectVMObservableCollection = TypeSettingSetting.ProductProjectVMObservableCollection,
                      SeparateBoxVMObservableCollection = TypeSettingSetting.SeparateBoxVMObservableCollection
                  },
              };
            ProjectDistributeModelList.ForEach(projectDistribute =>
            {
                ProjectDistributeVMObservableCollection.Add(new ProjectDistributeViewModel(projectDistribute));
            });
            ProjectDistributeVM = ProjectDistributeVMObservableCollection.FirstOrDefault();
        }

        public TypeSettingSettingModel TypeSettingSetting { get; }

        /// <summary>
        /// 建立用的model
        /// </summary>
        public ProjectDistributeModel NewProjectDistribute
        {
            get; set;
        } = new();



        public ICommand CreateProjectDistributeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    AddLogData("btnAddProject");

                    NewProjectDistribute.CreatedDate = DateTime.Now;
                    var Clone = NewProjectDistribute.DeepCloneByJson();
                    Clone.ProductProjectVMObservableCollection = TypeSettingSetting.ProductProjectVMObservableCollection;
                    Clone.SeparateBoxVMObservableCollection = TypeSettingSetting.SeparateBoxVMObservableCollection;
                    ProjectDistributeVMObservableCollection.Add(new ProjectDistributeViewModel(Clone));
                });
            }
        }

        private bool _addProjectDistributeDarggableIsPopup;
        public bool AddProjectDistributeDarggableIsPopup { get=> _addProjectDistributeDarggableIsPopup; set { _addProjectDistributeDarggableIsPopup = value;OnPropertyChanged(); } }






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
                    ProjectDistributeVM.PartsParameterVMObservableCollectionRefresh();
                    //ProjectDistributeVM.RefreshCommand();
                }
            });
        }

        private ProjectDistributeViewModel _projectDistributeVM;
        public ProjectDistributeViewModel ProjectDistributeVM { get=> _projectDistributeVM; set { _projectDistributeVM = value; OnPropertyChanged(); } }


    }
}
