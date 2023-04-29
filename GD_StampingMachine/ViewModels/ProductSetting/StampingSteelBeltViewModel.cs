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
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingSteelBeltViewModel");
        public StampingSteelBeltViewModel(StampingSteelBeltModel _StampingSteelBelt)
        {
            StampingSteelBelt = _StampingSteelBelt;
        }

        private StampingSteelBeltModel _stampingSteelBelt;
        public StampingSteelBeltModel StampingSteelBelt { get => _stampingSteelBelt ??= new StampingSteelBeltModel(); set => _stampingSteelBelt = value; }

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
