using DevExpress.Mvvm.Native;
using DevExpress.XtraDataLayout;
using GD_StampingMachine.Model;
using GD_StampingMachine.Model.ProductionSetting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Method
{
    public class GD_CsvHelperMethod: CsvHelperMethod
    {
        public virtual string NumberSettingFilePath
        {
            get => Path.Combine(Directory.GetCurrentDirectory(), "NumberSetting", "Normal.csv");
        }
        public virtual string QRNumberSettingFilePath
        {
            get => Path.Combine(Directory.GetCurrentDirectory(), "NumberSetting", "QR.csv");
        }

        public const string ProductProjectFileName = "Project.csv";

        /// <summary>
        /// 取得一般號碼牌設定
        /// </summary>
        /// <param name="SavedCollection"></param>
        /// <returns></returns>
        public bool ReadNumberSetting(out List<NumberSettingModel> SavedCollection)
        {
            return this.ReadCSVFileIEnumerable(NumberSettingFilePath, out SavedCollection);
        }

        /// <summary>
        /// 取得一般號碼牌設定
        /// </summary>
        /// <param name="SavedCollection"></param>
        /// <returns></returns>
        public bool ReadNumberSetting(string FilePath, out List<NumberSettingModel> SavedCollection)
        {
            return this.ReadCSVFileIEnumerable(FilePath, out SavedCollection);
        }


        /// <summary>
        /// 存取一般號碼牌設定
        /// </summary>
        /// <param name="NumberSettingModelSavedList"></param>
        /// <returns></returns>
        public bool WriteNumberSetting(string FilePath, List<NumberSettingModel> NumberSettingModelSavedList)
        {
            return this.WriteCSVFileIEnumerable<NumberSettingModel>(FilePath, NumberSettingModelSavedList);
        }
        /// <summary>
        /// 存取一般號碼牌設定
        /// </summary>
        /// <param name="NumberSettingModelSavedList"></param>
        /// <returns></returns>
        public bool WriteNumberSetting(List<NumberSettingModel> NumberSettingModelSavedList)
        {
            return this.WriteCSVFileIEnumerable<NumberSettingModel>(NumberSettingFilePath, NumberSettingModelSavedList);
        }

        /// <summary>
        /// 取得QR號碼牌設定
        /// </summary>
        /// <param name="SavedCollection"></param>
        /// <returns></returns>
        public bool ReadQRNumberSetting(out List<QRSettingModel> SavedCollection)
        {
            return this.ReadCSVFileIEnumerable(QRNumberSettingFilePath, out SavedCollection);
        }
        public bool ReadQRNumberSetting(string FilePath, out List<QRSettingModel> SavedCollection)
        {
            return this.ReadCSVFileIEnumerable(FilePath, out SavedCollection);
        }
        /// <summary>
        /// 存取QR號碼牌設定
        /// </summary>
        /// <param name="NumberSettingModelSavedList"></param>
        /// <returns></returns>
        public bool WriteQRNumberSetting(List<QRSettingModel> NumberSettingModelSavedList)
        {
            return this.WriteCSVFileIEnumerable<QRSettingModel>(QRNumberSettingFilePath, NumberSettingModelSavedList);
        }

        public bool WriteQRNumberSetting(string FilePath, List<QRSettingModel> NumberSettingModelSavedList)
        {
            return this.WriteCSVFileIEnumerable<QRSettingModel>(FilePath, NumberSettingModelSavedList);
        }


        public bool ReadProductProject(string FilePath, out ProductProjectModel ProductProject)
        {
            return this.ReadCSVFile(FilePath, out ProductProject);
        }

        public bool WriteProductProject(ProductProjectModel ProductProject)
        {
            var ProjectPath = Path.Combine(ProductProject.ProjectPath, ProductProject.Name);
            var SaveProjectPath = Path.Combine(ProjectPath, ProductProjectFileName);

            new JsonHelperMethod().WriteJsonFileIEnumerable(SaveProjectPath, ProductProject);


            this.WriteCSVFile<ProductProjectModel>(SaveProjectPath, ProductProject);
            ProductProject.PartsParameterObservableCollection.ForEach(obj =>
            {
                this.WriteCSVFile<PartsParameterModel>(SaveProjectPath, obj.PartsParameter);
            });
            return true;
        }









    }
}
