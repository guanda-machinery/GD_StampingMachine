using DevExpress.DataAccess.Json;
using DevExpress.Office.Forms;
using DevExpress.Utils.StructuredStorage.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using GD_StampingMachine.Model.ProductionSetting;
using GD_StampingMachine.Properties;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    /// <summary>
    /// ABC參數加工型
    /// </summary>
    public class PartsParameterViewModel : BaseViewModelWithLog
    {
        public PartsParameterViewModel(PartsParameterModel PParameter)
        {
            PartsParameter = PParameter;
        }
        public PartsParameterModel PartsParameter = new();
        public double FinishProgress
        {
            get => PartsParameter.FinishProgress;
            set
            {
                PartsParameter.FinishProgress = value;
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
        public string ProjectName
        {
            get => PartsParameter.ProjectName;
            set
            {
                PartsParameter.ProjectName = value; OnPropertyChanged();
            }
        }


        public string ParameterA
        {
            get => PartsParameter.Parametert_A;
            set
            {
                PartsParameter.Parametert_A = value;
                OnPropertyChanged(nameof(ParameterA));
            }
        }
        public string ParameterB
        {
            get => PartsParameter.Parametert_B;
            set
            {
                PartsParameter.Parametert_B = value;
                OnPropertyChanged(nameof(ParameterB));
            }
        }
        public string ParameterC
        {
            get => PartsParameter.Parametert_C;
            set
            {
                PartsParameter.Parametert_C = value;
                OnPropertyChanged(nameof(ParameterC));
            }
        }

        public MachiningStatusEnum MachiningStatus
        {
            get => PartsParameter.MachiningStatus;
            set
            {
                PartsParameter.MachiningStatus =value; 
                OnPropertyChanged(nameof(MachiningStatus));
            }
        }
   

        /// <summary>
        /// (加工)盒子編號
        /// </summary>
        public int? BoxNumber
        {
            get => PartsParameter.BoxNumber;
            set
            {
                PartsParameter.BoxNumber = value;
                OnPropertyChanged(nameof(BoxNumber));
            }
        }


        /// <summary>
        /// 金屬牌樣式
        /// </summary>
        public SettingViewModelBase SettingVMBase
        {
            get
            {
                return PartsParameter.SettingVMBase;
             }
            set
            {
                PartsParameter.SettingVMBase = value;
                OnPropertyChanged(nameof(SettingVMBase));
            }
        }

        private RelayParameterizedCommand _projectEditCommand;
        public RelayParameterizedCommand ProjectEditCommand
        {
            get
            {
                if (_projectEditCommand == null)
                {
                    _projectEditCommand = new RelayParameterizedCommand(obj =>
                    {
                        if (obj is GridControl ObjGridControl)
                        {
                            if (ObjGridControl.ItemsSource is ObservableCollection<PartsParameterViewModel> GridItemSource)
                            {
                                if (MethodWinUIMessageBox.AskDelProject(this.SettingVMBase.NumberSetting.NumberSettingMode))
                                    GridItemSource.Remove(this);
                            }
                        }
                    });
                }
                return _projectEditCommand;
            }
            set
            {
                _projectEditCommand = value;
                OnPropertyChanged(nameof(ProjectEditCommand));
            }
        }

        //private RelayParameterizedCommand _projectDeleteCommand;
        public RelayParameterizedCommand ProjectDeleteCommand
        {
            get
            {
                return new RelayParameterizedCommand(obj =>
                       {
                           if (obj is GridControl ObjGridControl)
                           {
                               if (ObjGridControl.ItemsSource is ObservableCollection<PartsParameterViewModel> GridItemSource)
                               {
                                   if (MethodWinUIMessageBox.AskDelProject(this.SettingVMBase.NumberSetting.NumberSettingMode))
                                       GridItemSource.Remove(this);
                               }
                           }
                       });
            }
        }









    }

  





}
