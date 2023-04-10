using DevExpress.Data.Extensions;
using DevExpress.Utils.Extensions;
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
            MachiningProjectVMObservableCollection.Add(new MachiningProjectViewModel(new MachiningProjectModel() { ProjectName = "專案一", WorkPieceCurrent = 350, WorkPieceTarget = 500 }));
            MachiningProjectVMObservableCollection.Add(new MachiningProjectViewModel(new MachiningProjectModel() { ProjectName = "專案二", WorkPieceCurrent = 400, WorkPieceTarget = 500 }));
            MachiningProjectVMObservableCollection.Add(new MachiningProjectViewModel(new MachiningProjectModel() { ProjectName = "專案三", WorkPieceCurrent = 370, WorkPieceTarget = 600 }));
        }

        private ObservableCollection<ProductProjectViewModel> _productProjectVMObservableCollection;
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
                _productProjectVMObservableCollection.ForEach(productProject =>
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
                });
                OnPropertyChanged(nameof(ProductProjectVMObservableCollection));
            }
        }

        private ObservableCollection<PartsParameterViewModel> _partsParameterVMObservableCollection;
        /// <summary>
        /// GridControl ABC參數
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


        public ObservableCollection<PartsParameterViewModel> BoxPartsParameterVMObservableCollection { get; set; } = new ObservableCollection<PartsParameterViewModel>();

        public ObservableCollection<MachiningProjectViewModel> MachiningProjectVMObservableCollection { get; set; } = new ObservableCollection<MachiningProjectViewModel>();



        public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; } = new ObservableCollection<ParameterSetting.SeparateBoxViewModel>();



        /// <summary>
        /// 丟入箱子內
        /// </summary>
        public ICommand Box_OnDragRecordOverCommand
        {
            get
            {
                return new RelayParameterizedCommand(obj =>
                {
                    if (obj is DevExpress.Xpf.Core.DragRecordOverEventArgs e)
                    {
                        if (e.TargetRecord is PartsParameterViewModel PartsParameterVM)
                        {
                            //看目前選擇哪一個箱子
                            PartsParameterVM.BoxNumber = 5;
                        }
                        e.Effects = System.Windows.DragDropEffects.Move;
                    }

                });
            }
        }

        /// <summary>
        /// 從箱子拿出來  
        /// </summary>

        public ICommand NoneBox_OnDragRecordOverCommand
        {
            get
            {
                return new RelayParameterizedCommand(obj =>
                {
                    if (obj is DevExpress.Xpf.Core.DragRecordOverEventArgs e)
                    {
                        if (e.TargetRecord is PartsParameterViewModel PartsParameterVM)
                        {
                            PartsParameterVM.BoxNumber = null;
                        }
                        e.Effects = System.Windows.DragDropEffects.Move;
                    }

                });
            }
        }
    }
}
