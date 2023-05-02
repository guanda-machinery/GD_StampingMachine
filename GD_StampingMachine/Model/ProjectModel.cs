using DevExpress.CodeParser;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    public class ProjectModel
    {
        /// <summary>
        /// 路徑
        /// </summary>
        public string ProjectPath { get; set; }
        /// <summary>
        /// 工程編號
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string Name { get; set; }

       /* public static implicit operator ProjectModel(ProductProjectViewModel v)
        {
            return new ProjectModel
            {
                Name = v.ProductProjectName,
                ProjectPath = v.ProductProjectPath,
                Number = v.ProductProjectNumber,
            };
        }*/
    }
}
