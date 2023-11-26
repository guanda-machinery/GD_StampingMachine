﻿using CommunityToolkit.Mvvm.Input;
using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Office.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.StructuredStorage.Internal;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.XtraScheduler.Native;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class MachineFunctionViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("btnDescription_MachineFunction");



        public ParameterSettingViewModel ParameterSettingVM { get => Singletons.StampingMachineSingleton.Instance.ParameterSettingVM; }
        public StampingFontChangedViewModel StampingFontChangedVM { get => Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM; }
     
        /// <summary>
        /// 已選擇的加工專案
        /// </summary>
        public ProjectDistributeViewModel ProjectDistributeVMSelected
        {
            get => StampingMachineSingleton.Instance.SelectedProjectDistributeVM;
            set
            {
                StampingMachineSingleton.Instance.SelectedProjectDistributeVM = value;
                OnPropertyChanged();
            }
        }






        public MachineFunctionViewModel()
        {


        }

        //private int SeparateBoxIndexNow =-1;







        /// <summary>
        /// 從機台端蒐集到的資料
        /// </summary>
        public StampMachineDataSingleton StampMachineData { get; set; } = Singletons.StampMachineDataSingleton.Instance;


        private ICommand _gridControlSizeChangedCommand;
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
                            var pageSize = (tableview.ActualHeight - tableview.HeaderPanelMinHeight) / (40 + tableview.FixedLineHeight);
                            tableview.PageSize = (pageSize < 3 ? 3 : (int)pageSize);
                        }
                    }
                }
            });
        }







        private bool _feeding_Component_Button_IsChecked = false;
        /// <summary>
        /// 選擇進料組
        /// </summary>
        public bool Feeding_Component_Button_IsChecked
        {
            get => _feeding_Component_Button_IsChecked;
            set { _feeding_Component_Button_IsChecked = value; OnPropertyChanged(); }

        }


        private bool _qrCode_Component_Button_IsChecked = false;
        public bool QRCode_Component_Button_IsChecked
        {
            get => _qrCode_Component_Button_IsChecked;
            set { _qrCode_Component_Button_IsChecked = value; OnPropertyChanged(); }
        }



        private bool _stamping_Component_Button_IsChecked = false;
        /// <summary>
        /// 選擇進料組
        /// </summary>
        public bool Stamping_Component_Button_IsChecked
        {
            get => _stamping_Component_Button_IsChecked;
            set { _stamping_Component_Button_IsChecked = value; OnPropertyChanged(); }

        }
        private bool _shearCut_Component_Button_IsChecked = false;
        /// <summary>
        /// 選擇進料組
        /// </summary>
        public bool ShearCut_Component_Button_IsChecked
        {
            get => _shearCut_Component_Button_IsChecked;
            set { _shearCut_Component_Button_IsChecked = value; OnPropertyChanged(); }

        }
        private bool _Separator_Component_Button_IsChecked = false;
        /// <summary>
        /// 選擇進料組
        /// </summary>
        public bool Separator_Component_Button_IsChecked
        {
            get => _Separator_Component_Button_IsChecked;
            set { _Separator_Component_Button_IsChecked = value; OnPropertyChanged(); }

        }

        private bool _isManualMode = false;
        public bool IsManualMode
        {
            get => _isManualMode; set { _isManualMode = value; OnPropertyChanged(); }
        }





        private AsyncRelayCommand _separateBox_ClockwiseRotateCommand;
        public AsyncRelayCommand SeparateBox_ClockwiseRotateCommand
        {
            get => _separateBox_ClockwiseRotateCommand??=new(async () =>
            {

                try
                {
                    var Index = StampMachineData.SeparateBoxIndex;
                    // var Index = SeparateBoxIndexNow;
                    var LocationIndex = Index + 1;
                    if (LocationIndex >= ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.Count)
                        LocationIndex = 0;
                    if (LocationIndex < 0)
                        LocationIndex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.Count - 1;


                    if (await StampMachineData.SetSeparateBoxNumber(LocationIndex))
                    {

                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }

            },()=> !_separateBox_ClockwiseRotateCommand.IsRunning);
        }

        private AsyncRelayCommand _separateBox_CounterClockwiseRotateCommand;
        public AsyncRelayCommand SeparateBox_CounterClockwiseRotateCommand
        {
            get => _separateBox_CounterClockwiseRotateCommand??=new AsyncRelayCommand(async () =>
            {
                try
                {
                    var Index = StampMachineData.SeparateBoxIndex;
                    // var Index = SeparateBoxIndexNow;
                    var LocationIndex = Index - 1;
                    if (LocationIndex >= ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.Count)
                        LocationIndex = 0;
                    if (LocationIndex < 0)
                        LocationIndex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.Count - 1;
                    //SeparateBox_Rotate(LocationIndex, 1);

                    if (await StampMachineData.SetSeparateBoxNumber(LocationIndex))
                    {

                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            },()=> _separateBox_CounterClockwiseRotateCommand.IsRunning);
        }






        /// <summary>
        /// 可計算轉盤上 下一個要轉的目標位置
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        
        private int NextSeparateBox_Rotate(int step)
        {
            var MinIndex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.ToList().FindIndex(x => x.BoxIsEnabled);
            var Maxindex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.ToList().FindLastIndex(x => x.BoxIsEnabled);
            var IsUsingindex = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.FindIndex(x => x.IsUsing);
            if (IsUsingindex == -1)
            {
                if (step > 0)
                {
                    IsUsingindex = MinIndex;
                }
                else if (step <= 0)
                {
                    IsUsingindex = Maxindex;
                }
                ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection[IsUsingindex].IsUsing = true;
                return IsUsingindex;
            }
            else
                ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection[IsUsingindex].IsUsing = false;

            IsUsingindex += step;


            if (IsUsingindex < MinIndex)
            {
                IsUsingindex = Maxindex;
            }

            if (IsUsingindex > Maxindex)
            {
                //IsUsingindex = 0;
                IsUsingindex = MinIndex;
                //最小的可用index
            }
            return IsUsingindex;
        }





    }
}
