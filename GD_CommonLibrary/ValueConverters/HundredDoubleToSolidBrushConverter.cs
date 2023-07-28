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
using DevExpress.Charts.Native;
using DevExpress.Data.Extensions;
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

        //public SolidColorBrush[] BrushArray { get; set; } = new SolidColorBrush[] { Brushes.Red, Brushes.Orange , Brushes.Yellow};


        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return Brushes.Black;

            var BrushArray = new List<SolidColorBrush> { Brushes.Red, Brushes.Orange, Brushes.Yellow };
            if (parameter is IEnumerable<SolidColorBrush> parameterBrushList)
            {
                BrushArray = parameterBrushList.ToList();
            }

            if (parameter is SolidColorBrush parameterSolidColorBrush)
            {
                return parameterSolidColorBrush;
            }

            var BrushCount = BrushArray.Count();
            if (BrushCount <= 1)
                return BrushArray.FirstOrDefault();

            //Debugger.Break();

            var Denominator = Math.Abs(SliderMax - SliderMin);
            //思路:先將HEX色碼分成三部分 #00 00 00
            //將16進制轉成10進制
            //比較兩個色碼的差值
            //計算目前落在哪個區間->變色
            if (double.TryParse(value.ToString(), out double result))
            {
                //先找出result的區間 再找陣列中的位置
                var Result_step = result / Denominator;
                //找出區間
                //0...1...2...3...4...5
                //| 1 | 2 | 3 | 4 | 5 |

                SolidColorBrush LeftBrush = BrushArray.FirstOrDefault();
                SolidColorBrush RightBrush = BrushArray.LastOrDefault();
                var Brush_step = 1.0 / (BrushCount-1);
                for (int i = 0; i <= BrushCount; i ++)
                {
                    if(((double)(i+1.0)* Brush_step) > Result_step)
                    {
                        if (BrushArray.TryGetValue(i, out var oLeftBrush))
                            LeftBrush = oLeftBrush;
                        if(BrushArray.TryGetValue(i+1, out var oRightBrush))
                            RightBrush = oRightBrush;

                        //若有三種顏色
                        //max =200 , min =0 , value =104
                        //先將104/(200-0) =0.52
                        //找出該顏色落在第 i = "1" 區間 使用"1"和"2"的顏色 間距 = 1/(3-1) =0.5
                        //0.52- 0.5 * 1

                        Result_step = (result / Denominator - (Brush_step * i)) * (BrushCount - 1);

                        break;
                    }
                }

                var A_Diff = RightBrush.Color.A - LeftBrush.Color.A;
                var R_Diff = RightBrush.Color.R - LeftBrush.Color.R;
                var G_Diff = RightBrush.Color.G - LeftBrush.Color.G;
                var B_Diff = RightBrush.Color.B - LeftBrush.Color.B;

                var A = (byte)(LeftBrush.Color.A + A_Diff * Result_step);
                var R = (byte)(LeftBrush.Color.R + R_Diff * Result_step);
                var G = (byte)(LeftBrush.Color.G + G_Diff * Result_step);
                var B = (byte)(LeftBrush.Color.B + B_Diff * Result_step);

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
