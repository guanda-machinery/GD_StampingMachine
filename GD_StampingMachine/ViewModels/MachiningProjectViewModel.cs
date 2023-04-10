﻿using DevExpress.Utils.Drawing;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class MachiningProjectViewModel:ViewModelBase
    {
        public MachiningProjectViewModel(MachiningProjectModel _machiningProject)
        {
            MachiningProject = _machiningProject;
        }

        private readonly MachiningProjectModel MachiningProject = new();
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjectName { get => MachiningProject.ProjectName; set { MachiningProject.ProjectName = value;OnPropertyChanged(); } }
        /// <summary>
        /// 製作數量
        /// </summary>
        public double WorkPieceCurrent { get => MachiningProject.WorkPieceCurrent; set { MachiningProject.WorkPieceCurrent = value; OnPropertyChanged(); } }

        /// <summary>
        /// 目標製作數量
        /// </summary>
        public double WorkPieceTarget { get => MachiningProject.WorkPieceTarget; set { MachiningProject.WorkPieceTarget = value; OnPropertyChanged(); } }













    }
}
