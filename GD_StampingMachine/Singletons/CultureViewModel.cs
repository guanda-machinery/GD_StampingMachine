using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace GD_StampingMachine.Singletons
{

    // public class CultureViewModel : GD_CommonLibrary.BaseViewModel
    public class CultureViewModel : GD_CommonLibrary.BaseSingleton<CultureViewModel>
    {
        private CultureViewModel()
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
                var CurrentCulture = CultureInfo.CurrentCulture;
                if (SupportedCultures.Contains(CurrentCulture))
                {
                    SelectedCulture = CurrentCulture;
                }
            }
        }


        public string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_LanguageSettingViewModel");

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

                        Thread.CurrentThread.CurrentCulture = value;
                        CultureInfo.CurrentCulture = value;

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
