using GD_CommonControlLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Singletons;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels.MachineMonitor
{

    public class PlateMonitorModel 
    {
        public SettingBaseViewModel? SettingBaseVM { get; set; }

        public SteelBeltStampingStatusEnum StampingStatus { get; set; }

        public int ID { get; set; }
        public string? ProductProjectName { get; set; }
        public bool DataMatrixIsFinish { get; set; }
        public bool EngravingIsFinish { get; set; }
        public bool ShearingIsFinish { get; set; }
    }


    public class PlateMonitorViewModel : BaseViewModel
    {
        public override string ViewModelName => nameof(PlateMonitorViewModel);

        public PlateMonitorViewModel() 
        {
            PlateMonitor = new();
        }

        public PlateMonitorViewModel(IronPlateDataModel ironPlateData)
        {

            //取得字元長度
            var string1Length = ironPlateData.sIronPlateName1.Length;
            var string2Length = ironPlateData.sIronPlateName2.Length;
            SpecialSequenceEnum specialSequence;
            if (string2Length > 0)
                specialSequence = SpecialSequenceEnum.TwoRow;
            else
                specialSequence = SpecialSequenceEnum.OneRow;


            SteelBeltStampingStatusEnum steelBeltStampingStatus = SteelBeltStampingStatusEnum.None;
            if (ironPlateData.bEngravingFinish)
                steelBeltStampingStatus = SteelBeltStampingStatusEnum.Stamping;
            else if (ironPlateData.bDataMatrixFinish)
                steelBeltStampingStatus = SteelBeltStampingStatusEnum.QRCarving;

            int rowLength = Math.Max(string1Length, string2Length);
            SettingBaseViewModel settingBaseVM;
            //沒有QR加工
            if (string.IsNullOrEmpty(ironPlateData.sDataMatrixName1) && string.IsNullOrEmpty(ironPlateData.sDataMatrixName2))
            {
                settingBaseVM = new NumberSettingViewModel();
            }
            else
            {
                settingBaseVM = new QRSettingViewModel();
            }
            settingBaseVM.SpecialSequence = specialSequence;
            settingBaseVM.SequenceCount = rowLength;
            settingBaseVM.PlateNumber = string.Concat(ironPlateData.sIronPlateName1.PadRight(rowLength).AsSpan(0, rowLength), ironPlateData.sIronPlateName2);
            settingBaseVM.QrCodeContent = ironPlateData.sDataMatrixName1;
            settingBaseVM.QR_Special_Text = ironPlateData.sDataMatrixName2;
            settingBaseVM.StampingMarginPosVM = new StampingMarginPosViewModel()
            {
                rXAxisPos1 = ironPlateData.rXAxisPos1,
                rYAxisPos1 = ironPlateData.rYAxisPos1 - MachineConstants.StampingMachineYPosition,
                rXAxisPos2 = ironPlateData.rXAxisPos2,
                rYAxisPos2 = ironPlateData.rYAxisPos2 - MachineConstants.StampingMachineYPosition,
            };
            foreach (var num1 in settingBaseVM.PlateNumberList1)
            {
                if (string.IsNullOrWhiteSpace(num1.FontString))
                    num1.IsUsed = false;
            }
            foreach (var num2 in settingBaseVM.PlateNumberList2)
            {
                if (string.IsNullOrWhiteSpace(num2.FontString))
                    num2.IsUsed = false;
            }

            string productProjectName = string.Empty;
            foreach (var projectDistribute in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
            {
                //去盒子裡面找是否有對應的鐵片
                var boxPartsCollection = projectDistribute.StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x => x.BoxPartsParameterVMCollection);
                var foundPart = boxPartsCollection.FirstOrDefault(x => x.ID == ironPlateData.iIronPlateID && x.IsSended);
                if (foundPart != null)
                {
                    foundPart.DataMatrixIsFinish = ironPlateData.bDataMatrixFinish;
                    foundPart.EngravingIsFinish = ironPlateData.bEngravingFinish;
                    productProjectName = foundPart.ProductProjectName;

                    try
                    {
                        _ = Task.Run(async () =>
                        {
                            await projectDistribute.SaveProductProjectVMCollectionAsync();
                        });
                    }
                    catch
                    {

                    }
                    break;
                }
            }

            PlateMonitor = new()
            {
                ID = ironPlateData.iIronPlateID,
                ProductProjectName = productProjectName,
                SettingBaseVM = settingBaseVM,
                StampingStatus = steelBeltStampingStatus,
                DataMatrixIsFinish = ironPlateData.bDataMatrixFinish,
                EngravingIsFinish = ironPlateData.bEngravingFinish,
            };



        }






        readonly PlateMonitorModel PlateMonitor;

        public SettingBaseViewModel? SettingBaseVM
        {
            get => PlateMonitor.SettingBaseVM;
            set { PlateMonitor.SettingBaseVM = value; OnPropertyChanged(); }
        }

        public SteelBeltStampingStatusEnum StampingStatus
        {
            get => PlateMonitor.StampingStatus;
            set
            {
                PlateMonitor.StampingStatus = value;
                OnPropertyChanged();
            }

        }

        public int ID
        {
            get => PlateMonitor.ID; 
            set
            {
                PlateMonitor.ID = value; OnPropertyChanged();
            }
        }

        public string? ProductProjectName
        {
            get => PlateMonitor.ProductProjectName;
            set
            {
                PlateMonitor.ProductProjectName = value; OnPropertyChanged();
            }
        }


        public bool DataMatrixIsFinish
        {
            get => PlateMonitor.DataMatrixIsFinish; set { PlateMonitor.DataMatrixIsFinish = value; OnPropertyChanged(); }
        }

        public bool EngravingIsFinish
        {
            get => PlateMonitor.EngravingIsFinish; set { PlateMonitor.EngravingIsFinish = value; OnPropertyChanged(); }
        }

        public bool ShearingIsFinish
        {
            get => PlateMonitor.ShearingIsFinish; set { PlateMonitor.ShearingIsFinish = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 沒有使用到本屬性
        /// </summary>
        [Obsolete]
        public bool IsFinish
        {
            get => false;
        }
    }
}
