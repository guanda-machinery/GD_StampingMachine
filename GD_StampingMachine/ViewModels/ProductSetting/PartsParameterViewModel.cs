
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
using GD_CommonLibrary.Extensions;
using Microsoft.Xaml.Behaviors;
using DevExpress.CodeParser;
using Newtonsoft.Json.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.Input;
using GD_StampingMachine.Singletons;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    /// <summary>
    /// ABC參數加工型
    /// </summary>
    public partial class PartsParameterViewModel : GD_CommonLibrary.BaseViewModel
    {
        [JsonIgnore]
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_PartsParameterViewModel");

        public PartsParameterViewModel()
        { 

        }
       public PartsParameterViewModel(PartsParameterModel PParameter)
       {
            PartsParameter = PParameter;
        }



        private PartsParameterModel _partsParameter;
        public PartsParameterModel PartsParameter 
        {
            get => _partsParameter ??= new PartsParameterModel();
            private set => _partsParameter = value;
        }

        /// <summary>
        /// 加工進程
        /// </summary>
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
        /// <summary>
        /// 加工識別id
        /// </summary>
        public int ID
        {
            get => PartsParameter.ID;
            set
            {
                PartsParameter.ID = value; OnPropertyChanged();
            }
        }

        /// <summary>
        /// QR陣列已完成
        /// </summary>
        public bool DataMatrixIsFinish
        {
            get => PartsParameter.DataMatrixIsFinish;
            set
            {
                PartsParameter.DataMatrixIsFinish = value; OnPropertyChanged();
            }
        }
        /// <summary>
        /// 鋼印已完成
        /// </summary>
        public bool EngravingIsFinish
        {
            get => PartsParameter.EngravingIsFinish;
            set
            {
                PartsParameter.ShearingIsFinish = value; OnPropertyChanged();
            }
        }
        /// <summary>
        /// 已剪斷
        /// </summary>
        public bool ShearingIsFinish
        {
            get => PartsParameter.ShearingIsFinish;
            set
            {
                PartsParameter.ShearingIsFinish = value; OnPropertyChanged();
            }
        }

        public bool IsFinish
        {
            get => PartsParameter.IsFinish;
            set
            {
                PartsParameter.IsFinish = value; OnPropertyChanged();
            }
        }


        /// <summary>
        /// 已送進機台內
        /// </summary>
        public bool IsSended
        {
            get => PartsParameter.SendMachineCommand.IsSended; 
            set
            { 
                PartsParameter.SendMachineCommand.IsSended = value; 
                OnPropertyChanged(); 
            }
        }

        /// <summary>
        /// 加工順序編號
        /// </summary>
        public int WorkIndex
        {
            get => PartsParameter.SendMachineCommand.WorkIndex;
            set
            { 
                PartsParameter.SendMachineCommand.WorkIndex = value; 
                OnPropertyChanged(); 
            }
        }




        public string ParameterA
        {
            get => PartsParameter.ParamA;
            set
            {
                PartsParameter.ParamA = value;
                OnPropertyChanged();
            }
        }
        public string ParameterB
        {
            get => PartsParameter.ParamB;
            set
            {
                PartsParameter.ParamB = value;
                OnPropertyChanged();
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

        public string IronPlateString
        { 
            get=> PartsParameter.IronPlateString; 
            set
            { 
                PartsParameter.IronPlateString = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SettingBaseVM));
            }
        }

        public string QrCodeContent
        {
            get => PartsParameter.QrCodeContent;
            set
            {
                PartsParameter.QrCodeContent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SettingBaseVM));
            }
        }

        /// <summary>
        /// 側邊字串(橫著打)
        /// </summary>
        public string QR_Special_Text
        {
            get => PartsParameter.QR_Special_Text;
            set
            {
                PartsParameter.QR_Special_Text = value;
                OnPropertyChanged();
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

        private SettingBaseViewModel _settingBaseVM;//= new NumberSettingViewModel();
        /// <summary>
        /// 金屬牌樣式
        /// </summary>
        public SettingBaseViewModel SettingBaseVM
        {
            get
            {
                if (PartsParameter.StampingPlate.SheetStampingTypeForm == SheetStampingTypeFormEnum.qrcode)
                {
                    _settingBaseVM ??= new QRSettingViewModel(PartsParameter.StampingPlate);
                }
                else
                {
                    _settingBaseVM ??= new NumberSettingViewModel(PartsParameter.StampingPlate);
                }
                _settingBaseVM.PlateNumber = IronPlateString;
              


                return _settingBaseVM;
            }
            set
            {
                 _settingBaseVM = value;
                if (value != null)
                {
                    PartsParameter.StampingPlate = value.StampPlateSetting;
                }
                OnPropertyChanged();
            }
        }
        
                                           

        public bool _editPartDarggableIsPopup;
        /// <summary>
        /// 編輯視窗
        /// </summary>
        public bool EditPartDarggableIsPopup
        {
            get => _editPartDarggableIsPopup;
            set  { _editPartDarggableIsPopup = value;OnPropertyChanged();
            }
        }


        [JsonIgnore]
        public RelayCommand ProjectEditCommand
        {
            get => new(() =>
            {
                EditPartDarggableIsPopup = true;
            });
        }

        [JsonIgnore]
        public AsyncRelayCommand<GridControl> ProjectDeleteCommand
        {
            get => new (async obj =>
            {
                    if (obj is not null)
                    {
                        if (obj.ItemsSource is ObservableCollection<PartsParameterViewModel> GridItemSource)
                        {
                            if (SettingBaseVM != null)
                            {
                                if (await MethodWinUIMessageBox.AskDelProject(this.SettingBaseVM.PlateNumber))
                                {
                                    GridItemSource.Remove(this);
                                }
                            }
                            else
                                GridItemSource.Remove(this);

                        }
                    }
            });
        }



        private SendMachineCommandViewModel _sendMachineCommandVM;

      //  [JsonIgnore]
       /* public SendMachineCommandViewModel SendMachineCommandVM
        {
            get => _sendMachineCommandVM ??= new SendMachineCommandViewModel(PartsParameter.SendMachineCommand);
            set { _sendMachineCommandVM = value;OnPropertyChanged();}
        }*/
    }







}
