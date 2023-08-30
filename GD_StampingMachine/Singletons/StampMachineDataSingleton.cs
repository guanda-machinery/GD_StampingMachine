using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Scheduling.Themes;
using GD_CommonLibrary.Method;
using GD_MachineConnect;
using GD_MachineConnect.Machine;
using GD_MachineConnect.Machine.Interfaces;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GD_StampingMachine.Singletons
{
    public class StampMachineDataSingleton : GD_CommonLibrary.BaseSingleton<StampMachineDataSingleton>, INotifyPropertyChanged
    {
        /*private string OpcUASettingFilePath
        {
            get => Method.StampingMachineJsonHelper.GetJsonFilePath(Method.StampingMachineJsonHelper.MachineSettingNameEnum.OpcUASetting);
        }*/

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
        public bool IsScaning { get=> _isScaning; set { _isScaning = value; OnPropertyChanged(); } }
        public void ScanOpcua()
        {
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
                            if (GD_Stamping.Connect(OpcUASetting.HostString, OpcUASetting.Port.Value, "ns=4;s=APPL", OpcUASetting.UserName, OpcUASetting.Password))
                            {
                                //進料馬達
                                if (GD_Stamping.GetFeedingPosition(out var fPos))
                                    FeedingPosition = fPos;





                                
                            }
                        }
                        catch (Exception ex)
                        {

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
            return;
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

        















        private float _feedingPosition;
        /// <summary>
        /// 馬達目前位置
        /// </summary>
        public float FeedingPosition
        {
            get => _feedingPosition; set { _feedingPosition = value; OnPropertyChanged(); }
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
