using GD_CommonControlLibrary;
using GD_StampingMachine.Model;
using System.Text.Json.Serialization;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 字模
    /// </summary>
    public class PlateFontViewModel : BaseViewModel
    {
        public PlateFontViewModel()
        {
            PlateFont = new PlateFontModel();
        }

        public PlateFontViewModel(PlateFontModel plateFont)
        {
            PlateFont = plateFont;
        }

        [JsonIgnore]
        public readonly PlateFontModel PlateFont = new PlateFontModel();

        public override string ViewModelName => nameof(PlateFontViewModel);

        public ushort FontIndex
        {
            get => PlateFont.FontIndex;
            set
            {
                PlateFont.FontIndex = value;
                OnPropertyChanged();
            }
        }

        public bool IsUsed
        {
            get => PlateFont.IsUsed;
            set
            {
                PlateFont.IsUsed = value;
                OnPropertyChanged();
            }
        }

        public string FontString
        {
            get => PlateFont.FontString; set { PlateFont.FontString = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 是否可變更IsUsed
        /// </summary>
        public bool IsUsedEditedable
        {
            get => PlateFont.IsUsedEditedable; set { PlateFont.IsUsedEditedable = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        public double MachineConstFontWidth => MachineConst.FontWidth;
        [JsonIgnore]
        public double MachineConstFontHorizonInterval => MachineConst.FontHorizonInterval;
        [JsonIgnore]
        public double MachineConstFontHeight => MachineConst.FontHeight;
        [JsonIgnore]
        public double MachineConstFontVerticalInterval => MachineConst.FontVerticalInterval;


    }

}
