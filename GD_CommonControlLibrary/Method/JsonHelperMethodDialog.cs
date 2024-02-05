using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GD_CommonControlLibrary.Method
{
    public class JsonHelperMethodDialog: GD_CommonLibrary.Method.JsonHelperMethod
    {
        public bool ManualReadJsonFile<T>(string fileName, out T JsonData, out string FilePath)
        {
            FilePath = null;
            JsonData = default;

            OpenFileDialog sfd = new()
            {
                FileName = fileName,
                Filter = "Json files (*.json)|*.json"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (ReadJsonFile(sfd.FileName, out JsonData))
                    {
                        FilePath = sfd.FileName;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    // _ = MessageBoxResultShow.ShowExceptionAsync(ex);
                }
                return false;
            }
            return false;
        }

        public async Task<bool> ManualWriteJsonFile<T>(T JsonData)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Json files (*.json)|*.json"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    return await WriteJsonFileAsync(sfd.FileName, JsonData);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    //  _ = MessageBoxResultShow.ShowExceptionAsync(ex);
                }
                return false;
            }
            return false;
        }

        public bool ManualReadJsonFile<T>(out T JsonData)
        {
            return ManualReadJsonFile(string.Empty, out JsonData, out _);
        }

        public bool ManualReadJsonFile<T>(string fileName, out T JsonData)
        {
            return ManualReadJsonFile(fileName, out JsonData, out _);
        }


    }
}
