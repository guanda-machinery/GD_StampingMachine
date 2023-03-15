using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class CultureViewModel : ViewModelBase
    {
        public List<CultureInfo> SupportedCultures
        {
            get
            {
                return CulturesHelper.SupportedCultures;
            }
        }

        private CultureInfo _selectedCulture;
        public CultureInfo SelectedCultures
        {
            get
            {
                if (_selectedCulture == null)
                {
                    CulturesHelper.ChangeCulture(_selectedCulture);
                }
                return _selectedCulture;
            }
            set
            {
                _selectedCulture = value;
                OnPropertyChanged(nameof(SelectedCultures));
            }
        }
    }
}
