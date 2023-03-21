using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    public class ProductProjectModel : CloneableModelBase
    {
        public string ProjectPath { get; set; }
        /// <summary>
        /// 工程編號
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 型態
        /// </summary>
        public string Form { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改時間
        /// </summary>
        public DateTime? EditTime { get; set; }

        public double FinishProgress { get; set; }


        /// <summary>
        /// 參數設定表
        /// </summary>
        //public ObservableCollection













    }
}
