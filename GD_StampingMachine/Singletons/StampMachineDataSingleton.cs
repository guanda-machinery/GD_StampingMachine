using CommunityToolkit.Mvvm.Input;
using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Scheduling.Themes;
using DevExpress.XtraPrinting.Native.Extensions;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraRichEdit.Fields;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_MachineConnect;
using GD_MachineConnect.Machine;
using GD_MachineConnect.Machine.Interfaces;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels;
using GD_StampingMachine.ViewModels.ParameterSetting;
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
    public class StampMachineDataSingleton : GD_CommonLibrary.BaseSingleton<StampMachineDataSingleton>, INotifyPropertyChanged
    {
        public const string DataSingletonName = "Name_StampMachineDataSingleton";


        private IStampingMachineConnect GD_Stamping;

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

            if (JsonHM.ReadJsonSettingByEnum(MachineSettingNameEnum.IO_Table, out ObservableCollection<IO_InfoModel> io_info_Table))
            {
                IO_TableObservableCollection = io_info_Table;
            }
            else
            {

                string opcuaNodeHeader = GD_Stamping_Opcua.StampingOpcUANode.NodeHeader;
                //讀取失敗->建立新的io表
                io_info_Table = new ObservableCollection<IO_InfoModel>
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

                IO_TableObservableCollection = io_info_Table;
                JsonHM.WriteJsonSettingByEnum(MachineSettingNameEnum.IO_Table, IO_TableObservableCollection);
            }

            init_Stamping();
        }


        private void init_Stamping()
        {
            GD_Stamping = CommunicationSetting.Protocol switch
            {
                CommunicationProtocolEnum.Opcua => new GD_Stamping_Opcua(CommunicationSetting.HostString, CommunicationSetting.Port.Value, null, CommunicationSetting.UserName, CommunicationSetting.Password),
                _ => throw new NotImplementedException()
            };
        }



        ~StampMachineDataSingleton()
        {
            this.Dispose(false);
        }

        private bool disposedValue;
        protected override void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if (disposing)
                {
                    GD_Stamping.Disconnect();

                    // TODO: 處置受控狀態 (受控物件)
                }

                GD_Stamping = null;
                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }





        private CommunicationSettingModel _communicationSetting;
        public CommunicationSettingModel CommunicationSetting
        {
            get => _communicationSetting ??= new CommunicationSettingModel();
            set => _communicationSetting = value;
        }


        OpcuaTest _opcuaTest = new OpcuaTest();
        internal async Task TestConnect()
        {
            await Task.Run(() =>
            {
                try
                {
                    var ret = _opcuaTest.OpenFormBrowseServer(null);
                }
                catch (Exception ex)
                {

                }
            });
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

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public async Task StartScanOpcua()
        {
            if (IsScaning)
                return;

            IsScaning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            await Task.Run(() => RunScanTask(_cancellationTokenSource.Token));
            IsScaning = false;
        }





        public async Task RunScanTask(CancellationToken cancelToken)
        {
            var ManagerVM = new DXSplashScreenViewModel
            {
                Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Image/svg/NewLogo_1-2.svg"),
                Status = (string)System.Windows.Application.Current.TryFindResource("Connection_Init"),
                Progress = 0,
                IsIndeterminate = true,
                Subtitle = null,
                Copyright = null,
            };

            SplashScreenManager manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.ProcessingScreenWindow(), ManagerVM);
           // await Application.Current.Dispatcher.BeginInvoke(new Action(async delegate
           // {
                manager.Show(null, WindowStartupLocation.CenterOwner, true, InputBlockMode.Window);




            IsScaning = true;
            try
            {
                ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_Init");
                await Task.Delay(2000);
                //while (true)
                int retryCount = 1;
                do
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        cancelToken.ThrowIfCancellationRequested();
                    }
                    try
                    {
                        IsConnected= await GD_Stamping.AsyncConnect();
                        if (IsConnected)
                        {
                            ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_IsSucessful");
                            ManagerVM.Subtitle = string.Empty;
                            await Task.Delay(1000);
                            break;
                        }
                        else
                        {
                            ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_RetryConnect") +$" - {retryCount}";
                            ManagerVM.Subtitle = GD_Stamping.ConnectException.Message;
                            init_Stamping();
                            //ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_IsSucessful");
                        }

                    }
                    catch(Exception ex)
                    {
                        ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_Error");
                        ManagerVM.Subtitle = ex.Message;

                    }
                    retryCount++;
                    await Task.Delay(500);
                }
                while (!IsConnected);
                //while (true) ;

                manager.Close();

                while (!cancelToken.IsCancellationRequested)
                {
                    if (cancelToken.IsCancellationRequested)
                        cancelToken.ThrowIfCancellationRequested();
                    try
                    {
                        IsConnected = await GD_Stamping.AsyncConnect();
                        //if (!IsConnected)
                        if (IsConnected)
                        {
                            manager?.Close();
                            //進料馬達

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
                            var b = await GD_Stamping.GetIronPlateDataCollection();

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
                                            var  eq = RotatingTurntableInfoCollection[i].Equals(rotatingStampingTypeVMObservableCollection[i]);
                                            StampingFontCompareBooleanList.Add(eq);
                                        }
                                        if (StampingFontCompareBooleanList.Contains(false))
                                        {
                                            RotatingTurntableInfoCollection = rotatingStampingTypeVMObservableCollection;
                                        }
                                    }
                                    catch(Exception ex)
                                    {
                                        //RotatingTurntableInfoCollection = rotatingStampingTypeVMObservableCollection;
                                    }
                                }

                            }

                            var Move_IsUp = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.GuideRod_Move, DirectionsEnum.Up);
                            if ((await Move_IsUp).Item1)
                                Cylinder_GuideRod_Move_IsUp = (await Move_IsUp).Item2;

                            var Move_IsDown = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.GuideRod_Move, DirectionsEnum.Down);
                            if ((await Move_IsDown).Item1)
                                Cylinder_GuideRod_Move_IsDown = (await Move_IsDown).Item2;

                            var Fixed_IsUp = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Up);
                            if ((await Fixed_IsUp).Item1)
                                Cylinder_GuideRod_Fixed_IsUp = (await Fixed_IsUp).Item2;

                            var Fixed_IsDown = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Down);
                            if ((await Fixed_IsDown).Item1)
                                Cylinder_GuideRod_Fixed_IsDown = (await Fixed_IsDown).Item2;

                            var QRStamping_IsUp = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.QRStamping, DirectionsEnum.Up);
                            if ((await QRStamping_IsUp).Item1)
                                Cylinder_QRStamping_IsUp = (await QRStamping_IsUp).Item2;

                            var QRStamping_IsDown = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.QRStamping, DirectionsEnum.Down);
                            if ((await QRStamping_IsDown).Item1)
                                Cylinder_QRStamping_IsDown = (await QRStamping_IsDown).Item2;

                            var StampingSeat_IsUp = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.StampingSeat, DirectionsEnum.Up);
                            if ((await StampingSeat_IsUp).Item1)
                                Cylinder_StampingSeat_IsUp = (await StampingSeat_IsUp).Item2;

                            var StampingSeat_IsDown = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.StampingSeat, DirectionsEnum.Down);
                            if ((await StampingSeat_IsDown).Item1)
                                Cylinder_StampingSeat_IsDown = (await StampingSeat_IsDown).Item2;

                            var BlockingCylinder_IsUp = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.BlockingCylinder, DirectionsEnum.Up);
                            if ((await BlockingCylinder_IsUp).Item1)
                                Cylinder_BlockingCylinder_IsUp = (await BlockingCylinder_IsUp).Item2;

                            var BlockingCylindere_IsDown = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.BlockingCylinder, DirectionsEnum.Down);
                            if ((await BlockingCylindere_IsDown).Item1)
                                Cylinder_BlockingCylindere_IsDown = (await BlockingCylindere_IsDown).Item2;

                            var HydraulicCutting_IsUp = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.HydraulicCutting, DirectionsEnum.Up);
                            if ((await HydraulicCutting_IsUp).Item1)
                                Cylinder_HydraulicCutting_IsUp = (await HydraulicCutting_IsUp).Item2;

                            var HydraulicCutting_IsDown = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.HydraulicCutting, DirectionsEnum.Down);
                            if ((await HydraulicCutting_IsDown).Item1)
                                Cylinder_HydraulicCutting_IsDown = (await HydraulicCutting_IsDown).Item2;


                            var _hydraEngraving_IsUp = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Up);
                            if ((await _hydraEngraving_IsUp).Item1)
                                HydraulicEngraving_IsUp = (await _hydraEngraving_IsUp).Item2;

                            var _hydraEngraving_IsMiddle = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Middle);
                            if ((await _hydraEngraving_IsMiddle).Item2)
                                HydraulicEngraving_IsMiddle = (await _hydraEngraving_IsMiddle).Item2;

                            var _hydraEngraving_IsDown = GD_Stamping.GetCylinderActualPosition(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Down);
                            if ((await _hydraEngraving_IsDown).Item1)
                                HydraulicEngraving_IsDown = (await _hydraEngraving_IsDown).Item2;


                            var engravingRotateStation = GD_Stamping.GetEngravingRotateStation();
                            if ((await engravingRotateStation).Item1)
                            {
                                if (Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection.TryGetValue((await engravingRotateStation).Item2, out var stamptype))
                                {
                                    Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeModel_ReadyStamping = stamptype;
                                }
                            }

                            var HPumpIsActive = GD_Stamping.GetHydraulicPumpMotor();
                            if ((await HPumpIsActive).Item1)
                                HydraulicPumpIsActive = (await HPumpIsActive).Item2;

                            var sIronPlateString = GD_Stamping.GetHMIIronPlate();
                            if ((await sIronPlateString).Item1)
                            {
                                IronPlateName1 = (await sIronPlateString).Item2.sIronPlateName1;
                                IronPlateName2 = (await sIronPlateString).Item2.sIronPlateName2;
                                //IronPlateName3 = (await sIronPlateString).Item2;
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


                            //取得io資料表
                            if (GD_Stamping is GD_Stamping_Opcua GD_StampingOpcua)
                            {
                                foreach (var IO_Table in IO_TableObservableCollection)
                                {
                                    if (!string.IsNullOrEmpty(IO_Table.NodeID))
                                    {
                                        if (IO_Table.ValueType == typeof(bool))
                                        {
                                            var nodeboolValue = await GD_StampingOpcua.ReadNode<bool>(IO_Table.NodeID);
                                            if (nodeboolValue.Item1)
                                                IO_Table.IO_Value = nodeboolValue.Item2;
                                        }
                                        else if (IO_Table.ValueType == typeof(float))
                                        {
                                            var nodefloatValue = await GD_StampingOpcua.ReadNode<float>(IO_Table.NodeID);
                                            if (nodefloatValue.Item1)
                                                IO_Table.IO_Value = nodefloatValue.Item2;
                                        }
                                        else if (IO_Table.ValueType is object)
                                        {
                                            var nodeobjectValue = await GD_StampingOpcua.ReadNode<object>(IO_Table.NodeID);
                                            if (nodeobjectValue.Item1)
                                                IO_Table.IO_Value = nodeobjectValue.Item2;
                                        }
                                        else
                                        {
                                            IO_Table.IO_Value = null;
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (manager.State == SplashScreenState.Closed)
                            {
                                manager=null;
                                manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.ProcessingScreenWindow(), ManagerVM);
                                manager.Show();
                            }

                            ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_RetryConnect");
                            foreach (var IO_Table in IO_TableObservableCollection)
                            {
                                IO_Table.IO_Value = null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        //GD_Stamping.Disconnect();
                    }
                    await Task.Delay(1000);
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("工作已取消");
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsScaning = false;
                GD_Stamping.Disconnect();
                try
                {
                    IsConnected = false;
                    manager?.Close();
                    manager =null;
                }
                catch
                {

                }
            }
        }



        public async Task StopScanOpcua()
        {
            if (!IsScaning)
                return;

            await Task.Run(async () => 
            {
                //等待掃描解除
                _cancellationTokenSource?.Cancel();

                //等待isscan 消失
                var waitScanCancellation = new CancellationTokenSource();
                var waitScan = Task.Run(async () =>
                {
                   while(IsScaning)
                    {
                       await Task.Delay(50);
                    }
                }, waitScanCancellation.Token);

                Task.WaitAny(waitScan, Task.Delay(5000));
                waitScanCancellation.Cancel();

                //回復狀態
            });
            return;
        }

        public async Task<bool> CompareFontsSettingBetweenMachineAndSoftware()
        {                                //比較鋼印字模/機台字模的字元是否相同 -> 若不同則跳出一個視窗提示
            bool FontsIsSame = false;
            try
            {

                bool isSameCount = StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection.Count == RotatingTurntableInfoCollection.Count;
               // bool hasSameContent = true;
                List<int> DifferentContent_StampingTypeNumberList = new();
                if (isSameCount)
                {
                    //
                    for (int i = 0; i < RotatingTurntableInfoCollection.Count; i++)
                    {
                        if (StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection[i].StampingTypeString != RotatingTurntableInfoCollection[i].StampingTypeString)
                        {
                            //hasSameContent = false;
                            DifferentContent_StampingTypeNumberList.Add(StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection[i].StampingTypeNumber);
                            //紀錄字模編號
                            //break;
                        }
                    }

                }
                //跳出提示
                await Task.Run(async () =>
                {
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
                                    RotatingTurntableInfoCollection.Count,
                                    StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection.Count);
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
                });
            }
            catch(Exception ex)
            {
                await MessageBoxResultShow.ShowException(ex);
            }

            return FontsIsSame;
        }


        private ObservableCollection<IO_InfoModel> _io_tableObservableCollection;
        /// <summary>
        /// IO表
        /// </summary>
        public ObservableCollection<IO_InfoModel> IO_TableObservableCollection
        {
            get=> _io_tableObservableCollection ??= new ObservableCollection<IO_InfoModel>();
            set
            {
                _io_tableObservableCollection = value;
                OnPropertyChanged();
            }
        }



        /// <summary>
        /// 進料X軸回歸原點
        /// </summary>
        public AsyncRelayCommand Feeding_XAxis_ReturnToStandbyPosition
        {
            get => new(async token=>
            {
                if (await GD_Stamping.AsyncConnect())
                {
                    await GD_Stamping.FeedingPositionReturnToStandbyPosition();
                    //GD_Stamping.Disconnect();
                }
            }, ()=> !Feeding_XAxis_ReturnToStandbyPosition.IsRunning);
        }

        /// <summary>
        /// 進料X軸移動
        /// </summary>
        public AsyncRelayCommand<object> Feeding_XAxis_MoveCommand
        {
            get => new AsyncRelayCommand<object>(async Parameter =>
            {
                await Task.Run(async () =>
                {
                    if (Parameter != null)
                    {
                        if (float.TryParse(Parameter.ToString(), out var ParameterValue))
                        {
                            if (await GD_Stamping.AsyncConnect())
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
            } , e=>!Feeding_XAxis_MoveCommand.IsRunning);
        }

        public AsyncRelayCommand Feeding_XAxis_MoveStopCommand
        {
            get => new AsyncRelayCommand(async() =>
            {
                await Task.Run(async () =>
                {
                    for (int tryCount = 0; tryCount < 10; tryCount++)
                    {
                        try
                        {
                            if (await GD_Stamping.AsyncConnect())
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
            }, ()=>!Feeding_XAxis_MoveStopCommand.IsRunning);
        }



        /// <summary>
        /// 雙導桿缸-可動端 壓座控制 夾緊/放鬆
        /// </summary>
        public ICommand CylinderControl_Up_Command
        {
            get => new AsyncRelayCommand<object>(async para =>
            {
                await Task.Run(async () =>
                {
                    if (para is StampingCylinderType stampingCylinder)
                    {
                        Set_CylinderControl(stampingCylinder, DirectionsEnum.Up);
                    }
                });
            });
        }

        
        /// <summary>
        /// 雙導桿缸-可動端 壓座控制 夾緊/放鬆
        /// </summary>
        public ICommand CylinderControl_Middle_Command
        {
            get => new AsyncRelayCommand<object>(async para =>
            {
                await Task.Run(() =>
                {
                    if (para is StampingCylinderType stampingCylinder)
                    {
                        Set_CylinderControl(stampingCylinder, DirectionsEnum.Middle);
                    }
                });
            });
        }

        /// <summary>
        /// 雙導桿缸-固定端 壓座控制 夾緊/放鬆
        /// </summary>
        public ICommand CylinderControl_Down_Command
        {
            get => new AsyncRelayCommand<object>(para =>
            {
                if (para is StampingCylinderType stampingCylinder)
                {
                    Set_CylinderControl(stampingCylinder, DirectionsEnum.Down);
                }
                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// 啟用/禁用液壓馬達
        /// </summary>
        public AsyncRelayCommand ActiveHydraulicPumpMotor
        {
            get => new AsyncRelayCommand(async () =>
            {
                await Task.Run(async () =>
                {
                    if (await GD_Stamping.AsyncConnect())
                    {
                        var isActive = await GD_Stamping.GetHydraulicPumpMotor();
                        if (isActive.Item1)
                            await GD_Stamping.SetHydraulicPumpMotor(!isActive.Item2);
                    }
                });
            });
        }


        public async Task<bool> SetHMIIronPlateData(HMIIronPlateDataModel ironPlateData)
        {
            if (await GD_Stamping.AsyncConnect())
                return await GD_Stamping.SetHMIIronPlate(ironPlateData);
            else
                return false;
        }
        public async Task<(bool, HMIIronPlateDataModel)> GetHMIIronPlateData()
        {
            if (await GD_Stamping.AsyncConnect())
                return await GD_Stamping.GetHMIIronPlate();
            else
                return (false, new HMIIronPlateDataModel());
        }

        public async Task<bool> SetIronPlateDataCollection(List<IronPlateDataModel> ironPlateDataList)
        {
            if (await GD_Stamping.AsyncConnect())
                return await GD_Stamping.SetIronPlateDataCollection(ironPlateDataList);
            else
                return false;
        }


        public async Task<(bool, List<IronPlateDataModel>)> GetIronPlateDataCollection()
        {
            if (await GD_Stamping.AsyncConnect())
                return await GD_Stamping.GetIronPlateDataCollection();
            else
                return (false, new List<IronPlateDataModel>());
        }





        /// <summary>
        /// 第三排字設定
        /// </summary>
       /* public AsyncRelayCommand<object> SetIronPlateNameCommand
        {
            get => new AsyncRelayCommand<object>(async para =>
            {
                await Task.Run(async () =>
                {
                    if (para is string ParaString)
                        await SendStampingString(sIronPlate.sIronPlateName3, ParaString);
                });
            });
        }*/

        /// <summary>
        /// 傳送空字串
        /// </summary>
        public AsyncRelayCommand SendMachiningDataCommand
        {
            get => new AsyncRelayCommand(async (CancellationToken token) =>
            {
                await Task.Run(async () =>
                {
                    await AsyncSendMachiningData(new QRSettingViewModel(), token);
                });
            },()=>!SendMachiningDataCommand.IsRunning);
        }


        public async Task<bool> GetRequestDatabit()
        {
            if (await GD_Stamping.AsyncConnect())
            {
                var ret = await GD_Stamping.GetRequestDatabit();
                if (ret.Item1)
                    return ret.Item2;
            }
            return false;
        }

        public async Task<bool> SetRequestDatabit(bool databit)
        {
            if (await GD_Stamping.AsyncConnect())
            {
                var ret = await GD_Stamping.SetRequestDatabit(databit);
                return ret;
            }
            return false;
        }






        /// <summary> 
        /// 設定金屬板的字樣
        /// </summary>
        /// <param name="settingBaseVM"></param>
        /// <param name="timeout"></param>
        /// <returns>若連接成功會等待指定毫秒數內進行加工，若無法連接或超時皆回傳false</returns>
        public async Task<bool> AsyncSendMachiningData(SettingBaseViewModel settingBaseVM, CancellationToken Ctoken, int timeout = 5000)
        {
            await Task.Delay(10);

            var ret = false;
            var spiltString = settingBaseVM.PlateNumber.SpiltByLength(settingBaseVM.SequenceCount);
            string firstLine = string.Empty;
            string secondLine = string.Empty;
            spiltString.TryGetValue(0, out firstLine);
            if (settingBaseVM.SpecialSequence == SpecialSequenceEnum.OneRow)
            {

            }
            else if (settingBaseVM.SpecialSequence == SpecialSequenceEnum.TwoRow)
            {
                spiltString.TryGetValue(1, out secondLine);
            }
           
            if (await GD_Stamping.AsyncConnect())
            {
                var databit = Task<bool>.Run(async () =>
                {
                    CancellationTokenSource tokenSource = new();
                    var token = tokenSource.Token;
                    var getRequestDatabitTask = Task<bool>.Run(async () =>
                    {
                        while (!token.IsCancellationRequested )
                        {
                            var Rdatabit = await GD_Stamping.GetRequestDatabit();
                            if (Rdatabit.Item1)
                            {
                                return Rdatabit.Item2;
                            }
                            await Task.Delay(500);
                        }
                        return false;

                    }, token);

                    _ = Task.Run(async () =>
                    {
                        while(!Ctoken.IsCancellationRequested)
                        {
                             await Task.Delay(10);
                        }
                        tokenSource.Cancel();
                    });

                    if (Task.WaitAny(Task.Delay(timeout), getRequestDatabitTask) == 0)
                    {
                        //超時
                        tokenSource.Cancel();
                    }
                    else
                    {

                    }

                    return await getRequestDatabitTask;
                });

                if (await databit)
                {
                    var sIronPlateDataRet = await GD_Stamping.GetHMIIronPlate();
                    if (sIronPlateDataRet.Item1)
                    {
                        var sIronPlateData = sIronPlateDataRet.Item2;
                        sIronPlateData.sIronPlateName1 = firstLine ??= string.Empty;
                        sIronPlateData.sIronPlateName2 = secondLine ??= string.Empty;

                        ret = await GD_Stamping.SetHMIIronPlate(sIronPlateData);
                        ret = await GD_Stamping.SetRequestDatabit(false);
                        var ret2 = await GD_Stamping.GetHMIIronPlate();
                        var ret3 = await GD_Stamping.GetRequestDatabit();
                    }
                }
                return ret;
            }
            else
                return false;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="PlateName">字串位置</param>
        /// <param name="FontString"></param>
        /// <returns></returns>
     /*   public async Task<bool> SendStampingString(GD_Stamping_Opcua.StampingOpcUANode.sIronPlate PlateName , string FontString)
        {
            bool ret = false; 
            if (await GD_Stamping.AsyncConnect())
            {
                ret = await GD_Stamping.SetHMIIronPlateName(PlateName, FontString);
                //GD_Stamping.Disconnect();
            }
            return ret;
        }*/


        private async void Set_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum Direction)
        {
            if (await GD_Stamping.AsyncConnect())
            {
                await GD_Stamping.Set_IO_CylinderControl(stampingCylinder, Direction);
                //GD_Stamping.Disconnect();
            }
        }


        public async Task<bool> SetSeparateBoxNumber(int Index)
        {
            var ret = false;
            if (await GD_Stamping.AsyncConnect())
            {
                ret = await GD_Stamping.SetSeparateBoxNumber(Index);
                //GD_Stamping.Disconnect();
            }
            return ret;
        }

        public async Task<(bool, int)> GetSeparateBoxNumber()
        {
            var ret = (false, -1);
            if (await GD_Stamping.AsyncConnect())
            {
                ret =await GD_Stamping.GetSeparateBoxNumber();
                //GD_Stamping.Disconnect();
            }
            return ret;
        }


        public AsyncRelayCommand<object> EngravingYAxisFwdCommand
        {
            get => new(async obj =>
            {
                await Task.Run(async () =>
                {
                    if (obj is bool IsActived)
                        await SetEngravingYAxisFwd(IsActived);
                });
            }, obj=> IsConnected);
        }

        public AsyncRelayCommand<object> EngravingYAxisToStandbyPosCommand
        {
            get => new AsyncRelayCommand<object>(async obj =>
            {
                await Task.Run(async () =>
                {
                    if (obj is bool IsActived)
                        await SetEngravingYAxisToStandbyPos();
                });
            }, obj => IsConnected);
        }
        public AsyncRelayCommand<object> EngravingYAxisBwdCommand
        {
            get => new (async obj =>
            {
                await Task.Run(async () =>
                {
                    if (obj is bool IsActived)
                        await SetEngravingYAxisBwd(IsActived);
                });
            }, obj => IsConnected);
        }


        

        public AsyncRelayCommand<object> SetEngravingRotateClockwiseCommand
        {
            get => new AsyncRelayCommand<object>(async obj =>
            {
                await Task.Run(async () =>
                {
                    // if (obj is bool IsActived)
                    await SetEngravingRotateClockwise();
                });
            });
        }
        public AsyncRelayCommand<object> SetEngravingRotateCounterClockwiseCommand
        {
            get => new AsyncRelayCommand<object>(async obj =>
            {
                await Task.Run(async () =>
                {
                    //  if (obj is bool IsActived)
                    await SetEngravingRotateCounterClockwise();
                });

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

            if (await GD_Stamping.AsyncConnect())
            {
                ret = await GD_Stamping.SetEngravingYAxisBwd(false);
                ret = await GD_Stamping.SetEngravingYAxisFwd(false);
                ret = await GD_Stamping.SetEngravingYAxisToStandbyPos();
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
            if (await GD_Stamping.AsyncConnect())
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
            if (await GD_Stamping.AsyncConnect())
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
            if (await GD_Stamping.AsyncConnect())
            {
                ret = await GD_Stamping.SetEngravingRotateCW();
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
            if (await GD_Stamping.AsyncConnect())
            {
                ret = await GD_Stamping.SetEngravingRotateCCW();
                //GD_Stamping.Disconnect();
            }
            return ret;
        }




        //軸速度
        private float _feedingSpeed = 5;
        public float FeedingSpeed
        {
            get=> _feedingSpeed; 
            set
            {
                _feedingSpeed = value;
                OnPropertyChanged();
            }
        }



        private ObservableCollection<StampingTypeViewModel> _rotatingTurntableInfo;
        /// <summary>
        /// 字模轉盤上的字(實際)
        /// </summary>
        public ObservableCollection<StampingTypeViewModel> RotatingTurntableInfoCollection
        {
            get => _rotatingTurntableInfo??= new ObservableCollection<StampingTypeViewModel>() ; set { _rotatingTurntableInfo = value; OnPropertyChanged(); }
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
        private bool _cylinder_HydraulicCutting_IsUp;
        private bool _cylinder_HydraulicCutting_IsDown;

        private bool _hydraulicEngraving_IsUp;
        private bool _hydraulicEngraving_IsMiddle;
        private bool _hydraulicEngraving_IsDown; 

        private bool _hydraulicPumpIsActive;
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
        public bool HydraulicEngraving_IsUp
        {
            get => _hydraulicEngraving_IsUp; set { _hydraulicEngraving_IsUp = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 鋼印壓座組中間
        /// </summary>
        public bool HydraulicEngraving_IsMiddle
        {
            get => _hydraulicEngraving_IsMiddle; set { _hydraulicEngraving_IsMiddle = value; OnPropertyChanged(); }
        }




        /// <summary>
        /// 鋼印壓座組Z軸下方磁簧
        /// </summary>
        public bool HydraulicEngraving_IsDown
        {
            get => _hydraulicEngraving_IsDown; set { _hydraulicEngraving_IsDown = value; OnPropertyChanged(); }
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
        /// 切割油壓缸上方磁簧
        /// </summary>
        public bool Cylinder_HydraulicCutting_IsUp
        {
            get => _cylinder_HydraulicCutting_IsUp; set { _cylinder_HydraulicCutting_IsUp = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 切割油壓缸下方磁簧
        /// </summary>
        public bool Cylinder_HydraulicCutting_IsDown
        {
            get => _cylinder_HydraulicCutting_IsDown; set { _cylinder_HydraulicCutting_IsDown = value; OnPropertyChanged(); }
        }





        /// <summary>
        /// 油壓幫浦
        /// </summary>
        public bool HydraulicPumpIsActive
        {
            get => _hydraulicPumpIsActive; set { _hydraulicPumpIsActive = value; OnPropertyChanged(); }
        }

        private string _ironPlateName1, _ironPlateName2, _ironPlateName3;

        /// <summary>
        /// 鋼印第一排
        /// </summary>
        public string IronPlateName1
        {
            get => _ironPlateName1; set { _ironPlateName1 = value;OnPropertyChanged(); }
        }
        /// <summary>
        /// 鋼印第二排
        /// </summary>
        public string IronPlateName2
        {
            get => _ironPlateName2; set { _ironPlateName2 = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 鋼印第三排
        /// </summary>
        /*public string IronPlateName3
        {
            get => _ironPlateName3; set { _ironPlateName3 = value; OnPropertyChanged(); }
        }*/

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
            if (await GD_Stamping.AsyncConnect())
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
            if (await GD_Stamping.AsyncConnect())
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

        /// <summary>
        /// 旋轉到指定位置
        /// </summary>
        public AsyncRelayCommand<object> EngravingRotateCommand
        {
            get => new AsyncRelayCommand<object>(async (para,token) =>
            {
                await Task.Run(async () =>
                {
                    if (para is int paraInt)
                        if (paraInt >= 0)
                        {
                            var ret = await GD_Stamping.SetEngravingRotateStation(paraInt);
                        }
                });
            }, para =>!EngravingRotateCommand.IsRunning);
        }
        
        public AsyncRelayCommand<object> EngravingRotateCWCommand
        {
            get => new AsyncRelayCommand<object>(async (para, token) =>
            {
                await Task.Run(async () =>
                {
                    await GD_Stamping.SetEngravingRotateCW();
                });
            }); 
        }
        public AsyncRelayCommand<object> EngravingRotateCCWCommand
        {
            get => new AsyncRelayCommand<object>(async (para, token) =>
            {
                await Task.Run(async () =>
                {
                    await GD_Stamping.SetEngravingRotateCCW();
                });
            });
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
