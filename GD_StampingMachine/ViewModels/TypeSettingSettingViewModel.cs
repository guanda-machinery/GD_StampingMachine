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
using CommunityToolkit.Mvvm.Input;
using System.Threading;
using System.Windows;

namespace GD_StampingMachine.ViewModels
{
    //public class TypeSettingSettingModel
    //{

        /// <summary>
        /// 製品清單
        /// </summary>
        //public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get; set; } 
        /// <summary>
        /// 盒子列表
        /// </summary>
        //public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; } 
    //}
    /// <summary>
    /// 排版設定
    /// </summary>
    public class TypeSettingSettingViewModel : GD_CommonLibrary.BaseViewModel
    {
        StampingMachineJsonHelper JsonHM = new StampingMachineJsonHelper();

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_TypeSettingSettingViewModel");

        public TypeSettingSettingViewModel()
        {

        }


        /// <summary>
        /// 建立用的model
        /// </summary>
        public ProjectDistributeModel NewProjectDistribute
        {
            get; set;
        } = new();


        public ProductSettingViewModel ProductSettingVM { get => Singletons.StampingMachineSingleton.Instance.ProductSettingVM; }
        public ParameterSettingViewModel ParameterSettingVM { get => Singletons.StampingMachineSingleton.Instance.ParameterSettingVM; }


        [JsonIgnore]
        public AsyncRelayCommand CreateProjectDistributeCommand
        {
            get
            {
                return new AsyncRelayCommand(async (CancellationToken token) =>
                {
                    Singletons.LogDataSingleton.Instance.AddLogData(this.ViewModelName, "btnAddProject");
                    if (NewProjectDistribute.ProjectDistributeName == null)
                    {
                        await MethodWinUIMessageBox.CanNotCreateProjectFileNameIsEmpty();
                        return;
                    }
                    if (ProjectDistributeVMObservableCollection.FindIndex(x => x.ProjectDistributeName == NewProjectDistribute.ProjectDistributeName) != -1)
                    {
                        await MethodWinUIMessageBox.CanNotCreateProject(NewProjectDistribute.ProjectDistributeName);
                        return;
                    }

                    NewProjectDistribute.CreatedDate = DateTime.Now;
                    var Clone = NewProjectDistribute.DeepCloneByJson();
                    Clone.ProductProjectVMObservableCollection = ProductSettingVM.ProductProjectVMObservableCollection;
                    Clone.SeparateBoxVMObservableCollection = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection;
                    ProjectDistributeVMObservableCollection.Add(new ProjectDistributeViewModel(Clone));
                    var Model_IEnumerable = ProjectDistributeVMObservableCollection.Select(x => x.ProjectDistribute).ToList();
                    //存檔

                  await  JsonHM.WriteProjectDistributeListJsonAsync(Model_IEnumerable);

                }, () => !CreateProjectDistributeCommand.IsRunning);
            }
        }






        /// <summary>
        /// 切換工程號
        /// </summary>
        //[JsonIgnore]
        //public ICommand ChangeProjectDistributeCommand{get => Singletons.StampingMachineSingleton.Instance.MachineMonitorVM.ProjectDistributeVMChangeCommand; }


        private AsyncRelayCommand<object> _deleteProjectDistributeCommand;
        /// <summary>
        /// 刪除排版專案 並將盒子內的所有東西釋放回專案
        /// </summary>
        [JsonIgnore]
        public AsyncRelayCommand<object> DeleteProjectDistributeCommand
        {
            get => _deleteProjectDistributeCommand??=new AsyncRelayCommand<object>(async obj =>
            {
                if (obj is IList<ProjectDistributeViewModel> projectDistributeGridControlSelectedItems)
                {
                    var DelProjectsString = projectDistributeGridControlSelectedItems.ToList().Select(x => x.ProjectDistributeName).ExpandToString();
                    if (await MethodWinUIMessageBox.AskDelProject(DelProjectsString) is MessageBoxResult.Yes)
                    {
                        var Removed_List = new List<int>();
                        foreach (var _selectItem in projectDistributeGridControlSelectedItems)
                        {
                            _selectItem.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.ForEach(obj =>
                            {
                                if (_selectItem.ProjectDistributeName == _selectItem.StampingBoxPartsVM.ProjectDistributeName)
                                {
                                    obj.DistributeName = null;
                                    obj.BoxIndex = null;
                                }
                            });
                            await  _selectItem.SaveProductProjectVMObservableCollectionAsync();

                            var F_Index = ProjectDistributeVMObservableCollection.FindIndex(x => x == _selectItem);
                            Removed_List.Add(F_Index);
                        }
                        var DescendingRemoved_List = Removed_List.OrderByDescending(x => x);
                        foreach (var DescendingRemovedIndex in DescendingRemoved_List)
                        {
                            if (DescendingRemovedIndex != -1)
                            {
                                ProjectDistributeVMObservableCollection.RemoveAt(DescendingRemovedIndex);
                            }
                        }
                    await    new StampingMachineJsonHelper().WriteProjectDistributeListJsonAsync(ProjectDistributeVMObservableCollection.Select(x => x.ProjectDistribute).ToList());
                    }
                }
            });
        }



        private bool _addProjectDistributeDarggableIsPopup;
        public bool AddProjectDistributeDarggableIsPopup { get=> _addProjectDistributeDarggableIsPopup; set { _addProjectDistributeDarggableIsPopup = value;OnPropertyChanged(); } }



        public ObservableCollection<ProjectDistributeViewModel> ProjectDistributeSelectedItems { get; set; } = new();
        /// <summary>
        /// 全加工專案
        /// </summary>
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
                }
            });
        }

        private ProjectDistributeViewModel _projectDistributeVM;
        /// <summary>
        /// 準備加工的排版專案
        /// </summary>
        public ProjectDistributeViewModel ProjectDistributeVM { get=> _projectDistributeVM; set { _projectDistributeVM = value; OnPropertyChanged(); } }


    }
}
