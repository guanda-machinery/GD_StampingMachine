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
using System.Security.Policy;

namespace GD_StampingMachine.Method
{
    public class CsvFileManager
    {
        /// <summary>
        /// 複寫檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="CSVData"></param>
        /// <returns></returns>
        public bool WriteCSVFile<T>(string fileName, T CSVData, bool isAppend)
        {
            var CSVDataIEnumerable = new List<T>() { CSVData };
            return WriteCSVFileIEnumerable(fileName, CSVDataIEnumerable , true);
        }
        /// <summary>
        /// 寫入檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="CSVData"></param>
        /// <returns></returns>
        public bool WriteCSVFileIEnumerable<T>(string fileName , List<T> CSVDataIEnumerable , bool isAppend=false)
        {
            try
            {
                //先檢查副檔名

                if (!Path.HasExtension(fileName) || !Path.GetExtension(fileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = Path.ChangeExtension(fileName, "csv");
                }

                //建立資料夾
                if (!Directory.Exists(fileName))
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                var writeConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true
                };
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                bool append = false;
                if (isAppend)
                {
                    if (File.Exists(fileName))
                    {
                        writeConfiguration.HasHeaderRecord = false;
                        append = true;
                    }
                }


                using (var writer = new StreamWriter(fileName, append))
                using (var csv = new CsvWriter(writer, writeConfiguration))
                {
                    csv.WriteRecords(CSVDataIEnumerable);
                    return true;
                }
            }
            catch (IOException ex)
            {
                Debugger.Break();
                return false;
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return false;
            }
        }

        /// <summary>
        /// 讀取第一行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="CSVData"></param>
        /// <returns></returns>
        /*public bool ReadCSVFile<T>(string fileName, out T CSVData)
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
        }*/





        /// <summary>
        /// 讀取檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="CSVData"></param>
        /// <returns></returns>
        public bool ReadCSVFile<T>(string fileName , out List<T> CSVDataIEnumerable, bool hasHeaderRecord = true )
        {
            CSVDataIEnumerable = new List<T>();

            //先檢查副檔名
            if (!Path.HasExtension(fileName) || !Path.GetExtension(fileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                fileName = Path.ChangeExtension(fileName, "csv");
            }

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                return false;

            try
            {
                var readConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = hasHeaderRecord,
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
                Console.WriteLine(ex.ToString());
                return false;
            }
        }










    }
}



