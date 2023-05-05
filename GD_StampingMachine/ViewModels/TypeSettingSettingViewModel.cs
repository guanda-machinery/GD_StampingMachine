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
using Newtonsoft.Json;
using DevExpress.Mvvm.Native;
using DevExpress.Data.Extensions;
using GD_CommonLibrary.Method;
using GD_StampingMachine.Method;

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
    /// <summary>
    /// 排版設定
    /// </summary>
    public class TypeSettingSettingViewModel : BaseViewModelWithLog
    {

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_TypeSettingSettingViewModel");

        public TypeSettingSettingViewModel(TypeSettingSettingModel _typeSettingSetting)
        {
            if (_typeSettingSetting == null)
                _typeSettingSetting = new TypeSettingSettingModel();

            TypeSettingSetting = _typeSettingSetting;


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


        [JsonIgnore]
        public ICommand CreateProjectDistributeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    AddLogData("btnAddProject");

                    if(NewProjectDistribute.ProjectDistributeName == null)
                    {
                        MethodWinUIMessageBox.CanNotCreateProjectFileNameIsEmpty();
                        return;
                    }
               
                    if(ProjectDistributeVMObservableCollection.FindIndex(x=>x.ProjectDistributeName == NewProjectDistribute.ProjectDistributeName) !=-1)
                    {
                        MethodWinUIMessageBox.CanNotCreateProject(NewProjectDistribute.ProjectDistributeName); 
                        return;
                    }

                    NewProjectDistribute.CreatedDate = DateTime.Now;
                    var Clone = NewProjectDistribute.DeepCloneByJson();
                    Clone.ProductProjectVMObservableCollection = TypeSettingSetting.ProductProjectVMObservableCollection;
                    Clone.SeparateBoxVMObservableCollection = TypeSettingSetting.SeparateBoxVMObservableCollection;
                    ProjectDistributeVMObservableCollection.Add(new ProjectDistributeViewModel(Clone));
                    var Model_IEnumerable = ProjectDistributeVMObservableCollection.Select(x => x.ProjectDistribute).ToList();
                    //存檔
                    JsonHM.WriteProjectDistributeListJson(Model_IEnumerable);

                });
            }
        }

        private bool _addProjectDistributeDarggableIsPopup;
        public bool AddProjectDistributeDarggableIsPopup { get=> _addProjectDistributeDarggableIsPopup; set { _addProjectDistributeDarggableIsPopup = value;OnPropertyChanged(); } }






        public ObservableCollection<ProjectDistributeViewModel> ProjectDistributeVMObservableCollection { get; set; }= new();
        [JsonIgnore]
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
