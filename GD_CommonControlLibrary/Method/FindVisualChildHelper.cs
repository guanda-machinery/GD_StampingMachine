using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace GD_CommonControlLibrary.Extensions
{
    public static class FindVisualChildHelper
    {
        /// <summary>
        /// 尋找子項目
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="visualChildName"></param>
        /// <returns></returns>
        public static FrameworkElement FindVisualChild(this Visual visual , string visualChildName)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual child = (Visual)VisualTreeHelper.GetChild(visual, i);
                if (child is FrameworkElement frameworkElement && frameworkElement.Name == visualChildName)
                {
                    return frameworkElement;
                }
            }
            return null;
        }
    }
}
