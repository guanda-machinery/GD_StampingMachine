using DevExpress.Mvvm.Native;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine
{
    internal class SettingBaseVMListFilterWithParameterConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<SettingBaseViewModel> SettingBaseCollection)
            {
                if(parameter is SheetStampingTypeFormEnum TypeEnum)
                {
                    if(TypeEnum != SheetStampingTypeFormEnum.None)
                    {
                       return SettingBaseCollection.ToList().FindAll(x => x.SheetStampingTypeForm == TypeEnum).ToObservableCollection();
                    }
                }
                return value;
            }

            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
