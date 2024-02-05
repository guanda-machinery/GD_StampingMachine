using CommunityToolkit.Mvvm.Input;
using DevExpress.Xpf.Grid;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model.ProductionSetting;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels.ParameterSetting;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    /// <summary>
    /// ABC參數加工型
    /// </summary>
    public partial class PartsParameterViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        [JsonIgnore]
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_PartsParameterViewModel");

        public PartsParameterViewModel()
        {
            PartsParameter = new();
            //SettingBaseVM.StampPlateSetting = PartsParameter.StampingPlate;
        }
        public PartsParameterViewModel(PartsParameterModel PParameter)
        {
            PartsParameter = PParameter;
            //SettingBaseVM.StampPlateSetting = PartsParameter.StampingPlate;
        }


        //private PartsParameterModel _partsParameter;
        public readonly PartsParameterModel PartsParameter;

        /// <summary>
        /// 加工進程
        /// </summary>
        public float FinishProgress
        {
            get => PartsParameter.Processing;
            set
            {
                PartsParameter.Processing = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///加工分配專案名
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
        /// 製品設定專案名
        /// </summary>
        public string ProductProjectName
        {
            get => PartsParameter.ProductProjectName;
            set
            {
                PartsParameter.ProductProjectName = value; OnPropertyChanged();
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
                PartsParameter.EngravingIsFinish = value; OnPropertyChanged();
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
        /// <summary>
        /// 已完成
        /// </summary>
        public bool IsFinish
        {
            get => PartsParameter.IsFinish;
            set
            {
                PartsParameter.IsFinish = value; OnPropertyChanged();
            }
        }

        public bool IsTransported
        {
            get => PartsParameter.IsTransported;
            set
            {
                PartsParameter.IsTransported = value; OnPropertyChanged();
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
            get => PartsParameter.IronPlateString;
            set
            {
                PartsParameter.IronPlateString = value;
                SettingBaseVM.PlateNumber = value;
                OnPropertyChanged();
            }
        }
        /*public string ParameterB
        {
            get => PartsParameter.ParamB;
            set
            {
                PartsParameter.ParamB = value;
                OnPropertyChanged();
            }
        }*/
        public string ParameterC
        {
            get => PartsParameter.QrCodeContent;
            set
            {
                PartsParameter.QrCodeContent = value;
                SettingBaseVM.QrCodeContent = value;
                OnPropertyChanged(nameof(ParameterC));
            }
        }

        /*public string IronPlateString
        { 
            get=> PartsParameter.IronPlateString; 
            set
            { 
                PartsParameter.IronPlateString = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SettingBaseVM));
            }
        }*/

        /*public string QrCodeContent
        {
            get => PartsParameter.ParamC;
            set
            {
                PartsParameter.ParamC = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SettingBaseVM));
            }
        }*/

        /// <summary>
        /// 側邊字串(橫著打)
        /// </summary>
        public string QR_Special_Text
        {
            get => PartsParameter.QR_Special_Text;
            set
            {
                PartsParameter.QR_Special_Text = value;
                SettingBaseVM.QR_Special_Text = value;
                OnPropertyChanged();
            }
        }

        /*public MachiningStatusEnum MachiningStatus
        {
            get => PartsParameter.MachiningStatus;
            set
            {
                PartsParameter.MachiningStatus = value;
                OnPropertyChanged(nameof(MachiningStatus));
            }
        }*/


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
                if (PartsParameter.StampingPlate.SheetStampingTypeForm == SheetStampingTypeFormEnum.QRSheetStamping)
                {
                    _settingBaseVM ??= new QRSettingViewModel(PartsParameter.StampingPlate);
                }
                else
                {
                    _settingBaseVM ??= new NumberSettingViewModel(PartsParameter.StampingPlate);
                }

                _settingBaseVM.PlateNumber = this.ParameterA;
                _settingBaseVM.QR_Special_Text = this.QR_Special_Text;
                _settingBaseVM.QrCodeContent = this.ParameterC;
                return _settingBaseVM;
            }
            set
            {
                _settingBaseVM = value;
                if (value != null)
                    PartsParameter.StampingPlate = value.StampPlateSetting;
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
            set
            {
                _editPartDarggableIsPopup = value; OnPropertyChanged();
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
            get => new(async obj =>
            {
                if (obj is not null)
                {
                    if (obj.ItemsSource is ObservableCollection<PartsParameterViewModel> GridItemSource)
                    {
                        if (SettingBaseVM != null)
                        {
                            if (await MethodWinUIMessageBox.AskDelProjectAsync(this.SettingBaseVM.PlateNumber) == MessageBoxResult.Yes)
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



        // private SendMachineCommandViewModel _sendMachineCommandVM;

        //  [JsonIgnore]
        /* public SendMachineCommandViewModel SendMachineCommandVM
         {
             get => _sendMachineCommandVM ??= new SendMachineCommandViewModel(PartsParameter.SendMachineCommand);
             set { _sendMachineCommandVM = value;OnPropertyChanged();}
         }*/
    }







}
