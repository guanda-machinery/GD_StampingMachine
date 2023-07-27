using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
using GD_CommonLibrary;

namespace GD_CommonLibrary
{
    public class HundredDoubleToSolidBrushConverter : BaseValueConverter
    {
        public double SliderMin { get; set; } = 0;
        public double SliderMax { get; set; } = 100;

        public SolidColorBrush MinBrush { get; set; } = System.Windows.Media.Brushes.Red;
        public SolidColorBrush MaxBrush { get; set; } = System.Windows.Media.Brushes.Green;


        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var Denominator = Math.Abs(SliderMax - SliderMin);
            //思路:先將HEX色碼分成三部分 #00 00 00
            //將16進制轉成10進制
            //比較兩個色碼的差值
            //計算目前落在哪個區間->變色

            if (double.TryParse(value.ToString() , out double result))
            {
                var A_Diff = MaxBrush.Color.A- MinBrush.Color.A;
                var R_Diff = MaxBrush.Color.R - MinBrush.Color.R;
                var G_Diff = MaxBrush.Color.G - MinBrush.Color.G;
                var B_Diff = MaxBrush.Color.B - MinBrush.Color.B;


                var A = (byte)(MinBrush.Color.A + A_Diff * (result / Denominator));
                var R = (byte)(MinBrush.Color.R + R_Diff * (result / Denominator));
                var G = (byte)(MinBrush.Color.G + G_Diff * (result / Denominator));
                var B = (byte)(MinBrush.Color.B + B_Diff * (result / Denominator));

                SolidColorBrush ReturnBrush = new SolidColorBrush(Color.FromArgb(A, R, G, B));

                if (result < SliderMin)
                {
                    //    return (SolidColorBrush)new BrushConverter().ConvertFrom(Brushes.OrangeRed.ToString());
                    return System.Windows.Media.Brushes.Red;
                }
                if (result > SliderMax)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else
                    return ReturnBrush;
                /*
                if (result >= 0 && result < 25)
                {
                //    return (SolidColorBrush)new BrushConverter().ConvertFrom(Brushes.OrangeRed.ToString());
                    return System.Windows.Media.Brushes.Red;
                }
                if (result >= 25 && result < 50)
                {
                    return System.Windows.Media.Brushes.OrangeRed;
                }
                if (result >= 50 && result < 75)
                {
                    return System.Windows.Media.Brushes.Orange;
                }
                if (result >= 75 && result <100)
                {
                    return System.Windows.Media.Brushes.DarkOrange;
                }
                if (result >=100)
                {
                    return System.Windows.Media.Brushes.Green;
                }*/

                return System.Windows.Media.Brushes.Violet;
            }

           return null;
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }

    public class MuiltBrushes_DoubleToSolidBrushConverter : BaseValueConverter
    {
        public double SliderMin { get; set; } = 0;
        public double SliderMax { get; set; } = 100;

        public SolidColorBrush[] BrushArray { get; set; } = new SolidColorBrush[] { Brushes.Red, Brushes.Orange , Brushes.Yellow};


        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (BrushArray.Count() == 0)
                throw new Exception();
            Debugger.Break();

            var Denominator = Math.Abs(SliderMax - SliderMin);
            //思路:先將HEX色碼分成三部分 #00 00 00
            //將16進制轉成10進制
            //比較兩個色碼的差值
            //計算目前落在哪個區間->變色




            if (double.TryParse(value.ToString(), out double result))
            {
                //先找出result的區間 再找陣列中的位置

                




                SolidColorBrush LeftBrush = System.Windows.Media.Brushes.Red;
                SolidColorBrush RightBrush = System.Windows.Media.Brushes.Green;

                var A_Diff = RightBrush.Color.A - LeftBrush.Color.A;
                var R_Diff = RightBrush.Color.R - LeftBrush.Color.R;
                var G_Diff = RightBrush.Color.G - LeftBrush.Color.G;
                var B_Diff = RightBrush.Color.B - LeftBrush.Color.B;

                var A = (byte)(LeftBrush.Color.A + A_Diff * (result / Denominator));
                var R = (byte)(LeftBrush.Color.R + R_Diff * (result / Denominator));
                var G = (byte)(LeftBrush.Color.G + G_Diff * (result / Denominator));
                var B = (byte)(LeftBrush.Color.B + B_Diff * (result / Denominator));

                SolidColorBrush ReturnBrush = new SolidColorBrush(Color.FromArgb(A, R, G, B));

                if (result < SliderMin)
                {
                    //    return (SolidColorBrush)new BrushConverter().ConvertFrom(Brushes.OrangeRed.ToString());
                    return System.Windows.Media.Brushes.Red;
                }
                if (result > SliderMax)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else
                    return ReturnBrush;

            }

            return null;
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


}
