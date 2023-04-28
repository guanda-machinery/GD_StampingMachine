﻿using CsvHelper.Configuration;
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
    public class GD_JsonHelperMethod : GD_CommonLibrary.Method.JsonHelperMethod
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
            StampingFont
        }
        public enum ProjectSettingEnum
        {
            ProjectPathList
        }



        public bool ReadParameterSettingJsonSetting<T>(ParameterSettingNameEnum ParameterSettingName, out T JsonData, bool ShowMessageBox = false)
        {
            return this.ReadJsonSettingByEnum(ParameterSettingName, out JsonData, ShowMessageBox);
        }
        public bool WriteParameterSettingJsonSetting<T>(ParameterSettingNameEnum ParameterSettingName, T JsonData, bool ShowMessageBox = false)
        {
           return this.WriteJsonSettingByEnum(ParameterSettingName, JsonData, ShowMessageBox);
        }


        public bool ReadMachineSettingJson<T>(MachineSettingNameEnum ParameterSettingName, out T JsonData, bool ShowMessageBox = false)
        {
            return this.ReadJsonSettingByEnum(ParameterSettingName, out JsonData, ShowMessageBox);
        }
        public bool WriteMachineSettingJson<T>(MachineSettingNameEnum ParameterSettingName, T JsonData, bool ShowMessageBox = false)
        {
            return this.WriteJsonSettingByEnum(ParameterSettingName, JsonData, ShowMessageBox);
        }


        public bool ReadProjectSettingJson<T>(ProjectSettingEnum ParameterSettingName, out T JsonData, bool ShowMessageBox = false)
        {
            return this.ReadJsonSettingByEnum(ParameterSettingName, out JsonData, ShowMessageBox);
        }
        public bool WriteProjectSettingJson<T>(ProjectSettingEnum ParameterSettingName, T JsonData, bool ShowMessageBox = false)
        {
            return this.WriteJsonSettingByEnum(ParameterSettingName, JsonData, ShowMessageBox);
        }


        private const string ConstSettings = "Settings";

        private const string ConstNumberSetting = "NumberSetting";
        private const string ConstParameterSetting = "ParameterSetting";

        public bool ReadJsonSettingByEnum<T>(Enum ParameterSettingName , out T JsonData, bool ShowMessageBox = false)
        {
            var FilePath = Path.Combine( Directory.GetCurrentDirectory(), ConstSettings);

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
            FilePath = Path.Combine(FilePath, FileName);

            var Result = this.ReadJsonFile(FilePath, out JsonData);
            if(ShowMessageBox)
                MethodWinUIMessageBox.LoadSuccessful(FilePath, Result);
            return Result;
        }
        public bool WriteJsonSettingByEnum<T>(Enum ParameterSettingName, T JsonData, bool ShowMessageBox = false)
        {
            var FilePath = Directory.GetCurrentDirectory();
            if (ParameterSettingName is ParameterSettingNameEnum.NumberSetting ||
                ParameterSettingName is ParameterSettingNameEnum.QRSetting)
            {
                FilePath = Path.Combine(FilePath, ConstNumberSetting);
            }

            var FileName = Path.ChangeExtension(ParameterSettingName.ToString(), "json");
            FilePath = Path.Combine(FilePath, FileName);

            var Result = this.WriteJsonFile(FilePath, JsonData); 
            if (ShowMessageBox)
                MethodWinUIMessageBox.SaveSuccessful(FilePath , Result);
            return Result;
        }

    }
}