using DevExpress.Data.Extensions;
using DevExpress.Utils.Extensions;
using DevExpress.Xpf.Core;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
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
    public class TypeSettingSettingViewModel : ViewModelBase
    {
        public TypeSettingSettingViewModel()
        {
            //MachiningProjectVMObservableCollection.Add(new MachiningProjectViewModel(new MachiningProjectModel() { ProjectName = "專案一", WorkPieceCurrent = 350, WorkPieceTarget = 500 }));
          //  MachiningProjectVMObservableCollection.Add(new MachiningProjectViewModel(new MachiningProjectModel() { ProjectName = "專案二", WorkPieceCurrent = 400, WorkPieceTarget = 500 }));
           // MachiningProjectVMObservableCollection.Add(new MachiningProjectViewModel(new MachiningProjectModel() { ProjectName = "專案三", WorkPieceCurrent = 370, WorkPieceTarget = 600 }));
        }

        private ObservableCollection<ProductProjectViewModel> _productProjectVMObservableCollection;


        private ProductProjectViewModel _selectedProductProjectVM;
        public ProductProjectViewModel SelectedProductProjectVM
        {
            get
            {
                if(_selectedProductProjectVM != null)
                {
                    PartsParameterVMObservableCollection = _selectedProductProjectVM.PartsParameterVMObservableCollection;
                    
                }
                return _selectedProductProjectVM;
            }
            set
            {
                _selectedProductProjectVM = value;
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
                if (_productProjectVMObservableCollection == null)
                    _productProjectVMObservableCollection = new ObservableCollection<ProductProjectViewModel>();
                return _productProjectVMObservableCollection;
            }
            set
            {
                _productProjectVMObservableCollection = value;
                /*_productProjectVMObservableCollection.ForEach(productProject =>
                {
                    productProject.PartsParameterVMObservableCollection.ForEach((productProjectPartViewModel) =>
                    {
                        if (productProjectPartViewModel.BoxNumber.HasValue)
                        {
                            if (!BoxPartsParameterVMObservableCollection.Contains(productProjectPartViewModel))
                                BoxPartsParameterVMObservableCollection.Add(productProjectPartViewModel);
                        }
                        else
                        {
                            if (!PartsParameterVMObservableCollection.Contains(productProjectPartViewModel))
                                PartsParameterVMObservableCollection.Add(productProjectPartViewModel);
                        }
                        
                    });
                });*/
                OnPropertyChanged(nameof(ProductProjectVMObservableCollection));
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
        /// 箱子內
        /// </summary>
        private ObservableCollection<PartsParameterViewModel> _boxPartsParameterVMObservableCollection;
        public ObservableCollection<PartsParameterViewModel> BoxPartsParameterVMObservableCollection 
        { 
            get 
            {
                if(_boxPartsParameterVMObservableCollection==null)
                    _boxPartsParameterVMObservableCollection=new ObservableCollection<PartsParameterViewModel>();
                return _boxPartsParameterVMObservableCollection;
            }
            set
            {
                _boxPartsParameterVMObservableCollection=value;
                OnPropertyChanged();
            }
        } 

       // public ObservableCollection<MachiningProjectViewModel> MachiningProjectVMObservableCollection { get; set; } = new ObservableCollection<MachiningProjectViewModel>();



        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; } = new ObservableCollection<ParameterSetting.SeparateBoxViewModel>();



        private ParameterSetting.SeparateBoxViewModel _selectedSeparateBoxVM;
        /// <summary>
        /// 選擇盒子
        /// </summary>
        public ParameterSetting.SeparateBoxViewModel SelectedSeparateBoxVM
        {
            get
            {
                if (_selectedSeparateBoxVM != null)
                {
                    BoxPartsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();
                    ProductProjectVMObservableCollection.ForEach(productProject =>
                    {
                        productProject.PartsParameterVMObservableCollection.ForEach((productProjectPartViewModel) =>
                        {
                            if (productProjectPartViewModel.BoxNumber.HasValue)
                                if (_selectedSeparateBoxVM.BoxNumber == productProjectPartViewModel.BoxNumber.Value)
                                    if (!BoxPartsParameterVMObservableCollection.Contains(productProjectPartViewModel))
                                        BoxPartsParameterVMObservableCollection.Add(productProjectPartViewModel);

                        });
                    });
                }
                return _selectedSeparateBoxVM;
            }
            set
            {
                _selectedSeparateBoxVM = value;
                OnPropertyChanged();
            }
        }




        public ICommand NoneBox_OnDragRecordOverCommand
        {
            get
            {
                return new RelayParameterizedCommand(obj =>
                {
                    if (obj is DevExpress.Xpf.Core.DragRecordOverEventArgs e)
                    {
                        e.Effects = System.Windows.DragDropEffects.None;
                        var DragDropData = e.Data.GetData(typeof(RecordDragDropData)) as DevExpress.Xpf.Core.RecordDragDropData;
                        foreach (var _record in DragDropData.Records)
                        {
                            if (_record is PartsParameterViewModel PartsParameterVM)
                            {
                                if(SelectedProductProjectVM != null)
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
                        e.Effects = System.Windows.DragDropEffects.Copy;
                    }
                });
            }
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
                            e.Effects = System.Windows.DragDropEffects.Copy;
                    }
                });
            }
        }


        /// <summary>
        /// 從箱子拿出來  
        /// </summary>
        public ICommand NoneBox_OnDropRecordOverCommand
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
                                PartsParameterVM.BoxNumber = null;
                                e.Effects = System.Windows.DragDropEffects.Copy;
                            }
                        }

                    }
                });
            }
        }
   
        /// <summary>
        /// 丟入箱子內
        /// </summary>
        public ICommand Box_OnDropRecordOverCommand
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
                                //看目前選擇哪一個箱子
                                if (SelectedSeparateBoxVM != null)
                                {
                                    PartsParameterVM.BoxNumber = SelectedSeparateBoxVM.BoxNumber;
                                    e.Effects = System.Windows.DragDropEffects.Copy;
                                }
                            }
                        }
           
                    }

                });
            }
        }





    }
}
