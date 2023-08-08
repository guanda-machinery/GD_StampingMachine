﻿
using DevExpress.Office.Forms;
using DevExpress.Utils.StructuredStorage.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Interfaces;
using GD_StampingMachine.Method;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.GD_Model.ProductionSetting;
using GD_StampingMachine.Properties;
using GD_StampingMachine.ViewModels.ParameterSetting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using GD_StampingMachine.Model;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    /// <summary>
    /// ABC參數加工型
    /// </summary>
    public class PartsParameterViewModel : BaseViewModelWithLog
    {
        [JsonIgnore]
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_PartsParameterViewModel");
        public PartsParameterViewModel(PartsParameterModel PParameter)
       {
            PartsParameter = PParameter;
            PartsParameter ??= new PartsParameterModel();
        }

        public readonly PartsParameterModel PartsParameter;
        public float FinishProgress
        {
            get => PartsParameter.Processing;
            set
            {
                PartsParameter.Processing = value;
                OnPropertyChanged(nameof(FinishProgress));
            }
        }

        /// <summary>
        /// 加工專案名
        /// </summary>
        public string DistributeName
        {
            get => PartsParameter.DistributeName;
            set
            {
                PartsParameter.DistributeName = value; OnPropertyChanged();
            }
        }
        /// <summary>
        /// 專案名
        /// </summary>
        public string ProjectID
        {
            get => PartsParameter.ProjectID;
            set
            {
                PartsParameter.ProjectID = value; OnPropertyChanged();
            }
        }


        public string ParameterA
        {
            get => PartsParameter.ParamA;
            set
            {
                PartsParameter.ParamA = value;
                OnPropertyChanged(nameof(ParameterA));
            }
        }
        public string ParameterB
        {
            get => PartsParameter.ParamB;
            set
            {
                PartsParameter.ParamB = value;
                OnPropertyChanged(nameof(ParameterB));
            }
        }
        public string ParameterC
        {
            get => PartsParameter.ParamC;
            set
            {
                PartsParameter.ParamC = value;
                OnPropertyChanged(nameof(ParameterC));
            }
        }

        public MachiningStatusEnum MachiningStatus
        {
            get => PartsParameter.MachiningStatus;
            set
            {
                PartsParameter.MachiningStatus = value;
                OnPropertyChanged(nameof(MachiningStatus));
            }
        }


        /// <summary>
        /// (加工)盒子編號
        /// </summary>
        public int? BoxIndex
        {
            get => PartsParameter.BoxIndex;
            set
            {
                PartsParameter.BoxIndex = value;
                OnPropertyChanged();
            }
        }


      //  private SheetStampingTypeFormEnum _sheetStampingTypeForm;
        /// <summary>
        /// 型態
        /// </summary>
        [JsonIgnore]
        public SheetStampingTypeFormEnum SheetStampingTypeForm
        {
            get 
            { 
                if(_settingVMBase is QRSettingViewModel)
                {
                    return SheetStampingTypeFormEnum.QRSheetStamping;
                }
                if (_settingVMBase is NumberSettingViewModel)
                {
                    return SheetStampingTypeFormEnum.NormalSheetStamping;
                }
                return SheetStampingTypeFormEnum.NormalSheetStamping;
            }
        }



        /// <summary>
        /// 金屬牌樣式
        /// </summary>
        private SettingBaseViewModel _settingVMBase;
        public SettingBaseViewModel SettingVMBase
        {
            get
            {
                return _settingVMBase;
            }
            set
            {
                _settingVMBase = value;
                OnPropertyChanged();
            }
        }

        /*public StampPlateSettingModel SettingVMBase
        {
            get
            {
                if (SheetStampingTypeForm == SheetStampingTypeFormEnum.QRSheetStamping)
                    _settingVMBase ??= new QRSettingViewModel(PartsParameter.StampingPlate);
                else
                    _settingVMBase ??= new NumberSettingViewModel(PartsParameter.StampingPlate);
                return _settingVMBase;
            }
            set
            {
                _settingVMBase = value;
                if (_settingVMBase is QRSettingViewModel QrSettingVM)
                {
                    PartsParameter.StampingPlate = QrSettingVM.QRSetting;
                }
                if (_settingVMBase is NumberSettingViewModel NumberSettingVM)
                {
                    PartsParameter.StampingPlate = NumberSettingVM.NumberSetting;
                }
                OnPropertyChanged(nameof(SettingVMBase));
            }
        }*/

        [JsonIgnore]
        public RelayParameterizedCommand ProjectEditCommand
        {
            get => new(obj =>
            {
                if (obj is GridControl ObjGridControl)
                {
                    if (ObjGridControl.ItemsSource is ObservableCollection<PartsParameterViewModel> GridItemSource)
                    {
                        
                      if (MethodWinUIMessageBox.AskDelProject(this.SettingVMBase.NumberSettingMode))
                            GridItemSource.Remove(this);
                    }
                }
            });
        }

        [JsonIgnore]
        public RelayParameterizedCommand ProjectDeleteCommand
        {
            get => new (obj =>
            {
                if (obj is GridControl ObjGridControl)
                {
                    if (ObjGridControl.ItemsSource is ObservableCollection<PartsParameterViewModel> GridItemSource)
                    {
                        if (SettingVMBase != null)
                        {
                            if (MethodWinUIMessageBox.AskDelProject(this.SettingVMBase.NumberSettingMode))
                            {
                                GridItemSource.Remove(this);
                            }
                        }
                        else
                            GridItemSource.Remove(this);

                    }
                }
            }
            );
        }









    }

  





}
