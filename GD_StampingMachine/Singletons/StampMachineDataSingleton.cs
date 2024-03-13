using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Extensions;
using DevExpress.Xpf.Core;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_MachineConnect.Machine;
using GD_MachineConnect.Machine.Interfaces;
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
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace GD_StampingMachine.Singletons
{
    public class StampMachineDataSingleton : GD_CommonLibrary.BaseSingleton<StampMachineDataSingleton>
    {
        //  public const string DataSingletonName = "Name_StampMachineDataSingleton";
        public string DataSingletonName => (string)Application.Current.TryFindResource("Name_StampMachineDataSingleton");
        private AbstractOpcuaConnect opcUaClient;

        private StampMachineDataSingleton()
        {

        }

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
                    new()
                    {
                        Info = "氣壓總壓檢知(7kg/cm3 up)",
                        BondingCableTerminal = "I00",
                        KEBA_Definition = "DI_00",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_AirPressNotEnough",
                    },
                    new()
                    {
                        Info = "預留",
                        BondingCableTerminal = "I01",
                        KEBA_Definition = "DI_01",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                    },
                    new()
                    {
                        Info = "油壓單元液位檢知(低液位)",
                        BondingCableTerminal = "I02",
                        KEBA_Definition = "DI_02",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.OilMaintenance1.di_OilLevelOk"
                    },
                    new()
                    {
                        Info = "潤滑壓力檢知",
                        BondingCableTerminal = "I03",
                        KEBA_Definition = "DI_03",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Lubrication1.di_LubPressureAchieved"
                    },
                    new()
                    {
                        Info = "潤滑液位檢知(低液位)",
                        BondingCableTerminal = "I04",
                        KEBA_Definition = "DI_04",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Lubrication1.di_LubLimitAchieved"
                    },
                    new()
                    {
                        Info = "放料卷異常",
                        BondingCableTerminal = "I05",
                        KEBA_Definition = "DI_05",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_FeedRollsAbnormal"
                    },
                    new()
                    {
                        Info ="進料有無料件確認檢知" ,
                        BondingCableTerminal = "I06",
                        KEBA_Definition = "DI_06",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Feeding1.di_FeedMaterialConfirm"
                    },
                    new()
                    {
                        Info = "QRcode壓座組1_氣壓缸上限檢知",
                        BondingCableTerminal = "I07",
                        KEBA_Definition = "DI_07",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture1.di_StopUp"
                    },
                    new()
                    {
                        Info = "QRcode壓座組1_氣壓缸下限檢知",
                        BondingCableTerminal = "I08",
                        KEBA_Definition = "DI_08",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture1.di_StopDown"
                    },
                    new()
                    {
                        Info ="阻擋缸_氣壓缸上限檢知" ,
                        BondingCableTerminal = "I09",
                        KEBA_Definition = "DI_09",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture1.di_StopUp"
                    },
                    new()
                    {
                        Info ="阻擋缸_氣壓缸下限檢知" ,
                        BondingCableTerminal = "I10",
                        KEBA_Definition = "DI_10",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture1.di_StopDown"
                    },
                    new()
                    {
                        Info = "料品到QR code 位置檢知",
                        BondingCableTerminal = "I11",
                        KEBA_Definition = "DI_11",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.QRCode1.di_QRCodeMaterialConfirm"
                    },
                    new()
                    {
                        Info = "打字輪_Y軸行程 + 極限檢知 0 位置",
                        BondingCableTerminal = "I12",
                        KEBA_Definition = "DI_12",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.EngravingFeeding1.di_ServoHome"
                    },
                    new()
                    {
                        Info = "打字輪_Y軸行程 - 極限檢知 260 位置",
                        BondingCableTerminal = "I13",
                        KEBA_Definition = "DI_13",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.EngravingFeeding1.di_ServoPOT"
                    },
                    new()
                    {
                        Info = "預留",
                        BondingCableTerminal = "I14",
                        KEBA_Definition = "DI_14",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info ="預留" ,
                        BondingCableTerminal = "I15",
                        KEBA_Definition = "DI_15",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "料品到字碼刻印位置檢知",
                        BondingCableTerminal = "I16",
                        KEBA_Definition = "DI_16",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Engraving1.di_EngravingMaterialConfirm"
                    },

                    new()
                    {
                        Info = "進料X軸_原點",
                        BondingCableTerminal = "I17",
                        KEBA_Definition = "DI_17",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Feeding1.di_ServoHome"
                    },
                    new()
                    {
                        Info ="進料X軸_負極限" ,
                        BondingCableTerminal = "I18",
                        KEBA_Definition = "DI_18",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Feeding1.di_ServoNOT"
                    },

                    new()
                    {
                        Info = "刻印壓座組2_氣壓缸上限檢知",
                        BondingCableTerminal = "I00",
                        KEBA_Definition = "DI_100",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture2.di_StopUp"
                    },
                    new()
                    {
                        Info = "刻印壓座組2_氣壓缸下限檢知",
                        BondingCableTerminal = "I01",
                        KEBA_Definition = "DI_101",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture2.di_StopDown"
                    },
                    new()
                    {
                        Info ="字碼刻印組_Z軸油壓缸刻印位置" ,
                        BondingCableTerminal = "I02",
                        KEBA_Definition = "DI_102",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Engraving1.di_StopDown"
                    },
                    new()
                    {
                        Info = "字碼刻印組_Z軸油壓缸原點位置",
                        BondingCableTerminal = "I03",
                        KEBA_Definition = "DI_103",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Engraving1.di_StopUp"
                    },
                    new()
                    {
                        Info ="字碼刻印組_刻印轉輪原點位置" ,
                        BondingCableTerminal = "I04",
                        KEBA_Definition = "DI_104",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.EngravingRotate1.di_ServoHome"
                    },
                    new()
                    {
                        Info = "進料導桿缸1_氣壓缸上限檢知",
                        BondingCableTerminal = "I05",
                        KEBA_Definition = "DI_105",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.GuideRodsFixture1.di_StopUp"
                    },
                    new()
                    {
                        Info = "進料導桿缸1_氣壓缸下限檢知",
                        BondingCableTerminal = "I06",
                        KEBA_Definition = "DI_106",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.GuideRodsFixture1.di_StopDown"
                    },
                    new()
                    {
                        Info ="料品到裁切位置檢知" ,
                        BondingCableTerminal = "I07",
                        KEBA_Definition = "DI_107",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Cutting1.di_CuttingMaterialConfirm"
                    },
                    new()
                    {
                        Info = "裁切模組_位置檢知_上",
                        BondingCableTerminal = "I08",
                        KEBA_Definition = "DI_108",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Cutting1.di_CuttingOrigin"
                    },
                    new()
                    {
                        Info = "裁切模組_位置檢知_中",
                        BondingCableTerminal = "I09",
                        KEBA_Definition = "DI_109",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Cutting1.di_CuttingStandbyPoint"
                    },
                    new()
                    {
                        Info = "裁切模組_位置檢知_下",
                        BondingCableTerminal = "I10",
                        KEBA_Definition = "DI_110",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Cutting1.di_CuttingCutPoint"
                    },
                    new()
                    {
                        Info = "阻擋缸_進退動作-SW",
                        BondingCableTerminal = "I11",
                        KEBA_Definition = "DI_111",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "緊急停止-SW",
                        BondingCableTerminal = "I12",
                        KEBA_Definition = "DI_112",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.OperationMode1.di_EmergencyStop1"
                    },
                    new()
                    {
                        Info ="開機(power on)-SW" ,
                        BondingCableTerminal = "I13",
                        KEBA_Definition = "DI_113",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_PowerON"
                    },
                    new()
                    {
                        Info ="暫停(pause)-SW" ,
                        BondingCableTerminal = "I14",
                        KEBA_Definition = "DI_114",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_Pause"
                    },

                    new()
                    {
                        Info = "開始加工(start)-SW",
                        BondingCableTerminal = "I15",
                        KEBA_Definition = "DI_115",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_Start"
                    },

                    new()
                    {
                        Info ="放料卷方向" ,
                        BondingCableTerminal = "I16",
                        KEBA_Definition = "DI_116",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_FeedRollsDirection"
                    },
                    new()
                    {
                        Info = "原點復歸-SW",
                        BondingCableTerminal = "I17",
                        KEBA_Definition = "DI_117",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.system.di_Home"
                    },
                    new()
                    {
                        Info ="半自動-SW" ,
                        BondingCableTerminal = "I18",
                        KEBA_Definition = "DI_118",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.OperationMode1.di_ButtonHalfAuto"
                    },

                    new()
                    {
                        Info = "全自動",
                        BondingCableTerminal = "I00",
                        KEBA_Definition = "DI_200",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.OperationMode1.di_ButtonFullAuto"
                    },
                    new()
                    {
                        Info = "預留",
                        BondingCableTerminal = "I01",
                        KEBA_Definition = "DI_201",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "進料導桿缸2_氣壓缸上限檢知",
                        BondingCableTerminal = "I02",
                        KEBA_Definition = "DI_202",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.GuideRodsFixture2.di_StopUp"
                    },
                    new()
                    {
                        Info = "進料導桿缸2_氣壓缸下限檢知",
                        BondingCableTerminal = "I03",
                        KEBA_Definition = "DI_203",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.GuideRodsFixture2.di_StopDown"
                    },

                    new()
                    {
                        Info = "鑰匙開關_自動",
                        BondingCableTerminal = "I04",
                        KEBA_Definition = "DI_204",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "鑰匙開關_鎖固",
                        BondingCableTerminal = "I05",
                        KEBA_Definition = "DI_205",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "鑰匙開關_手動以上兩個訊號沒on為手動",
                        BondingCableTerminal = "I06",
                        KEBA_Definition = "DI_206",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "X+搖桿",
                        BondingCableTerminal = "I07",
                        KEBA_Definition = "DI_207",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "X-搖桿",
                        BondingCableTerminal = "I08",
                        KEBA_Definition = "DI_208",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "Y+搖桿",
                        BondingCableTerminal = "I09",
                        KEBA_Definition = "DI_209",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "Y-搖桿",
                        BondingCableTerminal = "I10",
                        KEBA_Definition = "DI_210",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "油壓過載",
                        BondingCableTerminal = "I11",
                        KEBA_Definition = "DI_211",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Motor1.di_Overload"
                    },
                    new()
                    {
                        Info = "刻印Y軸_負極限",
                        BondingCableTerminal = "I12",
                        KEBA_Definition = "DI_212",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.EngravingFeeding1.di_ServoNOT"
                    },
                    new()
                    {
                        Info = "進料X軸_正極限",
                        BondingCableTerminal = "I13",
                        KEBA_Definition = "DI_213",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Feeding1.di_ServoPOT"
                    },

                    new()
                    {
                        Info = "字碼刻印組_Z軸油壓缸待命位置",
                        BondingCableTerminal = "I14",
                        KEBA_Definition = "DI_214",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Engraving1.di_StandbyPoint"
                    },
                    new()
                    {
                        Info = "搖桿",
                        BondingCableTerminal = "I15",
                        KEBA_Definition = "DI_215",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "搖桿",
                        BondingCableTerminal = "I16",
                        KEBA_Definition = "DI_216",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "搖桿",
                        BondingCableTerminal = "I17",
                        KEBA_Definition = "DI_217",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "搖桿",
                        BondingCableTerminal = "I18",
                        KEBA_Definition = "DI_218",
                        SensorType = ioSensorType.DI,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },




                    new()
                    {
                        Info = "油壓單元啟動",
                        BondingCableTerminal = "O00",
                        KEBA_Definition = "DO_00",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Motor1.do_MotorOnMain"
                    },
                    new()
                    {
                        Info = "潤滑系統ON/OFF",
                        BondingCableTerminal = "O01",
                        KEBA_Definition = "DO_01",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Lubrication1.do_Lubrication"
                    },
                    new()
                    {
                        Info = "壓座組1_氣壓缸動作",
                        BondingCableTerminal = "O02",
                        KEBA_Definition = "DO_02",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture1.do_FixtureUp"
                    },
                    new()
                    {
                        Info = "預留",
                        BondingCableTerminal = "O03",
                        KEBA_Definition = "DO_03",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },
                    new()
                    {
                        Info = "阻擋缸_氣壓缸推出",
                        BondingCableTerminal = "O04",
                        KEBA_Definition = "DO_04",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.BlockingClips1.do_BlockingClipsUp"
                    },
                    new()
                    {
                        Info = "刻印壓座組2_氣壓缸動作",
                        BondingCableTerminal = "O05",
                        KEBA_Definition = "DO_05",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Fixture2.do_FixtureUp"
                    },
                    new()
                    {
                        Info = "字碼刻印組_油壓缸上升",
                        BondingCableTerminal = "O06",
                        KEBA_Definition = "DO_06",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Engraving1.do_Open"
                    },
                    new()
                    {
                        Info = "字碼刻印組_油壓缸下降",
                        BondingCableTerminal = "O07",
                        KEBA_Definition = "DO_07",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Engraving1.do_Close"
                    },
                    new()
                    {
                        Info = "字碼刻印組_制動煞車",
                        BondingCableTerminal = "O08",
                        KEBA_Definition = "DO_08",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.EngravingRotate1.do_brake"
                    },
                    new()
                    {
                        Info = "裁切模組_上升",
                        BondingCableTerminal = "O09",
                        KEBA_Definition = "DO_09",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Cutting1.do_Open"
                    },
                    new()
                    {
                        Info = "裁切模組_下降",
                        BondingCableTerminal = "O10",
                        KEBA_Definition = "DO_10",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.Cutting1.do_Close"
                    },
                    new()
                    {
                        Info = "進料導桿缸1-氣壓缸動作",
                        BondingCableTerminal = "O11",
                        KEBA_Definition = "DO_11",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.GuideRodsFixture1.do_GuideRodsFixtureUp"
                    },
  new()
                    {
                        Info = "進料導桿缸2-氣壓缸動作",
                        BondingCableTerminal = "O12",
                        KEBA_Definition = "DO_12",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $"{opcuaNodeHeader}.GuideRodsFixture2.do_GuideRodsFixtureUp"
                    },

                    new()
                    {
                        Info = "",
                        BondingCableTerminal = "O13",
                        KEBA_Definition = "DO_13",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },

                    new()
                    {
                        Info = "",
                        BondingCableTerminal = "O14",
                        KEBA_Definition = "DO_14",
                        SensorType = ioSensorType.DO,
                        ValueType = typeof(bool),
                        NodeID = $""
                    },

                    new()
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

            //一啟動就建立一個alarm
            //HasAlarm = true;
            Application.Current.Dispatcher.Invoke(() =>
            {
                AlarmMessageCollection.Add("PLC Is Not Connected");
            });

            AlarmLamp = true;
            _ = LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, "PLC Is Not Connected", true);


            //啟動時預設值

            for (int i = 0; i < MachineConst.PlateCount; i++)
            {
                PlateBaseObservableCollection.Add(new PlateMonitorViewModel()
                {
                    SettingBaseVM = new QRSettingViewModel()
                    { 
                        PlateNumber="Init-" +i,
                        QR_Special_Text=string.Empty,
                        QrCodeContent = i.ToString(),
                    }
                });
            }


            
            _ = Task.Run(async () =>
            {
            while (Debugger.IsAttached && !IsConnected)
            {
                try
                {
                        for (int i = 0; i < MachineConst.PlateCount; i++)
                        {
                            Random random = new Random();
                            var randomDouble = random.NextDouble() * 100;
                            
                            await Application.Current.Dispatcher.InvokeAsync(async () =>
                            {
                                PlateBaseObservableCollection[i] = (new PlateMonitorViewModel()
                                {
                                    SettingBaseVM = new QRSettingViewModel()
                                    {
                                        PlateNumber = "DEMO-" + randomDouble,
                                        QR_Special_Text = string.Empty,
                                        QrCodeContent = i.ToString(),
                                    }
                                });
                            });
                            await Task.Delay(100);
                        }
                    }
                    catch(Exception ex)
                    {
                        Debugger.Break();
                    }
                    await Task.Delay(1000);
                }

            });







        }







        protected override async ValueTask DisposeAsyncCoreAsync()
        {
            await StopScanOpcuaAsync();
            if (opcUaClient != null)
                await this.DisconnectAsync();
        }



        private CommunicationSettingModel? _communicationSetting;
        public CommunicationSettingModel CommunicationSetting
        {
            get => _communicationSetting ??= new CommunicationSettingModel();
            set => _communicationSetting = value;
        }



        //private bool ContinueScanning = true;
        private bool _isScanning = false;
        public bool IsScanning
        {
            get => _isScanning;
            private set
            {
                _isScanning = value; OnPropertyChanged();
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








        private Task? scanTask;
        private CancellationTokenSource? _cts;


        public async Task StartScanOpcuaAsync()
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
             
                try
                {
                    //scanTask = Task.Run(async () => await RunScanTaskAsync(token));
                    scanTask = RunScanTaskAsync(_cts);
                }
                catch (OperationCanceledException ocex)
                {
                    _ = LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, ocex.Message, true);
                }
                catch (Exception ex)
                {
                    _ = LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, ex.Message, true);
                }
            }

        }

        public async Task StopScanOpcuaAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    //等待掃描解除
                    _cts?.Cancel();
                    //await this.DisconnectAsync();
                    if (scanTask != null)
                    {
                        await Task.WhenAny(scanTask);
                        scanTask = null;
                    }
                }
                catch (Exception ex)
                {
                    _ = LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, ex.Message, true);
                }
            });
        }


        private Task RunScanTaskAsync(CancellationTokenSource cancelTokenSoruce)
        {
            var token = cancelTokenSoruce.Token;
            return Task.Run(async () =>
                {
                    IsScanning = true;
                    var ManagerVM = new DevExpress.Mvvm.DXSplashScreenViewModel
                    {
                        Logo = new Uri(@"pack://application:,,,/GD_StampingMachine;component/Image/svg/NewLogo_1-2.svg"),
                        Status = (string)System.Windows.Application.Current.TryFindResource("Connection_Init"),
                        Progress = 0,
                        IsIndeterminate = true,
                        Subtitle = null,
                        Copyright = null,
                    };
                    DevExpress.Xpf.Core.SplashScreenManager? manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.ProcessingScreenWindow(), ManagerVM);
                    try
                    {
                        do
                        {
                            if (token.IsCancellationRequested)
                            {
                                token.ThrowIfCancellationRequested();
                            }


                            try
                            {
                                if (opcUaClient != null)
                                    await this.DisconnectAsync();

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    manager?.Show(Application.Current?.MainWindow, WindowStartupLocation.CenterScreen, true, InputBlockMode.None);
                                });

                                this.IsConnected = false;
                                opcUaClient = new GD_OpcUaClient();
                                opcUaClient.IsConnectedChanged += (sender, e) =>
                                {
                                    this.IsConnected = e.NewValue;
                                };

                                ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_Init");
                                //while (true)
                                //初始化連線
                                int retryCount = 1;

                                var firstConnected = false;
                                do
                                {
                                    if (token.IsCancellationRequested)
                                    {
                                        token.ThrowIfCancellationRequested();
                                    }
                                    try
                                    {
                                        var hostPath = CommunicationSetting.HostString;
                                        if (!CommunicationSetting.HostString.Contains("opc.tcp://"))
                                            hostPath = "opc.tcp://" + hostPath;
                                        if (CommunicationSetting.Port.HasValue)
                                            hostPath += $":{CommunicationSetting.Port.Value}";
                                        var baseUrl = new Uri(new Uri(hostPath), CommunicationSetting.ServerDataPath);

                                        if (TcpPing.RetrieveIpAddress(hostPath, out var _ip))
                                        {
                                            if (!await TcpPing.IsPingableAsync(_ip) && !Debugger.IsAttached)
                                            {
                                                throw new PingException($"Ping Host: {_ip} is Failed");
                                            }
                                        }

                                        firstConnected = await opcUaClient.ConnectAsync(baseUrl.ToString(), CommunicationSetting.UserName, CommunicationSetting.Password);
                                        if (firstConnected)
                                        {
                                            await Application.Current.Dispatcher.InvokeAsync(() =>
                                            {
                                                AlarmMessageCollection.Clear();
                                            });

                                            await this.WriteNodeAsync<bool>($"{StampingOpcUANode.system.sv_bComputerBootUpComplete}", true);
                                            ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_IsSuccessful");
                                            ManagerVM.Subtitle = string.Empty;

                                            var feedingVelocityTuple = await GetFeedingVelocityAsync();
                                            if (feedingVelocityTuple.Item1)
                                                this.FeedingVelocity = feedingVelocityTuple.Item2;

                                            var engravingFeedingTuple = await GetEngravingFeedingVelocityAsync();
                                            if (engravingFeedingTuple.Item1)
                                                this.EngravingFeeding = engravingFeedingTuple.Item2;


                                            var rotateVelocityTuple = await GetEngravingRotateVelocityAsync();
                                            if (rotateVelocityTuple.Item1)
                                                this.RotateVelocity = rotateVelocityTuple.Item2;


                                            var dataMatrixTCPIPTuple = await GetDataMatrixTCPIPAsync();
                                            if (dataMatrixTCPIPTuple.Item1)
                                                this.DataMatrixTCPIP = dataMatrixTCPIPTuple.Item2;


                                            var dataMatrixPortTuple = await GetDataMatrixPortAsync();
                                            if (dataMatrixPortTuple.Item1)
                                            {
                                                if (int.TryParse(dataMatrixPortTuple.Item2, out var port))
                                                    this.DataMatrixPort = port;
                                                this.DataMatrixPort = 0;
                                            }


                                            var stampingPressureTuple = await GetStampingPressureAsync();
                                            if (stampingPressureTuple.Item1)
                                                this.StampingPressure = stampingPressureTuple.Item2;

                                            var stampingVelocityTuple = await GetStampingVelocityAsync();
                                            if (stampingVelocityTuple.Item1)
                                                this.StampingVelocity = stampingVelocityTuple.Item2;

                                            var shearingPressureTuple = await GetShearingPressureAsync();
                                            if (shearingPressureTuple.Item1)
                                                this.ShearingPressure = shearingPressureTuple.Item2;

                                            var shearingVelocityTuple = await GetShearingVelocityAsync();
                                            if (shearingVelocityTuple.Item1)
                                                this.ShearingVelocity = shearingVelocityTuple.Item2;









                                            var AlarmLamp_IsTrigger = GetAlarmLampAsync();
                                            if ((await AlarmLamp_IsTrigger).ret)
                                            {
                                                AlarmLamp = (await AlarmLamp_IsTrigger).lamp;
                                                if (!AlarmLamp)
                                                {
                                                    Application.Current.Dispatcher.Invoke(new Action(() =>
                                                    AlarmMessageCollection.Clear()
                                                    ));
                                                }
                                            }

                                            var ret10 = await GetLubricationActualTimeAsync();
                                            var ret11 = await GetLubricationActualOnTimeAsync();
                                            var ret12 = await GetLubricationActualOffTimeAsync();

                                            var engravingRotateSVelocity = await GetEngravingRotateSetupVelocityAsync();
                                            var engravingFeedingSVelocity = await GetEngravingFeedingSetupVelocityAsync();
                                            //初始化後直接設定其他數值

                                            /*var firstIronPlateIDTuple = await GetFirstIronPlateIDAsync();
                                            if (firstIronPlateIDTuple.Item1)
                                            {
                                                FirstIronPlateID = (0, firstIronPlateIDTuple.Item2);
                                            }*/


                                            //檢查字模

                                            await Task.Delay(1000);

                                            ObservableCollection<StampingTypeViewModel> rotatingStampingTypeVMObservableCollection = new();
                                            var rotatingTurntableInfoList = await GetRotatingTurntableInfoAsync();
                                            if (rotatingTurntableInfoList.Item1)
                                            {
                                                rotatingTurntableInfoList.Item2.ForEach(stamp =>
                                                {
                                                    rotatingStampingTypeVMObservableCollection.Add(new StampingTypeViewModel(stamp));
                                                });

                                                _ = await CompareFontsSettingBetweenMachineAndSoftwareAsync(null,
                                                     StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection, rotatingStampingTypeVMObservableCollection
                                                     );
                                            }


                                            await opcUaClient.SubscribeNodeDataChangeAsync<int>(StampingOpcUANode.system.sv_OperationMode, (sender, e) =>
                                            {
                                                OperationMode = (OperationModeEnum)e.NewValue;
                                            });


                                            await opcUaClient.SubscribeNodeDataChangeAsync<bool>(StampingOpcUANode.Motor1.sv_bMotorStarted, (sender, e) =>
                                            {
                                                HydraulicPumpIsActive = e.NewValue;
                                            });


                                            await opcUaClient.SubscribeNodeDataChangeAsync<bool>(StampingOpcUANode.system.sv_bRequestDatabit, (sender, e) =>
                                            {
                                                Rdatabit = e.NewValue;
                                            }, 500);


                                            await opcUaClient.SubscribeNodeDataChangeAsync<int>(StampingOpcUANode.EngravingRotate1.sv_iThisStation, (sender, e) =>
                                            {
                                                //value = 1~40 對應index = 0~39
                                                EngravingRotateStation = e.NewValue - 1;
                                                if (Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection.TryGetValue(EngravingRotateStation - 1, out var stamptype))
                                                {
                                                    Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeModel_ReadyStamping = stamptype;
                                                }
                                            });


                                            await opcUaClient.SubscribeNodeDataChangeAsync<float>(StampingOpcUANode.Feeding1.sv_rFeedVelocity,
                                            (sender, e) =>
                                            {
                                                try
                                                {
                                                    this.FeedingVelocity = e.NewValue;
                                                }
                                                catch
                                                {

                                                }
                                            });

                                            await opcUaClient.SubscribeNodeDataChangeAsync<float>(StampingOpcUANode.EngravingFeeding1.sv_rFeedVelocity,
                                                    (sender, e) => this.EngravingFeeding = e.NewValue);

                                            await opcUaClient.SubscribeNodeDataChangeAsync<float>(StampingOpcUANode.EngravingRotate1.sv_rRotateVelocity,
                                                   (sender, e) => this.RotateVelocity = e.NewValue);
                                            await this.opcUaClient.SubscribeNodeDataChangeAsync<string>(StampingOpcUANode.DataMatrix1.sv_sContactTCPIP,
                                                   (sender, e) => this.DataMatrixTCPIP = e.NewValue);
                                            await this.opcUaClient.SubscribeNodeDataChangeAsync<int>(StampingOpcUANode.DataMatrix1.sv_sContactTCPPort,
                                                   (sender, e) => this.DataMatrixPort = e.NewValue);

                                            await this.opcUaClient.SubscribeNodeDataChangeAsync<bool>(StampingOpcUANode.system.di_PowerON,
                                                (sender, e) => this.DI_PowerON = e.NewValue);
                                            await this.opcUaClient.SubscribeNodeDataChangeAsync<bool>(StampingOpcUANode.system.di_Pause,
                                                    (sender, e) => this.DI_Pause = e.NewValue);
                                            await this.opcUaClient.SubscribeNodeDataChangeAsync<bool>(StampingOpcUANode.system.di_Start,
                                                (sender, e) => this.DI_Start = e.NewValue);
                                            await this.opcUaClient.SubscribeNodeDataChangeAsync<bool>(StampingOpcUANode.OperationMode1.di_EmergencyStop1,
                                                (sender, e) => this.DI_EmergencyStop1 = e.NewValue);

                                            await this.opcUaClient.SubscribeNodeDataChangeAsync<bool>(StampingOpcUANode.OperationMode1.do_RunningLamp,
                                                (sender, e) => this.RunningLamp = e.NewValue);
                                            await this.opcUaClient.SubscribeNodeDataChangeAsync<bool>(StampingOpcUANode.OperationMode1.do_StoppedLamp,
                                                (sender, e) => this.StoppedLamp = e.NewValue);
                                            await this.opcUaClient.SubscribeNodeDataChangeAsync<bool>(StampingOpcUANode.OperationMode1.do_AlarmLamp,
                                                (sender, e) =>
                                                {
                                                    this.AlarmLamp = e.NewValue;
                                                    if (!e.NewValue)
                                                    {
                                                        Application.Current.Dispatcher.Invoke(new Action(() =>
                                                        AlarmMessageCollection.Clear()
                                                        ));
                                                    }
                                                });



                                            //切割位置
                                            await SubscribeHydraulicCutting_Position_CutPointAsync(value =>
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




                                            await opcUaClient.SubscribeNodeDataChangeAsync<bool>($"{StampingOpcUANode.Engraving1.di_StopDown}",
                                                (sender, e) =>
                                            {
                                                Cylinder_HydraulicEngraving_IsStopDown = e.NewValue;
                                            }, 10);

                                            await this.opcUaClient.SubscribeNodeDataChangeAsync<string>(StampingOpcUANode.OpcMonitor1.sv_OpcLastAlarmText,
                                                (sender, e) =>
                                                {
                                                    if (!string.IsNullOrWhiteSpace(e.NewValue))
                                                        if (!AlarmMessageCollection.Contains(e.NewValue))
                                                        {
                                                            _ = Application.Current.Dispatcher.InvokeAsync(() =>
                                                            {
                                                                AlarmMessageCollection.Add(e.NewValue);
                                                                _ = LogDataSingleton.Instance.AddLogDataAsync(DataSingletonName, e.NewValue, true);
                                                            });
                                                        }
                                                });

                                            await this.opcUaClient.SubscribeNodeDataChangeAsync<bool>(StampingOpcUANode.OperationMode1.sv_bButtonAlarmConfirm,
                                                (sender, e) =>
                                                {
                                                    if (e.NewValue)
                                                    {
                                                        Application.Current.Dispatcher.Invoke(() =>
                                                        {
                                                            AlarmMessageCollection.Clear();
                                                        });
                                                    }
                                                });





                                            break;
                                        }
                                        else
                                        {
                                            ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_RetryConnect") + $" - {retryCount}";
                                            //ManagerVM.Subtitle = opcUaClient.ConnectException?.Message;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        IsConnected = false;
                                        ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_RetryConnect") + $" - {retryCount}";
                                        ManagerVM.Subtitle = ex.Message;
                                    }
                                    retryCount++;
                                    await Task.Delay(500);
                                }
                                while (!firstConnected);


                                manager?.Close();
                                var machineTask = Task.Run(async () =>
                                {
                                    try
                                    {
                                        do
                                        {
                                            if (token.IsCancellationRequested)
                                            {
                                                token.ThrowIfCancellationRequested();
                                            }

                                            this.IsConnected = opcUaClient?.IsConnected == true;
                                            if (IsConnected)
                                            {
                                                try
                                                {
                                                    manager?.Close();

                                                    var alarmTask = await this.GetAlarmMessageAsync();
                                                    if (alarmTask.Item1)
                                                    {
                                                        var message = alarmTask.Item2;
                                                        if (!AlarmMessageCollection.Contains(message))
                                                        {
                                                            await Application.Current.Dispatcher.InvokeAsync(() =>
                                                            {
                                                                AlarmMessageCollection.Add(message);
                                                                _ = LogDataSingleton.Instance.AddLogDataAsync(DataSingletonName, message, true);
                                                            });
                                                        }
                                                    }

                                                    var fPos = await GetFeedingPositionAsync();
                                                    if (fPos.Item1)
                                                        FeedingPosition = fPos.Item2;

                                                    var HmiIronPlateTask = await GetHMIIronPlateAsync();
                                                    if (HmiIronPlateTask.Item1)
                                                    {
                                                        HMIIronPlateDataModel = HmiIronPlateTask.Item2;
                                                    }

                                                    var rotatingTurntableInfoList = await GetRotatingTurntableInfoAsync();
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
                                                                await LogDataSingleton.Instance.AddLogDataAsync(DataSingletonName, ex.Message);
                                                                //RotatingTurntableInfoCollection = rotatingStampingTypeVMObservableCollection;
                                                            }
                                                        }

                                                    }

                                                    var Move_IsUp = GetGuideRod_Move_Position_isUpAsync();
                                                    if ((await Move_IsUp).Item1)
                                                        Cylinder_GuideRod_Move_IsUp = (await Move_IsUp).Item2;

                                                    var Move_IsDown = GetGuideRod_Move_Position_isDownAsync();
                                                    if ((await Move_IsDown).Item1)
                                                        Cylinder_GuideRod_Move_IsDown = (await Move_IsDown).Item2;

                                                    var Fixed_IsUp = GetGuideRod_Fixed_Position_isUpAsync();
                                                    if ((await Fixed_IsUp).Item1)
                                                        Cylinder_GuideRod_Fixed_IsUp = (await Fixed_IsUp).Item2;

                                                    var Fixed_IsDown = GetGuideRod_Fixed_Position_isDownAsync();
                                                    if ((await Fixed_IsDown).Item1)
                                                        Cylinder_GuideRod_Fixed_IsDown = (await Fixed_IsDown).Item2;

                                                    var QRStamping_IsUp = GetQRStamping_Position_isUpAsync();
                                                    if ((await QRStamping_IsUp).Item1)
                                                        Cylinder_QRStamping_IsUp = (await QRStamping_IsUp).Item2;

                                                    var QRStamping_IsDown = GetQRStamping_Position_isDownAsync();
                                                    if ((await QRStamping_IsDown).Item1)
                                                        Cylinder_QRStamping_IsDown = (await QRStamping_IsDown).Item2;

                                                    var StampingSeat_IsUp = GetStampingSeat_Position_isUpAsync();
                                                    if ((await StampingSeat_IsUp).Item1)
                                                        Cylinder_StampingSeat_IsUp = (await StampingSeat_IsUp).Item2;

                                                    var StampingSeat_IsDown = GetStampingSeat_Position_isDownAsync();
                                                    if ((await StampingSeat_IsDown).Item1)
                                                        Cylinder_StampingSeat_IsDown = (await StampingSeat_IsDown).Item2;

                                                    var BlockingCylinder_IsUp = GetBlockingCylinder_Position_isUpAsync();
                                                    if ((await BlockingCylinder_IsUp).Item1)
                                                        Cylinder_BlockingCylinder_IsUp = (await BlockingCylinder_IsUp).Item2;

                                                    var BlockingCylinder_IsDown = GetBlockingCylinder_Position_isDownAsync();
                                                    if ((await BlockingCylinder_IsDown).Item1)
                                                        Cylinder_BlockingCylindere_IsDown = (await BlockingCylinder_IsDown).Item2;



                                                    var HydraulicEngraving_IsOrigin = GetHydraulicEngraving_Position_OriginAsync();
                                                    if ((await HydraulicEngraving_IsOrigin).Item1)
                                                        Cylinder_HydraulicEngraving_IsOrigin = (await HydraulicEngraving_IsOrigin).Item2;

                                                    var HydraulicEngraving_IsStandbyPoint = GetHydraulicEngraving_Position_StandbyPointAsync();
                                                    if ((await HydraulicEngraving_IsStandbyPoint).Item1)
                                                        Cylinder_HydraulicEngraving_IsStandbyPoint = (await HydraulicEngraving_IsStandbyPoint).Item2;

                                                    var HydraulicEngraving_IsStopDown = GetHydraulicEngraving_Position_StopDownAsync();
                                                    if ((await HydraulicEngraving_IsStopDown).Item1)
                                                        Cylinder_HydraulicEngraving_IsStopDown = (await HydraulicEngraving_IsStopDown).Item2;

                                                    var HydraulicCutting_IsOrigin = GetHydraulicCutting_Position_OriginAsync();
                                                    if ((await HydraulicCutting_IsOrigin).Item1)
                                                        Cylinder_HydraulicCutting_IsOrigin = (await HydraulicCutting_IsOrigin).Item2;

                                                    var HydraulicCutting_IsStandbyPoint = GetHydraulicCutting_Position_StandbyPointAsync();
                                                    if ((await HydraulicCutting_IsStandbyPoint).Item1)
                                                        Cylinder_HydraulicCutting_IsStandbyPoint = (await HydraulicCutting_IsStandbyPoint).Item2;

                                                    var HydraulicCutting_IsCutPoint = GetHydraulicCutting_Position_CutPointAsync();
                                                    if ((await HydraulicCutting_IsCutPoint).Item1)
                                                        Cylinder_HydraulicCutting_IsCutPoint = (await HydraulicCutting_IsCutPoint).Item2;

                                                    var RequestDatabit = GetRequestDatabitAsync();
                                                    if ((await RequestDatabit).Item1)
                                                        Rdatabit = (await RequestDatabit).Item2;

                                                    var engravingRotateStation = await GetEngravingRotateStationAsync();
                                                    if (engravingRotateStation.Item1)
                                                    {
                                                        EngravingRotateStation = engravingRotateStation.Item2;
                                                        /* if (Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeVMObservableCollection.TryGetValue((await engravingRotateStation).Item2, out var stamptype))
                                                         {
                                                             Singletons.StampingMachineSingleton.Instance.StampingFontChangedVM.StampingTypeModel_ReadyStamping = stamptype;
                                                         }*/
                                                    }


                                                    //箱子
                                                    var boxIndex = GetSeparateBoxNumberAsync();
                                                    if ((await boxIndex).Item1)
                                                    {
                                                        SeparateBoxIndex = (await boxIndex).Item2;
                                                    }

                                                    var engravingYPosition = GetEngravingYAxisPositionAsync();
                                                    if ((await engravingYPosition).Item1)
                                                        EngravingYAxisPosition = (await engravingYPosition).Item2;

                                                    /*      var engravingZposition = GetEngravingZAxisPositionAsync();
                                                          if ((await engravingZposition).Item1)
                                                              EngravingZAxisPosition = (await engravingZposition).Item2;*/

                                                    var engravingAStation = GetEngravingRotateStationAsync();
                                                    if ((await engravingAStation).Item1)
                                                        EngravingRotateStation = (await engravingAStation).Item2;



                                                }
                                                catch (Exception ex)
                                                {
                                                    await LogDataSingleton.Instance.AddLogDataAsync(DataSingletonName, ex.Message);
                                                    Debugger.Break();
                                                }
                                                finally
                                                {

                                                }
                                                await Task.Delay(2000);
                                            }
                                        }
                                        while (IsConnected);

                                        if (!IsConnected)
                                        {
                                            await Application.Current.Dispatcher.InvokeAsync(() =>
                                            {
                                                if (manager.State == DevExpress.Mvvm.SplashScreenState.Closed)
                                                {
                                                    manager = null;
                                                    manager = DevExpress.Xpf.Core.SplashScreenManager.Create(() => new GD_CommonLibrary.SplashScreenWindows.ProcessingScreenWindow(), ManagerVM);
                                                    manager.Show(Application.Current?.MainWindow, WindowStartupLocation.CenterScreen, true, InputBlockMode.None);
                                                }
                                                ManagerVM.Status = (string)System.Windows.Application.Current.TryFindResource("Connection_RetryConnect");
                                            });
                                        }



                                    }
                                    catch (Exception ex)
                                    {
                                        await LogDataSingleton.Instance.AddLogDataAsync(DataSingletonName, ex.Message);
                                    }
                                }, token);

                                var ironPlateCollectionTask = Task.Run(async () =>
                                {
                                    try
                                    {
                                        while (true)
                                        {
                                            if (IsConnected)
                                            {
                                                var (result, ironPlateCollection) = await GetIronPlateDataCollectionAsync();
                                                if (result)
                                                {
                                                    List<IronPlateDataModel> plateDataCollection = ironPlateCollection;
                                                    var plateMonitorVMCollection = new List<PlateMonitorViewModel>();
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

                                                        int rowLength = Math.Max(string1Length, string2Length);
                                                        SettingBaseViewModel settingBaseVM;
                                                        //沒有QR加工
                                                        if (string.IsNullOrEmpty(plateData.sDataMatrixName1) && string.IsNullOrEmpty(plateData.sDataMatrixName2))
                                                        {
                                                            settingBaseVM = new NumberSettingViewModel();
                                                        }
                                                        else
                                                        {
                                                            settingBaseVM = new QRSettingViewModel();
                                                        }
                                                        settingBaseVM.SpecialSequence = specialSequence;
                                                        settingBaseVM.SequenceCount = rowLength;
                                                        settingBaseVM.PlateNumber = string.Concat(plateData.sIronPlateName1.PadRight(rowLength).AsSpan(0, rowLength), plateData.sIronPlateName2);
                                                        settingBaseVM.QrCodeContent = plateData.sDataMatrixName1;
                                                        settingBaseVM.QR_Special_Text = plateData.sDataMatrixName2;
                                                        settingBaseVM.StampingMarginPosVM = new StampingMarginPosViewModel()
                                                        {
                                                            rXAxisPos1 = plateData.rXAxisPos1,
                                                            rYAxisPos1 = plateData.rYAxisPos1 - MachineConst.StampingMachineYPosition,
                                                            rXAxisPos2 = plateData.rXAxisPos2,
                                                            rYAxisPos2 = plateData.rYAxisPos2 - MachineConst.StampingMachineYPosition,
                                                        };

                                                        foreach (var num1 in settingBaseVM.PlateNumberList1)
                                                        {
                                                            if (string.IsNullOrWhiteSpace(num1.FontString))
                                                                num1.IsUsed = false;
                                                        }

                                                        foreach (var num2 in settingBaseVM.PlateNumberList2)
                                                        {
                                                            if (string.IsNullOrWhiteSpace(num2.FontString))
                                                                num2.IsUsed = false;
                                                        }


                                                        string productProjectName = string.Empty;
                                                        foreach (var projectDistribute in StampingMachineSingleton.Instance.TypeSettingSettingVM.ProjectDistributeVMObservableCollection)
                                                        {
                                                            //去盒子裡面找是否有對應的鐵片
                                                            var boxPartsCollection = projectDistribute.StampingBoxPartsVM.SeparateBoxVMObservableCollection.SelectMany(x => x.BoxPartsParameterVMCollection);
                                                            var foundPart = boxPartsCollection.FirstOrDefault(x => x.ID == plateData.iIronPlateID && x.IsSended);
                                                            if (foundPart != null)
                                                            {
                                                                foundPart.DataMatrixIsFinish = plateData.bDataMatrixFinish;
                                                                foundPart.EngravingIsFinish = plateData.bEngravingFinish;
                                                                productProjectName = foundPart.ProductProjectName;

                                                                try
                                                                {
                                                                    _ = Task.Run(async () =>
                                                                    {
                                                                        await projectDistribute.SaveProductProjectVMCollectionAsync();
                                                                    });
                                                                }
                                                                catch
                                                                {

                                                                }
                                                                break;
                                                            }
                                                        }

                                                        PlateMonitorViewModel PlateMonitorVM = new()
                                                        {
                                                            ID = plateData.iIronPlateID,
                                                            ProductProjectName = productProjectName,
                                                            SettingBaseVM = settingBaseVM,
                                                            StampingStatus = steelBeltStampingStatus,
                                                            DataMatrixIsFinish = plateData.bDataMatrixFinish,
                                                            EngravingIsFinish = plateData.bEngravingFinish,
                                                        };
                                                        plateMonitorVMCollection.Add(PlateMonitorVM);
                                                    }

                                                    var collection = plateMonitorVMCollection?.Take(MachineConst.PlateCount)?.ToList();
                                                    if (collection != null)
                                                    {
                                                        if (PlateBaseObservableCollection.Count != collection.Count)
                                                        {
                                                            await Application.Current.Dispatcher.InvokeAsync(async () =>
                                                            {
                                                                PlateBaseObservableCollection = new ObservableCollection<PlateMonitorViewModel>(collection);
                                                            });
                                                        }
                                                        else
                                                        {
                                                            for (int i = 0; i < collection.Count; i++)
                                                            {
                                                                int index = i;//防止閉包問題
                                                                if (PlateBaseObservableCollection.TryGetValue(index, out var machineSetting)
                                                                && collection.TryGetValue(index, out var plateMonitorVM))
                                                                {
                                                                    if (machineSetting.SettingBaseVM.PlateNumber != plateMonitorVM.SettingBaseVM.PlateNumber
                                                                    || machineSetting.StampingStatus != plateMonitorVM.StampingStatus
                                                                    || machineSetting.DataMatrixIsFinish != plateMonitorVM.DataMatrixIsFinish
                                                                    || machineSetting.EngravingIsFinish != plateMonitorVM.EngravingIsFinish
                                                                    || machineSetting.ShearingIsFinish != plateMonitorVM.ShearingIsFinish)
                                                                    {
                                                                        //  var invoke =
                                                                        await Application.Current.Dispatcher.InvokeAsync(async () =>
                                                                         {
                                                                             try
                                                                             {
                                                                                 if (PlateBaseObservableCollection != null)
                                                                                     PlateBaseObservableCollection[index] = plateMonitorVM;
                                                                             }
                                                                             catch
                                                                             {

                                                                             }
                                                                             await Task.Yield();
                                                                         });
                                                                        await Task.Delay(100);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                            await Task.Delay(3000, token);
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }, token);

                                var ioTask = Task.Run(async () =>
                                {
                                    try
                                    {
                                        List<(string nodeID, Action<object> action, int samplingInterval, bool checkDuplicates)> ioUpdateNodes = new();
                                        foreach (var IO_Table in IO_TableObservableCollection)
                                        {
                                            if (!string.IsNullOrEmpty(IO_Table.NodeID))
                                            {
                                                var (result, values) = await this.ReadNodeAsync<object>(IO_Table.NodeID);
                                                if (result)
                                                    IO_Table.IO_Value = values;

                                                //訂閱
                                                ioUpdateNodes.Add(new(IO_Table.NodeID, value =>
                                                {
                                                    IO_Table.IO_Value = value;
                                                }, 3000, false));
                                            }
                                        }

                                        if (token.IsCancellationRequested)
                                            token.ThrowIfCancellationRequested();
                                        var ioList = (await opcUaClient.SubscribeNodesDataChangeAsync(ioUpdateNodes)).ToList();
                                        await Task.Yield();
                                        await Task.Delay(60000);

                                    }
                                    catch (Exception ex)
                                    {
                                        await LogDataSingleton.Instance.AddLogDataAsync(DataSingletonName, ex.Message);
                                    }
                                }, token);

                                await Task.WhenAll(machineTask, ironPlateCollectionTask);
                            }
                            catch (OperationCanceledException ocex)
                            {
                                Debug.WriteLine("工作已取消");
                                _ = LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, ocex.Message);
                                break;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("工作已取消");
                                _ = LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, ex.Message, true);
                            }
                            finally
                            {
                                await this.DisconnectAsync();
                                IsConnected = false;
                            }
                        }
                        while (true);
                    }
                    catch (OperationCanceledException)
                    {

                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        IsConnected = false;
                        opcUaClient = null;
                        manager?.Close();
                        manager = null;
                        cancelTokenSoruce.Dispose();
                    }

                    IsScanning = false;
                }, token);
        }


        public async Task<bool> CompareFontsSettingBetweenMachineAndSoftwareAsync(Window? Parent ,
            ObservableCollection<StampingTypeViewModel> settingCollection)
        {
            return await CompareFontsSettingBetweenMachineAndSoftwareAsync(null, settingCollection, this.RotatingTurntableInfoCollection);
        }

        public async Task<bool> CompareFontsSettingBetweenMachineAndSoftwareAsync(Window? Parent ,
            ObservableCollection<StampingTypeViewModel> settingCollection, ObservableCollection<StampingTypeViewModel> rotatingCollection)
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
                            await MessageBoxResultShow.ShowOKAsync(Parent ,(string)Application.Current.TryFindResource("Text_notify"),
                                Outputstring , GD_MessageBoxNotifyResult.NotifyYe);
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

                            await MessageBoxResultShow.ShowOKAsync(null,(string)Application.Current.TryFindResource("Text_notify"),
                                Outputstring , GD_MessageBoxNotifyResult.NotifyYe);
                        }


                    }
                    else
                    {
                        FontsIsSame = true;
                    }
                }
                catch (Exception ex)
                {
                    await MessageBoxResultShow.ShowExceptionAsync(null, ex);
                }
            }
            catch (Exception ex)
            {
                await MessageBoxResultShow.ShowExceptionAsync(null, ex);
            }

            return FontsIsSame;
        }


        private ObservableCollection<IO_InfoViewModel>? _io_tableObservableCollection;
        /// <summary>
        /// IO表
        /// </summary>
        public ObservableCollection<IO_InfoViewModel> IO_TableObservableCollection
        {
            get => _io_tableObservableCollection ??= new ObservableCollection<IO_InfoViewModel>();
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
                if (opcUaClient?.IsConnected == true)
                {
                    await opcUaClient.FeedingPositionReturnToStandbyPosition();
                    //opcUaClient.Disconnect();
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
                        if (opcUaClient?.IsConnected == true)
                        {
                            if (ParameterValue > 0)
                            {
                                await FeedingPositionFwdAsync(true);
                                await Task.Delay(500);
                                await FeedingPositionFwdAsync(false);
                            }
                            else if (ParameterValue < 0)
                            {
                                await FeedingPositionBwdAsync(true);
                                await Task.Delay(500);
                                await FeedingPositionBwdAsync(false);
                            }
                            else
                            {
                                Debugger.Break();
                            }
                            /*if (opcUaClient.GetFeedingPosition(out var FPosition))
                            {

                                opcUaClient.SetFeedingPosition(FPosition + ParameterValue);



                                await Task.Delay(2000);

                                opcUaClient.GetFeedingPosition(out var FPosition2);
                                var value = FPosition2 - FPosition;



                            }*/
                        }
                    }
                }
            });
        }

        public AsyncRelayCommand Feeding_XAxis_MoveStopCommand
        {
            get => new (async () =>
            {
                await Task.Run(async () =>
                {
                    for (int tryCount = 0; tryCount < 10; tryCount++)
                    {
                        try
                        {
                            if (opcUaClient?.IsConnected == true)
                            {
                                var ret1 = await FeedingPositionFwdAsync(false);
                                var ret2 = await FeedingPositionBwdAsync(false);
                                if (ret1 && ret2)
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            await LogDataSingleton.Instance.AddLogDataAsync(DataSingletonName, ex.Message);
                        }
                    }
                });
            });
        }

        public AsyncRelayCommand<object> FeedingXAxisFwdCommand
        {
            get => new(async obj =>
            {
                await Task.Run(async () =>
                {
                    if (obj is bool isActivated)
                    {
                        await SetFeedingXAxisFwdAsync(isActivated);
                    }
                });
            }, obj => IsConnected);
        }

        public AsyncRelayCommand<object> FeedingXAxisBwdCommand
        {
            get => new(async obj =>
            {
                await Task.Run(async () =>
                {
                    if (obj is bool isActivated)
                    {
                        await SetFeedingXAxisBwdAsync(isActivated);
                    }
                });
            });
        }







        /// <summary>
        /// 雙導桿缸-可動端 壓座控制 放鬆
        /// </summary>
        public AsyncRelayCommand<object> GuideRod_Move_Up_Command
        {
            get => new (async para =>
            {
                await Task.Run(async () =>
                {
                    if (para is bool isTriggered)
                    {
                        if (opcUaClient?.IsConnected == true)
                            await Set_IO_CylinderControlAsync(StampingCylinderType.GuideRod_Move, DirectionsEnum.Up, isTriggered);
                    }
                });
            });
        }

        /// <summary>
        /// 雙導桿缸-可動端 壓座控制 夾緊
        /// </summary>
        public AsyncRelayCommand GuideRod_Move_Down_Command
        {
            get => new (async para =>
            {
                await Task.Run(async () =>
                {
                    if (opcUaClient?.IsConnected == true)
                    {
                        await Set_IO_CylinderControlAsync(StampingCylinderType.GuideRod_Move, DirectionsEnum.Down, true);
                        await Task.Delay(500);
                        await Set_IO_CylinderControlAsync(StampingCylinderType.GuideRod_Move, DirectionsEnum.Down, false);
                    }
                });
            });
        }


        /// <summary>
        /// 雙導桿缸-固定端 壓座控制 放鬆
        /// </summary>
        public AsyncRelayCommand GuideRod_Fixed_Up_Command
        {
            get => new(async para =>
            {
                await Task.Run(async () =>
                {
                    if (opcUaClient?.IsConnected == true)
                    {
                        await Set_IO_CylinderControlAsync(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Up, true);
                        await Task.Delay(500);
                        await Set_IO_CylinderControlAsync(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Up, false);

                    }
                });
            });
        }

        /// <summary>
        /// 雙導桿缸-固定端 壓座控制 夾緊
        /// </summary>
        public AsyncRelayCommand GuideRod_Fixed_Down_Command
        {
            get => new (async para =>
            {
                await Task.Run(async () =>
                {
                    if (opcUaClient?.IsConnected == true)
                    {
                        await Set_IO_CylinderControlAsync(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Down, true);
                        await Task.Delay(500);
                        await Set_IO_CylinderControlAsync(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Down, false);
                    }
                });
            });
        }


        /// <summary>
        /// QR壓座組
        /// </summary>

        public AsyncRelayCommand QRStamping_Up_Command
        {
            get => new (async para =>
            {
                await Task.Run(async () =>
                {
                    if (opcUaClient?.IsConnected == true)
                    {
                        await Set_IO_CylinderControlAsync(StampingCylinderType.QRStamping, DirectionsEnum.Up, true);
                        await Task.Delay(500, CancellationToken.None);
                        await Set_IO_CylinderControlAsync(StampingCylinderType.QRStamping, DirectionsEnum.Up, false);
                    }
                });
            }, () => !QRStamping_Up_Command.IsRunning);
        }




        private AsyncRelayCommand? _qRStamping_Down_Command;
        /// <summary>
        /// QR壓座組
        /// </summary>

        public AsyncRelayCommand QRStamping_Down_Command
        {
            get => _qRStamping_Down_Command ??= new (async token =>
            {
                await Task.Run(async () =>
                {
                    if (opcUaClient?.IsConnected == true)
                    {
                        try
                        {
                            await Set_IO_CylinderControlAsync(StampingCylinderType.QRStamping, DirectionsEnum.Down, true);
                            await Task.Delay(500, token);
                            await Set_IO_CylinderControlAsync(StampingCylinderType.QRStamping, DirectionsEnum.Down, false);
                        }
                        catch
                        {

                        }
                    }
                });
            }, () => !QRStamping_Down_Command.IsRunning);
        }



        private AsyncRelayCommand<object>? _stampingSeat_Up_Command;
        /// <summary>
        /// 轉盤壓座組上
        /// </summary>
        public AsyncRelayCommand<object> StampingSeat_Up_Command
        {
            get => _stampingSeat_Up_Command ??= new AsyncRelayCommand<object>(async obj =>
            {
                await Task.Run(async () =>
                {
                    if (obj is bool tirggered)
                    {
                        //  if (!tirggered)
                        //      await Task.Delay(200);
                        if (opcUaClient?.IsConnected == true)
                        {
                            await Set_IO_CylinderControlAsync(StampingCylinderType.StampingSeat, DirectionsEnum.Up, tirggered);
                        }
                    }
                });
            });
        }





        private AsyncRelayCommand<object>? _stampingSeat_Down_Command;
        /// <summary>
        /// 轉盤壓座組下
        /// </summary>
        public AsyncRelayCommand<object> StampingSeat_Down_Command
        {
            get => _stampingSeat_Down_Command ??= new AsyncRelayCommand<object>(async obj =>
            {
                await Task.Run(async () =>
                {
                    if (obj is bool tirggered)
                    {
                        //  if (!tirggered)
                        //      await Task.Delay(200);
                        if (opcUaClient?.IsConnected == true)
                        {
                            await Set_IO_CylinderControlAsync(StampingCylinderType.StampingSeat, DirectionsEnum.Down, tirggered);
                        }
                    }
                });
            });
        }






        //private AsyncRelayCommand? _blockingCylinder_Up_Command;
        /// <summary>
        /// 阻擋缸
        /// </summary>
        public AsyncRelayCommand BlockingCylinder_Up_Command
        {
            get => new (async token =>
            {
                await Task.Run(async () =>
                {
                    if (opcUaClient?.IsConnected == true)
                    {
                        await Set_IO_CylinderControlAsync(StampingCylinderType.BlockingCylinder, DirectionsEnum.Up, true);
                        await Task.Delay(500);
                        await Set_IO_CylinderControlAsync(StampingCylinderType.BlockingCylinder, DirectionsEnum.Up, false);
                    }
                });

            });
        }

        //private AsyncRelayCommand? _blockingCylinder_Down_Command;
        /// <summary>
        /// 阻擋缸
        /// </summary>
        public AsyncRelayCommand BlockingCylinder_Down_Command
        {
            get => new (async token =>
            {
                await Task.Run(async () =>
                {
                    if (opcUaClient?.IsConnected == true)
                    {
                        try
                        {
                            await Set_IO_CylinderControlAsync(StampingCylinderType.BlockingCylinder, DirectionsEnum.Down, true);
                            await Task.Delay(500, token);
                            await Set_IO_CylinderControlAsync(StampingCylinderType.BlockingCylinder, DirectionsEnum.Down, false);
                        }
                        catch
                        {

                        }
                    }
                });
            });
        }

        //private AsyncRelayCommand<object>? _hydraulicEngraving_Up_Command;
        /// <summary>
        /// Z軸油壓缸
        /// </summary>

        public AsyncRelayCommand<object> HydraulicEngraving_Up_Command
        {
            get =>new AsyncRelayCommand<object>(async obj =>
            {
                await Task.Run(async () =>
                {
                    if (obj is bool tirggered)
                    {
                        // if (!tirggered)
                        //     await Task.Delay(200);
                        if (opcUaClient?.IsConnected == true)
                        {
                            await Set_IO_CylinderControlAsync(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Up, tirggered);
                        }
                    }
                });
            });
        }


        //private AsyncRelayCommand<object>? _hydraulicEngraving_Down_Command;
        /// <summary>
        /// Z軸油壓缸
        /// </summary>
        public AsyncRelayCommand<object> HydraulicEngraving_Down_Command
        {
            get =>new AsyncRelayCommand<object>(async obj =>
            {
                await Task.Run(async () =>
                {
                    if (obj is bool tirggered)
                    {
                        if (opcUaClient?.IsConnected == true)
                        {
                            await Set_IO_CylinderControlAsync(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Down, tirggered);
                        }
                    }
                });
            });
        }







        //private AsyncRelayCommand<object>? _hydraulicCutting_Up_Command;
        /// <summary>
        /// 裁切
        /// </summary>
        public AsyncRelayCommand<object> HydraulicCutting_Up_Command
        {
            get => new AsyncRelayCommand<object>(async para =>
            {
                await Task.Run(async () =>
                {
                    if (para is bool isTriggered)
                    {
                        // if (!isTriggered)
                        //      await Task.Delay(200);
                        if (opcUaClient?.IsConnected == true)
                            await Set_IO_CylinderControlAsync(StampingCylinderType.HydraulicCutting, DirectionsEnum.Up, isTriggered);
                    }
                });
            });
        }


       // private AsyncRelayCommand<object>? _hydraulicCutting_Down_Command;
        /// <summary>
        /// 裁切
        /// </summary>
        public AsyncRelayCommand<object> HydraulicCutting_Down_Command
        {
            get => new AsyncRelayCommand<object>(async para =>
            {
                await Task.Run(async () =>
                {
                    if (para is bool isTriggered)
                    {
                        // if (!isTriggered)
                        //     await Task.Delay(200);
                        if (opcUaClient?.IsConnected == true)
                            await Set_IO_CylinderControlAsync(StampingCylinderType.HydraulicCutting, DirectionsEnum.Down, isTriggered);
                    }
                });
            });
        }





























        private AsyncRelayCommand? _activeHydraulicPumpMotorCommand;
        /// <summary>
        /// 啟用/禁用液壓馬達
        /// </summary>
        public AsyncRelayCommand ActiveHydraulicPumpMotorCommand
        {
            get => _activeHydraulicPumpMotorCommand ??= new (async () =>
            {
                await Task.Run(async () =>
                {
                    if (opcUaClient?.IsConnected == true)
                    {
                        var isActivated = await GetHydraulicPumpMotorAsync();
                        if (isActivated.Item1)
                        {
                            await SetHydraulicPumpMotorAsync(!isActivated.Item2);
                        }
                    }
                });
            }, () => !ActiveHydraulicPumpMotorCommand.IsRunning);
        }







        public async Task<bool> SetHMIIronPlateDataAsync(IronPlateDataModel ironPlateData)
        {
            if (opcUaClient?.IsConnected == true)
            {
                await SetDataMatrixModeAsync(true);
                return await SetHMIIronPlateAsync(ironPlateData);
            }
            else
                return false;
        }

        /// <summary>
        /// 設定下一片加工資料
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, IronPlateDataModel)> GetHMIIronPlateDataAsync()
        {
            if (opcUaClient?.IsConnected == true)
                return await GetHMIIronPlateAsync();
            else
                return (false, new IronPlateDataModel());
        }















        private AsyncRelayCommand? _returnToOriginCommand;
        /// <summary>
        /// 回歸原點
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        public AsyncRelayCommand ReturnToOriginCommand
        {
            get => _returnToOriginCommand ??= new(async () =>
            {
                await Task.Run(async() =>
                 {
                     IsReturningToOrigin = true;
                     try
                     {
                         CancellationTokenSource cts = new();
                         var token = cts.Token;
                         if (opcUaClient?.IsConnected == true)
                         {
                             var MotorIsActivated = await GetHydraulicPumpMotorAsync();
                             if (MotorIsActivated.Item1)
                             {
                                 if (!MotorIsActivated.Item2)
                                 {
                                     //油壓馬達尚未啟動
                                     var Result = MessageBoxResultShow.ShowOKAsync(null, (string)Application.Current.TryFindResource("Text_notify"),
                                        (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorIsNotActivated"), GD_MessageBoxNotifyResult.NotifyRd);

                                     _ = LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorIsNotActivated"));

                                     throw new Exception();
                                 }
                                 else
                                 {
                                     var opMode = await GetOperationModeAsync();
                                     if (opMode.Item1)
                                     {
                                         //OperationMode = opMode.Item2;
                                         //要在工程模式
                                         if (opMode.Item2 != OperationModeEnum.Setup)
                                         {
                                             //需在工程模式才可執行
                                             var Result = MessageBoxResultShow.ShowOKAsync(null, (string)Application.Current.TryFindResource("Text_notify"),
                                                (string)Application.Current.TryFindResource("Text_MachineNotInSetupMode"), GD_MessageBoxNotifyResult.NotifyYe);

                                             await LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, (string)Application.Current.TryFindResource("Text_MachineNotInSetupMode"));
                                         }
                                         else
                                         {
                                             if (opcUaClient?.IsConnected == true)
                                             {

                                                 bool EngravingToOriginSuccessful;
                                                 if ((await GetHydraulicEngraving_Position_OriginAsync()).Item2)
                                                 {
                                                     EngravingToOriginSuccessful = true;
                                                 }
                                                 else
                                                 {
                                                     await Set_IO_CylinderControlAsync(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Up, true);
                                                     var EngravingToOriginTask = Task.Run(async () =>
                                                    {
                                                     try
                                                     {
                                                         var EngravingIsOrigin = false;
                                                         do
                                                         {
                                                             if (token.IsCancellationRequested)
                                                                 token.ThrowIfCancellationRequested();
                                                             EngravingIsOrigin = (await GetHydraulicEngraving_Position_OriginAsync()).Item2;
                                                             await Task.Delay(10);
                                                         }
                                                         while (!EngravingIsOrigin);
                                                     }
                                                     catch (OperationCanceledException Cex)
                                                     {
                                                         await Singletons.LogDataSingleton.Instance.AddLogDataAsync(Cex.Source, Cex.Message);
                                                     }
                                                     catch (Exception ex)
                                                     {
                                                         await Singletons.LogDataSingleton.Instance.AddLogDataAsync(ex.Source, ex.Message);
                                                     }
                                                 });

                                                     Task completedTask = await Task.WhenAny(EngravingToOriginTask, Task.Delay(10000));
                                                     await Set_IO_CylinderControlAsync(StampingCylinderType.HydraulicEngraving, DirectionsEnum.Up, false);
                                                     EngravingToOriginSuccessful = completedTask == EngravingToOriginTask;
                                                 }


                                                 if (!EngravingToOriginSuccessful)
                                                 {
                                                     var Result = MessageBoxResultShow.ShowOKAsync(null, (string)Application.Current.TryFindResource("Text_notify"),
                                                        (string)Application.Current.TryFindResource("Text_EngravingToOriginTimeout"), GD_MessageBoxNotifyResult.NotifyRd);
                                                     return;
                                                 }

                                                 bool CuttungToOriginSuccessful;

                                                 //先檢查是否在原點
                                                 if ((await GetHydraulicCutting_Position_OriginAsync()).Item2)
                                                 {
                                                     CuttungToOriginSuccessful = true;
                                                 }
                                                 else
                                                 {

                                                     await Set_IO_CylinderControlAsync(StampingCylinderType.HydraulicCutting, DirectionsEnum.Up, true);
                                                     var CuttingToOriginTask = Task.Run(async () =>
                                                    {
                                                     try
                                                     {
                                                         var CuttingIsOrigin = false;
                                                         do
                                                         {
                                                             if (token.IsCancellationRequested)
                                                                 token.ThrowIfCancellationRequested();

                                                             CuttingIsOrigin = (await GetHydraulicCutting_Position_OriginAsync()).Item2;
                                                             await Task.Delay(10);
                                                         }
                                                         while (!CuttingIsOrigin);
                                                     }
                                                     catch (OperationCanceledException Cex)
                                                     {
                                                         await Singletons.LogDataSingleton.Instance.AddLogDataAsync(Cex.Source!, Cex.Message);
                                                     }
                                                     catch (Exception ex)
                                                     {
                                                         await Singletons.LogDataSingleton.Instance.AddLogDataAsync(ex.Source!, ex.Message);
                                                     }

                                                 });
                                                     Task completedTask = await Task.WhenAny(CuttingToOriginTask, Task.Delay(10000));
                                                     await Set_IO_CylinderControlAsync(StampingCylinderType.HydraulicCutting, DirectionsEnum.Up, false);
                                                     CuttungToOriginSuccessful = completedTask == CuttingToOriginTask;
                                                 }

                                                 if (!CuttungToOriginSuccessful)
                                                 {
                                                     var Result = MessageBoxResultShow.ShowOKAsync(null, (string)Application.Current.TryFindResource("Text_notify"),
                                                        (string)Application.Current.TryFindResource("Text_CuttingToOriginTimeout"), GD_MessageBoxNotifyResult.NotifyRd);
                                                     //超時
                                                     return;
                                                 }

                                                 //X軸回歸
                                                 var ret = await FeedingPositionReturnToStandbyPositionAsync();

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
                                                                 isHome = (await GetServoHomeAsync()).Item2;
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
                                                             var getHome = await GetServoHomeAsync();
                                                             if (getHome.Item1)
                                                             {
                                                                 if (!getHome.Item2)
                                                                 {
                                                                     var feedingPosition = await GetFeedingPositionAsync();
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
                                                                 await MessageBoxResultShow.ShowOKAsync(null, (string)Application.Current.TryFindResource("Text_notify"),
                                                                     (string)Application.Current.TryFindResource("Text_ToOriginisSuccessful"), GD_MessageBoxNotifyResult.NotifyGr);
                                                             }
                                                             else
                                                             {
                                                                 await MessageBoxResultShow.ShowOKAsync(null, (string)Application.Current.TryFindResource("Text_notify"),
                                                                     (string)Application.Current.TryFindResource("Text_ToOriginisFail"), GD_MessageBoxNotifyResult.NotifyRd);
                                                             }
                                                         }
                                                         else
                                                         {
                                                             await MessageBoxResultShow.ShowOKAsync(null, (string)Application.Current.TryFindResource("Text_notify"),
                                                                (string)Application.Current.TryFindResource("Text_ToOriginisTimeout"), GD_MessageBoxNotifyResult.NotifyRd);
                                                         }
                                                     }
                                                     catch (OperationCanceledException)
                                                     {
                                                         await MessageBoxResultShow.ShowOKAsync(null, (string)Application.Current.TryFindResource("Text_notify"),
                                                            (string)Application.Current.TryFindResource("Text_ToOriginisCancel"), GD_MessageBoxNotifyResult.NotifyYe);
                                                     }
                                                     catch (Exception ex)
                                                     {
                                                         await MessageBoxResultShow.ShowExceptionAsync(null, ex);
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
                             await MessageBoxResultShow.ShowOKAsync(null, (string)Application.Current.TryFindResource("Text_notify"),
                                (string)Application.Current.TryFindResource("Connection_Error"), GD_MessageBoxNotifyResult.NotifyRd);
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
                 });
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


        //private List<string> _alarmMessageList = new List<string>();
        private ObservableCollection<string> _alarmMessageCollection;
        public ObservableCollection<string> AlarmMessageCollection
        {
            get=>_alarmMessageCollection??= new ObservableCollection<string>();
            set
            {
                _alarmMessageCollection = value;
                OnPropertyChanged();
            }
        }

        // private bool _hasAlarm;
        /// <summary>
        /// 出現alarm
        /// </summary>
        /*public bool HasAlarm
        {
            get => _hasAlarm;
            set
            {
                _hasAlarm = value;
                OnPropertyChanged();
            }
        }*/



        public async Task<bool> SetSeparateBoxNumberAsync(int Index)
        {
            var ret = false;
            if (opcUaClient?.IsConnected == true)
            {
                return await this.WriteNodeAsync(StampingOpcUANode.Stacking1.sv_iThisStation, Index);
                //opcUaClient.Disconnect();
            }
            return ret;
        }

        public async Task<(bool, int)> GetSeparateBoxNumberAsync()
        {
            var ret = (false, -1);
            if (opcUaClient?.IsConnected == true)
            {
                return await this.ReadNodeAsync<int>(StampingOpcUANode.Stacking1.sv_iThisStation);
                //opcUaClient.Disconnect();
            }
            return ret;
        }

        private AsyncRelayCommand<object>? _engravingYAxisToStandbyPosCommand;
        public AsyncRelayCommand<object> EngravingYAxisToStandbyPosCommand
        {
            get => _engravingYAxisToStandbyPosCommand ??= new AsyncRelayCommand<object>(async obj =>
            {
                if (obj is bool IsActivated)
                    await SetEngravingYAxisToStandbyPosAsync();
            }, obj => IsConnected);
        }

        private AsyncRelayCommand<object>? _engravingYAxisFwdCommand;
        public AsyncRelayCommand<object> EngravingYAxisFwdCommand
        {
            get => _engravingYAxisFwdCommand ??= new(async obj =>
            {
                if (obj is bool isActivated)
                {

                    await SetEngravingYAxisFwdAsync(isActivated);
                }
            }, obj => IsConnected);
        }

        private AsyncRelayCommand<object>? _engravingYAxisBwdCommand;
        public AsyncRelayCommand<object> EngravingYAxisBwdCommand
        {
            get => _engravingYAxisBwdCommand ??= new(async obj =>
            {
                if (obj is bool isActivated)
                {
                    await SetEngravingYAxisBwdAsync(isActivated);
                }
            });
        }









        private AsyncRelayCommand<object>? _engravingRotateClockwiseCommand;
        public AsyncRelayCommand<object> EngravingRotateClockwiseCommand
        {
            get => _engravingRotateClockwiseCommand ??= new AsyncRelayCommand<object>(async obj =>
            {
                if (obj is bool isActivated)
                {
                    if (!isActivated && (await GetOperationModeAsync()).Item2 == OperationModeEnum.Manual)
                        await Task.Delay(200);
                    await SetEngravingRotateCWAsync(isActivated);
                }
            });
        }








        private AsyncRelayCommand<object>? _engravingRotateCounterClockwiseCommand;
        public AsyncRelayCommand<object> EngravingRotateCounterClockwiseCommand
        {
            get => _engravingRotateCounterClockwiseCommand ??= new AsyncRelayCommand<object>(async obj =>
            {
                if (obj is bool isActivated)
                {
                    if (!isActivated && (await GetOperationModeAsync()).Item2 == OperationModeEnum.Manual)
                        await Task.Delay(200);
                    await SetEngravingRotateCCWAsync(isActivated);
                }
            });
        }


        private async Task<bool> CheckHydraulicPumpMotorAsync()
        {
            var MotorIsActivated = await GetHydraulicPumpMotorAsync();
            if (MotorIsActivated.Item1)
            {
                if (MotorIsActivated.Item2)
                    return true;
                else
                {
                    //詢問後設定
                    //油壓馬達尚未啟動，是否要啟動油壓馬達？
                    var Result = MessageBoxResultShow.ShowYesNoAsync(null , (string)Application.Current.TryFindResource("Text_notify"),
                        (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorIsNotActivated") +
                        "\r\n" +
                        (string)Application.Current.TryFindResource("Text_AskActiveHydraulicPumpMotor") , GD_MessageBoxNotifyResult.NotifyYe);


                    if (await Result == MessageBoxResult.Yes)
                    {
                        if (await SetHydraulicPumpMotorAsync(true))
                        {
                            return true;
                        }
                        else
                        {
                            await MessageBoxResultShow.ShowOKAsync(null,(string)Application.Current.TryFindResource("Text_notify"),
                                   (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorActivatedFailure") , GD_MessageBoxNotifyResult.NotifyRd);
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
        private async Task<bool> SetFeedingXAxisFwdAsync(bool IsMove)
        {
            var ret = false;
            if (opcUaClient?.IsConnected == true)
            {
                ret = await FeedingPositionBwdAsync(false);
                ret = await FeedingPositionFwdAsync(IsMove);
            }
            return ret;
        }
        /// <summary>
        /// 鋼印X軸後退
        /// </summary>
        /// <param name="IsMove"></param>
        /// <returns></returns>
        private async Task<bool> SetFeedingXAxisBwdAsync(bool IsMove)
        {
            var ret = false;
            if (opcUaClient?.IsConnected == true)
            {
                ret = await FeedingPositionFwdAsync(false);
                ret = await FeedingPositionBwdAsync(IsMove);
            }
            return ret;
        }









        /// <summary>
        /// 鋼印旋轉 順時針
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SetEngravingRotateClockwiseAsync()
        {
            var ret = false;
            if (opcUaClient?.IsConnected == true)
            {
                ret = await SetEngravingRotateCWAsync(true);
                await Task.Delay(500);
                ret = await SetEngravingRotateCWAsync(false);
                //opcUaClient.Disconnect();
            }
            return ret;
        }

        /// <summary>
        /// 鋼印旋轉 逆時針
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SetEngravingRotateCounterClockwiseAsync()
        {
            var ret = false;
            if (opcUaClient?.IsConnected == true)
            {
                ret = await SetEngravingRotateCCWAsync(true);
                await Task.Delay(500);
                ret = await SetEngravingRotateCCWAsync(false);
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


        private string? _dataMatrixTCPIP;
        public string? DataMatrixTCPIP
        {
            get => _dataMatrixTCPIP = string.Empty;
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



        private bool _di_PowerON, _di_Pause, _di_Start, _di_EmergencyStop1;
        public bool DI_PowerON
        {
            get => _di_PowerON;
            set
            {
                _di_PowerON = value;
                OnPropertyChanged();
            }
        }
        public bool DI_Pause
        {
            get => _di_Pause;
            set
            {
                _di_Pause = value;
                OnPropertyChanged();
            }
        }
        public bool DI_Start
        {
            get => _di_Start;
            set
            {
                _di_Start = value;
                OnPropertyChanged();
            }
        }
        public bool DI_EmergencyStop1
        {
            get => _di_EmergencyStop1;
            set
            {
                _di_EmergencyStop1 = value;
                OnPropertyChanged();
            }
        }



        private bool _runningLamp, _stoppedLamp, _alarmLamp;
        public bool RunningLamp
        {
            get => _runningLamp;
            set
            {
                _runningLamp = value;OnPropertyChanged();
            }
        }
        public bool StoppedLamp
        {
            get => _stoppedLamp;
            set
            {
                _stoppedLamp = value; OnPropertyChanged();
            }
        }
        public bool AlarmLamp
        {
            get => _alarmLamp;
            set
            {
                _alarmLamp = value; OnPropertyChanged();
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
            get => _shearingVelocity;
            set
            {
                _shearingVelocity = value;
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


        private ICommand?_globalSpeedChangedCommand;
        /// <summary>
        /// 全域速度設定
        /// </summary>
        public ICommand GlobalSpeedChangedCommand
        {
            get => _globalSpeedChangedCommand ??= new AsyncRelayCommand<RoutedPropertyChangedEventArgs<double>>(async e =>
            {
                if (opcUaClient != null)
                {
                    if (opcUaClient?.IsConnected == true)
                    {
                        bool ret = false;
                        ret = await SetFeedingSetupVelocityAsync((float)e.NewValue);
                        ret = await SetFeedingVelocityAsync((float)e.NewValue);

                        ret = await SetEngravingFeedingSetupVelocityAsync((float)e.NewValue);
                        ret = await SetEngravingFeedingVelocityAsync((float)e.NewValue);

                        //ret = await SetEngravingRotateSetupVelocityAsync((float)e.NewValue);
                        //ret = await SetEngravingRotateVelocityAsync((float)e.NewValue);
                    }
                    else
                    {
                        this.FeedingVelocity = 0;
                    }
                }


            });
        }


        private ICommand?_feedingVelocityChangedCommand;
        public ICommand FeedingVelocityChangedCommand
        {
            get => _feedingVelocityChangedCommand ??= new AsyncRelayCommand<RoutedPropertyChangedEventArgs<double>>(async e =>
            {
                if (opcUaClient?.IsConnected == true)
                {
                    bool ret = false;
                    ret = await SetFeedingSetupVelocityAsync((float)e.NewValue);
                    ret = await SetFeedingVelocityAsync((float)e.NewValue);
                }
                else
                {
                    FeedingVelocity = 0;
                }
            });
        }

        private ICommand?_engravingFeedingVelocityChangedCommand;
        public ICommand EngravingFeedingVelocityChangedCommand
        {
            get => _engravingFeedingVelocityChangedCommand ??= new AsyncRelayCommand<RoutedPropertyChangedEventArgs<double>>(async e =>
            {
                if (opcUaClient?.IsConnected == true)
                {
                    bool ret = false;
                    ret = await SetEngravingFeedingSetupVelocityAsync((float)e.NewValue);
                    ret = await SetEngravingFeedingVelocityAsync((float)e.NewValue);
                }
                else
                {
                    EngravingFeeding = 0;
                }
            });
        }




        private ICommand?_rotateVelocityChangedCommand;
        public ICommand RotateVelocityChangedCommand
        {
            get => _rotateVelocityChangedCommand ??= new AsyncRelayCommand<RoutedPropertyChangedEventArgs<double>>(async e =>
            {
                if (opcUaClient?.IsConnected == true)
                {
                    bool ret = false;
                    ret = await SetEngravingRotateSetupVelocityAsync((float)e.NewValue);
                    ret = await SetEngravingRotateVelocityAsync((float)e.NewValue);
                }
                else
                {
                    RotateVelocity = 0;
                }
            });
        }







        private AsyncRelayCommand? _resetCommand;
        public AsyncRelayCommand ResetCommand
        {
            get => _resetCommand ??= new (async () =>
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        if (opcUaClient?.IsConnected == true)
                        {
                            var ret = await ResetAsync();
                        }
                    }
                    catch
                    {

                    }
                });
            }, () => !ResetCommand.IsRunning);
        }

        private AsyncRelayCommand? _cycleStartCommand;
        public AsyncRelayCommand CycleStartCommand
        {
            get => _cycleStartCommand ??= new (async () =>
            {
                await Task.Run(async() =>
                {
                    try
                    {
                        if (opcUaClient?.IsConnected == true)
                        {
                            var ret = await CycleStartAsync();
                        }
                    }
                    catch
                    {

                    }
                });
            }, () => !CycleStartCommand.IsRunning);
        }



        private ObservableCollection<StampingTypeViewModel>? _rotatingTurntableInfo;
        /// <summary>
        /// 字模轉盤上的字(實際)
        /// </summary>
        public ObservableCollection<StampingTypeViewModel> RotatingTurntableInfoCollection
        {
            get => _rotatingTurntableInfo ??= new ObservableCollection<StampingTypeViewModel>(); set { _rotatingTurntableInfo = value; OnPropertyChanged(); }
        }

        ObservableCollection<PlateMonitorViewModel> _plateBaseObservableCollection;
        /// <summary>
        /// 實際加工狀態[25]
        /// </summary>
        public ObservableCollection<PlateMonitorViewModel> PlateBaseObservableCollection
        {
            get => _plateBaseObservableCollection ??= new();
            set
            {
                _plateBaseObservableCollection = value;OnPropertyChanged();
            }
        }









        private IronPlateDataModel? _hMIIronPlateDataModel;
        public IronPlateDataModel HMIIronPlateDataModel
        {
            get => _hMIIronPlateDataModel ??= new IronPlateDataModel();
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
            get => _cylinder_HydraulicEngraving_IsStopDown;
            set
            {
                Cylinder_HydraulicEngraving_IsStopDownChanged?.Invoke(this, new GD_CommonLibrary.ValueChangedEventArgs<bool>(_cylinder_HydraulicEngraving_IsStopDown, value));
                _cylinder_HydraulicEngraving_IsStopDown = value;
                OnPropertyChanged();
            }
        }
        public event EventHandler<GD_CommonLibrary.ValueChangedEventArgs<bool>>? Cylinder_HydraulicEngraving_IsStopDownChanged;




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
            get => _cylinder_HydraulicCutting_IsCutPoint; 
            set
            {

                Cylinder_HydraulicCutting_IsCutPointChanged?.Invoke(this, new GD_CommonLibrary.ValueChangedEventArgs<bool>(_cylinder_HydraulicCutting_IsCutPoint, value));
                _cylinder_HydraulicCutting_IsCutPoint = value;
                OnPropertyChanged(); 
            }
        }

        public event EventHandler<GD_CommonLibrary.ValueChangedEventArgs<bool>>? Cylinder_HydraulicCutting_IsCutPointChanged;






        private (int oldValue, int newValue) _lastIronPlateID = (0, 0);
        /// <summary>
        /// 最後一片的id
        /// </summary>
        /*public (int oldValue, int newValue) LastIronPlateID
        {
            get => _lastIronPlateID;
            private set
            {
                _lastIronPlateID = value;
                OnPropertyChanged();
                LastIronPlateIDChanged?.Invoke(this, new GD_CommonLibrary.ValueChangedEventArgs<int>(value.oldValue, value.newValue));
            }
        }
        public event EventHandler<GD_CommonLibrary.ValueChangedEventArgs<int>>? LastIronPlateIDChanged;*/

        /*private (int oldValue, int newValue) _firstIronPlateID;
        public (int oldValue, int newValue) FirstIronPlateID
        {
            get => _firstIronPlateID;
            set
            {
                _firstIronPlateID = value;
                OnPropertyChanged();

                FirstIronPlateIDChanged?.Invoke(this, new GD_CommonLibrary.ValueChangedEventArgs<int>(value.oldValue, value.newValue));
            }
        }
        public event EventHandler<GD_CommonLibrary.ValueChangedEventArgs<int>>? FirstIronPlateIDChanged;*/


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

                        _ = SeparateBox_RotateAsync(value, step);
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBoxResultShow.ShowExceptionAsync(null, ex);
                }

                _separateBoxIndex = value;
                OnPropertyChanged();
            }
        }


        private CancellationTokenSource cts = new();
        private Task? RotateTask;

        readonly ParameterSettingViewModel ParameterSettingVM = Singletons.StampingMachineSingleton.Instance.ParameterSettingVM;

        private async Task SeparateBox_RotateAsync(int isUsingIndex, int step)
        {
            if (isUsingIndex != -1)
            {
                Parallel.ForEach(ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection, obj =>
                {
                    obj.IsUsing = false;
                });

                ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection[isUsingIndex].IsUsing = true;
                //取得

                    cts?.Cancel();

                if (RotateTask != null)
                    await RotateTask;

                RotateTask = Task.Run(async () =>
                {
                    //角度比例
                    var DegreeRate = 360 / ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection.Count;
                    //目標

                    //先取得目前的位置
                    var tempRotate = SeparateBox_RotateAngle;
                    //檢查正反轉
                    var endRotatePoint = 360 - DegreeRate * isUsingIndex;

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







        // private int oldvalue = 0;
        /// <summary>
        /// 取得第一片id
        /// </summary>
        /// <returns></returns>
       /* public async Task<(bool, int)> GetFirstIronPlateIDAsync()
        {
            if (opcUaClient?.IsConnected == true)
            {
                return await this.ReadNodeAsync<int>($"{StampingOpcUANode.system.sv_IronPlateData}[1].iIronPlateID");
            }
            return (false, 0);
        }*/

        // private int oldvalue = 0;
        /// <summary>
        /// 取得正在打印鋼印的金屬片id
        /// </summary>
        /// <returns></returns>
        /*public async Task<(bool, int)> GetEngravingIronPlateIDAsync()
        {
            if (opcUaClient?.IsConnected == true)
            {
                return await this.ReadNodeAsync<int>($"{StampingOpcUANode.system.sv_IronPlateData}[11].iIronPlateID");
            }
            return (false, 0);
        }*/

        // private int oldvalue = 0;
        /// <summary>
        /// 取得正在打印qr的金屬片id
        /// </summary>
        /// <returns></returns>
        /*public async Task<(bool, int)> GetDataMatrixIronPlateIDAsync()
        {
            if (opcUaClient?.IsConnected == true)
            {
                return await this.ReadNodeAsync<int>($"{StampingOpcUANode.system.sv_IronPlateData}[24].iIronPlateID");
            }
            return (false, 0);
        }*/




        /// <summary>
        /// 取得最後一片id
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, int)> GetLastIronPlateIDAsync()
        {
            if (opcUaClient?.IsConnected == true)
            {
                return await this.ReadNodeAsync<int>($"{StampingOpcUANode.system.sv_IronPlateData}[24].iIronPlateID");

            }
            return (false, 0);
        }




        private float _engravingYAxisPosition = 0;
        private int _engravingRotateStation = 0;
        /// <summary>
        /// 鋼印字模Y軸位置
        /// </summary>
        public float EngravingYAxisPosition
        {
            get => _engravingYAxisPosition; set { _engravingYAxisPosition = value; OnPropertyChanged(); }
        }
        /*public float EngravingZAxisPosition
        {
            get => _engravingZAxisPosition; set { _engravingZAxisPosition = value; OnPropertyChanged(); }
        }*/

        /// <summary>
        /// 鋼印轉輪目前旋轉位置
        /// </summary>
        public int EngravingRotateStation
        {
            get => _engravingRotateStation; set { _engravingRotateStation = value; OnPropertyChanged(); }
        }




        private bool  _key_EditMode = true ,_key_MachiningMode = true , _lightOn = false;
        /// <summary>
        /// 照明開關
        /// </summary>
        public bool LightOn
        {
            get => _lightOn;
            set
            {
                _lightOn = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 鑰匙-編輯模式
        /// </summary>
        public bool Key_EditMode
        {
            get => _key_EditMode; 
            set { _key_EditMode = value; OnPropertyChanged(); }
        }    
        /// <summary>
       /// 鑰匙-加工模式
        /// </summary>
        public bool Key_MachiningMode
        {
            get => _key_MachiningMode;
            set { _key_MachiningMode = value; OnPropertyChanged(); }
        }





        private AsyncRelayCommand<object>? _setOperationModeCommand;
        public AsyncRelayCommand<object> SetOperationModeCommand
        {
            get => _setOperationModeCommand ??= new AsyncRelayCommand<object>(async para =>
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        if (para is OperationModeEnum operationMode)
                        {
                            await SetOperationModeAsync(operationMode);
                        }
                        if (para is int operationIntMode)
                        {
                            try
                            {
                                await SetOperationModeAsync((OperationModeEnum)operationIntMode);
                            }
                            catch (Exception ex)
                            {
                                _ = MessageBoxResultShow.ShowExceptionAsync(null, ex);
                            }
                        }
                    }
                    catch
                    {

                    }
                });
                /* var oper=  await opcUaClient.GetOperationMode();
                 if (oper.Item1)
                     OperationMode = oper.Item2;
                 else
                     OperationMode = OperationModeEnum.None;*/
                //按完之後立刻刷新按鈕狀態


            }
            , para => !SetOperationModeCommand.IsRunning);
        }



        private AsyncRelayCommand<object>? _engravingRotateCommand;
        /// <summary>
        /// 旋轉到指定位置
        /// </summary>
        public AsyncRelayCommand<object> EngravingRotateCommand
        {
            get => _engravingRotateCommand ??= new AsyncRelayCommand<object>(async (para, token) =>
            {
                await Task.Run(async() =>
                {
                    try
                    {
                        if (para is int paraInt)
                            if (paraInt >= 0)
                            {
                                if (opcUaClient?.IsConnected == true)
                                {
                                    var slots = (await GetEngravingTotalSlotsAsync()).Item2;
                                    //先決定要順轉還是逆轉
                                    if (slots != 0)
                                    {
                                    }
                                }
                            }
                    }
                    catch
                    {

                    }
                });
            }, para => !EngravingRotateCommand.IsRunning);
        }
        public (int, int) FirstIronPlateID { get; private set; }

















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
                await this.DisconnectAsync();
                disposedValue = true;
            }
        }






        //public Exception ConnectException { get => opcUaClient.ConnectException; }



        public void Disconnect()
        {
            if (this.IsConnected)
                _ = this.WriteNodeAsync<bool>($"{StampingOpcUANode.system.sv_bComputerBootUpComplete}", false);
            opcUaClient?.Disconnect();
        }

        public async Task DisconnectAsync()
        {
            try
            {
                if (this.IsConnected)
                {
                    await this.WriteNodeAsync<bool>($"{StampingOpcUANode.system.sv_bComputerBootUpComplete}", false);
                    await opcUaClient?.DisconnectAsync();
                    opcUaClient = null;
                }
            }
            catch
            {

            }

        }




        public async Task<bool> FeedingPositionBwdAsync(bool Active)
        {
            await this.WriteNodeAsync(StampingOpcUANode.Feeding1.sv_bButtonFwd, false);
            return await this.WriteNodeAsync(StampingOpcUANode.Feeding1.sv_bButtonBwd, Active);
        }

        public async Task<bool> FeedingPositionFwdAsync(bool Active)
        {
            await this.WriteNodeAsync(StampingOpcUANode.Feeding1.sv_bButtonBwd, false);
            return await this.WriteNodeAsync(StampingOpcUANode.Feeding1.sv_bButtonFwd, Active);
        }

        public async Task<bool> FeedingPositionReturnToStandbyPositionAsync()
        {
            await this.WriteNodeAsync(StampingOpcUANode.Feeding1.sv_bUseHomeing, true);
            await Task.Delay(500);
            return await this.WriteNodeAsync(StampingOpcUANode.Feeding1.sv_bUseHomeing, false);
        }









        /// <summary>
        /// 取得機台狀態
        /// </summary>
        public async Task<(bool, OperationModeEnum)> GetOperationModeAsync()
        {
            var ret = await this.ReadNodeAsync<int>(StampingOpcUANode.system.sv_OperationMode);
            return (ret.result, (OperationModeEnum)ret.values);
        }
        /// <summary>
        /// 設定機台狀態
        /// </summary>
        public async Task<bool> SetOperationModeAsync(OperationModeEnum operationMode)
        {
            bool ret = false;
            if (opcUaClient?.IsConnected == true)
            {
                switch (operationMode)
                {
                    case OperationModeEnum.Setup:
                        ret = await this.WriteNodeAsync(StampingOpcUANode.OperationMode1.sv_bButtonSetup, true);
                        break;
                    case OperationModeEnum.Manual:
                        ret = await this.WriteNodeAsync(StampingOpcUANode.OperationMode1.sv_bButtonManual, true);
                        break;
                    case OperationModeEnum.HalfAutomatic:
                        ret = await this.WriteNodeAsync(StampingOpcUANode.OperationMode1.sv_bButtonHalfAuto, true);
                        break;
                    case OperationModeEnum.FullAutomatic:
                        ret = await this.WriteNodeAsync(StampingOpcUANode.OperationMode1.sv_bButtonFullAuto, true);
                        break;
                    default:
                        break;
                }
                await Task.Delay(100);
                await this.WriteNodeAsync(StampingOpcUANode.OperationMode1.sv_bButtonManual, false);
                await this.WriteNodeAsync(StampingOpcUANode.OperationMode1.sv_bButtonSetup, false);
                await this.WriteNodeAsync(StampingOpcUANode.OperationMode1.sv_bButtonHalfAuto, false);
                await this.WriteNodeAsync(StampingOpcUANode.OperationMode1.sv_bButtonFullAuto, false);
            }
            return ret;
        }

        /// <summary>
        /// 取得X軸是否在原點
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetServoHomeAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.Feeding1.di_ServoHome);
        }



        public async Task<(bool, float)> GetFeedingPositionAsync()
        {
            return await this.ReadNodeAsync<float>(StampingOpcUANode.Feeding1.sv_rFeedingPosition);
        }




        public async Task<bool> ResetAsync()
        {
            await this.WriteNodeAsync(StampingOpcUANode.OperationMode1.sv_bButtonAlarmConfirm, true);
            await Task.Delay(500);
            return await this.WriteNodeAsync(StampingOpcUANode.OperationMode1.sv_bButtonAlarmConfirm, false);
        }

        public async Task<bool> CycleStartAsync()
        {
            await this.WriteNodeAsync(StampingOpcUANode.OperationMode1.sv_bButtonCycleStart, true);
            await Task.Delay(500);
            return await this.WriteNodeAsync(StampingOpcUANode.OperationMode1.sv_bButtonCycleStart, false);
        }



        public async Task<bool> Set_IO_CylinderControlAsync(StampingCylinderType stampingCylinder, DirectionsEnum direction, bool IsTrigger)
        {
            bool ret = false;
            if (opcUaClient?.IsConnected == true)
            {
                for (int i = 0; i < 5 && !ret; i++)
                {
                    switch (stampingCylinder)
                    {
                        case StampingCylinderType.GuideRod_Move:
                            switch (direction)
                            {
                                case DirectionsEnum.Up:
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.GuideRodsFixture1.sv_bButtonDown, false);
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.GuideRodsFixture1.sv_bButtonUp, IsTrigger);
                                    break;
                                case DirectionsEnum.Down:
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.GuideRodsFixture1.sv_bButtonUp, false);
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.GuideRodsFixture1.sv_bButtonDown, IsTrigger);
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
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.GuideRodsFixture2.sv_bButtonDown, false);
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.GuideRodsFixture2.sv_bButtonUp, IsTrigger);
                                    break;
                                case DirectionsEnum.Down:
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.GuideRodsFixture2.sv_bButtonUp, false);
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.GuideRodsFixture2.sv_bButtonDown, IsTrigger);
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
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.Fixture1.sv_bButtonDown, false);
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.Fixture1.sv_bButtonUp, IsTrigger);
                                    break;
                                case DirectionsEnum.Down:
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.Fixture1.sv_bButtonUp, false);
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.Fixture1.sv_bButtonDown, IsTrigger);
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
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.Fixture2.sv_bButtonDown, false);
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.Fixture2.sv_bButtonUp, IsTrigger);
                                    break;
                                case DirectionsEnum.Down:
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.Fixture2.sv_bButtonUp, false);
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.Fixture2.sv_bButtonDown, IsTrigger);
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
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.BlockingClips1.sv_bButtonUp, IsTrigger);
                                    break;
                                case DirectionsEnum.Down:
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.BlockingClips1.sv_bButtonDown, IsTrigger);
                                    break;
                                default:
                                    var O_ret = this.WriteNodeAsync(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsUp, false);
                                    var C_ret = this.WriteNodeAsync(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsDown, false);
                                    ret = await O_ret && await C_ret;
                                    break;
                            }
                            break;
                        case StampingCylinderType.HydraulicEngraving:

                            switch (direction)
                            {
                                case DirectionsEnum.Up:
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.Engraving1.sv_bButtonOpen, IsTrigger);
                                    break;
                                case DirectionsEnum.Down:
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.Engraving1.sv_bButtonClose, IsTrigger);
                                    break;
                                default:
                                    var O_ret = this.WriteNodeAsync(StampingOpcUANode.Engraving1.sv_bButtonOpen, false);
                                    var C_ret = this.WriteNodeAsync(StampingOpcUANode.Engraving1.sv_bButtonClose, false);
                                    ret = await O_ret && await C_ret;
                                    break;
                            }

                            break;
                        case StampingCylinderType.HydraulicCutting:

                            switch (direction)
                            {
                                case DirectionsEnum.Up:
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.Cutting1.sv_bButtonOpen, IsTrigger);
                                    break;
                                case DirectionsEnum.Down:
                                    ret = await this.WriteNodeAsync(StampingOpcUANode.Cutting1.sv_bButtonClose, IsTrigger);
                                    break;
                                default:
                                    var O_ret = this.WriteNodeAsync(StampingOpcUANode.Cutting1.sv_bButtonOpen, false);
                                    var C_ret = this.WriteNodeAsync(StampingOpcUANode.Cutting1.sv_bButtonClose, false);
                                    ret = await O_ret && await C_ret;
                                    break;
                            }

                            break;
                        default: throw new NotImplementedException();
                    }
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
        public async Task<(bool, bool)> GetGuideRod_Move_Position_isUpAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.GuideRodsFixture1.sv_bGuideRodsFixtureUp);
        }
        /// <summary>
        ///  雙導桿缸(可動端)在下方
        /// </summary>
        /// <param name="stampingCylinder"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public async Task<(bool, bool)> GetGuideRod_Move_Position_isDownAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.GuideRodsFixture1.sv_bGuideRodsFixtureDown);
        }
        /// <summary>
        /// 雙導桿缸(固定端)在上方
        /// </summary>
        /// <param name="stampingCylinder"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public async Task<(bool, bool)> GetGuideRod_Fixed_Position_isUpAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.GuideRodsFixture2.sv_bGuideRodsFixtureUp);
        }
        /// <summary>
        /// 雙導桿缸(固定端)在下方
        /// </summary>
        /// <param name="stampingCylinder"></param>
        /// <param name="direction"></param>
        /// <returns></returns> 
        public async Task<(bool, bool)> GetGuideRod_Fixed_Position_isDownAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.GuideRodsFixture2.sv_bGuideRodsFixtureDown);
        }

        /// <summary>
        /// QR壓座組在上方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetQRStamping_Position_isUpAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.Fixture1.sv_bFixtureUp);
        }

        /// <summary>
        /// QR壓座組在下方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetQRStamping_Position_isDownAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.Fixture1.sv_bFixtureDown);
        }
        /// <summary>
        /// 鋼印壓座組在上方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetStampingSeat_Position_isUpAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.Fixture2.sv_bFixtureUp);

        }
        /// <summary>
        /// 鋼印壓座組在下方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetStampingSeat_Position_isDownAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.Fixture2.sv_bFixtureDown);
        }

        /// <summary>
        /// 阻擋缸在上方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetBlockingCylinder_Position_isUpAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsUp);
        }
        /// <summary>
        /// 阻擋缸在下方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetBlockingCylinder_Position_isDownAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsDown);
        }

        /// <summary>
        /// 原點位置
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicEngraving_Position_OriginAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.Engraving1.di_StopUp);
        }

        /// <summary>
        /// 鋼印待命
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicEngraving_Position_StandbyPointAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.Engraving1.di_StandbyPoint);
        }
        /// <summary>
        /// 鋼印下壓
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicEngraving_Position_StopDownAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.Engraving1.di_StopDown);
        }








        /// <summary>
        /// 切割原點
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicCutting_Position_OriginAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.Cutting1.di_CuttingOrigin);
        }
        /// <summary>
        /// 切割待命位置
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicCutting_Position_StandbyPointAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.Cutting1.di_CuttingStandbyPoint);
        }
        /// <summary>
        /// 切割位置
        /// </summary>
        /// <returns></returns>
        public async Task<(bool ret, bool point)> GetHydraulicCutting_Position_CutPointAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.Cutting1.di_CuttingCutPoint);
        }

        /// <summary>
        /// ALARM信號
        /// </summary>
        /// <returns></returns>
        public async Task<(bool ret, bool lamp)> GetAlarmLampAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.OperationMode1.do_AlarmLamp);
        }


        /// <summary>
        /// 訂閱切割位置
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SubscribeHydraulicCutting_Position_CutPointAsync(Action<bool> action)
        {
            return await opcUaClient.SubscribeNodeDataChangeAsync(StampingOpcUANode.Cutting1.di_CuttingCutPoint, action, 50, true);
        }

        public async Task<bool> SetHydraulicPumpMotorAsync(bool Active)
        {
            var pumptask = await GetHydraulicPumpMotorAsync();
            if (pumptask.Item1)
            {
                if (pumptask.Item2 == Active)
                    return true;
                await this.WriteNodeAsync(StampingOpcUANode.Motor1.sv_bButtonMotor, true);
                await Task.Delay(1000);
                return await this.WriteNodeAsync(StampingOpcUANode.Motor1.sv_bButtonMotor, false);
            }
            else
                return false;
        }

        /// <summary>
        /// 油壓單元
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicPumpMotorAsync()
        {
            return await this.ReadNodeAsync<bool>(StampingOpcUANode.Motor1.sv_bMotorStarted);
        }






        /// <summary>
        /// 取得鐵片下一片資訊-交握訊號
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        public async Task<(bool, bool)> GetRequestDatabitAsync()
        {
            return await this.ReadNodeAsync<bool>($"{StampingOpcUANode.system.sv_bRequestDatabit}");
        }





        /// <summary>
        /// 取消訂閱第一片ID
        /// </summary>
        /// <param name="action"></param>
      /*  public async Task<bool> UnsubscribeFirstIronPlate(int samplingInterval)
        {
            return opcUaClient.UnsubscribeNodeAsync($"{StampingOpcUANode.system.sv_IronPlateData}[1].iIronPlateID", samplingInterval);
        }*/




        /// <summary>
        /// 設定鐵片下一片資訊-交握訊號
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        public async Task<bool> SetRequestDatabitAsync(bool databit)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.system.sv_bRequestDatabit}", databit);
        }

        /// <summary>
        /// 取得預計要打的鋼片資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        /*public async Task<(bool,string)> GetHMIIronPlateName(StampingOpcUANode.sIronPlate ironPlateType)
        {
            return await this.ReadNodeAsync<string>($"{StampingOpcUANode.system.sv_HMIIronPlateName.NodeName}.{ironPlateType}");
        }*/

        /// <summary>
        /// 設定預計要打的鋼片資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
     /*   public async Task<bool> SetHMIIronPlateName(StampingOpcUANode.sIronPlate ironPlateType, string StringLine)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.system.sv_HMIIronPlateName.NodeName}.{ironPlateType}", StringLine);
        }*/


        /// <summary>
        /// 取得下一個要加工的鋼印
        /// </summary>

        public async Task<(bool, IronPlateDataModel)> GetHMIIronPlateAsync()
        {
            var rootNode = StampingOpcUANode.system.sv_HMIIronPlateName.NodeName;
            return await this.GetIronPlateAsync(rootNode);
        }
        /// <summary>
        /// 設定下一個鋼印
        /// </summary>
        public async Task<bool> SetHMIIronPlateAsync(IronPlateDataModel ironPlateData)
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
            var ret = await opcUaClient.WriteNodesAsync(wNodeTrees);
            //設定完後須更新hmi
            return !ret.Contains(false);
        }

        [Obsolete]
        public async Task<bool> SetDataMatrixModeAsync(bool IsUse)
        {
            bool ret = false;
            //ns=4;s=APPL.system.sv_DataMatrixMode
            var rootNode = StampingOpcUANode.system.sv_DataMatrixMode;
            ret = await this.WriteNodeAsync(rootNode, Convert.ToInt32(IsUse));
            //設定完後須更新hmi
            return ret;
        }

        /// <summary>
        /// 取得鐵片群資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        public async Task<(bool result, List<IronPlateDataModel> ironPlateCollection)> GetIronPlateDataCollectionAsync()
        {
            List<IronPlateDataModel> ironPlateDataList = new();
            if (opcUaClient?.IsConnected == true)
            {
                try
                {
                    //剪切
                    //ironPlateDataList.Add(new IronPlateDataModel());
                    for (int i = 1; i <= 24; i++)
                    {
                        var node = $"{StampingOpcUANode.system.sv_IronPlateData}[{i}]";
                        if ((await GetIronPlateAsync(node)).Item1)
                        {
                            ironPlateDataList.Add((await GetIronPlateAsync(node)).Item2);
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



        private async Task<(bool, IronPlateDataModel)> GetIronPlateAsync(string rootNode)
        {
            try
            {
                var newIronPlateData = new IronPlateDataModel();
                List<(Action<object>, string)> NodeActions = new()
            {
                (x => newIronPlateData.iIronPlateID = (int)x, rootNode + "." + "iIronPlateID"),
                (x => newIronPlateData.rXAxisPos1 = (float)x, rootNode + "." + "rXAxisPos1"),
                (x => newIronPlateData.rYAxisPos1 = (float)x, rootNode + "." + "rYAxisPos1"),
                (x => newIronPlateData.rXAxisPos2 = (float)x, rootNode + "." + "rXAxisPos2"),
                (x => newIronPlateData.rYAxisPos2 = (float)x, rootNode + "." + "rYAxisPos2"),
                (x => newIronPlateData.sIronPlateName1 = (string)x, rootNode + "." + "sIronPlateName1"),
                (x => newIronPlateData.sIronPlateName2 = (string)x, rootNode + "." + "sIronPlateName2"),
                (x => newIronPlateData.iStackingID = (int)x, rootNode + "." + "iStackingID"),
                (x => newIronPlateData.bEngravingFinish = (bool)x, rootNode + "." + "bEngravingFinish"),
                (x => newIronPlateData.bDataMatrixFinish = (bool)x, rootNode + "." + "bDataMatrixFinish"),
                (x => newIronPlateData.sDataMatrixName1 = (string)x, rootNode + "." + "sDataMatrixName1"),
                (x => newIronPlateData.sDataMatrixName2 = (string)x, rootNode + "." + "sDataMatrixName2")
            };
                var ret = await this.ReadNodesAsync(NodeActions);
                return (ret, newIronPlateData);
            }
            catch
            {
                return (false, new IronPlateDataModel());
            }


            /*
            var getTask_iIronPlateID = await this.ReadNodeAsync<int>(rootNode + "." + "iIronPlateID");
            var getTask_rXAxisPos1 = await this.ReadNodeAsync<float>(rootNode + "." + "rXAxisPos1");
            var getTask_rYAxisPos1 = await this.ReadNodeAsync<float>(rootNode + "." + "rYAxisPos1");
            var getTask_rXAxisPos2 = await this.ReadNodeAsync<float>(rootNode + "." + "rXAxisPos2");
            var getTask_rYAxisPos2 = await this.ReadNodeAsync<float>(rootNode + "." + "rYAxisPos2");
            var getTask_sIronPlateName1 = await this.ReadNodeAsync<string>(rootNode + "." + "sIronPlateName1");
            var getTask_sIronPlateName2 = await this.ReadNodeAsync<string>(rootNode + "." + "sIronPlateName2");
            var getTask_iStackingID = await this.ReadNodeAsync<int>(rootNode + "." + "iStackingID");
            var getTask_bEngravingFinish = await this.ReadNodeAsync<bool>(rootNode + "." + "bEngravingFinish");
            var getTask_bDataMatrixFinish = await this.ReadNodeAsync<bool>(rootNode + "." + "bDataMatrixFinish");
            var getTask_sDataMatrixName1 = await this.ReadNodeAsync<string>(rootNode + "." + "sDataMatrixName1");
            var getTask_sDataMatrixName2 = await this.ReadNodeAsync<string>(rootNode + "." + "sDataMatrixName2");

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
                // 分料盒
                iStackingID = (getTask_iStackingID).Item2,
                // 刻碼完成 
                bEngravingFinish = (getTask_bEngravingFinish).Item2,
                bDataMatrixFinish = (getTask_bDataMatrixFinish).Item2,
                sDataMatrixName1 = (getTask_sDataMatrixName1).Item2,
                sDataMatrixName2 = (getTask_sDataMatrixName2).Item2,

            };
            */
        }




        /// <summary>
        /// 設定鐵片群資訊(加工陣列)
        /// </summary>
        /// <param name="ironPlateDataList"></param>
        /// <returns></returns>
        public async Task<bool> SetIronPlateDataCollectionAsync(List<IronPlateDataModel> ironPlateDataList)
        {

            int ExistedDataCollectionCount = 1000;
            List<IronPlateDataModel> write_ironPlateDataList;
            //取得舊有的鐵片群資訊
            var getIronPlateDataCollectionTuple = await GetIronPlateDataCollectionAsync();
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
                    var WriteBooleanList = await opcUaClient.WriteNodesAsync(wNodeTreeNodes);
                    WriteBoolean = !WriteBooleanList.Contains(false);
                    if (WriteBoolean)
                        break;
                }
                WritedList.Add((node, WriteBoolean));
            }
            return WritedList.Exists(x => x.Item2 == true);
            // return true;
            //  return await opcUaClient.WriteNodesAsync(wNodeTrees);
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

















        public async Task<(bool, float)> GetEngravingYAxisPositionAsync()
        {
            return await this.ReadNodeAsync<float>($"{StampingOpcUANode.EngravingFeeding1.sv_rEngravingFeedingPosition}");
        }

        public async Task<(bool, float)> GetEngravingZAxisPositionAsync()
        {
            return await this.ReadNodeAsync<float>($"{StampingOpcUANode.Engraving1.sv_rEngravingPosition}");
        }


        /* public async Task<bool> GetEngravingZAxisHydraulicUp(out bool IsActivated)
         {
        return await this.ReadNodeAsync($"{StampingOpcUANode.Engraving1.sv_bButtonOpen}", out IsActivated);
         }

         public async Task<bool> GetEngravingZAxisHydraulicDown(out bool IsActivated)
         {
        return await this.ReadNodeAsync($"{StampingOpcUANode.Engraving1.sv_bButtonClose}", out IsActivated);
         }
         public async Task<bool> SetEngravingZAxisHydraulicUp(bool Activated)
         {
             return await this.WriteNodeAsync($"{StampingOpcUANode.Engraving1.sv_bButtonOpen}", Activated);
         }

         public async Task<bool> SetEngravingZAxisHydraulicDown(bool Activated)
         {
             return await this.WriteNodeAsync($"{StampingOpcUANode.Engraving1.sv_bButtonClose}", Activated);
         }*/





















        public async Task<bool> SetEngravingYAxisToStandbyPosAsync()
        {
            await SetEngravingYAxisBwdAsync(false);
            await SetEngravingYAxisFwdAsync(false);
            return await this.WriteNodeAsync($"{StampingOpcUANode.EngravingFeeding1.sv_rServoStandbyPos}", true);
        }


        public async Task<(bool, bool)> GetEngravingYAxisBwdAsync()
        {
            return await this.ReadNodeAsync<bool>($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonBwd}");
        }
        public async Task<(bool, bool)> GetEngravingYAxisFwdAsync()
        {
            return await this.ReadNodeAsync<bool>($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonFwd}");
        }

        public async Task<bool> SetEngravingYAxisBwdAsync(bool Active)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonBwd}", Active);
        }
        public async Task<bool> SetEngravingYAxisFwdAsync(bool Active)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonFwd}", Active);
        }




        public async Task<(bool, int)> GetEngravingRotateStationAsync()
        {
            int Station = -1;
            var ret = await this.ReadNodeAsync<int>($"{StampingOpcUANode.EngravingRotate1.sv_iThisStation}");

            if (ret.result)
            {
                //※須注意 目前使用的函式回傳index時是1為起始 所以需-1才能符合其他位置
                var StationIndex = ret.values;
                Station = StationIndex - 1;
            }

            return (ret.result, Station);
        }







        /// <summary>
        /// 總站數
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, int)> GetEngravingTotalSlotsAsync()
        {
            return await this.ReadNodeAsync<int>($"{StampingOpcUANode.EngravingRotate1.sv_iTotalSlots}");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, StampingTypeModel)> GetEngravingRotateStationCharAsync()
        {
            StampingTypeModel stampingType = new();
            var ret1 = await GetEngravingRotateStationAsync();
            var ret2 = await GetRotatingTurntableInfoAsync();

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
          //  await this.WriteNodeAsync($"{StampingOpcUANode.system.sv_iTargetAStation}", Station + 1);

            //return await this.WriteNodeAsync($"{StampingOpcUANode.system.sv_iTargetAStation}", Station + 1);
        }*/

        private async Task<(bool, int[])> GetRotatingTurntableInfoINTAsync()
            => await this.ReadNodeAsync<int[]>($"{StampingOpcUANode.system.sv_RotateCodeDefinition}");

        private async Task<bool> SetRotatingTurntableInfoINTAsync(int[] fonts)
        {
            var results = await Task.WhenAll(fonts.Select((font, i) =>
            this.WriteNodeAsync($"{StampingOpcUANode.system.sv_RotateCodeDefinition}[{i + 1}]", font)));
            return results.All(result => result == true);
        }










        /// <summary>
        /// 取得字模
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, List<StampingTypeModel>)> GetRotatingTurntableInfoAsync()
        {
            var ret = await GetRotatingTurntableInfoINTAsync();
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

        /// <summary>
        /// 設定字模
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        public async Task<bool> SetRotatingTurntableInfoAsync(IEnumerable<string> fonts)
        {
            List<char> charList = new();
            foreach (var font in fonts)
            {
                var charArray = font.ToCharArray();
                if (charArray.Length > 0)
                    charList.Add(charArray[0]);
                else
                    charList.Add(new char());
            }

            return await SetRotatingTurntableInfoINTAsync(charList.ToIntList().ToArray());
        }


        public async Task<bool> SetRotatingTurntableInfoAsync(int index, char font)
        {
            var ret = await GetRotatingTurntableInfoINTAsync();
            if (ret.Item1)
            {
                var codeInfoArray = ret.Item2;
                //確認沒超出範圍
                if (codeInfoArray.ToList().Count > index)
                {
                    codeInfoArray[index] = font.ToInt();
                    return await SetRotatingTurntableInfoINTAsync(codeInfoArray);
                }
            }

            return false;
        }




        public async Task<bool> SetEngravingRotateCWAsync(bool isActivated)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.EngravingRotate1.sv_bButtonCW}", isActivated);
        }

        public async Task<bool> SetEngravingRotateCCWAsync(bool isActivated)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.EngravingRotate1.sv_bButtonCCW}", isActivated);
        }

        public async Task<(bool, float)> GetFeedingSetupVelocityAsync()
        {
            return await this.ReadNodeAsync<float>($"{StampingOpcUANode.Feeding1.sv_rFeedSetupVelocity}");
        }
        public async Task<bool> SetFeedingSetupVelocityAsync(float percent)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.Feeding1.sv_rFeedSetupVelocity}", percent);
        }
        public async Task<(bool, float)> GetFeedingVelocityAsync()
        {
            return await this.ReadNodeAsync<float>($"{StampingOpcUANode.Feeding1.sv_rFeedVelocity}");
        }




        public async Task<bool> SetFeedingVelocityAsync(float percent)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.Feeding1.sv_rFeedVelocity}", percent);
        }





        /// <summary>
        /// 字模移動速度
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, float)> GetEngravingFeedingSetupVelocityAsync()
        {
            return await this.ReadNodeAsync<float>($"{StampingOpcUANode.EngravingFeeding1.sv_rFeedSetupVelocity}");
        }

        /// <summary>
        /// 設定字模移動速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<bool> SetEngravingFeedingSetupVelocityAsync(float SpeedPercent)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.EngravingFeeding1.sv_rFeedSetupVelocity}", SpeedPercent);

        }

        /// <summary>
        /// 字模移動速度
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, float)> GetEngravingFeedingVelocityAsync()
        {
            return await this.ReadNodeAsync<float>($"{StampingOpcUANode.EngravingFeeding1.sv_rFeedVelocity}");
        }

        /// <summary>
        /// 設定字模移動速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<bool> SetEngravingFeedingVelocityAsync(float SpeedPercent)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.EngravingFeeding1.sv_rFeedVelocity}", SpeedPercent);

        }




        /// <summary>
        /// 字模旋轉速度
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, float)> GetEngravingRotateSetupVelocityAsync()
        {
            return await this.ReadNodeAsync<float>($"{StampingOpcUANode.EngravingRotate1.sv_rRotateSetupVelocity}");
        }

        /// <summary>
        /// 設定字模旋轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<bool> SetEngravingRotateSetupVelocityAsync(float SpeedPercent)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.EngravingRotate1.sv_rRotateSetupVelocity}", SpeedPercent);

        }

        /// <summary>
        /// 字模旋轉速度
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, float)> GetEngravingRotateVelocityAsync()
        {
            return await this.ReadNodeAsync<float>($"{StampingOpcUANode.EngravingRotate1.sv_rRotateVelocity}");
        }

        /// <summary>
        /// 設定字模旋轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<bool> SetEngravingRotateVelocityAsync(float SpeedPercent)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.EngravingRotate1.sv_rRotateVelocity}", SpeedPercent);
        }


        /// <summary>
        /// 刻印壓力
        /// </summary>
        /// <returns></returns>

        public async Task<(bool, float)> GetStampingPressureAsync()
        {
            return await this.ReadNodeAsync<UInt32>($"{StampingOpcUANode.Pump1.sv_PressureLintab.LintabPoints.uNoOfPoints}");
        }
        /// <summary>
        /// 刻印速度
        /// </summary>
        /// <returns></returns>

        public async Task<(bool, float)> GetStampingVelocityAsync()
        {
            return await this.ReadNodeAsync<UInt32>($"{StampingOpcUANode.Pump1.sv_VelocityLintab.LintabPoints.uNoOfPoints}");
        }

        /// <summary>
        /// 裁斷壓力
        /// </summary>
        /// <returns></returns>

        public async Task<(bool, float)> GetShearingPressureAsync()
        {
            return await this.ReadNodeAsync<UInt32>($"{StampingOpcUANode.Pump2.sv_PressureLintab.LintabPoints.uNoOfPoints}");
        }
        /// <summary>
        /// 裁斷速度
        /// </summary>
        /// <returns></returns>

        public async Task<(bool, float)> GetShearingVelocityAsync()
        {
            return await this.ReadNodeAsync<UInt32>($"{StampingOpcUANode.Pump2.sv_VelocityLintab.LintabPoints.uNoOfPoints}");
        }








        /// <summary>
        /// QR機IP
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string)> GetDataMatrixTCPIPAsync()
        {
            return await this.ReadNodeAsync<string>(StampingOpcUANode.DataMatrix1.sv_sContactTCPIP);
        }

        /// <summary>
        /// QR機IP
        /// </summary>
        /// <returns></returns>
       /* public async Task SubscribeDataMatrixTCPIP(Action<string> action)
        {
            return opcUaClient.SubscribeNodeDataChange<string>(StampingOpcUANode.DataMatrix1.sv_sContactTCPIP, action);
        }*/


        /// <summary>
        /// QR機Port
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string)> GetDataMatrixPortAsync()
        {
            return await this.ReadNodeAsync<string>($"{StampingOpcUANode.DataMatrix1.sv_sContactTCPPort}");
        }

        /// <summary>
        /// QR機Port
        /// </summary>
        /// <returns></returns>

        /*public async Task SubscribeDataMatrixPort(Action<string> action)
        {
            return opcUaClient.SubscribeNodeDataChange<string>(StampingOpcUANode.DataMatrix1.sv_sContactTCPPort,action);
        }*/






        public async Task<(bool, float)> GetFeedingXHomeFwdVelocityAsync()
        {
            return await this.ReadNodeAsync<float>($"{StampingOpcUANode.Feeding1.sv_rHomeFwdVelocity}");
        }


        public async Task<(bool, float)> GetFeedingXHomeBwdVelocityAsync()
        {
            return await this.ReadNodeAsync<float>($"{StampingOpcUANode.Feeding1.sv_rHomeBwdVelocity}");
        }


        public async Task<bool> SetFeedingXHomeFwdVelocityAsync(float SpeedPercent)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.Feeding1.sv_rHomeFwdVelocity}", SpeedPercent);
        }


        public async Task<bool> SetFeedingXHomeBwdVelocityAsync(float SpeedPercent)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.Feeding1.sv_rHomeBwdVelocity}", SpeedPercent);
        }

        /// <summary>
        /// 潤滑設定時間
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<(bool, object)> GetLubricationSettingTimeAsync()
        {
            return await this.ReadNodeAsync<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationSetValues.dLubTime}");
        }

        /// <summary>
        /// 潤滑開設定時間
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, object)> GetLubricationSettingOnTimeAsync()
        {
            return await this.ReadNodeAsync<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationSetValues.dOnTime}");
        }
        /// <summary>
        /// 潤滑關設定時間
        /// </summary>
        /// <returns></returns>     
        public async Task<(bool, object)> GetLubricationSettingOffTimeAsync()
        {
            return await this.ReadNodeAsync<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationSetValues.dOffTime}");
        }


        /// <summary>
        /// 潤滑實際時間
        /// </summary>
        public async Task<(bool, object)> GetLubricationActualTimeAsync()
        {
            return await this.ReadNodeAsync<int>($"{StampingOpcUANode.Lubrication1.sv_LubricationActValues.dLubTime}");
        }
        /// <summary>
        /// 潤滑開實際時間
        /// </summary>
        public async Task<(bool, object)> GetLubricationActualOnTimeAsync()
        {
            return await this.ReadNodeAsync<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationActValues.dOnTime}");
        }

        /// <summary>
        /// 潤滑關實際時間
        /// </summary>
        public async Task<(bool, object)> GetLubricationActualOffTimeAsync()
        {
            return await this.ReadNodeAsync<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationActValues.dOffTime}");
        }



        /// <summary>
        /// 取得錯誤訊息
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string)> GetAlarmMessageAsync()
        {
            return await this.ReadNodeAsync<string>($"{StampingOpcUANode.OpcMonitor1.sv_OpcLastAlarmText}");
        }


        /// <summary>
        /// 關閉蜂鳴器
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public async Task<bool> SetBZPassAsync()
        {
            await this.WriteNodeAsync($"{StampingOpcUANode.OperationMode1.sv_bBZPass}", true);
            await Task.Delay(1000);
            return await this.WriteNodeAsync($"{StampingOpcUANode.OperationMode1.sv_bBZPass}", false);
        }



        /// <summary>
        ///  取得蜂鳴器時間
        /// </summary>
        /// <returns></returns>
        public async Task<(bool,float)> GetBuzzerTimeAsync()
        {
            return await this.ReadNodeAsync<float>($"{StampingOpcUANode.OperationMode1.sv_dBuzzerTime}");
        }

        /// <summary>
        /// 設定蜂鳴器時間
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public async Task<bool> SetBuzzerTimeAsync(float buzzTime)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.OperationMode1.sv_dBuzzerTime}", buzzTime);
        }

        /// <summary>
        /// 取得蜂鳴器on/off時間
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public async Task<(bool, float)> GetBuzzerPluseTimeAsync()
        {
            return await this.ReadNodeAsync<float>($"{StampingOpcUANode.OperationMode1.sv_dBuzzerPluseTime}");
        }
        /// <summary>
        /// 設定蜂鳴器on/off時間
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public async Task<bool> SetBuzzerPluseTimeAsync(float buzzTime)
        {
            return await this.WriteNodeAsync($"{StampingOpcUANode.OperationMode1.sv_dBuzzerPluseTime}", buzzTime);
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
            if (opcUaClient != null)
            {
                return await opcUaClient.SubscribeNodeDataChangeAsync<T>(NodeID, action, 200, checkDuplicates);
            }
            return false;
        }


        public async Task<IEnumerable<bool>> SubscribeNodesDataChangeAsync<T>(IList<(string NodeID, Action<T> updateAction, int samplingInterval, bool checkDuplicates)> nodeList)
        {
            if (opcUaClient != null)
            {
                return await opcUaClient.SubscribeNodesDataChangeAsync<T>(nodeList);
            }
            return new List<bool>();
        }

        public async Task<bool> ReadNodesAsync(IEnumerable<(Action<object>, string)> NodeTrees)
        {
            return await Task.Run(async () =>
            {
                if (opcUaClient != null)
                {
                    //T NodeValue = default(T);
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            if (this.IsConnected)
                            {
                                List<object> NodeValue = (await opcUaClient.ReadNodesAsync(NodeTrees.Select(x => x.Item2))).ToList();
                                var actionList = NodeTrees.Select(x => x.Item1).ToList();
                                for (int j = 0; j < NodeValue.Count; j++)
                                {
                                    actionList[j]?.Invoke(NodeValue[j]);
                                }

                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            await LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, ex.Message);
                            //Disconnect();
                        }
                    }
                }
                return false;
            });
        }

        public async Task<(bool, IEnumerable<object>)> ReadNodesAsync(IEnumerable<string> nodeIds)
        {
            return await Task.Run(async () =>
            {
                if (opcUaClient != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            if (this.IsConnected)
                            {
                                IEnumerable<object> NodeValue = await opcUaClient.ReadNodesAsync(nodeIds);

                                return (true, NodeValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            await LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, ex.Message);
                            //Disconnect();
                        }
                    }
                }
                return (false, new List<object>());
            });
        }



        public async Task<(bool result, T? values)> ReadNodeAsync<T>(string NodeTree)
        {
            return await Task.Run(async () =>
            {
                if (opcUaClient != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            if (this.IsConnected)
                            {
                                T? NodeValue = await opcUaClient.ReadNodeAsync<T>(NodeTree);
                                return (true, NodeValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            await LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, ex.Message);
                        }
                    }
                }
                return (false, default(T));
            });
        }

        public async Task<IEnumerable<bool>> WriteNodesAsync<T>(Dictionary<string, object> NodeTrees)
        {
            return await Task.Run(async () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        if (opcUaClient?.IsConnected == true)
                        {
                            IEnumerable<bool> NodeValue;
                            NodeValue = await opcUaClient.WriteNodesAsync(NodeTrees);
                            return NodeValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        await LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, ex.Message);
                    }
                }
                return Enumerable.Repeat(false, NodeTrees.Count());
            });
        }




        public async Task<bool> WriteNodeAsync<T>(string NodeTreeString, T WriteValue)
        {
            return await Task.Run(async() =>
            {
                 for (int i = 0; i < 5; i++)
                 {
                     try
                     {
                         if (opcUaClient?.IsConnected == true)
                         {
                             bool NodeValue = await opcUaClient.WriteNodeAsync(NodeTreeString, WriteValue);
                             return NodeValue;
                         }
                         else
                         {
                             return false;
                         }
                     }
                     catch (Exception ex)
                     {
                         await LogDataSingleton.Instance.AddLogDataAsync(this.DataSingletonName, ex.Message);
                     }
                 }
                 return false;
             });
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
                Lubrication1,
                OpcMonitor1

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
                public static string di_EmergencyStop1 => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_EmergencyStop1";

                /// <summary>
                /// 運轉燈號
                /// </summary>
                public static string do_RunningLamp => $"{NodeHeader}.{NodeVariable.OperationMode1}.do_RunningLamp";
                /// <summary>
                /// 停止燈號
                /// </summary>
                public static string do_StoppedLamp => $"{NodeHeader}.{NodeVariable.OperationMode1}.do_StoppedLamp";
                /// <summary>
                /// 警告燈號
                /// </summary>
                public static string do_AlarmLamp => $"{NodeHeader}.{NodeVariable.OperationMode1}.do_AlarmLamp";

                /// <summary>
                /// 關閉蜂鳴器
                /// </summary>
                public static string sv_bBZPass => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bBZPass";
                /// <summary>
                /// 警報時間
                /// </summary>
                public static string sv_dBuzzerTime => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_dBuzzerTime";
                /// <summary>
                ///警報on/off時間
                /// </summary>
                public static string sv_dBuzzerPluseTime => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_dBuzzerPluseTime";




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
                /// 系統開機完成
                /// </summary>
                public static string sv_bComputerBootUpComplete => $"{NodeHeader}.{NodeVariable.system}.sv_bComputerBootUpComplete";

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
                [Obsolete]
                public static string sv_DataMatrixMode => $"{NodeHeader}.{NodeVariable.system}.sv_DataMatrixMode";

                /// <summary>
                /// 開機
                /// </summary>
                public static string di_PowerON => $"{NodeHeader}.{NodeVariable.system}.di_PowerON";
                /// <summary>
                /// 暫停
                /// </summary>
                public static string di_Pause => $"{NodeHeader}.{NodeVariable.system}.di_Pause";

                /// <summary>
                /// 開始加工
                /// </summary>
                public static string di_Start => $"{NodeHeader}.{NodeVariable.system}.di_Start";


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

            /// <summary>
            /// 資料
            /// </summary>
            public class OpcMonitor1
            {
                public static string sv_OpcLastAlarmText => $"{NodeHeader}.{NodeVariable.OpcMonitor1}.sv_OpcLastAlarmText";
                public static string sv_OpcAlarmCount => $"{NodeHeader}.{NodeVariable.OpcMonitor1}.sv_OpcAlarmCount";
            }






        }

        #endregion














    }
}
