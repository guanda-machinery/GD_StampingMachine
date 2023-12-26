using System.Windows;
using System.Windows.Media;

namespace GD_StampingMachine.Singletons
{
    public class ColorSettingSingleton : GD_CommonLibrary.BaseSingleton<ColorSettingSingleton>
    {

        private Color _noneIsFinishSolidColor;
        public Color NoneIsFinishSolidColor
        {
            get => _noneIsFinishSolidColor;
            set
            {
                _noneIsFinishSolidColor = value;
                OnPropertyChanged();
                Application.Current.Resources["DataMatrixIsFinishSolidColorBrush"] = new SolidColorBrush(value);
            }
        }


    }
}
