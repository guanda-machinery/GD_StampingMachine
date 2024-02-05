using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace GD_StampingMachine.Singletons
{

    // public class CultureViewModel : GD_CommonControlLibrary.BaseViewModel
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

        public ObservableCollection<CultureInfo> SupportedCultures
        {
            get
            {
                return CulturesHelper.GetSupportedCultures().ToObservableCollection() ?? new ObservableCollection<CultureInfo>();
            }
        }


        private ICommand _changeToNextCultureCommand;
        public  ICommand ChangeToNextCultureCommand
        {
            get => _changeToNextCultureCommand??= new RelayCommand(()=>
            {
                ChangeToNextCulture();
            });
        }


        public void ChangeToNextCulture()
        {
            var index = SupportedCultures.IndexOf(SelectedCulture);
            SelectedCulture = index != -1 ? SupportedCultures[(index + 1) % SupportedCultures.Count] : SupportedCultures.FirstOrDefault();
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
