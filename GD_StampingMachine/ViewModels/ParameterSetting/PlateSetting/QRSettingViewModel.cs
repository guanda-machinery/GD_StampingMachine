using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using System.Windows;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 繼承
    /// </summary>
    public class QRSettingViewModel : SettingBaseViewModel//, IStampingPlateVM
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SettingViewModelQRViewModel");

        public QRSettingViewModel() : base(new StampPlateSettingModel())
        {
            this.SheetStampingTypeForm = SheetStampingTypeFormEnum.QRSheetStamping;
            this.SequenceCount = 6;
            this.SpecialSequence = SpecialSequenceEnum.TwoRow;
        }

        public QRSettingViewModel(StampPlateSettingModel stampPlateSetting)
        : base(stampPlateSetting)
        {
            this.SheetStampingTypeForm = SheetStampingTypeFormEnum.QRSheetStamping;

        }

        protected override int SequenceCountMax => 6;

        public override Visibility QRCodeVisibility { get => Visibility.Visible; }

        //public override SheetStampingTypeFormEnum SheetStampingTypeForm { get => SheetStampingTypeFormEnum.QRSheetStamping; }



        /// <summary>
        /// Code設定 字元數量
        /// </summary>
        public int CharactersCount { get => StampPlateSetting.CharactersCount; set { StampPlateSetting.CharactersCount = value; OnPropertyChanged(); } }
        /// <summary>
        /// Code設定 字元型態
        /// </summary>
        public CharactersFormEnum CharactersForm { get => StampPlateSetting.CharactersForm; set { StampPlateSetting.CharactersForm = value; OnPropertyChanged(); } }

        public string ModelSize { get => StampPlateSetting.ModelSize; set { StampPlateSetting.ModelSize = value; OnPropertyChanged(); } }

    }










}
