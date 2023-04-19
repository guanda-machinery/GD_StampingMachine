
using DevExpress.Utils.Extensions;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI.Internal;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class StampingBoxPartModel
    {
        public string ProjectDistributeName { get; set; }
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; }
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get; set; }
        public ObservableCollection<ProjectDistributeViewModel> ProjectDistributeVMObservableCollection { get; set; }
        public ICommand Box_OnDropRecordCommand { get; set; }
        public ICommand Box_OnDragRecordOverCommand { get; set; }
        public bool GridControl_MachiningStatusColumnVisible { get; set; } = true;
    }


    public class StampingBoxPartsViewModel: BaseViewModelWithLog
    {
        public StampingBoxPartsViewModel(StampingBoxPartModel _stampingBoxPart)
        {
            StampingBoxPart = _stampingBoxPart;
        }
        
        public string ProjectDistributeName { get => StampingBoxPart.ProjectDistributeName; set =>StampingBoxPart.ProjectDistributeName=value; }

        public bool MachiningStatusIsVisible
        { 
            get => StampingBoxPart.GridControl_MachiningStatusColumnVisible; 
            set => StampingBoxPart.GridControl_MachiningStatusColumnVisible = value; 
        }

        public StampingBoxPartModel StampingBoxPart = new();



        private ParameterSetting.SeparateBoxViewModel _selectedSeparateBoxVM;
        /// <summary>
        /// 選擇的盒子
        /// </summary>
        public ParameterSetting.SeparateBoxViewModel SelectedSeparateBoxVM 
        {
            get 
            {
                if (_selectedSeparateBoxVM == null)
                    if(SeparateBoxVMObservableCollection!=null)
                        _selectedSeparateBoxVM = SeparateBoxVMObservableCollection.FirstOrDefault();
                return _selectedSeparateBoxVM;
            }
            set => _selectedSeparateBoxVM = value;
        }
       




        /// <summary>
        /// 盒子
        /// </summary>
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get => StampingBoxPart.SeparateBoxVMObservableCollection; }
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get => StampingBoxPart.ProductProjectVMObservableCollection; }
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
                            BoxPartsParameterVMObservableCollectionRefresh(NewSeparateBoxVM.BoxNumber);
                        }
                    }
                }
            });
        }

        public ICommand BoxPartsParameterVMObservableCollectionRefreshCommand
        {
            get => new RelayCommand(() =>
            {
                BoxPartsParameterVMObservableCollectionRefresh();
            });
        }

        public void BoxPartsParameterVMObservableCollectionRefresh()
        {
            if (SelectedSeparateBoxVM != null)
            {
                BoxPartsParameterVMObservableCollectionRefresh(SelectedSeparateBoxVM.BoxNumber);
            }
        }

        private void BoxPartsParameterVMObservableCollectionRefresh(int _boxNumber)
        {
            BoxPartsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();
            ProductProjectVMObservableCollection.ForEach(productProject =>
            {
                productProject.PartsParameterVMObservableCollection.ForEach((productProjectPartViewModel) =>
                {
                    if (productProjectPartViewModel.BoxNumber.HasValue)
                        if (_boxNumber == productProjectPartViewModel.BoxNumber.Value && productProjectPartViewModel.DistributeName == StampingBoxPart.ProjectDistributeName)
                            if (!BoxPartsParameterVMObservableCollection.Contains(productProjectPartViewModel))
                                BoxPartsParameterVMObservableCollection.Add(productProjectPartViewModel);
                });
            });
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


        public ICommand Box_OnDropRecordCommand { get => StampingBoxPart.Box_OnDropRecordCommand; }
        public ICommand Box_OnDragRecordOverCommand { get => StampingBoxPart.Box_OnDragRecordOverCommand; }


    }
}
