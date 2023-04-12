
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
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; }
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get; set; }
        public ICommand SeparateBoxVMObservableCollectionelectionChangedCommand { get; set; }
        public ObservableCollection<PartsParameterViewModel> BoxPartsParameterVMObservableCollection { get; set; } = new();
        public ICommand Box_OnDragRecordOverCommand { get; set; }
        public ICommand Box_OnDropRecordCommand { get; set; }
    }


    public class StampingBoxPartsViewModel: ViewModelBase
    {
        public StampingBoxPartsViewModel(StampingBoxPartModel _stampingBoxPart)
        {
            StampingBoxPart = _stampingBoxPart;
        }
        public StampingBoxPartModel StampingBoxPart = new();

        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; }
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get; set; }
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

        public ObservableCollection<PartsParameterViewModel> BoxPartsParameterVMObservableCollection { get; set; } = new();
        public ICommand Box_OnDragRecordOverCommand { get; set; }
        public ICommand Box_OnDropRecordCommand { get; set; }
    }
}
