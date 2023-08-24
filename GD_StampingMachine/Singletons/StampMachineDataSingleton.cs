using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Scheduling.Themes;
using GD_CommonLibrary.Method;
using GD_MachineConnect;
using GD_MachineConnect.Machine;
using GD_StampingMachine.GD_Model;
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
        private string OpcUASettingFilePath
        {
            get => Method.StampingMachineJsonHelper.GetCurrentMachineSettingDirectory("OpcUASetting.json");
        }

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
            var JsonHM = new JsonHelperMethod();
            if(JsonHM.ReadJsonFile(OpcUASettingFilePath, out OpcUASettingModel OpcUASettingJson))
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
                JsonHM.WriteJsonFile(OpcUASettingFilePath, OpcUASetting);
            }

            gd_OpcUaHelperClient = new();
            if(!string.IsNullOrEmpty(OpcUASetting.UserName) && !string.IsNullOrEmpty(OpcUASetting.Password))
                gd_OpcUaHelperClient.UserIdentity = new UserIdentity(OpcUASetting.UserName, OpcUASetting.Password);
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

        private GD_OpcUaHelperClient gd_OpcUaHelperClient;
            //啟用掃描
        public async Task<bool> TestConnectToOpcua()
        {
            if (await gd_OpcUaHelperClient.OpcuaConnectAsync(OpcUASetting.HostString, OpcUASetting.Port, OpcUASetting.ServerDataPath))
            {
                gd_OpcUaHelperClient.ReadAllReference();
                gd_OpcUaHelperClient.Disconnect();
                return true;
            }
            return false;
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
                        try
                        {
                            if (await gd_OpcUaHelperClient.OpcuaConnectAsync(OpcUASetting.HostString, OpcUASetting.Port, OpcUASetting.ServerDataPath))
                            {
                                var a = gd_OpcUaHelperClient.ReadAllReference();
                                gd_OpcUaHelperClient.Disconnect();
                            }
                        }
                        catch (Exception ex)
                        {

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
        public async void WriteOpcua()
        {

            if (await gd_OpcUaHelperClient.OpcuaConnectAsync(OpcUASetting.HostString, OpcUASetting.Port, OpcUASetting.ServerDataPath))
            {
                var a = gd_OpcUaHelperClient.ReadAllReference();
                gd_OpcUaHelperClient.Disconnect();
            }
            return;
        }









    }
}
