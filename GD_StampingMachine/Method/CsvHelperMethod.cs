using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DevExpress.XtraRichEdit.Model;
using GD_StampingMachine.Model;
using DevExpress.Mvvm.Native;
using DevExpress.XtraGrid.Registrator;

namespace GD_StampingMachine.Method
{
    public class CsvHelperMethod
    {
        /// <summary>
        /// 複寫檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="CSVData"></param>
        /// <returns></returns>
        public bool WriteCSVFile<T>(string fileName, object CSVData)
        {
            //先建立資料夾
            if(!Directory.Exists(fileName))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            var CSVDataIEnumerable = new List<object>() { CSVData };
            return WriteCSVFileIEnumerable(fileName, CSVDataIEnumerable);
        }
        /// <summary>
        /// 複寫檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="CSVData"></param>
        /// <returns></returns>
        public bool WriteCSVFileIEnumerable<T>(string fileName , List<T> CSVDataIEnumerable)
        {
            try
            {
                var writeConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true
                };
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                using (var writer = new StreamWriter(fileName, false))
                using (var csv = new CsvWriter(writer, writeConfiguration))
                {
                    csv.WriteRecords(CSVDataIEnumerable);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return false;
            }
        }



        /// <summary>
        /// 讀取檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="CSVData"></param>
        /// <returns></returns>
        public bool ReadCSVFile<T>(string fileName, out T CSVData)
        {
            CSVData = default(T);
           if(ReadCSVFileIEnumerable(fileName, out List<T> CSVDataIEnumerable))
           {
                if(CSVDataIEnumerable.Count()!=0)
                {
                    CSVData = CSVDataIEnumerable.FirstOrDefault();
                    return true;
                }
           }
            return false;

        }

        /// <summary>
        /// 讀取檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="CSVData"></param>
        /// <returns></returns>
        public bool ReadCSVFileIEnumerable<T>(string fileName , out List<T> CSVDataIEnumerable)
        {
            CSVDataIEnumerable = new List<T>();

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                return false;


            try
            {
                var readConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true
                };

                using (var reader = new StreamReader(fileName))
                using (var csv = new CsvReader(reader, readConfiguration))
                {
                    CSVDataIEnumerable = csv.GetRecords<T>().ToList();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return false;
            }
        }







    }


    public class GD_CsvHelperMethod
    {
        public virtual string NumberSettingFilePath
        {
            get => Path.Combine(Directory.GetCurrentDirectory(), "NumberSetting", "Normal.csv");
        }
        public virtual string QRNumberSettingFilePath
        {
            get => Path.Combine(Directory.GetCurrentDirectory(), "NumberSetting", "QR.csv");
        }


        private CsvHelperMethod CsvHM = new CsvHelperMethod();

        /// <summary>
        /// 取得一般號碼牌設定
        /// </summary>
        /// <param name="SavedCollection"></param>
        /// <returns></returns>
        public bool ReadNumberSetting(out List<NumberSettingModel> SavedCollection)
        {
            return CsvHM.ReadCSVFileIEnumerable(NumberSettingFilePath, out SavedCollection);
        }

        /// <summary>
        /// 取得一般號碼牌設定
        /// </summary>
        /// <param name="SavedCollection"></param>
        /// <returns></returns>
        public bool ReadNumberSetting(string FilePath , out List<NumberSettingModel> SavedCollection)
        {
            return CsvHM.ReadCSVFileIEnumerable(FilePath, out SavedCollection);
        }


        /// <summary>
        /// 存取一般號碼牌設定
        /// </summary>
        /// <param name="NumberSettingModelSavedList"></param>
        /// <returns></returns>
        public bool WriteNumberSetting(string FilePath , List<NumberSettingModel> NumberSettingModelSavedList)
        {
            return CsvHM.WriteCSVFileIEnumerable<NumberSettingModel>(FilePath, NumberSettingModelSavedList);
        }
        /// <summary>
        /// 存取一般號碼牌設定
        /// </summary>
        /// <param name="NumberSettingModelSavedList"></param>
        /// <returns></returns>
        public bool WriteNumberSetting(List<NumberSettingModel> NumberSettingModelSavedList)
        {
            return CsvHM.WriteCSVFileIEnumerable<NumberSettingModel>(NumberSettingFilePath, NumberSettingModelSavedList);
        }

        /// <summary>
        /// 取得QR號碼牌設定
        /// </summary>
        /// <param name="SavedCollection"></param>
        /// <returns></returns>
        public bool ReadQRNumberSetting(out List<QRSettingModel> SavedCollection)
        {
            return CsvHM.ReadCSVFileIEnumerable(QRNumberSettingFilePath, out SavedCollection);
        }
        public bool ReadQRNumberSetting(string FilePath, out List<QRSettingModel> SavedCollection)
        {
            return CsvHM.ReadCSVFileIEnumerable(FilePath, out SavedCollection);
        }
        /// <summary>
        /// 存取QR號碼牌設定
        /// </summary>
        /// <param name="NumberSettingModelSavedList"></param>
        /// <returns></returns>
        public bool WriteQRNumberSetting(List<QRSettingModel> NumberSettingModelSavedList)
        {
            return CsvHM.WriteCSVFileIEnumerable<QRSettingModel>(QRNumberSettingFilePath, NumberSettingModelSavedList);
        }

        public bool WriteQRNumberSetting(string FilePath, List<QRSettingModel> NumberSettingModelSavedList)
        {
            return CsvHM.WriteCSVFileIEnumerable<QRSettingModel>(FilePath, NumberSettingModelSavedList);
        }


        public bool ReadProductProject(string FilePath ,out ProductProjectModel ProductProject)
        {
            return CsvHM.ReadCSVFile(FilePath, out ProductProject);
        }

        public bool WriteProductProject(string FilePath  , ProductProjectModel ProductProject)
        {
            return CsvHM.WriteCSVFile<ProductProjectModel>(FilePath, ProductProject);
        }

    }



}



