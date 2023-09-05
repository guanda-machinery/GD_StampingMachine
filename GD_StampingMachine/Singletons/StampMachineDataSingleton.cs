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
                io_info_Table = new ObservableCollection<IO_InfoModel>();
                io_info_Table.Add(new IO_InfoModel()
                {
                    Info = "氣壓總壓檢知(7kg/cm3 up)",
                    BondingCableTerminal = "I00",
                    KEBA_Definition = "DI_00",
                    SensorType = ioSensorType.DI,
                    NodeID = $"{opcuaNodeHeader}.system.di_AirPressNotEnough",
                });
                io_info_Table.Add(new IO_InfoModel()
                {
                    Info = "預留",
                    BondingCableTerminal = "I01",
                    KEBA_Definition = "DI_01",
                    SensorType = ioSensorType.DI,
                });
                io_info_Table.Add(new IO_InfoModel()
                {
                    Info = "油壓單元液位檢知(低液位)",
                    BondingCableTerminal = "I02",
                    KEBA_Definition = "DI_02",
                    SensorType = ioSensorType.DI,
                    NodeID = $"{opcuaNodeHeader}.OilMaintenance1.di_OilLevelOk"
                });
                io_info_Table.Add(new IO_InfoModel()
                {
                    Info = "潤滑壓力檢知",
                    BondingCableTerminal = "I03",
                    KEBA_Definition = "DI_03",
                    SensorType = ioSensorType.DI,
                    NodeID = $"{opcuaNodeHeader}.Lubrication1.di_LubPressureAchieved"
                });
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
                        IStampingMachineConnect GD_Stamping = new GD_Stamping_Opcua();
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


                                //取得io資料表
                                if (GD_Stamping is GD_Stamping_Opcua GD_StampingOpcua)
                                {
                                    foreach (var IO_Table in IO_TableObservableCollection)
                                    {
                                        if (!string.IsNullOrEmpty(IO_Table.NodeID))
                                        {
                                            if (GD_StampingOpcua.ReadNode(IO_Table.NodeID, out bool nodeValue))
                                            {
                                                IO_Table.IO_Value = nodeValue.TryConvertToDouble();
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
                IStampingMachineConnect GD_Stamping = new GD_Stamping_Opcua();
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
                        IStampingMachineConnect GD_Stamping = new GD_Stamping_Opcua();
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
                        IStampingMachineConnect GD_Stamping = new GD_Stamping_Opcua();

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
                        IStampingMachineConnect GD_Stamping = new GD_Stamping_Opcua();
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
                    IStampingMachineConnect GD_Stamping = new GD_Stamping_Opcua();
                    if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
                    {
                        if(GD_Stamping.GetHydraulicPumpMotor(out var isActive))
                            GD_Stamping.SetHydraulicPumpMotor(!isActive);
                        GD_Stamping.Disconnect();
                    }

                });
            });
        }





        private void Set_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum Direction)
        {
            IStampingMachineConnect GD_Stamping = new GD_Stamping_Opcua();
            if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, OpcUASetting.UserName, OpcUASetting.Password))
            {
                GD_Stamping.Set_IO_CylinderControl(stampingCylinder, Direction);
                GD_Stamping.Disconnect();
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
