using DevExpress.CodeParser;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Scheduling.Themes;
using GD_CommonLibrary;
using GD_CommonLibrary.Method;
using GD_MachineConnect;
using GD_MachineConnect.Machine;
using GD_MachineConnect.Machine.Interfaces;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels;
using Opc.Ua;
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
using System.Windows.Input;
using static GD_MachineConnect.GD_Stamping_Opcua.StampingOpcUANode;
using static GD_StampingMachine.Method.StampingMachineJsonHelper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GD_StampingMachine.Singletons
{
    public class StampMachineDataSingleton : GD_CommonLibrary.BaseSingleton<StampMachineDataSingleton>, INotifyPropertyChanged
    {
        /*private string OpcUASettingFilePath
        {
            get => Method.StampingMachineJsonHelper.GetJsonFilePath(Method.StampingMachineJsonHelper.MachineSettingNameEnum.OpcUASetting);
        }*/
        public const string DataSingletonName = "Name_StampMachineDataSingleton";

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        protected void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        protected override void Init()
        {
            var JsonHM = new StampingMachineJsonHelper();

            if(JsonHM.ReadMachineSettingJson(StampingMachineJsonHelper.MachineSettingNameEnum.OpcUASetting, out OpcUASettingModel OpcUASettingJson))
            {
                OpcUASetting = OpcUASettingJson;
            }
            else
            {
                //如果沒找到 產出一個新檔案後寫入
                OpcUASetting = new OpcUASettingModel()
                {
                    MachineName="GuandaStamping",
                    HostString = @"opc.tcp://192.168.1.123",
                    Port = 4842,
                    ServerDataPath= null,
                    UserName= "Administrator",
                    Password = "pass"               
                };
                JsonHM.WriteMachineSettingJson(StampingMachineJsonHelper.MachineSettingNameEnum.OpcUASetting, OpcUASetting);
            }
        }

        private OpcUASettingModel _opcUASetting;
        public OpcUASettingModel OpcUASetting
        {
            get=>_opcUASetting ??= new OpcUASettingModel();            
            set=> _opcUASetting = value;
        }



        private bool _opcuaTestIsOpen= false;
        public bool OpcuaTestIsOpen { get => _opcuaTestIsOpen; private set { _opcuaTestIsOpen = value; OnPropertyChanged(); } }
     
        OpcuaTest _opcuaTest = new OpcuaTest();
        internal async Task TestConnect()
        {
            await Task.Run(() =>
            {
                if (OpcuaTestIsOpen == false)
                {
                    try
                    {
                        OpcuaTestIsOpen = true;
                         _opcuaTest.OpenFormBrowseServer(null);
                    }
                    catch(Exception ex)
                    {

                    }
                    finally
                    {
                        OpcuaTestIsOpen = false;
                    }
                }

            });
        }


        private bool ContinueScanning = true;
        private bool _isScaning = false;
        public bool IsScaning { get => _isScaning; set { _isScaning = value; OnPropertyChanged(); } }
      
        
        private bool _isConnected = false;
        public bool IsConnected { get => _isConnected; private set { _isConnected = value; OnPropertyChanged(); } }

        readonly IStampingMachineConnect GD_Stamping = new GD_Stamping_Opcua();

        public void ScanOpcua()
        {
            StampingMachineJsonHelper JsonHM = new StampingMachineJsonHelper();
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

            if (!IsScaning)
            {
                IsScaning = true;
                Task.Run(async () =>
                {
                    do
                    {
                        try
                        {
                            IsConnected = GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password);
                            if(IsConnected)
                            {
                                //進料馬達

                                //GD_Stamping.GetMachineStatus
                                if (GD_Stamping.GetFeedingPosition(out var fPos))
                                    FeedingPosition = fPos;

                                //磁簧開關
                                if (GD_Stamping.GetCylinderActualPosition(StampingCylinderType.GuideRod_Move, DirectionsEnum.Up, out bool Move_IsUp))
                                    Cylinder_GuideRod_Move_IsUp = Move_IsUp;
                                if (GD_Stamping.GetCylinderActualPosition(StampingCylinderType.GuideRod_Move, DirectionsEnum.Down, out bool Move_IsDown))
                                    Cylinder_GuideRod_Move_IsDown = Move_IsDown;

                                if (GD_Stamping.GetCylinderActualPosition(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Up, out bool Fixed_IsUp))
                                    Cylinder_GuideRod_Fixed_IsUp = Fixed_IsUp;
                                if (GD_Stamping.GetCylinderActualPosition(StampingCylinderType.GuideRod_Fixed, DirectionsEnum.Down, out bool Fixed_IsDown))
                                    Cylinder_GuideRod_Fixed_IsDown = Fixed_IsDown;

                                if (GD_Stamping.GetCylinderActualPosition(StampingCylinderType.QRStamping, DirectionsEnum.Up, out bool QRStamping_IsUp))
                                    Cylinder_QRStamping_IsUp = QRStamping_IsUp;
                                if (GD_Stamping.GetCylinderActualPosition(StampingCylinderType.QRStamping, DirectionsEnum.Down, out bool QRStamping_IsDown))
                                    Cylinder_QRStamping_IsDown = QRStamping_IsDown;
                                if (GD_Stamping.GetCylinderActualPosition(StampingCylinderType.StampingSeat, DirectionsEnum.Up, out bool StampingSeat_IsUp))
                                    Cylinder_StampingSeat_IsUp = StampingSeat_IsUp;
                                if (GD_Stamping.GetCylinderActualPosition(StampingCylinderType.StampingSeat, DirectionsEnum.Down, out bool StampingSeat_IsDown))
                                    Cylinder_StampingSeat_IsDown = StampingSeat_IsDown;
                                if (GD_Stamping.GetCylinderActualPosition(StampingCylinderType.BlockingCylinder, DirectionsEnum.Up, out bool BlockingCylinder_IsUp))
                                    Cylinder_BlockingCylinder_IsUp = BlockingCylinder_IsUp;
                                if (GD_Stamping.GetCylinderActualPosition(StampingCylinderType.BlockingCylinder, DirectionsEnum.Down, out bool BlockingCylindere_IsDown))
                                    Cylinder_BlockingCylindere_IsDown = BlockingCylindere_IsDown;
                                if (GD_Stamping.GetCylinderActualPosition(StampingCylinderType.HydraulicCutting, DirectionsEnum.Up, out bool HydraulicCutting_IsUp))
                                    Cylinder_HydraulicCutting_IsUp = HydraulicCutting_IsUp;
                                if (GD_Stamping.GetCylinderActualPosition(StampingCylinderType.HydraulicCutting, DirectionsEnum.Down, out bool HydraulicCutting_IsDown))
                                    Cylinder_HydraulicCutting_IsDown = HydraulicCutting_IsDown;

                                if (GD_Stamping.GetHydraulicPumpMotor(out bool HPumpIsActive))
                                    HydraulicPumpIsActive = HPumpIsActive;

                                if (GD_Stamping.GetIronPlateName(GD_Stamping_Opcua.StampingOpcUANode.sIronPlate.sIronPlateName1, out string sIronPlateString1))
                                    IronPlateName1 = sIronPlateString1; 
                                if (GD_Stamping.GetIronPlateName(GD_Stamping_Opcua.StampingOpcUANode.sIronPlate.sIronPlateName2, out string sIronPlateString2))
                                    IronPlateName2 = sIronPlateString2;
                                if (GD_Stamping.GetIronPlateName(GD_Stamping_Opcua.StampingOpcUANode.sIronPlate.sIronPlateName3, out string sIronPlateString3))
                                    IronPlateName3 = sIronPlateString3;

                                if (GD_Stamping.GetSeparateBoxNumber(out int boxIndex))
                                {
                                    SeparateBoxIndex = boxIndex;
                                }



                                //取得io資料表
                                if (GD_Stamping is GD_Stamping_Opcua GD_StampingOpcua)
                                {
                                    foreach (var IO_Table in IO_TableObservableCollection)
                                    {
                                   
                                        if (!string.IsNullOrEmpty(IO_Table.NodeID))
                                        {
                                            if (IO_Table.ValueType == typeof(bool) && GD_StampingOpcua.ReadNode(IO_Table.NodeID, out bool nodeboolValue))
                                            {
                                                IO_Table.IO_Value = nodeboolValue;
                                            }
                                            else if (IO_Table.ValueType == typeof( float) &&  GD_StampingOpcua.ReadNode(IO_Table.NodeID, out float nodefloatValue))
                                            {
                                                IO_Table.IO_Value = nodefloatValue;
                                            }
                                            else if (IO_Table.ValueType is object && GD_StampingOpcua.ReadNode(IO_Table.NodeID, out object nodeobjectValue))
                                            {
                                                IO_Table.IO_Value = nodeobjectValue;
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
                                foreach (var IO_Table in IO_TableObservableCollection)
                                {
                                    IO_Table.IO_Value = null;
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            Debugger.Break();
                        }
                        finally 
                        {
                            GD_Stamping.Disconnect();
                        }

                        await Task.Delay(100);

                    } while (ContinueScanning);
                    IsScaning = false;
                });
            }
        }
        public async void StopScan()
        {
            if (!IsScaning)
                return;

            await Task.Run(async () => 
            {
                ContinueScanning = false;
               //等待掃描解除
                while(IsScaning)
                {
                    await Task.Delay(100);
                }

                //回復狀態
                ContinueScanning = true;
            });
            return;
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
        public ICommand Feeding_XAxis_ReturnToStandbyPosition
        {
            get => new RelayCommand(() =>
            {
                if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
                {
                    GD_Stamping.FeedingPositionReturnToStandbyPosition();
                    GD_Stamping.Disconnect();
                }
            });
        }

        /// <summary>
        /// 進料X軸移動
        /// </summary>
        public ICommand Feeding_XAxis_MoveCommand
        {
            get => new RelayParameterizedCommand(async Parameter => 
            {
                if(Parameter != null)
                {
                    if(float.TryParse(Parameter.ToString(), out var ParameterValue))
                    {
                        if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
                        {
                            if (GD_Stamping.GetFeedingPosition(out var FPosition))
                            {

                                GD_Stamping.SetFeedingPosition(FPosition + ParameterValue);



                                await Task.Delay(2000);

                                GD_Stamping.GetFeedingPosition(out var FPosition2);
                                var value = FPosition2 - FPosition;


                                GD_Stamping.Disconnect(); 
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 雙導桿缸-可動端 壓座控制 夾緊/放鬆
        /// </summary>
        public ICommand CylinderControl_Up_Command
        {
            get => new RelayParameterizedCommand(para =>
            {
                Task.Run(() =>
                {
                    if (para is StampingCylinderType stampingCylinder)
                    {

                        if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
                        {
                            if(GD_Stamping.Set_IO_CylinderControl(stampingCylinder, DirectionsEnum.Up))
                            {
                                Singletons.LogDataSingleton.Instance.AddLogData(DataSingletonName ,$"{stampingCylinder} " );
                            }
                            GD_Stamping.Disconnect();
                        }
                    }
                });
            });
        }


        public ICommand CylinderControl_Middle_Command
        {
            get => new RelayParameterizedCommand(para =>
            {
                Task.Run(() =>
                {
                    if (para is StampingCylinderType stampingCylinder)
                    {
                        if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
                        {
                            GD_Stamping.Set_IO_CylinderControl(stampingCylinder, DirectionsEnum.Middle);
                            GD_Stamping.Disconnect();
                        }
                    }
                });
            });
        }

        /// <summary>
        /// 雙導桿缸-固定端 壓座控制 夾緊/放鬆
        /// </summary>
        public ICommand CylinderControl_Down_Command
        {
            get => new RelayParameterizedCommand(para =>
            {
                Task.Run(() =>
                {
                    if (para is StampingCylinderType stampingCylinder)
                    {
                        Set_CylinderControl(stampingCylinder, DirectionsEnum.Down);
                    }
                });
            });
        }

        public ICommand ActiveHydraulicPumpMotor
        {
            get => new RelayCommand(() =>
            {
                Task.Run(() =>
                {
                    if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
                    {
                        if(GD_Stamping.GetHydraulicPumpMotor(out var isActive))
                            GD_Stamping.SetHydraulicPumpMotor(!isActive);
                        GD_Stamping.Disconnect();
                    }

                });
            });
        }

        public void SendStampingString()
        {
            if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
            {
                GD_Stamping.SetIronPlateName(GD_Stamping_Opcua.StampingOpcUANode.sIronPlate.sIronPlateName1 ,"DEF");
                GD_Stamping.Disconnect();
            }
        }


        private void Set_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum Direction)
        {
            if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
            {
                GD_Stamping.Set_IO_CylinderControl(stampingCylinder, Direction);
                GD_Stamping.Disconnect();
            }
        }


        public bool SetSeparateBoxNumber(int Index)
        {
            var ret = false;
            if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
            {
                ret = GD_Stamping.SetSeparateBoxNumber(Index);
                GD_Stamping.Disconnect();
            }
            return ret;
        }


        public ICommand EngravingYAxisFwdCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                Task.Run(() =>
                {
                    if (obj is bool IsActived)
                        SetEngravingYAxisFwd(IsActived);
                });
            });
        }

        public ICommand EngravingYAxisToStandbyPosCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                Task.Run(() =>
                {
                    if (obj is bool IsActived)
                       SetEngravingYAxisToStandbyPos();
                });
            });
        }
        public ICommand EngravingYAxisBwdCommand
        {
            get => new RelayParameterizedCommand(obj =>
            {
                Task.Run(() =>
                {
                    if (obj is bool IsActived)
                        SetEngravingYAxisBwd(IsActived);
                });

            });
        }


        /// <summary>
        /// 鋼印Y軸回歸原點
        /// </summary>
        /// <returns></returns>
        public bool SetEngravingYAxisToStandbyPos()
        {
            var ret = false;
            if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
            {
                ret = GD_Stamping.SetEngravingYAxisBwd(false);
                ret = GD_Stamping.SetEngravingYAxisFwd(false);
                ret = GD_Stamping.SetEngravingYAxisToStandbyPos();
                GD_Stamping.Disconnect();
            }
            return ret;
        }


        /// <summary>
        /// 鋼印Y軸前進
        /// </summary>
        /// <returns></returns>
        public bool SetEngravingYAxisFwd(bool IsMove)
        {
            var ret = false;
            if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
            {
                ret = GD_Stamping.SetEngravingYAxisBwd(false);
                ret = GD_Stamping.SetEngravingYAxisFwd(IsMove);
                GD_Stamping.Disconnect();
            }
            return ret;
        }
        /// <summary>
        /// 鋼印Y軸後退
        /// </summary>
        /// <returns></returns>
        public bool SetEngravingYAxisBwd(bool IsMove)
        {
            var ret = false;
            if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
            {
                ret = GD_Stamping.SetEngravingYAxisFwd(false);
                ret = GD_Stamping.SetEngravingYAxisBwd(IsMove);
                GD_Stamping.Disconnect();
            }
            return ret;
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

        public string _ironPlateName1;
        public string _ironPlateName2;
        public string _ironPlateName3;

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
        public string IronPlateName3
        {
            get => _ironPlateName3; set { _ironPlateName3 = value; OnPropertyChanged(); }
        }

        private int _separateBoxIndex = -1;

        /// <summary>
        /// 分料盒編號
        /// </summary>
        public int SeparateBoxIndex
        {
            get => _separateBoxIndex; set { _separateBoxIndex = value; OnPropertyChanged(); }
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
