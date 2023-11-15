
using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Xpf;
using DevExpress.Utils.Extensions;
using GD_StampingMachine.ViewModels.ProductSetting;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class StampingBoxPartModel
    {
        public string ProjectDistributeName { get; set; }
        /// <summary>
        /// 盒子列表
        /// </summary>
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; }

        [JsonIgnore]
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get; set; }

        public bool GridControl_MachiningStatusColumnVisible { get; set; } = true;
    }


    public class StampingBoxPartsViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingBoxPartsViewModel");

        public StampingBoxPartsViewModel(StampingBoxPartModel _stampingBoxPart)
        {
            StampingBoxPart = _stampingBoxPart;
            ReLoadBoxPartsParameterVMObservableCollection();
        }


        public void ReLoadBoxPartsParameterVMObservableCollection()
        {
            BoxPartsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();
            if (ProductProjectVMObservableCollection != null)
            {
                ProductProjectVMObservableCollection.ForEach(productProject =>
                {
                    productProject.PartsParameterVMObservableCollection.ForEach((productProjectPartViewModel) =>
                    {
                        if (productProjectPartViewModel.BoxIndex.HasValue)
                            if (productProjectPartViewModel.DistributeName == StampingBoxPart.ProjectDistributeName)
                                if (!BoxPartsParameterVMObservableCollection.Contains(productProjectPartViewModel))
                                    BoxPartsParameterVMObservableCollection.Add(productProjectPartViewModel);
                    });
                });
            }
        }




        public string ProjectDistributeName { get => StampingBoxPart.ProjectDistributeName; set => StampingBoxPart.ProjectDistributeName = value; }

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
                    if (SeparateBoxVMObservableCollection != null)
                        _selectedSeparateBoxVM = SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxIsEnabled);
                return _selectedSeparateBoxVM;
            }
            set
            {
                _selectedSeparateBoxVM = value;
                OnPropertyChanged();
            }

        }





        /// <summary>
        /// 盒子列表
        /// </summary>
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection
        {
            get => StampingBoxPart.SeparateBoxVMObservableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get => StampingBoxPart.ProductProjectVMObservableCollection;
        }


        private ICommand _separateBoxVMObservableCollectionelectionChangedCommand;
        [JsonIgnore]
        public ICommand SeparateBoxVMObservableCollectionelectionChangedCommand
        {
            get => _separateBoxVMObservableCollectionelectionChangedCommand??=new RelayCommand<object>(obj =>
            {
                OnPropertyChanged(nameof(BoxPartsParameterVMRowFilterCommand));

               /* if (obj is System.Windows.Controls.SelectionChangedEventArgs e)
                {
                    if (e.AddedItems.Count > 0)
                    {
                        if (e.AddedItems[0] is GD_StampingMachine.ViewModels.ParameterSetting.SeparateBoxViewModel NewSeparateBoxVM)
                        {
                            ReLoadBoxPartsParameterVMObservableCollection();
                            //BoxPartsParameterVMObservableCollectionRefresh(NewSeparateBoxVM.BoxIndex);
                        }
                    }
                }*/
            });
        }





        //篩選器
        [JsonIgnore]
        public ICommand BoxPartsParameterVMRowFilterCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<RowFilterArgs>(async args =>
            {
                    if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.PartsParameterViewModel PParameter)
                    {
                        if (SelectedSeparateBoxVM != null)
                        {
                            if (PParameter.DistributeName == this.ProjectDistributeName && PParameter.BoxIndex == SelectedSeparateBoxVM.BoxIndex)
                                args.Visible = true;
                            else
                                args.Visible = false;
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
                _boxPartsParameterVMObservableCollection ??= new ObservableCollection<PartsParameterViewModel>();
                return _boxPartsParameterVMObservableCollection;
            }
            set
            {
                _boxPartsParameterVMObservableCollection = value;
                OnPropertyChanged();
            }
        }





        [JsonIgnore]
        public ICommand Box_OnDragRecordOverCommand
        {
            get => GD_Command.Box_OnDragRecordOverCommand;
        }

        [JsonIgnore]
        public ICommand Box_OnDropRecordCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                if (obj is DevExpress.Xpf.Core.DropRecordEventArgs e)
                {
                    if (e.Data.GetData(typeof(DevExpress.Xpf.Core.RecordDragDropData)) is DevExpress.Xpf.Core.RecordDragDropData DragDropData)
                    {
                        foreach (var _record in DragDropData.Records)
                        {
                            if (_record is PartsParameterViewModel PartsParameterVM)
                            {
                                //看目前選擇哪一個盒子
                                if (SelectedSeparateBoxVM != null)
                                {
                                    PartsParameterVM.DistributeName = ProjectDistributeName;// ProjectDistribute.ProjectDistributeName;
                                    PartsParameterVM.BoxIndex = SelectedSeparateBoxVM.BoxIndex;
                                    e.Effects = System.Windows.DragDropEffects.Copy; 
                                }
                            }
                        }
                    }
                }
            });
        }





    }
}
