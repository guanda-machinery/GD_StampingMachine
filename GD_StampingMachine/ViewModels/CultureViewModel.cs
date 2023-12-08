using GD_StampingMachine.Singletons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class CultureViewModel : GD_CommonLibrary.BaseViewModel
    {
        public CultureViewModel()
        {
            var defaultCulture = CultureInfo.InvariantCulture;
            try
            {
                defaultCulture = Properties.Settings.Default.DefaultCulture;
            }
            catch
            {

            }

            if (defaultCulture != CultureInfo.InvariantCulture)
            {
                SelectedCulture = defaultCulture;
            }
            else
            {
                var CurrentCulture = Thread.CurrentThread.CurrentCulture;
                if (SupportedCultures.Contains(CurrentCulture))
                {
                    SelectedCulture = CurrentCulture;
                }
            }
        }


         public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_LanguageSettingViewModel");

        public List<CultureInfo> SupportedCultures
        {
            get
            {
                return CulturesHelper.GetSupportedCultures();
            }
        }



        private CultureInfo _selectedCulture;

        public CultureInfo SelectedCulture
        {
            get
            {
                return _selectedCulture;
            }
            set
            {
                _selectedCulture = value;
                if (_selectedCulture != null)
                {
                    try
                    {
                        CulturesHelper.ChangeCulture(_selectedCulture);
                        Properties.Settings.Default.DefaultCulture = _selectedCulture;
                        Properties.Settings.Default.Save();

                    }
                    catch (Exception ex)
                    {
                        _ = LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ex.Message);
                    }
                }
                OnPropertyChanged();
            }
        }
    }
}
