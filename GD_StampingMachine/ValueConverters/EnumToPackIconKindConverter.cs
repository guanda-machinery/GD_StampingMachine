using GD_StampingMachine.Extensions;
using GD_StampingMachine.GD_Enum;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml.Linq;

namespace GD_StampingMachine.ValueConverters
{
    internal class EnumToPackIconKindConverter : BaseValueConverter
    {
         public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is System.Enum EnumValue)
            {
                if (EnumValue is VerticalAlignEnum VerticalAlign)
                {
                    switch (VerticalAlign)
                    {
                        case (VerticalAlignEnum.Top):
                            return PackIconKind.FormatAlignTop;
                        case (VerticalAlignEnum.Center):
                            return PackIconKind.FormatAlignMiddle;
                        case (VerticalAlignEnum.Bottom):
                            return PackIconKind.FormatAlignBottom;
                        default:
                            break;
                    }
                }

                if (EnumValue is HorizontalAlignEnum HorizontalAlign)
                {
                    switch (HorizontalAlign)
                    {
                        case (HorizontalAlignEnum.Left):
                            return PackIconKind.FormatAlignLeft;
                        case (HorizontalAlignEnum.Center):
                            return PackIconKind.FormatAlignCenter;
                        case (HorizontalAlignEnum.Right):
                            return PackIconKind.FormatAlignRight;
                        default:
                            break;
                    }
                }

                if (EnumValue is SpecialSequenceEnum SpecialSequence)
                {
                    switch (SpecialSequence)
                    {
                        case (SpecialSequenceEnum.OneRow):
                            return PackIconKind.Minus;
                        case (SpecialSequenceEnum.TwoRow):
                            return PackIconKind.Equal;
                        default:
                            break;
                    }
                }


                return PackIconKind.Help;
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (System.Enum EnumA in System.Enum.GetValues(targetType))
            {
                if (EnumA.GetDescription() == value.ToString())
                {
                    return EnumA;
                }



            }

            return value;
        }

       
    }
}
