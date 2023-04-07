﻿using DevExpress.Mvvm.CodeGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace GD_StampingMachine.Views
{
    /// <summary>
    /// Interaction logic for StampingMainView.xaml
    /// </summary>
    public partial class StampingMainView : UserControl
    {
        public StampingMainView()
        {
            InitializeComponent();
        }

        private void TableView_DragRecordOver(object sender, DevExpress.Xpf.Core.DragRecordOverEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }
    }
}
