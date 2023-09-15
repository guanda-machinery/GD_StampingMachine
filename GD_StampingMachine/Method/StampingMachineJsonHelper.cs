using CsvHelper.Configuration;
using CsvHelper;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI;
using GD_StampingMachine.GD_Model;
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
using GD_CommonLibrary.Method;

namespace GD_StampingMachine.Method
{
    public class StampingMachineJsonHelper : GD_CommonLibrary.Method.JsonHelperMethod
    {

        public enum ParameterSettingNameEnum
        {
            AxisSetting,
            EngineerSetting,
            InputOutput,
            NumberSetting,
            QRSetting,
            SeparateBox,
            SeparateSetting,
            TimingSetting
        }
        public enum MachineSettingNameEnum
        {
            CommunicationSetting,
            IO_Table,
            StampingFont
        }
        public enum ProjectSettingEnum
        {
            /// <summary>
            /// 專案檔案路徑清單
            /// </summary>
            ProjectPathList,
            /// <summary>
            /// 加工專案清單
            /// </summary>
            ProjectDistributeList
        }





        /* private string MainSetting
         {
             get =>Path.Combine(Directory.GetCurrentDirectory(), ConstSettings, "MainSetting.json");
         }*/
        /* public bool ReadStampingAllData(out StampingMainModel StampingMain)
         {
             return ReadJsonFile(MainSetting ,out StampingMain);
         }
         public bool WriteStampingAllData(StampingMainModel StampingMain)
         {
             return WriteJsonFile(MainSetting, StampingMain);
         }*/






        public bool ReadParameterSettingJsonSetting<T>(ParameterSettingNameEnum ParameterSettingName, out T JsonData, bool ShowMessageBox = false)
        {
            return ReadJsonSettingByEnum(ParameterSettingName, out JsonData, ShowMessageBox);
        }
        public bool WriteParameterSettingJsonSetting<T>(ParameterSettingNameEnum ParameterSettingName, T JsonData, bool ShowMessageBox = false)
        {
            return WriteJsonSettingByEnum(ParameterSettingName, JsonData, ShowMessageBox);
        }


        public bool ReadMachineSettingJson<T>(MachineSettingNameEnum ParameterSettingName, out T JsonData, bool ShowMessageBox = false)
        {
            return ReadJsonSettingByEnum(ParameterSettingName, out JsonData, ShowMessageBox);
        }
        public bool WriteMachineSettingJson<T>(MachineSettingNameEnum ParameterSettingName, T JsonData, bool ShowMessageBox = false)
        {
            return WriteJsonSettingByEnum(ParameterSettingName, JsonData, ShowMessageBox);
        }


        public bool ReadProjectSettingJson(out List<ProjectModel> PathList, bool ShowMessageBox = false)
        {
            return ReadJsonSettingByEnum(ProjectSettingEnum.ProjectPathList, out PathList, ShowMessageBox);
        }
        public bool WriteProjectSettingJson(List<ProjectModel> JsonData, bool ShowMessageBox = false)
        {
            return WriteJsonSettingByEnum(ProjectSettingEnum.ProjectPathList, JsonData, ShowMessageBox);
        }

        /// <summary>
        /// 加工專案讀取
        /// </summary>
        /// <param name="PathList"></param>
        /// <param name="ShowMessageBox"></param>
        /// <returns></returns>
        public bool ReadProjectDistributeListJson(out List<ProjectDistributeModel> PathList, bool ShowMessageBox = false)
        {
            return ReadJsonSettingByEnum(ProjectSettingEnum.ProjectDistributeList, out PathList, ShowMessageBox);
        }
        /// <summary>
        /// 加工專案寫入
        /// </summary>
        /// <param name="JsonData"></param>
        /// <param name="ShowMessageBox"></param>
        /// <returns></returns>
        public bool WriteProjectDistributeListJson(List<ProjectDistributeModel> JsonData, bool ShowMessageBox = false)
        {
            return WriteJsonSettingByEnum(ProjectSettingEnum.ProjectDistributeList, JsonData, ShowMessageBox);
        }

        /// <summary>
        /// 取得加工專案 並將更改路徑和檔名對應到正確的位置
        /// </summary>
        /// <param name="ProductProject"></param>
        /// <returns></returns>
        public bool ManualReadProductProject(out ProductProjectModel ProductProject)
        {
            var Result = ManualReadJsonFile(out ProductProject, out string FilePath);
            if (Result)
            {
                ProductProject.Name = Path.GetFileNameWithoutExtension(FilePath);
                ProductProject.ProjectPath = Path.GetDirectoryName(FilePath);
            }
            return Result;
        }


        public string GetCurrentSettingsDirectory(string FilePath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), ConstSettings, FilePath);
        }

        private const string ConstSettings = "Settings";
        private const string ConstMachineSettings = "MachineSettings";

        private const string ConstNumberSetting = "Numbers";
        private const string ConstParameterSetting = "ParameterSetting";

        public bool ReadJsonSettingByEnum<T>(Enum ParameterSettingName , out T JsonData, bool ShowMessageBox = false)
        {
            var FilePath = GetJsonFilePath(ParameterSettingName);

            var Result = this.ReadJsonFile(FilePath, out JsonData);
            if(ShowMessageBox)
                MethodWinUIMessageBox.LoadSuccessful(FilePath, Result);
            return Result;
        }
        public bool WriteJsonSettingByEnum<T>(Enum ParameterSettingName, T JsonData, bool ShowMessageBox = false)
        {
            var FilePath = GetJsonFilePath(ParameterSettingName);

            var Result = this.WriteJsonFile(FilePath, JsonData); 
            if (ShowMessageBox)
                MethodWinUIMessageBox.SaveSuccessful(FilePath , Result);
            return Result;
        }

        private static string GetJsonFilePath(Enum ParameterSettingName)
        {
            var FilePath = "";// Path.Combine();

            if (ParameterSettingName is ParameterSettingNameEnum)
            {
                FilePath = Path.Combine(FilePath, ConstParameterSetting);
                if (ParameterSettingName is ParameterSettingNameEnum.NumberSetting ||
                    ParameterSettingName is ParameterSettingNameEnum.QRSetting)
                {
                    FilePath = Path.Combine(FilePath, ConstNumberSetting);
                }
            }


            var FileName = Path.ChangeExtension(ParameterSettingName.ToString(), "json");

            return Path.Combine(Directory.GetCurrentDirectory(), ConstSettings ,FilePath, FileName);
        }

        

    }
}
