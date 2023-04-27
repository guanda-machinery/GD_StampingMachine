using CsvHelper.Configuration;
using CsvHelper;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI;
using GD_StampingMachine.Model;
using GD_StampingMachine.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GD_StampingMachine.Method
{
    public class GD_JsonHelperMethod: JsonHelperMethod
    {
        public virtual string NumberSettingFilePath
        {
            get => Path.Combine(Directory.GetCurrentDirectory(), "NumberSetting", "Normal.json");
        }
        public virtual string QRNumberSettingFilePath
        {
            get => Path.Combine(Directory.GetCurrentDirectory(), "NumberSetting", "QR.json");
        }
        public virtual string MachiningSettingFilePath
        {
            get => Path.Combine(Directory.GetCurrentDirectory(), "MachiningSettingFilePath");
        }

        public virtual string StampingFontChangedFilePath
        {
            get => Path.Combine(MachiningSettingFilePath, "StampingFontChanged.json");
        }

        public bool ReadStampingFontChanged(out StampingFontChangedViewModel StampingFontChangedVM)
        {
            return this.ReadJsonFile(StampingFontChangedFilePath, out StampingFontChangedVM);
        }
        public bool WriteStampingFontChanged(StampingFontChangedViewModel StampingFontChangedVM)
        {
            return this.WriteJsonFile(StampingFontChangedFilePath, StampingFontChangedVM);
        }





        /// <summary>
        /// 存取一般號碼牌設定
        /// </summary>
        /// <param name="NumberSettingModelSavedList"></param>
        /// <returns></returns>
        public bool ReadNumberSetting(out ObservableCollection<NumberSettingModel> NumberSetting)
        {
            return this.ReadJsonFile(QRNumberSettingFilePath, out NumberSetting);
        }
        public bool WriteNumberSetting(ObservableCollection<NumberSettingModel> NumberSetting)
        {
            return this.WriteJsonFile(QRNumberSettingFilePath, NumberSetting);
        }

        /// <summary>
        /// 取得QR號碼牌設定
        /// </summary>
        /// <param name="SavedCollection"></param>
        /// <returns></returns>
        public bool ReadQRNumberSetting(out ObservableCollection<QRSettingModel> QRNumberSetting)
        {
            return this.ReadJsonFile(QRNumberSettingFilePath, out QRNumberSetting);
        }
        public bool WriteQRNumberSetting(ObservableCollection<QRSettingModel> QRSettingModelCollection)
        {
            return this.WriteJsonFile(QRNumberSettingFilePath, QRSettingModelCollection);
        }

        public bool WriteProductProject(ProductProjectModel ProductProject)
        {
            var ProjectPath = Path.Combine(ProductProject.ProjectPath, ProductProject.Name);

            if (!Path.HasExtension(ProjectPath))
            {
                ProjectPath = Path.ChangeExtension(ProjectPath, "csv");
            }

            return this.WriteJsonFile(ProjectPath, ProductProject);
        }
        public bool ReadProductProject(string FilePath ,out ProductProjectModel ProductProject)
        {
            return this.ReadJsonFile(FilePath,out ProductProject);
        }




















    }
}
