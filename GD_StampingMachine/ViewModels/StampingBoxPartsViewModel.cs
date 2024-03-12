﻿
using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Xpf;
using DevExpress.Utils.Extensions;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels.ParameterSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class StampingBoxPartsViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingBoxPartsViewModel");
        public StampingBoxPartsViewModel(SeparateBoxExtViewModelObservableCollection separateBoxExtViewModelCollection)
        {
            ProjectDistributeName = string.Empty;
            SeparateBoxVMObservableCollection = separateBoxExtViewModelCollection;

        }

        public StampingBoxPartsViewModel(string? projectDistributeName, SeparateBoxExtViewModelObservableCollection separateBoxExtViewModelCollection)
        {
            ProjectDistributeName = projectDistributeName;
            SeparateBoxVMObservableCollection = separateBoxExtViewModelCollection;

        }








        private string? _projectDistributeName;
        public string? ProjectDistributeName { get => _projectDistributeName; set { _projectDistributeName = value; OnPropertyChanged(); } }


        //public StampingBoxPartModel StampingBoxPart = new();



        private SeparateBoxExtViewModel? _selectedSeparateBoxVM;
        /// <summary>
        /// 選擇的盒子
        /// </summary>
        public SeparateBoxExtViewModel? SelectedSeparateBoxVM
        {
            get
            {
                if (_selectedSeparateBoxVM == null)
                    if (SeparateBoxVMObservableCollection != null)
                    { 
                        _selectedSeparateBoxVM = SeparateBoxVMObservableCollection.FirstOrDefault(x => x.BoxIsEnabled);
                    }
                return _selectedSeparateBoxVM;
            }
            set
            {
                _selectedSeparateBoxVM = value;
                OnPropertyChanged();
            }
        }



        private SeparateBoxExtViewModelObservableCollection _separateBoxVMObservableCollection;
        /// <summary>
        /// 盒子列表
        /// </summary>
        public SeparateBoxExtViewModelObservableCollection SeparateBoxVMObservableCollection
        {
            get => _separateBoxVMObservableCollection;
            set { _separateBoxVMObservableCollection = value; OnPropertyChanged(); }
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
