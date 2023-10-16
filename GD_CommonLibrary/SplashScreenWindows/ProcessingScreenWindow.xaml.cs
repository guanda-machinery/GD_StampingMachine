﻿using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GD_CommonLibrary.SplashScreenWindows
{
    /// <summary>
    /// ProcessingScreenWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ProcessingScreenWindow : SplashScreenWindow
    {
        public ProcessingScreenWindow()
        {
            InitializeComponent();
            var orgintopmost = this.Topmost;
            this.Topmost = true;
            this.Topmost = orgintopmost;
        }
    }
}
