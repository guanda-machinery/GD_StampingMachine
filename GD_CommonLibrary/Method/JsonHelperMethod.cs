using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using DevExpress.XtraSpreadsheet.Model;
using Newtonsoft.Json;
using Application = System.Windows.Application;

namespace GD_CommonLibrary.Method
{
    public class JsonHelperMethod
    {
        public bool WriteJsonFile<T>(string fileName, T JsonData)
        {
            try
            {
                //先檢查副檔名
                if (!Path.HasExtension(fileName) || Path.GetExtension(fileName).ToLower() != "json")
                {
                    fileName = Path.ChangeExtension(fileName, "json");
                }

                if (string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
                {
                    //如果檔案名不包含根目錄 幫他建在工作目錄下
                    fileName =Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(fileName));
                }

                //建立資料夾
                if (!Directory.Exists(fileName))
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                if (JsonData == null)
                    throw new ArgumentNullException();

                string output = JsonConvert.SerializeObject(JsonData);
                //檢查檔案是否存在於該目錄 若存在則將先將檔案寫入temp
                if (File.Exists(fileName))
                {
                    var Temp_fileName =  fileName+".tmp";

                    File.WriteAllText(Temp_fileName, output);
                    File.Delete(fileName);
                    File.Move(Temp_fileName, fileName);
                }
                else
                {
                    File.WriteAllText(fileName, output);
                }
                return true;
            }
            catch (Exception ex)
            {
                 _ =  MessageBoxResultShow.ShowException(ex);
                Debugger.Break();
                return false;
            }
        }
        public bool ReadJsonFile<T>(string fileName, out T JsonData)
        {
            JsonData = default(T);
            if (!Path.HasExtension(fileName) || Path.GetExtension(fileName).ToLower() != "json")
            {
                fileName = Path.ChangeExtension(fileName, "json");
            }

            if (string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
            {
                //如果檔案名不包含根目錄 幫他建在工作目錄下
                fileName = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(fileName));
            }

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                return false;
            
            if (!File.Exists(fileName))
                return false;

            try
            {
                StreamReader r = new StreamReader(fileName);
                string jsonString = r.ReadToEnd();
                JsonData = JsonConvert.DeserializeObject<T>(jsonString);

                return JsonData != null;
            }
            catch (JsonException JEX) 
            {
                MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"), (string)Application.Current.TryFindResource("Text_JsonError"));
                Debugger.Break();
            }
            catch (Exception ex)
            {
             _=   MessageBoxResultShow.ShowException(ex);
                Debugger.Break();
            }
            return false;
        }


        public bool ManualReadJsonFile<T>(out T JsonData)
        {
            return ManualReadJsonFile(out JsonData, out _);
        }

        public bool ManualReadJsonFile<T>(out T JsonData , out string FilePath)
        {
            FilePath = null;
            JsonData = default;

            OpenFileDialog sfd = new()
            {
                Filter = "Json files (*.json)|*.json"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if(ReadJsonFile(sfd.FileName, out JsonData))
                    {
                        FilePath = sfd.FileName;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                 _ =   MessageBoxResultShow.ShowException(ex);
                }
                return false;
            }
            return false;
        }

        public bool ManualWriteJsonFile<T>(T JsonData)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            { 
                Filter = "Json files (*.json)|*.json"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    WriteJsonFile(sfd.FileName, JsonData);
                    return true;
                }
                catch (Exception ex)
                {
                    _ = MessageBoxResultShow.ShowException(ex);
                }
                return false;
            }
            return false;
        }


    }
}
