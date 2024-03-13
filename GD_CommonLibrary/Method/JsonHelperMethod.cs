using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace GD_CommonLibrary.Method
{
    public class JsonHelperMethod
    {
        public async Task<bool> WriteJsonFileAsync<T>(string fileName, T JsonData)
        {
            return await Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Yield();
                    var Wret = WriteJsonFile(fileName, JsonData);
                    if (Wret)
                    {
                        return true;
                    }
                }
                return false;
            });
        }


        public bool WriteJsonFile<T>(string fileName, T JsonData)
        {
            //先檢查副檔名
            if (!Path.HasExtension(fileName) || !Path.GetExtension(fileName).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                fileName = Path.ChangeExtension(fileName, "json");
            }

            if (string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
            {
                //如果檔案名不包含根目錄 幫他建在工作目錄下
                fileName = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(fileName));
            }

            //建立資料夾
            if (!Directory.Exists(fileName))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            if (JsonData == null)
                throw new ArgumentNullException();

            string output = JsonConvert.SerializeObject(JsonData);

            //檢查檔案是否存在於該目錄 若存在則將先將檔案寫入temp

            var wirteRet = false;

            try
            {
                if (File.Exists(fileName))
                {
                    var Temp_fileName = fileName + ".tmp";

                    File.WriteAllText(Temp_fileName, output);

                    File.Delete(fileName);
                    File.Copy(Temp_fileName, fileName);
                    if (File.Exists(fileName))
                        File.Delete(Temp_fileName);
                    //File.Move(Temp_fileName, fileName);
                }
                else
                {
                    File.WriteAllText(fileName, output);
                }
                wirteRet = true;
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex);
            }

            //檢查是否有成功寫入
            var readRet = ReadJsonFile<T>(fileName, out _);
            if (!readRet)
            {
                Debugger.Break();
            }
            return wirteRet && readRet;
        }


        /// <summary>
        /// 讀取json 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="JsonData"></param>
        /// <param name="showMessageBox"></param>
        /// <returns></returns>
        /*public bool ReadJsonFileWithoutMessageBox<T>(string fileName, out T JsonData)
        {
            return ReadJsonFile(fileName, out JsonData, false);
        }*/

        /// <summary>
        /// 讀取json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="JsonData"></param>
        /// <param name="showMessageBox"></param>
        /// <returns></returns>
        /*public bool ReadJsonFile<T>(string fileName, out T JsonData)
        {
            return readJson(fileName, out JsonData, true); ;
        }*/
        /// <summary>
        /// 讀取json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="JsonData"></param>
        /// <returns></returns>
        public bool ReadJsonFile<T>(string fileName, out T JsonData )
        {
            JsonData = default(T);
            if (!Path.HasExtension(fileName) || !Path.GetExtension(fileName).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                fileName = Path.ChangeExtension(fileName, "json");
            }

            if (string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
            {
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
                Debug.WriteLine(JEX);
                /* if(showMessageBox)
                      _ = MessageBoxResultShow.ShowOKAsync(null,(string)Application.Current.TryFindResource("Text_notify"),
                          fileName + "\r\n"+(string)Application.Current.TryFindResource("Text_JsonError"));*/
                //Debugger.Break();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                // if (showMessageBox)
                //    _ =   MessageBoxResultShow.ShowExceptionAsync(null,ex);
                //Debugger.Break();
            }
            return false;
        }










    }
}
