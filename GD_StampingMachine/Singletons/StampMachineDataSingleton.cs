using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Browsing;
using DevExpress.Data.Extensions;
using DevExpress.Utils.About;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpo.Logger.Transport;
using DevExpress.XtraRichEdit.Model;
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace GD_StampingMachine.Singletons
{
    public class StampMachineDataSingleton : GD_CommonLibrary.BaseSingleton<StampMachineDataSingleton>
    {
        //  public const string DataSingletonName = "Name_StampMachineDataSingleton";
        public string DataSingletonName => (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorIsNotAcitved");
        private GD_OpcUaFxClient GD_OpcUaClient = new GD_OpcUaFxClient();

        protected override void Init()
        {
            var JsonHM = new StampingMachineJsonHelper();
            if (JsonHM.ReadCommunicationSetting(out CommunicationSettingModel CSettingJson))
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
               _ = JsonHM.WriteCommunicationSettingAsync(CommunicationSetting);
            }

            if (JsonHM.ReadIO_Table(out List<IO_InfoModel> io_info_TableJson))
            {
                IO_TableObservableCollection = new ObservableCollection<IO_InfoViewModel>(
                    io_info_TableJson.Select(model => new IO_InfoViewModel(model)));
            }
            else
            {
                string opcuaNodeHeader = StampingOpcUANode.NodeHeader;
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

               _ = JsonHM.WriteIO_TableAsync(IO_TableObservableCollection);
            }

        }




        protected override async ValueTask DisposeAsyncCoreAsync()
        {
            await StopScanOpcuaAsync();
            if (GD_OpcUaClient != null) 
                await GD_OpcUaClient.DisposeAsync();
        }



        private CommunicationSettingModel _communicationSetting;
        public CommunicationSettingModel CommunicationSetting
        {
            get => _communicationSetting ??= new CommunicationSettingModel();
            set => _communicationSetting = value;
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


        public async Task StartScanOpcuaAsync()
        {
            await Task.Run(async () =>
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
                return Task.CompletedTask;
            });
        }

        public async Task StopScanOpcuaAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    //等待掃描解除
                    _cts?.Cancel();
                    await GD_OpcUaClient.DisconnectAsync();
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
                    await WaitForCondition.WaitAsync(() => IsConnected, false, new CancellationToken());
                }
                catch
                {

                }
                return Task.CompletedTask;
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
                    await GD_OpcUaClient?.DisconnectAsync();

                    GD_OpcUaClient = new GD_OpcUaFxClient(CommunicationSetting.HostString, CommunicationSetting.Port.Value, null, CommunicationSetting.UserName, CommunicationSetting.Password);
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
                            IsConnected = await GD_OpcUaClient.AsyncConnect();
                            if (IsConnected)
                            {
                                ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_IsSucessful");
                                ManagerVM.Subtitle = string.Empty;

                                var feedingVelocityTuple = await GetFeedingVelocity();
                                if (feedingVelocityTuple.Item1)
                                    this.FeedingVelocity = feedingVelocityTuple.Item2;

                                var engravingFeedingTuple = await GetEngravingFeedingVelocity();
                                if (engravingFeedingTuple.Item1)
                                    this.EngravingFeeding = engravingFeedingTuple.Item2;

                                var rotateVelocityTuple = await GetEngravingRotateVelocity();
                                if (rotateVelocityTuple.Item1)
                                    this.RotateVelocity = rotateVelocityTuple.Item2;

                                var dataMatrixTCPIPTuple = await GetDataMatrixTCPIP();
                                if (dataMatrixTCPIPTuple.Item1)
                                    this.DataMatrixTCPIP = dataMatrixTCPIPTuple.Item2;

                                var dataMatrixPortTuple = await GetDataMatrixPort();
                                if (dataMatrixPortTuple.Item1)
                                {
                                    if (int.TryParse(dataMatrixPortTuple.Item2, out var port))
                                        this.DataMatrixPort = port;
                                    this.DataMatrixPort = 0;
                                }

                                var stampingPressureTuple = await GetStampingPressure();
                                if (stampingPressureTuple.Item1)
                                    this.StampingPressure = stampingPressureTuple.Item2;

                                var stampingVelocityTuple = await GetStampingVelocity();
                                if (stampingVelocityTuple.Item1)
                                    this.StampingVelocity = stampingVelocityTuple.Item2;

                                var shearingPressureTuple = await GetShearingPressure();
                                if (shearingPressureTuple.Item1)
                                    this.ShearingPressure = shearingPressureTuple.Item2;

                                var shearingVelocityTuple = await GetShearingVelocity();
                                if (shearingVelocityTuple.Item1)
                                    this.ShearingVelocity = shearingVelocityTuple.Item2;




                                var HydraulicCutting_IsCutPoint = GetHydraulicCutting_Position_CutPoint();
                                if ((await HydraulicCutting_IsCutPoint).Item1)
                                    Cylinder_HydraulicCutting_IsCutPoint = (await HydraulicCutting_IsCutPoint).Item2;



                                //var ret7 = await GD_OpcUaClient.GetLubricationSettingTime();
                                //var ret8 = await GD_OpcUaClient.GetLubricationSettingOnTime();
                                //var ret9 = await GD_OpcUaClient.GetLubricationSettingOffTime();


                                var ret10 = await GetLubricationActualTime();
                                var ret11 = await GetLubricationActualOnTime();
                                var ret12 = await GetLubricationActualOffTime();


                                var engravingRotateSVelocity = await GetEngravingRotateSetupVelocity();
                                var engravingFeedingfeedSVelocity = await GetEngravingFeedingSetupVelocity();
                                //初始化後直接設定其他數值
                                await Task.Delay(1);

                                //檢查字模

                                await Task.Delay(1000);

                                ObservableCollection<StampingTypeViewModel> rotatingStampingTypeVMObservableCollection = new();
                                var rotatingTurntableInfoList = await GetRotatingTurntableInfo();
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
                                ManagerVM.Subtitle = GD_OpcUaClient.ConnectException?.Message;
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

                                IsConnected = await GD_OpcUaClient.AsyncConnect();
                                if (IsConnected)
                                {
                                    manager?.Close();



                                    await SubscribeOperationMode(value => OperationMode = (OperationModeEnum)value);
                                    await SubscribeHydraulicPumpMotor(value => HydraulicPumpIsActive = value);
                                    await SubscribeRequestDatabit(async value => { await Task.Delay(1000); Rdatabit = value; });



                                    await SubscribeEngravingRotateStation(value =>
                                    { 
                                        EngravingRotateStation = value;
                                        if (Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection.TryGetValue(value, out var stamptype))
                                        {
                                            Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeModel_ReadyStamping = stamptype;
                                        }
                                    });

                                    await SubscribeLastIronPlateID(value =>
                                    {
                                        try
                                        {
                                            LastIronPlateID = value;
                                        }
                                        catch
                                        {

                                        }
                                    });


                                    //切割位置
                                    await SubscribeHydraulicCutting_Position_CutPoint(value =>
                                    {
                                        try
                                        {
                                            Cylinder_HydraulicCutting_IsCutPoint = value;
                                        }
                                        catch
                                        {

                                        }
                                    });

                                    //刻字下壓
                                    await SubscribeHydraulicEngraving_Position_StopDown(value =>
                                    {
                                        try
                                        {
                                            Cylinder_HydraulicEngraving_IsStopDown = value;
                                        }
                                        catch
                                        {

                                        }
                                    });



          





                                    








                                    /*var opMode = await GD_OpcUaClient.GetOperationMode();
                                    if (opMode.Item1)
                                        OperationMode = opMode.Item2;*/

                                    //GD_OpcUaClient.GetMachineStatus
                                    var fPos = await GetFeedingPosition();
                                    if (fPos.Item1)
                                        FeedingPosition = fPos.Item2;

                                    //磁簧開關
                                    /*var Move_IsUp = Task.Run(() => 
                                    {
                                        if(GD_OpcUaClient.GetCylinderActualPosition(StampingCylinderType.GuideRod_Move, DirectionsEnum.Up, out var move_IsUp))
                                            return move_IsUp;
                                        else
                                            return false;
                                    });
                                    Cylinder_GuideRod_Move_IsUp = await Move_IsUp;*/
                                    var PlateDataCollectionTask = await GetIronPlateDataCollection();
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
                                                    PlateNumber = plateData.sIronPlateName1.PadRight(rowLength).Substring(0, rowLength) + plateData.sIronPlateName2,
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
                                                              //比較差異 若兩個ID相同則不複寫
                                                if (MachineSettingBaseCollection[index].SettingBaseVM.PlateNumber != plateMonitorVMCollection[index].SettingBaseVM.PlateNumber
                                                || MachineSettingBaseCollection[index].StampingStatus != plateMonitorVMCollection[index].StampingStatus
                                                || MachineSettingBaseCollection[index].DataMatrixIsFinish != plateMonitorVMCollection[index].DataMatrixIsFinish
                                                || MachineSettingBaseCollection[index].EngravingIsFinish != plateMonitorVMCollection[index].EngravingIsFinish
                                                || MachineSettingBaseCollection[index].ShearingIsFinish != plateMonitorVMCollection[index].ShearingIsFinish)              
                                                {
                                                    var invoke = Application.Current?.Dispatcher.InvokeAsync(async () =>
                                                    {
                                                        await Task.Delay(1);
                                                    });
                                                    invokeList.Add(invoke);
                                                }
                                            }
                                            await Task.WhenAll(invokeList?.Select(op => op.Task) ?? Enumerable.Empty<Task>());
                                            if(  invokeList.Count > 0)
                                            {
                                                MachineSettingBaseCollection = plateMonitorVMCollection;
                                            }

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
                                                    try
                                                    {
                                                        await projectDistribute.SaveProductProjectVMObservableCollectionAsync();
                                                    }
                                                    catch
                                                    {

                                                    }
                                                    break;
                                                }
                                            }


                                        }
                                        //比較新舊兩個加工陣列
                                        //先檢查新陣列的id是否只有0 若只有0代表是被重新設定 不設定為完成(若有需要則另外設定)
                                    /*    if (newlronDataIList.Count(x => x != 0) > 0)
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
                                        lastIronDataIList = newlronDataIList;*/
                                    }


                                    var HmiIronPlateTask = await GetHMIIronPlate();
                                    if (HmiIronPlateTask.Item1)
                                    {
                                        HMIIronPlateDataModel = HmiIronPlateTask.Item2;
                                    }

                                    var rotatingTurntableInfoList = await GetRotatingTurntableInfo();
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

                                    var Move_IsUp = GetGuideRod_Move_Position_isUp();
                                    if ((await Move_IsUp).Item1)
                                        Cylinder_GuideRod_Move_IsUp = (await Move_IsUp).Item2;

                                    var Move_IsDown = GetGuideRod_Move_Position_isDown();
                                    if ((await Move_IsDown).Item1)
                                        Cylinder_GuideRod_Move_IsDown = (await Move_IsDown).Item2;

                                    var Fixed_IsUp = GetGuideRod_Fixed_Position_isUp();
                                    if ((await Fixed_IsUp).Item1)
                                        Cylinder_GuideRod_Fixed_IsUp = (await Fixed_IsUp).Item2;

                                    var Fixed_IsDown = GetGuideRod_Fixed_Position_isDown();
                                    if ((await Fixed_IsDown).Item1)
                                        Cylinder_GuideRod_Fixed_IsDown = (await Fixed_IsDown).Item2;

                                    var QRStamping_IsUp = GetQRStamping_Position_isUp();
                                    if ((await QRStamping_IsUp).Item1)
                                        Cylinder_QRStamping_IsUp = (await QRStamping_IsUp).Item2;

                                    var QRStamping_IsDown = GetQRStamping_Position_isDown();
                                    if ((await QRStamping_IsDown).Item1)
                                        Cylinder_QRStamping_IsDown = (await QRStamping_IsDown).Item2;

                                    var StampingSeat_IsUp = GetStampingSeat_Position_isUp();
                                    if ((await StampingSeat_IsUp).Item1)
                                        Cylinder_StampingSeat_IsUp = (await StampingSeat_IsUp).Item2;

                                    var StampingSeat_IsDown = GetStampingSeat_Position_isDown();
                                    if ((await StampingSeat_IsDown).Item1)
                                        Cylinder_StampingSeat_IsDown = (await StampingSeat_IsDown).Item2;

                                    var BlockingCylinder_IsUp = GetBlockingCylinder_Position_isUp();
                                    if ((await BlockingCylinder_IsUp).Item1)
                                        Cylinder_BlockingCylinder_IsUp = (await BlockingCylinder_IsUp).Item2;

                                    var BlockingCylindere_IsDown = GetBlockingCylinder_Position_isDown();
                                    if ((await BlockingCylindere_IsDown).Item1)
                                        Cylinder_BlockingCylindere_IsDown = (await BlockingCylindere_IsDown).Item2;



                                    var HydraulicEngraving_IsOrigin = GetHydraulicEngraving_Position_Origin();
                                    if ((await HydraulicEngraving_IsOrigin).Item1)
                                        Cylinder_HydraulicEngraving_IsOrigin = (await HydraulicEngraving_IsOrigin).Item2;

                                    var HydraulicEngraving_IsStandbyPoint = GetHydraulicEngraving_Position_StandbyPoint();
                                    if ((await HydraulicEngraving_IsStandbyPoint).Item1)
                                        Cylinder_HydraulicEngraving_IsStandbyPoint = (await HydraulicEngraving_IsStandbyPoint).Item2;

                                    var HydraulicEngraving_IsStopDown = GetHydraulicEngraving_Position_StopDown();
                                    if ((await HydraulicEngraving_IsStopDown).Item1)
                                        Cylinder_HydraulicEngraving_IsStopDown = (await HydraulicEngraving_IsStopDown).Item2;


                                    var HydraulicCutting_IsOrigin = GetHydraulicCutting_Position_Origin();
                                    if ((await HydraulicCutting_IsOrigin).Item1)
                                        Cylinder_HydraulicCutting_IsOrigin = (await HydraulicCutting_IsOrigin).Item2;

                                    var HydraulicCutting_IsStandbyPoint = GetHydraulicCutting_Position_StandbyPoint();
                                    if ((await HydraulicCutting_IsStandbyPoint).Item1)
                                        Cylinder_HydraulicCutting_IsStandbyPoint = (await HydraulicCutting_IsStandbyPoint).Item2;

                                    var HydraulicCutting_IsCutPoint = GetHydraulicCutting_Position_CutPoint();
                                    if ((await HydraulicCutting_IsCutPoint).Item1)
                                        Cylinder_HydraulicCutting_IsCutPoint = (await HydraulicCutting_IsCutPoint).Item2;




                                    var engravingRotateStation = await GetEngravingRotateStation();
                                    if ( engravingRotateStation.Item1)
                                    {
                                        EngravingRotateStation = engravingRotateStation.Item2;
                                       /* if (Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection.TryGetValue((await engravingRotateStation).Item2, out var stamptype))
                                        {
                                            Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeModel_ReadyStamping = stamptype;
                                        }*/
                                    }


                                    //箱子
                                    var boxIndex = GetSeparateBoxNumber();
                                    if ((await boxIndex).Item1)
                                    {
                                        SeparateBoxIndex = (await boxIndex).Item2;
                                    }

                                    var engravingYposition = GetEngravingYAxisPosition();
                                    if ((await engravingYposition).Item1)
                                        EngravingYAxisPosition = (await engravingYposition).Item2;

                                    var engravingZposition = GetEngravingZAxisPosition();
                                    if ((await engravingZposition).Item1)
                                        EngravingZAxisPosition = (await engravingZposition).Item2;

                                    var engravingAStation = GetEngravingRotateStation();
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
                                await GD_OpcUaClient.DisconnectAsync();
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
                                var Rtask = await GD_OpcUaClient.AsyncReadNode<object>(IO_Table.NodeID);
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

                       var ioList =(await GD_OpcUaClient.SubscribeNodesDataChangeAsync(ioUpdateNodes)).ToList();



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
                                    IO_Table.IO_Value = GD_OpcUaClient.ReadNode<bool>(IO_Table.NodeID);
                                }
                                else if (IO_Table.ValueType == typeof(float))
                                {
                                    IO_Table.IO_Value = GD_OpcUaClient.ReadNode<float>(IO_Table.NodeID);
                                }
                                else if (IO_Table.ValueType is object)
                                {
                                    IO_Table.IO_Value = GD_OpcUaClient.ReadNode<object>(IO_Table.NodeID);
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
                    await DisconnectAsync();
                    Console.WriteLine("工作已取消");
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    await DisconnectAsync();
                    await WaitForCondition.WaitAsync(() => IsConnected, false,  cancelToken);
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
                            await MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
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

                            await MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
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
                    await MessageBoxResultShow.ShowExceptionAsync(ex);
                }
            }
            catch (Exception ex)
            {
                await MessageBoxResultShow.ShowExceptionAsync(ex);
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
                if (await GD_OpcUaClient.AsyncConnect())
                {
                    await GD_OpcUaClient.FeedingPositionReturnToStandbyPosition();
                    //GD_OpcUaClient.Disconnect();
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
                            if (await GD_OpcUaClient.AsyncConnect())
                            {
                                if (ParameterValue > 0)
                                {
                                    await FeedingPositionFwd(true);
                                    await Task.Delay(500);
                                    await FeedingPositionFwd(false);
                                }
                                else if (ParameterValue < 0)
                                {
                                    await FeedingPositionBwd(true);
                                    await Task.Delay(500);
                                    await FeedingPositionBwd(false);
                                }
                                else
                                {
                                    Debugger.Break();
                                }
                                /*if (GD_OpcUaClient.GetFeedingPosition(out var FPosition))
                                {

                                    GD_OpcUaClient.SetFeedingPosition(FPosition + ParameterValue);



                                    await Task.Delay(2000);

                                    GD_OpcUaClient.GetFeedingPosition(out var FPosition2);
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
                        if (await GD_OpcUaClient.AsyncConnect())
                        {
                            var ret1 = await FeedingPositionFwd(false);
                            var ret2 = await FeedingPositionBwd(false);
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
                    if (await AsyncConnect())
                        await Set_IO_CylinderControl(StampingCylinderType.GuideRod_Move, DirectionsEnum.Up, isTriggered);
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
                if (await GD_OpcUaClient.AsyncConnect())
                {
                    await Set_IO_CylinderControl(StampingCylinderType.GuideRod_Move, DirectionsEnum.Down, true);
                    await Task.Delay(500);
                    await Set_IO_CylinderControl(StampingCylinderType.GuideRod_Move, DirectionsEnum.Down, false);
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
                if (await GD_OpcUaClient.AsyncConnect())
                {
                    await Set_IO_CylinderControl(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Up, true);
                    await Task.Delay(500);
                    await Set_IO_CylinderControl(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Up, false);

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
                if (await AsyncConnect())
                {
                    await Set_IO_CylinderControl(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Down, true);
                    await Task.Delay(500);
                    await Set_IO_CylinderControl(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Down, false);
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
                if (await GD_OpcUaClient.AsyncConnect())
                {
                    await Set_IO_CylinderControl(StampingCylinderType.QRStamping, DirectionsEnum.Up, true);
                    await Task.Delay(500);
                    await Set_IO_CylinderControl(StampingCylinderType.QRStamping, DirectionsEnum.Up, false);
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
                if (await GD_OpcUaClient.AsyncConnect())
                {
                    await Set_IO_CylinderControl(StampingCylinderType.QRStamping, DirectionsEnum.Down, true);
                    await Task.Delay(500);
                    await Set_IO_CylinderControl(StampingCylinderType.QRStamping, DirectionsEnum.Down, false);
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
                if (await GD_OpcUaClient.AsyncConnect())
                {
                    await Set_IO_CylinderControl(StampingCylinderType.StampingSeat, DirectionsEnum.Up, true);
                    await Task.Delay(500);
                    await Set_IO_CylinderControl(StampingCylinderType.StampingSeat, DirectionsEnum.Up, false);
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
                if (await GD_OpcUaClient.AsyncConnect())
                {
                    await Set_IO_CylinderControl(StampingCylinderType.StampingSeat, DirectionsEnum.Down, true);
                    await Task.Delay(500);
                    await Set_IO_CylinderControl(StampingCylinderType.StampingSeat, DirectionsEnum.Down, false);
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
                    if (await GD_OpcUaClient.AsyncConnect())
                    {
                        await Set_IO_CylinderControl(StampingCylinderType.BlockingCylinder, DirectionsEnum.Up, true);
                        await Task.Delay(500);
                        await Set_IO_CylinderControl(StampingCylinderType.BlockingCylinder, DirectionsEnum.Up, false);
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
                if (await GD_OpcUaClient.AsyncConnect())
                {
                    await Set_IO_CylinderControl(StampingCylinderType.BlockingCylinder, DirectionsEnum.Down, true);
                    await Task.Delay(500);
                    await Set_IO_CylinderControl(StampingCylinderType.BlockingCylinder, DirectionsEnum.Down, false);
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
                    if (await GD_OpcUaClient.AsyncConnect())
                    {
                        await Set_IO_CylinderControl(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Up, tirggered);
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
                    if (await GD_OpcUaClient.AsyncConnect())
                    {
                        await Set_IO_CylinderControl(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Down, tirggered);
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
                        if (await GD_OpcUaClient.AsyncConnect())
                            await Set_IO_CylinderControl(StampingCylinderType.HydraulicCutting, DirectionsEnum.Up, isTriggered);
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
                    if (await GD_OpcUaClient.AsyncConnect())
                        await Set_IO_CylinderControl(StampingCylinderType.HydraulicCutting, DirectionsEnum.Down, isTriggered);
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
                if (await GD_OpcUaClient.AsyncConnect())
                {
                    var isActived = await GetHydraulicPumpMotor();
                    if (isActived.Item1)
                    {
                        await SetHydraulicPumpMotor(!isActived.Item2);
                    }
                }

            },()=> !_activeHydraulicPumpMotor.IsRunning);
        }

        public async Task<bool> SetHMIIronPlateData(IronPlateDataModel ironPlateData)
        {
            if (await GD_OpcUaClient.AsyncConnect())
            {
                /*
                await GD_OpcUaClient.SetDataMatrixMode(
                    !string.IsNullOrEmpty(ironPlateData.sDataMatrixName1) 
                    || !string.IsNullOrEmpty(ironPlateData.sDataMatrixName2));
                */
                await SetDataMatrixMode(true);
                return await SetHMIIronPlate(ironPlateData);
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
            if (await GD_OpcUaClient.AsyncConnect())
                return await GetHMIIronPlate();
            else
                return (false, new IronPlateDataModel());
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
                    if (await GD_OpcUaClient.AsyncConnect())
                    {
                        var MotorIsActived = await GetHydraulicPumpMotor();
                        if (MotorIsActived.Item1)
                        {
                            if (!MotorIsActived.Item2)
                            {
                                //油壓馬達尚未啟動
                                var Result = MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                                    (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorIsNotAcitved"));

                                LogDataSingleton.Instance.AddLogData(this.DataSingletonName, (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorIsNotAcitved"));

                                throw new Exception();
                            }
                            else
                            {
                                var opMode = await GetOperationMode();
                                if (opMode.Item1)
                                {
                                    //OperationMode = opMode.Item2;
                                    //要在工程模式
                                    if (opMode.Item2 != OperationModeEnum.Setup)
                                    {
                                        //需在工程模式才可執行
                                        var Result = MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                                            (string)Application.Current.TryFindResource("Text_MachineNotInSetupMode"));

                                        LogDataSingleton.Instance.AddLogData(this.DataSingletonName, (string)Application.Current.TryFindResource("Text_MachineNotInSetupMode"));
                                    }
                                    else
                                    {
                                        if (await GD_OpcUaClient.AsyncConnect())
                                        {

                                            bool EngravingToOriginSucessful;
                                            if ((await GetHydraulicEngraving_Position_Origin()).Item2)
                                            {
                                                EngravingToOriginSucessful = true;
                                            }
                                            else
                                            {
                                                await Set_IO_CylinderControl(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Up, true);
                                                var EngravingToOriginTask = Task.Run(async () =>
                                                {
                                                    try
                                                    {
                                                        var EngravingIsOrigin = false;
                                                        do
                                                        {
                                                            if (token.IsCancellationRequested)
                                                                token.ThrowIfCancellationRequested();
                                                            EngravingIsOrigin = (await GetHydraulicEngraving_Position_Origin()).Item2;
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
                                                await Set_IO_CylinderControl(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Up, false);
                                                EngravingToOriginSucessful = completedTask == EngravingToOriginTask;
                                            }


                                            if (!EngravingToOriginSucessful)
                                            {
                                                var Result = MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                                                    (string)Application.Current.TryFindResource("Text_EngravingToOriginTimeout"));
                                                return;
                                            }

                                            bool CuttungToOriginSucessful;

                                            //先檢查是否在原點
                                            if ((await GetHydraulicCutting_Position_Origin()).Item2)
                                            {
                                                CuttungToOriginSucessful = true;
                                            }
                                            else
                                            {

                                                await Set_IO_CylinderControl(StampingCylinderType.HydraulicCutting, DirectionsEnum.Up, true);
                                                var CuttingToOriginTask = Task.Run(async () =>
                                                {
                                                    try
                                                    {
                                                        var CuttingIsOrigin = false;
                                                        do
                                                        {
                                                            if (token.IsCancellationRequested)
                                                                token.ThrowIfCancellationRequested();

                                                            CuttingIsOrigin = (await GetHydraulicCutting_Position_Origin()).Item2;
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
                                                await Set_IO_CylinderControl(StampingCylinderType.HydraulicCutting, DirectionsEnum.Up, false);
                                                CuttungToOriginSucessful = completedTask == CuttingToOriginTask;
                                            }

                                            if (!CuttungToOriginSucessful)
                                            {
                                                var Result = MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                                                    (string)Application.Current.TryFindResource("Text_CuttingToOriginTimeout"));
                                                //超時
                                                return;
                                            }

                                            //X軸回歸
                                            var ret = await FeedingPositionReturnToStandbyPosition();

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
                                                                isHome = (await GetServoHome()).Item2;
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
                                                            var getHome = await GetServoHome();
                                                            if (getHome.Item1)
                                                            {
                                                                if (!getHome.Item2)
                                                                {
                                                                    var feedingPosition = await GetFeedingPosition();
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
                                                            await MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                                                                 (string)Application.Current.TryFindResource("Text_ToOriginisSuccessful"));
                                                        }
                                                        else
                                                        {
                                                            await MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                                                                 (string)Application.Current.TryFindResource("Text_ToOriginisFail"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        await MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                                                            (string)Application.Current.TryFindResource("Text_ToOriginisTimeout"));
                                                    }
                                                }
                                                catch (OperationCanceledException cex)
                                                {
                                                    await MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                                                        (string)Application.Current.TryFindResource("Text_ToOriginisCancel"));
                                                }
                                                catch (Exception ex)
                                                {
                                                    await MessageBoxResultShow.ShowExceptionAsync(ex);
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
                        await MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
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




        public async Task<bool> SetSeparateBoxNumber(int Index)
        {
            var ret = false;
            if (await GD_OpcUaClient.AsyncConnect())
            {
                return await GD_OpcUaClient.AsyncWriteNode<int>(StampingOpcUANode.Stacking1.sv_iThisStation, Index);
                //GD_OpcUaClient.Disconnect();
            }
            return ret;
        }

        public async Task<(bool, int)> GetSeparateBoxNumber()
        {
            var ret = (false, -1);
            if (await GD_OpcUaClient.AsyncConnect())
            {
                return await GD_OpcUaClient.AsyncReadNode<int>(StampingOpcUANode.Stacking1.sv_iThisStation);
                //GD_OpcUaClient.Disconnect();
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
                  


                    if (!isActived && (await GetOperationMode()).Item2 == OperationModeEnum.Manual)
                        await Task.Delay(200);
                    await SetEngravingRotateCW(isActived);
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
                    if (!isActived && (await GetOperationMode()).Item2 == OperationModeEnum.Manual)
                        await Task.Delay(200);
                    await SetEngravingRotateCCW(isActived);
                }
            });
        }


        private async Task<bool> CheckHydraulicPumpMotor()
        {
            var MotorIsActived = await GetHydraulicPumpMotor();
            if (MotorIsActived.Item1)
            {
                if (MotorIsActived.Item2)
                    return true;
                else
                {
                    //詢問後設定
                    //油壓馬達尚未啟動，是否要啟動油壓馬達？
                    var Result = MessageBoxResultShow.ShowYesNoAsync((string)Application.Current.TryFindResource("Text_notify"),
                        (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorIsNotAcitved") +
                        "\r\n" +
                        (string)Application.Current.TryFindResource("Text_AskActiveHydraulicPumpMotor"));


                    if (await Result == MessageBoxResult.Yes)
                    {
                        if ( await SetHydraulicPumpMotor(true))
                        {
                            return true;
                        }
                        else
                        {
                         await   MessageBoxResultShow.ShowOKAsync((string)Application.Current.TryFindResource("Text_notify"),
                                (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorAcitvedFailure"));
                        }
                    }


                }
            }

            return false;
        }












        /// <summary>
        /// 鋼印X軸前進
        /// </summary>
        /// <param name="IsMove"></param>
        /// <returns></returns>
        private async Task<bool> SetFeedingXAxisFwd(bool IsMove)
        {
            var ret = false;
            if (await GD_OpcUaClient.AsyncConnect())
            {
                ret = await FeedingPositionBwd(false);
                ret = await FeedingPositionFwd(IsMove);
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
            if (await GD_OpcUaClient.AsyncConnect())
            {
                ret = await FeedingPositionFwd(false);
                ret = await FeedingPositionBwd(IsMove);
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
            if (await GD_OpcUaClient.AsyncConnect())
            {
                ret = await SetEngravingRotateCW(true);
                await Task.Delay(500);
                ret = await SetEngravingRotateCW(false);
                //GD_OpcUaClient.Disconnect();
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
            if (await GD_OpcUaClient.AsyncConnect())
            {
                ret = await SetEngravingRotateCCW(true);
                await Task.Delay(500);
                ret = await SetEngravingRotateCCW(false);
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
                    if (await AsyncConnect())
                    {
                        bool ret = false;
                        ret = await SetFeedingSetupVelocity((float)e.NewValue);
                        ret = await SetFeedingVelocity((float)e.NewValue);

                        ret = await SetEngravingFeedingSetupVelocity((float)e.NewValue);
                        ret = await SetEngravingFeedingVelocity((float)e.NewValue);

                        ret = await SetEngravingRotateSetupVelocity((float)e.NewValue);
                        ret = await SetEngravingRotateVelocity((float)e.NewValue);
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
                if (await GD_OpcUaClient.AsyncConnect())
                {
                    bool ret = false;
                    ret = await SetFeedingSetupVelocity((float)e.NewValue);
                    ret = await SetFeedingVelocity((float)e.NewValue);
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
                if (await GD_OpcUaClient.AsyncConnect())
                {
                    bool ret = false;
                    ret = await SetEngravingFeedingSetupVelocity((float)e.NewValue);
                    ret = await SetEngravingFeedingVelocity((float)e.NewValue);
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
                if (await GD_OpcUaClient.AsyncConnect())
                {
                    bool ret = false;
                    ret = await SetEngravingRotateSetupVelocity((float)e.NewValue);
                    ret = await SetEngravingRotateVelocity((float)e.NewValue);
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
                if (await AsyncConnect())
                {
                    var ret = await Reset();
                }
            }, () => !_resetCommand.IsRunning);
        }

        private AsyncRelayCommand _cycleStartCommand;
        public AsyncRelayCommand CycleStartCommand
        {
            get => _cycleStartCommand ??= new AsyncRelayCommand(async () =>
            {
                if (await AsyncConnect())
                {
                    var ret = await CycleStart();
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
                    if (_machineSettingBaseCollection == null)
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
                    }

                        };

                        for(int i=0;i<10;i++)
                        {
                            _machineSettingBaseCollection.Add(new PlateMonitorViewModel()
                            {
                                SettingBaseVM = new QRSettingViewModel()
                            });
                        }

                    }



                }
                


                return _machineSettingBaseCollection ??= new AsyncObservableCollection<PlateMonitorViewModel>();
            }
            set { _machineSettingBaseCollection = value; 
                OnPropertyChanged(); }
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
        /// 鋼印固定座上方氣壓缸磁簧
        /// </summary>
        public bool Cylinder_StampingSeat_IsUp
        {
            get => _cylinder_StampingSeat_IsUp; set { _cylinder_StampingSeat_IsUp = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 鋼印固定座下方氣壓缸磁簧
        /// </summary>
        public bool Cylinder_StampingSeat_IsDown
        {
            get => _cylinder_StampingSeat_IsDown; set { _cylinder_StampingSeat_IsDown = value; OnPropertyChanged(); }
        }




        /// <summary>
        /// 鋼印壓座組Z軸油壓缸上方磁簧
        /// </summary>
        public bool Cylinder_HydraulicEngraving_IsOrigin
        {
            get => _cylinder_HydraulicEngraving_IsOrigin; set { _cylinder_HydraulicEngraving_IsOrigin = value; OnPropertyChanged(); }
        }






        /// <summary>
        /// 鋼印壓座組油壓缸中間
        /// </summary>
        public bool Cylinder_HydraulicEngraving_IsStandbyPoint
        {
            get => _cylinder_HydraulicEngraving_IsStandbyPoint; set { _cylinder_HydraulicEngraving_IsStandbyPoint = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 鋼印壓座組Z軸油壓缸下方磁簧
        /// </summary>
        public bool Cylinder_HydraulicEngraving_IsStopDown
        {
            get => _cylinder_HydraulicEngraving_IsStopDown; set { _cylinder_HydraulicEngraving_IsStopDown = value; OnPropertyChanged(); }
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

        private int _lastIronPlateID = 0;
        /// <summary>
        /// 最後一片的id
        /// </summary>
        public int LastIronPlateID
        {
            get => _lastIronPlateID; private set { _lastIronPlateID = value; OnPropertyChanged(); }
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
        /// 設定字模
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        public async Task<bool> SetRotatingTurntableInfoAsync(List<StampingTypeModel> Info)
        {
            var ret = false;
            if (await GD_OpcUaClient.AsyncConnect())
            {
                ret =await SetRotatingTurntableInfo(Info);
                //GD_OpcUaClient.Disconnect();
            }
            return ret;
        }


        /// <summary>
        /// 取得第一片id
        /// </summary>
        /// <returns></returns>
        public async Task<(bool,int)> GetFirstIronPlateID()
        {
            if (await GD_OpcUaClient.AsyncConnect())
            {
                return await GD_OpcUaClient.AsyncReadNode<int>($"{StampingOpcUANode.system.sv_IronPlateData}[1].iIronPlateID");
            }
            return (false, 0);
        }

        /// <summary>
        /// 取得最後一片id
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, int)> GetLastIronPlateID()
        {
            if (await GD_OpcUaClient.AsyncConnect())
            {
                return await GD_OpcUaClient.AsyncReadNode<int>($"{StampingOpcUANode.system.sv_IronPlateData}[24].iIronPlateID");
         
            }
            return (false, 0);
        }

        /// <summary>
        /// 訂閱最後一片id
        /// </summary>
        /// <param name="action"></param>
        public async Task<bool> SubscribeLastIronPlateID(Action<int> action)
        {
            return await GD_OpcUaClient.SubscribeNodeDataChangeAsync<int>($"{StampingOpcUANode.system.sv_IronPlateData}[24].iIronPlateID", action, 200, true);
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
                    await SetOperationMode(operationMode);
                }
                if (para is int operationIntMode)
                {
                    try
                    {
                        await SetOperationMode((OperationModeEnum)operationIntMode);
                    }
                    catch (Exception ex)
                    {

                    }
                }

               /* var oper=  await GD_OpcUaClient.GetOperationMode();
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
                            if (await GD_OpcUaClient.AsyncConnect())
                            {
                                 var slots =    (await GetEngravingTotalSlots()).Item2;
                                //先決定要順轉還是逆轉
                                if (slots != 0)
                                {
                                    /*

                                    ret = await GD_OpcUaClient.SetEngravingRotateCCW(true);
                                    await Task.Delay(500);
                                    ret = await GD_OpcUaClient.SetEngravingRotateCCW(false);

                                    var ret = await GD_OpcUaClient.SetEngravingRotateStation(paraInt);*/
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





        private bool disposedValue;


        public override async ValueTask DisposeAsync()
        {
            if (!disposedValue)
            {
                await GD_OpcUaClient.DisposeAsync();
                disposedValue = true;
            }

        }





        /// <summary>
        /// 建立連線
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AsyncConnect()
        {
            return await GD_OpcUaClient .AsyncConnect();
        }

        public Exception ConnectException { get => GD_OpcUaClient.ConnectException; }



        public async Task DisconnectAsync()
        {
            await GD_OpcUaClient?.DisconnectAsync();
        }










        public async Task<bool> FeedingPositionBwd(bool Active)
        {
            await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_bButtonFwd, false);
            return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_bButtonBwd, Active);
        }

        public async Task<bool> FeedingPositionFwd(bool Active)
        {
            await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_bButtonBwd, false);
            return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_bButtonFwd, Active);
        }

        public async Task<bool> FeedingPositionReturnToStandbyPosition()
        {
            await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_bUseHomeing, true);
            await Task.Delay(500);
            return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_bUseHomeing, false);
        }




        /// <summary>
        /// 訂閱機台模式
        /// </summary>
        /// <param name="action"></param>
        public async Task<bool> SubscribeOperationMode(Action<int> action)
        {
            return await GD_OpcUaClient.SubscribeNodeDataChangeAsync<int>(StampingOpcUANode.system.sv_OperationMode, action, 200, true);
        }





        /// <summary>
        /// 取得機台狀態
        /// </summary>
        public async Task<(bool, OperationModeEnum)> GetOperationMode()
        {
            var ret = await GD_OpcUaClient.AsyncReadNode<int>(StampingOpcUANode.system.sv_OperationMode);
            return (ret.Item1, (OperationModeEnum)ret.Item2);
        }
        /// <summary>
        /// 設定機台狀態
        /// </summary>
        public async Task<bool> SetOperationMode(OperationModeEnum operationMode)
        {
            bool ret = false;
            if (GD_OpcUaClient.IsConnected)
            {
                switch (operationMode)
                {
                    case OperationModeEnum.Setup:
                        ret = await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonSetup, true);
                        break;
                    case OperationModeEnum.Manual:
                        ret = await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonManual, true);
                        break;
                    case OperationModeEnum.HalfAutomatic:
                        ret = await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonHalfAuto, true);
                        break;
                    case OperationModeEnum.FullAutomatic:
                        ret = await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonFullAuto, true);
                        break;
                    default:
                        break;
                }
                await Task.Delay(100);
                await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonManual, false);
                await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonSetup, false);
                await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonHalfAuto, false);
                await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonFullAuto, false);
            }
            return ret;
        }

        /// <summary>
        /// 取得X軸是否在原點
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetServoHome()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Feeding1.di_ServoHome);
        }



        public async Task<(bool, float)> GetFeedingPosition()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>(StampingOpcUANode.Feeding1.sv_rFeedingPosition);
        }




        public async Task<bool> Reset()
        {
            await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonAlarmConfirm, true);
            await Task.Delay(100);
            return await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonAlarmConfirm, false);
        }

        public async Task<bool> CycleStart()
        {
            await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonCycleStart, true);
            await Task.Delay(100);
            return await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonCycleStart, false);
        }



        public async Task<bool> Set_IO_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum direction, bool IsTrigger)
        {
            bool ret = false;
            if (await AsyncConnect())
            {
                switch (stampingCylinder)
                {
                    case StampingCylinderType.GuideRod_Move:
                        switch (direction)
                        {
                            case DirectionsEnum.Up:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonDown, false);
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonUp, IsTrigger);
                                break;
                            case DirectionsEnum.Down:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonUp, false);
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonDown, IsTrigger);
                                break;
                            default:
                                ret = false;
                                break;
                        }
                        break;
                    case StampingCylinderType.GuideRod_Fixed:
                        switch (direction)
                        {
                            case DirectionsEnum.Up:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonDown, false);
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonUp, IsTrigger);
                                break;
                            case DirectionsEnum.Down:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonUp, false);
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonDown, IsTrigger);
                                break;
                            default:
                                ret = false;
                                break;
                        }
                        break;
                    case StampingCylinderType.QRStamping:
                        switch (direction)
                        {
                            case DirectionsEnum.Up:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture1.sv_bButtonDown, false);
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture1.sv_bButtonUp, IsTrigger);
                                break;
                            case DirectionsEnum.Down:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture1.sv_bButtonUp, false);
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture1.sv_bButtonDown, IsTrigger);
                                break;
                            default:
                                ret = false;
                                break;
                        }
                        break;
                    case StampingCylinderType.StampingSeat:
                        switch (direction)
                        {
                            case DirectionsEnum.Up:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture2.sv_bButtonDown, false);
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture2.sv_bButtonUp, IsTrigger);
                                break;
                            case DirectionsEnum.Down:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture2.sv_bButtonUp, false);
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture2.sv_bButtonDown, IsTrigger);
                                break;
                            default:
                                ret = false;
                                break;
                        }
                        break;
                    case StampingCylinderType.BlockingCylinder:
                        switch (direction)
                        {
                            case DirectionsEnum.Up:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.BlockingClips1.sv_bButtonUp, IsTrigger);
                                break;
                            case DirectionsEnum.Down:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.BlockingClips1.sv_bButtonDown, IsTrigger);
                                break;
                            default:
                                var O_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsUp, false);
                                var C_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsDown, false);
                                ret = await O_ret && await C_ret;
                                break;
                        }
                        break;
                    case StampingCylinderType.HydraulicEngraving:

                        switch (direction)
                        {
                            case DirectionsEnum.Up:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonOpen, IsTrigger);
                                break;
                            case DirectionsEnum.Down:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonClose, IsTrigger);
                                break;
                            default:
                                var O_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonOpen, false);
                                var C_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonClose, false);
                                ret = await O_ret && await C_ret;
                                break;
                        }

                        break;
                    case StampingCylinderType.HydraulicCutting:

                        switch (direction)
                        {
                            case DirectionsEnum.Up:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Cutting1.sv_bButtonOpen, IsTrigger);
                                break;
                            case DirectionsEnum.Down:
                                ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Cutting1.sv_bButtonClose, IsTrigger);
                                break;
                            default:
                                var O_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Cutting1.sv_bButtonOpen, false);
                                var C_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Cutting1.sv_bButtonClose, false);
                                ret = await O_ret && await C_ret;
                                break;
                        }

                        break;
                    default: throw new NotImplementedException();
                }
            }
            return ret;
        }



        /// <summary>
        ///  雙導桿缸(可動端)在上方
        /// </summary>
        /// <param name="stampingCylinder"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public async Task<(bool, bool)> GetGuideRod_Move_Position_isUp()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture1.sv_bGuideRodsFixtureUp);
        }
        /// <summary>
        ///  雙導桿缸(可動端)在下方
        /// </summary>
        /// <param name="stampingCylinder"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public async Task<(bool, bool)> GetGuideRod_Move_Position_isDown()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture1.sv_bGuideRodsFixtureDown);
        }
        /// <summary>
        /// 雙導桿缸(固定端)在上方
        /// </summary>
        /// <param name="stampingCylinder"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public async Task<(bool, bool)> GetGuideRod_Fixed_Position_isUp()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture2.sv_bGuideRodsFixtureUp);
        }
        /// <summary>
        /// 雙導桿缸(固定端)在下方
        /// </summary>
        /// <param name="stampingCylinder"></param>
        /// <param name="direction"></param>
        /// <returns></returns> 
        public async Task<(bool, bool)> GetGuideRod_Fixed_Position_isDown()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture2.sv_bGuideRodsFixtureDown);
        }

        /// <summary>
        /// QR壓座組在上方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetQRStamping_Position_isUp()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture1.sv_bFixtureUp);
        }

        /// <summary>
        /// QR壓座組在下方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetQRStamping_Position_isDown()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture1.sv_bFixtureDown);
        }
        /// <summary>
        /// 鋼印壓座組在上方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetStampingSeat_Position_isUp()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture2.sv_bFixtureUp);

        }
        /// <summary>
        /// 鋼印壓座組在下方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetStampingSeat_Position_isDown()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture2.sv_bFixtureDown);
        }

        /// <summary>
        /// 阻擋缸在上方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetBlockingCylinder_Position_isUp()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsUp);
        }
        /// <summary>
        /// 阻擋缸在下方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetBlockingCylinder_Position_isDown()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsDown);
        }

        /// <summary>
        /// 原點位置
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicEngraving_Position_Origin()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StopUp);
        }

        /// <summary>
        /// 鋼印待命
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicEngraving_Position_StandbyPoint()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StandbyPoint);
        }
        /// <summary>
        /// 鋼印下壓
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicEngraving_Position_StopDown()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StopDown);
        }

        /// <summary>
        ///訂閱刻字下壓
        /// </summary>
        /// <param name="action"></param>
        /// <param name="samplingInterval"></param>
        /// <param name="checkDuplicates"></param>
        /// <returns></returns>
        public Task<bool> SubscribeHydraulicEngraving_Position_StopDown(Action<bool> action)
        {
            return GD_OpcUaClient.SubscribeNodeDataChangeAsync($"{StampingOpcUANode.Engraving1.di_StopDown}", action, 100, true);
        }







        /// <summary>
        /// 切割原點
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicCutting_Position_Origin()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Cutting1.di_CuttingOrigin);
        }
        /// <summary>
        /// 切割待命位置
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicCutting_Position_StandbyPoint()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Cutting1.di_CuttingStandbyPoint);
        }
        /// <summary>
        /// 切割位置
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicCutting_Position_CutPoint()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Cutting1.di_CuttingCutPoint);
        }


        /// <summary>
        /// 訂閱切割位置
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SubscribeHydraulicCutting_Position_CutPoint(Action<bool> action)
        {
            return await GD_OpcUaClient.SubscribeNodeDataChangeAsync(StampingOpcUANode.Cutting1.di_CuttingCutPoint, action, 50, true);
        }

        public async Task<bool> SetHydraulicPumpMotor(bool Active)
        {
            var pumptask = await GetHydraulicPumpMotor();
            if (pumptask.Item1)
            {
                if (pumptask.Item2 == Active)
                    return true;
                await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Motor1.sv_bButtonMotor, true);
                await Task.Delay(1000);
                return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Motor1.sv_bButtonMotor, false);
            }
            else
                return false;
        }

        /// <summary>
        /// 油壓單元
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicPumpMotor()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Motor1.sv_bMotorStarted);
        }


        /// <summary>
        /// 訂閱油壓單元
        /// </summary>
        /// <param name="action"></param>
        public async Task<bool> SubscribeHydraulicPumpMotor(Action<bool> action)
        {
            return await GD_OpcUaClient.SubscribeNodeDataChangeAsync(StampingOpcUANode.Motor1.sv_bMotorStarted, action, 50, true);
        }




        /// <summary>
        /// 取得鐵片下一片資訊-交握訊號
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        public async Task<(bool, bool)> GetRequestDatabit()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>($"{StampingOpcUANode.system.sv_bRequestDatabit}");
        }


        /// <summary>
        /// 訂閱加工許可交握訊號
        /// </summary>
        /// <param name="action"></param>
        public Task<bool> SubscribeRequestDatabit(Action<bool> action)
        {
            return GD_OpcUaClient.SubscribeNodeDataChangeAsync<bool>(StampingOpcUANode.system.sv_bRequestDatabit, action, 50, true);
        }




        /// <summary>
        /// 取消訂閱第一片ID
        /// </summary>
        /// <param name="action"></param>
      /*  public Task<bool> UnsubscribeFirstIronPlate(int samplingInterval)
        {
            return GD_OpcUaClient.UnsubscribeNodeAsync($"{StampingOpcUANode.system.sv_IronPlateData}[1].iIronPlateID", samplingInterval);
        }*/




        /// <summary>
        /// 設定鐵片下一片資訊-交握訊號
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        public async Task<bool> SetRequestDatabit(bool databit)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_bRequestDatabit}", databit);
        }

        /// <summary>
        /// 取得預計要打的鋼片資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        /*public async Task<(bool,string)> GetHMIIronPlateName(StampingOpcUANode.sIronPlate ironPlateType)
        {
            return await GD_OpcUaClient.AsyncReadNode<string>($"{StampingOpcUANode.system.sv_HMIIronPlateName.NodeName}.{ironPlateType}");
        }*/

        /// <summary>
        /// 設定預計要打的鋼片資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
     /*   public async Task<bool> SetHMIIronPlateName(StampingOpcUANode.sIronPlate ironPlateType, string StringLine)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_HMIIronPlateName.NodeName}.{ironPlateType}", StringLine);
        }*/


        /// <summary>
        /// 取得下一個要加工的鋼印
        /// </summary>

        public async Task<(bool, IronPlateDataModel)> GetHMIIronPlate()
        {
            var rootNode = StampingOpcUANode.system.sv_HMIIronPlateName.NodeName;
            return await this.GetIronPlate(rootNode);
        }
        /// <summary>
        /// 設定下一個鋼印
        /// </summary>
        public async Task<bool> SetHMIIronPlate(IronPlateDataModel ironPlateData)
        {

            // bool ret = false;
            // var wNodeTrees = IronPlateDataTreeNode(StampingOpcUANode.system.sv_HMIIronPlateName.NodeName, ironPlateData);
            var rootNode = StampingOpcUANode.system.sv_HMIIronPlateName.NodeName;
            var wNodeTrees = new Dictionary<string, object>
            {
                [rootNode + "." + "iIronPlateID"] = ironPlateData.iIronPlateID,
                [rootNode + "." + "rXAxisPos1"] = ironPlateData.rXAxisPos1,
                [rootNode + "." + "rYAxisPos1"] = ironPlateData.rYAxisPos1,
                [rootNode + "." + "rXAxisPos2"] = ironPlateData.rXAxisPos2,
                [rootNode + "." + "rYAxisPos2"] = ironPlateData.rYAxisPos2,
                [rootNode + "." + "sIronPlateName1"] = ironPlateData.sIronPlateName1,
                [rootNode + "." + "sIronPlateName2"] = ironPlateData.sIronPlateName2,
                [rootNode + "." + "iStackingID"] = ironPlateData.iStackingID,
                [rootNode + "." + "bEngravingFinish"] = ironPlateData.bEngravingFinish,
                [rootNode + "." + "bDataMatrixFinish"] = ironPlateData.bDataMatrixFinish,
                [rootNode + "." + "sDataMatrixName1"] = ironPlateData.sDataMatrixName1,
                [rootNode + "." + "sDataMatrixName2"] = ironPlateData.sDataMatrixName2

            };
            var ret = await GD_OpcUaClient.AsyncWriteNodes(wNodeTrees);
            //設定完後須更新hmi
            return !ret.Contains(false);
        }

        [Obsolete]
        public async Task<bool> SetDataMatrixMode(bool IsUse)
        {
            bool ret = false;
            //ns=4;s=APPL.system.sv_DataMatrixMode
            var rootNode = StampingOpcUANode.system.sv_DataMatrixMode;
            ret = await GD_OpcUaClient.AsyncWriteNode(rootNode, Convert.ToInt32(IsUse));
            //設定完後須更新hmi
            return ret;
        }

        /// <summary>
        /// 取得鐵片群資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        public async Task<(bool result, List<IronPlateDataModel> ironPlateCollection)> GetIronPlateDataCollection()
        {
            List<IronPlateDataModel> ironPlateDataList = new();
            if (GD_OpcUaClient.IsConnected)
            {
                try
                {
                    for (int i = 1; i <= 25; i++)
                    {
                        var node = $"{StampingOpcUANode.system.sv_IronPlateData}[{i}]";
                        if ((await GetIronPlate(node)).Item1)
                        {
                            ironPlateDataList.Add((await GetIronPlate(node)).Item2);
                        }
                        else
                        {
                            return (false, ironPlateDataList);
                        }
                    }
                    return (true, ironPlateDataList);
                }
                catch (Exception)
                {

                }
            }
            return (false, ironPlateDataList);
        }



        private async Task<(bool, IronPlateDataModel)> GetIronPlate(string rootNode)
        {
            var getTask_iIronPlateID = await GD_OpcUaClient.AsyncReadNode<int>(rootNode + "." + "iIronPlateID");
            var getTask_rXAxisPos1 = await GD_OpcUaClient.AsyncReadNode<float>(rootNode + "." + "rXAxisPos1");
            var getTask_rYAxisPos1 = await GD_OpcUaClient.AsyncReadNode<float>(rootNode + "." + "rYAxisPos1");
            var getTask_rXAxisPos2 = await GD_OpcUaClient.AsyncReadNode<float>(rootNode + "." + "rXAxisPos2");
            var getTask_rYAxisPos2 = await GD_OpcUaClient.AsyncReadNode<float>(rootNode + "." + "rYAxisPos2");
            var getTask_sIronPlateName1 = await GD_OpcUaClient.AsyncReadNode<string>(rootNode + "." + "sIronPlateName1");
            var getTask_sIronPlateName2 = await GD_OpcUaClient.AsyncReadNode<string>(rootNode + "." + "sIronPlateName2");
            var getTask_iStackingID = await GD_OpcUaClient.AsyncReadNode<int>(rootNode + "." + "iStackingID");
            var getTask_bEngravingFinish = await GD_OpcUaClient.AsyncReadNode<bool>(rootNode + "." + "bEngravingFinish");
            var getTask_bDataMatrixFinish = await GD_OpcUaClient.AsyncReadNode<bool>(rootNode + "." + "bDataMatrixFinish");
            var getTask_sDataMatrixName1 = await GD_OpcUaClient.AsyncReadNode<string>(rootNode + "." + "sDataMatrixName1");
            var getTask_sDataMatrixName2 = await GD_OpcUaClient.AsyncReadNode<string>(rootNode + "." + "sDataMatrixName2");

            var ret = ((getTask_iIronPlateID).Item1 &&
                (getTask_rXAxisPos1).Item1 &&
                (getTask_rYAxisPos1).Item1 &&
                (getTask_sIronPlateName1).Item1 &&
                (getTask_rXAxisPos2).Item1 &&
                (getTask_rYAxisPos2).Item1 &&
                (getTask_sIronPlateName2).Item1 &&
                (getTask_iStackingID).Item1 &&
                (getTask_bEngravingFinish).Item1 &&
                (getTask_bDataMatrixFinish).Item1 &&
                (getTask_sDataMatrixName1).Item1 &&
                (getTask_sDataMatrixName2).Item1
                );

            var newIronPlateData = new IronPlateDataModel()
            {    //ID
                iIronPlateID = (getTask_iIronPlateID).Item2,
                // 字串1的x座標
                rXAxisPos1 = (getTask_rXAxisPos1).Item2,
                // 字串1的y座標
                rYAxisPos1 = (getTask_rYAxisPos1).Item2,
                // 字串1的內容
                sIronPlateName1 = (getTask_sIronPlateName1).Item2,
                // 字串2的x座標
                rXAxisPos2 = (getTask_rXAxisPos2).Item2,
                // 字串2的y座標
                rYAxisPos2 = (getTask_rYAxisPos2).Item2,

                /// 字串2的內容
                sIronPlateName2 = (getTask_sIronPlateName2).Item2,
                /// 字串3的x座標
                //rXAxisPos3 = (await getTask_rXAxisPos3).Item2,
                /// 字串3的y座標
                //rYAxisPos3 = (await getTask_rYAxisPos3).Item2,
                /// 字串3的內容
                //sIronPlateName3  =(await getTask_sIronPlateName3).Item2,
                // QR Code的字串
                //sQRCodeName1=  (await getTask_sQRCodeName1).Item2,

                //  QR Code的x座標
                //rQRcodeXAxisPos  =(await getTask_rQRcodeXAxisPos).Item2,
                // QR Code前字串
                //sQRCodeName2 = (await getTask_sQRCodeName2).Item2,
                // 分料盒
                iStackingID = (getTask_iStackingID).Item2,
                // QRCode完成
                //bQRCodeFinish  =(await getTask_bQRCodeFinish).Item2,
                // 刻碼完成 
                bEngravingFinish = (getTask_bEngravingFinish).Item2,
                bDataMatrixFinish = (getTask_bDataMatrixFinish).Item2,
                sDataMatrixName1 = (getTask_sDataMatrixName1).Item2,
                sDataMatrixName2 = (getTask_sDataMatrixName2).Item2,

            };

            return (ret, newIronPlateData);
        }




        /// <summary>
        /// 設定鐵片群資訊(加工陣列)
        /// </summary>
        /// <param name="ironPlateDataList"></param>
        /// <returns></returns>
        public async Task<bool> SetIronPlateDataCollection(List<IronPlateDataModel> ironPlateDataList)
        {

            int ExistedDataCollectionCount = 1000;
            List<IronPlateDataModel> write_ironPlateDataList;
            //取得舊有的鐵片群資訊
            var getIronPlateDataCollectionTuple = await GetIronPlateDataCollection();
            if (getIronPlateDataCollectionTuple.result)
            {
                write_ironPlateDataList = getIronPlateDataCollectionTuple.ironPlateCollection;
                ExistedDataCollectionCount = write_ironPlateDataList.Count;
                //超出範圍的不寫
            }
            else
            {
                write_ironPlateDataList = Enumerable.Repeat(new IronPlateDataModel()
                {
                    sIronPlateName1 = string.Empty,
                    sIronPlateName2 = string.Empty,
                }, ExistedDataCollectionCount).ToList();
            }

            for (int i = 0; i < write_ironPlateDataList.Count; i++)
            {
                if (i < ironPlateDataList.Count)
                    write_ironPlateDataList[i] = ironPlateDataList[i];
                else
                {
                    write_ironPlateDataList[i].sIronPlateName1 = string.Empty;
                    write_ironPlateDataList[i].sIronPlateName2 = string.Empty;
                    write_ironPlateDataList[i].sDataMatrixName1 = string.Empty;
                    write_ironPlateDataList[i].sDataMatrixName2 = string.Empty;
                }
            }



            List<(string, bool)> WritedList = new();
            for (int i = 0; i < write_ironPlateDataList.Count; i++)
            {
                IronPlateDataModel ironPlateData = write_ironPlateDataList[i];

                string node = $"{StampingOpcUANode.system.sv_IronPlateData}[{i + 1}]";
                var wNodeTreeNodes = IronPlateDataTreeNode(node, ironPlateData);

                var WriteBoolean = false;
                for (int j = 0; j < 5; j++)
                {
                    var WriteBooleanList = await GD_OpcUaClient.AsyncWriteNodes(wNodeTreeNodes);
                    WriteBoolean = !WriteBooleanList.Contains(false);
                    if (WriteBoolean)
                        break;
                }
                WritedList.Add((node, WriteBoolean));
            }
            return WritedList.Exists(x => x.Item2 == true);
            // return true;
            //  return await GD_OpcUaClient.AsyncWriteNodes(wNodeTrees);
        }


        private Dictionary<string, object> IronPlateDataTreeNode(string node, IronPlateDataModel ironPlateData)
        {
            return new Dictionary<string, object>
            {
                [node + "." + "iIronPlateID"] = ironPlateData.iIronPlateID,
                [node + "." + "rXAxisPos1"] = ironPlateData.rXAxisPos1,
                [node + "." + "rYAxisPos1"] = ironPlateData.rYAxisPos1,
                [node + "." + "rXAxisPos2"] = ironPlateData.rXAxisPos2,
                [node + "." + "rYAxisPos2"] = ironPlateData.rYAxisPos2,
                [node + "." + "sIronPlateName1"] = ironPlateData.sIronPlateName1,
                [node + "." + "sIronPlateName2"] = ironPlateData.sIronPlateName2,
                [node + "." + "sDataMatrixName1"] = ironPlateData.sDataMatrixName1,
                [node + "." + "sDataMatrixName2"] = ironPlateData.sDataMatrixName2,
                [node + "." + "iStackingID"] = ironPlateData.iStackingID,
                [node + "." + "bEngravingFinish"] = ironPlateData.bEngravingFinish,
                [node + "." + "bDataMatrixFinish"] = ironPlateData.bDataMatrixFinish
            };
        }

















        public async Task<(bool, float)> GetEngravingYAxisPosition()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_rEngravingFeedingPosition}");
        }

        public async Task<(bool, float)> GetEngravingZAxisPosition()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Engraving1.sv_rEngravingPosition}");
        }


        /* public async Task<bool> GetEngravingZAxisHydraulicUp(out bool IsActived)
         {
        return await GD_OpcUaClient.AsyncReadNode($"{StampingOpcUANode.Engraving1.sv_bButtonOpen}", out IsActived);
         }

         public async Task<bool> GetEngravingZAxisHydraulicDown(out bool IsActived)
         {
        return await GD_OpcUaClient.AsyncReadNode($"{StampingOpcUANode.Engraving1.sv_bButtonClose}", out IsActived);
         }
         public async Task<bool> SetEngravingZAxisHydraulicUp(bool Actived)
         {
             return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Engraving1.sv_bButtonOpen}", Actived);
         }

         public async Task<bool> SetEngravingZAxisHydraulicDown(bool Actived)
         {
             return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Engraving1.sv_bButtonClose}", Actived);
         }*/





















        public async Task<bool> SetEngravingYAxisToStandbyPos()
        {
             await SetEngravingYAxisBwd(false);
             await SetEngravingYAxisFwd(false);
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_rServoStandbyPos}", true);
        }


        public async Task<(bool, bool)> GetEngravingYAxisBwd()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonBwd}");
        }
        public async Task<(bool, bool)> GetEngravingYAxisFwd()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonFwd}");
        }

        public async Task<bool> SetEngravingYAxisBwd(bool Active)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonBwd}", Active);
        }
        public async Task<bool> SetEngravingYAxisFwd(bool Active)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonFwd}", Active);
        }




        public async Task<(bool, int)> GetEngravingRotateStation()
        {
            int Station = -1;
            var ret = await GD_OpcUaClient.AsyncReadNode<int>($"{StampingOpcUANode.EngravingRotate1.sv_iThisStation}");

            if (ret.Item1)
            {
                //※須注意 目前使用的函式回傳index時是1為起始 所以需-1才能符合其他位置
                var StationIndex = ret.Item2;
                Station = StationIndex - 1;
            }

            return (ret.Item1, Station);
        }


        /// <summary>
        /// 訂閱鋼印位置
        /// </summary>
        /// <param name="action"></param>
        public Task<bool> SubscribeEngravingRotateStation(Action<int> action)
        {
            return GD_OpcUaClient.SubscribeNodeDataChangeAsync(StampingOpcUANode.EngravingRotate1.sv_iThisStation, action, 100, true);
        }




        /// <summary>
        /// 總站數
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, int)> GetEngravingTotalSlots()
        {
            return await GD_OpcUaClient.AsyncReadNode<int>($"{StampingOpcUANode.EngravingRotate1.sv_iTotalSlots}");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, StampingTypeModel)> GetEngravingRotateStationChar()
        {
            StampingTypeModel stampingType = new();
            var ret1 = await GetEngravingRotateStation();
            var ret2 = await GetRotatingTurntableInfo();

            if (ret1.Item1 && ret2.Item1)
            {
                try
                {
                    var index = ret1.Item2;
                    var tableInfo = ret2.Item2;

                    stampingType = tableInfo[index];
                    return (true, stampingType);
                }
                catch
                {

                }
            }
            return (false, stampingType);

        }



        /*public async Task<bool> SetEngravingRotateStation(int Station)
        {
          //  await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_iTargetAStation}", Station + 1);

            //return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_iTargetAStation}", Station + 1);
        }*/

        private async Task<(bool, int[])> GetRotatingTurntableInfoINT()
            => await GD_OpcUaClient.AsyncReadNode<int[]>($"{StampingOpcUANode.system.sv_RotateCodeDefinition}");

        private async Task<bool> SetRotatingTurntableInfoINT(int[] fonts)
            => await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_RotateCodeDefinition}", fonts);










        /// <summary>
        /// 取得字模
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, List<StampingTypeModel>)> GetRotatingTurntableInfo()
        {
            var ret = await GetRotatingTurntableInfoINT();
            var turntableInfoList = new List<StampingTypeModel>();
            if (ret.Item1)
            {
                for (int i = 0; i < ret.Item2.Length; i++)
                {
                    turntableInfoList.Add(new StampingTypeModel()
                    {
                        StampingTypeNumber = i + 1,
                        StampingTypeString = ret.Item2[i].ToChar().ToString()
                    });
                }
            }
            return (ret.Item1, turntableInfoList);
        }

        public async Task<bool> SetRotatingTurntableInfo(List<StampingTypeModel> fonts)
        {
            List<char> charList = new List<char>();
            foreach (var font in fonts)
            {
                var charArray = font.StampingTypeString.ToCharArray();
                if (charArray.Length > 0)
                    charList.Add(charArray[0]);
                else
                    charList.Add(new char());
            }

            return await SetRotatingTurntableInfoINT(charList.ToIntList().ToArray());
        }


        public async Task<bool> SetRotatingTurntableInfo(int index, char font)
        {
            var ret = await GetRotatingTurntableInfoINT();
            if (ret.Item1)
            {
                var codeInfoArray = ret.Item2;
                //確認沒超出範圍
                if (codeInfoArray.ToList().Count > index)
                {
                    codeInfoArray[index] = font.ToInt();
                    return await SetRotatingTurntableInfoINT(codeInfoArray);
                }
            }

            return false;
        }




        public async Task<bool> SetEngravingRotateCW(bool isActived)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_bButtonCW}", isActived);
        }

        public async Task<bool> SetEngravingRotateCCW(bool isActived)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_bButtonCCW}", isActived);
        }

        public async Task<(bool, float)> GetFeedingSetupVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_rFeedSetupVelocity}");
        }
        public async Task<bool> SetFeedingSetupVelocity(float percent)
        {
            return await GD_OpcUaClient.AsyncWriteNode<float>($"{StampingOpcUANode.Feeding1.sv_rFeedSetupVelocity}", percent);
        }
        public async Task<(bool, float)> GetFeedingVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_rFeedVelocity}");
        }
        public async Task<bool> SetFeedingVelocity(float percent)
        {
            return await GD_OpcUaClient.AsyncWriteNode<float>($"{StampingOpcUANode.Feeding1.sv_rFeedVelocity}", percent);
        }



        /// <summary>
        /// 字模移動速度
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, float)> GetEngravingFeedingSetupVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_rFeedSetupVelocity}");
        }

        /// <summary>
        /// 設定字模移動速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<bool> SetEngravingFeedingSetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_rFeedSetupVelocity}", SpeedPercent);

        }

        /// <summary>
        /// 字模移動速度
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, float)> GetEngravingFeedingVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_rFeedVelocity}");
        }

        /// <summary>
        /// 設定字模移動速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<bool> SetEngravingFeedingVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_rFeedVelocity}", SpeedPercent);

        }




        /// <summary>
        /// 字模旋轉速度
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, float)> GetEngravingRotateSetupVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingRotate1.sv_rRotateSetupVelocity}");
        }

        /// <summary>
        /// 設定字模旋轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<bool> SetEngravingRotateSetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_rRotateSetupVelocity}", SpeedPercent);

        }

        /// <summary>
        /// 字模旋轉速度
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, float)> GetEngravingRotateVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingRotate1.sv_rRotateVelocity}");
        }

        /// <summary>
        /// 設定字模旋轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<bool> SetEngravingRotateVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_rRotateVelocity}", SpeedPercent);
        }


        /// <summary>
        /// 刻印壓力
        /// </summary>
        /// <returns></returns>

        public async Task<(bool, float)> GetStampingPressure()
        {
            return await GD_OpcUaClient.AsyncReadNode<UInt32>($"{StampingOpcUANode.Pump1.sv_PressureLintab.LintabPoints.uNoOfPoints}");
        }
        /// <summary>
        /// 刻印速度
        /// </summary>
        /// <returns></returns>

        public async Task<(bool, float)> GetStampingVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<UInt32>($"{StampingOpcUANode.Pump1.sv_VelocityLintab.LintabPoints.uNoOfPoints}");
        }

        /// <summary>
        /// 裁斷壓力
        /// </summary>
        /// <returns></returns>

        public async Task<(bool, float)> GetShearingPressure()
        {
            return await GD_OpcUaClient.AsyncReadNode<UInt32>($"{StampingOpcUANode.Pump2.sv_PressureLintab.LintabPoints.uNoOfPoints}");
        }
        /// <summary>
        /// 裁斷速度
        /// </summary>
        /// <returns></returns>

        public async Task<(bool, float)> GetShearingVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<UInt32>($"{StampingOpcUANode.Pump2.sv_VelocityLintab.LintabPoints.uNoOfPoints}");
        }








        /// <summary>
        /// QR機IP
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string)> GetDataMatrixTCPIP()
        {
            return await GD_OpcUaClient.AsyncReadNode<string>(StampingOpcUANode.DataMatrix1.sv_sContactTCPIP);
        }

        /// <summary>
        /// QR機IP
        /// </summary>
        /// <returns></returns>
       /* public Task SubscribeDataMatrixTCPIP(Action<string> action)
        {
            return GD_OpcUaClient.SubscribeNodeDataChange<string>(StampingOpcUANode.DataMatrix1.sv_sContactTCPIP, action);
        }*/


        /// <summary>
        /// QR機Port
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string)> GetDataMatrixPort()
        {
            return await GD_OpcUaClient.AsyncReadNode<string>($"{StampingOpcUANode.DataMatrix1.sv_sContactTCPPort}");
        }

        /// <summary>
        /// QR機Port
        /// </summary>
        /// <returns></returns>

        /*public Task SubscribeDataMatrixPort(Action<string> action)
        {
            return GD_OpcUaClient.SubscribeNodeDataChange<string>(StampingOpcUANode.DataMatrix1.sv_sContactTCPPort,action);
        }*/






        public async Task<(bool, float)> GetFeedingXHomeFwdVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_rHomeFwdVelocity}");
        }


        public async Task<(bool, float)> GetFeedingXHomeBwdVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_rHomeBwdVelocity}");
        }


        public async Task<bool> SetFeedingXHomeFwdVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Feeding1.sv_rHomeFwdVelocity}", SpeedPercent);
        }


        public async Task<bool> SetFeedingXHomeBwdVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Feeding1.sv_rHomeBwdVelocity}", SpeedPercent);
        }

        /// <summary>
        /// 潤滑設定時間
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<(bool, object)> GetLubricationSettingTime()
        {
            return await GD_OpcUaClient.AsyncReadNode<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationSetValues.dLubTime}");
        }

        /// <summary>
        /// 潤滑開設定時間
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, object)> GetLubricationSettingOnTime()
        {
            return await GD_OpcUaClient.AsyncReadNode<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationSetValues.dOnTime}");
        }
        /// <summary>
        /// 潤滑關設定時間
        /// </summary>
        /// <returns></returns>     
        public async Task<(bool, object)> GetLubricationSettingOffTime()
        {
            return await GD_OpcUaClient.AsyncReadNode<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationSetValues.dOffTime}");
        }


        /// <summary>
        /// 潤滑實際時間
        /// </summary>
        public async Task<(bool, object)> GetLubricationActualTime()
        {
            return await GD_OpcUaClient.AsyncReadNode<int>($"{StampingOpcUANode.Lubrication1.sv_LubricationActValues.dLubTime}");
        }
        /// <summary>
        /// 潤滑開實際時間
        /// </summary>
        public async Task<(bool, object)> GetLubricationActualOnTime()
        {
            return await GD_OpcUaClient.AsyncReadNode<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationActValues.dOnTime}");
        }

        /// <summary>
        /// 潤滑關實際時間
        /// </summary>
        public async Task<(bool, object)> GetLubricationActualOffTime()
        {
            return await GD_OpcUaClient.AsyncReadNode<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationActValues.dOffTime}");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeID">節點id</param>
        /// <param name="action">動作</param>
        /// <param name="checkDuplicates">是否檢查重複項</param>
        /// <returns></returns>
        public async Task<bool> SubscribeNodeDataChangeAsync<T>(string NodeID, Action<T> action, bool checkDuplicates)
        {
            return await GD_OpcUaClient.SubscribeNodeDataChangeAsync<T>(NodeID, action, 200, checkDuplicates);
        }


        public async Task<IList<bool>> SubscribeNodesDataChangeAsync<T>(IList<(string NodeID, Action<T> updateAction, int samplingInterval, bool checkDuplicates)> nodeList)
        {
            return await GD_OpcUaClient.SubscribeNodesDataChangeAsync<T>(nodeList);
        }





        #region 節點
        /// <summary>
        /// 節點對應字串
        /// </summary>
        public class StampingOpcUANode
        {
            /// <summary>
            /// ns=4;s=APPL
            /// </summary>
            public static readonly string NodeHeader = "ns=4;s=APPL";

            /// <summary>
            /// 大節點
            /// </summary>
            private enum NodeVariable
            {
                /// <summary>
                /// 進料X軸馬達
                /// </summary>
                Feeding1,
                /// <summary>
                /// 雙導桿缸-移動
                /// </summary>
                GuideRodsFixture1,
                /// <summary>
                /// 雙導桿缸-固定
                /// </summary>
                GuideRodsFixture2,
                /// <summary>
                /// QR壓座組
                /// </summary>
                Fixture1,
                /// <summary>
                /// 鋼印壓座組
                /// </summary>
                Fixture2,
                /// <summary>
                /// 阻擋缸
                /// </summary>
                BlockingClips1,
                /// <summary>
                /// 鋼印轉盤進料
                /// </summary>
                EngravingFeeding1,
                /// <summary>
                /// 鋼印轉盤進料
                /// </summary>
                Engraving1,

                /// <summary>
                /// 鋼印轉盤旋轉
                /// </summary>
                EngravingRotate1,
                /// <summary>
                /// QrCode機器
                /// </summary>
                DataMatrix1,
                /// <summary>
                /// 系統
                /// </summary>
                OperationMode1,
                /// <summary>
                /// 系統
                /// </summary>
                system,
                /// <summary>
                /// 油壓單元
                /// </summary>
                Motor1,
                /// <summary>
                /// 裁切
                /// </summary>
                Cutting1,
                /// <summary>
                /// 分料
                /// </summary>
                Stacking1,
                /// <summary>
                /// 刻印
                /// </summary>
                Pump1,
                /// <summary>
                /// 裁斷
                /// </summary>
                Pump2,
                /// <summary>
                /// 潤滑
                /// </summary>
                Lubrication1

            }

            /// <summary>
            /// 按鈕
            /// </summary>
            enum BButton
            {
                sv_bButtonFwd,
                sv_bButtonBwd,
                sv_bButtonUp,
                sv_bButtonDown,
                sv_bButtonOpen,
                sv_bButtonClose,
                sv_bButtonCW,
                sv_bButtonCCW,
                sv_bButtonMotor,

            }

            /// <summary>
            /// 裁切
            /// </summary>
            enum BCutting
            {
                sv_bCuttingOpen,
                sv_bCuttingClosed,
            }



            /// <summary>
            /// 伺服控制
            /// </summary>
            enum SServoMove
            {
                sv_rFeedingPosition,
                sv_rServoMovePos,
                sv_rServoStandbyPos
            }


            enum REngraving
            {
                /// <summary>
                /// Y軸馬達目前位置
                /// </summary>
                sv_rEngravingFeedingPosition,
                /// <summary>
                /// 鋼印目前Z軸位置
                /// </summary>
                sv_rEngravingPosition
            }



            /// <summary>
            /// 一般磁簧
            /// </summary>
            enum SMagneticSwitch
            {


                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                sv_bFixtureUp,
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                sv_bFixtureDown
            }
            /// <summary>
            /// 導桿缸磁簧
            /// </summary>
            enum SMagneticSwitch_GuideRods
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                sv_bGuideRodsFixtureUp,

                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                sv_bGuideRodsFixtureDown,
            }

            /// <summary>
            /// 阻擋缸磁簧
            /// </summary>
            enum SMagneticSwitch_BlockingClips
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                sv_bBlockingClipsUp,

                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                sv_bBlockingClipsDown,
            }

            /// <summary>
            /// 站號
            /// </summary>
            enum TargetStation
            {
                /// <summary>
                /// 目前站號
                /// </summary>
                sv_iThisStation,
                /// <summary>
                /// 目標站號
                /// </summary>
                sv_iTargetAStation
            }

            enum HMI
            {
                sv_HMIIronPlateName
            }

            /// <summary>
            /// 字串1 2 3
            /// </summary>
          /*public  enum sIronPlate
            {
                sIronPlateName1,
                sIronPlateName2,
                sIronPlateName3
            }*/

            /// <summary>
            /// 進料X軸馬達
            /// </summary>
            public class Feeding1
            {
                /// <summary>
                /// 馬達目前位置
                /// </summary>
                public static string sv_rFeedingPosition => $"{NodeHeader}.{NodeVariable.Feeding1}.{SServoMove.sv_rFeedingPosition}";
                /// <summary>
                /// 馬達位置移動命令
                /// </summary>
                public static string sv_rServoMovePos => $"{NodeHeader}.{NodeVariable.Feeding1}.{SServoMove.sv_rServoMovePos}";
                /// <summary>
                /// x軸開機回零
                /// </summary>
                public static string sv_bUseHomeing => $"{NodeHeader}.{NodeVariable.Feeding1}.sv_bUseHomeing";

                /// <summary>
                /// 原點訊號
                /// </summary>
                public static string di_ServoHome => $"{NodeHeader}.{NodeVariable.Feeding1}.di_ServoHome";




                /// <summary>
                /// 手動前進
                /// </summary>
                public static string sv_bButtonFwd => $"{NodeHeader}.{NodeVariable.Feeding1}.{BButton.sv_bButtonFwd}";
                /// <summary>
                /// 手動後退
                /// </summary>
                public static string sv_bButtonBwd => $"{NodeHeader}.{NodeVariable.Feeding1}.{BButton.sv_bButtonBwd}";
                /// <summary>
                /// 自動模式進給速度
                /// </summary>
                public static string sv_rFeedSetupVelocity => $"{NodeHeader}.{NodeVariable.Feeding1}.sv_rFeedSetupVelocity";
                /// <summary>
                /// 手動模式進給速度
                /// </summary>
                public static string sv_rFeedVelocity => $"{NodeHeader}.{NodeVariable.Feeding1}.sv_rFeedVelocity";
                /// <summary>
                /// homeing前進速度
                /// </summary>
                public static string sv_rHomeFwdVelocity => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_rHomeFwdVelocity}";
                /// <summary>
                ///homeing後退速度
                /// </summary>
                public static string sv_rHomeBwdVelocity => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_rHomeBwdVelocity}";
                /// <summary>
                /// 設定模式前進速度
                /// </summary>       
                //public static string sv_ConstFwdSetup => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_ConstFwdSetup}.{VelocityNode}";
                /// <summary>
                /// 設定模式後退速度
                /// </summary>
                //public static string sv_ConstBwdSetup => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_ConstBwdSetup}.{VelocityNode}";
                /// <summary>
                /// 手動/自動前進速度
                /// </summary>
                //public static string sv_ConstFwd => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_ConstFwd}.{VelocityNode}";
                /// <summary>
                /// 手動/自動後退速度
                /// </summary>
                //public static string sv_ConstBwd => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_ConstBwd}.{VelocityNode}";


            }

            /// <summary>
            /// 雙導桿缸-移動
            /// </summary>
            public class GuideRodsFixture1
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                public static string sv_bGuideRodsFixtureUp => $"{NodeHeader}.{NodeVariable.GuideRodsFixture1}.{SMagneticSwitch_GuideRods.sv_bGuideRodsFixtureUp}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                public static string sv_bGuideRodsFixtureDown => $"{NodeHeader}.{NodeVariable.GuideRodsFixture1}.{SMagneticSwitch_GuideRods.sv_bGuideRodsFixtureDown}";
                /// <summary>
                ///手動氣壓缸升命令
                /// </summary>
                public static string sv_bButtonUp => $"{NodeHeader}.{NodeVariable.GuideRodsFixture1}.{BButton.sv_bButtonUp}";
                /// <summary>
                ///手動氣壓缸降命令
                /// </summary>
                public static string sv_bButtonDown => $"{NodeHeader}.{NodeVariable.GuideRodsFixture1}.{BButton.sv_bButtonDown}";
            }

            /// <summary>
            /// 雙導桿缸-固定
            /// </summary>
            public class GuideRodsFixture2
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                public static string sv_bGuideRodsFixtureUp => $"{NodeHeader}.{NodeVariable.GuideRodsFixture2}.{SMagneticSwitch_GuideRods.sv_bGuideRodsFixtureUp}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                public static string sv_bGuideRodsFixtureDown => $"{NodeHeader}.{NodeVariable.GuideRodsFixture2}.{SMagneticSwitch_GuideRods.sv_bGuideRodsFixtureDown}";
                /// <summary>
                ///手動氣壓缸升命令
                /// </summary>
                public static string sv_bButtonUp => $"{NodeHeader}.{NodeVariable.GuideRodsFixture2}.{BButton.sv_bButtonUp}";
                /// <summary>
                ///手動氣壓缸降命令
                /// </summary>
                public static string sv_bButtonDown => $"{NodeHeader}.{NodeVariable.GuideRodsFixture2}.{BButton.sv_bButtonDown}";
            }

            /// <summary>
            /// QR機
            /// </summary>
            public class DataMatrix1
            {
                /// <summary>
                /// 設定ip
                /// </summary>
                public static string sv_sContactTCPIP => $"{NodeHeader}.{NodeVariable.DataMatrix1}.sv_sContactTCPIP";
                /// <summary>
                /// 設定port
                /// </summary>
                public static string sv_sContactTCPPort => $"{NodeHeader}.{NodeVariable.DataMatrix1}.sv_sContactTCPPort";
            }

            /// <summary>
            /// QR壓座組
            /// </summary>
            public class Fixture1
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                public static string sv_bFixtureUp => $"{NodeHeader}.{NodeVariable.Fixture1}.{SMagneticSwitch.sv_bFixtureUp}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                public static string sv_bFixtureDown => $"{NodeHeader}.{NodeVariable.Fixture1}.{SMagneticSwitch.sv_bFixtureDown}";
                /// <summary>
                ///手動氣壓缸升命令
                /// </summary>
                public static string sv_bButtonUp => $"{NodeHeader}.{NodeVariable.Fixture1}.{BButton.sv_bButtonUp}";
                /// <summary>
                ///手動氣壓缸降命令
                /// </summary>
                public static string sv_bButtonDown => $"{NodeHeader}.{NodeVariable.Fixture1}.{BButton.sv_bButtonDown}";
            }

            /// <summary>
            /// 鋼印壓座組
            /// </summary>
            public class Fixture2
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                public static string sv_bFixtureUp => $"{NodeHeader}.{NodeVariable.Fixture2}.{SMagneticSwitch.sv_bFixtureUp}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                public static string sv_bFixtureDown => $"{NodeHeader}.{NodeVariable.Fixture2}.{SMagneticSwitch.sv_bFixtureDown}";
                /// <summary>
                ///手動氣壓缸升命令
                /// </summary>
                public static string sv_bButtonUp => $"{NodeHeader}.{NodeVariable.Fixture2}.{BButton.sv_bButtonUp}";
                /// <summary>
                ///手動氣壓缸降命令
                /// </summary>
                public static string sv_bButtonDown => $"{NodeHeader}.{NodeVariable.Fixture2}.{BButton.sv_bButtonDown}";
            }

            /// <summary>
            /// 阻擋缸
            /// </summary>
            public class BlockingClips1
            {
                /// <summary>
                /// 磁簧訊號上限 
                /// </summary>
                public static string sv_bBlockingClipsUp => $"{NodeHeader}.{NodeVariable.BlockingClips1}.{SMagneticSwitch_BlockingClips.sv_bBlockingClipsUp}";
                /// <summary>
                /// 磁簧訊號下限 
                /// </summary>
                public static string sv_bBlockingClipsDown => $"{NodeHeader}.{NodeVariable.BlockingClips1}.{SMagneticSwitch_BlockingClips.sv_bBlockingClipsDown}";

                /// <summary>
                /// 手動氣壓缸升命令 
                /// </summary>
                public static string sv_bButtonUp => $"{NodeHeader}.{NodeVariable.BlockingClips1}.{BButton.sv_bButtonUp}";
                /// <summary>
                /// 手動氣壓缸降命令 
                /// </summary>
                public static string sv_bButtonDown => $"{NodeHeader}.{NodeVariable.BlockingClips1}.{BButton.sv_bButtonDown}";

            }


            public class OperationMode1
            {
                public static string sv_bButtonManual => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bButtonManual";
                public static string sv_bButtonSetup => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bButtonSetup";
                public static string sv_bButtonHalfAuto => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bButtonHalfAuto";
                public static string sv_bButtonFullAuto => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bButtonFullAuto";
                public static string sv_bButtonAlarmConfirm => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bButtonAlarmConfirm";
                public static string sv_bButtonCycleStart => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bButtonCycleStart";


                public static string di_ButtonManual => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_ButtonManual";
                public static string di_ButtonSetup => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_ButtonSetup";
                public static string di_ButtonHalfAuto => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_ButtonHalfAuto";
                public static string di_ButtonFullAuto => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_ButtonFullAuto";
                public static string di_ButtonAlarmConfirm => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_ButtonAlarmConfirm";
                public static string di_ButtonCycleStart => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_ButtonCycleStart";
            }

            public class Pump1
            {
                public class sv_PressureLintab
                {
                    public class LintabPoints
                    {
                        /// <summary>
                        /// 壓力校正表段數
                        /// </summary>
                        public static string uNoOfPoints => $"{NodeHeader}.{NodeVariable.Pump1}.sv_PressureLintab.LintabPoints.uNoOfPoints";
                    }
                }
                public class sv_VelocityLintab
                {
                    public class LintabPoints
                    {
                        /// <summary>
                        /// 壓力校正表段數
                        /// </summary>
                        public static string uNoOfPoints => $"{NodeHeader}.{NodeVariable.Pump1}.sv_PressureLintab.LintabPoints.uNoOfPoints";
                    }
                }

                //public static string sv_rAOPressure => $"{NodeHeader}.{NodeVariable.Pump1}.sv_rAOPressure";

                /// <summary>
                /// AO_刻印壓力
                /// </summary>
                //public static string sv_rAOPressure => $"{NodeHeader}.{NodeVariable.Pump1}.sv_rAOPressure";
                /// <summary>
                ///  AO_刻印速度
                /// </summary>
                //public static string sv_rAOVelocity => $"{NodeHeader}.{NodeVariable.Pump1}.sv_rAOVelocity";
            }

            public class Pump2
            {
                public class sv_PressureLintab
                {
                    public class LintabPoints
                    {
                        /// <summary>
                        /// 壓力校正表段數
                        /// </summary>
                        public static string uNoOfPoints => $"{NodeHeader}.{NodeVariable.Pump1}.sv_PressureLintab.LintabPoints.uNoOfPoints";
                    }
                }
                public class sv_VelocityLintab
                {
                    public class LintabPoints
                    {
                        /// <summary>
                        /// 壓力校正表段數
                        /// </summary>
                        public static string uNoOfPoints => $"{NodeHeader}.{NodeVariable.Pump1}.sv_PressureLintab.LintabPoints.uNoOfPoints";
                    }
                }
                /// <summary>
                /// AO_裁斷壓力
                /// </summary>
                //public static string sv_rAOPressure => $"{NodeHeader}.{NodeVariable.Pump2}.sv_rAOPressure";
                /// <summary>
                ///  AO_裁斷速度
                /// </summary>
                //public static string sv_rAOVelocity => $"{NodeHeader}.{NodeVariable.Pump2}.sv_rAOVelocity";
            }




            //public const string VelocityNode = "Velocity.Output.rOutputValue";


            /// <summary>
            /// 速度模式
            /// </summary>
            private enum ConstSpeedSetup
            {
                /// <summary>
                /// homeing前進速度
                /// </summary>
                sv_rHomeFwdVelocity,
                /// <summary>
                /// homeing後退速度
                /// </summary>
                sv_rHomeBwdVelocity,
                /// <summary>
                /// 設定模式前進速度
                /// </summary>            
                sv_ConstFwdSetup,
                /// <summary>
                /// 設定模式後退速度
                /// </summary>
                sv_ConstBwdSetup,
                /// <summary>
                /// 手動/自動前進速度
                /// </summary>
                sv_ConstFwd,
                /// <summary>
                /// 手動/自動後退速度
                /// </summary>
                sv_ConstBwd,

                /// <summary>
                /// 設定模式旋轉速度
                /// </summary>            
                sv_ConstRotateSetup,
                /// <summary>
                /// 手動/自動正轉速度
                /// </summary>
                sv_ConstRotateCW,
                /// <summary>
                /// 手動/自動逆轉速度
                /// </summary>
                sv_ConstRotateCCW,
            }

            /// <summary>
            /// 鋼印進料
            /// </summary>
            public class EngravingFeeding1
            {




                /// <summary>
                /// Y軸馬達目前位置
                /// </summary>
                public static string sv_rEngravingFeedingPosition => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{REngraving.sv_rEngravingFeedingPosition}";


                /// <summary>
                /// 回歸基準點命令
                /// </summary>
                public static string sv_rServoStandbyPos => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{SServoMove.sv_rServoStandbyPos}";

                /// <summary>
                /// 手動前進
                /// </summary>
                public static string sv_bButtonFwd => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{BButton.sv_bButtonFwd}";
                /// <summary>
                ///  手動後退
                /// </summary>
                public static string sv_bButtonBwd => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{BButton.sv_bButtonBwd}";


                public static string sv_rFeedSetupVelocity => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.sv_rFeedSetupVelocity";
                public static string sv_rFeedVelocity => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.sv_rFeedVelocity";
                /// <summary>
                /// 設定模式前進速度
                /// </summary>       
                //public static string sv_ConstFwdSetup => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{ConstSpeedSetup.sv_ConstFwdSetup}.{VelocityNode}";
                /// <summary>
                /// 設定模式後退速度
                /// </summary>
                //public static string sv_ConstBwdSetup => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{ConstSpeedSetup.sv_ConstBwdSetup}.{VelocityNode}";
                /// <summary>
                /// 手動/自動前進速度
                /// </summary>
                //public static string sv_ConstFwd => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{ConstSpeedSetup.sv_ConstFwd}.{VelocityNode}";
                /// <summary>
                /// 手動/自動後退速度
                /// </summary>
                //public static string sv_ConstBwd => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{ConstSpeedSetup.sv_ConstBwd}.{VelocityNode}";



            }

            public enum AxisPos
            {
                rXAxisPos1, rYAxisPos1,
                rXAxisPos2, rYAxisPos2,
            }


            /// <summary>
            /// 鋼印
            /// </summary>
            public class Engraving1
            {
                /// <summary>
                /// 鋼印目前Z軸位置 
                /// </summary>
                public static string sv_rEngravingPosition => $"{NodeHeader}.{NodeVariable.Engraving1}.{REngraving.sv_rEngravingPosition}";


                /// <summary>
                /// 手動油壓缸升命令 
                /// </summary>
                public static string sv_bButtonClose => $"{NodeHeader}.{NodeVariable.Engraving1}.{BButton.sv_bButtonClose}";
                /// <summary>
                /// 手動油壓缸降命令 
                /// </summary>
                public static string sv_bButtonOpen => $"{NodeHeader}.{NodeVariable.Engraving1}.{BButton.sv_bButtonOpen}";

                /// <summary>
                /// 油壓缸上升降狀態 
                /// </summary>
                public static string sv_bEngravingClosed => $"{NodeHeader}.{NodeVariable.Engraving1}.sv_bEngravingClosed";
                /// <summary>
                /// 油壓缸下降狀態 
                /// </summary>
                public static string sv_bEngravingOpen => $"{NodeHeader}.{NodeVariable.Engraving1}.sv_bEngravingOpen";

                /// <summary>
                /// DI_刻印z軸原點位置
                /// </summary>
                public static string di_StopUp => $"{NodeHeader}.{NodeVariable.Engraving1}.di_StopUp";
                /// <summary>
                /// DI_刻印z軸待命位置
                /// </summary>
                public static string di_StandbyPoint => $"{NodeHeader}.{NodeVariable.Engraving1}.di_StandbyPoint";
                /// <summary>
                /// DI_刻印z軸刻印位置
                /// </summary>
                public static string di_StopDown => $"{NodeHeader}.{NodeVariable.Engraving1}.di_StopDown";





            }


            /// <summary>
            /// 鋼印旋轉
            /// </summary>
            public class EngravingRotate1
            {
                /// <summary>
                /// 刻印a軸當前站號
                /// </summary>
                public static string sv_iThisStation => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{TargetStation.sv_iThisStation}";
                /// <summary>
                /// 刻印a軸總站數
                /// </summary>
                public static string sv_iTotalSlots => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.sv_iTotalSlots";
                /// <summary>
                /// 刻印a軸當前位置(角度)
                /// </summary>
                //public static string sv_rEngravingRotatePosition => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.sv_rEngravingRotatePosition";

                /// <summary>
                /// 手動前進
                /// </summary>
                public static string sv_bButtonCW => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{BButton.sv_bButtonCW}";
                /// <summary>
                /// 手動後退
                /// </summary>
                public static string sv_bButtonCCW => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{BButton.sv_bButtonCCW}";

                /// <summary>
                /// 設定模式移動速度
                /// </summary>       
                //public static string sv_ConstRotateSetup => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{ConstSpeedSetup.sv_ConstRotateSetup}.{VelocityNode}";
                public static string sv_rRotateSetupVelocity => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.sv_rRotateSetupVelocity";
                public static string sv_rRotateVelocity => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.sv_rRotateVelocity";
                /// <summary>
                /// 手動/自動正轉速度
                /// </summary>
                // public static string sv_ConstRotateCW => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{ConstSpeedSetup.sv_ConstRotateCCW}.{VelocityNode}";
                /// <summary>
                /// 手動/自動逆轉速度
                /// </summary>
                // public static string sv_ConstRotateCCW => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{ConstSpeedSetup.sv_ConstRotateCCW}.{VelocityNode}";




            }

            /// <summary>
            /// 系統
            /// </summary>
            public class system
            {
                /// <summary>
                /// 鋼印目前選定的字元 - 變更鋼印目前選定的字元命令
                /// </summary>
                //public static string sv_iTargetAStation => $"{NodeHeader}.{NodeVariable.system}.{TargetStation.sv_iTargetAStation}";

                /// <summary>
                /// 機台模式
                /// </summary>
                public static string sv_OperationMode => $"{NodeHeader}.{NodeVariable.system}.sv_OperationMode";


                /// <summary>
                /// 進行自動加工時需傳入資料 鐵片下一片資訊
                /// </summary>
                //public static string sv_HMIIronPlateName => $"{NodeHeader}.{NodeVariable.system}.{HMI.sv_HMIIronPlateName}";
                /// <summary>
                /// 鐵片下一片資訊-交握訊號
                /// </summary>
                public static string sv_bRequestDatabit => $"{NodeHeader}.{NodeVariable.system}.sv_bRequestDatabit";

                /// <summary>
                /// 鐵片陣列
                /// </summary>
                public static string sv_IronPlateData => $"{NodeHeader}.{NodeVariable.system}.sv_IronPlateData";
                /// <summary>
                /// 特定鐵片
                /// </summary>
                /// <param name="index"></param>
                /// <returns></returns>
                //public static string sv_IronPlateData_Index(int index) =>   $"{NodeHeader}.{NodeVariable.system}.sv_IronPlateData[{index}]";
                /// <summary>
                /// 開啟QR功能
                /// </summary>
                /// <param name="index"></param>
                /// <returns></returns>
                public static string sv_DataMatrixMode => $"{NodeHeader}.{NodeVariable.system}.sv_DataMatrixMode";

                //  public static string sv_bResetTotEnergy=> $"{NodeHeader}.{NodeVariable.system}.sv_bResetTotEnergy";

                /// <summary>
                /// 人機上直接輸入對應字碼的ascii code滿共40格
                /// </summary>
                public static string sv_RotateCodeDefinition => $"{NodeHeader}.{NodeVariable.system}.sv_RotateCodeDefinition";
                /// <summary>
                /// 人機上直接輸入對應字碼的ascii code滿共40格
                /// </summary>
                /// <param name="index"></param>
                /// <returns></returns>
                public static string sv_RotateCodeDefinitionOfIndex(int index) => $"{NodeHeader}.{NodeVariable.system}.sv_RotateCodeDefinition[{index}]";



                public class sv_HMIIronPlateName
                {
                    public static string NodeName => $"{NodeHeader}.{NodeVariable.system}.{HMI.sv_HMIIronPlateName}";
                }



            }

            /// <summary>
            /// 分料組
            /// </summary>
            public class Stacking1
            {
                /// <summary>
                /// 分料組當前位置
                /// </summary>
                public static string sv_iThisStation => $"{NodeHeader}.{NodeVariable.Stacking1}.{TargetStation.sv_iThisStation}";
                /// <summary>
                /// 手動前進
                /// </summary>
                public static string sv_bButtonFwd => $"{NodeHeader}.{NodeVariable.Stacking1}.{BButton.sv_bButtonFwd}";
                /// <summary>
                /// 手動後退
                /// </summary>
                public static string sv_bButtonBwd => $"{NodeHeader}.{NodeVariable.Stacking1}.{BButton.sv_bButtonBwd}";
            }


            /// <summary>
            /// 油壓單元
            /// </summary>
            public class Motor1
            {
                /// <summary>
                /// 馬達按鈕啟動
                /// </summary>
                public static string sv_bButtonMotor => $"{NodeHeader}.{NodeVariable.Motor1}.{BButton.sv_bButtonMotor}";
                /// <summary>
                /// 馬達啟動
                /// </summary>
                public static string sv_bMotorStarted => $"{NodeHeader}.{NodeVariable.Motor1}.sv_bMotorStarted";
            }

            /// <summary>
            /// 裁切
            /// </summary>
            public class Cutting1
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                //public static string sv_bCuttingOpen => $"{NodeHeader}.{NodeVariable.Cutting1}.{BCutting.sv_bCuttingOpen}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                //public static string sv_bCuttingClosed => $"{NodeHeader}.{NodeVariable.Cutting1}.{BCutting.sv_bCuttingClosed}";
                /// <summary>
                /// 手動油壓缸升命令
                /// </summary>
                public static string sv_bButtonOpen => $"{NodeHeader}.{NodeVariable.Cutting1}.{BButton.sv_bButtonOpen}";
                /// <summary>
                /// 手動油壓缸降命令
                /// </summary>
                public static string sv_bButtonClose => $"{NodeHeader}.{NodeVariable.Cutting1}.{BButton.sv_bButtonClose}";


                /// <summary>
                /// 油壓缸原點
                /// </summary>
                public static string di_CuttingOrigin => $"{NodeHeader}.{NodeVariable.Cutting1}.di_CuttingOrigin";

                /// <summary>
                /// 油壓缸待命位置
                /// </summary>
                public static string di_CuttingStandbyPoint => $"{NodeHeader}.{NodeVariable.Cutting1}.di_CuttingStandbyPoint";

                /// <summary>
                /// 油壓缸切割位置
                /// </summary>
                public static string di_CuttingCutPoint => $"{NodeHeader}.{NodeVariable.Cutting1}.di_CuttingCutPoint";


            }

            /// <summary>
            /// 潤滑
            /// </summary>
            public class Lubrication1
            {
                /// <summary>
                /// DO_潤滑系統ON/OFF
                /// </summary>
                public static string do_Lubrication => $"{NodeHeader}.{NodeVariable.Lubrication1}.do_Lubrication";
                /// <summary>
                /// SW_潤滑按鈕
                /// </summary>
                public static string sv_bButtonLubrication => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_bButtonLubrication";

                public class sv_LubricationSetValues
                {
                    /// <summary>
                    /// 潤滑設定時間
                    /// </summary>
                    public static string dLubTime => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_LubricationSetValues.dLubTime ";

                    /// <summary>
                    /// 潤滑開設定時間
                    /// </summary>
                    /// <returns></returns>
                    public static string dOnTime => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_LubricationSetValues.dOnTime ";
                    /// <summary>
                    /// 潤滑關設定時間
                    /// </summary>
                    /// <returns></returns>
                    public static string dOffTime => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_LubricationSetValues.dOffTime ";
                }



                public class sv_LubricationActValues
                {
                    /// <summary>
                    /// 潤滑實際時間
                    /// </summary>
                    public static string dLubTime => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_LubricationActValues.dLubTime ";

                    /// <summary>
                    /// 潤滑開實際時間
                    /// </summary>
                    /// <returns></returns>
                    public static string dOnTime => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_LubricationActValues.dOnTime ";
                    /// <summary>
                    /// 潤滑關實際時間
                    /// </summary>
                    /// <returns></returns>
                    public static string dOffTime => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_LubricationActValues.dOffTime ";
                }

            }







        }

        #endregion














    }
}
