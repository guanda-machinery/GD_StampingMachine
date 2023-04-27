using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper;
using System.Windows;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Xpf.WindowsUI;
using Newtonsoft.Json;
using System.Windows.Forms;
using DevExpress.Pdf.Native.DocumentSigning;

namespace GD_StampingMachine.Method
{
    public class JsonHelperMethod
    {
        protected bool WriteJsonFile<T>(string fileName, T JsonData)
        {
            try
            {
                //先檢查副檔名
                if (!Path.HasExtension(fileName) || Path.GetExtension(fileName).ToLower() != "json")
                {
                    fileName = Path.ChangeExtension(fileName, "json");
                }
                //建立資料夾
                if (!Directory.Exists(fileName))
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                string output = JsonConvert.SerializeObject(JsonData);
                File.WriteAllText(fileName, output);
                return true;
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return false;
            }
        }
        protected bool ReadJsonFile<T>(string fileName, out T JsonData)
        {
            JsonData = default(T);
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                return false;
            
            if (!File.Exists(fileName))
                return false;

            try
            {
                StreamReader r = new StreamReader(fileName);
                string jsonString = r.ReadToEnd();
                JsonData = JsonConvert.DeserializeObject<T>(jsonString);
                return true;
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return false;
            }
        }


        private static readonly OpenFileDialog sfd = new()
        {
            Filter = "Json files (*.json)|*.json"
        };

        public bool ManualReadJsonFile<T>(out T JsonData)
        {
            JsonData=default;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ReadJsonFile(sfd.FileName, out JsonData);
                    return true;
                }
                catch(Exception ex)
                {
                    MethodWinUIMessageBox.ShowException(ex);
                }
                return false;
            }
            return false;
        }

        public bool ManualWriteJsonFile<T>(T JsonData)
        {
            JsonData = default;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    WriteJsonFile(sfd.FileName, JsonData);
                    return true;
                }
                catch (Exception ex)
                {
                    MethodWinUIMessageBox.ShowException(ex);
                }
                return false;
            }
            return false;
        }


    }
}
