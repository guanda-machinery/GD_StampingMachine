using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    public class MachiningProjectModel
    {          
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 製作數量
        /// </summary>
        public double WorkPieceCurrent { get; set; }

        /// <summary>
        /// 目標製作數量
        /// </summary>
        public double WorkPieceTarget { get; set; }

    }
}
