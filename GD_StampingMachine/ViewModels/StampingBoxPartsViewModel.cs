
using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Xpf;
using DevExpress.Utils.Extensions;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels.ProductSetting;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class StampingBoxPartModel
    {
        public string? ProjectDistributeName { get; set; }
        /// <summary>
        /// 盒子列表
        /// </summary>
        public ObservableCollection<ParameterSetting.SeparateBoxViewModel>? SeparateBoxVMObservableCollection { get; set; }

        [JsonIgnore]
        public ObservableCollection<ProductProjectViewModel>? ProductProjectVMObservableCollection { get; set; }

       // [JsonIgnore]
        //public bool GridControl_MachiningStatusColumnVisible { get; set; } = false;
    }


    public class StampingBoxPartsViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingBoxPartsViewModel");

        public StampingBoxPartsViewModel(StampingBoxPartModel stampingBoxPart)
        {
            StampingBoxPart = stampingBoxPart;
            _ = ReLoadBoxPartsParameterVMObservableCollectionAsync();
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        for (int i = 0; i < SeparateBoxVMObservableCollection.Count; i++)
                        {
                            var separateBox = SeparateBoxVMObservableCollection[i];
                            var indexCollection = BoxPartsParameterVMObservableCollection.Where(x => x.BoxIndex == separateBox.BoxIndex).ToList();

                            //已排定
                            separateBox.BoxPieceValue = indexCollection.Count;

                            //加工完成但尚未被移除的
                            separateBox.UnTransportedFinishedBoxPieceValue = indexCollection.FindAll(x => x.IsFinish && !x.IsTransported).Count;

                            //已經被分配加工且尚未被移除
                            separateBox.UnTransportedBoxPieceValue = indexCollection.FindAll(x => x.WorkIndex >=0 && !x.IsTransported).Count;

                            //只有已完成
                            separateBox.FinishedBoxPieceValue = indexCollection.FindAll(x => x.IsFinish).Count;
                            separateBox.UnFinishedBoxPieceValue = indexCollection.FindAll(x => !x.IsFinish).Count;

                        }
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                        //   Debugger.Break();
                    }
                    await Task.Delay(1000);
                }
            });

        }


        public async Task ReLoadBoxPartsParameterVMObservableCollectionAsync()
        {
            var newCollection = new ObservableCollection<PartsParameterViewModel>();
            if (ProductProjectVMObservableCollection != null)
            {
                ProductProjectVMObservableCollection.ForEach(productProject =>
                {
                    productProject.PartsParameterVMObservableCollection.ForEach((productProjectPartViewModel) =>
                    {
                        if (productProjectPartViewModel.BoxIndex.HasValue)
                            if (productProjectPartViewModel.DistributeName == StampingBoxPart.ProjectDistributeName)
                                if (!newCollection.Contains(productProjectPartViewModel))
                                    newCollection.Add(productProjectPartViewModel);
                    });
                });
            }
            BoxPartsParameterVMObservableCollection = new();
            foreach (var pParameter in newCollection)
            {
                await Application.Current?.Dispatcher.InvokeAsync(() =>
                {
                    BoxPartsParameterVMObservableCollection.Add(pParameter);
                });
            }
        }




        public string ProjectDistributeName { get => StampingBoxPart.ProjectDistributeName; set => StampingBoxPart.ProjectDistributeName = value; }


        public StampingBoxPartModel StampingBoxPart = new();



        private ParameterSetting.SeparateBoxViewModel? _selectedSeparateBoxVM;
        /// <summary>
        /// 選擇的盒子
        /// </summary>
        public ParameterSetting.SeparateBoxViewModel? SelectedSeparateBoxVM
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
            set { StampingBoxPart.SeparateBoxVMObservableCollection = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get => StampingBoxPart.ProductProjectVMObservableCollection;
        }














        /// <summary>
        /// 盒子內的專案
        /// </summary>
        private ObservableCollection<PartsParameterViewModel>? _boxPartsParameterVMObservableCollection;

        public ObservableCollection<PartsParameterViewModel> BoxPartsParameterVMObservableCollection
        {
            get=>_boxPartsParameterVMObservableCollection ??= new ObservableCollection<PartsParameterViewModel>();
            set
            {
                _boxPartsParameterVMObservableCollection = value;
                OnPropertyChanged();
            }
        }




        private ICommand?_gridControlSizeChangedCommand;
        [JsonIgnore]
        public ICommand GridControlSizeChangedCommand
        {
            get => _gridControlSizeChangedCommand ??= new RelayCommand<object>(obj =>
            {

                if (obj is System.Windows.SizeChangedEventArgs e)
                {
                    if (e.Source is DevExpress.Xpf.Grid.GridControl gridcontrol)
                    {
                        if (gridcontrol.View is DevExpress.Xpf.Grid.TableView tableview)
                        {
                            var pageSize = ((tableview.ActualHeight - tableview.HeaderPanelMinHeight - 30) / 45);
                            tableview.PageSize = (pageSize < 3 ? 3 : (int)pageSize);
                        }
                    }
                }
            });
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
                try
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
                                        PartsParameterVM.WorkIndex = -1;
                                        e.Effects = System.Windows.DragDropEffects.Move;
                                        //RefreshBoxPartsParameterVMRowFilter();
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {

                }

            });
        }





    }
}
