
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
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class StampingBoxPartModel
    {
        public string ProjectDistributeName { get; set; }
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; }
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get; set; }
        public ICommand Box_OnDropRecordCommand { get; set; }
        public ICommand Box_OnDragRecordOverCommand { get; set; }
    }


    public class StampingBoxPartsViewModel: ViewModelBase
    {
        public StampingBoxPartsViewModel(StampingBoxPartModel _stampingBoxPart)
        {
            StampingBoxPart = _stampingBoxPart;
        }

        public StampingBoxPartModel StampingBoxPart = new();

        /// <summary>
        /// 選擇的盒子
        /// </summary>
        public ParameterSetting.SeparateBoxViewModel SelectedSeparateBoxVM { get; set; }
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
                            BoxPartsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();
                            ProductProjectVMObservableCollection.ForEach(productProject =>
                            {
                                productProject.PartsParameterVMObservableCollection.ForEach((productProjectPartViewModel) =>
                                {
                                    if (productProjectPartViewModel.BoxNumber.HasValue)
                                        if (NewSeparateBoxVM.BoxNumber == productProjectPartViewModel.BoxNumber.Value && productProjectPartViewModel.DistributeName == StampingBoxPart.ProjectDistributeName)
                                            if (!BoxPartsParameterVMObservableCollection.Contains(productProjectPartViewModel))
                                                BoxPartsParameterVMObservableCollection.Add(productProjectPartViewModel);
                                });
                            });
                        }
                    }

                }
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
