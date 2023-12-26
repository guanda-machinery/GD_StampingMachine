using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using System.Windows;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 繼承
    /// </summary>
    public class NumberSettingViewModel : SettingBaseViewModel
    {
        public NumberSettingViewModel() : base(new StampPlateSettingModel())
        {
            SheetStampingTypeForm = SheetStampingTypeFormEnum.NormalSheetStamping;
            this.SpecialSequence = SpecialSequenceEnum.TwoRow;
            this.SequenceCount = 8;
            this.StampingMarginPosVM.rXAxisPos1 = 10;
            this.StampingMarginPosVM.rXAxisPos2 = 25;
            this.StampingMarginPosVM.rYAxisPos1 = 14;
            this.StampingMarginPosVM.rYAxisPos2 = 14;
        }
        public NumberSettingViewModel(StampPlateSettingModel stampPlateSetting)
        : base(stampPlateSetting)
        {
            SheetStampingTypeForm = SheetStampingTypeFormEnum.NormalSheetStamping;
        }

        protected override int SequenceCountMax => 8;
        public override Visibility QRCodeVisibility => Visibility.Collapsed;
    }




}









