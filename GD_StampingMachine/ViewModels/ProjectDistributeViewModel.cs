using DevExpress.Data.Extensions;
using DevExpress.Utils.Extensions;
using DevExpress.Xpf.Core;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using GD_StampingMachine.Model.ProductionSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{

    public class ProjectDistributeModel
    {
        public string ProjectDistributeName { get; set; }
        /// <summary>
        /// 製品清單
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get; set; }
        /// <summary>
        /// 盒子
        /// </summary>
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; }
    }


    public class ProjectDistributeViewModel : ViewModelBase
    {
        public ProjectDistributeViewModel(ProjectDistributeModel _projectDistribute)
        {
            ProjectDistribute = _projectDistribute;
        }
        protected ProjectDistributeModel ProjectDistribute = new ProjectDistributeModel();

        private bool _IsInDistributePage = false;
        public bool IsInDistributePage { get => _IsInDistributePage; set { _IsInDistributePage = value; OnPropertyChanged(); } }


        public string ProjectDistributeName { get => ProjectDistribute.ProjectDistributeName; }

        // private ObservableCollection<ProductProjectViewModel> _productProjectVMObservableCollection;
        private ProductProjectViewModel _selectedProductProjectVM;
        public ProductProjectViewModel SelectedProductProjectVM
        {
            get => _selectedProductProjectVM;
            set
            {
                _selectedProductProjectVM = value;
                if (_selectedProductProjectVM != null)
                {
                    PartsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();
                    _selectedProductProjectVM.PartsParameterVMObservableCollection.Where(x => x.BoxNumber == null && x.DistributeName  == null).ForEach(obj =>
                    {
                        PartsParameterVMObservableCollection.Add(obj);
                    });
                }
                OnPropertyChanged(nameof(SelectedProductProjectVM));
            }
        }

        /// <summary>
        /// 製品
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get
            {
                if (ProjectDistribute.ProductProjectVMObservableCollection == null)
                    ProjectDistribute.ProductProjectVMObservableCollection = new ObservableCollection<ProductProjectViewModel>();
                return ProjectDistribute.ProductProjectVMObservableCollection;
            }
            set
            {
                ProjectDistribute.ProductProjectVMObservableCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PartsParameterViewModel> _partsParameterVMObservableCollection;
        /// <summary>
        /// GridControl ABC參數 沒放進箱子內的
        /// </summary>
        public ObservableCollection<PartsParameterViewModel> PartsParameterVMObservableCollection
        {
            get
            {
                _partsParameterVMObservableCollection ??= new ObservableCollection<PartsParameterViewModel>();
                return _partsParameterVMObservableCollection;
            }
            set
            {
                _partsParameterVMObservableCollection = value;
                OnPropertyChanged(nameof(PartsParameterVMObservableCollection));
            }
        }


        /// <summary>
        /// 盒子內的專案
        /// </summary>
        private ObservableCollection<PartsParameterViewModel> _boxPartsParameterVMObservableCollection;
        public ObservableCollection<PartsParameterViewModel> BoxPartsParameterVMObservableCollection
        {
            get
            {
                if (_boxPartsParameterVMObservableCollection == null)
                    _boxPartsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();
                return _boxPartsParameterVMObservableCollection;
            }
            set
            {
                _boxPartsParameterVMObservableCollection = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 盒子列表
        /// </summary>
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection
        {
            get => ProjectDistribute.SeparateBoxVMObservableCollection;
            set
            {
                ProjectDistribute.SeparateBoxVMObservableCollection = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 選擇的盒子
        /// </summary>
        public ParameterSetting.SeparateBoxViewModel SelectedSeparateBoxVM{get;set;}



        /// <summary>
        /// 選擇盒子觸發的行為
        /// </summary>
        public ICommand SeparateBoxVMObservableCollectionelectionChangedCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                if (obj is System.Windows.Controls.SelectionChangedEventArgs e)
                {
                    if (e.AddedItems.Count > 0)
                    {
                        if (e.AddedItems[0] is GD_StampingMachine.ViewModels.ParameterSetting.SeparateBoxViewModel NewSeparateBoxVM)
                        {
                            BoxPartsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();
                            ProductProjectVMObservableCollection.ForEach(productProject =>
                            {
                                productProject.PartsParameterVMObservableCollection.ForEach((productProjectPartViewModel) =>
                                {
                                    if (productProjectPartViewModel.BoxNumber.HasValue)
                                        if (NewSeparateBoxVM.BoxNumber == productProjectPartViewModel.BoxNumber.Value && productProjectPartViewModel.DistributeName == this.ProjectDistributeName)
                                            if (!BoxPartsParameterVMObservableCollection.Contains(productProjectPartViewModel))
                                                BoxPartsParameterVMObservableCollection.Add(productProjectPartViewModel);
                                });
                            });
                        }
                    }
                    
                }
             });
        }
        





        public ICommand NoneBox_OnDragRecordOverCommand
        {
            get => new RelayParameterizedCommand(obj =>
                {
                    if (obj is DevExpress.Xpf.Core.DragRecordOverEventArgs e)
                    {
                        e.Effects = System.Windows.DragDropEffects.None;
                        var DragDropData = e.Data.GetData(typeof(RecordDragDropData)) as DevExpress.Xpf.Core.RecordDragDropData;
                        foreach (var _record in DragDropData.Records)
                        {
                            if (_record is PartsParameterViewModel PartsParameterVM)
                            {
                                if (SelectedProductProjectVM != null)
                                {
                                    if (PartsParameterVM.ProjectName != SelectedProductProjectVM.ProductProjectName)
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        e.Effects = System.Windows.DragDropEffects.Move;
                    }
                });
        }

        public ICommand Box_OnDragRecordOverCommand
        {
            get
            {
                return new RelayParameterizedCommand(obj =>
                {
                    if (obj is DevExpress.Xpf.Core.DragRecordOverEventArgs e)
                    {
                        if(SelectedSeparateBoxVM != null)
                            e.Effects = System.Windows.DragDropEffects.Move;
                    }
                });
            }
        }


        /// <summary>
        /// 從箱子拿出來  
        /// </summary>
        public ICommand NoneBox_OnDropRecordCommand
        {
            get
            {
                return new RelayParameterizedCommand(obj =>
                {
                    if (obj is DevExpress.Xpf.Core.DropRecordEventArgs e)
                    {
                        var DragDropData = e.Data.GetData(typeof(RecordDragDropData)) as DevExpress.Xpf.Core.RecordDragDropData;
                        foreach (var _record in DragDropData.Records)
                        {
                            if (_record is PartsParameterViewModel PartsParameterVM)
                            {
                                PartsParameterVM.DistributeName = null;
                                PartsParameterVM.BoxNumber = null;
                                e.Effects = System.Windows.DragDropEffects.Move;
                            }
                        }

                    }
                });
            }
        }

        /// <summary>
        /// 丟入盒子內
        /// </summary>
        public ICommand Box_OnDropRecordCommand
        {
            get
            {
                return new RelayParameterizedCommand(obj =>
                {
                    if (obj is DevExpress.Xpf.Core.DropRecordEventArgs e)
                    {
                        var DragDropData =  e.Data.GetData(typeof(RecordDragDropData)) as DevExpress.Xpf.Core.RecordDragDropData;

                        foreach (var _record in DragDropData.Records)
                        {
                            if (_record is PartsParameterViewModel PartsParameterVM)
                            {
                                //看目前選擇哪一個盒子
                                if (SelectedSeparateBoxVM != null)
                                {
                                    PartsParameterVM.DistributeName = this.ProjectDistributeName;
                                    PartsParameterVM.BoxNumber = SelectedSeparateBoxVM.BoxNumber;
                                    e.Effects = System.Windows.DragDropEffects.Move;
                                }
                            }
                        }
           
                    }

                });
            }
        }





    }
}
