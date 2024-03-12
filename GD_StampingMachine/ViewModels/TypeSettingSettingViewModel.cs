using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels.ProductSetting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace GD_StampingMachine.ViewModels
{
    //public class TypeSettingSettingModel
    //{

    /// <summary>
    /// 製品清單
    /// </summary>
    //public ObservableCollection<ProductProjectViewModel> ProductProjectVMCollection { get; set; } 
    /// <summary>
    /// 盒子列表
    /// </summary>
    //public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; } 
    //}
    /// <summary>
    /// 排版設定
    /// </summary>
    public class TypeSettingSettingViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        StampingMachineJsonHelper JsonHM = new StampingMachineJsonHelper();

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_TypeSettingSettingViewModel");

        public TypeSettingSettingViewModel()
        {

        }

        //private ProjectDistributeViewModel? _newProjectDistributeVM;
        /// <summary>
        /// 建立用的model
        /// </summary>
        public ProjectDistributeModel NewProjectDistribute { get; set; } = new();
        public ProductSettingViewModel ProductSettingVM { get => Singletons.StampingMachineSingleton.Instance.ProductSettingVM; }
        public ParameterSettingViewModel ParameterSettingVM { get => Singletons.StampingMachineSingleton.Instance.ParameterSettingVM; }



        private AsyncRelayCommand? _createProjectDistributeCommand;
        [JsonIgnore]
        public AsyncRelayCommand CreateProjectDistributeCommand
        {
            get
            {
                return _createProjectDistributeCommand??=new(async (CancellationToken token) =>
                {
                    _ = Singletons.LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, "btnAddProject");
                    if (string.IsNullOrEmpty(NewProjectDistribute.ProjectDistributeName))
                    {
                        await MethodWinUIMessageBox.CanNotCreateProjectFileNameIsEmptyAsync();
                        return;
                    }
                    if (ProjectDistributeVMObservableCollection.FindIndex(x => x.ProjectDistributeName == NewProjectDistribute.ProjectDistributeName) != -1)
                    {
                        await MethodWinUIMessageBox.CanNotCreateProjectAsync(null, NewProjectDistribute.ProjectDistributeName);
                        return;
                    }

                    NewProjectDistribute.CreatedDate = DateTime.Now;
                    var Clone = NewProjectDistribute.DeepCloneByJson();

                    ProjectDistributeViewModel NewProjectDistributeVM = new(
                        NewProjectDistribute,
                        ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection,
                        ProductSettingVM.ProductProjectVMCollection);

                    ProjectDistributeVMObservableCollection.Add(NewProjectDistributeVM);
                    var Model_IEnumerable = ProjectDistributeVMObservableCollection.Select(x => x.ProjectDistribute).ToList();
                    //存檔
                    await JsonHM.WriteProjectDistributeListJsonAsync(Model_IEnumerable);

                }, () => !CreateProjectDistributeCommand.IsRunning);
            }
        }






        /// <summary>
        /// 切換工程號
        /// </summary>
        //[JsonIgnore]
        //public ICommand ChangeProjectDistributeCommand{get => Singletons.StampingMachineSingleton.Instance.MachineMonitorVM.ProjectDistributeVMChangeCommand; }


        private AsyncRelayCommand<object>? _deleteProjectDistributeCommand;
        /// <summary>
        /// 刪除排版專案 並將盒子內的所有東西釋放回專案
        /// </summary>
        [JsonIgnore]
        public AsyncRelayCommand<object> DeleteProjectDistributeCommand
        {
            get => _deleteProjectDistributeCommand ??= new AsyncRelayCommand<object>(async obj =>
            {
                if (obj is IList<ProjectDistributeViewModel> projectDistributeGridControlSelectedItems)
                {
                    var DelProjectsString = projectDistributeGridControlSelectedItems.ToList().Select(x => x.ProjectDistributeName).ExpandToString();
                    if (await MethodWinUIMessageBox.AskDelProjectAsync(null, DelProjectsString))
                    {
                        var Removed_List = new List<int>();
                        foreach (var _selectItem in projectDistributeGridControlSelectedItems)
                        {
                            _selectItem.StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x=>x.BoxPartsParameterVMCollection).ForEach(obj =>
                            {
                                if (_selectItem.ProjectDistributeName == _selectItem.StampingBoxPartsVM.ProjectDistributeName)
                                {
                                    obj.DistributeName = null;
                                    obj.BoxIndex = null;
                                }
                            });
                            await _selectItem.SaveProductProjectVMCollectionAsync();

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
                        await new StampingMachineJsonHelper().WriteProjectDistributeListJsonAsync(ProjectDistributeVMObservableCollection.Select(x => x.ProjectDistribute).ToList());
                    }
                }
            });
        }



        private bool _addProjectDistributeDarggableIsPopup;
        public bool AddProjectDistributeDarggableIsPopup { get => _addProjectDistributeDarggableIsPopup; set { _addProjectDistributeDarggableIsPopup = value; OnPropertyChanged(); } }



        public ObservableCollection<ProjectDistributeViewModel> ProjectDistributeSelectedItems { get; set; } = new();
       
        
        /// <summary>
        /// 全加工專案
        /// </summary>
        public ObservableCollection<ProjectDistributeViewModel> ProjectDistributeVMObservableCollection { get; set; } = new();


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
                    //ProjectDistributeVM.PartsParameterVMCollectionRefresh();
                }
            });
        }

        private ProjectDistributeViewModel? _projectDistributeVM;
        /// <summary>
        /// 準備加工的排版專案
        /// </summary>
        public ProjectDistributeViewModel ProjectDistributeVM { get => _projectDistributeVM; set { _projectDistributeVM = value; OnPropertyChanged(); } }


    }
}
