using GD_StampingMachine.GD_Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using GD_CommonLibrary;

namespace GD_StampingMachine
{
    public class EnumToBrushesConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is SteelBeltStampingStatusEnum SteelBeltStampingStatus)
            {
                switch(SteelBeltStampingStatus)
                {
                    case (SteelBeltStampingStatusEnum.None):
                        return System.Windows.Media.Brushes.Transparent;
                    case (SteelBeltStampingStatusEnum.QRCarving):
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#e82127");
                    case (SteelBeltStampingStatusEnum.Stamping): 
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#ff9900");
                    case (SteelBeltStampingStatusEnum.Shearing): 
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#25cb55");
                    default:
                        break;
                }
                return System.Windows.Media.Brushes.White;
            }

            if(value is MachiningStatusEnum MachiningStatus)
            {
                switch(MachiningStatus)
                {
                    case (MachiningStatusEnum.Unknown):
                        return Brushes.White;
                    case (MachiningStatusEnum.Stop):
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#ff0000");
                    case (MachiningStatusEnum.Run):
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#ff9900");
                    case (MachiningStatusEnum.Finish):
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#14ff00");
                    default:
                        break;
                }
            }

            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
