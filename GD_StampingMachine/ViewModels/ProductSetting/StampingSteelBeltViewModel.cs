using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model.ProductionSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    public class StampingSteelBeltViewModel : BaseViewModelWithLog
    {
        public StampingSteelBeltViewModel(StampingSteelBeltModel _stampingSteelBelt)
        {
            StampingSteelBelt = _stampingSteelBelt;
        }
        public StampingSteelBeltModel StampingSteelBelt { get; } =default;

        public string BeltString
        {
            get => StampingSteelBelt.BeltString;
            set
            {
                StampingSteelBelt.BeltString=value; 
                OnPropertyChanged();
            }
        }
        public string BeltNumberString
        {
            get => StampingSteelBelt.BeltNumberString;
            set
            {
                StampingSteelBelt.BeltNumberString = value; 
                OnPropertyChanged();
            }
        }
        public SteelBeltStampingStatusEnum MachiningStatus
        {
            get => StampingSteelBelt.MachiningStatus;
            set
            {
                StampingSteelBelt.MachiningStatus = value; 
                OnPropertyChanged();
            }
        }

    }
}
