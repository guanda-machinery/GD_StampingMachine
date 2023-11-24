using CommunityToolkit.Mvvm.Input;
using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Utils.Design;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Scheduling.Themes;
using DevExpress.XtraPrinting.Native.Extensions;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_MachineConnect;
using GD_MachineConnect.Machine;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels;
using GD_StampingMachine.ViewModels.MachineMonitor;
using GD_StampingMachine.ViewModels.ParameterSetting;
using GD_StampingMachine.ViewModels.ProductSetting;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using Opc.Ua.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using static GD_MachineConnect.GD_Stamping_Opcua.StampingOpcUANode;
using static GD_StampingMachine.Method.StampingMachineJsonHelper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GD_StampingMachine.Singletons
{
    public class StampMachineDataSingleton : GD_CommonLibrary.BaseSingleton<StampMachineDataSingleton>
    {
        //  public const string DataSingletonName = "Name_StampMachineDataSingleton";
        public string DataSingletonName => (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorIsNotAcitved");

        private GD_Stamping_Opcua GD_Stamping = new GD_Stamping_Opcua();


        protected override void Init()
        {
            var JsonHM = new StampingMachineJsonHelper();
            if (JsonHM.ReadMachineSettingJson(StampingMachineJsonHelper.MachineSettingNameEnum.CommunicationSetting, out CommunicationSettingModel CSettingJson))
            {
                CommunicationSetting = CSettingJson;
            }
            else
            {
                //如果沒找到 產出一個新檔案後寫入
                CommunicationSetting = new CommunicationSettingModel()
                {
                    MachineName = "GuandaStamping",
                    HostString = @"192.168.1.123",
                    Port = 4842,
                    ServerDataPath = null,
                    UserName = "Administrator",
                    Password = "pass",
                    Protocol = CommunicationProtocolEnum.Opcua,
                };
                JsonHM.WriteMachineSettingJson(StampingMachineJsonHelper.MachineSettingNameEnum.CommunicationSetting, CommunicationSetting);
            }

            if (JsonHM.ReadJsonSettingByEnum(MachineSettingNameEnum.IO_Table, out List<IO_InfoModel> io_info_TableJson))
            {
                IO_TableObservableCollection = new ObservableCollection<IO_InfoViewModel>(
                    io_info_TableJson.Select(model => new IO_InfoViewModel(model)));
            }
            else
            {
                string opcuaNodeHeader = GD_Stamping_Opcua.StampingOpcUANode.NodeHeader;
                //讀取失敗->建立新的io表
                var io_info_Table = new List<IO_InfoModel>
                {
                    new IO_InfoModel()
                    {
                        Info = "氣壓總壓檢知(7kg/cm3 up)",
                        BondingCableTerminal = "I00",
                        KEBA_Definition = "DI_00",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_AirPressNotEnough",
                    },
                    new IO_InfoModel()
                    {
                        Info = "預留",
                        BondingCableTerminal = "I01",
                        KEBA_Definition = "DI_01",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                    },
                    new IO_InfoModel()
                    {
                        Info = "油壓單元液位檢知(低液位)",
                        BondingCableTerminal = "I02",
                        KEBA_Definition = "DI_02",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.OilMaintenance1.di_OilLevelOk"
                    },
                    new IO_InfoModel()
                    {
                        Info = "潤滑壓力檢知",
                        BondingCableTerminal = "I03",
                        KEBA_Definition = "DI_03",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Lubrication1.di_LubPressureAchieved"
                    },
                    new IO_InfoModel()
                    {
                        Info = "潤滑液位檢知(低液位)",
                        BondingCableTerminal = "I04",
                        KEBA_Definition = "DI_04",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Lubrication1.di_LubLimitAchieved"
                    },
                    new IO_InfoModel()
                    {
                        Info = "放料卷異常",
                        BondingCableTerminal = "I05",
                        KEBA_Definition = "DI_05",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_FeedRollsAbnormal"
                    },
                    new IO_InfoModel()
                    {
                        Info ="進料有無料件確認檢知" ,
                        BondingCableTerminal = "I06",
                        KEBA_Definition = "DI_06",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Feeding1.di_FeedMaterialConfirm"
                    },
                    new IO_InfoModel()
                    {
                        Info = "QRcode壓座組1_氣壓缸上限檢知",
                        BondingCableTerminal = "I07",
                        KEBA_Definition = "DI_07",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture1.di_StopUp"
                    },
                    new IO_InfoModel()
                    {
                        Info = "QRcode壓座組1_氣壓缸下限檢知",
                        BondingCableTerminal = "I08",
                        KEBA_Definition = "DI_08",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture1.di_StopDown"
                    },
                    new IO_InfoModel()
                    {
                        Info ="阻擋缸_氣壓缸上限檢知" ,
                        BondingCableTerminal = "I09",
                        KEBA_Definition = "DI_09",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture1.di_StopUp"
                    },
                    new IO_InfoModel()
                    {
                        Info ="阻擋缸_氣壓缸下限檢知" ,
                        BondingCableTerminal = "I10",
                        KEBA_Definition = "DI_10",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture1.di_StopDown"
                    },
                    new IO_InfoModel()
                    {
                        Info = "料品到QR code 位置檢知",
                        BondingCableTerminal = "I11",
                        KEBA_Definition = "DI_11",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.QRCode1.di_QRCodeMaterialConfirm"
                    },
                    new IO_InfoModel()
                    {
                        Info = "打字輪_Y軸行程 + 極限檢知 0 位置",
                        BondingCableTerminal = "I12",
                        KEBA_Definition = "DI_12",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.EngravingFeeding1.di_ServoHome"
                    },
                    new IO_InfoModel()
                    {
                        Info = "打字輪_Y軸行程 - 極限檢知 260 位置",
                        BondingCableTerminal = "I13",
                        KEBA_Definition = "DI_13",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.EngravingFeeding1.di_ServoPOT"
                    },
                    new IO_InfoModel()
                    {
                        Info = "預留",
                        BondingCableTerminal = "I14",
                        KEBA_Definition = "DI_14",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info ="預留" ,
                        BondingCableTerminal = "I15",
                        KEBA_Definition = "DI_15",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "料品到字碼刻印位置檢知",
                        BondingCableTerminal = "I16",
                        KEBA_Definition = "DI_16",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Engraving1.di_EngravingMaterialConfirm"
                    },

                    new IO_InfoModel()
                    {
                        Info = "進料X軸_原點",
                        BondingCableTerminal = "I17",
                        KEBA_Definition = "DI_17",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Feeding1.di_ServoHome"
                    },
                    new IO_InfoModel()
                    {
                        Info ="進料X軸_負極限" ,
                        BondingCableTerminal = "I18",
                        KEBA_Definition = "DI_18",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Feeding1.di_ServoNOT"
                    },

                    new IO_InfoModel()
                    {
                        Info = "刻印壓座組2_氣壓缸上限檢知",
                        BondingCableTerminal = "I00",
                        KEBA_Definition = "DI_100",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture2.di_StopUp"
                    },
                    new IO_InfoModel()
                    {
                        Info = "刻印壓座組2_氣壓缸下限檢知",
                        BondingCableTerminal = "I01",
                        KEBA_Definition = "DI_101",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture2.di_StopDown"
                    },
                    new IO_InfoModel()
                    {
                        Info ="字碼刻印組_Z軸油壓缸刻印位置" ,
                        BondingCableTerminal = "I02",
                        KEBA_Definition = "DI_102",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Engraving1.di_StopDown"
                    },
                    new IO_InfoModel()
                    {
                        Info = "字碼刻印組_Z軸油壓缸原點位置",
                        BondingCableTerminal = "I03",
                        KEBA_Definition = "DI_103",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Engraving1.di_StopUp"
                    },
                    new IO_InfoModel()
                    {
                        Info ="字碼刻印組_刻印轉輪原點位置" ,
                        BondingCableTerminal = "I04",
                        KEBA_Definition = "DI_104",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.EngravingRotate1.di_ServoHome"
                    },
                    new IO_InfoModel()
                    {
                        Info = "進料導桿缸1_氣壓缸上限檢知",
                        BondingCableTerminal = "I05",
                        KEBA_Definition = "DI_105",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.GuideRodsFixture1.di_StopUp"
                    },
                    new IO_InfoModel()
                    {
                        Info = "進料導桿缸1_氣壓缸下限檢知",
                        BondingCableTerminal = "I06",
                        KEBA_Definition = "DI_106",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.GuideRodsFixture1.di_StopDown"
                    },
                    new IO_InfoModel()
                    {
                        Info ="料品到裁切位置檢知" ,
                        BondingCableTerminal = "I07",
                        KEBA_Definition = "DI_107",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Cutting1.di_CuttingMaterialConfirm"
                    },
                    new IO_InfoModel()
                    {
                        Info = "裁切模組_位置檢知_上",
                        BondingCableTerminal = "I08",
                        KEBA_Definition = "DI_108",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Cutting1.di_CuttingOrigin"
                    },
                    new IO_InfoModel()
                    {
                        Info = "裁切模組_位置檢知_中",
                        BondingCableTerminal = "I09",
                        KEBA_Definition = "DI_109",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Cutting1.di_CuttingStandbyPoint"
                    },
                    new IO_InfoModel()
                    {
                        Info = "裁切模組_位置檢知_下",
                        BondingCableTerminal = "I10",
                        KEBA_Definition = "DI_110",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Cutting1.di_CuttingCutPoint"
                    },
                    new IO_InfoModel()
                    {
                        Info = "阻擋缸_進退動作-SW",
                        BondingCableTerminal = "I11",
                        KEBA_Definition = "DI_111",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "緊急停止-SW",
                        BondingCableTerminal = "I12",
                        KEBA_Definition = "DI_112",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.OperationMode1.di_EmergencyStop1"
                    },
                    new IO_InfoModel()
                    {
                        Info ="開機(power on)-SW" ,
                        BondingCableTerminal = "I13",
                        KEBA_Definition = "DI_113",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_PowerON"
                    },
                    new IO_InfoModel()
                    {
                        Info ="暫停(pause)-SW" ,
                        BondingCableTerminal = "I14",
                        KEBA_Definition = "DI_114",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_Pause"
                    },

                    new IO_InfoModel()
                    {
                        Info = "開始加工(start)-SW",
                        BondingCableTerminal = "I15",
                        KEBA_Definition = "DI_115",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_Start"
                    },

                    new IO_InfoModel()
                    {
                        Info ="放料卷方向" ,
                        BondingCableTerminal = "I16",
                        KEBA_Definition = "DI_116",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_FeedRollsDirection"
                    },
                    new IO_InfoModel()
                    {
                        Info = "原點復歸-SW",
                        BondingCableTerminal = "I17",
                        KEBA_Definition = "DI_117",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_Home"
                    },
                    new IO_InfoModel()
                    {
                        Info ="半自動-SW" ,
                        BondingCableTerminal = "I18",
                        KEBA_Definition = "DI_118",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.OperationMode1.di_ButtonHalfAuto"
                    },

                    new IO_InfoModel()
                    {
                        Info = "全自動",
                        BondingCableTerminal = "I00",
                        KEBA_Definition = "DI_200",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.OperationMode1.di_ButtonFullAuto"
                    },
                    new IO_InfoModel()
                    {
                        Info = "預留",
                        BondingCableTerminal = "I01",
                        KEBA_Definition = "DI_201",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "進料導桿缸2_氣壓缸上限檢知",
                        BondingCableTerminal = "I02",
                        KEBA_Definition = "DI_202",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.GuideRodsFixture2.di_StopUp"
                    },
                    new IO_InfoModel()
                    {
                        Info = "進料導桿缸2_氣壓缸下限檢知",
                        BondingCableTerminal = "I03",
                        KEBA_Definition = "DI_203",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.GuideRodsFixture2.di_StopDown"
                    },

                    new IO_InfoModel()
                    {
                        Info = "鑰匙開關_自動",
                        BondingCableTerminal = "I04",
                        KEBA_Definition = "DI_204",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "鑰匙開關_鎖固",
                        BondingCableTerminal = "I05",
                        KEBA_Definition = "DI_205",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "鑰匙開關_手動以上兩個訊號沒on為手動",
                        BondingCableTerminal = "I06",
                        KEBA_Definition = "DI_206",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "X+搖桿",
                        BondingCableTerminal = "I07",
                        KEBA_Definition = "DI_207",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "X-搖桿",
                        BondingCableTerminal = "I08",
                        KEBA_Definition = "DI_208",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "Y+搖桿",
                        BondingCableTerminal = "I09",
                        KEBA_Definition = "DI_209",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "Y-搖桿",
                        BondingCableTerminal = "I10",
                        KEBA_Definition = "DI_210",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "油壓過載",
                        BondingCableTerminal = "I11",
                        KEBA_Definition = "DI_211",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Motor1.di_Overload"
                    },
                    new IO_InfoModel()
                    {
                        Info = "刻印Y軸_負極限",
                        BondingCableTerminal = "I12",
                        KEBA_Definition = "DI_212",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.EngravingFeeding1.di_ServoNOT"
                    },
                    new IO_InfoModel()
                    {
                        Info = "進料X軸_正極限",
                        BondingCableTerminal = "I13",
                        KEBA_Definition = "DI_213",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Feeding1.di_ServoPOT"
                    },

                    new IO_InfoModel()
                    {
                        Info = "字碼刻印組_Z軸油壓缸待命位置",
                        BondingCableTerminal = "I14",
                        KEBA_Definition = "DI_214",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Engraving1.di_StandbyPoint"
                    },
                    new IO_InfoModel()
                    {
                        Info = "搖桿",
                        BondingCableTerminal = "I15",
                        KEBA_Definition = "DI_215",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "搖桿",
                        BondingCableTerminal = "I16",
                        KEBA_Definition = "DI_216",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "搖桿",
                        BondingCableTerminal = "I17",
                        KEBA_Definition = "DI_217",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "搖桿",
                        BondingCableTerminal = "I18",
                        KEBA_Definition = "DI_218",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },




                    new IO_InfoModel()
                    {
                        Info = "油壓單元啟動",
                        BondingCableTerminal = "O00",
                        KEBA_Definition = "DO_00",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Motor1.do_MotorOnMain"
                    },
                    new IO_InfoModel()
                    {
                        Info = "潤滑系統ON/OFF",
                        BondingCableTerminal = "O01",
                        KEBA_Definition = "DO_01",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Lubrication1.do_Lubrication"
                    },
                    new IO_InfoModel()
                    {
                        Info = "壓座組1_氣壓缸動作",
                        BondingCableTerminal = "O02",
                        KEBA_Definition = "DO_02",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture1.do_FixtureUp"
                    },
                    new IO_InfoModel()
                    {
                        Info = "預留",
                        BondingCableTerminal = "O03",
                        KEBA_Definition = "DO_03",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new IO_InfoModel()
                    {
                        Info = "阻擋缸_氣壓缸推出",
                        BondingCableTerminal = "O04",
                        KEBA_Definition = "DO_04",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.BlockingClips1.do_BlockingClipsUp"
                    },
                    new IO_InfoModel()
                    {
                        Info = "刻印壓座組2_氣壓缸動作",
                        BondingCableTerminal = "O05",
                        KEBA_Definition = "DO_05",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture2.do_FixtureUp"
                    },
                    new IO_InfoModel()
                    {
                        Info = "字碼刻印組_油壓缸上升",
                        BondingCableTerminal = "O06",
                        KEBA_Definition = "DO_06",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Engraving1.do_Open"
                    },
                    new IO_InfoModel()
                    {
                        Info = "字碼刻印組_油壓缸下降",
                        BondingCableTerminal = "O07",
                        KEBA_Definition = "DO_07",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Engraving1.do_Close"
                    },
                    new IO_InfoModel()
                    {
                        Info = "字碼刻印組_制動煞車",
                        BondingCableTerminal = "O08",
                        KEBA_Definition = "DO_08",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.EngravingRotate1.do_brake"
                    },
                    new IO_InfoModel()
                    {
                        Info = "裁切模組_上升",
                        BondingCableTerminal = "O09",
                        KEBA_Definition = "DO_09",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Cutting1.do_Open"
                    },
                    new IO_InfoModel()
                    {
                        Info = "裁切模組_下降",
                        BondingCableTerminal = "O10",
                        KEBA_Definition = "DO_10",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Cutting1.do_Close"
                    },
                    new IO_InfoModel()
                    {
                        Info = "進料導桿缸1-氣壓缸動作",
                        BondingCableTerminal = "O11",
                        KEBA_Definition = "DO_11",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.GuideRodsFixture1.do_GuideRodsFixtureUp"
                    },
  new IO_InfoModel()
                    {
                        Info = "進料導桿缸2-氣壓缸動作",
                        BondingCableTerminal = "O12",
                        KEBA_Definition = "DO_12",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.GuideRodsFixture2.do_GuideRodsFixtureUp"
                    },

                    new IO_InfoModel()
                    {
                        Info = "",
                        BondingCableTerminal = "O13",
                        KEBA_Definition = "DO_13",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },

                    new IO_InfoModel()
                    {
                        Info = "",
                        BondingCableTerminal = "O14",
                        KEBA_Definition = "DO_14",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },

                    new IO_InfoModel()
                    {
                        Info = "",
                        BondingCableTerminal = "O15",
                        KEBA_Definition = "DO_15",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                };
                IO_TableObservableCollection = new ObservableCollection<IO_InfoViewModel>(
                    io_info_Table.Select(model => new IO_InfoViewModel(model)));

                JsonHM.WriteJsonSettingByEnum(MachineSettingNameEnum.IO_Table, IO_TableObservableCollection);
            }

        }
        /*private void init_Stamping()
        {
            GD_Stamping = CommunicationSetting.Protocol switch
            {
                CommunicationProtocolEnum.Opcua => new GD_Stamping_Opcua(CommunicationSetting.HostString, CommunicationSetting.Port.Value, null, CommunicationSetting.UserName, CommunicationSetting.Password),
                _ => throw new NotImplementedException()
            };
        }*/


        protected override async ValueTask DisposeAsyncCore()
        {
            await StopScanOpcua();
            if (GD_Stamping != null) 
                await GD_Stamping.DisposeAsync();
        }



        private CommunicationSettingModel _communicationSetting;
        public CommunicationSettingModel CommunicationSetting
        {
            get => _communicationSetting ??= new CommunicationSettingModel();
            set => _communicationSetting = value;
        }


        //OpcuaTest _opcuaTest = new OpcuaTest();
        internal async Task TestConnect()
        {
            try
            {
               // var ret = await _opcuaTest.OpenFormBrowseServerAsync(null);
            }
            catch (Exception ex)
            {

            }
        }


        //private bool ContinueScanning = true;
        private bool _isScaning = false;
        public bool IsScaning
        {
            get => _isScaning;
            private set
            {
                _isScaning = value; OnPropertyChanged();
            }
        }


        private bool _isConnected = false;
        public bool IsConnected { get => _isConnected; private set { _isConnected = value; OnPropertyChanged(); } }

        private OperationModeEnum _operationMode;
        public OperationModeEnum OperationMode
        {
            get => _operationMode; 
            private set
            { 
                _operationMode = value; OnPropertyChanged(); 
            }
        }






        private Task scanTask;
        private CancellationTokenSource _cts;


        public async Task StartScanOpcua()
        {
            if (scanTask != null)
            {
                if (scanTask.Status == TaskStatus.Running)
                {
                    _cts?.Cancel();
                    await scanTask;
                }
            }

            if (scanTask == null || scanTask.Status != TaskStatus.Running)
            {
                _cts = new CancellationTokenSource();
                scanTask = Task.Run(() => RunScanTask(_cts.Token));
            }

        }

        public async Task StopScanOpcua()
        {
            await Task.Run(async () =>
            {
                try
                {
                    //等待掃描解除
                    _cts?.Cancel();
                    await GD_Stamping.DisconnectAsync();
                    //等待isscan 消失
                    if (scanTask != null)
                    {
                        if (scanTask.Status != TaskStatus.WaitingForActivation)
                        {
                            //等待Disconnect
                            await scanTask;
                            //scanTask = null;
                        }
                    }
                    await WaitForCondition.WaitAsync(() => GD_Stamping.IsConnected, false,new CancellationToken());
                }
                catch
                {

                }
            });
        }



        private async Task<bool> RunScanTask(CancellationToken cancelToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            _ = Task.Run(async() =>
            {
                IsScaning = true;
                var ManagerVM = new DevExpress.Mvvm.DXSplashScreenViewModel
                {
                    Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Image/svg/NewLogo_1-2.svg"),
                    Status = (string)System.Windows.Application.Current.TryFindResource("Connection_Init"),
                    Progress = 0,
                    IsIndeterminate = true,
                    Subtitle = null,
                    Copyright = null,
                };
                SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.ProcessingScreenWindow(), ManagerVM);
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    manager.Show(Application.Current.MainWindow, WindowStartupLocation.CenterScreen, true, InputBlockMode.None);
                });
                try
                {
                    await GD_Stamping?.DisconnectAsync();

                    GD_Stamping = new GD_Stamping_Opcua(CommunicationSetting.HostString, CommunicationSetting.Port.Value, null, CommunicationSetting.UserName, CommunicationSetting.Password);
                    ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_Init");
                    await Task.Delay(500);
                    //while (true)
                    //初始化連線
                    int retryCount = 1;
                    do
                    {
                        if (cancelToken.IsCancellationRequested)
                        {
                            cancelToken.ThrowIfCancellationRequested();
                        }
                        try
                        {
                            IsConnected = await GD_Stamping.AsyncConnect();
                            if (IsConnected)
                            {
                                ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_IsSucessful");
                                ManagerVM.Subtitle = string.Empty;

                                var feedingVelocityTuple = await GD_Stamping.GetFeedingVelocity();
                                if (feedingVelocityTuple.Item1)
                                    this.FeedingVelocity = feedingVelocityTuple.Item2;

                                var engravingFeedingTuple = await GD_Stamping.GetEngravingFeedingVelocity();
                                if (engravingFeedingTuple.Item1)
                                    this.EngravingFeeding = engravingFeedingTuple.Item2;

                                var rotateVelocityTuple = await GD_Stamping.GetEngravingRotateVelocity();
                                if (rotateVelocityTuple.Item1)
                                    this.RotateVelocity = rotateVelocityTuple.Item2;

                                var dataMatrixTCPIPTuple = await GD_Stamping.GetDataMatrixTCPIP();
                                if (dataMatrixTCPIPTuple.Item1)
                                    this.DataMatrixTCPIP = dataMatrixTCPIPTuple.Item2;

                                var dataMatrixPortTuple = await GD_Stamping.GetDataMatrixPort();
                                if (dataMatrixPortTuple.Item1)
                                {
                                    if (int.TryParse(dataMatrixPortTuple.Item2, out var port))
                                        this.DataMatrixPort = port;
                                    this.DataMatrixPort = 0;
                                }

                                var stampingPressureTuple = await GD_Stamping.GetStampingPressure();
                                if (stampingPressureTuple.Item1)
                                    this.StampingPressure = stampingPressureTuple.Item2;

                                var stampingVelocityTuple = await GD_Stamping.GetStampingVelocity();
                                if (stampingVelocityTuple.Item1)
                                    this.StampingVelocity = stampingVelocityTuple.Item2;

                                var shearingPressureTuple = await GD_Stamping.GetShearingPressure();
                                if (shearingPressureTuple.Item1)
                                    this.ShearingPressure = shearingPressureTuple.Item2;

                                var shearingVelocityTuple = await GD_Stamping.GetShearingVelocity();
                                if (shearingVelocityTuple.Item1)
                                    this.ShearingVelocity = shearingVelocityTuple.Item2;


                               
                                while (!await GD_Stamping.SubscribeOperationMode(value =>
                                  {
                                      OperationMode = (OperationModeEnum)value;
                                  }))
                                {
                                    await Task.Delay(100);
                                }

                                await GD_Stamping.SubscribeHydraulicPumpMotor(value =>
                                {
                                    HydraulicPumpIsActive = value;
                                });

                                await GD_Stamping.SubscribeRequestDatabit(value =>
                                Rdatabit = value, true);




                                //var ret7 = await GD_Stamping.GetLubricationSettingTime();
                                //var ret8 = await GD_Stamping.GetLubricationSettingOnTime();
                                //var ret9 = await GD_Stamping.GetLubricationSettingOffTime();


                                var ret10 = await GD_Stamping.GetLubricationActualTime();
                                var ret11 = await GD_Stamping.GetLubricationActualOnTime();
                                var ret12 = await GD_Stamping.GetLubricationActualOffTime();


                                var engravingRotateSVelocity = await GD_Stamping.GetEngravingRotateSetupVelocity();
                                var engravingFeedingfeedSVelocity = await GD_Stamping.GetEngravingFeedingSetupVelocity();
                                //初始化後直接設定其他數值
                                await Task.Delay(1);

                                //檢查字模

                                await Task.Delay(1000);

                                ObservableCollection<StampingTypeViewModel> rotatingStampingTypeVMObservableCollection = new();
                                var rotatingTurntableInfoList = await GD_Stamping.GetRotatingTurntableInfo();
                                if (rotatingTurntableInfoList.Item1)
                                {
                                    rotatingTurntableInfoList.Item2.ForEach(stamp =>
                                    {
                                        rotatingStampingTypeVMObservableCollection.Add(new StampingTypeViewModel(stamp));
                                    });

                                    await CompareFontsSettingBetweenMachineAndSoftware(
                                        StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection, rotatingStampingTypeVMObservableCollection
                                        );
                                }
                                //等待連線 並檢查字模是否設定正確->

                                break;
                            }
                            else
                            {
                                ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_RetryConnect") + $" - {retryCount}";
                                ManagerVM.Subtitle = GD_Stamping.ConnectException?.Message;
                                //ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_IsSucessful");
                            }

                        }
                        catch (Exception ex)
                        {
                            ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_Error");
                            ManagerVM.Subtitle = ex.Message;
                        }
                        retryCount++;
                        await Task.Delay(500);
                    }
                    while (!IsConnected);
                    //while (true) ;

                    manager?.Close();

                    //先前上一輪加工陣列的id
                    List<int> lastIronDataIList = new List<int>();

                    var machineTask = Task.Run(async () =>
                    {
                        while (true)
                        {
                            if (cancelToken.IsCancellationRequested)
                            {
                                cancelToken.ThrowIfCancellationRequested();
                            }
                            try
                            {
                                if (cancelToken.IsCancellationRequested)
                                    cancelToken.ThrowIfCancellationRequested();

                                IsConnected = await GD_Stamping.AsyncConnect();
                                if (IsConnected)
                                {


                                    manager?.Close();


                                    /*var opMode = await GD_Stamping.GetOperationMode();
                                    if (opMode.Item1)
                                        OperationMode = opMode.Item2;*/

                                    //GD_Stamping.GetMachineStatus
                                    var fPos = await GD_Stamping.GetFeedingPosition();
                                    if (fPos.Item1)
                                        FeedingPosition = fPos.Item2;

                                    //磁簧開關
                                    /*var Move_IsUp = Task.Run(() => 
                                    {
                                        if(GD_Stamping.GetCylinderActualPosition(StampingCylinderType.GuideRod_Move, DirectionsEnum.Up, out var move_IsUp))
                                            return move_IsUp;
                                        else
                                            return false;
                                    });
                                    Cylinder_GuideRod_Move_IsUp = await Move_IsUp;*/
                                    var PlateDataCollectionTask = await GD_Stamping.GetIronPlateDataCollection();
                                    if (PlateDataCollectionTask.Item1)
                                    {
                                        List<IronPlateDataModel> plateDataCollection = PlateDataCollectionTask.Item2;

                                        var plateMonitorVMCollection = new AsyncObservableCollection<PlateMonitorViewModel>();
                                        //產出圖形
                                        foreach (var plateData in plateDataCollection)
                                        {
                                            //取得字元長度
                                            var string1Length = plateData.sIronPlateName1.Length;
                                            var string2Length = plateData.sIronPlateName2.Length;

                                            SpecialSequenceEnum specialSequence;
                                            if (string2Length > 0)
                                                specialSequence = SpecialSequenceEnum.TwoRow;
                                            else
                                                specialSequence = SpecialSequenceEnum.OneRow;

                                            SteelBeltStampingStatusEnum steelBeltStampingStatus = SteelBeltStampingStatusEnum.None;
                                            if (plateData.bEngravingFinish)
                                                steelBeltStampingStatus = SteelBeltStampingStatusEnum.Stamping;
                                            else if (plateData.bDataMatrixFinish)
                                                steelBeltStampingStatus = SteelBeltStampingStatusEnum.QRCarving;

                                            int rowLength = string1Length > string2Length ? string1Length : string2Length;
                                            SettingBaseViewModel settingBaseVM;
                                            //沒有QR加工
                                            if (string.IsNullOrEmpty(plateData.sDataMatrixName1) && string.IsNullOrEmpty(plateData.sDataMatrixName2))
                                            {
                                                //補空白
                                                settingBaseVM = new NumberSettingViewModel()
                                                {
                                                    SpecialSequence = specialSequence,
                                                    SheetStampingTypeForm = SheetStampingTypeFormEnum.NormalSheetStamping,
                                                    // PlateNumber = plateData.sIronPlateName1++++ plateData.sIronPlateName2,
                                                    PlateNumber = plateData.sIronPlateName1.PadRight(rowLength) + plateData.sIronPlateName2,
                                                    SequenceCount = rowLength
                                                };
                                            }
                                            else
                                            {
                                                settingBaseVM = new QRSettingViewModel()
                                                {
                                                    SpecialSequence = specialSequence,
                                                    SheetStampingTypeForm = SheetStampingTypeFormEnum.QRSheetStamping,
                                                    // PlateNumber = plateData.sIronPlateName1++++ + plateData.sIronPlateName2,
                                                    PlateNumber = plateData.sIronPlateName1.PadRight(rowLength) + plateData.sIronPlateName2,
                                                    SequenceCount = rowLength,
                                                    //SequenceCount = string1Length > string2Length ? string1Length : string2Length,
                                                    QrCodeContent = plateData.sDataMatrixName1,
                                                    QR_Special_Text = plateData.sDataMatrixName2,
                                                };
                                            };

                                            PlateMonitorViewModel PlateMonitorVM = new PlateMonitorViewModel()
                                            {
                                                SettingBaseVM = settingBaseVM,
                                                StampingStatus = steelBeltStampingStatus,
                                                DataMatrixIsFinish = plateData.bDataMatrixFinish,
                                                EngravingIsFinish = plateData.bEngravingFinish,
                                            };
                                            plateMonitorVMCollection.Add(PlateMonitorVM);
                                        }

                                        //MachineSettingBaseCollection = plateMonitorVMCollection;
                                        if (MachineSettingBaseCollection.Count != plateMonitorVMCollection.Count)
                                        {
                                            await Application.Current.Dispatcher.InvokeAsync(() =>
                                            {
                                                MachineSettingBaseCollection = plateMonitorVMCollection;
                                            });
                                        }
                                        else
                                        {
                                            List<DispatcherOperation> invokeList = new();
                                            for (int i = 0; i < plateMonitorVMCollection.Count; i++)
                                            {
                                                int index = i;//防止閉包問題
                                                var invoke = Application.Current?.Dispatcher.InvokeAsync(async () =>
                                                {
                                                    MachineSettingBaseCollection[index] = plateMonitorVMCollection[index];
                                                    await Task.Delay(1);
                                                });
                                                invokeList.Add(invoke);
                                            }
                                            var invokeTasks = invokeList.ConvertAll(op => op.Task);
                                            await Task.WhenAll(invokeTasks);
                                        }



                                        //將現在的資料展開後寫入
                                        //  if (lastIronDataIList != null)
                                        //  {
                                        //foreach (var lastIronData in lastIronDataIList)
                                        // {
                                        var newlronDataIList = plateDataCollection.Select(x => x.iIronPlateID).ToList();
                                        foreach (var plateData in plateDataCollection)
                                        {
                                            //先找
                                            foreach (var projectDistribute in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                                            {
                                                //去盒子裡面找是否有對應的鐵片
                                                var boxPartsCollection = projectDistribute.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection;
                                                var partIndex = boxPartsCollection.FindIndex(x => x.ID == plateData.iIronPlateID && x.IsSended);
                                                if (partIndex != -1)
                                                {
                                                    boxPartsCollection[partIndex].MachiningStatus = MachiningStatusEnum.Run;
                                                    boxPartsCollection[partIndex].DataMatrixIsFinish = plateData.bDataMatrixFinish;
                                                    boxPartsCollection[partIndex].EngravingIsFinish = plateData.bEngravingFinish;
                                                    break;
                                                }
                                            }
                                        }
                                        //比較新舊兩個加工陣列
                                        //先檢查新陣列的id是否只有0 若只有0代表是被重新設定 不設定為完成(若有需要則另外設定)
                                        if (newlronDataIList.Count(x => x != 0) > 0)
                                        {
                                            var lastIronDataIListExcept = lastIronDataIList.Except(newlronDataIList).ToList();
                                            foreach (var ironDataID in lastIronDataIListExcept)
                                            {
                                                foreach (var projectDistribute in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                                                {
                                                    //去盒子裡面找是否有對應的鐵片
                                                    var boxPartsCollection = projectDistribute.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection;
                                                    var partIndex = boxPartsCollection.FindIndex(x => x.ID == ironDataID && x.IsSended);
                                                    if (partIndex != -1)
                                                    {
                                                        boxPartsCollection[partIndex].MachiningStatus = MachiningStatusEnum.Finish;
                                                        boxPartsCollection[partIndex].ShearingIsFinish = true;
                                                        boxPartsCollection[partIndex].IsFinish = true;
                                                        break;
                                                    }
                                                }

                                            }
                                        }
                                        lastIronDataIList = newlronDataIList;
                                    }


                                    var HmiIronPlateTask = await GD_Stamping.GetHMIIronPlate();
                                    if (HmiIronPlateTask.Item1)
                                    {
                                        HMIIronPlateDataModel = HmiIronPlateTask.Item2;
                                    }

                                    var rotatingTurntableInfoList = await GD_Stamping.GetRotatingTurntableInfo();
                                    if (rotatingTurntableInfoList.Item1)
                                    {
                                        var rotatingStampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>(); ;
                                        rotatingTurntableInfoList.Item2.ForEach(stamp =>
                                        {
                                            rotatingStampingTypeVMObservableCollection.Add(new StampingTypeViewModel(stamp));
                                        });

                                        if (RotatingTurntableInfoCollection.Count != rotatingStampingTypeVMObservableCollection.Count)
                                        {
                                            RotatingTurntableInfoCollection = rotatingStampingTypeVMObservableCollection;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                List<bool> StampingFontCompareBooleanList = new();
                                                for (int i = 0; i < RotatingTurntableInfoCollection.Count; i++)
                                                {
                                                    var eq = RotatingTurntableInfoCollection[i].Equals(rotatingStampingTypeVMObservableCollection[i]);
                                                    StampingFontCompareBooleanList.Add(eq);
                                                }
                                                if (StampingFontCompareBooleanList.Contains(false))
                                                {
                                                    RotatingTurntableInfoCollection = rotatingStampingTypeVMObservableCollection;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                //RotatingTurntableInfoCollection = rotatingStampingTypeVMObservableCollection;
                                            }
                                        }

                                    }

                                    var Move_IsUp = GD_Stamping.GetGuideRod_Move_Position_isUp();
                                    if ((await Move_IsUp).Item1)
                                        Cylinder_GuideRod_Move_IsUp = (await Move_IsUp).Item2;

                                    var Move_IsDown = GD_Stamping.GetGuideRod_Move_Position_isDown();
                                    if ((await Move_IsDown).Item1)
                                        Cylinder_GuideRod_Move_IsDown = (await Move_IsDown).Item2;

                                    var Fixed_IsUp = GD_Stamping.GetGuideRod_Fixed_Position_isUp();
                                    if ((await Fixed_IsUp).Item1)
                                        Cylinder_GuideRod_Fixed_IsUp = (await Fixed_IsUp).Item2;

                                    var Fixed_IsDown = GD_Stamping.GetGuideRod_Fixed_Position_isDown();
                                    if ((await Fixed_IsDown).Item1)
                                        Cylinder_GuideRod_Fixed_IsDown = (await Fixed_IsDown).Item2;

                                    var QRStamping_IsUp = GD_Stamping.GetQRStamping_Position_isUp();
                                    if ((await QRStamping_IsUp).Item1)
                                        Cylinder_QRStamping_IsUp = (await QRStamping_IsUp).Item2;

                                    var QRStamping_IsDown = GD_Stamping.GetQRStamping_Position_isDown();
                                    if ((await QRStamping_IsDown).Item1)
                                        Cylinder_QRStamping_IsDown = (await QRStamping_IsDown).Item2;

                                    var StampingSeat_IsUp = GD_Stamping.GetStampingSeat_Position_isUp();
                                    if ((await StampingSeat_IsUp).Item1)
                                        Cylinder_StampingSeat_IsUp = (await StampingSeat_IsUp).Item2;

                                    var StampingSeat_IsDown = GD_Stamping.GetStampingSeat_Position_isDown();
                                    if ((await StampingSeat_IsDown).Item1)
                                        Cylinder_StampingSeat_IsDown = (await StampingSeat_IsDown).Item2;

                                    var BlockingCylinder_IsUp = GD_Stamping.GetBlockingCylinder_Position_isUp();
                                    if ((await BlockingCylinder_IsUp).Item1)
                                        Cylinder_BlockingCylinder_IsUp = (await BlockingCylinder_IsUp).Item2;

                                    var BlockingCylindere_IsDown = GD_Stamping.GetBlockingCylinder_Position_isDown();
                                    if ((await BlockingCylindere_IsDown).Item1)
                                        Cylinder_BlockingCylindere_IsDown = (await BlockingCylindere_IsDown).Item2;



                                    var HydraulicEngraving_IsOrigin = GD_Stamping.GetHydraulicEngraving_Position_Origin();
                                    if ((await HydraulicEngraving_IsOrigin).Item1)
                                        Cylinder_HydraulicEngraving_IsOrigin = (await HydraulicEngraving_IsOrigin).Item2;

                                    var HydraulicEngraving_IsStandbyPoint = GD_Stamping.GetHydraulicEngraving_Position_StandbyPoint();
                                    if ((await HydraulicEngraving_IsStandbyPoint).Item1)
                                        Cylinder_HydraulicEngraving_IsStandbyPoint = (await HydraulicEngraving_IsStandbyPoint).Item2;

                                    var HydraulicEngraving_IsStopDown = GD_Stamping.GetHydraulicEngraving_Position_StopDown();
                                    if ((await HydraulicEngraving_IsStopDown).Item1)
                                        Cylinder_HydraulicEngraving_IsStopDown = (await HydraulicEngraving_IsStopDown).Item2;


                                    var HydraulicCutting_IsOrigin = GD_Stamping.GetHydraulicCutting_Position_Origin();
                                    if ((await HydraulicCutting_IsOrigin).Item1)
                                        Cylinder_HydraulicCutting_IsOrigin = (await HydraulicCutting_IsOrigin).Item2;

                                    var HydraulicCutting_IsStandbyPoint = GD_Stamping.GetHydraulicCutting_Position_StandbyPoint();
                                    if ((await HydraulicCutting_IsStandbyPoint).Item1)
                                        Cylinder_HydraulicCutting_IsStandbyPoint = (await HydraulicCutting_IsStandbyPoint).Item2;

                                    var HydraulicCutting_IsCutPoint = GD_Stamping.GetHydraulicCutting_Position_CutPoint();
                                    if ((await HydraulicCutting_IsCutPoint).Item1)
                                        Cylinder_HydraulicCutting_IsCutPoint = (await HydraulicCutting_IsCutPoint).Item2;




                                    var engravingRotateStation = GD_Stamping.GetEngravingRotateStation();
                                    if ((await engravingRotateStation).Item1)
                                    {
                                        if (Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection.TryGetValue((await engravingRotateStation).Item2, out var stamptype))
                                        {
                                            Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeModel_ReadyStamping = stamptype;
                                        }
                                    }


                                    //箱子
                                    var boxIndex = GD_Stamping.GetSeparateBoxNumber();
                                    if ((await boxIndex).Item1)
                                    {
                                        SeparateBoxIndex = (await boxIndex).Item2;
                                    }

                                    var engravingYposition = GD_Stamping.GetEngravingYAxisPosition();
                                    if ((await engravingYposition).Item1)
                                        EngravingYAxisPosition = (await engravingYposition).Item2;

                                    var engravingZposition = GD_Stamping.GetEngravingZAxisPosition();
                                    if ((await engravingZposition).Item1)
                                        EngravingZAxisPosition = (await engravingZposition).Item2;

                                    var engravingAStation = GD_Stamping.GetEngravingRotateStation();
                                    if ((await engravingAStation).Item1)
                                        EngravingRotateStation = (await engravingAStation).Item2;

                                }
                                else
                                {
                                    await Application.Current.Dispatcher.InvokeAsync(() =>
                                    {
                                        if (manager.State == DevExpress.Mvvm.SplashScreenState.Closed)
                                        {
                                            manager = null;
                                            manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.ProcessingScreenWindow(), ManagerVM);
                                            manager.Show(Application.Current.MainWindow, WindowStartupLocation.CenterScreen, true, InputBlockMode.None);
                                        }

                                        ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_RetryConnect");
                                    });
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                await GD_Stamping.DisconnectAsync();
                                break;
                            }
                            catch (Exception ex)
                            {
                                Debugger.Break();
                            }
                            finally
                            {

                            }
                            await Task.Delay(1000);
                        }
                    }, cancelToken);

                    var ioTask = Task.Run(async () =>
                    {
                        List<(string nodeID, Action<object> action ,int samplingInterval, bool checkDuplicates)> ioUpdateNodes = new();

                        foreach (var IO_Table in IO_TableObservableCollection)
                        {
                            if (!string.IsNullOrEmpty(IO_Table.NodeID))
                            {
                                var Rtask = await GD_Stamping.ReadNode<object>(IO_Table.NodeID);
                                if (Rtask.Item1)
                                    IO_Table.IO_Value = Rtask.Item2;

                                ioUpdateNodes.Add(new(IO_Table.NodeID, value =>
                                {
                                    IO_Table.IO_Value = value;
                                },1000 , false));

                                /*
                                if (IO_Table.ValueType == typeof(bool))
                                {
                                    bool_ioUpdateNodes.Add(new(IO_Table.NodeID, value =>
                                    {
                                        IO_Table.IO_Value = value;
                                    }));
                                }
                                else if (IO_Table.ValueType == typeof(float))
                                {
                                    float_ioUpdateNodes.Add(new(IO_Table.NodeID, value =>
                                    {
                                        IO_Table.IO_Value = value ;
                                    }));
                                }
                                else if (IO_Table.ValueType is object)
                                {
                                    object_ioUpdateNodes.Add(new(IO_Table.NodeID, value =>
                                    {
                                        IO_Table.IO_Value = value;
                                    }));
                                }
                                else
                                {
                                    IO_Table.IO_Value = null;
                                }*/
                            }
                        }

                       var ioList =(await GD_Stamping.SubscribeNodesDataChangeAsync(ioUpdateNodes)).ToList();



                        /*
                while (true)
                {
                    if (GD_Stamping is GD_Stamping_Opcua GD_StampingOpcua)
                    {
                        foreach (var IO_Table in IO_TableObservableCollection)
                        {
                            if (cancelToken.IsCancellationRequested)
                            {
                                cancelToken.ThrowIfCancellationRequested();
                            }
                            //註冊
                            if (!string.IsNullOrEmpty(IO_Table.NodeID))
                            {
                                if (IO_Table.ValueType == typeof(bool))
                                {
                                    IO_Table.IO_Value = GD_Stamping.ReadNode<bool>(IO_Table.NodeID);
                                }
                                else if (IO_Table.ValueType == typeof(float))
                                {
                                    IO_Table.IO_Value = GD_Stamping.ReadNode<float>(IO_Table.NodeID);
                                }
                                else if (IO_Table.ValueType is object)
                                {
                                    IO_Table.IO_Value = GD_Stamping.ReadNode<object>(IO_Table.NodeID);
                                }
                                else
                                {
                                    IO_Table.IO_Value = null;
                                }
                            }
                        }
                    }
                }*/
                    }, cancelToken);
                    await Task.WhenAll(machineTask);
                }
                catch (OperationCanceledException cex)
                {
                    await GD_Stamping?.DisconnectAsync();
                    Console.WriteLine("工作已取消");
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    await GD_Stamping?.DisconnectAsync();
                    await WaitForCondition.WaitAsync(() => GD_Stamping.IsConnected, false,  cancelToken);
                    IsConnected = false;
                    try
                    {
                        manager?.Close();
                        manager = null;
                    }
                    catch
                    {

                    }
                }
                IsScaning = false;
                tcs.SetResult(true);
            });
            return await tcs.Task;
        }

        public async Task<bool> CompareFontsSettingBetweenMachineAndSoftware(ObservableCollection<StampingTypeViewModel> settingCollection)
        {
            return await CompareFontsSettingBetweenMachineAndSoftware(settingCollection, this.RotatingTurntableInfoCollection);
        }

        public async Task<bool> CompareFontsSettingBetweenMachineAndSoftware(ObservableCollection<StampingTypeViewModel> settingCollection,  ObservableCollection<StampingTypeViewModel> rotatingCollection)
        {                        
            //比較鋼印字模/機台字模的字元是否相同 -> 若不同則跳出一個視窗提示               
            // var settingCollection =  StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection;  
            //var rotatingCollection = RotatingTurntableInfoCollection;
            bool FontsIsSame = false;
            try
            {

                bool isSameCount = settingCollection.Count == rotatingCollection.Count;
                // bool hasSameContent = true;
                List<int> DifferentContent_StampingTypeNumberList = new();
                if (isSameCount)
                {
                    //
                    for (int i = 0; i < rotatingCollection.Count; i++)
                    {
                        if (settingCollection[i].StampingTypeString != rotatingCollection[i].StampingTypeString)
                        {
                            //hasSameContent = false;
                            DifferentContent_StampingTypeNumberList.Add(settingCollection[i].StampingTypeNumber);
                            //紀錄字模編號
                            //break;
                        }
                    }

                }
                //跳出提示

                try
                {
                    // if (true)
                    //字模數量不相等
                    if (!isSameCount)
                    {
                        //if()
                        var countIsDifferent = (string)Application.Current.TryFindResource("Notify_PunchedFontsCountIsDifferent");
                        if (countIsDifferent != null)
                        {
                            string Outputstring = string.Format(countIsDifferent,
                                rotatingCollection.Count,
                                settingCollection.Count);
                            //兩邊字模數量不合
                            await MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                                Outputstring);
                        }
                    }
                    // else if (!hasSameContent)
                    //字模內容不相同
                    else if (DifferentContent_StampingTypeNumberList.Count > 0)
                    {
                        var dataIsDifferent = (string)Application.Current.TryFindResource("Notify_PunchedFontsDataIsDifferent");
                        if (dataIsDifferent != null)
                        {
                            var numberlist = DifferentContent_StampingTypeNumberList.ExpandToString();
                            string Outputstring = string.Format(dataIsDifferent, numberlist);

                            await MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                                Outputstring);
                        }


                    }
                    else
                    {
                        FontsIsSame = true;
                    }
                }
                catch (Exception ex)
                {
                    await MessageBoxResultShow.ShowException(ex);
                }
            }
            catch (Exception ex)
            {
                await MessageBoxResultShow.ShowException(ex);
            }

            return FontsIsSame;
        }


        private ObservableCollection<IO_InfoViewModel> _io_tableObservableCollection;
        /// <summary>
        /// IO表
        /// </summary>
        public ObservableCollection<IO_InfoViewModel> IO_TableObservableCollection
        {
            get=> _io_tableObservableCollection ??= new ObservableCollection<IO_InfoViewModel>();
            set
            {
                _io_tableObservableCollection = value;
                OnPropertyChanged();
            }
        }



        /// <summary>
        /// 進料X軸回歸原點
        /// </summary>
        /*public AsyncRelayCommand Feeding_XAxis_ReturnToStandbyPosition
        {
            get => new(async token=>
            {
                if (GD_Stamping.IsConnected)
                {
                    await GD_Stamping.FeedingPositionReturnToStandbyPosition();
                    //GD_Stamping.Disconnect();
                }
            }, ()=> !Feeding_XAxis_ReturnToStandbyPosition.IsRunning);
        }*/






        /// <summary>
        /// 進料X軸移動
        /// </summary>
        public AsyncRelayCommand<object> Feeding_XAxis_MoveCommand
        {
            get => new AsyncRelayCommand<object>(async Parameter =>
            {
                    if (Parameter != null)
                    {
                        if (float.TryParse(Parameter.ToString(), out var ParameterValue))
                        {
                            if (GD_Stamping.IsConnected)
                            {
                                if (ParameterValue > 0)
                                {
                                    await GD_Stamping.FeedingPositionFwd(true);
                                    await Task.Delay(500);
                                    await GD_Stamping.FeedingPositionFwd(false);
                                }
                                else if (ParameterValue < 0)
                                {
                                    await GD_Stamping.FeedingPositionBwd(true);
                                    await Task.Delay(500);
                                    await GD_Stamping.FeedingPositionBwd(false);
                                }
                                else
                                {
                                    Debugger.Break();
                                }
                                /*if (GD_Stamping.GetFeedingPosition(out var FPosition))
                                {

                                    GD_Stamping.SetFeedingPosition(FPosition + ParameterValue);



                                    await Task.Delay(2000);

                                    GD_Stamping.GetFeedingPosition(out var FPosition2);
                                    var value = FPosition2 - FPosition;



                                }*/
                            }
                        }
                    }
            });
        }

        public AsyncRelayCommand Feeding_XAxis_MoveStopCommand
        {
            get => new AsyncRelayCommand(async () =>
            {
                for (int tryCount = 0; tryCount < 10; tryCount++)
                {
                    try
                    {
                        if (GD_Stamping.IsConnected)
                        {
                            var ret1 = await GD_Stamping.FeedingPositionFwd(false);
                            var ret2 = await GD_Stamping.FeedingPositionBwd(false);
                            if (ret1 && ret2)
                                break;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            });
        }

        public AsyncRelayCommand<object> FeedingXAxisFwdCommand
        {
            get => new(async obj =>
            {
                    if (obj is bool isActived)
                    {
                        await SetFeedingXAxisFwd(isActived);
                    }
            }, obj => IsConnected);
        }

        public AsyncRelayCommand<object> FeedingXAxisBwdCommand
        {
            get => new(async obj =>
            {
                if (obj is bool isActived)
                {
                    await SetFeedingXAxisBwd(isActived);
                }
            });
        }







        /// <summary>
        /// 雙導桿缸-可動端 壓座控制 放鬆
        /// </summary>
        public AsyncRelayCommand<object> GuideRod_Move_Up_Command
        {
            get => new AsyncRelayCommand<object>(async para =>
            {
                if (para is bool isTriggered)
                {
                    if (GD_Stamping.IsConnected)
                        await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.GuideRod_Move, DirectionsEnum.Up, isTriggered);
                }
            });
        }

        /// <summary>
        /// 雙導桿缸-可動端 壓座控制 夾緊
        /// </summary>
        public AsyncRelayCommand GuideRod_Move_Down_Command
        {
            get => new AsyncRelayCommand(async para =>
            {
                if (GD_Stamping.IsConnected)
                {
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.GuideRod_Move, DirectionsEnum.Down, true);
                    await Task.Delay(500);
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.GuideRod_Move, DirectionsEnum.Down, false);
                }
            });
        }


        /// <summary>
        /// 雙導桿缸-固定端 壓座控制 放鬆
        /// </summary>
        public AsyncRelayCommand GuideRod_Fixed_Up_Command
        {
            get => new AsyncRelayCommand(async para =>
            {
                if (GD_Stamping.IsConnected)
                {
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Up, true);
                    await Task.Delay(500);
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Up, false);

                }

            });
        }

        /// <summary>
        /// 雙導桿缸-固定端 壓座控制 夾緊
        /// </summary>
        public AsyncRelayCommand GuideRod_Fixed_Down_Command
        {
            get => new AsyncRelayCommand(async para =>
            {
                if (GD_Stamping.IsConnected)
                {
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Down, true);
                    await Task.Delay(500);
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Down, false);
                }
            });
        }


        /// <summary>
        /// QR壓座組
        /// </summary>

        public AsyncRelayCommand QRStamping_Up_Command
        {
            get => new AsyncRelayCommand(async para =>
            {
                if (GD_Stamping.IsConnected)
                {
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.QRStamping, DirectionsEnum.Up, true);
                    await Task.Delay(500);
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.QRStamping, DirectionsEnum.Up, false);
                }
            }, () => !QRStamping_Up_Command.IsRunning);
        }




        private AsyncRelayCommand _qRStamping_Down_Command;
        /// <summary>
        /// QR壓座組
        /// </summary>

        public AsyncRelayCommand QRStamping_Down_Command
        {
            get => _qRStamping_Down_Command??=new AsyncRelayCommand(async token =>
            {
                if (GD_Stamping.IsConnected)
                {
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.QRStamping, DirectionsEnum.Down, true);
                    await Task.Delay(500);
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.QRStamping, DirectionsEnum.Down, false);
                }
            }, () => !QRStamping_Down_Command.IsRunning);
        }


        private AsyncRelayCommand _stampingSeat_Up_Command;
        /// <summary>
        /// 鋼印壓座組
        /// </summary>
        public AsyncRelayCommand StampingSeat_Up_Command
        {
            get => _stampingSeat_Up_Command ??= new AsyncRelayCommand(async token =>
            {
                if (GD_Stamping.IsConnected)
                {
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.StampingSeat, DirectionsEnum.Up, true);
                    await Task.Delay(500);
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.StampingSeat, DirectionsEnum.Up, false);
                }
            });
        }



        private AsyncRelayCommand _stampingSeat_Down_Command;
        /// <summary>
        /// 鋼印壓座組
        /// </summary>
        public AsyncRelayCommand StampingSeat_Down_Command
        {
            get => _stampingSeat_Down_Command??=new AsyncRelayCommand(async token =>
            {
                if (GD_Stamping.IsConnected)
                {
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.StampingSeat, DirectionsEnum.Down, true);
                    await Task.Delay(500);
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.StampingSeat, DirectionsEnum.Down, false);
                }
            });
        }


        private AsyncRelayCommand _blockingCylinder_Up_Command;
        /// <summary>
        /// 阻擋缸
        /// </summary>
        public AsyncRelayCommand BlockingCylinder_Up_Command
        {
            get => _blockingCylinder_Up_Command??= new AsyncRelayCommand(async token =>
            {
                    if (GD_Stamping.IsConnected)
                    {
                        await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.BlockingCylinder, DirectionsEnum.Up, true);
                        await Task.Delay(500);
                        await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.BlockingCylinder, DirectionsEnum.Up, false);
                    }

            });
        }

        private AsyncRelayCommand _blockingCylinder_Down_Command;
        /// <summary>
        /// 阻擋缸
        /// </summary>
        public AsyncRelayCommand BlockingCylinder_Down_Command
        {
            get => _blockingCylinder_Down_Command ??= new AsyncRelayCommand(async token =>
            {
                if (GD_Stamping.IsConnected)
                {
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.BlockingCylinder, DirectionsEnum.Down, true);
                    await Task.Delay(500);
                    await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.BlockingCylinder, DirectionsEnum.Down, false);
                }
            });
        }

        private AsyncRelayCommand<object> _hydraulicEngraving_Up_Command;
        /// <summary>
        /// Z軸油壓缸
        /// </summary>

        public AsyncRelayCommand<object> HydraulicEngraving_Up_Command
        {
            get => _hydraulicEngraving_Up_Command ??= new AsyncRelayCommand<object>(async obj =>
            {
                if (obj is bool tirggered)
                {
                    // if (!tirggered)
                    //     await Task.Delay(200);
                    if (GD_Stamping.IsConnected)
                    {
                        await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Up, tirggered);
                    }
                }
            });
        }


        private AsyncRelayCommand<object> _hydraulicEngraving_Down_Command;
        /// <summary>
        /// Z軸油壓缸
        /// </summary>
        public AsyncRelayCommand<object> HydraulicEngraving_Down_Command
        {
            get => _hydraulicEngraving_Down_Command ??= new AsyncRelayCommand<object>(async obj =>
            {
                if (obj is bool tirggered)
                {
                    //  if (!tirggered)
                    //      await Task.Delay(200);
                    if (GD_Stamping.IsConnected)
                    {
                        await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Down, tirggered);
                    }
                }
            });
        }



        private AsyncRelayCommand<object> _hydraulicCutting_Up_Command;
        /// <summary>
        /// 裁切
        /// </summary>
        public AsyncRelayCommand<object> HydraulicCutting_Up_Command
        {
            get => _hydraulicCutting_Up_Command??= new AsyncRelayCommand<object>(async para =>
            {
                    if (para is bool isTriggered)
                    {
                       // if (!isTriggered)
                      //      await Task.Delay(200);
                        if (GD_Stamping.IsConnected)
                            await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.HydraulicCutting, DirectionsEnum.Up, isTriggered);
                    }
            });
        }


        private AsyncRelayCommand<object> _hydraulicCutting_Down_Command;
        /// <summary>
        /// 裁切
        /// </summary>
        public AsyncRelayCommand<object> HydraulicCutting_Down_Command
        {
            get => _hydraulicCutting_Down_Command ??= new AsyncRelayCommand<object>(async para =>
            {
                if (para is bool isTriggered)
                {
                    // if (!isTriggered)
                    //     await Task.Delay(200);
                    if (GD_Stamping.IsConnected)
                        await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.HydraulicCutting, DirectionsEnum.Down, isTriggered);
                }
            });
        }





























        private AsyncRelayCommand _activeHydraulicPumpMotor;
        /// <summary>
        /// 啟用/禁用液壓馬達
        /// </summary>
        public AsyncRelayCommand ActiveHydraulicPumpMotor
        {
            get => _activeHydraulicPumpMotor??= new AsyncRelayCommand(async () =>
            {
                if (GD_Stamping.IsConnected)
                {
                    var isActived = await GD_Stamping.GetHydraulicPumpMotor();
                    if (isActived.Item1)
                    {
                        await GD_Stamping.SetHydraulicPumpMotor(!isActived.Item2);
                    }
                }

            },()=> !_activeHydraulicPumpMotor.IsRunning);
        }

        public async Task<bool> SetHMIIronPlateData(IronPlateDataModel ironPlateData)
        {
            if (GD_Stamping.IsConnected)
            {
                /*
                await GD_Stamping.SetDataMatrixMode(
                    !string.IsNullOrEmpty(ironPlateData.sDataMatrixName1) 
                    || !string.IsNullOrEmpty(ironPlateData.sDataMatrixName2));
                */
                await GD_Stamping.SetDataMatrixMode(true);
                return await GD_Stamping.SetHMIIronPlate(ironPlateData);
            }
            else
                return false;
        }

        /// <summary>
        /// 設定下一片加工資料
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, IronPlateDataModel)> GetHMIIronPlateData()
        {
            if (GD_Stamping.IsConnected)
                return await GD_Stamping.GetHMIIronPlate();
            else
                return (false, new IronPlateDataModel());
        }

        /// <summary>
        /// 設定加工陣列
        /// </summary>
        /// <param name="ironPlateDataList"></param>
        /// <returns></returns>
        public async Task<bool> SetIronPlateDataCollection(List<IronPlateDataModel> ironPlateDataList)
        {
            if (GD_Stamping.IsConnected)
                return await GD_Stamping.SetIronPlateDataCollection(ironPlateDataList);
            else
                return false;
        }

        /// <summary>
        /// 取得加工資料
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, List<IronPlateDataModel>)> GetIronPlateDataCollection()
        {
            if (GD_Stamping.IsConnected)
                return await GD_Stamping.GetIronPlateDataCollection();
            else
                return (false, new List<IronPlateDataModel>());
        }


        /// <summary>
        /// 允許加工訊號
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetRequestDatabit()
        {
            if (GD_Stamping.IsConnected)
            {
                var ret = await GD_Stamping.GetRequestDatabit();
                if (ret.Item1)
                    return ret.Item2;
            }
            return false;
        }

        /// <summary>
        /// 訂閱第一片ID
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SubscribeFirstIronPlate(Action<int> action , int samplingInterval, bool checkDuplicates = false)
        {
            var ret = false;
            if (GD_Stamping.IsConnected)
            {
                ret = await GD_Stamping.SubscribeFirstIronPlate(action , samplingInterval, checkDuplicates);
            }
            return ret;
        }

        /// <summary>
        /// 取消訂閱第一片ID
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UnsubscribeFirstIronPlate(int samplingInterval)
        {
            var ret = false;
            if (GD_Stamping.IsConnected)
            {
                ret = await GD_Stamping.UnsubscribeFirstIronPlate(samplingInterval);
            }
            return ret;
        }







        /// <summary>
        /// 加工訊號交握
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        public async Task<bool> SetRequestDatabit(bool databit)
        {
            if (GD_Stamping.IsConnected)
            {
                var ret = await GD_Stamping.SetRequestDatabit(databit);
                return ret;
            }
            return false;
        }







        private AsyncRelayCommand _returnToOriginCommand;
        /// <summary>
        /// 回歸原點
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        public AsyncRelayCommand ReturnToOriginCommand
        {
            get => _returnToOriginCommand??= new (async () =>
            {
                IsReturningToOrigin = true;
                try
                {
                    CancellationTokenSource cts = new CancellationTokenSource();
                    var token = cts.Token;
                    if (GD_Stamping.IsConnected)
                    {
                        var MotorIsActived = await GD_Stamping.GetHydraulicPumpMotor();
                        if (MotorIsActived.Item1)
                        {
                            if (!MotorIsActived.Item2)
                            {
                                //油壓馬達尚未啟動
                                var Result = MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                                    (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorIsNotAcitved"));

                                LogDataSingleton.Instance.AddLogData(this.DataSingletonName, (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorIsNotAcitved"));

                                throw new Exception();
                            }
                            else
                            {
                                var opMode = await GD_Stamping.GetOperationMode();
                                if (opMode.Item1)
                                {
                                    //OperationMode = opMode.Item2;
                                    //要在工程模式
                                    if (opMode.Item2 != OperationModeEnum.Setup)
                                    {
                                        //需在工程模式才可執行
                                        var Result = MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                                            (string)Application.Current.TryFindResource("Text_MachineNotInSetupMode"));

                                        LogDataSingleton.Instance.AddLogData(this.DataSingletonName, (string)Application.Current.TryFindResource("Text_MachineNotInSetupMode"));
                                    }
                                    else
                                    {
                                        if (GD_Stamping.IsConnected)
                                        {

                                            bool EngravingToOriginSucessful;
                                            if ((await GD_Stamping.GetHydraulicEngraving_Position_Origin()).Item2)
                                            {
                                                EngravingToOriginSucessful = true;
                                            }
                                            else
                                            {
                                                await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Up, true);
                                                var EngravingToOriginTask = Task.Run(async () =>
                                                {
                                                    try
                                                    {
                                                        var EngravingIsOrigin = false;
                                                        do
                                                        {
                                                            if (token.IsCancellationRequested)
                                                                token.ThrowIfCancellationRequested();
                                                            EngravingIsOrigin = (await GD_Stamping.GetHydraulicEngraving_Position_Origin()).Item2;
                                                            await Task.Delay(10);
                                                        }
                                                        while (!EngravingIsOrigin);
                                                    }
                                                    catch (OperationCanceledException Cex)
                                                    {
                                                        Singletons.LogDataSingleton.Instance.AddLogData(Cex.Source, Cex.Message);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Singletons.LogDataSingleton.Instance.AddLogData(ex.Source, ex.Message);
                                                    }
                                                });

                                                Task completedTask = await Task.WhenAny(EngravingToOriginTask, Task.Delay(10000));
                                                await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Up, false);
                                                EngravingToOriginSucessful = completedTask == EngravingToOriginTask;
                                            }


                                            if (!EngravingToOriginSucessful)
                                            {
                                                var Result = MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                                                    (string)Application.Current.TryFindResource("Text_EngravingToOriginTimeout"));
                                                return;
                                            }

                                            bool CuttungToOriginSucessful;

                                            //先檢查是否在原點
                                            if ((await GD_Stamping.GetHydraulicCutting_Position_Origin()).Item2)
                                            {
                                                CuttungToOriginSucessful = true;
                                            }
                                            else
                                            {

                                                await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.HydraulicCutting, DirectionsEnum.Up, true);
                                                var CuttingToOriginTask = Task.Run(async () =>
                                                {
                                                    try
                                                    {
                                                        var CuttingIsOrigin = false;
                                                        do
                                                        {
                                                            if (token.IsCancellationRequested)
                                                                token.ThrowIfCancellationRequested();

                                                            CuttingIsOrigin = (await GD_Stamping.GetHydraulicCutting_Position_Origin()).Item2;
                                                            await Task.Delay(10);
                                                        }
                                                        while (!CuttingIsOrigin);
                                                    }
                                                    catch (OperationCanceledException Cex)
                                                    {
                                                        Singletons.LogDataSingleton.Instance.AddLogData(Cex.Source, Cex.Message);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Singletons.LogDataSingleton.Instance.AddLogData(ex.Source, ex.Message);
                                                    }

                                                });
                                                Task completedTask = await Task.WhenAny(CuttingToOriginTask, Task.Delay(10000));
                                                await GD_Stamping.Set_IO_CylinderControl(StampingCylinderType.HydraulicCutting, DirectionsEnum.Up, false);
                                                CuttungToOriginSucessful = completedTask == CuttingToOriginTask;
                                            }

                                            if (!CuttungToOriginSucessful)
                                            {
                                                var Result = MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                                                    (string)Application.Current.TryFindResource("Text_CuttingToOriginTimeout"));
                                                //超時
                                                return;
                                            }

                                            //X軸回歸
                                            var ret = await GD_Stamping.FeedingPositionReturnToStandbyPosition();

                                            //double lastPositon = 0;
                                            if (ret)
                                            {
                                                try
                                                {
                                                    var servoHomeTask = Task.Run(async () =>
                                                    {
                                                        bool isHome = false;
                                                        try
                                                        {
                                                            do
                                                            {
                                                                if (token.IsCancellationRequested)
                                                                    token.ThrowIfCancellationRequested();
                                                                isHome = (await GD_Stamping.GetServoHome()).Item2;
                                                            }
                                                            while (!isHome);
                                                        }
                                                        catch (OperationCanceledException)
                                                        {

                                                        }
                                                        catch (Exception)
                                                        {

                                                        }
                                                        return isHome;
                                                    }, token);

                                                    var timeOutServoHomeTask = Task.Run(async () =>
                                                    {
                                                        double lastPos = 0;
                                                        int posCount = 0;
                                                        //如果取得的數值都沒變->代表X軸停止->
                                                        while (!token.IsCancellationRequested)
                                                        {
                                                            var getHome = await GD_Stamping.GetServoHome();
                                                            if (getHome.Item1)
                                                            {
                                                                if (!getHome.Item2)
                                                                {
                                                                    var feedingPosition = await GD_Stamping.GetFeedingPosition();
                                                                    if (feedingPosition.Item1)
                                                                    {
                                                                        var NowPos = feedingPosition.Item2;
                                                                        if (Math.Abs(NowPos - lastPos) < 0.1)
                                                                        {
                                                                            //位置很相近
                                                                            //紀錄次數 若超過50次則視為超時
                                                                            if (posCount > 50)
                                                                                break;
                                                                            posCount++;
                                                                        }
                                                                        else
                                                                        {
                                                                            lastPos = NowPos;
                                                                            posCount = 0;
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                            await Task.Delay(1000);
                                                        }
                                                        await Task.Delay(500);
                                                    });
                                                    var completedTask = await Task.WhenAny(servoHomeTask, timeOutServoHomeTask);
                                                    if (completedTask == servoHomeTask)
                                                    {
                                                        if (await servoHomeTask)
                                                        {
                                                            await MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                                                                 (string)Application.Current.TryFindResource("Text_ToOriginisSuccessful"));
                                                        }
                                                        else
                                                        {
                                                            await MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                                                                 (string)Application.Current.TryFindResource("Text_ToOriginisFail"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        await MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                                                            (string)Application.Current.TryFindResource("Text_ToOriginisTimeout"));
                                                    }
                                                }
                                                catch (OperationCanceledException cex)
                                                {
                                                    await MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                                                        (string)Application.Current.TryFindResource("Text_ToOriginisCancel"));
                                                }
                                                catch (Exception ex)
                                                {
                                                    await MessageBoxResultShow.ShowException(ex);
                                                }
                                                //超時
                                                finally
                                                {

                                                }

                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        await MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                            (string)Application.Current.TryFindResource("Connection_Error"));
                    }
                    cts.Cancel();
                }
                catch (OperationCanceledException)
                {

                }
                catch
                {

                }
                IsReturningToOrigin = false;

            }, () => !ReturnToOriginCommand.IsRunning);
        }

        private bool _isReturningToOrigin;
        public bool IsReturningToOrigin
        {
            get => _isReturningToOrigin;
            set
            {
                _isReturningToOrigin = value;
                OnPropertyChanged();
            }
        }




    /*    private async void Set_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum Direction, bool IsTrigger)
        {
            if (GD_Stamping.IsConnected)
            {
                await GD_Stamping.Set_IO_CylinderControl(stampingCylinder, Direction, IsTrigger);
                //GD_Stamping.Disconnect();
            }
        }*/


        public async Task<bool> SetSeparateBoxNumber(int Index)
        {
            var ret = false;
            if (GD_Stamping.IsConnected)
            {
                ret = await GD_Stamping.SetSeparateBoxNumber(Index);
                //GD_Stamping.Disconnect();
            }
            return ret;
        }

        public async Task<(bool, int)> GetSeparateBoxNumber()
        {
            var ret = (false, -1);
            if (GD_Stamping.IsConnected)
            {
                ret =await GD_Stamping.GetSeparateBoxNumber();
                //GD_Stamping.Disconnect();
            }
            return ret;
        }



        private AsyncRelayCommand<object> _engravingYAxisToStandbyPosCommand;
        public AsyncRelayCommand<object> EngravingYAxisToStandbyPosCommand
        {
            get => _engravingYAxisToStandbyPosCommand??= new AsyncRelayCommand<object>(async obj =>
            {
                if (obj is bool IsActived)
                    await SetEngravingYAxisToStandbyPos();
            }, obj => IsConnected);
        }

        private AsyncRelayCommand<object> _engravingYAxisFwdCommand;
        public AsyncRelayCommand<object> EngravingYAxisFwdCommand
        {
            get => _engravingYAxisFwdCommand ??= new(async obj =>
            {
                if (obj is bool isActived)
                {

                    await SetEngravingYAxisFwd(isActived);
                }
            }, obj => IsConnected);
        }

        private AsyncRelayCommand<object> _engravingYAxisBwdCommand;
        public AsyncRelayCommand<object> EngravingYAxisBwdCommand
        {
            get => _engravingYAxisBwdCommand ??= new(async obj =>
            {
                if (obj is bool isActived)
                {
                    await SetEngravingYAxisBwd(isActived);
                }
            });
        }









        private AsyncRelayCommand<object> _engravingRotateClockwiseCommand;
        public AsyncRelayCommand<object> EngravingRotateClockwiseCommand
        {
            get => _engravingRotateClockwiseCommand??= new AsyncRelayCommand<object>(async obj =>
            {
                if (obj is bool isActived)
                {
                  


                    if (!isActived && (await GD_Stamping.GetOperationMode()).Item2 == OperationModeEnum.Manual)
                        await Task.Delay(200);
                    await GD_Stamping.SetEngravingRotateCW(isActived);
                }
            });
        }








        private AsyncRelayCommand<object> _engravingRotateCounterClockwiseCommand;
         public AsyncRelayCommand<object> EngravingRotateCounterClockwiseCommand
        {
            get => _engravingRotateCounterClockwiseCommand ??= new AsyncRelayCommand<object>(async obj =>
            {
                if (obj is bool isActived)
                {
                    if (!isActived && (await GD_Stamping.GetOperationMode()).Item2 == OperationModeEnum.Manual)
                        await Task.Delay(200);
                    await GD_Stamping.SetEngravingRotateCCW(isActived);
                }
            });
        }


        private async Task<bool> CheckHydraulicPumpMotor()
        {
            var MotorIsActived = await GD_Stamping.GetHydraulicPumpMotor();
            if (MotorIsActived.Item1)
            {
                if (MotorIsActived.Item2)
                    return true;
                else
                {
                    //詢問後設定
                    //油壓馬達尚未啟動，是否要啟動油壓馬達？
                    var Result = MessageBoxResultShow.ShowYesNo((string)Application.Current.TryFindResource("Text_notify"),
                        (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorIsNotAcitved") +
                        "\r\n" +
                        (string)Application.Current.TryFindResource("Text_AskActiveHydraulicPumpMotor"));


                    if (await Result == MessageBoxResult.Yes)
                    {
                        if ( await GD_Stamping.SetHydraulicPumpMotor(true))
                        {
                            return true;
                        }
                        else
                        {
                         await   MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                                (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorAcitvedFailure"));
                        }
                    }


                }
            }

            return false;
        }








        /// <summary>
        /// 鋼印Y軸回歸原點
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetEngravingYAxisToStandbyPos()
        {
            var ret = false;
            if (GD_Stamping.IsConnected)
            {
                ret = await GD_Stamping.SetEngravingYAxisBwd(false);
                ret = await GD_Stamping.SetEngravingYAxisFwd(false);
                ret = await GD_Stamping.SetEngravingYAxisToStandbyPos();
            }
            
            return ret;
        }




        /// <summary>
        /// 鋼印X軸前進
        /// </summary>
        /// <param name="IsMove"></param>
        /// <returns></returns>
        private async Task<bool> SetFeedingXAxisFwd(bool IsMove)
        {
            var ret = false;
            if (GD_Stamping.IsConnected)
            {
                ret = await GD_Stamping.FeedingPositionBwd(false);
                ret = await GD_Stamping.FeedingPositionFwd(IsMove);
            }
            return ret;
        }
        /// <summary>
        /// 鋼印X軸後退
        /// </summary>
        /// <param name="IsMove"></param>
        /// <returns></returns>
        private async Task<bool> SetFeedingXAxisBwd(bool IsMove)
        {
            var ret = false;
            if (GD_Stamping.IsConnected)
            {
                ret = await GD_Stamping.FeedingPositionFwd(false);
                ret = await GD_Stamping.FeedingPositionBwd(IsMove);
            }
            return ret;
        }




        /// <summary>
        /// 鋼印Y軸前進
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SetEngravingYAxisFwd(bool IsMove)
        {
            var ret = false;
            if (GD_Stamping.IsConnected)
            {
                ret = await GD_Stamping.SetEngravingYAxisBwd(false);
                ret = await GD_Stamping.SetEngravingYAxisFwd(IsMove);
            }
            return ret;
        }
        /// <summary>
        /// 鋼印Y軸後退
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SetEngravingYAxisBwd(bool IsMove)
        {
            var ret = false;
            if (GD_Stamping.IsConnected)
            {
                ret = await GD_Stamping.SetEngravingYAxisFwd(false);
                ret = await GD_Stamping.SetEngravingYAxisBwd(IsMove);
            }
            return ret;
        }






        /// <summary>
        /// 鋼印旋轉 順時針
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SetEngravingRotateClockwise()
        {
            var ret = false;
            if (GD_Stamping.IsConnected)
            {
                ret = await GD_Stamping.SetEngravingRotateCW(true);
                await Task.Delay(500);
                ret = await GD_Stamping.SetEngravingRotateCW(false);
                //GD_Stamping.Disconnect();
            }
            return ret;
        }

        /// <summary>
        /// 鋼印旋轉 逆時針
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SetEngravingRotateCounterClockwise()
        {
            var ret = false;
            if (GD_Stamping.IsConnected)
            {
                ret = await GD_Stamping.SetEngravingRotateCCW(true);
                await Task.Delay(500);
                ret = await GD_Stamping.SetEngravingRotateCCW(false);
            }
            return ret;
        }

        private float _feedingVelocity = 0;
        public float FeedingVelocity
        {
            get => _feedingVelocity;
            set
            {
                _feedingVelocity = value;
                OnPropertyChanged();
            }
        }
        private float _engravingFeeding = 0;
        public float EngravingFeeding
        {
            get => _engravingFeeding;
            set
            {
                _engravingFeeding = value;
                OnPropertyChanged();
            }
        }

        private float _rotateVelocity = 0;
        public float RotateVelocity
        {
            get => _rotateVelocity;
            set
            {
                _rotateVelocity = value;
                OnPropertyChanged();
            }
        }


        private string _dataMatrixTCPIP;
        public string DataMatrixTCPIP
        {
            get => _dataMatrixTCPIP;
            set
            {
                _dataMatrixTCPIP = value;
                OnPropertyChanged();

            }
        }
        private int _dataMatrixPort;
        public int DataMatrixPort
        {
            get => _dataMatrixPort;
            set
            {
                _dataMatrixPort = value;
                OnPropertyChanged();
            }
        }


        private float _stampingPressure;
        public float StampingPressure
        {
            get => _stampingPressure;
            set
            {
                _stampingPressure = value;
                OnPropertyChanged();
            }
        }

        private float _stampingVelocity;
        public float StampingVelocity
        {
            get => _stampingVelocity;
            set
            {
                _stampingVelocity = value;
                OnPropertyChanged();
            }
        }

        private float _shearingPressure;
        public float ShearingPressure
        {
            get => _shearingPressure;
            set
            {
                _shearingPressure = value; OnPropertyChanged();
            }
        }

        private float _shearingVelocity;
        public float ShearingVelocity
        {
            get => _stampingVelocity;
            set
            {
                _stampingVelocity = value;
                OnPropertyChanged();
            }
        }



        //軸速度
        /*private float _feedingSpeed = 0;
        public float FeedingSpeed
        {
            get=> _feedingSpeed; 
            set
            {
                _feedingSpeed = value;
                OnPropertyChanged();
            }
        }*/


        private ICommand _globalSpeedChangedCommand;
        /// <summary>
        /// 全域速度設定
        /// </summary>
        public ICommand GlobalSpeedChangedCommand
        {
            get => _globalSpeedChangedCommand ??= new AsyncRelayCommand<RoutedPropertyChangedEventArgs<double>>(async e=>
            {
                    if (GD_Stamping.IsConnected)
                    {
                        bool ret = false;
                        ret = await GD_Stamping.SetFeedingSetupVelocity((float)e.NewValue);
                        ret = await GD_Stamping.SetFeedingVelocity((float)e.NewValue);

                        ret = await GD_Stamping.SetEngravingFeedingSetupVelocity((float)e.NewValue);
                        ret = await GD_Stamping.SetEngravingFeedingVelocity((float)e.NewValue);

                        ret = await GD_Stamping.SetEngravingRotateSetupVelocity((float)e.NewValue);
                        ret = await GD_Stamping.SetEngravingRotateVelocity((float)e.NewValue);
                    }
                    else
                    {
                        this.FeedingVelocity = 0;
                    }


            });
        }


        private ICommand _feedingVelocityChangedCommand;
        public ICommand FeedingVelocityChangedCommand
        {
            get => _feedingVelocityChangedCommand ??= new AsyncRelayCommand<RoutedPropertyChangedEventArgs<double>>(async e =>
            {
                if (GD_Stamping.IsConnected)
                {
                    bool ret = false;
                    ret = await GD_Stamping.SetFeedingSetupVelocity((float)e.NewValue);
                    ret = await GD_Stamping.SetFeedingVelocity((float)e.NewValue);
                }
                else
                {
                    FeedingVelocity = 0;
                }
            });
        }

        public ICommand _engravingFeedingVelocityChangedCommand;
        public ICommand EngravingFeedingVelocityChangedCommand
        {
            get => _engravingFeedingVelocityChangedCommand??= new AsyncRelayCommand<RoutedPropertyChangedEventArgs<double>>(async e =>
            {
                if (GD_Stamping.IsConnected)
                {
                    bool ret = false;
                    ret = await GD_Stamping.SetEngravingFeedingSetupVelocity((float)e.NewValue);
                    ret = await GD_Stamping.SetEngravingFeedingVelocity((float)e.NewValue);
                }
                else
                {
                    EngravingFeeding = 0;
                }
            });
        }




        public ICommand _rotateVelocityChangedCommand;
        public ICommand RotateVelocityChangedCommand
        {
            get => _rotateVelocityChangedCommand??=new AsyncRelayCommand<RoutedPropertyChangedEventArgs<double>>(async e =>
            {
                if (GD_Stamping.IsConnected)
                {
                    bool ret = false;
                    ret = await GD_Stamping.SetEngravingRotateSetupVelocity((float)e.NewValue);
                    ret = await GD_Stamping.SetEngravingRotateVelocity((float)e.NewValue);
                }
                else
                {
                    RotateVelocity = 0;
                }
            });
        }







        private AsyncRelayCommand _resetCommand;
        public AsyncRelayCommand ResetCommand
        {
            get => _resetCommand ??= new AsyncRelayCommand(async () =>
            {
                if (GD_Stamping.IsConnected)
                {
                    var ret = await GD_Stamping.Reset();
                }
            }, () => !_resetCommand.IsRunning);
        }

        private AsyncRelayCommand _cycleStartCommand;
        public AsyncRelayCommand CycleStartCommand
        {
            get => _cycleStartCommand ??= new AsyncRelayCommand(async () =>
            {
                if (GD_Stamping.IsConnected)
                {
                    var ret = await GD_Stamping.CycleStart();
                }
            }, () => !_cycleStartCommand.IsRunning);
        }



        private ObservableCollection<StampingTypeViewModel> _rotatingTurntableInfo;
        /// <summary>
        /// 字模轉盤上的字(實際)
        /// </summary>
        public ObservableCollection<StampingTypeViewModel> RotatingTurntableInfoCollection
        {
            get => _rotatingTurntableInfo??= new ObservableCollection<StampingTypeViewModel>() ; set { _rotatingTurntableInfo = value; OnPropertyChanged(); }
        }

        private AsyncObservableCollection<PlateMonitorViewModel> _machineSettingBaseCollection;
        /// <summary>
        /// 實際加工狀態[25]
        /// </summary>
        public AsyncObservableCollection<PlateMonitorViewModel> MachineSettingBaseCollection
        {
            get 
            {
                if (Debugger.IsAttached)
                {
                    _machineSettingBaseCollection ??= new AsyncObservableCollection<PlateMonitorViewModel>()
                {
                    new PlateMonitorViewModel()
                    {
                        SettingBaseVM=new  QRSettingViewModel()
                        {
                            PlateNumber = "Test1"
                        },
                        DataMatrixIsFinish = true ,
                        ShearingIsFinish = true,
                        EngravingIsFinish = true
                    },
                    new PlateMonitorViewModel()
                    {
                        SettingBaseVM=new  QRSettingViewModel()
                        {
                            PlateNumber = "Test2"
                        },
                        DataMatrixIsFinish = true ,
                        ShearingIsFinish = true,
                        EngravingIsFinish = false
                    },
                    new PlateMonitorViewModel()
                    {
                        SettingBaseVM=new  QRSettingViewModel()
                        {
                            PlateNumber = "Test3"
                        },
                        DataMatrixIsFinish = true ,
                        ShearingIsFinish = false,
                        EngravingIsFinish = false
                    },
                    new PlateMonitorViewModel()
                    {
                        SettingBaseVM=new  QRSettingViewModel()
                        {
                            PlateNumber = "Test4"
                        },
                        DataMatrixIsFinish = false ,
                        ShearingIsFinish = false,
                        EngravingIsFinish = false
                    },





                };
                }
                return _machineSettingBaseCollection ??= new AsyncObservableCollection<PlateMonitorViewModel>();
            }
            set { _machineSettingBaseCollection = value; OnPropertyChanged(); }
        }

        private IronPlateDataModel _hMIIronPlateDataModel;
        public IronPlateDataModel HMIIronPlateDataModel
        {
            get=> _hMIIronPlateDataModel??= new IronPlateDataModel();
            set
            {
                _hMIIronPlateDataModel = value;
                OnPropertyChanged();
            }
        }




        private float _feedingPosition;
        /// <summary>
        /// 進料馬達目前位置
        /// </summary>
        public float FeedingPosition
        {
            get => _feedingPosition; set { _feedingPosition = value; OnPropertyChanged(); }
        }

        private bool _cylinder_GuideRod_Move_IsUp;
        private bool _cylinder_GuideRod_Move_IsDown;
        private bool _cylinder_GuideRod_Fixed_IsUp;
        private bool _cylinder_GuideRod_Fixed_IsDown;
        private bool _cylinder_QRStamping_IsUp;
        private bool _cylinder_QRStamping_IsDown;


        private bool _cylinder_StampingSeat_IsUp;
        private bool _cylinder_StampingSeat_IsDown;
        private bool _cylinder_BlockingCylinder_IsUp;
        private bool _cylinder_BlockingCylindere_IsDown;

        private bool _cylinder_HydraulicEngraving_IsOrigin;
        private bool _cylinder_HydraulicEngraving_IsStandbyPoint;
        private bool _cylinder_HydraulicEngraving_IsStopDown;

        private bool _cylinder_HydraulicCutting_IsOrigin;
        private bool _cylinder_HydraulicCutting_IsStandbyPoint;
        private bool _cylinder_HydraulicCutting_IsCutPoint;

        private bool _hydraulicPumpIsActive;
        private bool _rdatabit;

        /// <summary>
        /// 氣壓缸1上方磁簧
        /// </summary>
        public bool Cylinder_GuideRod_Move_IsUp
        {
            get => _cylinder_GuideRod_Move_IsUp; set { _cylinder_GuideRod_Move_IsUp = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 氣壓缸1下方磁簧
        /// </summary>
        public bool Cylinder_GuideRod_Move_IsDown
        {
            get => _cylinder_GuideRod_Move_IsDown; set { _cylinder_GuideRod_Move_IsDown = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 氣壓缸2上方磁簧
        /// </summary>
        public bool Cylinder_GuideRod_Fixed_IsUp
        {
            get => _cylinder_GuideRod_Fixed_IsUp; set { _cylinder_GuideRod_Fixed_IsUp = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 氣壓缸2下方磁簧
        /// </summary>
        public bool Cylinder_GuideRod_Fixed_IsDown
        {
            get => _cylinder_GuideRod_Fixed_IsDown; set { _cylinder_GuideRod_Fixed_IsDown = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// QR壓座組上方磁簧
        /// </summary>
        public bool Cylinder_QRStamping_IsUp
        {
            get => _cylinder_QRStamping_IsUp; set { _cylinder_QRStamping_IsUp = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// QR壓座組下方磁簧
        /// </summary>
        public bool Cylinder_QRStamping_IsDown
        {
            get => _cylinder_QRStamping_IsDown; set { _cylinder_QRStamping_IsDown = value; OnPropertyChanged(); }
        }




        /// <summary>
        /// 鋼印壓座組Z軸上方磁簧
        /// </summary>
        public bool Cylinder_HydraulicEngraving_IsOrigin
        {
            get => _cylinder_HydraulicEngraving_IsOrigin; set { _cylinder_HydraulicEngraving_IsOrigin = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 鋼印壓座組中間
        /// </summary>
        public bool Cylinder_HydraulicEngraving_IsStandbyPoint
        {
            get => _cylinder_HydraulicEngraving_IsStandbyPoint; set { _cylinder_HydraulicEngraving_IsStandbyPoint = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 鋼印壓座組Z軸下方磁簧
        /// </summary>
        public bool Cylinder_HydraulicEngraving_IsStopDown
        {
            get => _cylinder_HydraulicEngraving_IsStopDown; set { _cylinder_HydraulicEngraving_IsStopDown = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 鋼印壓座組上方磁簧
        /// </summary>
        public bool Cylinder_StampingSeat_IsUp
        {
            get => _cylinder_StampingSeat_IsUp; set { _cylinder_StampingSeat_IsUp = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 鋼印壓座組下方磁簧
        /// </summary>
        public bool Cylinder_StampingSeat_IsDown
        {
            get => _cylinder_StampingSeat_IsDown; set { _cylinder_StampingSeat_IsDown = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 阻擋缸上方磁簧
        /// </summary>
        public bool Cylinder_BlockingCylinder_IsUp
        {
            get => _cylinder_BlockingCylinder_IsUp; set { _cylinder_BlockingCylinder_IsUp = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 阻擋缸下方磁簧
        /// </summary>
        public bool Cylinder_BlockingCylindere_IsDown
        {
            get => _cylinder_BlockingCylindere_IsDown; set { _cylinder_BlockingCylindere_IsDown = value; OnPropertyChanged(); }
        }



        /// <summary>
        /// 切割油壓缸上方
        /// </summary>
        public bool Cylinder_HydraulicCutting_IsOrigin
        {
            get => _cylinder_HydraulicCutting_IsOrigin; set { _cylinder_HydraulicCutting_IsOrigin = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 切割油壓缸中間
        /// </summary>
        public bool Cylinder_HydraulicCutting_IsStandbyPoint
        {
            get => _cylinder_HydraulicCutting_IsStandbyPoint; set { _cylinder_HydraulicCutting_IsStandbyPoint = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 切割油壓缸下方
        /// </summary>
        public bool Cylinder_HydraulicCutting_IsCutPoint
        {
            get => _cylinder_HydraulicCutting_IsCutPoint; set { _cylinder_HydraulicCutting_IsCutPoint = value; OnPropertyChanged(); }
        }


        /// <summary>
        /// 加工許可訊號
        /// </summary>
        public bool Rdatabit
        {
            get => _rdatabit; private set { _rdatabit = value; OnPropertyChanged(); }
        }


        /// <summary>
        /// 油壓幫浦
        /// </summary>
        public bool HydraulicPumpIsActive
        {
            get => _hydraulicPumpIsActive; private set { _hydraulicPumpIsActive = value; OnPropertyChanged(); }
        }


        private int _separateBoxIndex = -1;

        /// <summary>
        /// 分料盒編號
        /// </summary>
        public int SeparateBoxIndex
        {
            get => _separateBoxIndex; set 
            {
                try
                {
                    if (value != -1 && value != _separateBoxIndex && ParameterSettingVM != null)
                    {
                        int step;
                        if (value == 0 && _separateBoxIndex == ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.Count - 1)
                        {
                            step = 1;
                        }
                        else if (_separateBoxIndex == 0 && value == ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.Count - 1)
                        {
                            step = -1;
                        }
                        else if (value - _separateBoxIndex > 0)
                        {
                            step = 1;
                        }
                        else
                        {
                            step = -1;
                        }

                        SeparateBox_Rotate(value, step);
                    }
                }
                catch(Exception ex)
                {

                }

                _separateBoxIndex = value;
                OnPropertyChanged(); 
            }
        }


        private CancellationTokenSource cts = new CancellationTokenSource();
        private Task RorateTask;

        ParameterSettingViewModel ParameterSettingVM = Singletons.StampingMachineSingleton.Instance.ParameterSettingVM;

        private async void SeparateBox_Rotate(int IsUsingindex, int step)
        {
            if (IsUsingindex != -1)
            {
                Parallel.ForEach(ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection, obj =>
                {
                    obj.IsUsing = false;
                });

                ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection[IsUsingindex].IsUsing = true;
                //取得

                if (cts != null)
                    cts.Cancel();

                if (RorateTask != null)
                    await RorateTask;

                RorateTask = Task.Run(async () =>
                {
                    //角度比例
                    var DegreeRate = 360 / ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.Count;
                    //目標

                    //先取得目前的位置
                    var tempRotate = SeparateBox_RotateAngle;
                    //檢查正反轉
                    var endRotatePoint = 360 - DegreeRate * IsUsingindex;

                    if (step != 0)
                    {
                        while (true)
                        {
                            SeparateBox_RotateAngle -= step;
                            if (Math.Abs(SeparateBox_RotateAngle - endRotatePoint) < 2 || Math.Abs(SeparateBox_RotateAngle - endRotatePoint) > 360)
                                break;

                            if (cts.IsCancellationRequested)
                                break;
                            await Task.Delay(1);
                        }
                    }

                    if (!cts.IsCancellationRequested)
                        SeparateBox_RotateAngle = endRotatePoint;
                });
            }
        }

        private float _separateBox_RotateAngle = 0;
        /// <summary>
        /// 旋轉角
        /// </summary>
        public float SeparateBox_RotateAngle
        {
            get => _separateBox_RotateAngle;
            set
            {
                _separateBox_RotateAngle = value; OnPropertyChanged();
            }
        }

        /*object SeparateBox_Rotatelock = new();
        private bool _isRotating = false;
        public bool IsRotating
        {
            get => _isRotating; set { _isRotating = value; OnPropertyChanged(); }
        }*/

        /// <summary>
        /// 取得字模
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        public async Task<(bool, List<StampingTypeModel>)> GetRotatingTurntableInfo()
        {
            var ret = (false, new List<StampingTypeModel>());
            if (GD_Stamping.IsConnected)
            {
                ret= await GD_Stamping.GetRotatingTurntableInfo();
                //GD_Stamping.Disconnect();
            }
            return ret;
        }
        /// <summary>
        /// 設定字模
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        public async Task<bool> SetRotatingTurntableInfo(List<StampingTypeModel> Info)
        {
            var ret = false;
            if (GD_Stamping.IsConnected)
            {
                ret =await GD_Stamping.SetRotatingTurntableInfo(Info);
                //GD_Stamping.Disconnect();
            }
            return ret;
        }






























        private float _engravingYAxisPosition, _engravingZAxisPosition  = 0;
        private int _engravingRotateStation = 0;
        /// <summary>
        /// 鋼印字模Y軸位置
        /// </summary>
        public float EngravingYAxisPosition
        {
            get => _engravingYAxisPosition; set { _engravingYAxisPosition = value; OnPropertyChanged(); }
        }
        public float EngravingZAxisPosition
        {
            get => _engravingZAxisPosition; set { _engravingZAxisPosition = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 鋼印轉輪目前旋轉位置
        /// </summary>
        public int EngravingRotateStation
        {
            get => _engravingRotateStation; set { _engravingRotateStation = value; OnPropertyChanged(); }
        }




        private AsyncRelayCommand<object> _setOperationModeCommand;
        public AsyncRelayCommand<object> SetOperationModeCommand
        {
            get => _setOperationModeCommand ??=new AsyncRelayCommand<object>(async para =>
            {
                if (para is OperationModeEnum operationMode)
                {
                    await GD_Stamping.SetOperationMode(operationMode);
                }
                if (para is int operationIntMode)
                {
                    try
                    {
                        await GD_Stamping.SetOperationMode((OperationModeEnum)operationIntMode);
                    }
                    catch (Exception ex)
                    {

                    }
                }

               /* var oper=  await GD_Stamping.GetOperationMode();
                if (oper.Item1)
                    OperationMode = oper.Item2;
                else
                    OperationMode = OperationModeEnum.None;*/
                //按完之後立刻刷新按鈕狀態


            }
            , para => !SetOperationModeCommand.IsRunning);
        }



        private AsyncRelayCommand<object> _engravingRotateCommand;
        /// <summary>
        /// 旋轉到指定位置
        /// </summary>
        public AsyncRelayCommand<object> EngravingRotateCommand
        {
            get => _engravingRotateCommand??= new AsyncRelayCommand<object>(async (para,token) =>
            {
                    if (para is int paraInt)
                        if (paraInt >= 0)
                        {
                            if (GD_Stamping.IsConnected)
                            {
                                 var slots =    (await GD_Stamping.GetEngravingTotalSlots()).Item2;
                                //先決定要順轉還是逆轉
                                if (slots != 0)
                                {
                                    /*

                                    ret = await GD_Stamping.SetEngravingRotateCCW(true);
                                    await Task.Delay(500);
                                    ret = await GD_Stamping.SetEngravingRotateCCW(false);

                                    var ret = await GD_Stamping.SetEngravingRotateStation(paraInt);*/
                                }
                            }
                        }
            }, para =>!EngravingRotateCommand.IsRunning);
        }
        





        /*馬達目前位置
馬達位置移動命令
回歸基準點命令
手動前進
手動後退
磁簧訊號上限
磁簧訊號下限
手動氣壓缸升命令
手動氣壓缸降命令
磁簧訊號上限
磁簧訊號下限
手動氣壓缸升命令
手動氣壓缸降命令
磁簧訊號上限
磁簧訊號下限
手動氣壓缸升命令
手動氣壓缸降命令
磁簧訊號上限
磁簧訊號下限
手動氣壓缸升命令
手動氣壓缸降命令
磁簧訊號上限
磁簧訊號下限
手動氣壓缸升命令
手動氣壓缸降命令
切換雕刻圖形
開始/暫停/中斷
進度百分比(非必要)
Y軸馬達目前位置
Y軸馬達位置移動命令
回歸基準點命令
手動前進
手動後退
鋼印目前Z軸位置
回歸基準點命令
鋼印Z軸命令(單動/一次完整行程)
手動油壓缸升命令
手動油壓缸降命令
鋼印目前選定的字元
目前旋轉角度/或選定的字元
變更鋼印目前選定的字元命令
回歸基準點命令
手動前進
手動後退
馬達啟動
磁簧訊號上限
磁簧訊號下限
手動油壓缸升命令
手動油壓缸降命令
分料組當前位置
分料組旋轉
手動前進
手動後退
所有能控制的零件都回到原點
*/













    }
}
