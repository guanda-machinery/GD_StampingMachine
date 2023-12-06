using GD_CommonLibrary;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 字模
    /// </summary>
    public class PlateFontViewModel : BaseViewModel
    {
        public readonly PlateFontModel PlateFont = new PlateFontModel();

        public override string ViewModelName => nameof(PlateFontViewModel);
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



    }

}
